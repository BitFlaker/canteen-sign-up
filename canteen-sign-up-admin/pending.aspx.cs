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

                Control ctrl = Page.FindControl(ctrlName);

                if (ctrlName != "" && ctrl.ID.StartsWith("txt"))
                {
                    if (txtContent == "")
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
                string pattern = txt.Text.Trim().Replace('*', '%') + "%";
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
            finally
            {
                DataFilter.ChangeTBContent(this);
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

        private void FormUploadFinished(object sender, DialogEventArgs e)
        {
            if (e.Result == DialogEventArgs.EventResults.Ok) {
                DialogBox dbox = sender as DialogBox;
                string uploadedFile = dbox.FileUpload.FileName;
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string tempPath = Path.GetTempPath();
                string baseDir = appData + @"\CanteenRegistrations\";
                if (!Directory.Exists(baseDir)){
                    Directory.CreateDirectory(baseDir);
                }
                dbox.FileUpload.SaveAs(baseDir + uploadedFile);

                string ghostScriptPath = appData + @"\GSWIN";
                if (!Directory.Exists(ghostScriptPath)) {
                    // TODO: ERROR: inform user GhostScript is missing
                    return;
                }
                MagickNET.SetGhostscriptDirectory(ghostScriptPath);
                MagickReadSettings settings = new MagickReadSettings();
                settings.Density = new Density(300);
                string tempImageName = Guid.NewGuid().ToString() + ".png";

                using (MagickImageCollection images = new MagickImageCollection()) {
                    images.Read(baseDir + uploadedFile, settings);
                    using (var vertical = images.AppendVertically()) {
                        vertical.Write(tempPath + @"\CaReSc_" + tempImageName);
                    }
                }

                // scan qr code and assign pdf location here

                File.Delete(tempPath + @"\CaReSc_" + tempImageName);
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