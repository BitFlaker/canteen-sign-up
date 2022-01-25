using Htlvb.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HttpData
{
    public class HttpRequestMessage
    {
        public HttpRequestMessage(string statusLine, List<HttpHeader> headers, string body) : this(statusLine, headers)
        {
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public HttpRequestMessage(string statusLine, List<HttpHeader> headers)
        {
            StatusLine = statusLine ?? throw new ArgumentNullException(nameof(statusLine));
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
        }

        public string StatusLine { get; }
        public List<HttpHeader> Headers { get; }
        public string Body { get; }

        public static HttpRequestMessage ReadFrom(Stream stream)
        {
            HttpStreamReader requestReader = new HttpStreamReader(stream);
            string statusLine = requestReader.ReadLine();
            List<HttpHeader> headers = ReadHeaders(requestReader);
            HttpHeader contentLengthHeader = headers.Find(httpHeader => httpHeader.Name.Equals("Content-Length", StringComparison.InvariantCultureIgnoreCase));
            if (contentLengthHeader == null) { return new HttpRequestMessage(statusLine, headers); }
            if (!int.TryParse(contentLengthHeader.Value, out int contentLength)) { throw new Exception("Can't parse content length header as int"); }
            requestReader.Encoding = Encoding.UTF8;    // TODO get from Content-Type header
            string body = requestReader.ReadBytesAsText(contentLength);
            return new HttpRequestMessage(statusLine, headers, body);
        }

        private static List<HttpHeader> ReadHeaders(HttpStreamReader requestReader)
        {
            List<HttpHeader> result = new List<HttpHeader>();
            while (true)
            {
                string line = requestReader.ReadLine();
                if (line == "")
                {
                    break;
                }
                HttpHeader header = HttpHeader.Parse(line);
                result.Add(header);
            }
            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(StatusLine);
            foreach (HttpHeader header in Headers)
            {
                sb.AppendLine(header.ToString());
            }
            sb.AppendLine();
            sb.AppendLine(Body);
            return sb.ToString();
        }
    }
}
