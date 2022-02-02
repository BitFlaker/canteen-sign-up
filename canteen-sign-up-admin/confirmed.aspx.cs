using DatabaseWrapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace canteen_sign_up_admin
{
    public partial class confirmed : System.Web.UI.Page
    {
        Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
        DataFilter filter = new DataFilter();
        bool gvDataBoundRecently = false;
        int entryLimit = 2;
        int page = 0;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                string limit = $"LIMIT {entryLimit * page}, {entryLimit}";
                string sqlCmd = DataFilter.GetSqlCmd(DataFilter.tableColumnNamesEnglish, DataFilter.tableColumnNamesGerman);
                ViewState["CurrLimit"] = limit;
                ViewState["Limitpage"] = page;

                DataTable filteredTable = filter.GetStateFilteredInfo(sqlCmd, 2 /*(confirmed)*/, (string)ViewState["CurrLimit"]);
                DataTable originalTable = filter.GetStateFilteredInfo(sqlCmd, 2 /*(confirmed)*/);

                gvStudentsData.DataSource = filteredTable;

                lblDataInfo.Text = GetInfoText(originalTable, filteredTable);

                gvStudentsData.DataBind();
            }
            else
            {
                //get the event target name and find the control
                string ctrlName = Page.Request.Params.Get("__EVENTTARGET");
                string txtContent = Page.Request.Form[ctrlName];

                filter.AddTextboxesToGV(gvStudentsData, this.Txt_Changed, 2);


                Control ctrl = Page.FindControl(ctrlName);

                if (ctrlName != "" && ctrl.ID.StartsWith("txt"))
                {
                    if (txtContent == "")
                    {
                        Txt_Changed(ctrl, null);
                    }
                }
            }
        }

        protected void GridViewStudentsData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView gridView = (GridView)sender;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == 0 && gvDataBoundRecently == false)
                {
                    gvDataBoundRecently = true; //gehört zu AddEmptyRow
                    filter.AddEmtpyRow(ref gridView);

                    filter.AddTextboxesToGV(gridView, this.Txt_Changed, 2);
                }
            }
        }

        protected void Txt_Changed(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            List<string> columnsEng = DataFilter.columnNamesEnglish;
            string columnName = null;
            int IDNumber = Convert.ToInt32(txt.ID.Substring(txt.ID.Length - 2));
            DataTable filtered = new DataTable();

            if (txt.ID.Substring(3, txt.ID.Length - 5) == columnsEng[IDNumber])
            {
                columnName = columnsEng[IDNumber];
            }
            try
            {
                string pattern = txt.Text.Trim().Replace('*', '%') + "%";
                string sqlCmd = "SELECT " + DataFilter.ColumnsEngToGer(DataFilter.tableColumnNamesEnglish, DataFilter.tableColumnNamesGerman) +
                                                $" FROM signed_up_users " +
                                                $"LEFT JOIN students " +
                                                $"ON signed_up_users.email = students.email " +
                                                $"LEFT JOIN states " +
                                                $"ON signed_up_users.state_id = states.state_id " +
                                                $"WHERE signed_up_users.state_id = 2 ";

                if (columnName == "students.student_id" && pattern == "%%")
                {
                    filtered = db.RunQuery(sqlCmd);
                }
                else
                {
                    filtered = db.RunQuery(sqlCmd + $" AND {columnName} LIKE ?", pattern);
                }

                if (filtered.Rows.Count == 0)
                {
                    filtered.Rows.Add(filtered.NewRow());
                    if (filtered.Columns[IDNumber].DataType == typeof(int))
                    {
                        filtered.Rows[0][IDNumber] = -1;
                    }
                    else if (filtered.Columns[IDNumber].DataType == typeof(string))
                    {
                        filtered.Rows[0][IDNumber] = txt.Text + " not found";
                    }
                    else
                    {
                        filtered.Rows[0][IDNumber] = DBNull.Value;
                    }
                }

                gvStudentsData.DataSource = filtered;
                gvStudentsData.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnNextEntries_Click(object sender, EventArgs e)
        {
            page = (int)ViewState["Limitpage"];
            string newLimit = $"LIMIT {entryLimit * ++page}, {entryLimit}";
            string sqlCmd = DataFilter.GetSqlCmd(DataFilter.tableColumnNamesEnglish, DataFilter.tableColumnNamesGerman);
            DataTable filteredTable = filter.GetStateFilteredInfo(sqlCmd, 2 /*(confirmed)*/, newLimit);
            DataTable originalTable = filter.GetStateFilteredInfo(sqlCmd, 2 /*(confirmed)*/);

            if(filteredTable.Rows.Count == 0)
            {
                gvStudentsData.DataSource = filter.GetStateFilteredInfo(sqlCmd, 2 /*(confirmed)*/, (string)ViewState["CurrLimit"]);
                gvStudentsData.DataBind();
                return;
            }
            ViewState["CurrLimit"] = newLimit;
            ViewState["Limitpage"] = page;

            gvStudentsData.DataSource = filteredTable;

            lblDataInfo.Text = GetInfoText(originalTable, filteredTable);

            gvStudentsData.DataBind();
        }

        /// <summary>
        /// Checks for identical primarykeys to create an info text.
        /// </summary>
        /// <param name="original">The original table, to check from.</param>
        /// <param name="filtered">The filtere table, to check with.</param>
        /// <returns>A detailed info concerning entries shown in the GridView.</returns>
        private string GetInfoText(DataTable original, DataTable filtered)
        {
            DataRow dataRow = null;
            string email = (string)filtered.Rows[filtered.Rows.Count - 1][0];
            int revision = (int)filtered.Rows[filtered.Rows.Count - 1][5];

            foreach (DataRow dr in original.Rows)
            {
                if (dr[0].Equals(email) && dr[5].Equals(revision))
                {
                    dataRow = dr;
                    break;
                }
            }
            if (dataRow != null)
            {
                return $"Einträge {entryLimit * page + 1} bis {original.Rows.IndexOf(dataRow) + 1} " +
                                   $"von {original.Rows.Count} Einträgen werden angezeigt.";
            }
            else
            {
                return $"Email {email} und Überarbeitungsnummer {revision} müssten vorhanden sein, fehlen aber";
            }
        }
    }
}
