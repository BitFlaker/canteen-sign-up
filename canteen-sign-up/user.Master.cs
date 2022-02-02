using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace canteen_sign_up
{
    public partial class user : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string ProgressImage
        {
            get {
                return imgProgressImage.ImageUrl;
            }
            set {
                imgProgressImage.ImageUrl = value;
            }
        }
    }
}