using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestSharp;

namespace SubmitCaseAndWorkorder
{
	class Program
	{
		static void Main( string[] args )
		{
			var apiBaseUrl = args[0];
			var username = args[1];
			var password = args[2];

			Console.WriteLine( "API base: " + apiBaseUrl );
			Console.WriteLine();
			Console.WriteLine( "This sample submits a new case with a workorder with no resource assigned.");
			Console.WriteLine( "Operations used in this sample: GET POST");
			Console.WriteLine();
			Console.WriteLine( "Press any key to begin...");
			Console.ReadKey();

			// Set up the HTTP client
			var client = new RestClient( apiBaseUrl )
			{
				Authenticator = new HttpBasicAuthenticator( username, password )
			};
			client.AddDefaultHeader( "Accept", "application/json" );

			// Get links to a Unit (and its Contract), which we will assign to our Case, from the units collection endpoint 
			var unit = client.GetLinkCollection( "units" ).FirstOrDefault();
			Link contract = null;
			if ( unit != null )
			{
				var unitResource = client.Get<Unit>(unit);
				if ( unitResource.Contract != null )
				{
					contract = unitResource.Contract;
				}
			}

			// Create the Case with some data
			var newCase = new Case {Href = "cases"};
			Console.Write( "Enter case title: " );
			newCase.Title = Console.ReadLine();
			Console.Write( "Enter case description: " );
			newCase.Description = Console.ReadLine();
			newCase.Unit = unit;
			newCase.Contract = contract;

			// Print out details of Contract and Unit
			if ( unit != null )
			{
				Console.WriteLine( "Setting unit '{0}' on case.", unit.Title );
			}
			if ( contract != null )
			{
				Console.WriteLine("Setting contract '{0}' on case.", contract.Title);
			}
	
			
			// Submit the Case to the cases collection
			var submittedCase = client.Post( newCase );

			// The submitted Case has now got a permanent link that we can use to reference the resource
			// We use this to add a workorder to the Case 
			var newWorkOrder = new WorkOrder { Href = "workorders", Case = submittedCase, Title = submittedCase.Title, Description = submittedCase.Description };
			// Submit the Work Order to the workorders collection
			var submittedWorkOrder = client.Post(newWorkOrder);


			// Print out the details
			Console.WriteLine("=== Case details ===");
			Console.WriteLine( "       Href: " + submittedCase.Href );
			Console.WriteLine( "   Revision: " + submittedCase.Revision );
			Console.WriteLine( "    Case ID: " + submittedCase.Id );
			Console.WriteLine( "      Title: " + submittedCase.Title );
			Console.WriteLine( "Description: " + submittedCase.Description );
			Console.WriteLine( "   Contract: " + ( ( submittedCase.Contract != null ) ? submittedCase.Contract.Title : String.Empty ) );
			Console.WriteLine( "       Unit: " + ( ( submittedCase.Unit != null ) ? submittedCase.Unit.Title : String.Empty ) );

			Console.WriteLine( "=== Work order details ===" );
			Console.WriteLine( "         Href: " + submittedWorkOrder.Href );
			Console.WriteLine( "     Revision: " + submittedWorkOrder.Revision );
			Console.WriteLine( "Work order ID: " + submittedWorkOrder.Id );
			Console.WriteLine( "        Title: " + submittedWorkOrder.Title );
			Console.WriteLine( "  Description: " + submittedWorkOrder.Description );

			Console.WriteLine();
			Console.WriteLine( "Press any key to exit...");
			Console.ReadKey();
		}
	}

	public class ReferenceList
	{
		public List<Link> Link { get; set; }
	}
	public class Link : ILocatable
	{
		public string Href { get; set; }
		public string Title { get; set; }
	}
	public class Case : Link
	{
		public int Revision { get; set; }
		public string Id { get; set; }
		public string Description { get; set; }
		public Link Contract { get; set; }
		public Link Unit { get; set; }
		public DateTime Created { get; set; }
		public Link CreatedBy { get; set; }
		public DateTime Updated { get; set; }
		public Link UpdatedBy { get; set; }
	}
	public class Unit : Link
	{
		public int Revision { get; set; }
		public string Id { get; set; }
		public string Description { get; set; }
		public Link Contract { get; set; }
		public DateTime Created { get; set; }
		public Link CreatedBy { get; set; }
		public DateTime Updated { get; set; }
		public Link UpdatedBy { get; set; }
	}
	public class WorkOrder : Link
	{
		public int Revision { get; set; }
		public string Id { get; set; }
		public string Description { get; set; }
		public Link Case { get; set; }
		public DateTime Created { get; set; }
		public Link CreatedBy { get; set; }
		public DateTime Updated { get; set; }
		public Link UpdatedBy { get; set; }
	}

	public interface ILocatable
	{
		string Href { get; set; }
	}

	public static class RestClientExtensions
	{
		public static TResource Post<TResource>( this RestClient client, TResource apiResource ) where TResource : ILocatable, new()
		{
			var response = client.Execute<TResource>( new RestRequest( apiResource.Href, Method.POST ) { RequestFormat = DataFormat.Json }.AddBody(apiResource) );
			if ( response.ResponseStatus == ResponseStatus.Completed && response.StatusCode == HttpStatusCode.Created )
				return response.Data;
			if ( response.ResponseStatus == ResponseStatus.Error )
				throw new ApplicationException( response.ErrorMessage, response.ErrorException );
			return default( TResource );
		}

		public static IEnumerable<Link> GetLinkCollection( this RestClient client, string collectionResource )
		{
			var response = client.Execute<ReferenceList>( new RestRequest( collectionResource, Method.GET ) { RequestFormat = DataFormat.Json } );
			if ( response.ResponseStatus == ResponseStatus.Completed && response.StatusCode == HttpStatusCode.OK )
				return response.Data.Link;
			if ( response.ResponseStatus == ResponseStatus.Error )
				throw new ApplicationException( response.ErrorMessage, response.ErrorException );
			return Enumerable.Empty<Link>();
		}

		public static TResource Get<TResource>( this RestClient client, Link resource ) where TResource : new()
		{
			var response = client.Get<TResource>( new RestRequest(resource.Href) { RequestFormat = DataFormat.Json } );
			if ( response.ResponseStatus == ResponseStatus.Completed && response.StatusCode == HttpStatusCode.OK )
				return response.Data;
			if( response.ResponseStatus == ResponseStatus.Error )
				throw new ApplicationException( response.ErrorMessage, response.ErrorException );
			return default( TResource );
		}
	}

}
