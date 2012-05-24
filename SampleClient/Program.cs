using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RestSharp;

namespace SampleClient
{
	class Program
	{
		static void Main( string[] args )
		{
			var apiBaseUrl = args[0];
			var username = args[1];
			var password = args[2];
			var client = new RestClient(apiBaseUrl)
			{
				Authenticator = new HttpBasicAuthenticator(username, password)
			};
			client.AddDefaultHeader( "Accept", "application/json" );

			Console.WriteLine( "API base: " + apiBaseUrl );

			Console.WriteLine();
			Console.WriteLine( "This sample fetches the first created case from the list of cases and prompts for a new title to give the entity.");
			Console.WriteLine( "The case will then be updated using a HTTP PUT.");
			Console.WriteLine( "Operations used in this sample: GET PUT");
			Console.WriteLine();
			Console.WriteLine( "Press any key to begin...");
			Console.ReadKey();

			var caseListRequest = new RestRequest("cases");
			var caseListResponse = client.Execute<ReferenceList>(caseListRequest);
			var firstCaseLink = caseListResponse.Data.Link.First();
			var caseRequest = new RestRequest(firstCaseLink.Href);
			var caseResponse = client.Execute<Case>(caseRequest);
			var firstCase = caseResponse.Data;

			Console.WriteLine( "       Href: " + firstCase.Href );
			Console.WriteLine( "   Revision: " + firstCase.Revision );
			Console.WriteLine( "    Case ID: " + firstCase.Id );
			Console.WriteLine( "      Title: " + firstCase.Title );
			Console.WriteLine( "Description: " + firstCase.Description );
			Console.WriteLine( "   Contract: " + ( ( firstCase.Contract != null ) ? firstCase.Contract.Title : String.Empty ) );
			Console.WriteLine( "       Unit: " + ( ( firstCase.Unit != null ) ? firstCase.Unit.Title : String.Empty ) );

			Console.Write( "Enter new title: " );
			firstCase.Title = Console.ReadLine();
			var updatedCaseResponse = client.Execute<Case>(
				new RestRequest(firstCase.Href, Method.PUT) { RequestFormat = DataFormat.Json }
				.AddHeader("If-Match", "\"" + firstCase.Revision + "\"")
				.AddBody(firstCase)
			); 
			
			Console.WriteLine( "Status for update: " + updatedCaseResponse.StatusCode );
			if ( updatedCaseResponse.StatusCode == HttpStatusCode.NoContent )
			{
				Console.WriteLine("New revision for updated entity: " + updatedCaseResponse.Headers.First( h => h.Name == "ETag" ).Value );
			}

			Console.WriteLine();
			Console.WriteLine( "Press any key to exit...");
			Console.ReadKey();
		}
	}

	public class ReferenceList
	{
		public List<Link> Link { get; set; }
	}
	public class Link
	{
		public string Href { get; set; }
		public string Title { get; set; }
	}
	public class Case
	{
		public string Href { get; set; }
		public int Revision { get; set; }
		public string Title { get; set; }
		public string Id { get; set; }
		public string Description { get; set; }
		public Link Contract { get; set; }
		public Link Unit { get; set; }
	}
}
