using System;

namespace canteen_sign_up_admin
{
    public partial class StatDisplayBox : System.Web.UI.UserControl
    {
        public void SetData(string heading, string content, string subContent, Colors color)
        {
            lblSubContent.Text = subContent;
            SetData(heading, content, color);
        }

        public void SetData(string heading, string content, Colors color)
        {
            lblHeading.Text = heading;
            lblContent.Text = content;
            switch (color) {
                case Colors.Green:
                    pnlBackground.CssClass = "card green";
                    break;
                case Colors.Orange:
                    pnlBackground.CssClass = "card orange";
                    break;
                case Colors.Blue:
                    pnlBackground.CssClass = "card blue";
                    break;
                default:
                    break;
            }
        }

        public enum Colors
        {
            Green,
            Orange,
            Blue
        }
    }
}