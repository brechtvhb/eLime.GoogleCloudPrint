using System;
using System.Collections.Generic;
using System.Text;

namespace eLime.GoogleCloudPrint
{
    internal class PostData
    {
        private const string Crlf = "\r\n";

        internal string Boundary { get; set; }

        internal List<PostDataParamaeter> Parameters { get; set; }

        internal PostData()
        {
            // Get boundary, default is --AaB03x
            Boundary = "----CloudPrintFormBoundary-" + DateTime.UtcNow.Ticks;

            // The set of parameters
            Parameters = new List<PostDataParamaeter>();
        }

        internal string GetRawPostData()
        {
            var sb = new StringBuilder();
            foreach (var p in Parameters)
            {
                sb.Append("--" + Boundary).Append(Crlf);

                if (p.Type == PostDataParamType.File)
                {
                    sb.Append($"Content-Disposition: form-data; name=\"{p.Name}\"; filename=\"{p.FileName}\"").Append(Crlf);
                    sb.Append("Content-Type: ").Append(p.FileMimeType).Append(Crlf);
                    sb.Append("Content-Transfer-Encoding: base64").Append(Crlf);
                    sb.Append("").Append(Crlf);
                    sb.Append(p.Value).Append(Crlf);
                }
                else
                {
                    sb.Append($"Content-Disposition: form-data; name=\"{p.Name}\"").Append(Crlf);
                    sb.Append("").Append(Crlf);
                    sb.Append(p.Value).Append(Crlf);
                }
            }

            sb.Append("--" + Boundary + "--").Append(Crlf);

            return sb.ToString();
        }
    }

    internal enum PostDataParamType
    {
        Field,
        File
    }

    internal class PostDataParamaeter
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string FileMimeType { get; set; }
        public string Value { get; set; }
        public PostDataParamType Type { get; set; }

        public PostDataParamaeter()
        {
            FileMimeType = "text/plain";
        }
    }
}