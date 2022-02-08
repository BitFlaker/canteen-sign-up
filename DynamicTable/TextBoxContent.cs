using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTables
{
    public class TextBoxContent
    {
        public string ID { get; private set; }
        public string Content { get; private set; }

        public TextBoxContent(string id, string content)
        {
            ID = id;
            Content = content;
        }
    }
}
