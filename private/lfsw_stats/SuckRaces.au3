#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_outfile=C:\inwork\SuckItem\SuckItem.exe
#AutoIt3Wrapper_Res_Comment=http://www.hiddensoft.com/autoit3/compiled.html
#AutoIt3Wrapper_Res_Description=AutoIt v3 Compiled Script
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****

#include <guiconstants.au3>
#include <file.au3>

#region Option, Declaration Variables, Includes
Opt("CaretCoordMode", 1)        ;1=absolute, 0=relative
Opt("ColorMode", 0)				;0 = Colors are defined as RGB 1 = Colors are defined as BGR (0xBBGGRR)
Opt("ExpandEnvStrings", 0)      ;0=don't expand, 1=do expand
Opt("ExpandVarStrings",	0) 		;Changes how literal strings and variable/macro ($ and @) symbols are interpreted 0=Default
Opt("FtpBinaryMode" , 1)		;Binary=1 AscII=0
Opt("GUICloseOnESC", 0)			; 0=Default
Opt("GUICoordMode",1)			; 0=relative to CTRL 1=absolute To Box(default) 2= Cell Positioning
Opt("GUIDataSeparatorChar","|") ; default "|"
Opt("GUIOnEventMode",0)			;Default =0
Opt("GUIResizeMode",0)			;Default=0 See Resize mode
Opt("MouseClickDelay", 10)      ;10 milliseconds
Opt("MouseClickDownDelay", 10)  ;10 milliseconds
Opt("MouseClickDragDelay", 250) ;250 milliseconds
Opt("MouseCoordMode",0)			;Default=0
Opt("MustDeclareVars", 0)       ;0=no, 1=require pre-declare
Opt("OnExitFunc","onexit") ;Default=OnAutoItExit
Opt("PixelCoordMode", 1)        ;1=absolute, 0=relative
Opt("RunErrorsFatal", 0)        ;1=fatal, 0=silent set @error
Opt("SendAttachMode", 0)        ;0=don't attach, 1=do attach
Opt("SendCapslockMode", 1)      ;1=store and restore, 0=don't
Opt("SendKeyDelay", 8)          ;5 milliseconds
Opt("SendKeyDownDelay", 1)      ;1 millisecond
Opt("TCPTimeout",100)			;Default=100
Opt("TrayAutoPause",0)			;Default=1
Opt("TrayIconDebug", 1)         ;0=no info, 1=debug line info
Opt("TrayIconHide", 1)          ;0=show, 1=hide tray icon
Opt("TrayMenuMode",0)			;2=User 1=Disable
Opt("TrayOnEventMode",0)		;Default=0
Opt("WinDetectHiddenText", 1)   ;0=don't detect, 1=do detect
Opt("WinSearchChildren", 1)     ;0=no, 1=search children also
Opt("WinTextMatchMode",1)		;Default=1 2=modeFast
Opt("WintitleMatchMode", 2)     ;1=start, 2=subStr, 3=exact, 4=...
Opt("WinWaitDelay", 250)        ;250 milliseconds
#endregion

Global $logfilenot = @ScriptDir&"\Race_NotFound.Log"
Global $logfile = @ScriptDir&"\Races_Found.Log"

Global $textFile = ""
Global $saveFileInterval = 0
Global $start = 2128522
Global $end = 2128555
Global $wrongCount = 0;

ProcessSetPriority(@AutoItPID,1)

;ProgressOn ( "Suck", "Suck2" , "subtext",-1,500,16)

If UBound($cmdline) > 1 Then
	$start = $cmdline[1]
	$end = $cmdline[2]
EndIf

suck()


func cleanup()

	FileDelete(@ScriptDir&"\Races\*.*")

EndFunc



Func suck()
	for $x = $start to $end
		;ProgressSet ( 0 , "Race Number "&$x)
		$file = "Race-"&$x&".html"
		$y = 0
		$count = 0
	    InetGet("http://www.lfsworld.net/winloader.php?win=races&whichTab=detail&raceId="&$x, @ScriptDir&"\Races\"&$file,1,1)
		while @InetGetActive
			sleep(500)
			if $count > 80 Then
				InetGet("abort")
				ContinueLoop 2
			EndIf
			$count += 1
		wend
		if @InetGetBytesRead <  6100 Then ;1731 == invalide, 6000 == empty
			FileDelete(@ScriptDir&"\Races\"&$file)
			$wrongCount += 1
			ConsoleWrite($wrongCount&@crlf)
		Else
			parse(@ScriptDir&"\Races\"&$file,$x)
			FileDelete(@ScriptDir&"\Races\"&$file)
		EndIf
		if $saveFileInterval < $x Then
			$saveFileInterval = $x+50
			saveFile()
		EndIf
		if $wrongCount > 10 Then
			ConsoleWrite("exited cause wrong count"&@CRLF)
			ExitLoop 1
		EndIf
	Next
	saveFile()
EndFunc

Func parse($file,$guid)
	$fread=FileRead($file)
	if @error Then
		ConsoleWrite($wrongCount&@crlf)
		$wrongCount += 1
		return
	EndIf
	$lines = StringSplit($fread,@LF,1)
	if UBound($lines) > 51 Then
		$textFile &= $guid & " | " & $lines[18]&@CRLF
		$wrongCount = 0
		ConsoleWrite($wrongCount&@crlf)
		return true
	EndIf
EndFunc


Func saveFile()
	ConsoleWrite("Saving..."&@CRLF)
	FileWrite(@ScriptDir&"\Races\RacesData_"&$start&"-"&$end&".txt",$textFile)
	$textFile = ""
EndFunc

