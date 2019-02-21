using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace eLime.GoogleCloudPrint.Models
{
    public class PrintJobs
    {
        public bool Success { get; set; }

        public IEnumerable<PrintJob> Jobs { get; set; } = new List<PrintJob>();
    }

    public class PrintJob
    {
        public string Id { get; set; }
        public String Printerid { get; set; }
        public String PrinterName { get; set; }
        public String Title { get; set; }
        public String ContentType { get; set; }
        public Int64 CreateTime { get; set; }
        public Int64 UpdateTime { get; set; }

        public SemanticState SemanticState { get; set; }
        public String Status { get; set; }

        public Int32 NumberOfPages { get; set; }
    }

    public class SemanticState
    {
        [JsonProperty("delivery_attempts")]
        public Int32 DeliveryAttempts { get; set; }

        public string Version { get; set; }
        public State State { get; set; }
    }

    public class State
    {
        public string Type { get; set; }
    }
}