using DatabaseWrapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DynamicTables
{
    [Serializable]
    public class DynamicTable
    {
        SelectedColumns dataColumns;
        string baseSql;
        int countPerPage;
        string noEntriesFoundMsg = "Keine Einträge gefunden.";
        [field: NonSerialized()]
        StateBag viewState;
        [field: NonSerialized()]
        GridView grdData;
        [field: NonSerialized()]
        public event EventHandler<EventArgs> OnFilterChanged;
        [field: NonSerialized()]
        public event EventHandler<EventArgs> OnLinkButtonClick;

        public bool HasNextPage { get { return ((int)viewState["CurrentPage"]) + 1 < (int)viewState["PageCount"]; } }

        public bool HasPreviousPage { get { return ((int)viewState["CurrentPage"]) > 0; } }

        public int CurrentPage { get { return (int)viewState["CurrentPage"] + 1; } }

        public int PageCount { get { return Math.Max(1, (int)viewState["PageCount"]); } }

        /// <summary>
        /// Handler for dynamic tables with filter, selection and multi page support
        /// </summary>
        /// <param name="dataColumns">columns to be selected from the database</param>
        /// <param name="baseSql">SQL-Command (without 'SELECT * ', should start with ' FROM ...') which returns base dataset to be displayed at beginning and used for filtering</param>
        /// <param name="countPerPage">amount of entries per page</param>
        /// <param name="grdData">the GridView to display queried data in</param>
        /// <param name="viewState">the current ViewState for storing data necessary for the DynTable to work</param>
        public DynamicTable(SelectedColumns dataColumns, string baseSql, int countPerPage, GridView grdData, StateBag viewState)
        {
            this.dataColumns = dataColumns;
            this.baseSql = baseSql;
            this.countPerPage = countPerPage;
            this.viewState = viewState;
            this.grdData = grdData;
            viewState["DynTable"] = this;
            viewState["CurrentPage"] = 0;
            viewState["CountPerPage"] = countPerPage;
            viewState["DataColumns"] = dataColumns;
            Init();
        }

        /// <summary>
        /// Loads all additional necessary controls.
        /// </summary>
        /// <param name="grdData">the GridView to display queried data in</param>
        /// <param name="viewState">the current ViewState for storing data necessary for the DynTable to work</param>
        public void ObjInit(GridView grdData, StateBag viewState)
        {
            if (this.viewState == null) { this.viewState = viewState; }
            if (this.grdData == null) { this.grdData = grdData; }
            countPerPage = (int)viewState["CountPerPage"];
            FillFilterTextboxInGrid();
            FillContentControlsInGrid();
        }

        public void Init()
        {
            Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
            DataTable dt = db.RunQuery($"SELECT {dataColumns}{baseSql} LIMIT {(int)viewState["CurrentPage"] * countPerPage}, {countPerPage}", new string[0]);
            DataRow dr = dt.NewRow();
            dt.Rows.InsertAt(dr, 0);
            BindDataToTable(dt);
            viewState["PageCount"] = RefreshPageCount(new List<string>(), "");
        }

        private void BindDataToTable(DataTable dt)
        {
            if (dt.Rows.Count <= 1)
            {
                DataRow dr = dt.NewRow();
                dr[0] = noEntriesFoundMsg;
                dt.Rows.Add(dr);
                grdData.DataSource = dt;
                grdData.DataBound += Bound;
                grdData.DataBind();
            }
            else
            {
                grdData.DataSource = dt;
                grdData.DataBind();
            }
        }

        private void Bound(object sender, EventArgs e)
        {
            grdData.Rows[1].Cells.RemoveAt(0);
            grdData.Rows[1].Cells[0].ColumnSpan = grdData.Rows[1].Cells.Count + 1;
            int totalCount = grdData.Rows[1].Cells.Count;
            for (int i = 1; i < totalCount; i++)
            {
                grdData.Rows[1].Cells.RemoveAt(1);
            }
            grdData.Rows[1].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            grdData.DataBound -= Bound;
        }

        public void ChangeEntriesPerPageCount(int newCount)
        {
            countPerPage = newCount;
            viewState["CountPerPage"] = newCount;
            LoadData();
        }

        public void UpdatePageButtonStates(Button btnPrev, Button btnNext)
        {
            if (!HasPreviousPage) { btnPrev.Enabled = false; }
            else { btnPrev.Enabled = true; }
            if (!HasNextPage) { btnNext.Enabled = false; }
            else { btnNext.Enabled = true; }
        }

        public int RefreshPageCount(List<string> args, string querySql)
        {
            Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
            return (int)(decimal)db.RunQueryScalar($"SELECT CEIL(COUNT({dataColumns[0]})/{countPerPage}){baseSql} {querySql}", args.ToArray());
        }

        public List<string> GetSelectedEntries(int columnIndex)
        {
            List<string> data = new List<string>();
            for (int i = 1; i < grdData.Rows.Count; i++)
            {
                if (grdData.Rows[i].Cells[0].Controls.Count == 0) { return data; } // No Entries found was set
                bool chk = ((CheckBox)grdData.Rows[i].Cells[0].Controls[0]).Checked;
                TableCell cell = grdData.Rows[i].Cells[columnIndex + 1];
                string dataEntry = null;
                if (cell.Controls.Count > 0)
                {
                    if (cell.Controls[0].GetType() == typeof(LinkButton))
                    {
                        dataEntry = ((LinkButton)grdData.Rows[i].Cells[columnIndex + 1].Controls[0]).Text;
                    }
                }
                else
                {
                    dataEntry = cell.Text == "&nbsp;" ? "" : cell.Text;
                }
                if (chk == true)
                {
                    data.Add(dataEntry);
                }
            }
            return data;
        }

        public void FillFilterTextboxInGrid()
        {
            int cellCount = grdData.Rows[0].Cells.Count;
            for (int j = 1; j < cellCount; j++)
            {
                TextBox textBox = new TextBox();
                textBox.ID = "txtFilter" + (j - 1).ToString();
                textBox.Attributes.Add("style", "width: -webkit-fill-available;");
                textBox.TextChanged += FilterTextChanged;
                textBox.Attributes.Add("placeholder", "Filter");
                textBox.AutoPostBack = true;
                textBox.EnableViewState = true;
                grdData.Rows[0].Cells[j].Controls.Add(textBox);
            }
        }

        private void FilterTextChanged(object sender, EventArgs e)
        {
            if (OnFilterChanged != null) { OnFilterChanged(sender, e); }
            LoadData();
        }

        public void FillContentControlsInGrid()
        {
            for (int i = 0; i < grdData.Rows.Count; i++)
            {
                if (grdData.Rows[i].Cells[0].Controls.Count == 0) { return; } // No Entries found was set
                grdData.Rows[i].Cells[0].Controls.RemoveAt(0);
                if (i > 0)
                {
                    CheckBox chkSelRow = new CheckBox();
                    chkSelRow.ID = "chk" + i.ToString();
                    chkSelRow.Attributes.Add("class", "chkRow");
                    grdData.Rows[i].Cells[0].Controls.Add(chkSelRow);

                    string email = grdData.Rows[i].Cells[1].Text;
                    LinkButton lbGetDetails = new LinkButton();
                    lbGetDetails.ID = "lbGetDetails" + i.ToString();
                    lbGetDetails.Click += DetailsButtonClicked;
                    lbGetDetails.Text = email;
                    grdData.Rows[i].Cells[1].Controls.Add(lbGetDetails);
                }
                else
                {
                    CheckBox chkSelRow = new CheckBox();
                    chkSelRow.ID = "chk" + i.ToString();
                    chkSelRow.Attributes.Add("class", "chkRowAll");
                    grdData.Rows[i].Cells[0].Controls.Add(chkSelRow);
                }
            }

            if (grdData.Rows.Count == 2 && grdData.Rows[1].Cells[0].Text == HttpUtility.HtmlEncode(noEntriesFoundMsg))
            {
                grdData.Rows[1].Cells.RemoveAt(0);
                grdData.Rows[1].Cells[0].ColumnSpan = grdData.Rows[1].Cells.Count + 1;
                grdData.Rows[1].Cells[0].Text = HttpUtility.HtmlEncode(noEntriesFoundMsg);
                int totalCount = grdData.Rows[1].Cells.Count;
                for (int i = 1; i < totalCount; i++)
                {
                    grdData.Rows[1].Cells.RemoveAt(1);
                }
                grdData.Rows[1].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
        }

        private void DetailsButtonClicked(object sender, EventArgs e)
        {
            OnLinkButtonClick(sender, e);
        }

        public void NextPage()
        {
            if (!HasNextPage) { return; }
            viewState["CurrentPage"] = (int)viewState["CurrentPage"] + 1;
            LoadData();
        }

        public void PreviousPage()
        {
            if (!HasPreviousPage) { return; }
            viewState["CurrentPage"] = (int)viewState["CurrentPage"] - 1;
            LoadData();
        }

        public void LoadData()
        {
            List<SQLParamQueryPart> queries;
            List<string> args;
            List<TextBoxContent> filterContent = GetFilters(out queries);
            string querySql = FiltersToWhereQuery(queries, out args);
            Database db = new Database(WebConfigurationManager.ConnectionStrings["AppDbInt"].ConnectionString);
            DataTable dt = db.RunQuery($"SELECT {dataColumns}{baseSql}{querySql} LIMIT {(int)viewState["CurrentPage"] * countPerPage}, {countPerPage}", args.ToArray());
            DataRow dr = dt.NewRow();
            dt.Rows.InsertAt(dr, 0);
            BindDataToTable(dt);

            FillFilterTextboxInGrid();
            RefillFilterTextboxContent(filterContent);
            FillContentControlsInGrid();
            int newPageCount = RefreshPageCount(args, querySql);
            viewState["PageCount"] = newPageCount;
        }

        private void RefillFilterTextboxContent(List<TextBoxContent> filterContent)
        {
            foreach (TableCell cell in grdData.Rows[0].Cells)
            {
                if (cell.Controls[0].GetType() != typeof(TextBox)) { continue; }
                TextBox tb = cell.Controls[0] as TextBox;
                foreach (TextBoxContent data in filterContent)
                {
                    if (tb.ID == data.ID)
                    {
                        tb.Text = data.Content;
                        break;
                    }
                }
            }
        }

        private string FiltersToWhereQuery(List<SQLParamQueryPart> queries, out List<string> args)
        {
            string querySql = "";
            args = new List<string>();
            if (queries.Count > 0)
            {
                if (!baseSql.Contains("WHERE")) { querySql += " WHERE "; }
                else { querySql += " AND "; }
                for (int i = 0; i < queries.Count; i++)
                {
                    querySql += queries[i].QueryTemplate;
                    if (i != queries.Count - 1) { querySql += "AND "; }
                    args.Add("%" + queries[i].Parameter + "%");
                }
            }
            return querySql;
        }

        private List<TextBoxContent> GetFilters(out List<SQLParamQueryPart> queries)
        {
            List<TextBoxContent> filterContent = new List<TextBoxContent>();
            queries = new List<SQLParamQueryPart>();
            System.Collections.IList cells = grdData.Rows[0].Cells;
            for (int i = 1; i < cells.Count; i++)
            {
                TableCell cell = (TableCell)cells[i];
                if (cell.Controls[0].GetType() != typeof(TextBox)) { continue; }
                TextBox tb = cell.Controls[0] as TextBox;
                filterContent.Add(new TextBoxContent(tb.ID, tb.Text));
                if (tb.Text != "")
                {
                    string queryCol = dataColumns[i - 1];
                    queries.Add(new SQLParamQueryPart(queryCol + " LIKE ? ", tb.Text));
                }
            }
            return filterContent;
        }
    }
}
