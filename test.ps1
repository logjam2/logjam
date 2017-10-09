# Dump info for https://github.com/xunit/xunit/issues/1345#issuecomment-335274948
 dir -r -fi *.deps.json | select FullName
"Configuration: $env:Configuration" 
" "

$testProjects =  ( "LogJam.UnitTests", "LogJam.Internal.UnitTests", "LogJam.XUnit2.UnitTests" )
foreach ( $testProject in $testProjects) {
  pushd ".\test\$testProject"
  if ($env:Configuration)
  {
    dotnet xunit -nobuild -internaldiagnostics -configuration $env:Configuration $testProject.csproj
  }
  else
  {
    dotnet xunit -nobuild -internaldiagnostics $testProject.csproj
  }
  popd
}
