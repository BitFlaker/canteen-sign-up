using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
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


namespace canteen_sign_up_admin
{
    public partial class confirmed : System.Web.UI.Page
    {
        DynamicTable dynTable;
        Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
        string baseSql = " FROM signed_up_users INNER JOIN(SELECT email, ao_firstname, ao_lastname, MAX(revision) AS revision FROM signed_up_users GROUP BY email) b ON signed_up_users.email = b.email AND signed_up_users.revision = b.revision LEFT JOIN students ON signed_up_users.email = students.email WHERE signed_up_users.state_id = 2";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ((admin)this.Master).ActivateConfirmedButtonClass();
                SelectedColumns dataColumns = new SelectedColumns();
                dataColumns.Add("signed_up_users.email", "E-Mail");
                dataColumns.Add("students.student_id", "Schülerausweis-Nr.");
                dataColumns.Add("students.firstname", "Vorname");
                dataColumns.Add("students.lastname", "Nachname");
                dataColumns.Add("signed_up_users.revision", "Überarbeitungsnummer");
                dynTable = new DynamicTable(dataColumns, baseSql, int.Parse(ddlEntriesPerPage.SelectedValue), grdData, ViewState);
                lblCurrPage.Text = "Bestätigte Registrierungen";
            }
            else
            {
                dynTable = (DynamicTable)ViewState["DynTable"];
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
            long entriesCount = (long)db.RunQueryScalar("SELECT COUNT(*) FROM signed_up_users WHERE signed_up_users.state_id = 2");
            long outdatedEntriesCount = entriesCount - latestEntriesCount;

            DateTime date = (DateTime)db.RunQueryScalar("SELECT MAX(change_date) FROM signed_up_users WHERE state_id = 2;");

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

        protected void btnActivate_Click(object sender, EventArgs e)
        {
            List<string> selectedEmails = dynTable.GetSelectedEntries(columnIndex: 0);
            foreach (string s in selectedEmails)
            {
                db.RunNonQuery($"UPDATE signed_up_users SET state_id = 3 WHERE email = ? AND revision = {db.RunQueryScalar($"SELECT MAX(revision) FROM signed_up_users WHERE email = ?", s)}", s);
            }

            dynTable.LoadData();
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
            if (dt != null && dt.Rows.Count > 0) {
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
    }
}
