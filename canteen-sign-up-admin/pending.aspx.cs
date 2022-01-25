using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using canteen_sign_up_admin;
using DatabaseWrapper;
using System.Web.Configuration;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using ZXing;

namespace canteen_sign_up_admin
{
    public partial class _default : System.Web.UI.Page
    {
        Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
        bool gvDataBoundRecently = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                DataFilter filter = new DataFilter();
                gvStudentsData.DataSource = filter.GetPending();
                gvStudentsData.DataBind();
            }
            //else
            //{
            //    AddTextboxesToGV(gvStudentsData);
            //}
        }

        protected void GridViewStudentsData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    if (e.Row.RowIndex == 0 && gvDataBoundRecently == false)
            //    {
            //        AddEmtpyRow((GridView)sender);
            //        AddTextboxesToGV(gvStudentsData);
            //    }
            //}
        }

        //private void AddTextboxesToGV(GridView gv)
        //{
        //    List<string> txtIDs = GetColumnNames(gv);
        //    GridViewRow row = gv.Rows[0];
        //    TableCell tc = null;

        //    for (int i = 0; i < row.Cells.Count; i++)
        //    {
        //        tc = row.Cells[i];
        //        TextBox txt = new TextBox();
        //        tc.Text = "";
        //        txt.Text = tc.Text;
        //        txt.ID = txtIDs[i];
        //        txt.EnableViewState = true;
        //        txt.AutoPostBack = true;
        //        txt.TextChanged += new System.EventHandler(this.Txt_Changed);
        //        tc.Controls.Add(txt);
        //    }
        //}

        ///// <summary>
        ///// Runs through the gridview data and formats all column names to textbox names.
        ///// </summary>
        ///// <param name="gv">GridView to read data from.</param>
        ///// <returns>String-Type List containing all formated names.</returns>
        //private List<string> GetColumnNames(GridView gv)
        //{
        //    List<string> txtIDs = new List<string>();
        //    foreach (DataColumn dc in gv.Columns)       //???
        //    {
        //        txtIDs.Add("txt" + dc.ColumnName);
        //    }
        //    return txtIDs;
        //}

        //private void AddEmtpyRow([Optional] GridView sender)
        //{
        //    DataTable dt = (DataTable)sender.DataSource;
        //    dt.Rows.InsertAt(dt.NewRow(), 0);
        //    gvStudentsData.DataSource = dt;
        //    gvDataBoundRecently = true;
        //    gvStudentsData.DataBind();
        //}

        //protected void Txt_Changed(object sender, EventArgs e)
        //{
        //    TextBox txt = (TextBox)sender;

        //    try
        //    {
        //        DataTable filtered = db.RunQuery("SELECT students.email AS 'E-Mail', " +
        //        $"students.student_id AS 'Schülerkennzahl', " +
        //        $"students.firstname AS 'Vorname', " +
        //        $"students.lastname AS 'Nachname', students.class AS 'Klasse', " +
        //        $"revision AS 'Überarbeitungsnummer', " +
        //        $"states.description AS 'Status', " +
        //        $"ao_firstname AS 'Vorname / Kontoinhaber', " +
        //        $"ao_lastname AS 'Nachname / Kontoinhaber', " +
        //        $"street AS 'Straße', " +
        //        $"house_number AS 'Hausnummer', " +
        //        $"zipcode AS 'PLZ', " +
        //        $"city AS 'Ort', " +
        //        $"IBAN, " +
        //        $"BIC, " +
        //        $"PDF_path AS 'PDF-Pfad' " +
        //        $"FROM signed_up_users " +
        //        $"LEFT JOIN students " +
        //        $"ON signed_up_users.email = students.email " +
        //        $"LEFT JOIN states " +
        //        $"ON signed_up_users.state_id = states.state_id " +
        //        $"WHERE {txt.ID.Substring(3)} LIKE ?; ", "%"+txt.Text+"%");

        //        if (filtered.Rows.Count == 0)
        //        {
        //            lblInfo.Text = "Keine Datensätze";
        //            lblInfo.ForeColor = Color.Gray;
        //            lblInfo.Font.Italic = true;
        //        }

        //        gvStudentsData.DataSource = filtered;
        //        gvStudentsData.DataBind();
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }


        //}

        protected void btnPdfUpload_Click(object sender, EventArgs e)
        {
            string outputFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\_Mensa-Anmeldungen";
            try
            {
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                string extension = "";
                Guid uuid = Guid.NewGuid();
                string filename = uuid.ToString() + ".pdf";

                string inputPdfFilePath = Path.GetTempPath() + filename;

                if (btnPdfBrowse.HasFile)
                {
                    extension = System.IO.Path.GetExtension(btnPdfBrowse.PostedFile.FileName);
                }
                else
                {
                    throw new ArgumentException($"Bitte wählen Sie eine Datei aus!");
                }

                if (extension.ToLower() == ".pdf")
                {
                    btnPdfBrowse.SaveAs(inputPdfFilePath);
                }
                else
                {
                    throw new ArgumentException($"Die hochgeladene Datei muss ein .pdf Dokument sein.");
                }

                PdfDocument inputPdfFile = PdfReader.Open(inputPdfFilePath, PdfDocumentOpenMode.Import);
                int totalPagesInInputPdfFile = inputPdfFile.PageCount;

                while (totalPagesInInputPdfFile != 0)
                {
                    //Create new uuid
                    uuid = Guid.NewGuid();

                    //Create an instance of the PDF document in memory
                    PdfDocument outputPdfDocument = new PdfDocument();

                    // Add a specific page to the PdfDocument instance
                    outputPdfDocument.AddPage(inputPdfFile.Pages[totalPagesInInputPdfFile - 1]);

                    //save the PDF document
                    // Output file path
                    string outputPdfFilePath = Path.Combine(outputFolder, uuid + ".pdf");

                    //Save the document
                    outputPdfDocument.Save(outputPdfFilePath);

                    totalPagesInInputPdfFile--;

                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = ex.Message;
            }

            IBarcodeReader reader = new BarcodeReader();

            Bitmap barcodeBitmap = (Bitmap)Bitmap.FromFile(outputFolder + @"\GenerationExample.pdf");
            // detect and decode the barcode inside the bitmap
            string result = reader.Decode(barcodeBitmap).ToString();
            if(result == null)  //Just placeholder
            {

            }
        }
    }
}