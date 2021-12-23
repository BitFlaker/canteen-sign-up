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
                gvStudentsData.DataSource = filter.FillUpStudentsDataTable();
                gvStudentsData.DataBind();
            }
            else
            {
                AddTextboxesToGV(gvStudentsData.Rows[0]);
            }
        }

        protected void GridViewStudentsData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == 0 && gvDataBoundRecently == false)
                {
                    AddEmtpyRow((GridView)sender);
                    AddTextboxesToGV(gvStudentsData.Rows[0]);
                }
            }
        }

        private void AddTextboxesToGV(GridViewRow row)
        {
            string[] txtIDs = new string[] {"txtstudents.email", "txtstudent_id", "txtfirstname", "txtlastname", "txtclass",
            "txtrevision", "txtstates.description", "txtao_firstname", "txtao_lastname", "txtstreet", "txthouse_number",
            "txtzipcode", "txtcity", "txtIBAN", "txtBIC", "txtPDF_path"};

            TableCell tc = null;
            for (int i = 0; i < row.Cells.Count; i++)
            {
                tc = row.Cells[i];
                TextBox txt = new TextBox();
                tc.Text = "";
                txt.Text = tc.Text;
                txt.ID = txtIDs[i];
                txt.EnableViewState = true;
                txt.AutoPostBack = true;
                txt.TextChanged += new System.EventHandler(this.Txt_Changed);
                tc.Controls.Add(txt);
            }
        }

        private void AddEmtpyRow([Optional] GridView sender)
        {
            DataTable dt = (DataTable)sender.DataSource;
            dt.Rows.InsertAt(dt.NewRow(), 0);
            gvStudentsData.DataSource = dt;
            gvDataBoundRecently = true;
            gvStudentsData.DataBind();
        }

        protected void Txt_Changed(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;

            try
            {
                DataTable filtered = db.RunQuery("SELECT students.email AS 'E-Mail', " +
                $"students.student_id AS 'Schülerkennzahl', " +
                $"students.firstname AS 'Vorname', " +
                $"students.lastname AS 'Nachname', students.class AS 'Klasse', " +
                $"revision AS 'Überarbeitungsnummer', " +
                $"states.description AS 'Status', " +
                $"ao_firstname AS 'Vorname / Kontoinhaber', " +
                $"ao_lastname AS 'Nachname / Kontoinhaber', " +
                $"street AS 'Straße', " +
                $"house_number AS 'Hausnummer', " +
                $"zipcode AS 'PLZ', " +
                $"city AS 'Ort', " +
                $"IBAN, " +
                $"BIC, " +
                $"PDF_path AS 'PDF-Pfad' " +
                $"FROM signed_up_users " +
                $"LEFT JOIN students " +
                $"ON signed_up_users.email = students.email " +
                $"LEFT JOIN states " +
                $"ON signed_up_users.state_id = states.state_id " +
                $"WHERE {txt.ID.Substring(3)} LIKE ?; ", "%"+txt.Text+"%");

                if (filtered.Rows.Count == 0)
                {
                    lblInfo.Text = "Keine Datensätze";
                    lblInfo.ForeColor = Color.Gray;
                    lblInfo.Font.Italic = true;
                }

                gvStudentsData.DataSource = filtered;
                gvStudentsData.DataBind();
            }
            catch(Exception ex)
            {
                throw ex;
            }


        }
    }
}