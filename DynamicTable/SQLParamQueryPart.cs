using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTables
{
    public class SQLParamQueryPart
    {
        public string QueryTemplate { get; private set; }
        public string Parameter { get; private set; }

        public SQLParamQueryPart(string queryTemplate, string parameter)
        {
            QueryTemplate = queryTemplate;
            Parameter = parameter;
        }
    }
}
