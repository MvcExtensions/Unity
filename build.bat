set msbuild=%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild
set configuration=%1
if "%configuration%" == "" set configuration=Release

%msbuild% MvcExtensions.Unity.build /t:Full /p:Configuration=%configuration% /m:2

pause