using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicTables
{
    [Serializable]
    public class SelectedColumns
    {
        List<string> columns;
        List<string> columnNames;

        public SelectedColumns()
        {
            columns = new List<string>();
            columnNames = new List<string>();
        }

        public void Add(string sqlColumn, string columnName)
        {
            columns.Add(sqlColumn);
            columnNames.Add(columnName);
        }

        public void Remove(string sqlColumn)
        {
            int indx = columns.IndexOf(sqlColumn);
            columns.RemoveAt(indx);
            columnNames.RemoveAt(indx);
        }

        public int Count { get { return columns.Count; } }

        public string this[int index]
        {
            get { return columns[index]; }
        }

        public int this[string column]
        {
            get { return columns.IndexOf(column); }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < columns.Count; i++)
            {
                sb.Append($"{columns[i]} as '{columnNames[i]}'");
                if (i != columns.Count - 1) { sb.Append(", "); }
            }
            return sb.ToString();
        }
    }
}
