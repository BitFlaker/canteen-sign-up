using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

        public DialogBox()
        {
            ID = Guid.NewGuid().ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public string Title { 
            get { return title; } 
            set { title = value; txtTitle.Text = value; } 
        }

        public void setFileUploadSelect(string description)
        {
            Label lblDesc = new Label();
            lblDesc.Text = description;
            lblDesc.Attributes["style"] = "margin-bottom: 20px;";
            fu = new FileUpload();
            Control content = FindControlRecursive(this, "dialogContent");
            content.Controls.Add(lblDesc);
            content.Controls.Add(fu);
            btnCancel.Visible = true;
        }

        public void setErrorMessage(string description)
        {
            Label lblDesc = new Label();
            lblDesc.Text = description;
            lblDesc.Attributes["style"] = "margin-bottom: 10px;";
            Control content = FindControlRecursive(this, "dialogContent");
            content.Controls.Add(lblDesc);
            btnCancel.Visible = false;
        }

        public void SetStudentInformation(DataTable dt)
        {
            Label lblDesc = new Label();
            string description = "<table class=\"underlinedTable\">";
            string pdfPath = "";
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i == dt.Columns.Count - 1 && dt.Rows[0][i] != DBNull.Value)
                {
                    string[] splitPath = dt.Rows[0][i].ToString().Split('\\');
                    switch (dt.Columns[i].ToString())
                    {
                        case "PDF-Pfad":
                            pdfPath = splitPath[splitPath.Length - 1];
                            break;
                        default:
                            break;
                    }
                    description += "<tr><td><b>" + dt.Columns[i].ToString() + ":</b></td> <td>" + splitPath[splitPath.Length - 1] + "<br /></td></tr>";
                }
                else if (dt.Rows[0][i] == DBNull.Value || Convert.ToString(dt.Rows[0][i]) == "")
                {
                    description += "<tr><td><b>" + dt.Columns[i].ToString() + ":</b></td> <td> - <br /></td></tr>";
                }
                else
                {
                    description += "<tr><td><b>" + dt.Columns[i].ToString() + ":</b></td><td>" + dt.Rows[0][i].ToString() + "<br/></td></tr>";
                }
            }
            lblDesc.Text = description + "</table>";
            lblDesc.Attributes["style"] = "margin-bottom: 20px;";
            lblDesc.Attributes["style"] = "text-align: left;";
            Control content = FindControlRecursive(this, "dialogContent");
            Button downloadPDF = new Button();
            if (pdfPath != "") {
                downloadPDF.Text = "Bestätigung herunterladen";
                downloadPDF.Attributes["class"] = "ActionButton SmallPadding";
                downloadPDF.Attributes["style"] = "margin-top:15px;margin-bottom:-15px;height:25px;";
                downloadPDF.Attributes["data"] = pdfPath;
                downloadPDF.Click += DownloadPDF_Click;
            }
            content.Controls.Add(lblDesc);
            if (pdfPath != "") {
                content.Controls.Add(downloadPDF);
            }
            btnCancel.Visible = false;
        }

        private void DownloadPDF_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\_CanteenRegistrations\" + btn.Attributes["data"];
            FileInfo file = new FileInfo(filename);
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AddHeader("Content-Disposition", $"attachment; filename={btn.Attributes["data"]}");
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.ContentType = "text/plain";
            Response.Flush();
            Response.TransmitFile(file.FullName);
            Response.End();
        }

        public FileUpload FileUpload { get { return fu; } }

        private Control FindControlRecursive(Control rootControl, string controlID)
        {
            if (rootControl.ID == controlID) { return rootControl; }

            foreach (Control controlToSearch in rootControl.Controls) {
                Control controlToReturn = FindControlRecursive(controlToSearch, controlID);
                if (controlToReturn != null) return controlToReturn;
            }
            return null;
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            if (DialogFinished != null) { DialogFinished(this, new DialogEventArgs(DialogEventArgs.EventResults.Ok)); }
            Parent.Controls.Remove(this);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            DialogFinished(this, new DialogEventArgs(DialogEventArgs.EventResults.Cancel));
            Parent.Controls.Remove(this);
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