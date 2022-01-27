using System;
using System.Web.UI;

namespace canteen_sign_up
{
    public partial class inprogress : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) {
                ((user)this.Master).ProgressImage = "~/images/Progress2.svg";
                WritingUsernameInStartPage();
                DisplayStoredData();
            }
        }

        private void DisplayStoredData()
        {
            txtAccountOwner.Text = "<strong>Kontoinhaber</strong>: Max Muster";
            txtZipCodeCity.Text = "<strong>PLZ, Ort</strong>: 4444 Porthaus";
            txtStreetHouseNr.Text = "<strong>Straße, Hausnummer</strong>: Kranzgraben 23";
            txtIBAN.Text = "<strong>IBAN</strong>: AT00 1234 1234 1234 1234";
            txtBIC.Text = "<strong>BIC (optional)</strong>: -";
        }

        private void WritingUsernameInStartPage()
        {
            string getName = Environment.UserName;
            string[] username = getName.Split('.');

            string firstname = username[0][0].ToString().ToUpper() + username[0].Substring(1);
            string lastname = username[1][0].ToString().ToUpper() + username[1].Substring(1);

            lblMessage.Text = ($"Hallo {firstname} {lastname},<br /><br />Die Registrierung wurde gespeichert! " +
                $"Lasse nun das gedruckte Formular vom Kontoinhaber unterschreiben und gib es in der Schule ab. " +
                $"Weiters besteht die Möglichkeit, die eingegebenen Daten zu bearbeiten und erneut zu drucken.");
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {

        }
    }
}