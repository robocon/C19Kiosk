using ESC_POS_USB_NET.Printer;
using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using ZXing;

namespace C19Kiosk
{
    class EpsonSlip
    {
        static readonly SmConfigure smConfig = new SmConfigure();

        public Bitmap DrawTextImg(String text, Font font, int setHeight = 23)
        {
            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //set the stringformat flags to rtl
            StringFormat sf = new StringFormat();
            sf.Trimming = StringTrimming.Word;

            // set text center
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap(306, setHeight);

            drawing = Graphics.FromImage(img);
            //Adjust for high quality
            drawing.CompositingQuality = CompositingQuality.HighQuality;
            drawing.InterpolationMode = InterpolationMode.HighQualityBilinear;
            drawing.PixelOffsetMode = PixelOffsetMode.HighQuality;
            drawing.SmoothingMode = SmoothingMode.HighQuality;
            drawing.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            //paint the background
            drawing.Clear(Color.White);

            //create a brush for the text
            Brush textBrush = new SolidBrush(Color.Black);

            drawing.DrawString(text, font, textBrush, new RectangleF(0, 0, 306, setHeight), sf);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            // https://stackoverflow.com/questions/3801275/how-to-convert-image-to-byte-array
            MemoryStream mStream = new MemoryStream();

            // stream in png
            img.Save(mStream, ImageFormat.Bmp);

            // 
            img.Dispose();

            return new Bitmap(Bitmap.FromStream(mStream));
        }

        public void printOutSlip(responseSaveVn app)
        {
            try
            {
                string fontName = "Tahoma";
                Font fontRegular = new Font(fontName, 16, FontStyle.Regular, GraphicsUnit.Pixel);
                Font fontBold = new Font(fontName, 16, FontStyle.Bold, GraphicsUnit.Pixel);
                Font fontBoldUnderline = new Font(fontName, 16, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Pixel);
                Font fontSuperBold = new Font(fontName, 28, FontStyle.Bold, GraphicsUnit.Pixel);
                Font superBoldUnderline = new Font(fontName, 28, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Pixel);

                Font superBoldv2 = new Font(fontName, 24, FontStyle.Bold, GraphicsUnit.Pixel);

                byte[] PartialCut = { 0x0A, 0x0A, 0x0A, 0x1B, 0x69 };
                System.Globalization.CultureInfo _cultureTHInfo = new System.Globalization.CultureInfo("th-TH");
                string currDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss", _cultureTHInfo);

                Printer printer = new Printer(smConfig.printerName);

                printer.AlignCenter();

                //printer.Image(DrawTextImg($"คิวห้องตรวจ {app.queueNumber}", superBoldv2));
                //printer.NewLines(3);

                printer.Image(new Bitmap(Bitmap.FromFile("Images/small-icon.bmp")));
                printer.Image(DrawTextImg(currDate, fontRegular));
                printer.Image(DrawTextImg(app.ex, fontRegular));
                printer.Image(DrawTextImg("HN : " + app.hn, superBoldUnderline));
                printer.Image(DrawTextImg("VN : " + app.vn, superBoldUnderline));
                printer.NewLine();
                printer.Image(DrawTextImg($"ชื่อ : {app.ptname}", fontBold));
                printer.NewLine();
                printer.Image(DrawTextImg($"สิทธิ : {app.ptright}", fontBoldUnderline));
                if (!String.IsNullOrEmpty(app.hospCode))
                {
                    printer.Image(DrawTextImg(app.hospCode, fontRegular));
                }

                printer.Image(DrawTextImg($"อายุ {app.age}", fontRegular));
                printer.Image(DrawTextImg($"บัตร ปชช. : {app.idcard}", fontRegular));
                printer.Image(DrawTextImg(app.mx, fontRegular));
                printer.NewLine();
                //printer.Image(DrawTextImg($"คิวซักประวัติที่ {app.fakeQueue}", fontBold));
                //printer.NewLine();

                /*
                if (!String.IsNullOrEmpty(app.doctor))
                {
                    printer.Image(DrawTextImg(app.doctor, fontRegular));
                }
                else
                {
                    printer.Image(DrawTextImg("แพทย์.....................", fontRegular));
                }
                */

                /*
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = 160,
                        Width = 306
                    }
                };
                var qrCodeImg = writer.Write(app.hn);
                printer.Image(qrCodeImg);
                */

                var writer2 = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_39,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = 60,
                        Width = 306,
                        PureBarcode = true
                    }
                };
                var barcodeImg = writer2.Write(app.hn);
                printer.Image(barcodeImg);
                printer.NewLine();
                printer.Image(DrawTextImg("คิวฉีดวัคซีนที่ ", fontBold));
                printer.NewLine();
                printer.Image(DrawTextImg(app.queue_vaccine, superBoldUnderline));

                //printer.Image(DrawTextImg("ยื่นรับยาที่ช่องหมายเลข6", fontBold));
                //printer.Image(new Bitmap(Bitmap.FromFile("Images/extra.bmp")));

                //printer.Image(DrawTextImg($"หากที่อยู่ของท่านมีการเปลี่ยนแปลง", fontBold));
                //printer.Image(DrawTextImg($"กรุณาแจ้งแผนกทะเบียน", fontBold));
                //printer.Image(DrawTextImg($"เพื่อประโยชน์และสิทธิ์ของท่านเอง", fontBold));
                printer.NewLines(8);
                printer.Append(PartialCut);
                // ตัดกระดาษ

                printer.Image(DrawTextImg("บัตรคิวสำหรับผู้ป่วย ", fontBoldUnderline));
                printer.NewLine();
                printer.Image(DrawTextImg("คิวฉีดวัคซีนที่ ", fontBold));
                printer.NewLine();
                printer.Image(DrawTextImg(app.queue_vaccine, fontSuperBold));
                printer.NewLine();

                printer.Image(DrawTextImg("HN : " + app.hn, fontSuperBold));
                printer.Image(DrawTextImg("VN : " + app.vn, fontSuperBold));
                printer.NewLine();

                printer.Image(DrawTextImg($"ชื่อ : {app.ptname}", fontBold));
                printer.NewLine();

                printer.Image(barcodeImg);
                
                printer.NewLines(8);
                printer.Append(PartialCut);

                //////
                if (app.queueStatus == "y")
                {
                    //printer.Image(new Bitmap(Bitmap.FromFile("Images/small-icon2.bmp")));
                    //printer.Image(DrawTextImg(currDate, fontRegular));
                    //printer.NewLine();
                    /*
                    printer.Image(DrawTextImg("บัตรคิวซักประวัติ", fontBold));
                    printer.NewLine();
                    printer.Image(DrawTextImg(app.queueRoom, fontRegular));
                    printer.NewLine();
                    printer.Image(DrawTextImg($"HN : {app.hn}", fontSuperBold));
                    printer.NewLine();
                    printer.Image(DrawTextImg($"ชื่อ : {app.ptname}", fontRegular));
                    printer.Image(DrawTextImg($"ประเภท : {app.ptType}", fontRegular));
                    printer.NewLine();
                    printer.Image(DrawTextImg(app.queueNumber, fontSuperBold));
                    printer.NewLine();
                    printer.Image(DrawTextImg($"คิวพบแพทย์ผู้ป่วยนัด", fontRegular));
                    printer.NewLines(8);
                    printer.Append(PartialCut);
                    */

                    //printer.Image(new Bitmap(Bitmap.FromFile("Images/small-icon2.bmp")));
                    //printer.Image(DrawTextImg(currDate, fontRegular));
                    //printer.NewLine();
                    printer.Image(DrawTextImg("คิวพบแพทย์ผู้ป่วยนัด", fontBold));
                    printer.Image(DrawTextImg(currDate, fontRegular));
                    printer.Image(DrawTextImg(app.queueNumber, fontSuperBold));
                    printer.Image(DrawTextImg(app.queueRoom + " คิวที่ " + app.runNumber, fontRegular));
                    //printer.Image(DrawTextImg($"เลขคิวห้องตรวจ {app.runNumber}", fontSuperBold, 32));
                    printer.NewLine();
                    printer.Image(DrawTextImg($"คิวซักประวัติที่ {app.fakeQueue}", fontSuperBold, 32));
                    printer.NewLine();
                    if (!String.IsNullOrEmpty(app.doctor))
                    {
                        printer.Image(DrawTextImg(app.doctor, fontRegular));
                    }
                    else
                    {
                        printer.Image(DrawTextImg("แพทย์.....................", fontRegular));
                    }
                    printer.Image(DrawTextImg($"HN : {app.hn}", fontBold));
                    printer.Image(DrawTextImg($"ชื่อ : {app.ptname}", fontRegular));
                    printer.Image(DrawTextImg($"ประเภท : {app.ptType}", fontRegular));
                    printer.Image(DrawTextImg($"จำนวนคิวที่รอ {app.queueWait} คิว", fontRegular));
                    printer.Image(DrawTextImg($"ใบคิวสำหรับผู้ป่วย โปรดเก็บไว้กับตัว", fontBold));
                    printer.NewLines(8);
                    printer.Append(PartialCut);
                }





                //////
                printer.PrintDocument();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ไม่สามารถพิมพ์ได้ " + ex.Message, "แจ้งเตือน");
                Console.WriteLine(ex.Message);
            }
        }

        
    }
}
