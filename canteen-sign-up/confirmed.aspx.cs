using DatabaseWrapper;
using System;
using System.Data;
using System.Web.Configuration;
using System.Web.UI;

namespace canteen_sign_up
{
    public partial class confirmed : Page
    {
        EditablePresetData registeredUserData;
        Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) {
                ((user)this.Master).ProgressImage = "~/images/Progress3.svg";
                PageSelector.RedirectToCorrectPage(PageSelector.RegState.Confirmed, this);
                GetUserRegistrationData();
                WritingUsernameInStartPage();
                DisplayStoredData();
            }
        }

        private void GetUserRegistrationData()
        {
            UserData user = new UserData(Environment.UserName + "@htlvb.at");
            DataTable dt = db.RunQuery($"SELECT email, revision, state_id, ao_firstname, ao_lastname, street, house_number, zipcode, city, IBAN, BIC FROM signed_up_users WHERE email = '{user.UserMail}' AND revision = (SELECT MAX(revision) FROM signed_up_users WHERE email = '{user.UserMail}')");
            if (dt.Rows.Count == 1) {
                registeredUserData = new EditablePresetData();
                registeredUserData.Firstname = (string)dt.Rows[0]["ao_firstname"];
                registeredUserData.Lastname = (string)dt.Rows[0]["ao_lastname"];
                registeredUserData.ZipCode = (string)dt.Rows[0]["zipcode"];
                registeredUserData.City = (string)dt.Rows[0]["city"];
                registeredUserData.Street = (string)dt.Rows[0]["street"];
                registeredUserData.HouseNumber = (string)dt.Rows[0]["house_number"];
                registeredUserData.IBAN = (string)dt.Rows[0]["IBAN"];
                registeredUserData.BIC = (string)dt.Rows[0]["BIC"];
                if (((Int32)dt.Rows[0]["state_id"]) == 3) {
                    activeAccountField.Attributes.Add("class", "activeAccount");
                    lblAccountStatus.Text = "Ihr Mensa-Login ist aktiv";
                    imgCheckOrCross.ImageUrl = "~/images/Check.svg";
                }
                else {
                    activeAccountField.Attributes.Add("class", "inactiveAccount");
                    lblAccountStatus.Text = "Ihr Mensa-Login ist inaktiv";
                    imgCheckOrCross.ImageUrl = "~/images/Disabled.svg";
                }
            }
        }

        private void DisplayStoredData()
        {
            txtAccountOwner.Text = $"<strong>Kontoinhaber</strong>: {registeredUserData.Firstname} {registeredUserData.Lastname}";
            txtZipCodeCity.Text = $"<strong>PLZ, Ort</strong>: {registeredUserData.ZipCode} {registeredUserData.City}";
            txtStreetHouseNr.Text = $"<strong>Straße, Hausnummer</strong>: {registeredUserData.Street} {registeredUserData.HouseNumber}";
            txtIBAN.Text = $"<strong>IBAN</strong>: {registeredUserData.IBAN}";
            txtBIC.Text = $"<strong>BIC (optional)</strong>: {(registeredUserData.BIC == "" ? "-" : registeredUserData.BIC)}";
        }

        private void WritingUsernameInStartPage()
        {
            string getName = Environment.UserName;
            string[] username = getName.Split('.');

            string firstname = username[0][0].ToString().ToUpper() + username[0].Substring(1);
            string lastname = username[1][0].ToString().ToUpper() + username[1].Substring(1);

            lblMessage.Text = ($"Hallo {firstname} {lastname},<br /><br />die Registrierung ist abgeschlossen! " +
                $"Alle erfassten Daten wurden erfolgreich gespeichert und bestätigt.");
        }
    }
}