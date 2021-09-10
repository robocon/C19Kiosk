﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThaiNationalIDCard;

namespace C19Kiosk
{
    public partial class Form1 : Form
    {
        static readonly SmConfigure smConfig = new SmConfigure();
        static readonly HttpClient client = new HttpClient();
        private ThaiIDCard idcard;
        public Personal person;

        // ตัวจับเวลา scan qr code
        private static System.Timers.Timer aTimer;

        public Form1()
        {
            InitializeComponent();
        }

        string[] cardReaders;
        private void Form1_Load(object sender, EventArgs e)
        {
            var date1 = new DateTime(1949, 1, 1, 0, 0, 0);
            //Console.WriteLine(date1.ToString());
            //paymentNoti.Text = date1.ToString();

            DateTime paymentDate = new DateTime(2021,04,27);
            DateTime datetimeNow = DateTime.Now;
            TimeSpan dateDiff = datetimeNow.Subtract(paymentDate);
            //paymentNoti.Text = "ผ่านมาแล้ว "+dateDiff.Days.ToString()+"วัน ที่โปรแกรมนี้ไม่ได้จ่ายเงิน";
            //paymentNoti.Text = "พัฒนาระบบโดย นายกฤษณะศักดิ์ กันธรส ติดต่อโทร. 06-1318-1864";

            this.KeyPreview = true; //ตั้งค่าเอาไว้ให้มัน focus ที่ key event
            // ฟอร์มโหลด แล้วตั้งเวลาหน่วงไว้ 5วิ
            aTimer = new System.Timers.Timer(5000);
            aTimer.Elapsed += TimerElapsed;

            try
            {
                idcard = new ThaiIDCard();
                cardReaders = idcard.GetReaders();
                idcard.MonitorStart(cardReaders[0].ToString());
                idcard.eventCardInserted += new handleCardInserted(CardInsertedCallback);
                //idcard.eventCardRemoved += new handleCardRemoved(CardRemoveCallback);
            }
            catch (Exception ex)
            {
                label1SetText("ไม่พบเครื่องอ่านบัตร Smart Card");
                Console.WriteLine(ex.Message);
            }
            
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            Console.WriteLine("TimerElapsed: TIME STOPPPPPPP");
            aTimer.Enabled = false;
            testGetKeyChar = "";
        }

        private async void CardInsertedCallback(Personal personal)
        {
            Console.WriteLine("card was inserted");
            label1SetText("กำลังอ่านข้อมูลบัตรประชาชน กรุณารอสักครู่...");

            // ดึงค่าจากบัตรประชาชน
            var person = await RunCardReadder();
            if (person == null)
            {
                label1SetText("ไม่สามารถอ่านข้อมูลบัตรประชาชนได้ กรุณาเสียบบัตรใหม่อีกครั้ง\nถ้าทำซ้ำแล้วยังใช้งานไม่ได้ท่านสามารถ\n1. กดเลขบัตรประชาชนหรือHNที่ช่องทางด้านซ้ายมือ\n2. กดปุ่มลงทะเบียน");
            }
            else
            {
                label1SetText("ระบบกำลังตรวจสอบหมายเลขบัตรประชาชน\nกับข้อมูลของโรงพยาบาล กรุณารอสักครู่...");
                // ตัวเดิม smConfig.searchOpcardUrl
                string searchByIdcard = "http://localhost/smbroker/searchOpcardByIdcard.php";
                string idcard = person.Citizenid;

                // คำนวณอายุ
                int enDay = int.Parse(person.Birthday.ToString("dd"));
                int enMonth = int.Parse(person.Birthday.ToString("MM"));
                int enYear = int.Parse(person.Birthday.ToString("yyyy")) - 543;
                /*string birthDayEn = person.Birthday.ToString("dd/MM") + "/" + enYear.ToString();*/

                DateTime dateOfBirth = new DateTime(enYear, enMonth, enDay);
                CalculateAge(dateOfBirth, out enYear, out enMonth, out enDay);
                if (enYear < 18)
                {
                    string monthAndDay = enYear + "ปี ";
                    if (enMonth > 0)
                    {
                        monthAndDay = monthAndDay + enMonth + "เดือน ";
                    }

                    if (enDay > 0)
                    {
                        monthAndDay = monthAndDay + enDay + "วัน";
                    }

                    var confirmResult = MessageBox.Show("คุณ" + person.Th_Firstname + " " + person.Th_Lastname + " อายุไม่ถึง 18ปีบริบูรณ์\nขณะนี้ท่านกำลังอายุ " + monthAndDay + "\n\nกด Yes ถ้าต้องการดำเนินการต่อ\nกด No ถ้าต้องการยกเลิก",
                                     "แจ้งเตือนอายุไม่ถึง",
                                     MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.No)
                    {
                        label1SetText("");
                        return;
                    }
                }


                Console.WriteLine(searchByIdcard);
                Console.WriteLine(idcard);
                // ค้นหาHNจากเลขบัตรประชาชน
                string testOpcard = await Task.Run(() => searchFromSmByIdcard(searchByIdcard, idcard));
                if (!string.IsNullOrEmpty(testOpcard))
                {
                    resOpcardByIdcard resultOpcard;
                    try
                    {
                        resultOpcard = JsonConvert.DeserializeObject<resOpcardByIdcard>(testOpcard);
                        if (resultOpcard.statusCode == 200)
                        {
                            string content = await Task.Run(() => saveVn(resultOpcard.hn));
                            if (!String.IsNullOrEmpty(content))
                            {
                                responseSaveVn app = JsonConvert.DeserializeObject<responseSaveVn>(content);
                                EpsonSlip es = new EpsonSlip();
                                es.printOutSlip(app);
                                textBox1.BeginInvoke(new MethodInvoker(delegate { textBox1.Text = ""; }));
                            }
                            label1SetText("");
                        }
                        else // ถ้าไม่พบ HN
                        {
                            //
                            if(resultOpcard.errorType=="alert")
                            {
                                label1SetText(resultOpcard.errorMsg);
                            }
                            else
                            {
                                SelectedCreateOpcard frm = new SelectedCreateOpcard();
                                frm.notifyOpcard = resultOpcard.errorMsg;
                                frm.person = person;
                                frm.ShowDialog();
                                label1SetText("");
                            }
                        }
                    }
                    catch(JsonReaderException ex)
                    {
                        label1SetText(ex.Message);
                    }

                } // end if is null
            } // else idcard is not null
        }

        // Run Card Reader
        public async Task<Personal> RunCardReadder()
        {
            Console.WriteLine("get data from cardreader");
            var person = await Task.Run(() => GetPersonalCardreader());
            return person;
        }

        // ดึงข้อมูลบัตรประชาชน
        public Personal GetPersonalCardreader()
        {
            idcard = new ThaiIDCard();
            Personal person = idcard.readAllPhoto();
            return person;
        }

        // แสดงข้อความแจ้งเตือนกรณีเสียบบัตร
        public void label1SetText(string label1Text)
        {
            responseText.BeginInvoke(new MethodInvoker(delegate { responseText.Text = label1Text; }));
        }

        // ตอนกดปุ่มตัวเลข
        public void ButtonAddIdcard_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            textBox1.Text = textBox1.Text + button.Text;

            label1.Focus();

        }

        // ตอนกดปุ่มลบ
        private void button11_Click(object sender, EventArgs e)
        {
            var tl = textBox1.Text.Length;
            if (tl > 0)
            {
                textBox1.Text = textBox1.Text.Substring(0, tl - 1);
            }
            //textBox1.Focus();
            label1.Focus();
        }

        // Clear Text
        private void button13_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            //textBox1.Focus();
            label1.Focus();
        }

        // Submit จากการกดเลขบัตรหรือHN
        public string submitHn;
        private async void submitBtn_Click(object sender, EventArgs e)
        {
            string testHnOrIdcard = textBox1.Text.Trim();
            responseText.Text = "";
            label1.Focus();

            submitBtn.Enabled = false;

            // ถ้าเป็น hn จะมีขีดกลาง
            if (Regex.IsMatch(testHnOrIdcard, @"\d+\-\d+", RegexOptions.IgnoreCase))
            {
                submitHn = testHnOrIdcard;
            }
            else
            {
                Console.WriteLine($"Manual ค้นหาจาก idcard {smConfig.searchOpcardUrl}");
                if (testHnOrIdcard.Length != 13)
                {
                    responseText.Text = "หมายเลขบัตรประชาชนไม่ครบ13หลัก\nกรุณาตรวจสอบหมายเลขบัตรของท่านอีกครั้ง";
                    submitBtn.Enabled = true;
                    return;
                }

                string testOpcard = await Task.Run(() => searchFromSmByIdcard(smConfig.searchOpcardUrl, testHnOrIdcard));
                Console.WriteLine(testOpcard);
                if (!string.IsNullOrEmpty(testOpcard))
                {
                    responseOpcard resultOpcard = JsonConvert.DeserializeObject<responseOpcard>(testOpcard);
                    if (string.IsNullOrEmpty(resultOpcard.errorMsg))
                    {
                        submitHn = resultOpcard.hn;
                    }
                    else
                    {
                        responseText.Text = resultOpcard.errorMsg;
                    }
                }
            }
            testLoadData(submitHn);
            submitBtn.Enabled = true;
        }

        public async void testLoadData(string hn)
        {
            string testOpcard = "";
            if (Regex.IsMatch(hn, "-", RegexOptions.IgnoreCase))
            {
                // ตรวจสอบ HN
                testOpcard = await Task.Run(() => searchFromSmByHn(smConfig.searchOpcardUrl, hn));
            }
            else
            {
                testOpcard = await Task.Run(() => searchFromSmByIdcard(smConfig.searchOpcardUrl, hn));
            }

            if (!string.IsNullOrEmpty(testOpcard))
            {
                
                responseOpcard resultOpcard = JsonConvert.DeserializeObject<responseOpcard>(testOpcard);
                if (string.IsNullOrEmpty(resultOpcard.errorMsg))
                {
                    
                    // คำนวณอายุ
                    int enDay = int.Parse(Convert.ToDateTime(resultOpcard.dob).ToString("dd"));
                    int enMonth = int.Parse(Convert.ToDateTime(resultOpcard.dob).ToString("MM"));
                    int enYear = int.Parse(Convert.ToDateTime(resultOpcard.dob).ToString("yyyy")) - 543;
                    /*string birthDayEn = person.Birthday.ToString("dd/MM") + "/" + enYear.ToString();*/

                    enDay = 01;
                    enMonth = 01;
                    enYear = 2006;

                    DateTime dateOfBirth = new DateTime(enYear, enMonth, enDay);
                    CalculateAge(dateOfBirth, out enYear, out enMonth, out enDay);
                    if (enYear < 18)
                    {
                        string monthAndDay = enYear + "ปี ";
                        if (enMonth > 0)
                        {
                            monthAndDay = monthAndDay + enMonth + "เดือน ";
                        }

                        if (enDay > 0)
                        {
                            monthAndDay = monthAndDay + enDay + "วัน";
                        }

                        var confirmResult = MessageBox.Show("คุณ" + resultOpcard.ptname + " อายุไม่ถึง 18ปีบริบูรณ์\nขณะนี้ท่านกำลังอายุ " + monthAndDay + "\n\nกด Yes ถ้าต้องการดำเนินการต่อ\nกด No ถ้าต้องการยกเลิก",
                                         "แจ้งเตือนอายุไม่ถึง",
                                         MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.No)
                        {
                            label1SetText("");
                            return;
                        }
                    }

                    string content = await Task.Run(() => saveVn(resultOpcard.hn));
                    if (!String.IsNullOrEmpty(content))
                    {
                        responseSaveVn app = JsonConvert.DeserializeObject<responseSaveVn>(content);
                        EpsonSlip es = new EpsonSlip();
                        es.printOutSlip(app);

                        textBox1.Text = "";
                    }
                }
                else // ถ้าไม่พบ HN
                {
                    responseText.Text = $"Error : {resultOpcard.errorMsg}";
                }
            }
            else
            {
                responseText.Text = $"Error : {smConfig.searchOpcardUrl}";
            }
        }

        /**
         * ตรวจสอบข้อมูลจาก HN
         */
        static async Task<string> searchFromSmByHn(string posturi, string hn)
        {
            string content = null;
            try
            {
                sendSearchOpCard appoint = new sendSearchOpCard();
                appoint.hn = hn;

                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsJsonAsync(posturi, appoint);
                response.EnsureSuccessStatusCode();
                content = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show(e.Message);
            }

            return content;
        }

        static async Task<string> searchFromSmByIdcard(string posturi, string idcard)
        {
            string content = null;
            try
            {
                sendSearchOpCard appoint = new sendSearchOpCard();
                appoint.Idcard = idcard;

                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsJsonAsync(posturi, appoint);
                response.EnsureSuccessStatusCode();
                content = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show(e.Message);
            }

            return content;
        }

        /**
         * ออก VN
         */
        public async Task<string> saveVn(string hn)
        {
            string content = null;
            try
            {
                saveVn savevn = new saveVn();
                savevn.hn = hn;

                Console.WriteLine(smConfig.createVnUrl);

                var response = await client.PostAsJsonAsync(smConfig.createVnUrl, savevn);
                response.EnsureSuccessStatusCode();
                content = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {

                responseText.Text = $"Message :{ex.Message}";
            }
            return content;
        }

        private string testGetKeyChar = "";
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            testGetKeyChar += e.KeyChar;
            if (e.KeyChar == (char)13)
            {
                label1SetText("กำลังลงทะเบียนด้วย QR Code กรุณารอสักครู่...");
                System.Threading.Thread.Sleep(500);
                Console.WriteLine("Form1_KeyPress : " + testGetKeyChar);
                if (!aTimer.Enabled)
                {
                    aTimer.Enabled = true;

                    string hn = testGetKeyChar.Trim();

                    /*if (!Regex.IsMatch(hn, @"\d+\-\d+", RegexOptions.IgnoreCase))
                    {
                        label1SetText("กรุณาใช้ QR Code ที่เป็น HN โรงพยาบาลเท่านั้น");
                        return;
                    }*/
                    
                    testLoadData(hn);
                    label1SetText("");
                    // ล้างค่า
                    textBox1.Text = hn = testGetKeyChar = "";
                    

                }
                else
                {
                    label1SetText("มีการแสกนบาร์โค้ดเร็วเกินไป กรุณารอสักครู่...");
                    return;
                }
            } // End enter from barcode
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            label1.Focus();
        }

        ///
        /// Calculate the Age of a person given the birthdate.
        /// https://raasukutty.wordpress.com/2009/06/18/c-calculate-age-in-years-month-and-days/
        ///
        static void CalculateAge(DateTime adtDateOfBirth, out int aintNoOfYears, out int aintNoOfMonths, out int aintNoOfDays)
        {
            // get current date.
            DateTime adtCurrentDate = DateTime.Now;

            // find the literal difference
            aintNoOfDays = adtCurrentDate.Day - adtDateOfBirth.Day;
            aintNoOfMonths = adtCurrentDate.Month - adtDateOfBirth.Month;
            aintNoOfYears = adtCurrentDate.Year - adtDateOfBirth.Year;

            if (aintNoOfDays < 0)
            {
                aintNoOfDays += DateTime.DaysInMonth(adtCurrentDate.Year, adtCurrentDate.Month);
                aintNoOfMonths--;
            }

            if (aintNoOfMonths < 0)
            {
                aintNoOfMonths += 12;
                aintNoOfYears--;
            }
        }
    }

    public class MyIdcard
    {
        public string En_Firstname { get; set; }
        public string En_Prefix { get; set; }
        public string Th_Lastname { get; set; }
        public string Th_Firstname { get; set; }
        public string Th_Prefix { get; set; }
        public string Sex { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime Expire { get; set; }
        public DateTime Issue { get; set; }
        public string En_Lastname { get; set; }
        public string addrProvince { get; set; }
        public string addrTambol { get; set; }
        public string addrRoad { get; set; }
        public string addrLane { get; set; }
        public string addrVillageNo { get; set; }
        public string addrHouseNo { get; set; }
        public string Address { get; set; }
        public string PhotoRaw { get; set; }
        public string Citizenid { get; set; }
        public string addrAmphur { get; set; }
    }

    public class MyIdcardResponse
    {
        public int messageCode { get; set; }
        public string hn { get; set; }
    }

    public class sendSearchOpCard
    {
        public string Idcard { get; set; }
        public string hn { get; set; }
    }
    public class responseOpcard
    {
        public string opcardStatus { set; get; }
        public string idcard { set; get; }
        public string hn { set; get; }
        public string ptname { set; get; }
        public string hosPtRight { set; get; }

        public string PtRightMain { set; get; }
        public string PtRightSub { set; get; }
        public string errorMsg { set; get; }
        public string dob { set; get; }
    }

    public class resOpcardByIdcard : responseOpcard
    {
        public int statusCode { set; get; }
        public string errorType { set; get; }
    }

    public class saveVn
    {
        public string hn { set; get; }
    }

    public class responseSaveVn
    {
        public string appointStatus { set; get; }
        public string dateSave { set; get; }
        public string ex { set; get; }
        public string vn { set; get; }
        public string ptname { set; get; }
        public string hn { set; get; }
        public string age { set; get; }
        public string mx { set; get; }
        public string ptright { set; get; }
        public string idcard { set; get; }
        public string hospCode { set; get; }
        public string doctor { set; get; }
        public string room { set; get; }
        public string queueStatus { set; get; }
        public string ptType { set; get; }
        public string queueNumber { set; get; }
        public int queueWait { set; get; }
        public string queueRoom { set; get; }
        public string runNumber { set; get; }
        public string fakeQueue { set; get; }
        public string queue_vaccine { set; get; }
    }

    public class savePhoto
    {
        public string rawPhoto { set; get; }
        public string idCard { set; get; }
    }

    public class responseSavePhoto
    {
        public string saveStatus { set; get; }
    }
}
