using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using DatabaseWrapper;

namespace canteen_sign_up_admin
{
    public class DataFilter
    {
        #region Variables
        Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
        public static List<string> columnNamesEnglish= new List<string>() { "students.email", "students.student_id", "students.firstname", "students.lastname", "students.class",
                                                                            "signed_up_users.revision", "states.description", "signed_up_users.ao_firstname", "signed_up_users.ao_lastname",
                                                                            "signed_up_users.street", "signed_up_users.house_number", "signed_up_users.zipcode", "signed_up_users.city",
                                                                            "signed_up_users.IBAN", "signed_up_users.BIC", "signed_up_users.PDF_path"};

        public static List<string> pendingColumnNamesEnglish = new List<string>() { "students.email", "students.student_id", "students.firstname", "students.lastname", "students.class", "signed_up_users.revision" };

        public static List<string> columnNamesGerman = new List<string>() { "EMail", "Schülerkennzahl", "Vorname", "Nachname", "Klasse",
                                                                            "Überarbeitungsnummer", "Status", "Vorname des Kontoinhaber", "Nachname des Kontoinhabers",
                                                                            "Straße", "Hausnummer", "PLZ", "Ort", "IBAN", "BIC", "PDF-Pfad"};
        
        public static List<string> pendingColumnNamesGerman = new List<string>() { "EMail", "Schülerkennzahl", "Vorname", "Nachname", "Klasse", "Überarbeitungsnummer"};

        #endregion

        public DataFilter()
        {
            
        }
        
        /// <summary>
        /// Creates a SQL-command with possibility for WHERE-clauses, to gain access to all tables.
        /// </summary>
        /// <param name="englishColumns">Specified list of english columnnames.</param>
        /// <param name="germanColumns">Specified list of german columnnames.</param>
        /// <returns>A fully built SQL-command containing both lists as columns with german names.</returns>
        public static string GetSqlCmd(List<string> englishColumns, List<string> germanColumns)
        {
            return "SELECT " + ColumnsEngToGer(englishColumns, germanColumns) +
                    $" FROM signed_up_users " +
                    $"LEFT JOIN students " +
                    $"ON signed_up_users.email = students.email " +
                    $"LEFT JOIN states " +
                    $"ON signed_up_users.state_id = states.state_id ";
        }

        /// <summary>
        /// A Methode to remove code duplication when creating SELECT commands for the database.
        /// </summary>
        /// <param name="englishNames">The list with english column names.</param>
        /// <param name="germanNames">The list with german column names.</param>
        /// <returns>A joined string with values from both lists.</returns>
        public static string ColumnsEngToGer(List<string> englishNames, List<string> germanNames)
        {
            string result = "";
            for (int i = 0; i < englishNames.Count; i++) 
            {
                result += englishNames[i] + " AS '" + germanNames[i] + "'";
                if (i < englishNames.Count - 1) { result += ", "; }
            }
            return result;
        }

        /// <summary>
        /// Connects to the Database and gets all signed up users, with state_id > 0.
        /// </summary>
        /// <returns>All student info inside a DataTable.</returns>
        public DataTable GetAllStudentInfo()
        {
            DataTable studentsData = db.RunQuery(GetSqlCmd(columnNamesEnglish, pendingColumnNamesGerman) +
                $"WHERE states.state_id > 0");

            return studentsData;
        }

        /// <summary>
        /// Connects to the Database and gets all signed up users, with state_id = 1.
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <param name="stateID">The state_id, to filter</param>
        /// <returns>DataTable with all the students found in the DB.</returns>
        public DataTable GetStateFilteredInfo(string sqlCmd, int stateID)
        {
            sqlCmd += $"WHERE signed_up_users.state_id = {stateID} ";

            DataTable studentsData = db.RunQuery(sqlCmd);

            return studentsData;
        }
    }
}