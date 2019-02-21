using System;
using System.Collections.Generic;

namespace eLime.GoogleCloudPrint.Models
{
    public class Printer
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string DefaultDisplayName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string Proxy { get; set; }

        public string CreateTime { get; set; }

        public string AccessTime { get; set; }

        public string UpdateTime { get; set; }

        public bool IsTosAccepted { get; set; }

        public IEnumerable<String> Tags { get; set; } = new List<string>();

        public Capabilities Capabilities { get; set; }

        public string CapsHash { get; set; }

        public string OwnerId { get; set; }

        public string OwnerName { get; set; }

        public IEnumerable<PrinterAccess> Access { get; set; } = new List<PrinterAccess>();

        public bool QuotaEnabled { get; set; }

        public bool DailyQuota { get; set; }

        public bool CurrentQuota { get; set; }

        public string GcpVersion { get; set; }

        public string NotificationChannel { get; set; }

        public string Manufacturer { get; set; }

        public Status? ConnectionStatus { get; set; }

        public Int32? QueuedJobsCount { get; set; }
    }
}

public enum Status
{
    Unknown,

    Online,

    Offline,

    Dormant
}