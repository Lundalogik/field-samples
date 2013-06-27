<#
.SYNOPSIS
	Sample notification subscriber in PowerShell
.DESCRIPTION
	Uses parameters from notification event and appends the received event to a notification log file.

	Pro tip: You can use the Notification Cannon application to manually test this script without the 
	         need of a real Notification Service sending the notifications for you.
.PARAMETER event
	The notification event type. EntityAdd, EntityUpdate, EntityDelete, Management, etc.
.PARAMETER href
	The relative href of the entity that is the source of the event
.PARAMETER subscriptions
	Array of Subscription objects given as PSObjects to avoid need of assembly references.
	Each subscription has the properties Name, SubscriptionType and SubscriptionHref
.PARAMETER changes
	Array of NotificationChange objects given as PSObjects to avoid need of assembly references.
	Each change object has the properties Property, OldValue and NewValue
.RETURNS
	Boolean value. True will mark the notification as received and false will tell the
	notification service that the notification should be sent again at a later point in time.
#>
param( 
	[string] $event, 
	[string] $href, 
	[psobject[]] $subscriptions, 
	[psobject[]] $changes 
)

# set up the path to the log file, $env:Temp is always writable - right?
$notificationLog = join-path $env:TEMP "notifications.log"

# append the href + event type to the log as a CSV record
new-object psobject -property @{ "Href" = $href; "Event" = $event } | convertto-csv -notypeinformation | out-file $notificationlog -append

# append the list of subscriptions to the log as CSV record(s)
$subscriptions | convertto-csv -notypeinformation | out-file $notificationlog -append

# append the list of changes to the log as CSV record(s)
$changes | convertto-csv -notypeinformation | out-file $notificationlog -append

# return $true to mark the notification event as received
return $true