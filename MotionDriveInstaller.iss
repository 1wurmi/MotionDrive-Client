[Setup]
AppName=MotionDrive
AppVersion=0.2.1
DefaultDirName={localappdata}\MotionDrive
DefaultGroupName=MotionDrive
OutputDir=.
OutputBaseFilename=MotionDriveInstaller
Compression=lzma
SolidCompression=yes

[Files]
Source: "MotionDrive.Desktop.Desktop\bin\Release\net8.0\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\MotionDrive"; Filename: "{app}\MotionDrive.exe"

[Run]
Filename: "{app}\MotionDrive.exe"; Description: "Launch MotionDrive"; Flags: nowait postinstall