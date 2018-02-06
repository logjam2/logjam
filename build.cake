#tool "GitVersion.CommandLine&prerelease"
#addin "Cake.Incubator"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var solutionFile = "LogJam.sln";
var nugetOutputDir = "./NuGetOut/";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
  DotNetCoreClean(solutionFile);
});

Task("Set-Version")
    .Does(() =>
{
  var gitVersionResult = GitVersion();

  Information("Found Version info: " + gitVersionResult.Dump());
  if (AppVeyor.IsRunningOnAppVeyor)
  {
    AppVeyor.UpdateBuildVersion(gitVersionResult.NuGetVersionV2 + "++" + AppVeyor.Environment.Build.Number);
  }
  else if (TeamCity.IsRunningOnTeamCity)
  {
    TeamCity.SetBuildNumber(gitVersionResult.NuGetVersionV2);
  }

  // Set environment variables that are picked up by Directory.Build.props
  Environment.SetEnvironmentVariable("GitVersion_SemVer", gitVersionResult.SemVer);
  Environment.SetEnvironmentVariable("GitVersion_AssemblySemVer", gitVersionResult.AssemblySemVer);
  Environment.SetEnvironmentVariable("GitVersion_AssemblySemFileVer", gitVersionResult.AssemblySemFileVer);
  Environment.SetEnvironmentVariable("GitVersion_NuGetVersion", gitVersionResult.NuGetVersion);
});

Task("Restore-NuGet-Packages")
    .Does(() =>
{
  DotNetCoreRestore(solutionFile);
});

Task("Build")
    .IsDependentOn("Set-Version")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
  MSBuild(solutionFile, settings =>
    settings.SetConfiguration(configuration));
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .DoesForEach(GetFiles("./test/*.UnitTests/*.csproj"), (unitTestProject) =>
{
  DotNetCoreTool(unitTestProject, "xunit", "-nobuild -configuration " + configuration);
}); 

Task("Package")
    .IsDependentOn("Build")
    .Does(() =>
{
  CreateDirectory(nugetOutputDir);
})
    .DoesForEach(GetFiles("./src/*/*.csproj"), (srcProjectFile) =>
{
  DotNetCorePack(srcProjectFile.FullPath, new DotNetCorePackSettings
     {
         Configuration = configuration,
         OutputDirectory = nugetOutputDir,
         NoBuild = true,
         NoRestore = true
     });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
