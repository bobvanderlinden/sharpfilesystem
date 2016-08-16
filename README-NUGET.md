# Publishing NuGet Packages

## Updating Version

NuGet requires the nuget version of a package to change before it is
republished. And when you change the version of the SharpFileSystem
nuget package, all of the dependent packages (e.g.,
SharpFileSystem.Resources, SharpFileSystem.SevenZip) need new nuget
packages built so that the automatically generated dependency can
point at the new version of SharFileSystem.

When bumping the version of a package, please keep
[semver](http://semver.org/) in mind. Semantive versioning informs
other developers of how much of a build-breaking, feature-adding, or
bug-fixing a change is meant to be. A semver consists of
MAJOR.MINOR.PATCH. The nuget packages will automatically read the
version from each package’s/assembly’s
`AssemblyInformationalVersionAttribute` set at the end of each
`Properties\Assemblyinfo.cs` file. When coming up with a new version,
choose the first rule that matches in the following list which
describes what changes have happened since the last nuget release
(earliest match should win!).

1. MAJOR: The changes since the last release will break the ABI or API
   and require programs using the library to be at least recompiled if
   not modified to use the new version of the library. Bump the MAJOR
   version and set MINOR and PATCH to 0. `1.2.3` would become
   `2.0.0`. For example, adding a new member to `IFileSystem` will
   require code changes in everything implementing that
   interface. Removing a member from `IFileSystem` will require
   recompilation (for implicit implementations) or code changes (for
   explicit implementations).

2. MINOR: The changes since the last release will keep the ABI and API
   but add new features or additional APIs. Bump the MINOR and set
   PATCH to 0. `1.2.3` would become `1.3.0`. For example, new
   extension methods allowing new code to be written more succinctly
   without breaking old code or assemblies.

3. PATCH: The changes since the last release will keep the ABI and API
   but don’t introduce any new features. Bump PATCH. `1.2.3` would
   beceom `1.2.4`. For example, if SharpFileSystem is published to add
   a new feature, SharpFileSystem.Resources has to have a new nuget
   package built and published so that it can depend on the newer
   version of SharpFileSystem. Or a bug small fix.

## Packing

You will need [`nuget.exe`](https://nuget.org/nuget.exe). You may
place it in a directory and point `PATH` there or copy it to each
project directory.

Before nuget can pack the files, you need to build the whole solution
in Visual Studio for the Release target. Nuget will not automatically
build things for you, it relies on VS to generate the assemblies and
place them in `bin\Release`.

To produce the `.nupkg` files to publish, enter each project’s
directory and run `..\nuget-pack`. Then use either the
https://nuget.org/ website or [the CLI to
publish](https://docs.nuget.org/create/creating-and-publishing-a-package#publishing-in-nuget-gallery)
as you prefer.

## Followup

Please consider tagging the release in git and pushing the tags to
github so that it is easier to track changes between versions and
match up nuget packages with source code.
