using System;
using System.Windows.Forms;

namespace C19Kiosk
{
    public partial class FormSelectDate : Form
    {
        public string daySelected = "";
        public string monthSelected = "";
        public string yearSelected = "";

        public FormSelectDate()
        {
            InitializeComponent();
        }

        private void FormSelectDate_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

            DateTime preDate = DateTime.Now;
            var currentYear = Int32.Parse(preDate.ToString("yyyy"));
            var prevYear = currentYear - 120;
            //comboBoxYear.DisplayMember = "Text";
            //comboBoxYear.ValueMember = "Value";
            //List<Object> yearLists = new List<Object>();
            comboBoxYear.Items.Add("เลือกปี");
            while (currentYear >= prevYear)
            {
                // Text is show to display
                // Value is data behide
                //yearLists.Add(new { Text = currentYear, Value = currentYear });
                comboBoxYear.Items.Add(currentYear);
                currentYear--;
            }
            //comboBoxYear.DataSource = yearLists;
            comboBoxYear.SelectedIndex = 0;

            // 

            comboBoxMonth.Items.Add("เลือกเดือน");
            comboBoxMonth.SelectedIndex = 0;
            /*comboBoxMonth.DisplayMember = "Text";
            comboBoxMonth.ValueMember = "Value";
            var items = new[] {
                new { Text = "เลือกเดือน", Value = "0" },
                new { Text = "มกราคม", Value = "1" },
                new { Text = "กุมภาพันธ์", Value = "2" },
                new { Text = "มีนาคม", Value = "3" },
                new { Text = "เมษายน", Value = "4" },
                new { Text = "พฤษภาคม", Value = "5" },
                new { Text = "มิถุนายน", Value = "6" },
                new { Text = "กรกฎาคม", Value = "7" },
                new { Text = "สิงหาคม", Value = "8" },
                new { Text = "กันยายน", Value = "9" },
                new { Text = "ตุลาคม", Value = "10" },
                new { Text = "พฤศจิกายน", Value = "11" },
                new { Text = "ธันวาคม", Value = "12" }
            };
            
            comboBoxMonth.DataSource = items;
            comboBoxMonth.SelectedIndex = 0;*/

            // เพิ่มวันที่
            var i = 1;
            comboBoxDay.Items.Add("เลือกวันที่");
            while ( i <= 31)
            {
                comboBoxDay.Items.Add(i);
                i++;
            }
            comboBoxDay.SelectedIndex = 0;

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Form1.frmSelectDay = daySelected;
            Form1.frmSelectMonth = monthSelected;
            Form1.frmSelectYear = yearSelected;

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBoxDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            daySelected = comboBoxDay.Items[cmb.SelectedIndex].ToString();
        }

        private void comboBoxDay_DropDown(object sender, EventArgs e)
        {
            if (comboBoxDay.Items.Contains("เลือกวันที่"))
            {
                comboBoxDay.Items.Remove("เลือกวันที่");
            }
        }

        private void comboBoxMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            //monthSelected = comboBoxMonth.SelectedValue.ToString();
            //KeyValuePair<string, string> selectedPair = (KeyValuePair<string, string>)comboBoxMonth.SelectedItem;

            //Console.WriteLine(selectedPair.Key);
            //Console.WriteLine(selectedPair.Value);

            monthSelected = (string)comboBoxMonth.SelectedValue;

            //Console.WriteLine(comboBoxMonth.SelectedItem);
            //Console.WriteLine(comboBoxMonth.SelectedValue);

            //Console.WriteLine("select index : "+comboBoxMonth.SelectedValue.ToString());
        }

        private void comboBoxMonth_DropDown(object sender, EventArgs e)
        {
            /*if (comboBoxMonth.Items.Contains("เลือกเดือน"))
            {
                comboBoxMonth.Items.Remove("เลือกเดือน");
            }*/
            //Console.WriteLine("dropdown : "+comboBoxMonth.SelectedValue.ToString());

            comboBoxMonth.DataSource = null;
            comboBoxMonth.Items.Clear();

            /*Console.WriteLine(comboBoxMonth.SelectedIndex);
            int monthSelectedIndex = comboBoxMonth.SelectedIndex;
            comboBoxMonth.Items.RemoveAt(monthSelectedIndex);*/

            comboBoxMonth.DisplayMember = "Text";
            comboBoxMonth.ValueMember = "Value";
            var items = new[] {
                new { Text = "มกราคม", Value = "1" },
                new { Text = "กุมภาพันธ์", Value = "2" },
                new { Text = "มีนาคม", Value = "3" },
                new { Text = "เมษายน", Value = "4" },
                new { Text = "พฤษภาคม", Value = "5" },
                new { Text = "มิถุนายน", Value = "6" },
                new { Text = "กรกฎาคม", Value = "7" },
                new { Text = "สิงหาคม", Value = "8" },
                new { Text = "กันยายน", Value = "9" },
                new { Text = "ตุลาคม", Value = "10" },
                new { Text = "พฤศจิกายน", Value = "11" },
                new { Text = "ธันวาคม", Value = "12" }
            };
            comboBoxMonth.DataSource = items;

            
        }

        private void comboBoxYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            //yearSelected = comboBoxYear.SelectedValue.ToString();

            ComboBox cmb = (ComboBox)sender;
            yearSelected = comboBoxYear.Items[cmb.SelectedIndex].ToString();
        }

        private void comboBoxYear_DropDown(object sender, EventArgs e)
        {
            if (comboBoxYear.Items.Contains("เลือกปี"))
            {
                comboBoxYear.Items.Remove("เลือกปี");
            }
        }
    }
}
