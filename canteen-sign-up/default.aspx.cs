using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using DataBaseWrapper;
using System.Data;

namespace canteen_sign_up
{
    public partial class _default : System.Web.UI.Page
    {
        DataBase_Secure dbs = new DataBase_Secure(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lblInfo.Text = dbs.TryToConnect();
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

            newRow.ItemArray = new string[] { "email", $"{GetHighestRevision()}", "1", "{txtfirstname}", "{txtlastname}",
                                              "{txtStreet}", "{txtHN}", "{txtZip}", "{txtcity}", $"{VerifyIBAN()}",
                                              "{txtBIC}", null};

            dt.Rows.Add(newRow);
        }
    }
}