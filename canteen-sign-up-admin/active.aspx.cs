using System;
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
    public partial class active : System.Web.UI.Page
    {
        DynamicTable dynTable;
        Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
        string baseSql = " FROM signed_up_users INNER JOIN(SELECT email, ao_firstname, ao_lastname, MAX(revision) AS revision " +
                         "FROM signed_up_users GROUP BY email) b ON signed_up_users.email = b.email AND signed_up_users.revision = b.revision " +
                         "LEFT JOIN students ON signed_up_users.email = students.email WHERE signed_up_users.state_id = 3";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SelectedColumns dataColumns = new SelectedColumns();
                dataColumns.Add("signed_up_users.email", "E-Mail");
                dataColumns.Add("students.student_id", "Schülerausweis-Nr.");
                dataColumns.Add("students.firstname", "Vorname");
                dataColumns.Add("students.lastname", "Nachname");
                dataColumns.Add("signed_up_users.revision", "Überarbeitungsnummer");
                dynTable = new DynamicTable(dataColumns, baseSql, int.Parse(ddlEntriesPerPage.SelectedValue), grdData, ViewState);
            }
            else
            {
                dynTable = (DynamicTable)ViewState["DynTable"];
            }

            dynTable.ObjInit(grdData, ViewState);
            dynTable.OnLinkButtonClick += DynTableLinkButtonClick;
            lblPageInfo.Text = $"Seite {dynTable.CurrentPage} von {dynTable.PageCount}";
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

        protected void btnDeactivate_Click(object sender, EventArgs e)
        {
            List<string> selectedEmails = dynTable.GetSelectedEntries(columnIndex: 0);
            // TODO activate user in db
            dynTable.LoadData();
        }

        private void DynTableLinkButtonClick(object sender, EventArgs e)
        {
            LinkButton lbGetDetails = sender as LinkButton;
            string email = lbGetDetails.Text;
            DialogBox studentInfo = (DialogBox)Page.LoadControl("DialogBox.ascx"); ;

            string sqlCmd = DataFilter.GetSqlCmd(DataFilter.columnNamesEnglish, DataFilter.columnNamesGerman) + "WHERE signed_up_users.email = ?";
            DataTable dt = db.RunQuery(sqlCmd, email);
            if (dt != null && dt.Rows.Count > 0)
            {
                studentInfo.Title = "Information über Schüler";
                studentInfo.SetStudentInformation(dt);
                ((admin)this.Master).Form.Controls.Add(studentInfo);
            }
        }

        protected void ddlEntriesPerPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            dynTable.ChangeEntriesPerPageCount(int.Parse(ddlEntriesPerPage.SelectedValue));
        }
    }
}