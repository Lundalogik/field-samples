<#
.SYNOPSIS
	Exports available commands to a Microsoft Excel work book.
.DESCRIPTION
	Use with Get-AvailableCommands.ps1 to generate a work book that can be used to 
	enter command based data that later on can be imported to RemoteX using the 
	script Import-ExcelSheetToRemoteX.ps1.
#>
param(
	[parameter(mandatory=$true, helpmessage="Enter the path to the Excel work book to be created")]
	$path,
	[parameter(mandatory=$true, valuefrompipeline=$true, helpmessage="The commands to be exported to the Excel work book")]
	$command
)
begin {
	Write-Progress -Activity "Initiating" -Status "Starting Excel..." -PercentComplete 0

	# Need some Win32 functionality to get the PID of the Excel app
	Add-Type -TypeDefinition @"
	using System;
	using System.Runtime.InteropServices;

	public static class Win32Api
	{
	[System.Runtime.InteropServices.DllImportAttribute( "User32.dll", EntryPoint =  "GetWindowThreadProcessId" )]
	public static extern int GetWindowThreadProcessId ( [System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, out int lpdwProcessId );

	[DllImport("User32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
	}
"@
	$xl = New-Object -com "Excel.Application" 
	$excelPid = [IntPtr]::Zero
	[Win32Api]::GetWindowThreadProcessId( $xl.HWND, [ref] $excelPid ) | Out-Null

	# Workaround for "Old type library..." bug for non-US cultures
	$thread = [System.Threading.Thread]::CurrentThread
	$currentCulture = $thread.CurrentCulture
	$thread.CurrentCulture = New-Object System.Globalization.CultureInfo("en-US")

	$xl.Application.Interactive = $false
	$wb=$xl.workbooks.add() 
	$xl.displayalerts=$False 

	$sheets = @()
	Write-Progress -Activity "Initiating" -Status "Starting Excel..." -PercentComplete 100
}
process {
	if( !$command ) { continue }
	if( $command.CommandName -cmatch "[A-Z][^A-Z]+$" ) {
		$noun = $matches[0]
	} else {
		$noun = $command.CommandName
	}

	$sheets += New-Object psobject -Property @{ `
		"SheetName" = $command.CommandName; `
		"Noun" = $noun; `
		"Columns" = $command.Parameters.Parameter `
	}
}
end {
	if( $sheets ) {
		$xlWorksheet = -4167
		$count = ( $sheets | measure).Count
		$firstSheet = $wb.Sheets.item(1)
		$totalItems = ( $sheets | %{ $_.Columns } | measure ).Count
		$itemNumber = 1
		$sheets | group Noun | sort Name | select -ExpandProperty Group | %{
			Write-Progress -Activity "Creating work sheets" -Status "Creating $($_.SheetName)" -PercentComplete (100 * ($itemNumber / $totalItems ))
			$wb.Sheets.Add() | Out-Null
			$sheet = $wb.ActiveSheet
			$sheetName = $_.SheetName
			$sheet.Name = $sheetName
			$cells = $sheet.Cells
			$columnIndex = 1
			$_.Columns | %{
				Write-Progress -Activity "Creating work sheets" -Status "Creating $sheetName" -CurrentOperation "Adding column $($_.ParameterName)" -PercentComplete (100 * ($itemNumber / $totalItems ))
				$cells.Item(1, $columnIndex) = "[{0}]" -f $_.ParameterName
				$cells.Item(1, $columnIndex++).AddComment( $_.Description ) | Out-Null
				$itemNumber++
			}
		}
		
		Write-Progress -Activity "Creating work sheets" -Status "Saving work book" -PercentComplete 0
		# Save and close the file
		$xlsxFormat = 51
		$wb.SaveAs($path, $xlsxFormat, $null, $null, $false, $false, $false)
		Write-Progress -Activity "Creating work sheets" -Status "Saving work book" -PercentComplete 30
		$wb.close($false)
	}

	Write-Progress -Activity "Creating work sheets" -Status "Saving work book" -PercentComplete 60
	$xl.quit() 
	kill -Id $excelPid

	$thread.CurrentCulture = $currentCulture
	Write-Progress -Activity "Creating work sheets" -Status "Saving work book" -PercentComplete 100
}