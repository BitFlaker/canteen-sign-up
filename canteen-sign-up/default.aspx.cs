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


namespace canteen_sign_up
{
    public partial class _default : System.Web.UI.Page
    {
        DataBase db = new DataBase(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lblInfo.Text = db.TryToConnect();
                WritingUsernameInStartPage();
            }
        }

        private void WritingUsernameInStartPage()
        {
            string getName = System.Environment.UserName;
            string[] username = getName.Split('.');

            string firstname = username[0][0].ToString().ToUpper() + username[0].Substring(1);
            string lastname = username[1][0].ToString().ToUpper() + username[1].Substring(1);

            lblMessage.Text = ($"Hallo {firstname} {lastname},<br /><br />Fülle die nachfolgenden Daten aus, " +
                "um dich bei der Mensa zu registrieren. Nach dem Absenden des Formulars muss eien Bestätigung " +
                "gedruckt, unterschrieben und abschließend abgegeben werden.");
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
    }
}