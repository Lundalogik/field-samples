using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;

namespace CommandsClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var apiBaseUrl = args[0];
            var username = args[1];
            var password = args[2];
            var client = new RestClient(apiBaseUrl)
                {
                    Authenticator = new HttpBasicAuthenticator(username, password)
                };
            client.AddDefaultHeader("Accept", "application/json");

            Console.WriteLine("API base: " + apiBaseUrl);

            Console.WriteLine();
            Console.WriteLine("This sample will put a series of commands to the commands endpoint.");
            Console.WriteLine("The commands will update a Remotex workorder status, update title and creata an usagequantity/artikle row");
            Console.WriteLine("Operations used in this sample: POST");
            Console.WriteLine();
            Console.WriteLine("Press any key to begin...");
            Console.ReadKey();

            // The case "cases/130610-0027", resource "resources/130225-0011" and workorder reference "workorders/130610-0027-1" 
            // are just example href based on the command xml files.
            var workOrderStatus = Commands.UpsertWorkOrder("workorders/130610-0027-1")
                                                .Parameter("State", "NotStarted"); // NotStarted/Started/Finished
          
            var workOrderTitleAndDescription = Commands.UpsertWorkOrder("workorders/130610-0027-1")
                    .Parameter("Title", "New title")
                    .Parameter("Description", "Updated using commands sample");

            var addUsageQuantity = Commands.CreateUsageQuantity("cases/130610-0027", "resources/130225-0011",
                                                                new Dictionary<string, string>
                                                                    {
                                                                       { "Activity", "System.Picklist.UsageQuantity.Activity.Felsökning" },
                                                                       { "Description", "Demo lägga till text"}
                                                                    });

            var batch = CommandBatch.Create(new[] { workOrderStatus, workOrderTitleAndDescription, addUsageQuantity });

            var request = new RestRequest("commands", Method.POST) {RequestFormat = DataFormat.Json}
                .AddBody(batch);

            var commandResponse = client.Execute<CommandBatch>(request);

            Console.WriteLine("Status for update: " + commandResponse.StatusCode);
            if (commandResponse.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("Command successfull");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}