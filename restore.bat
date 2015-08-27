setlocal
set logjamDir=%~dp0
pushd %logjamDir%

nuget sources add -name logjam-local-packages -source %logjamDir%build\nuget-packages

nuget restore .\LogJam.sln -Verbosity detailed

#nuget sources remove -name logjam-local-packages

popd