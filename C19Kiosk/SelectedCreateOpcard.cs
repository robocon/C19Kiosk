using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThaiNationalIDCard;

namespace C19Kiosk
{
    public partial class SelectedCreateOpcard : Form
    {
        public string notifyOpcard;
        public Personal person;
        public DateTime newBirthday;
        static readonly HttpClient client = new HttpClient();
        static readonly SmConfigure smConfig = new SmConfigure();

        public SelectedCreateOpcard()
        {
            InitializeComponent();
        }

        private void SelectedCreateOpcard_Load(object sender, EventArgs e)
        {
            label1.Text = notifyOpcard;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void confirmBtn_Click(object sender, EventArgs e)
        {
            try
            {
                confirmBtn.Enabled = false;
                label1.Text = "ระบบกำลังดำเนินการสร้างHN กรุณารอสักครู่...";

                MyIdcard m = new MyIdcard();
                m.Citizenid = person.Citizenid;
                m.Th_Prefix = person.Th_Prefix;
                m.Th_Firstname = person.Th_Firstname;
                m.Th_Lastname = person.Th_Lastname;
                m.En_Prefix = person.En_Prefix;
                m.En_Firstname = person.En_Firstname;
                m.En_Lastname = person.En_Lastname;
                m.Birthday = person.Birthday;
                m.Sex = person.Sex;
                m.newBirthday = newBirthday;
                m.Address = person.Address;
                m.addrHouseNo = person.addrHouseNo;
                m.addrLane = person.addrLane;
                m.addrProvince = person.addrProvince;
                m.addrAmphur = person.addrAmphur;
                m.addrTambol = person.addrTambol;
                m.addrRoad = person.addrRoad;
                m.addrVillageNo = person.addrVillageNo;

                m.Expire = person.Expire;
                m.Issue = person.Issue;

                Bitmap Photo1 = new Bitmap(person.PhotoBitmap, new Size(207, 248));
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                Photo1.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                await Task.Run(async () => {

                    savePhoto pho = new savePhoto();
                    pho.rawPhoto = Convert.ToBase64String(stream.ToArray());
                    pho.idCard = person.Citizenid;
                    Console.WriteLine("Status : save photo from idcard");
                    try
                    {
                        HttpClient httpClient = new HttpClient();
                        var response = await httpClient.PostAsJsonAsync("http://192.168.131.250/sm3/save_photo.php", pho);
                        response.EnsureSuccessStatusCode();
                        string savePhoto = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(savePhoto);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                });

                string cont = "";
                cont = await Task.Run(async () => {

                    var response = await client.PostAsJsonAsync("http://localhost/smbroker/saveOpcard.php", m);
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(content);
                    return content;

                });

                if (!string.IsNullOrEmpty(cont))
                {
                    resOpcardByIdcard opById = JsonConvert.DeserializeObject<resOpcardByIdcard>(cont);
                    if(opById.statusCode == 200)
                    {
                        string saveVnContent = await Task.Run(() => saveVn(opById.hn));
                        if (!String.IsNullOrEmpty(saveVnContent))
                        {
                            responseSaveVn app = JsonConvert.DeserializeObject<responseSaveVn>(saveVnContent);
                            EpsonSlip es = new EpsonSlip();
                            es.printOutSlip(app);
                            confirmBtn.Enabled = true;
                            label1.Text = "";
                            this.Close();
                        }
                    }
                }
                Console.WriteLine(cont);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //label1SetText(ex.Message);
            }
        }

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
                Console.WriteLine(ex.Message);
                // responseText.Text = $"Message :{ex.Message}";
            }
            return content;
        }
    }
}
