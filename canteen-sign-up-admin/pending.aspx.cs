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
using System.Web.UI.HtmlControls;
using ImageMagick;
using System.Diagnostics;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using System.IO;
using System.Drawing.Imaging;
using ZXing;

namespace canteen_sign_up_admin
{
    public partial class _default : System.Web.UI.Page
    {
        Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
        DataFilter filter = new DataFilter();
        bool gvDataBoundRecently = false;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                gvStudentsData.DataSource = filter.GetStateFilteredInfo(DataFilter.GetSqlCmd(DataFilter.tableColumnNamesEnglish,
                                                                                             DataFilter.tableColumnNamesGerman),
                                                                                             1 /*(pending)*/);
                gvStudentsData.DataBind();
            }
            else
            {
                //get the event target name and find the control
                string ctrlName = Page.Request.Params.Get("__EVENTTARGET");
                string txtContent = Page.Request.Form[ctrlName];

                filter.AddTextboxesToGV(gvStudentsData, this.Txt_Changed, 1);

                if (ctrlName != "" && Page.FindControl(ctrlName).ID.StartsWith("txt"))
                {
                    if (txtContent.Trim() == "")
                    {
                        Txt_Changed(Page.FindControl(ctrlName), null);
                    }
                }

                if (ViewState["UploadDialogID"] != null)
                {
                    GenUploadDialog((string)ViewState["UploadDialogID"]);
                }
            }
        }

        protected void GridViewStudentsData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView gridView = (GridView)sender;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == 0 && gvDataBoundRecently == false)
                {
                    gvDataBoundRecently = true; //gehört zu AddEmptyRow
                    filter.AddEmtpyRow(ref gridView);

                    filter.AddTextboxesToGV(gridView, this.Txt_Changed, 1);
                }
            }
        }

        protected void Txt_Changed(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            List<string> columnsEng = DataFilter.tableColumnNamesEnglish;
            string columnName = null;
            int IDNumber = Convert.ToInt32(txt.ID.Substring(txt.ID.Length - 2));
            DataTable filtered = new DataTable();

            if (txt.ID.Substring(3, txt.ID.Length - 5) == columnsEng[IDNumber])
            {
                columnName = columnsEng[IDNumber];
            }
            try
            {
                string pattern = txt.Text.Replace('*', '%') + "%";
                string sqlCmd = "SELECT " + DataFilter.ColumnsEngToGer(DataFilter.tableColumnNamesEnglish, DataFilter.tableColumnNamesGerman) +
                                                $" FROM signed_up_users " +
                                                $"LEFT JOIN students " +
                                                $"ON signed_up_users.email = students.email " +
                                                $"LEFT JOIN states " +
                                                $"ON signed_up_users.state_id = states.state_id " +
                                                $"WHERE signed_up_users.state_id = 1 ";

                if (columnName == "students.student_id" && pattern == "%%")
                {
                    filtered = db.RunQuery(sqlCmd);
                }
                else
                {
                    filtered = db.RunQuery(sqlCmd + $" AND {columnName} LIKE ?", pattern);
                }

                if (filtered.Rows.Count == 0)
                {
                    filtered.Rows.Add(filtered.NewRow());
                    if (filtered.Columns[IDNumber].DataType == typeof(int))
                    {
                        filtered.Rows[0][IDNumber] = -1;
                    }
                    else if (filtered.Columns[IDNumber].DataType == typeof(string))
                    {
                        filtered.Rows[0][IDNumber] = txt.Text + " not found";
                    }
                    else
                    {
                        filtered.Rows[0][IDNumber] = DBNull.Value;
                    }
                }



                gvStudentsData.DataSource = filtered;
                gvStudentsData.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnUploadFile_Click(object sender, EventArgs e)
        {
            GenUploadDialog();
        }

        private void GenUploadDialog(string id = null)
        {
            DialogBox dbox = (DialogBox)Page.LoadControl("DialogBox.ascx");
            if (id != null) { dbox.ID = id; }
            dbox.Title = "Anmeldeformulare hochladen";
            dbox.setFileUploadSelect("Wählen Sie ein .pdf Dokument aus, in welchem<br/>sich die eingescannten Anmeldungsformulare befinden.");
            dbox.DialogFinished += FormUploadFinished;
            ((admin)this.Master).Form.Controls.Add(dbox);
            ViewState["UploadDialogID"] = dbox.ID;
        }

        private void GenErrorDialog(string errorMessage)
        {
            DialogBox dbox = (DialogBox)Page.LoadControl("DialogBox.ascx");
            dbox.Title = "Auswahl der Datei fehlgeschlagen!";
            dbox.setFileUploadSelect(errorMessage);
            ((admin)this.Master).Form.Controls.Add(dbox);
        }

        private void FormUploadFinished(object sender, DialogEventArgs e)
        {
            GetPathForQrCodeScan(sender, e);
        }

        private void GetPathForQrCodeScan(object sender, DialogEventArgs e)
        {
            if (e.Result == DialogEventArgs.EventResults.Ok)
            {
                DialogBox dbox = sender as DialogBox;
                string extension;

                if (dbox.FileUpload.HasFile == true)
                {
                    extension = System.IO.Path.GetExtension(dbox.FileUpload.PostedFile.FileName);
                    if (extension == ".pdf")
                    {
                        Guid uuid = Guid.NewGuid();
                        string filename = uuid.ToString() + ".pdf";
                        string uploadedFile = dbox.FileUpload.FileName;
                        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        string tempPath = Path.GetTempPath();
                        string baseDir = appData + @"\_CanteenRegistrations\";
                        string tempImageName = default;

                        if (!Directory.Exists(baseDir))
                        {
                            Directory.CreateDirectory(baseDir);
                        }
                        dbox.FileUpload.SaveAs(tempPath + uploadedFile);

                        string ghostScriptPath = appData + @"\GSWIN";
                        if (!Directory.Exists(ghostScriptPath))
                        {
                            // TODO: ERROR: inform user GhostScript is missing
                            return;
                        }
                        MagickNET.SetGhostscriptDirectory(ghostScriptPath);
                        MagickReadSettings settings = new MagickReadSettings();
                        settings.Density = new Density(300);

                        // scan qr code and assign pdf location here
                        SplitPdfAndScanQrCode(ref uuid, uploadedFile, tempPath, baseDir, ref tempImageName, settings);
                        File.Delete(tempPath + dbox.FileUpload.FileName);
                    }
                    else
                    {
                        GenErrorDialog("Sie müssen ein .pdf-Dokument wählen.");
                    }
                }
                else
                {
                    GenErrorDialog("Sie müssen eine Datei auswählen.");
                }
            }
        }

        private void SplitPdfAndScanQrCode(ref Guid uuid, string uploadedFile, string tempPath, string baseDir, ref string tempImageName, MagickReadSettings settings)
        {
            PdfDocument inputPdfFile = PdfReader.Open(tempPath + uploadedFile, PdfDocumentOpenMode.Import);
            int totalPagesInInputPdfFile = inputPdfFile.PageCount;
            IBarcodeReader reader = new BarcodeReader();
            Bitmap barcodeBitmap;
            while (totalPagesInInputPdfFile != 0)
            {
                uuid = Guid.NewGuid();
                PdfDocument outputPdfDocument = new PdfDocument();

                outputPdfDocument.AddPage(inputPdfFile.Pages[totalPagesInInputPdfFile - 1]);
                string outputPdfFilePath = Path.Combine(baseDir, uuid + ".pdf");
                outputPdfDocument.Save(outputPdfFilePath);
                totalPagesInInputPdfFile--;

                tempImageName = Guid.NewGuid().ToString() + ".png";

                using (MagickImageCollection images = new MagickImageCollection())
                {
                    images.Read(outputPdfFilePath, settings);
                    using (var vertical = images.AppendVertically())
                    {
                        vertical.Write(tempPath + @"\CaReSc_" + tempImageName);

                        barcodeBitmap = (Bitmap)Bitmap.FromFile(tempPath + @"\CaReSc_" + tempImageName);
                        // detect and decode the barcode inside the bitmap
                        string result = reader.Decode(barcodeBitmap).ToString();

                        db.RunNonQuery("UPDATE signed_up_users " +
                            "SET PDF_path = ? " +
                            "WHERE email = ?; ", outputPdfFilePath, result);
                    }
                }
            }
        }

        private Control FindControlRecursive(Control rootControl, string controlID)
        {
            if (rootControl.ID == controlID) return rootControl;

            foreach (Control controlToSearch in rootControl.Controls) {
                Control controlToReturn = FindControlRecursive(controlToSearch, controlID);
                if (controlToReturn != null) return controlToReturn;
            }
            return null;
        }
    }
}