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
        public DataFilter()
        {
            
        }

        public DataTable FillUpStudentsDataTable()
        {
            DataTable studentsData = db.RunQuery($"SELECT students.email AS 'E-Mail', " +
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
                $"ON signed_up_users.state_id = states.state_id;");

            return studentsData;
        }
    }
}