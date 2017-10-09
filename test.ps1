# Dump info for https://github.com/xunit/xunit/issues/1345#issuecomment-335274948
 dir -r -fi *.deps.json | select FullName
"Configuration: $env:Configuration" 
" "

$ErrorActionPreference='Stop'

# Helper function, throws when an external executable returns a non-zero exit code.
function Exec([scriptblock]$cmd, [string]$errorMessage = "Error executing command: " + $cmd) { 
  & $cmd 
  if ($LastExitCode -ne 0) {
    throw $errorMessage 
  } 
}

$testProjects =  ( "LogJam.UnitTests", "LogJam.Internal.UnitTests", "LogJam.XUnit2.UnitTests" )
foreach ( $testProject in $testProjects) {
  pushd ".\test\$testProject"
  if ($env:Configuration)
  {
    exec { dotnet xunit -nobuild -internaldiagnostics -configuration $env:Configuration $testProject.csproj }
  }
  else
  {
    exec { dotnet xunit -nobuild -internaldiagnostics $testProject.csproj }
  }
  popd
}

