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
using System.Text;
using System.Net.Sockets;
using System.Net.Security;
using HttpData;

namespace canteen_sign_up
{
    public partial class _default : System.Web.UI.Page
    {
        Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
        bool isInEditMode = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) {
                lblInfo.Text = db.TryToConnect();
                FillDataIfInEditMode();
                ViewState["EditMode"] = isInEditMode;
                if (!isInEditMode) { 
                    ((user)this.Master).ProgressImage = "~/images/Progress1.svg";
                    PageSelector.RedirectToCorrectPage(PageSelector.RegState.NotRegistered, this);
                    WritingUsernameInStartPage("fülle die nachfolgenden Daten aus, " +
                    "um dich bei der Mensa zu registrieren. Nach dem Absenden des Formulars muss eine Bestätigung " +
                    "gedruckt, unterschrieben und abschließend abgegeben werden.");
                }
            }
        }

        private void FillDataIfInEditMode()
        {
            if (Session["EditData"] != null) {
                isInEditMode = true;
                EditablePresetData registeredData = (EditablePresetData)Session["EditData"];
                txtFirstname.Text = registeredData.Firstname;
                txtLastname.Text = registeredData.Lastname;
                txtZipCode.Text = registeredData.ZipCode;
                txtCity.Text = registeredData.City;
                txtStreet.Text = registeredData.Street;
                txtHouseNumber.Text = registeredData.HouseNumber;
                txtIban.Text = registeredData.IBAN;
                txtBic.Text = registeredData.BIC;
                ((user)this.Master).ProgressImage = "~/images/Progress2.svg";
                WritingUsernameInStartPage("ändere die nachfolgenden Daten und drucke das Formular erneut aus. " +
                    "Das alte Formular kann vernichtet werden. Gib dann nur das neu gedruckte Formular unterschriben ab. " +
                    "Falls doch nichts geändert werden sollte, kann dieses Fenster einfach geschlossen werden.");
            }
        }

        private void WritingUsernameInStartPage(string textBelow)
        {
            string getName = System.Environment.UserName;
            string[] username = getName.Split('.');

            string firstname = username[0][0].ToString().ToUpper() + username[0].Substring(1);
            string lastname = username[1][0].ToString().ToUpper() + username[1].Substring(1);

            lblMessage.Text = ($"Hallo {firstname} {lastname},<br /><br />" + textBelow);
        }

        public void SendUserData()
        {
            UserData user = new UserData(Environment.UserName + "@htlvb.at");
            // TODO: iban check!
            db.RunNonQuery($"INSERT INTO signed_up_users " +
                            $"(email, revision, state_id, ao_firstname, ao_lastname, street, house_number, zipcode, city, IBAN, BIC, PDF_name, change_date)" +
                            $"VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, NULL, ?)", user.UserMail, GetNextRevision(user).ToString(), 1.ToString(), txtFirstname.Text, txtLastname.Text, txtStreet.Text, txtHouseNumber.Text, txtZipCode.Text, txtCity.Text, txtIban.Text, txtBic.Text, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private bool isIbanValid()
        {
            IbanValidator validator = new IbanValidator();
            ValidationResult validationResult = validator.Validate(txtIban.Text);
            if (validationResult.IsValid) { return true; }
            return false;
        }

        private Int32 GetNextRevision(UserData user)
        {
            try {
                object revNum = db.RunQueryScalar($"SELECT MAX(revision) FROM signed_up_users WHERE email = '{user.UserMail}'");
                return revNum.GetType() != typeof(Int32) ? 0 : (Int32)revNum + 1;
            }
            catch(Exception ex) {
                lblInfo.Text= ex.Message; 
            }

            return -1;
        }

        protected void btnSendAndPrint_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) {
                lblInfo.Text = "Bitte fülle alle markierten Felder aus.";
                return;
            }

            double pageIndent = 70;
            double qrIndent = 40;

            UserData user = new UserData(Environment.UserName + "@htlvb.at");
            PdfDocument document = new PdfDocument();
            document.Info.Title = "HTLVB Mensa Registrierung";
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Calibri", 12, XFontStyle.Regular);
            XFont fontBold = new XFont("Calibri", 14, XFontStyle.Bold);
            XFont fontBoldSmaller = new XFont("Calibri", 12, XFontStyle.Bold);
            XTextFormatter tf = new XTextFormatter(gfx);
            double regularLineHeight = font.CellSpace * 12 / font.Metrics.UnitsPerEm;
            double regularLineAscend = font.CellAscent * 12 / font.Metrics.UnitsPerEm;
            double boldLineHeight = fontBold.CellSpace * 14 / fontBold.Metrics.UnitsPerEm;

            // Get and draw top heading graphic
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

            // Generate and draw QR-Code
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(user.UserMail + ";rev=0", QRCodeGenerator.ECCLevel.H);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(4);
            MemoryStream memoryStream = new MemoryStream();
            qrCodeImage.Save(memoryStream, ImageFormat.Png);
            XImage image = XImage.FromStream(memoryStream);
            double width = image.PixelWidth * 75 / image.HorizontalResolution;
            double height = image.PixelHeight * 75 / image.HorizontalResolution;
            gfx.DrawImage(image, page.Width - width - qrIndent, 165, width, height);

            // Get and draw Creditor-ID and Mandate Reference
            string mandateReference = "Mandatsreferenz: " + GetNextUniqueMandateReference();
            string creditor = "Creditor-ID: AT09HTL00000011802";
            double mandateReferenceWidth = gfx.MeasureString(mandateReference, font).Width;
            double creditorWidth = gfx.MeasureString(creditor, font).Width;
            double longestText = mandateReferenceWidth > creditorWidth ? mandateReferenceWidth : creditorWidth;

            tf.DrawString(creditor, font, XBrushes.Black, new XRect(page.Width - qrIndent - longestText, 140, page.Width - 2 * qrIndent, 0), XStringFormats.TopLeft);
            tf.DrawString(mandateReference, font, XBrushes.Black, new XRect(page.Width - qrIndent - longestText, 140 + regularLineHeight, page.Width - 2 * qrIndent, 0), XStringFormats.TopLeft);

            // Draw text
            tf.DrawString("SEPA Lastschrift-Mandat", fontBold, XBrushes.Black, new XRect(pageIndent, 180, page.Width - 2 * pageIndent, 0), XStringFormats.TopLeft);
            tf.DrawString("für die HTL-Ausspeisung", fontBold, XBrushes.Black, new XRect(pageIndent, 180 + boldLineHeight, page.Width - 2 * pageIndent, 0), XStringFormats.TopLeft);

            double topMargin = 300;
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

            double inset;
            double space = 5;
            double largerSpace = 15;

            // Draw static info text
            tf.DrawString(textBlock1, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2*pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += GetSplittedLineCount(gfx, textBlock1, font, page.Width - 2 * pageIndent) * regularLineHeight + spaceHeight;
            tf.DrawString(textBlock2, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += GetSplittedLineCount(gfx, textBlock2, font, page.Width - 2 * pageIndent) * regularLineHeight + spaceHeight;
            tf.DrawString(textBlock3, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += GetSplittedLineCount(gfx, textBlock3, font, page.Width - 2 * pageIndent) * regularLineHeight + spaceHeight;
            topMargin += 15;

            // Draw user data
            tf.DrawString(textHeadingName, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            inset = gfx.MeasureString(textHeadingName, font).Width;
            tf.DrawString(user.Firstname + " " + user.Lastname, fontBoldSmaller, XBrushes.Black, new XRect(pageIndent + inset + space, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += regularLineHeight + largerSpace;

            tf.DrawString(textHeadingClass, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            inset = gfx.MeasureString(textHeadingClass, font).Width;
            tf.DrawString(user.Class, fontBoldSmaller, XBrushes.Black, new XRect(pageIndent + inset + space, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += regularLineHeight + largerSpace;

            tf.DrawString(textHeadingNumber, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            inset = gfx.MeasureString(textHeadingNumber, font).Width;
            tf.DrawString(user.UserNumber, fontBoldSmaller, XBrushes.Black, new XRect(pageIndent + inset + space, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += regularLineHeight + largerSpace;

            tf.DrawString(textHeadingBankAccountOwnerName, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            inset = gfx.MeasureString(textHeadingBankAccountOwnerName, font).Width;
            tf.DrawString(txtFirstname.Text + " " + txtLastname.Text, fontBoldSmaller, XBrushes.Black, new XRect(pageIndent + inset + space, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += regularLineHeight + largerSpace;

            tf.DrawString(textHeadingBankAccountOwnerAddress, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            inset = gfx.MeasureString(textHeadingBankAccountOwnerAddress, font).Width;
            string address = txtZipCode.Text + " " + txtCity.Text + "\r\n" +  txtStreet.Text + " " + txtHouseNumber.Text;
            tf.DrawString(address, fontBoldSmaller, XBrushes.Black, new XRect(pageIndent + inset + space, topMargin, page.Width.Value - 2 * pageIndent - inset, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += GetSplittedLineCount(gfx, address, fontBold, page.Width.Value - 2 * pageIndent - inset) * regularLineHeight + largerSpace + largerSpace;

            tf.DrawString(textHeadingIBAN, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            inset = gfx.MeasureString(textHeadingIBAN, font).Width;
            tf.DrawString(txtIban.Text, fontBoldSmaller, XBrushes.Black, new XRect(pageIndent + inset + space, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            topMargin += regularLineHeight + largerSpace;

            tf.DrawString(textHeadingBIC, font, XBrushes.Black, new XRect(pageIndent, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);
            inset = gfx.MeasureString(textHeadingBIC, font).Width;
            tf.DrawString(txtBic.Text, fontBoldSmaller, XBrushes.Black, new XRect(pageIndent + inset + space, topMargin, page.Width.Value - 2 * pageIndent, page.Height - topMargin), XStringFormats.TopLeft);

            double dateLineWidth = 130;
            double signatureLineWidth = 200;
            double spaceBetween = 30;
            double marginBottom = 80;
            double blockWidth = dateLineWidth + spaceBetween + signatureLineWidth;
            double sideMargin = (page.Width - blockWidth) / 2.0;
            string dateText = "Ort, Datum";
            string signatureText = "Unterschrift Kontoinhaber";

            // Draw date and signature fields
            gfx.DrawString(dateText, font, XBrushes.Black, new XRect(sideMargin, page.Height - marginBottom, dateLineWidth, 0), XStringFormats.Center);
            gfx.DrawString(signatureText, font, XBrushes.Black, new XRect(sideMargin + dateLineWidth + spaceBetween, page.Height - marginBottom, signatureLineWidth, 0), XStringFormats.Center);
            gfx.DrawLine(XPens.Black, new XPoint(sideMargin, page.Height - marginBottom - regularLineAscend), new XPoint(sideMargin + dateLineWidth, page.Height - marginBottom - regularLineAscend));
            gfx.DrawLine(XPens.Black, new XPoint(sideMargin + dateLineWidth + spaceBetween, page.Height - marginBottom - regularLineAscend), new XPoint(sideMargin + dateLineWidth + spaceBetween + signatureLineWidth, page.Height - marginBottom - regularLineAscend));

            string filename = Path.GetTempPath() + Guid.NewGuid() + ".pdf";
            document.Save(filename);

            SendUserData();

            FileInfo file = new FileInfo(filename);
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AddHeader("Content-Disposition", $"attachment; filename=Mensa-Registrierung {user.Firstname} {user.Lastname}.pdf");
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.ContentType = "text/plain";
            Response.Flush();
            Response.TransmitFile(file.FullName);
            Response.End();

            PageSelector.RedirectToCorrectPage(PageSelector.RegState.NotRegistered, this);
        }

        private string GetNextUniqueMandateReference()
        {
            string envelope = "<?xml version='1.0' encoding='utf-8'?>\n" +
                             "<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' " +
                             "xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>\n" +
                             "<soap:Header>\n" +
                             "<WSHeader xmlns='MensaWebservice_MolJware'>\n" +
                             "<Username>anonymous</Username>\n" +
                             "<Password>string</Password>\n" +
                             "</WSHeader>\n" +
                             "</soap:Header>\n" +
                             "<soap:Body>\n" +
                             "<NextUniqueMandatsrefrenz xmlns='MensaWebservice_MolJware' />\n" +
                             "</soap:Body>\n" +
                             "</soap:Envelope>\n";
            return HttpsPost("www.htlvb.at", 443, "/MensaWebservice/MensaWebService.asmx", envelope);
        }

        private static string HttpsPost(string host, int port, string path, string requestBody)
        {
            using (TcpClient httpClient = new TcpClient()) {
                httpClient.Connect(host, port);
                using (NetworkStream httpStream = httpClient.GetStream()) {
                    using (SslStream secureHttpsStream = new SslStream(httpStream)) {
                        secureHttpsStream.AuthenticateAsClient(host);
                        SendPostRequest(secureHttpsStream, host, path, requestBody);
                        return ReadResponse(secureHttpsStream);
                    }
                }
            }
        }

        private static void SendPostRequest(Stream stream, string host, string path, string requestBody)
        {
            Encoding bodyEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

            using (StreamWriter headerWriter = new StreamWriter(stream, Encoding.ASCII, Int16.MaxValue, leaveOpen: true)) {
                headerWriter.NewLine = "\r\n";
                headerWriter.WriteLine("POST " + path + " HTTP/1.1");
                headerWriter.WriteLine("Host: " + host);
                headerWriter.WriteLine("Content-Length: " + bodyEncoding.GetByteCount(requestBody));
                headerWriter.WriteLine("Content-Type: text/xml; charset=" + bodyEncoding.WebName);
                headerWriter.WriteLine();
            }

            using (StreamWriter bodyWriter = new StreamWriter(stream, bodyEncoding, Int16.MaxValue, leaveOpen: true)) {
                bodyWriter.WriteLine(requestBody);
            }
        }

        private static string ReadResponse(Stream stream)
        {
            HttpRequestMessage httpResonse = HttpRequestMessage.ReadFrom(stream);
            string mandate = httpResonse.Body;
            mandate = mandate.Substring(mandate.IndexOf("<NextUniqueMandatsrefrenzResult>") + "<NextUniqueMandatsrefrenzResult>".Length, mandate.IndexOf("</NextUniqueMandatsrefrenzResult>") - mandate.IndexOf("<NextUniqueMandatsrefrenzResult>") - +"<NextUniqueMandatsrefrenzResult>".Length);
            return mandate;
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

        protected void custValIban_ServerValidate(object source, ServerValidateEventArgs args)
        { 
            IbanValidator iv = new IbanValidator();
            ValidationResult vr = iv.Validate(txtIban.Text);

            if (vr.IsValid)
                args.IsValid = true;
            else
                args.IsValid = false;
        }
    }
}