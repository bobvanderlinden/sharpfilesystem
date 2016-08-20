using System.Reflection;

[assembly: AssemblyProduct("SharpFileSystem")]

[assembly: AssemblyCompany("Bob van der Linden")]
[assembly: AssemblyCopyright("Copyright © 2010")]

#if DEBUG
 [assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyInformationalVersion("1.0.0")]

// Never change this. It forces people to change assembly references and mostly just problems.
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
