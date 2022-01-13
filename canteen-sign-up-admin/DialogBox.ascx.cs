using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace canteen_sign_up_admin
{
    public partial class DialogBox : System.Web.UI.UserControl
    {
        private string title;
        public delegate void DialogEventHandler(object sender, DialogEventArgs e);
        public event DialogEventHandler DialogFinished;
        private FileUpload fu;

        protected void Page_Load(object sender, EventArgs e)
        {
            ID = Guid.NewGuid().ToString();
        }

        public string Title { 
            get { return title; } 
            set { title = value; txtTitle.Text = value; } 
        }

        internal void setFileUploadSelect(string description)
        {
            Label lblDesc = new Label();
            lblDesc.Text = description;
            lblDesc.Attributes["style"] = "margin-bottom: 20px;";
            fu = new FileUpload();

            Control content = FindControlRecursive(FindControlRecursive(Page, ID), "dialogContent");
            content.Controls.Add(lblDesc);
            content.Controls.Add(fu);
        }

        public FileUpload FileUpload { get { return fu; } }

        private Control FindControlRecursive(Control rootControl, string controlID)
        {
            if (rootControl.ID == controlID) return rootControl;

            foreach (Control controlToSearch in rootControl.Controls) {
                Control controlToReturn = FindControlRecursive(controlToSearch, controlID);
                if (controlToReturn != null) return controlToReturn;
            }
            return null;
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            DialogFinished(this, new DialogEventArgs(DialogEventArgs.EventResults.Ok));
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            DialogFinished(this, new DialogEventArgs(DialogEventArgs.EventResults.Cancel));
        }
    }

    public class DialogEventArgs : EventArgs
    {
        public EventResults Result { get; private set; }

        public DialogEventArgs(EventResults result)
        {
            Result = result;
        }

        public enum EventResults
        {
            Ok,
            Cancel
        }
    }
}