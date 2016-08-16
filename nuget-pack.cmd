: Because nuget pack will only work properly if you give the correct
: arguments, use this script file instead of invoking nuget directly.
: That should make getting things built right easier.
:
: To invoke, first CD to the subdirectory of the project which needs
: publishing and then run “..\nuget-pack”
:
: The parameters I placed here are suggested by
: https://docs.nuget.org/create/creating-and-publishing-a-package
:
: -IncludeReferencedProjects will automatically discover, for example,
: that SharpFileSystem.Resources depends on SharpFileSystem and notice
: that SharpFileSystem has a .nuspec and thus that a nuget dependency
: should be automatically made. Note that, while -IncludeReferencedProjects
: is convenient, it hardcodes the full version of the dependency into
: the .nupkg’s .nuspec. This means that bumping SharpFileSystem, even
: for a minor change, will require you to bump the versions of all other
: packages you manage in this repository. You could remove this option
: and manually write semver ranges in the .nuspecs and avoid unnecessary
: bumps when the ABI wouldn’t change, but that would require care and
: it is likely easier to make mistakes with that route.
:
: -Prop Configuration=Release will distribute the optimized build (it
: basically points nuget at bin\Release rathar than bin\Debug).

@ECHO OFF
ECHO.
ECHO Do not forget to increment AssemblyInformationalVersion in Properties\AssemblyInfo.cs
ECHO before running this script. Remember to run this script in each project
ECHO folder that needs to be published. Also, you need to build the whole solution
ECHO in Release mode in Visual Studio before using this script.
ECHO.
ECHO Invoking nuget pack

FOR %%G IN (*.csproj) DO CALL :invokenuget %%G
IF ERRORLEVEL 1 EXIT /B 1
EXIT /B 0

:invokenuget
@ECHO ON
CALL nuget pack %1 -IncludeReferencedProjects -Prop Configuration=Release
@IF ERRORLEVEL 1 (
	@ECHO nuget pack failed. Have you placed http://nuget.org/nuget.exe in a folder and added it to PATH?
	@ECHO OFF
	EXIT /B 1
)
@ECHO OFF
ECHO.
ECHO nuget pack succeeded. Now publish the generated .nupkg:
ECHO.
ECHO   https://docs.nuget.org/create/creating-and-publishing-a-package#publishing-in-nuget-gallery
ECHO.
EXIT /B 0

