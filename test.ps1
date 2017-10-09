$testProjects =  ( "LogJam.UnitTests", "LogJam.Internal.UnitTests", "LogJam.XUnit2.UnitTests" )
foreach ( $testProject in $testProjects) {
  pushd ".\test\$testProject"
  dotnet xunit -nobuild $testProject.csproj
  popd
}
