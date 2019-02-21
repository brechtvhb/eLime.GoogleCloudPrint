using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eLime.GoogleCloudPrint.Example
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using (var printService = new GoogleCloudPrintService("eLime.GoogleCoudPrintTest"))
            {
                await printService.AuthorizeJsonStringAsync(@"<json string");
                //await printService.AuthorizeJsonFileAsync(@"<json path>");
                //await printService.AuthorizeP12Async("<service-account@email.com>", "<p12 path>", "<secret>");

                Console.WriteLine("Accepting invite...");

                var printerid = "<printerid>";

                //accept invite for printer that was shared with service account
                var response = await printService.ProcessInvite(printerid);

                //get a printer
                var printer = await printService.GetPrinter(printerid, new List<string> { "connectionStatus", "queuedJobsCount" });

                var cjt = @"
                {
                    ""version"":""1.0"",
                    ""print"":{
                        ""color"":{ ""vendor_id"":""psk:Color"",""type"":""STANDARD_COLOR""},
                        ""duplex"":{ ""type"":""LONG_EDGE""},
                        ""vendor_ticket_item"":[
                            {""id"":""psk:PageInputBin"",""value"":""epns200:Front1""},
                        ]
                    }
                }";

                //print document. Set print in color, duplex printing and paper tray 2 as source

                var printjob = printService.PrintDocument(printerid, "example.pdf", cjt, "http://www.africau.edu/images/default/sample.pdf");
            }
        }
    }
}