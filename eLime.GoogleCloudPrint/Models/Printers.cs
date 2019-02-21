using System.Collections.Generic;

namespace eLime.GoogleCloudPrint.Models
{
    public class PrintersHolder
    {
        public bool Success { get; set; }

        public List<Printer> Printers { get; set; } = new List<Printer>();
    }
}