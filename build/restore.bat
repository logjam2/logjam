pushd %~dp0
pushd ..
REM Restore non-public nuget packages
nuget restore LogJam.sln -source %cd%\build\nuget-packages\ -Verbosity detailed
rem nuget restore LogJam.sln