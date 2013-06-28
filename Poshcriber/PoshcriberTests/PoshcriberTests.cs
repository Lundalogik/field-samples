using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Poshcriber;
using RemoteX.Libraries.NotificationLibrary.NotificationLibrary;

namespace PoshcriberTests
{
	[TestFixture]
	public class PoshcriberTests
	{
		[SetUp]
		public void Setup()
		{
			TraceLog = new TestTraceListener();
			Trace.Listeners.Add(TraceLog);
		}

		protected TestTraceListener TraceLog { get; set; }

		[Test]
		public void CanCallValidPowershellSubscriberScriptOnNotificationEvent()
		{
			CreateScript();
			var subscriber = new Service();
			var notification = SetupExpectedNotification();

			var result = subscriber.SubscriptionEventNotification(notification);

			Assert.AreEqual(ReadFile("expectedhref.txt"), ReadFile("actualhref.txt"));
			Assert.AreEqual(ReadFile("expectedevent.txt"), ReadFile("actualevent.txt"));
			Assert.AreEqual(ReadFile("expectedsubscriptions.txt"), ReadFile("actualsubscriptions.txt"));
			Assert.AreEqual(ReadFile("expectedchanges.txt"), ReadFile("actualchanges.txt"));
			Assert.IsTrue(result);
		}

		[Test]
		public void ReturningFalseFromScriptReturnsFalseOnReceviedNotificationEvent()
		{
			CreateScript( result: false);
			var subscriber = new Service();
			var notification = SetupExpectedNotification();

			var result = subscriber.SubscriptionEventNotification(notification);

			Assert.IsFalse(result);
		}

		[Test]
		public void ReadHostInScriptReturnsFalseOnReceviedNotificationEventAndWritesToTrace()
		{
			CreateScriptWithReadHost();
			var subscriber = new Service();
			var notification = SetupExpectedNotification();

			var result = subscriber.SubscriptionEventNotification(notification);

			Assert.IsFalse(result);
			Assert.AreEqual( 1, TraceLog.Traces.Count() );
		}

		[Test]
		public void UnknownCommandInScriptReturnsFalseOnReceviedNotificationEventAndWritesToTrace()
		{
			CreateScriptWithUnknownCommand();
			var subscriber = new Service();
			var notification = SetupExpectedNotification();

			var result = subscriber.SubscriptionEventNotification(notification);

			Assert.IsFalse(result);
			Assert.AreEqual(1, TraceLog.Traces.Count());
		}

		private static SubscriptionNotification SetupExpectedNotification()
		{
			var subscription = new Subscription {Name = "a|b|c", SubscriptionHref = "http://my.url/svc", SubscriptionType = NotificationType.EntityUpdate};
			var change = new NotificationChange { Property = "title", NewValue = "new", OldValue = "old"} ;
			var notification = new SubscriptionNotification {RelativeHref = "foo/bar", SubscriptionArray = new[] {subscription}, ChangesArray = new[] { change }};
			WriteFile("expectedevent.txt", notification.NotificationType.ToString());
			WriteFile("expectedhref.txt", notification.RelativeHref);
			var subscriptionBuilder = new StringBuilder();
			subscriptionBuilder.AppendLine("\"Name\",\"SubscriptionType\",\"SubscriptionHref\"");
			subscriptionBuilder.AppendLine("\"" + subscription.Name + "\",\"" + subscription.SubscriptionType + "\",\"" + subscription.SubscriptionHref + "\"");
			WriteFile("expectedsubscriptions.txt", subscriptionBuilder.ToString().Trim());
			var changeBuilder = new StringBuilder();
			changeBuilder.AppendLine("\"Property\",\"OldValue\",\"NewValue\"");
			changeBuilder.AppendLine("\"" + change.Property + "\",\"" + change.OldValue + "\",\"" + change.NewValue + "\"");
			WriteFile("expectedchanges.txt", changeBuilder.ToString().Trim());
			return notification;
		}

		static void CreateScript( bool result = true )
		{
			var psSubscriber =
				@"
param( [string] $event, [string] $href, [psobject[]] $subscriptions, [psobject[]] $changes )
sc actualhref.txt $href
sc actualevent.txt $event
$subscriptions | convertto-csv -notypeinformation | out-file actualsubscriptions.txt
$changes | convertto-csv -notypeinformation | out-file actualchanges.txt
$" + result.ToString( CultureInfo.InvariantCulture );
			WriteFile("Invoke-Subscriber.ps1", psSubscriber);
		}

		static void CreateScriptWithReadHost()
		{
			const string psSubscriber = @"
param( [string] $event, [string] $href, [psobject[]] $subscriptions, [psobject[]] $changes )
sc actualhref.txt $href
sc actualevent.txt $event
$subscriptions | convertto-csv -notypeinformation | out-file actualsubscriptions.txt
$changes | convertto-csv -notypeinformation | out-file actualchanges.txt
read-host";
			WriteFile("Invoke-Subscriber.ps1", psSubscriber);
		}

		static void CreateScriptWithUnknownCommand()
		{
			const string psSubscriber = @"
push-thingy -unknown
";
			WriteFile("Invoke-Subscriber.ps1", psSubscriber);
		}

		static void WriteFile(string path, string content)
		{
			using(var fw = new StreamWriter(path, false, Encoding.UTF8))
				fw.WriteLine(content);
		}

		static string ReadFile(string path)
		{
			using(var fw = new StreamReader(path, true))
				return fw.ReadToEnd();
		}
	}

	public class TestTraceListener : TraceListener
	{
		private readonly List<string> _traces = new List<string>();

		public override void Write(string message)
		{
			_traces.Add(message);
		}

		public override void WriteLine(string message)
		{
			_traces.Add( message );
		}

		public IEnumerable<string> Traces { get { return _traces; } } 
	}
}
