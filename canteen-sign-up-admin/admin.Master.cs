using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace canteen_sign_up_admin
{
    public partial class admin : System.Web.UI.MasterPage
    {
        string activeButtonStyles = "sideNavButton sideNavButtonActive";
        string inactiveButtonStyles = "sideNavButton";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnPending_Click(object sender, EventArgs e)
        {
            Response.Redirect("pending.aspx");
        }

        protected void btnConfirmed_Click(object sender, EventArgs e)
        {
            Response.Redirect("confirmed.aspx");
        }
        
        protected void btnActive_Click(object sender, EventArgs e)
        {
            Response.Redirect("active.aspx");
        }
        
        protected void btnDeactivated_Click(object sender, EventArgs e)
        {
            Response.Redirect("deactivated.aspx");
        }

        private void DeactivateAllNavButtons(Button activeButton)
        {
            btnActive.Attributes["class"] = inactiveButtonStyles;
            btnConfirmed.Attributes["class"] = inactiveButtonStyles;
            btnPending.Attributes["class"] = inactiveButtonStyles;
            btnDeactivated.Attributes["class"] = inactiveButtonStyles;
            activeButton.Attributes["class"] = activeButtonStyles;
        }

        public HtmlForm Form { get { return form1; } }
        public void ActivatePendingButtonClass() { DeactivateAllNavButtons(btnPending); /*lblCurrPage.Text = "Ausstehende Registrierungen";*/ }
        public void ActivateConfirmedButtonClass() { DeactivateAllNavButtons(btnConfirmed); /*lblCurrPage.Text = "Bestätigte Registrierungen";*/ }
        public void ActivateActiveButtonClass() { DeactivateAllNavButtons(btnActive); /*lblCurrPage.Text = "Aktive Nutzer";*/ }
        public void ActivateDeactivatedButtonClass() { DeactivateAllNavButtons(btnDeactivated); /*lblCurrPage.Text = "Deaktivierte Nutzer";*/ }
    }
}