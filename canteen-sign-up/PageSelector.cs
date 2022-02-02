using DatabaseWrapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

namespace canteen_sign_up
{
    public class PageSelector
    {
        private static Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
        private static RegState currentState = RegState.Confirmed;

        private PageSelector() { }

        public static void RedirectToCorrectPage(RegState callingPageState, Page page)
        {
            UserData user = new UserData(Environment.UserName + "@htlvb.at");
            DataTable dt = db.RunQuery($"SELECT email, state_id, revision FROM signed_up_users WHERE email = '{user.UserMail}' AND revision = (SELECT MAX(revision) FROM signed_up_users WHERE email = '{user.UserMail}')");
            if (dt.Rows.Count > 0) {
                currentState = (RegState)dt.Rows[0]["state_id"];
            }
            else {
                currentState = RegState.NotRegistered;
            }
            if (callingPageState == currentState || callingPageState == RegState.Confirmed && (currentState == RegState.Active || currentState == RegState.Deactivated)) { return; }
            switch (currentState) {
                case RegState.NotRegistered:
                    page.Response.Redirect("register.aspx");
                    break;
                case RegState.WaitingForConfirmation:
                    page.Response.Redirect("inprogress.aspx");
                    break;
                case RegState.Active:
                case RegState.Deactivated:
                case RegState.Confirmed:
                    page.Response.Redirect("confirmed.aspx");
                    break;
            }
        }

        public enum RegState
        {
            Deactivated,
            WaitingForConfirmation,
            Confirmed,
            Active,
            NotRegistered
        }
    }
}