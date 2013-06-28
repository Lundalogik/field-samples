using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation.Runspaces;
using RemoteX.Libraries.NotificationLibrary.NotificationLibrary;

namespace Poshcriber
{
	public class Service : ISubscriberService
	{
		public bool SubscriptionEventNotification(SubscriptionNotification notification)
		{
			Runspace runspace = RunspaceFactory.CreateRunspace();
			runspace.ApartmentState = System.Threading.ApartmentState.STA;
			runspace.ThreadOptions = PSThreadOptions.UseCurrentThread;

			runspace.Open();

			Pipeline pipeline = runspace.CreatePipeline();
			var myCmd = new Command( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Invoke-Subscriber.ps1" ) );
			myCmd.Parameters.Add( new CommandParameter( "event", notification.NotificationType.ToString() ));
			myCmd.Parameters.Add( new CommandParameter( "href", notification.RelativeHref ));
			myCmd.Parameters.Add( new CommandParameter( "subscriptions", notification.SubscriptionArray ));
			myCmd.Parameters.Add( new CommandParameter( "changes", notification.ChangesArray ));
			pipeline.Commands.Add( myCmd );

			// Execute PowerShell script
			// Instead of implementing our own Host and HostUI we keep this extremely simple by 
			// catching everything to cope with HostExceptions and UnknownCommandExceptions etc.
			// The first will be thrown if someone tries to access unsupported (i.e. interactive) 
			// host features such as Read-Host and the latter will occur for all unsupported commands.
			// That can easily happen if a script is missing an import-module or just contains a mispelled command
			try
			{
				var result = pipeline.Invoke().FirstOrDefault();
				return result != null && result.BaseObject is bool && (bool)result.BaseObject;
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Exception caught when invoking powershell script: " + ex);
				return false;
			}
		}
    }

}
