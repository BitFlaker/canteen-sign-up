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

        public HtmlForm Form { get { return form1; } }

    }
}