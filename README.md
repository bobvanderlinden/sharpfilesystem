# SharpFileSystem

[![AppVeyor](https://ci.appveyor.com/api/projects/status/github/bobvanderlinden/sharpfilesystem?svg=true)](https://ci.appveyor.com/project/bobvanderlinden/sharpfilesystem) [![Travis](https://img.shields.io/travis/bobvanderlinden/sharpfilesystem.svg?maxAge=2592000&label=travis)](https://travis-ci.org/bobvanderlinden/sharpfilesystem) [![license](https://img.shields.io/github/license/mashape/apistatus.svg?maxAge=2592000)](LICENSE.txt)

SharpFileSystem is a [Virtual File System (VFS)](http://en.wikipedia.org/wiki/Virtual_file_system) implementation for .NET to allow access to different filesystems in the same way for normal files and directories.

## Motivation

After looking a long time for a VFS for .NET, so that I could read files and directories in archives (zip and rar) the same way as any ordinary files and directories. I couldn't find any complete solution for this problem, so I decided to implement a VFS myself. Also what I didn't like in the ordinary filesystems was the path-system. It allowed relative paths (like ..), which can often lead to security issues without explicit checking. It allows referring to directories the same way as referring to files. This often leads to strange behavior, like copying a directory (source) to another directory (destination), here it is often vague whether the destination directory should be overwritten or if it should copy the source-directory inside the destination-directory.

## Goals

* Using multiple types of filesystems (ie. zip, rar, ftp, etc) in the same way.
* A way to combine these systems to structure multiple parts of your program resources and configurations.
* A way to restrict or hide parts of your filesystem to parts of your program.
* A robust way of handling paths.

## Features

At the moment the following filesystems are implemented:

* **PhysicalFileSystem**: gives access to the real operating system filesystem (uses System.IO of .NET/Mono).
* **MemoryFileSystem**: simulates a filesystem in-memory.
* **SevenZipFileSystem**: gives access to any 7-Zip supported archive.

There are also filesystems that alter the exposed structure of existing filesystems. These allow you to mix different systems together to get the desired file-structure:

* **ReadOnlyFileSystem**: Only allows read-operations on the wrapped filesystem.
* **MergedFileSystem**: Merges multiple filesystems into a single file-structure.
* **FileSystemMounter**: A unix-like mount system, which mounts other filesystems to a specified path.
* **SubFileSystem**: Allows you to create a filesystem from a directory of another filesystem and therefore restricts access to parent-directories.
* **SeamlessSevenZipFileSystem**: Allows access to 7-Zip archives as if they're directories.

## Implementation

At the heart of the system there is the `IFileSystem` interface. This describes the operations that should be provided by every filesystem:

	public interface IFileSystem : IDisposable
	{
		ICollection<FileSystemPath> GetEntities(FileSystemPath path);
		bool Exists(FileSystemPath path);
		Stream CreateFile(FileSystemPath path);
		Stream OpenFile(FileSystemPath path, FileAccess access);
		void CreateDirectory(FileSystemPath path);
		void Delete(FileSystemPath path);
	}

Normally in .NET/Mono we refer to files by their path coded as a string. This sometimes leads to inconsistencies, which is why I've created the type FileSystemPath to encapsulate the path in SharpFileSystem. There are operations that help create and alter the path, but the rules of using them are more strict, which creates a much more robust environment for the programmer:

* Independent of the OS, the directory-separator is always `/`.
* All paths always start with `/`. This means: there are no relative paths.
* All paths that refer to a directory end with `/`.
* All paths that refer to a file do not end with `/`.
* The path cannot be manipulated as a string, only new paths can be created through a small set of operations (like `System.IO.Path.Combine` for .NET).

Some examples of using `FileSystemPath`:

Operation | Result
--- | ---
`var root = FileSystemPath.Root` | `/`
`var mydir = root.AppendDirectory("mydir")` | `/mydir/`
`var myfile = mydir.AppendFile("myfile")` | `/mydir/myfile`
`myfile.AppendFile("myfile2")` | Error: The specified `FileSystemPath` is not a directory.
`mydir.ParentPath` | `/`
