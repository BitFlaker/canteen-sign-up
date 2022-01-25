using DatabaseWrapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace canteen_sign_up
{
    public class UserData
    {
        public string UserNumber { get; private set; }
        public string Class { get; private set; }
        public string UserMail { get; private set; }
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }

        private Database db;

        public UserData(string email)
        {
            this.UserMail = email;
            db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
            ResolveDataByMail();
        }

        private void ResolveDataByMail()
        {
            DataTable dt = db.RunQuery("SELECT firstname, lastname, class, student_id FROM students WHERE email = ?", UserMail);
            if (dt.Rows.Count > 0 && dt.Columns.Count == 4) {
                Firstname = (string)dt.Rows[0][0];
                Lastname = (string)dt.Rows[0][1];
                Class = (string)dt.Rows[0][2];
                UserNumber = (string)dt.Rows[0][3];
            }
        }
    }
}