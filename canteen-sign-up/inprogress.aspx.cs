using System;
using System.Web.UI;

namespace canteen_sign_up
{
    public partial class inprogress : Page
    {
        EditablePresetData registeredUserData;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) {
                ((user)this.Master).ProgressImage = "~/images/Progress2.svg";
                PageSelector.RedirectToCorrectPage(PageSelector.RegState.WaitingForConfirmation, this);
                GetUserRegistrationData();
                WritingUsernameInStartPage();
                DisplayStoredData();
            }
        }

        private void GetUserRegistrationData()
        {
            // TODO: Gather data from database
            registeredUserData = new EditablePresetData();
            registeredUserData.Firstname = "Max";
            registeredUserData.Lastname = "Muster";
            registeredUserData.ZipCode = "4545";
            registeredUserData.City = "Hansenberg";
            registeredUserData.Street = "Graben";
            registeredUserData.HouseNumber = "69";
            registeredUserData.IBAN = "AT00 0000 0000 0000 0000";
            registeredUserData.BIC = "-";
            ViewState["RegisteredUserData"] = registeredUserData;
        }

        private void DisplayStoredData()
        {
            txtAccountOwner.Text = $"<strong>Kontoinhaber</strong>: {registeredUserData.Firstname} {registeredUserData.Lastname}";
            txtZipCodeCity.Text = $"<strong>PLZ, Ort</strong>: {registeredUserData.ZipCode} {registeredUserData.City}";
            txtStreetHouseNr.Text = $"<strong>Straße, Hausnummer</strong>: {registeredUserData.Street} {registeredUserData.HouseNumber}";
            txtIBAN.Text = $"<strong>IBAN</strong>: {registeredUserData.IBAN}";
            txtBIC.Text = $"<strong>BIC (optional)</strong>: {registeredUserData.BIC}";
        }

        private void WritingUsernameInStartPage()
        {
            string getName = Environment.UserName;
            string[] username = getName.Split('.');

            string firstname = username[0][0].ToString().ToUpper() + username[0].Substring(1);
            string lastname = username[1][0].ToString().ToUpper() + username[1].Substring(1);

            lblMessage.Text = ($"Hallo {firstname} {lastname},<br /><br />die Registrierung wurde gespeichert! " +
                $"Lasse nun das gedruckte Formular vom Kontoinhaber unterschreiben und gib es in der Schule ab. " +
                $"Weiters besteht die Möglichkeit, die eingegebenen Daten zu bearbeiten und erneut zu drucken.");
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Session["EditData"] = ViewState["RegisteredUserData"];
            Response.Redirect("register.aspx");
        }
    }
}