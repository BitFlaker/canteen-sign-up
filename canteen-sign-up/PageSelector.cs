using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace canteen_sign_up
{
    public class PageSelector
    {
        private static RegState currentState = RegState.Confirmed;

        private PageSelector() { }

        public static void RedirectToCorrectPage(RegState callingPageState, Page page)
        {
            if (callingPageState == currentState) { return; }
            switch (currentState) {
                case RegState.NotRegistered:
                    page.Response.Redirect("register.aspx");
                    break;
                case RegState.WaitingForConfirmation:
                    page.Response.Redirect("inprogress.aspx");
                    break;
                case RegState.Confirmed:
                    page.Response.Redirect("confirmed.aspx");
                    break;
            }
        }

        public enum RegState
        {
            NotRegistered,
            WaitingForConfirmation,
            Confirmed
        }
    }
}