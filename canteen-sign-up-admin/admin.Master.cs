using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace canteen_sign_up_admin
{
    public partial class admin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnPending_Click(object sender, EventArgs e)
        {
            DeactivateAllNavButtons(sender);
            Response.Redirect("pending.aspx");
        }

        protected void btnConfirmed_Click(object sender, EventArgs e)
        {
            DeactivateAllNavButtons(sender);
            Response.Redirect("confirmed.aspx");
        }
        
        protected void btnActive_Click(object sender, EventArgs e)
        {
            DeactivateAllNavButtons(sender);
            Response.Redirect("active.aspx");
        }

        private void DeactivateAllNavButtons(object sender)
        {
            Button navButton = sender as Button;
            btnActive.Attributes["class"] = "sideNavButton";
            btnConfirmed.Attributes["class"] = "sideNavButton";
            btnPending.Attributes["class"] = "sideNavButton";
            btnDeactivated.Attributes["class"] = "sideNavButton";
        }

        public HtmlForm Form { get { return form1; } }
        public Button PendingButton { get { return btnPending; } }

        protected void btnDeactivated_Click(object sender, EventArgs e)
        {

        }
    }
}