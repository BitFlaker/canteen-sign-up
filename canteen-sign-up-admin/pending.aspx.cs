using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using DatabaseWrapper;
using System.Web.Configuration;
using System.Drawing;
using ImageMagick;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;
using ZXing;
using DynamicTables;
using System.Data;

namespace canteen_sign_up_admin
{
    public partial class pending : System.Web.UI.Page
    {
        DynamicTable dynTable;
        Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
        string baseSql = " FROM signed_up_users INNER JOIN(SELECT email, ao_firstname, ao_lastname, MAX(revision) AS revision " +
                         "FROM signed_up_users GROUP BY email) b ON signed_up_users.email = b.email AND signed_up_users.revision = b.revision " +
                         "LEFT JOIN students ON signed_up_users.email = students.email WHERE signed_up_users.state_id = 1";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) {
                ((admin)this.Master).ActivatePendingButtonClass();
                SelectedColumns dataColumns = new SelectedColumns();
                dataColumns.Add("signed_up_users.email", "E-Mail");
                dataColumns.Add("students.student_id", "Schülerausweis-Nr.");
                dataColumns.Add("students.firstname", "Vorname");
                dataColumns.Add("students.lastname", "Nachname");
                dataColumns.Add("signed_up_users.revision", "Überarbeitungsnummer");
                dynTable = new DynamicTable(dataColumns, baseSql, int.Parse(ddlEntriesPerPage.SelectedValue), grdData, ViewState);
                lblCurrPage.Text = "Ausstehende Registrierungen";
            }
            else {
                dynTable = (DynamicTable)ViewState["DynTable"];
                if (ViewState["UploadDialogID"] != null) {
                    GenUploadDialog((string)ViewState["UploadDialogID"]);
                }
                if (ViewState["UserInfoDialogID"] != null && ViewState["UserInfoDialogEmail"] != null) {
                    GenerateUserInfo((string)ViewState["UserInfoDialogEmail"], (string)ViewState["UserInfoDialogID"]);
                }
            }

            dynTable.ObjInit(grdData, ViewState);
            dynTable.OnLinkButtonClick += DynTableLinkButtonClick;
            lblPageInfo.Text = $"Seite {dynTable.CurrentPage} von {dynTable.PageCount}";
            GenerateStats();
        }

        private void GenerateStats()
        {
            long latestEntriesCount = (long)db.RunQueryScalar("SELECT COUNT(*)" + baseSql);
            long entriesCount = (long)db.RunQueryScalar("SELECT COUNT(*) FROM signed_up_users WHERE signed_up_users.state_id = 1");
            long outdatedEntriesCount = entriesCount - latestEntriesCount;

            DateTime date = (DateTime)db.RunQueryScalar("SELECT MAX(change_date) FROM signed_up_users WHERE state_id = 1;");

            StatDisplayBox sdboxMostRecentEntries = (StatDisplayBox)Page.LoadControl("StatDisplayBox.ascx");
            sdboxMostRecentEntries.SetData("Aktuelle Einträge", latestEntriesCount.ToString(), StatDisplayBox.Colors.Green);
            pnlStats.Controls.Add(sdboxMostRecentEntries);
            StatDisplayBox sdboxOutdatedEntries = (StatDisplayBox)Page.LoadControl("StatDisplayBox.ascx");
            sdboxOutdatedEntries.SetData("Veraltete Einträge", outdatedEntriesCount.ToString(), StatDisplayBox.Colors.Orange);
            pnlStats.Controls.Add(sdboxOutdatedEntries);
            StatDisplayBox sdboxLastChanges = (StatDisplayBox)Page.LoadControl("StatDisplayBox.ascx");
            sdboxLastChanges.SetData("Letzte Änderung", date.ToString("HH:mm"), date.ToString("dd.MM.yyyy"), StatDisplayBox.Colors.Blue);
            pnlStats.Controls.Add(sdboxLastChanges);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            dynTable.UpdatePageButtonStates(btnPrev, btnNext);
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            dynTable.NextPage();
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            dynTable.PreviousPage();
        }

        private void DynTableLinkButtonClick(object sender, EventArgs e)
        {
            LinkButton lbGetDetails = sender as LinkButton;
            string email = lbGetDetails.Text;
            GenerateUserInfo(email);
        }

        private void GenerateUserInfo(string email, string id = null)
        {
            DialogBox studentInfo = (DialogBox)Page.LoadControl("DialogBox.ascx");
            if (id != null) { studentInfo.ID = id; }

            string sqlCmd = DataFilter.GetSqlCmd(DataFilter.columnNamesEnglish, DataFilter.columnNamesGerman) + "WHERE signed_up_users.email = ?";
            DataTable dt = db.RunQuery(sqlCmd, email);
            if (dt != null && dt.Rows.Count > 0)
            {
                studentInfo.Title = "Information über Schüler";
                studentInfo.SetStudentInformation(dt);
                studentInfo.DialogFinished += StudentInfo_DialogFinished;
                ((admin)this.Master).Form.Controls.Add(studentInfo);
                ViewState["UserInfoDialogID"] = studentInfo.ID;
                ViewState["UserInfoDialogEmail"] = email;
            }
        }

        private void StudentInfo_DialogFinished(object sender, DialogEventArgs e)
        {
            ViewState["UserInfoDialogID"] = null;
            ViewState["UserInfoDialogEmail"] = null;
        }

        protected void ddlEntriesPerPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            dynTable.ChangeEntriesPerPageCount(int.Parse(ddlEntriesPerPage.SelectedValue));
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
            dbox.Title = "Fehler!";
            dbox.setErrorMessage(errorMessage);
            ((admin)this.Master).Form.Controls.Add(dbox);
        }

        private void FormUploadFinished(object sender, DialogEventArgs e)
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
                        string uploadedFile = uuid.ToString();
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
                        File.Delete(tempPath + uploadedFile);
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
            else
            {
                ViewState["UploadDialogID"] = null;
            }
        }

        private void SplitPdfAndScanQrCode(ref Guid uuid, string uploadedFile, string tempPath, string baseDir, ref string tempImageName, MagickReadSettings settings)
        {
            PdfDocument inputPdfFile = PdfReader.Open(tempPath + uploadedFile, PdfDocumentOpenMode.Import);
            int totalPagesInInputPdfFile = inputPdfFile.PageCount;
            IBarcodeReader reader = new BarcodeReader();
            Bitmap barcodeBitmap;
            while (totalPagesInInputPdfFile > 0)
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
                            "SET PDF_name = ?, state_id = 2 " +
                            "WHERE email = ?; ", uuid + ".pdf", result);
                    }
                }
            }
        }
    }
}