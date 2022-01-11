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
        Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
        public string SqlCmdFull => "SELECT students.email AS 'E-Mail', " +
                           $"students.student_id AS 'Schülerkennzahl', " +
                           $"students.firstname AS 'Vorname', " +
                           $"students.lastname AS 'Nachname', students.class AS 'Klasse', " +
                           $"revision AS 'Überarbeitungsnummer', " +
                           $"states.description AS 'Status', " +
                           $"ao_firstname AS 'Vorname / Kontoinhaber', " +
                           $"ao_lastname AS 'Nachname / Kontoinhaber', " +
                           $"street AS 'Straße', " +
                           $"house_number AS 'Hausnummer', " +
                           $"zipcode AS 'PLZ', " +
                           $"city AS 'Ort', " +
                           $"IBAN, " +
                           $"BIC, " +
                           $"PDF_path AS 'PDF-Pfad' " +
                           $"FROM signed_up_users " +
                           $"LEFT JOIN students " +
                           $"ON signed_up_users.email = students.email " +
                           $"LEFT JOIN states " +
                           $"ON signed_up_users.state_id = states.state_id ";

        public string SqlCmdStudentInfos => "SELECT students.email AS 'E-Mail', " +
                                   $"students.student_id AS 'Schülerkennzahl', " +
                                   $"students.firstname AS 'Vorname', " +
                                   $"students.lastname AS 'Nachname', students.class AS 'Klasse', " +
                                   $"revision AS 'Überarbeitungsnummer' " +
                                   $"FROM signed_up_users " +
                                   $"LEFT JOIN students " +
                                   $"ON signed_up_users.email = students.email ";

        public DataFilter()
        {
            
        }

        /// <summary>
        /// Connects to the Database and gets all signed up users, with state_id > 0
        /// </summary>
        /// <returns>Return </returns>
        public DataTable GetAllStudentInfo()
        {
            DataTable studentsData = db.RunQuery(SqlCmdFull +
                $"WHERE states.state_id > 0");

            return studentsData;
        }

        /// <summary>
        /// Connects to the Database and gets all signed up users, with state_id = 1
        /// </summary>
        /// <returns>DataTable with all the students found in the DB</returns>
        public DataTable GetPending(string sqlCmd = null)
        {
            if(sqlCmd == null)
            {
                sqlCmd = SqlCmdStudentInfos;
            }

            DataTable studentsData = db.RunQuery(sqlCmd +
                $"WHERE state_id = 1");

            return studentsData;
        }
    }
}