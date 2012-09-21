Command Samples
===============

The command samples can be executed using the script called Execute-Commands.ps1.
The script requires cURL to exist in the path. cURL can be downloaded from (http://curl.haxx.se/download.html).

When installed properly, you will get the following output when typing 'curl'<Enter> in your PowerShell prompt:

    PS C:\> curl
    curl: try 'curl --help' for more information
   
Execute-Commands.ps1
====================

To use Execute-Commands.ps1 you will need to supply some arguments.
* ServiceUri - Refers to the URL which ends with '/api'
* Username and Password - The credentials to use when authenticating with the API

So for example you could send the CommandBatch (CreateCaseSample.xml) to a local API instance using the following command line:

    PS C:\Samples> .\Execute-Commands.ps1 CreateCaseSample.xml -ServiceUri http://localhost:8100/api -Username user -Password pass
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


Available Commands
==================

In order to see the commands we provide in the API you can use an HTTP GET to the endpoint https://your-remotex-instance/api/commands.
We are planning to include some kind of "Commands Playground" in the API help site, but until we get there a good tip is to 
use our HTML formatter using the URL https://your-remotex-instance/api/commands?format=html.

API support
===========
Not quite sure how you should consume the API in your specific case? Please contact your RemoteX representative 
with your inquiries and we will do our best to respond to your needs.

Other useful tools
==================
(https://github.com/remotex/Samples/blob/master/Commands/Convert-CsvToCommandBatch.ps1)

Converts CSV data to command batches, ready to be sent to the API using Execute-Commands.ps1 (see above)

(https://github.com/remotex/Samples/blob/master/Commands/Import-ExcelSheetToRemoteX.ps1)

Wrapper around the above and Execute-Commands which targets an input Microsoft Excel file and saves it to CSV using 
the Excel.Application COM object. Very handy if you need to import massive amounts of data to RemoteX Applications
and wants to author it using Microsoft Excel.