[_ISTool]
EnableISX=true

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "ko"; MessagesFile: "compiler:Languages\Korean-5-5.1.11.isl"

[Setup]
AppId=ManZooGy
AppName=������
AppVerName=������
AppPublisher=JinwooMin
AppPublisherURL=http://manzoogy.tistory.com/
VersionInfoCompany=JinwooMin
VersionInfoVersion=0.5.1.2
VersionInfoCopyright=ManZooGy License
VersionInfoDescription=������
MinVersion=4.1,4.0
DefaultDirName={pf}\ManZooGy
DefaultGroupName=������
UninstallDisplayIcon={app}\ManZooGy.exe
Compression=lzma
SolidCompression=true
OutputBaseFilename=ManZooGySetup

[Files]
Source: C:\Program Files\ISTool\isxdl.dll; Flags: dontcopy
Source: ManZooGy.exe; DestDir: {app}
Source: ManZooGy.ini; DestDir: {app}
Source: manzoogy.py; DestDir: {app}
Source: *.dll; DestDir: {app}
Source: ipy\*.*; DestDir: {app}\ipy; Excludes: ".svn"
Source: res\*.*; DestDir: {app}\res; Flags: recursesubdirs; Excludes: ".svn"
Source: common\*.*; DestDir: {app}\common; Flags: recursesubdirs; Excludes: ".svn"
Source: image\*.*; DestDir: {app}\image; Flags: recursesubdirs; Excludes: ".svn"

[Messages]
WinVersionTooLowError='������'�� ������ NT4 �̻��� ������ �ʿ�� �մϴ�.

[Icons]
Name: {group}\������; Filename: {app}\ManZooGy.exe; WorkingDir: "{app}"
Name: {group}\������ ����; Filename: {uninstallexe}

[Run]
Filename: {app}\ManZooGy.exe; WorkingDir: "{app}"; Description: "������ ����"; Flags: postinstall nowait

[Code]
var
  dotnetRedistPath: string;
  dotnetLangPackPath: string;
  downloadNeeded: boolean;
  dotNetNeeded: boolean;
  dotNetLangPackNeeded: boolean;
  memoDependenciesNeeded: string;

procedure isxdl_AddFile(URL, Filename: PChar);
external 'isxdl_AddFile@files:isxdl.dll stdcall';
function isxdl_DownloadFiles(hWnd: Integer): Integer;
external 'isxdl_DownloadFiles@files:isxdl.dll stdcall';
function isxdl_SetOption(Option, Value: PChar): Integer;
external 'isxdl_SetOption@files:isxdl.dll stdcall';


const
  // 1.1
  //dotnetRedistURL = 'http://download.microsoft.com/download/a/a/c/aac39226-8825-44ce-90e3-bf8203e74006/dotnetfx.exe';
  // 2.0
  dotnetRedistURL = 'http://download.microsoft.com/download/5/6/7/567758a3-759e-473e-bf8f-52154438565a/dotnetfx.exe';
  // kor lang pack
  dotnetKorLangPackURL = 'http://download.microsoft.com/download/f/2/9/f293d0f9-d815-48b4-bc03-07d5bfae279f/langpack.exe';
  // local system for testing...	
  // dotnetRedistURL = 'http://192.168.1.1/dotnetfx.exe';

  // reg for ManZooGy
  mwKey = 'Software\ManZooGy\Install';
  mwUninstallerPath = 'UninstallerPath';

function InitializeSetup(): Boolean;
var
  UninstallerPath: string;
  ResultCode: Integer;

begin
  Result := true;

  // uninstall previous version
  if RegQueryStringValue(HKLM, mwKey, mwUninstallerPath, UninstallerPath) then begin
    if Exec(UninstallerPath, '/SILENT', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then begin
      if not (ResultCode = 0) then begin
        Result := false;
      end;
    end;
  end;
  
  // Check for required netfx installation
  dotNetNeeded := false;
  if (not RegKeyExists(HKLM, 'Software\Microsoft\.NETFramework\policy\v2.0')) then begin
    dotNetNeeded := true;
    if (not IsAdminLoggedOn()) then begin
      MsgBox('�����̴� Microsoft .NET Framework 2.0�� ������ �������� ��ġ�Ǿ� �־�� �մϴ�.', mbInformation, MB_OK);
      Result := false;
    end else begin
      memoDependenciesNeeded := memoDependenciesNeeded + '      .NET Framework 2.0' #13;
      dotnetRedistPath := ExpandConstant('{src}\dotnetfx.exe');
      if not FileExists(dotnetRedistPath) then begin
        dotnetRedistPath := ExpandConstant('{tmp}\dotnetfx.exe');
        if not FileExists(dotnetRedistPath) then begin
          isxdl_AddFile(dotnetRedistURL, dotnetRedistPath);
          downloadNeeded := true;
        end;
      end;
      SetIniString('install', 'dotnetRedist', dotnetRedistPath, ExpandConstant('{tmp}\dep.ini'));
    end;
  end;

  // Check for required langpack installation
  dotNetLangPackNeeded := false;
  if (not RegKeyExists(HKLM, 'Software\Microsoft\NET Framework Setup\NDP\v2.0.50727\1042')) then begin
    dotNetLangPackNeeded := true;
    if (not IsAdminLoggedOn()) then begin
      MsgBox('�����̴� Microsoft .NET Framework 2.0 �ѱ��� ������� ������ �������� ��ġ�Ǿ� �־�� �մϴ�.', mbInformation, MB_OK);
      Result := false;
    end else begin
      memoDependenciesNeeded := memoDependenciesNeeded + '      .NET Framework 2.0 Korean Language Pack' #13;
      dotnetLangPackPath := ExpandConstant('{src}\langpack.exe');
      if not FileExists(dotnetLangPackPath) then begin
        dotnetLangPackPath := ExpandConstant('{tmp}\langpack.exe');
        if not FileExists(dotnetLangPackPath) then begin
          isxdl_AddFile(dotnetKorLangPackURL, dotnetLangPackPath);
          downloadNeeded := true;
        end;
      end;
      SetIniString('install', 'dotnetLangPack', dotnetLangPackPath, ExpandConstant('{tmp}\dep.ini'));
    end;
  end;

end;

function NextButtonClick(CurPage: Integer): Boolean;
var
  hWnd: Integer;
  ResultCode: Integer;

begin
  Result := true;

  if CurPage = wpReady then begin

    hWnd := StrToInt(ExpandConstant('{wizardhwnd}'));

    // don't try to init isxdl if it's not needed because it will error on < ie 3
    if downloadNeeded then begin

      isxdl_SetOption('label', 'Microsoft .NET Framework 2.0 �ٿ�ε�');
      isxdl_SetOption('description', '�����̴� Microsoft .NET Framework 2.0�� �ʿ�� �մϴ�. �ʿ��� ������ �ٿ�ε� ���Դϴ�.');
      if isxdl_DownloadFiles(hWnd) = 0 then Result := false;
    end;
    if (Result = true) then begin
      if (dotNetNeeded = true) then begin
        // unattended install parameter
        // ref: http://blogs.msdn.com/astebner/archive/2006/02/07/527219.aspx
        if Exec(ExpandConstant(dotnetRedistPath), '/q:a /c:"install.exe /qb"', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then begin
           // handle success if necessary; ResultCode contains the exit code
           if not (ResultCode = 0) then begin
             Result := false;
           end;
        end else begin
           // handle failure if necessary; ResultCode contains the error code
           Result := false;
        end;
      end;

      if (dotNetLangPackNeeded = true) then begin
        // unattended install parameter
        // ref: http://blogs.msdn.com/astebner/archive/2006/02/07/527219.aspx
        if Exec(ExpandConstant(dotnetLangPackPath), '/q:a /c:"install.exe /qb"', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then begin
           // handle success if necessary; ResultCode contains the exit code
           if not (ResultCode = 0) then begin
             Result := false;
           end;
        end else begin
           // handle failure if necessary; ResultCode contains the error code
           Result := false;
        end;
      end;
    end;
  end;
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  s: string;

begin
  if memoDependenciesNeeded <> '' then s := s + 'Dependencies to install:' + NewLine + memoDependenciesNeeded + NewLine;
  s := s + MemoDirInfo + NewLine + NewLine;

  Result := s
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if (CurStep = ssDone) then begin
    // register uninstaller path
    RegWriteStringValue(HKLM, mwKey,
      mwUninstallerPath, ExpandConstant('{uninstallexe}'));
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if (CurUninstallStep = usDone) then begin
    if RegDeleteKeyIncludingSubkeys(HKLM, mwKey) = False then begin
      // do something
    end;
  end;
end;


