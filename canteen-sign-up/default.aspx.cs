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
            }
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
            PdfDocument document = new PdfDocument();
            document.Info.Title = "HTLVB Mensa Registrierung";
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Arial", 14, XFontStyle.Regular);

            gfx.DrawString("Hello, World!", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
            
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("41742720210095;rev=0", QRCodeGenerator.ECCLevel.H);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(4);


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
            double width1 = image1.PixelWidth * 40 / image1.HorizontalResolution;
            double height1 = image1.PixelHeight * 40 / image1.HorizontalResolution;
            gfx.DrawImage(image1, 10, 0, width1, height1);

            MemoryStream memoryStream = new MemoryStream();
            qrCodeImage.Save(memoryStream, ImageFormat.Png);
            XImage image = XImage.FromStream(memoryStream);
            double width = image.PixelWidth * 72 / image.HorizontalResolution;
            double height = image.PixelHeight * 72 / image.HorizontalResolution;
            gfx.DrawImage(image, page.Width - width, 200, width, height);

            string filename = Path.GetTempPath() + "HelloWorld.pdf";
            document.Save(filename);
            Process.Start(filename);
        }
    }
}