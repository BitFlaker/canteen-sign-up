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
                gvStudentsData.DataSource = filter.GetStateFilteredInfo(DataFilter.GetSqlCmd(DataFilter.pendingColumnNamesEnglish, DataFilter.pendingColumnNamesGerman), 1);
                gvStudentsData.DataBind();
            }
            else
            {
                if (ViewState["UploadDialogID"] != null) {
                    GenUploadDialog((string)ViewState["UploadDialogID"]);
                }
            }
        }

        public static Control GetPostBackControl(Page page)
        {
            Control control = null;
            string ctrlname = page.Request.Params.Get("__EVENTTARGET");
            if (ctrlname != null && ctrlname != String.Empty)
            {
                control = page.FindControl(ctrlname);
            }
            else
            {
                foreach (string ctl in page.Request.Form)
                {
                    Control c = page.FindControl(ctl);
                    if (c != null)
                    {
                        control = c;
                        break;
                    }
                }
            }
            return control;
        }

        protected void GridViewStudentsData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView gridView = (GridView)sender;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == 0 && gvDataBoundRecently == false)
                {
                    AddEmtpyRow(gridView);
                    AddTextboxesToGV(gridView);
                }
            }
        }

        private void AddEmtpyRow(GridView sender)
        {
            DataTable dt = (DataTable)sender.DataSource;
            dt.Rows.InsertAt(dt.NewRow(), 0);
            gvStudentsData.DataSource = dt;
            ViewState["DataSource"] = dt;
            gvDataBoundRecently = true;
            gvStudentsData.DataBind();
        }

        private void AddTextboxesToGV(GridView gv)
        {
            DataFilter filter = new DataFilter();
            List<string> txtIDs = GetColumnNames(filter.GetStateFilteredInfo(DataFilter.GetSqlCmd(DataFilter.pendingColumnNamesEnglish, DataFilter.pendingColumnNamesEnglish), 1)); 
            GridViewRow row = gv.Rows[0];
            TableCell tc = null;

            for (int i = 0; i < row.Cells.Count; i++)
            {
                tc = row.Cells[i];
                tc.Text = "";

                TextBox txt = new TextBox();
                txt.ID = txtIDs[i];
                txt.EnableViewState = true;
                txt.TextChanged += new System.EventHandler(this.Txt_Changed);
                tc.Controls.Add(txt);
            }
        }
        private List<string> GetColumnNames(DataTable dt)
        {
            List<string> txtIDs = new List<string>();
            int count = 0;
            int zeros = 2;
            foreach (DataColumn dc in dt.Columns)
            {
                txtIDs.Add("txt" + dc.ColumnName + count.ToString("D" + zeros));
                count++;
            }
            return txtIDs;
        }

        /// <summary>
        /// Runs through the gridview data and formats all column names to textbox names.
        /// </summary>
        /// <param name="gv">GridView to read data from.</param>
        /// <returns>String-Type List containing all formated names.</returns>
        protected void Txt_Changed(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            List<string> columnsEng = DataFilter.columnNamesEnglish;
            string columnName = null;
            int IDNumber = Convert.ToInt32(txt.ID.Substring(txt.ID.Length - 2));

            if (txt.ID.Substring(3, txt.ID.Length - 5) == columnsEng[IDNumber])
            {
                columnName = columnsEng[IDNumber];
            }
            try
            {
                string pattern = txt.Text.Replace('*', '%') + "%";
                DataTable filtered = db.RunQuery("SELECT " + DataFilter.ColumnsEngToGer(DataFilter.pendingColumnNamesEnglish, DataFilter.pendingColumnNamesGerman) + 
                                                $" FROM signed_up_users " +
                                                $"LEFT JOIN students " +
                                                $"ON signed_up_users.email = students.email " +
                                                $"LEFT JOIN states " +
                                                $"ON signed_up_users.state_id = states.state_id " +
                                                $"WHERE {columnName} LIKE ? AND signed_up_users.state_id = 1 ",  pattern );

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
            form1.Controls.Add(dbox);
            ViewState["UploadDialogID"] = dbox.ID;
        }

        private void FormUploadFinished(object sender, DialogEventArgs e)
        {
            if (e.Result == DialogEventArgs.EventResults.Ok) {
                DialogBox dbox = sender as DialogBox;
                string uploadedFile = dbox.FileUpload.FileName;
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