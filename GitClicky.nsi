; NSIS script for the GitClicky installer
; Ben Scott - @bendetat, http://bendetat.com

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;; Configure and set up the installer ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; Elevated privs are required to register the extension
RequestExecutionLevel highest

!include FileFunc.nsh

Name GitClicky
OutFile releases\gitclicky-${version}.exe
!define UNINSTALL_REG_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\GitClicky"
InstallDir "$LOCALAPPDATA\bendetat\GitClicky\${version}"
ShowInstDetails show

; Version info
VIProductVersion ${version}.0
VIAddVersionKey ProductName GitClicky
VIAddVersionKey Comments "GitClicky installer"
VIAddVersionKey CompanyName "Software by Ben Pty Ltd"
VIAddVersionKey LegalCopyright "Ben Scott - @bendetat, http://bendetat.com"
VIAddVersionKey FileDescription "Context menu helper for getting public urls to files from GitHub and BitBucket - bendetat.com"
VIAddVersionKey FileVersion ${version}
VIAddVersionKey ProductVersion ${version}
VIAddVersionKey InternalName GitClicky
VIAddVersionKey LegalTrademarks "Ben Scott - @bendetat, http://bendetat.com"
VIAddVersionKey OriginalFilename gitclicky-${version}.exe

; Pages
Page instfiles

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;; Installation ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
Section ""

  SetOutPath "$INSTDIR"

  ; Copy the files
  File "src\GitClicky.Extension\bin\Release\*.exe"
  File "src\GitClicky.Extension\bin\Release\*.dll"

  ; Unregister then register the extension
  ExecWait "$INSTDIR\srm.exe uninstall GitClicky.Extension.dll -codebase"
  ExecWait "$INSTDIR\srm.exe install GitClicky.Extension.dll -codebase"

  ; Create uninstaller
  WriteUninstaller "$INSTDIR\uninstall.exe"

  ; Add uninstaller to registry
  WriteRegStr HKLM "${UNINSTALL_REG_KEY}" "DisplayName" "GitClicky"
  WriteRegStr HKLM "${UNINSTALL_REG_KEY}" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
  WriteRegStr HKLM "${UNINSTALL_REG_KEY}" "Publisher" "Ben Scott - @bendetat"
  WriteRegStr HKLM "${UNINSTALL_REG_KEY}" "HelpLink" "https://github.com/bendetat/GitClicky/"
  WriteRegStr HKLM "${UNINSTALL_REG_KEY}" "DisplayVersion" "${version}"
  WriteRegDWORD HKLM "${UNINSTALL_REG_KEY}" "NoModify" 1
  WriteRegDWORD HKLM "${UNINSTALL_REG_KEY}" "NoRepair" 1
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $0 "0x%08X" $0
  WriteRegDWORD HKLM "${UNINSTALL_REG_KEY}" "EstimatedSize" "$0"

  ; Restart explorer
  nsRestartExplorer::nsRestartExplorer restart 10000
  Pop $1
  DetailPrint $1

SectionEnd ; end the section

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;; Uninstallation ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
Section Uninstall
 ExecWait "$INSTDIR\srm.exe uninstall GitClicky.Extension.dll -codebase"
  RmDir /r $INSTDIR
  DeleteRegKey HKLM "${UNINSTALL_REG_KEY}"

  ; Restart explorer
  nsRestartExplorer::nsRestartExplorer restart 10000
  Pop $1
  DetailPrint $1

SectionEnd

