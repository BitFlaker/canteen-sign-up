using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using DatabaseWrapper;
using System.Data;
using IbanNet;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Diagnostics;
using System.IO;
using QRCoder;
using System.Drawing;
using System.Net;
using System.Drawing.Imaging;
using PdfSharp.Drawing.Layout;


namespace canteen_sign_up
{
    public partial class _default : System.Web.UI.Page
    {
        Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
        private XGraphicsState state;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lblInfo.Text = db.TryToConnect();
                WritingUsernameInStartPage();
            }
        }

        private void WritingUsernameInStartPage()
        {
            string getName = System.Environment.UserName;
            string[] username = getName.Split('.');

            string firstname = username[0][0].ToString().ToUpper() + username[0].Substring(1);
            string lastname = username[1][0].ToString().ToUpper() + username[1].Substring(1);

            lblMessage.Text = ($"Hallo {firstname} {lastname},<br /><br />Fülle die nachfolgenden Daten aus, " +
                "um dich bei der Mensa zu registrieren. Nach dem Absenden des Formulars muss eien Bestätigung " +
                "gedruckt, unterschrieben und abschließend abgegeben werden.");
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                lblInfo.Text = "Bitte fülle alle markierten Felder aus.";
            }
            else
            {
                SendUserData(CreateRegistrationDataTable());
            }
        }

        public void SendUserData(DataTable dt)
        {
            string values = "'";
            int affectedRows = 0;
            string[] valuesArray = (string[]) dt.Rows[0].ItemArray;

            for (int i = 0; i < valuesArray.Length - 1; i++)
            {
                values +=  values[i] + "', '";
            }
            values += $"{values[values.Length - 1]}'";

            affectedRows = db.RunNonQuery($"INSERT INTO signed_up_users " +
                            $"(email, revision, state_id, ao_firstname, ao_lastname, street, house_number, zipcode, city, IBAN, BIC, PDF_path)" +
                            $"VALUES(?)", values);

            lblInfo.Text = affectedRows.ToString();
        }

        private DataTable CreateRegistrationDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("email", typeof(string));
            dt.Columns.Add("revision", typeof(int));
            dt.Columns.Add("state_id", typeof(int));
            dt.Columns.Add("ao_firstname", typeof(string));
            dt.Columns.Add("ao_lastname", typeof(string));
            dt.Columns.Add("street", typeof(string));
            dt.Columns.Add("house_number", typeof(string));
            dt.Columns.Add("zipcode", typeof(string));
            dt.Columns.Add("city", typeof(string));
            dt.Columns.Add("IBAN", typeof(string));
            dt.Columns.Add("BIC", typeof(string));
            dt.Columns.Add("PDF_path", typeof(string));

            DataRow newRow = dt.NewRow();

            newRow.ItemArray = new string[] { "email", $"{GetNextRevision()}", "1", $"{txtFirstname}", $"{txtLastname}",
                                              $"{txtStreet}", $"{txtHouseNumber}", $"{txtZipCode}", $"{txtCity}", $"{VerifyIBAN()}",
                                              $"{txtBic}", null};

            dt.Rows.Add(newRow);

            return dt;
        }

        private string VerifyIBAN()
        {
            IbanValidator validator = new IbanValidator();
            ValidationResult validationResult = validator.Validate(txtIban.Text);
            if (validationResult.IsValid) return txtIban.Text;
            return null;
        }

        private string GetNextRevision()
        {
            string maxRevision = "0";
            try
            {
                maxRevision = (string)db.RunQueryScalar("SELECT MAX(revision) FROM signed_up_students");
            }
            catch(Exception ex)
            {
                lblInfo.Text= ex.Message; 
            }

            return maxRevision + 1;
        }

        protected void btnSendAndPrint_Click(object sender, EventArgs e)
        {
            double pageIndent = 70;

            PdfDocument document = new PdfDocument();
            document.Info.Title = "HTLVB Mensa Registrierung";
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Arial", 12, XFontStyle.Regular);
            XFont fontBold = new XFont("Arial", 14, XFontStyle.Bold);
            XFont fontBoldSmaller = new XFont("Arial", 12, XFontStyle.Bold);
            XTextFormatter tf = new XTextFormatter(gfx);
            double regularLineHeight = font.CellSpace * 12 / font.Metrics.UnitsPerEm;
            double boldLineHeight = fontBold.CellSpace * 14 / fontBold.Metrics.UnitsPerEm;

            // Generate and draw QR-Code
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("41742720210095;rev=0", QRCodeGenerator.ECCLevel.H);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(4);
            MemoryStream memoryStream = new MemoryStream();
            qrCodeImage.Save(memoryStream, ImageFormat.Png);
            XImage image = XImage.FromStream(memoryStream);
            double width = image.PixelWidth * 40 / image.HorizontalResolution;
            double height = image.PixelHeight * 40 / image.HorizontalResolution;
            gfx.DrawImage(image, page.Width - width - 40, 185, width, height);

            tf.DrawString("SEPA Lastschrift-Mandat", fontBold, XBrushes.Black, new XRect(pageIndent, 180, page.Width - 2 * pageIndent, 0), XStringFormats.TopLeft);
            tf.DrawString("für die HTL-Ausspeisung", fontBold, XBrushes.Black, new XRect(pageIndent, 180 + boldLineHeight, page.Width - 2 * pageIndent, 0), XStringFormats.TopLeft);

            double topMargin = 270;
            double spaceHeight = 20;
            string textBlock1 = "Ich ermächtige/ Wir ermächtigen die HTL Vöcklabruck, Zahlungen von meinem/ unserem Konto mittels SEPA-Lastschrift einzuziehen. Zugleich weise ich mein/ weisen wir unser Kreditinstitut an, die von dem Bildungszentrum HTL-Vöcklabruck auf mein/ unser Konto gezogenen SEPA-Lastschriften einzulösen.";
            string textBlock2 = "Ich kann/ Wir können innerhalb von acht Wochen, beginnend mit dem Belastungsdatum, die Erstattung des belasteten Betrages verlangen. Es gelten dabei die mit meinem/ unserem Kreditinstitut vereinbarten Bedingungen.";
            string textBlock3 = "Der Betrag für die bestellten Essen wird im Nachhinein für die vergangenen 4 Wochen, eingezogen.";
            
            string textHeadingName = "Name des Schülers:";
            string textHeadingClass = "Klasse:";
            string textHeadingNumber = "Schülerausweisnummer (falls bereits bekannt):";
            string textHeadingBankAccountOwnerName = "Name des Kontoinhabers:";
            string textHeadingBankAccountOwnerAddress = "Adresse des Kontoinhabers:";
            string textHeadingIBAN = "IBAN:";
            string textHeadingBIC = "BIC (nur für Auslandskonten):";

            double inset = 0;
            double space = 5;

            tf.DrawString(textBlock1, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2*pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += GetSplittedLineCount(gfx, textBlock1, font, page.Width - 2 * pageIndent) * regularLineHeight + spaceHeight;
            tf.DrawString(textBlock2, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += GetSplittedLineCount(gfx, textBlock2, font, page.Width - 2 * pageIndent) * regularLineHeight + spaceHeight;
            tf.DrawString(textBlock3, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += GetSplittedLineCount(gfx, textBlock3, font, page.Width - 2 * pageIndent) * regularLineHeight + spaceHeight;
            topMargin += 15;
            
            tf.DrawString(textHeadingName, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            inset = gfx.MeasureString(textHeadingName, font).Width;
            tf.DrawString("NAME OF USER", fontBoldSmaller, XBrushes.Black, new XRect(pageIndent + inset + space, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += regularLineHeight + space;

            tf.DrawString(textHeadingClass, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            inset = gfx.MeasureString(textHeadingClass, font).Width;
            tf.DrawString("CLASS OF USER", fontBoldSmaller, XBrushes.Black, new XRect(pageIndent + inset + space, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += regularLineHeight + space;

            tf.DrawString(textHeadingNumber, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            inset = gfx.MeasureString(textHeadingNumber, font).Width;
            tf.DrawString("NUMBER OF USER", fontBoldSmaller, XBrushes.Black, new XRect(pageIndent + inset + space, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += regularLineHeight + space;

            MemoryStream memoryStream1 = new MemoryStream();
            var test = Server.MapPath("images/HTLHeader.png");
            using (WebClient webClient = new WebClient()) {
                byte[] data = webClient.DownloadData(Server.MapPath("images/HTLHeader.png"));
                using (MemoryStream mem = new MemoryStream(data)) {
                    using (var yourImage = System.Drawing.Image.FromStream(mem)) {
                        yourImage.Save(memoryStream1, ImageFormat.Png);
                    }
                }
            }
            XImage image1 = XImage.FromStream(memoryStream1);
            double width1 = image1.PixelWidth * 11.5 / image1.HorizontalResolution;
            double height1 = image1.PixelHeight * 10.5 / image1.VerticalResolution;
            double spaceWidth = (gfx.PageSize.Width - width1) / 2;
            gfx.DrawImage(image1, spaceWidth, 0, width1, height1);

            string filename = Path.GetTempPath() + "HelloWorld.pdf";
            document.Save(filename);
            Process.Start(filename);
        }

        /// <summary>
        /// Calculate the number of soft line breaks
        /// Copied from: https://www.py4u.net/discuss/740567
        /// </summary>
        private static int GetSplittedLineCount(XGraphics gfx, string content, XFont font, double maxWidth)
        {
            Func<string, IList<string>> listFor = val => new List<string> { val };
            Func<string, bool> nOe = str => string.IsNullOrEmpty(str);
            Func<string, string> sIe = str => nOe(str) ? " " : str;
            Func<string, string, bool> canFitText = (t1, t2) => gfx.MeasureString($"{(nOe(t1) ? "" : $"{t1} ")}{sIe(t2)}", font).Width <= maxWidth;
            Func<IList<string>, string, IList<string>> appendtoLast =
                    (list, val) => list.Take(list.Count - 1)
                                       .Concat(listFor($"{(nOe(list.Last()) ? "" : $"{list.Last()} ")}{sIe(val)}"))
                                       .ToList();
            var splitted = content.Split(' ');
            var lines = splitted.Aggregate(listFor(""),
                    (lfeed, next) => canFitText(lfeed.Last(), next) ? appendtoLast(lfeed, next) : lfeed.Concat(listFor(next)).ToList(),
                    list => list.Count());

            return lines;
        }
    }
}