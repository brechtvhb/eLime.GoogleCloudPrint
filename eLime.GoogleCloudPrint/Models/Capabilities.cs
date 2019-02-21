using Newtonsoft.Json.Linq;
using System;

namespace eLime.GoogleCloudPrint.Models
{
    public class Capabilities
    {
        public JObject Printer { get; set; }
        public String Version { get; set; }
    }
}