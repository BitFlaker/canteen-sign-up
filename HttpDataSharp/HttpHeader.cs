using System;
using System.Text;

namespace HttpData
{
    public class HttpHeader
    {
        public string Name { get; }
        public string Value { get; }

        public HttpHeader(string name, string value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static HttpHeader Parse(string line)
        {
            int separation = line.IndexOf(':');
            string name = line.Substring(0, separation);
            string value = line.Substring(separation + 1).TrimStart();
            return new HttpHeader(name, value);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(": ");
            sb.Append(Value);
            return sb.ToString();
        }
    }
}