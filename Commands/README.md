Command Samples
===============

The command samples can be executed using the script called Execute-Commands.ps1.
The script requires cURL to exist in the path. cURL can be downloaded from [the cURL download page](http://curl.haxx.se/download.html).

When installed properly, you will get the following output when typing 'curl'<Enter> in your PowerShell prompt:

    PS C:\> curl
    curl: try 'curl --help' for more information
   
Available Commands
==================

In order to see the commands we provide in the API you can use an HTTP GET to the endpoint https://your-remotex-instance/api/commands.
We are planning to include some kind of "Commands Playground" in the API help site, but until we get there a good tip is to 
use our HTML formatter using the URL https://your-remotex-instance/api/commands?format=html.

API support
===========
Not quite sure how you should consume the API in your specific case? Please contact your RemoteX representative 
with your inquiries and we will do our best to respond to your needs.

About the scripts in this repo
==============================

We have put together some handy scripts that we use internally that we have made public for you to use!
Here is a brief explanation, but most of the details are explained in the scripts themselves.

Execute-Commands.ps1
--------------------

To use Execute-Commands.ps1 you will need to supply some arguments.
* ServiceUri - Refers to the URL which ends with '/api'
* Username and Password - The credentials to use when authenticating with the API

So for example you could send the CommandBatch [CreateCaseSample.xml](https://github.com/remotex/Samples/blob/master/Commands/CreateCaseSample.xml) to a local API instance using the following command line:

    PS C:\Samples> .\Execute-Commands.ps1 CreateCaseSample.xml -ServiceUri http://localhost:8100/api -Username user -Password pass -Debug
    Response written to C:\Samples\Commands\CreateCaseWithLogMessage_output.xml
    cURL trace output written to C:\Samples\Commands\CreateCaseWithLogMessage_trace.txt
    Successfully executed commands:
    Command: CreateCase
      Error:
       Parameter: CaseId
           Value: X1234
       Parameter: Title
           Value: Case title
       Parameter: Description
           Value: Case created using commands sample
    Affected items:
       cases/X1234 (Revision: 1) 
       
    Command: WriteToLog
      Error:
       Parameter: Text
           Value: This is the log message
    Affected items:
       log/cases/X1234 (Revision: )

Get-AvailableCommands.ps1
-------------------------

This script lists all commands that are available in a RemoteX Applications instance.
You can use its output to examine what the commands are and their parameters, but most important of all is how its output can be used with other scripts.
Pipe it to: 
* ..[Export-Functions.ps1](https://github.com/remotex/Samples/blob/master/Commands/Export-Functions.ps1) to get a complete Commands API client with parameter tab completion and command help
* ..[Export-ExportExcelWorkBook.ps1](https://github.com/remotex/Samples/blob/master/Commands/Export-ExportExcelWorkBook.ps1) to get an instance specific Excel work book that can be used to create bulk data import files

Export-Functions.ps1
--------------------

Calling this script will generate one PowerShell file per command. For example you will get a script called Write-ToLog.ps1.
Here is how you can create log messages from a CSV file using the generated script:

    PS C:\Samples> "Message","test message","another message" | sc messages.csv
    PS C:\Samples> Import-Csv messages.csv | %{ .\Write-ToLog.ps1 -target mylog -Text $_.Message } | .\Post-Commands.ps1
    Sending commands to http://localhost:8100/api using  (0,00Mb in size)
    Command batch execution took 0m 0s
    Successfully executed commands:
    
    Command: WriteToLog
      Error:
       Parameter: Category
           Value: Info
       Parameter: Text
           Value: test message
    Affected items:
       log/mylog (Revision: )
    
    Command: WriteToLog
      Error:
       Parameter: Category
           Value: Info
       Parameter: Text
           Value: another message
    Affected items:
       log/mylog (Revision: )

Export-ExcelWorkBook.ps1
------------------------

This script will generate an Excel work book for you to fill with as much data you want!
It will both generate columns for system fields and instance specific fields.
When your are done, you can use [Import-ExcelSheetToRemoteX.ps1](https://github.com/remotex/Samples/blob/master/Commands/Import-ExcelSheetToRemoteX.ps1) to import the data, sheet by sheet.


    C:\Samples> .\Get-AvailableCommands.ps1 -ServiceUri http://localhost:8100/api -Username user | .\Export-ExcelWorkBook.ps1 importtemplate.xlsx


Other useful tools
==================
[Convert-CsvToCommandBatch.ps1](https://github.com/remotex/Samples/blob/master/Commands/Convert-CsvToCommandBatch.ps1)

Converts CSV data to command batches, ready to be sent to the API using Execute-Commands.ps1 (see above)

[Import-ExcelSheetToRemoteX.ps1](https://github.com/remotex/Samples/blob/master/Commands/Import-ExcelSheetToRemoteX.ps1)

Wrapper around the above and Execute-Commands which targets an input Microsoft Excel file and saves it to CSV using 
the Excel.Application COM object. Very handy if you need to import massive amounts of data to RemoteX Applications
and wants to author it using Microsoft Excel.