; Inno Setup 6 — CodeBrief
#define MyAppName "CodeBrief"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "CodeBrief"
#define MyAppExeName "CodeBrief.exe"

[Setup]
AppId={{7E2C1BAA-4C8D-4F11-9B3E-CODEBRIEF0001}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\CodeBrief
DefaultGroupName=CodeBrief
OutputDir=..\dist
OutputBaseFilename=CodeBrief-Setup-1.0.0
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
SetupIconFile=..\src\CodeBrief.Desktop\Assets\app.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
DisableProgramGroupPage=yes

[Languages]
Name: "swedish"; MessagesFile: "compiler:Languages\Swedish.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Skapa en genväg på skrivbordet"; GroupDescription: "Genvägar:"

[Files]
Source: "..\publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\CodeBrief"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\CodeBrief"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Starta CodeBrief"; Flags: nowait postinstall skipifsilent
