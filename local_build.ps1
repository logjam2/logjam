dotnet restore
msbuild
.\test.ps1

dotnet pack src\LogJam\LogJam.csproj -o $env:LREPO /P:Version=1.1.0-netcorejc001 --no-build
dotnet pack src\LogJam.XUnit2\LogJam.XUnit2.csproj -o $env:LREPO /P:Version=1.1.0-netcorejc001 --no-build