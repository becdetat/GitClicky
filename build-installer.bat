@echo off

rem "********** Get the version **********"
	set /P version=Installer version: 
	if /i "%version%" == "" goto :help
	if not exist releases mkdir releases

rem "********** Include vs vars if not already included ***********"
	if not "%INCLUDED_VSVARS%" == "" goto :skipvsvars
		if exist "%VS100COMNTOOLS%vsvars32.bat" call "%VS100COMNTOOLS%vsvars32.bat"
		if exist "%VS110COMNTOOLS%vsvars32.bat" call "%VS110COMNTOOLS%vsvars32.bat"
		if exist "%VS120COMNTOOLS%vsvars32.bat" call "%VS120COMNTOOLS%vsvars32.bat"
		set INCLUDED_VSVARS=1
	:skipvsvars

rem "********** Build the application and installer **********"
	echo Release build:
	msbuild.exe /ToolsVersion:4.0 "src\gitclicky.sln" /p:configuration=Release

	echo Building installer:
	tools\nsis-3.0a2\makensis.exe /NOCD /V2 /Dversion=%version% GitClicky.nsi

rem "********** Append release info **********"
	for /F "tokens=*" %%i in ('tools\calcmd5.exe "releases\gitclicky-%version%.exe"') do set md5hash=%%i
	for %%j in (releases\gitclicky-%version%.exe) do @set installerSize=%%~zj
	echo %md5hash% gitclicky-%version%.exe %installerSize% >> releases\RELEASES
	goto :done

:help
	echo Make sure to include the version!

:done
	pause




