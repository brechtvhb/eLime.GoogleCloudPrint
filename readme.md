# GoogleCloudPrint for C&#35;

**This repository is based on https://github.com/io7/GoogleCloudPrint** but has been extended with some additional features.

This library uses Google's OAuth2 library from https://www.nuget.org/packages/Google.Apis.Oauth2.v2/.
This enables you to print to a cloud enabled printer using a Service Account.

## Usage

1. Create a service API account at https://console.developers.google.com:
  Go to "Credentials", click "Create credentials" and select "Service account key".
2. Generate a P12 key or a json key based on the service account you just made and export it.
3. Place this file in your project folder and ensure the file properties are set to "Content" and "Copy if newer". If you wish to you can also store the json file as a string in the database and authenticate using GoogleCloudPrintService.AuthorizeJsonStringAsync
4. Go to https://console.developers.google.com/iam-admin/serviceaccounts/ and save the email address of the service account that was generated for your service account. You need to share your printer with this email address in order for the service account to be able to print to your printer.
5. Go to https://www.google.com/cloudprint/#printers , select your printer and click the share button on top. Use the email address you copied in Step 4. 
6. Now click on the details button and then open the "advanced details" panel. Save the printer ID.
6. There are two ways to get your service account to accept the share invitation:
    
    ##### Using this library:

    1.  Run the following code in a new console program:
    
        ```csharp
		using (var printService = new GoogleCloudPrintService("eLime.GoogleCoudPrintTest"))
		{
			await printService.AuthorizeJsonStringAsync(@"<json string>");
			Console.WriteLine("Accepting invite...");
			
			var printerid = "<Printer ID from Step 6>";
			var response = printService.ProcessInvite(printerid);

			Console.WriteLine($"Result was: {response.success} {response.message}");
		}
                
        Console.ReadLine();
        ```    

    ##### Creating a Google Private Group:

    1. Create a Google Private Group (can be done within Google Apps for Business) which includes a normal e-mail and the service account's email. Make your own email an owner, and the service account email a member.
    2. Share your printer with the group's email, then log in with your own account and accept the share request (on behalf of the entire group). Your service account now has access to the printer!
    
7. Get the properties of a printer (The "source" parameter is just a name for your application, for example "eLime.GoogleCoudPrintTest"):

    ```csharp
		using (var printService = new GoogleCloudPrintService("eLime.GoogleCoudPrintTest"))
		{
			await printService.AuthorizeJsonStringAsync(@"<json string>");
			var printerid = "<Printer ID from Step 6>";
			var printer = await printService.GetPrinter(printerid, new List<string> { "connectionStatus", "queuedJobsCount" });
		}
    ```
	
Note that the capabilities are stored in a Jobject for now. I can create POCO classes for this if there would be need for it. In my application did not need this.

8. Print a document	and setting the print settings using a cloud job ticket
	```csharp
		using (var printService = new GoogleCloudPrintService("eLime.GoogleCoudPrintTest"))
		{
                await printService.AuthorizeJsonStringAsync(@"<json string>");
			
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
			
                var printerid = "<Printer ID from Step 6>";
                var printjob = printService.PrintDocument(printerid, "example", cjt, "http://www.africau.edu/images/default/sample.pdf");
		}
    ```
The cloud job ticket is a json sting here. If needed i could create poco classes for this too.	