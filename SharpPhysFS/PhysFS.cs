using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SharpPhysFS
{
  public class PhysFSLibNotFound : Exception
  {
    public PhysFSLibNotFound()
      : base("Couldn't locate PhysFS library")
    { }
  }

  /// <summary>
  /// Main class for SharpPhysFS
  /// </summary>
  public partial class PhysFS
    : IDisposable
  {
    public class PhysFSException : Exception
    {
      public PhysFSException(PhysFS physFS)
        : base(physFS.GetLastError())
      { }
    }

    public PhysFS(string argv0)
    {
      Init(argv0);
    }

    public PhysFS(string argv0, string libname)
    {
      Init(argv0);
    }

    static T FromPtr<T>(IntPtr ptr)
    {
      return (T)Marshal.PtrToStructure(ptr, typeof(T));
    }

    void ThrowException(int err)
    {
      if (err == 0)
      {
        throw new PhysFSException(this);
      }
    }

    /// <summary>
    /// Get the version of PhysicsFS that is linked against your program
    /// </summary>
    /// <returns>The version of PhysicsFS that is linked against your program</returns>
    public Version GetLinkedVersion()
    {
      Version v = new Version();
      Interop.PHYSFS_getLinkedVersion(ref v);
      return v;
    }

    /// <summary>
    /// Initialize the PhysicsFS library.
    /// </summary>
    /// <remarks>
    /// Must be called before any other PhysicsFS function.
    /// This should be called prior to any attempts to change your process's current working directory.
    /// NOTE: This is automatically called when the class is created.
    /// </remarks>
    /// <param name="argv0">This should be the path of the executable (first arguments passed to main function in C programs)</param>
    public void Init(string argv0)
    {
      int err = Interop.PHYSFS_init(argv0);
      ThrowException(err);
    }

    /// <summary>
    /// Deinitialize the PhysicsFS library.
    /// </summary>
    /// 
    /// <para>
    /// This closes any files opened via PhysicsFS, blanks the search/write paths, frees memory, and invalidates all of your file handles.
    /// </para>
    /// 
    /// <remarks>
    /// This call can throw if there's a file open for writing that refuses to close
    /// (for example, the underlying operating system was buffering writes to network filesystem,
    /// and the fileserver has crashed, or a hard drive has failed, etc). It is usually best to
    /// close all write handles yourself before calling this function, so that you can gracefully handle a specific failure.
    /// Once successfully deinitialized, Init can be called again to restart the subsystem.
    /// All default API states are restored at this point, with the exception of any custom allocator you might have specified, which survives between initializations.
    /// NOTE: This is called automatically on disposal.
    /// </remarks>
    public void Deinit()
    {
      int err = Interop.PHYSFS_deinit();
      ThrowException(err);
    }

    /// <summary>
    /// Add an archive or directory to the search path.
    /// </summary>
    /// <para>
    /// If this is a duplicate, the entry is not added again, even though the function succeeds.
    /// You may not add the same archive to two different mountpoints: duplicate checking is done against the archive and not the mountpoint.
    /// </para>
    /// <para>
    /// When you mount an archive, it is added to a virtual file system...all files in all of the archives are interpolated into a single hierachical file tree.
    /// Two archives mounted at the same place (or an archive with files overlapping another mountpoint) may have overlapping files:
    /// in such a case, the file earliest in the search path is selected, and the other files are inaccessible to the application. 
    /// This allows archives to be used to override previous revisions; you can use the mounting mechanism to place archives at a specific point
    /// in the file tree and prevent overlap; this is useful for downloadable mods that might trample over application data or each other, for example.
    /// </para>
    /// <para>
    /// The mountpoint does not need to exist prior to mounting, which is different than those familiar with the Unix concept of "mounting" may not expect.
    /// As well, more than one archive can be mounted to the same mountpoint, or mountpoints and archive contents can overlap...the interpolation mechanism still functions as usual.
    /// </para>
    /// <param name="dir">directory or archive to add to the path, in platform-dependent notation.</param>
    /// <param name="mountPoint">Location in the interpolated tree that this archive will be "mounted", in platform-independent notation. null or "" is equivalent to "/".</param>
    /// <param name="appendToPath">True to append to search path, false to prepend.</param>
    public void Mount(string dir, string mountPoint, bool appendToPath)
    {
      int err = Interop.PHYSFS_mount(dir, mountPoint, appendToPath ? 1 : 0);
      ThrowException(err);
    }

    /// <summary>
    /// Get human-readable error information.
    /// </summary>
    /// <para>
    /// Get the last PhysicsFS error message as a human-readable string.
    /// This will be null if there's been no error since the last call to this function.
    /// Each thread has a unique error state associated with it, but each time a new error message is set,
    /// it will overwrite the previous one associated with that thread.
    /// It is safe to call this function at anytime, even before PhysFS.Init().
    /// </para>
    /// <returns>String of last error message.</returns>
    public string GetLastError()
    {
      return Marshal.PtrToStringAnsi(Interop.PHYSFS_getLastError());
    }

    /// <summary>
    /// Get platform-dependent dir separator string.
    /// </summary>
    /// <para>
    /// This returns "\\" on win32, "/" on Unix, and ":" on MacOS.
    /// It may be more than one character, depending on the platform, and your code should take that into account.
    /// </para>
    /// <remarks>
    /// This is only useful for setting up the search/write paths, since access into those dirs always use '/' (platform-independent notation) 
    /// </remarks>
    /// <returns>Platform-dependent dir separator string</returns>
    public string GetDirSeparator()
    {
      return Marshal.PtrToStringAnsi(Interop.PHYSFS_getDirSeparator());
    }

    /// <summary>
    /// Get a file listing of a search path's directory.
    /// </summary>
    /// <para>
    /// Matching directories are interpolated.
    /// That is, if "C:\mydir" is in the search path and contains a directory "savegames" that contains "x.sav", "y.sav", and "z.sav",
    /// and there is also a "C:\userdir" in the search path that has a "savegames" subdirectory with "w.sav", then the following code:
    ///   <code>
    ///   foreach(var file in PhysFS.EnumerateFiles("savegames"))
    ///   {
    ///     System.Console.WriteLine(" * We've got [{0}].", file);
    ///   }
    ///   </code>
    ///   Will print...
    ///   <code>
    ///   * We've got [x.sav].
    ///   * We've got [y.sav].
    ///   * We've got [z.sav].
    ///   * We've got [w.sav].
    ///   </code>
    /// </para>
    /// <param name="dir">Directory in platform-independent notation to enumerate.</param>
    public string[] EnumerateFiles(string dir)
    {
      var list = new List<string>();
      EnumerateFilesCallback(dir, (o, f) => list.Add(f));
      return list.ToArray();
    }

    /// <summary>
    /// Get an enumeration of supported archive types.
    /// </summary>
    /// <para>
    /// Get a list of archive types supported by this implementation of PhysicFS.
    /// These are the file formats usable for search path entries.
    /// This is for informational purposes only.
    /// </para>
    /// <remarks>
    /// The extension listed is merely convention: if we list "ZIP", you can open a PkZip-compatible archive with an extension of "XYZ", if you like.
    /// </remarks>
    /// <returns>An enumeration of supported archive types</returns>
    public IEnumerable<ArchiveInfo> SupportedArchiveTypes()
    {
      IntPtr archives = Interop.PHYSFS_supportedArchiveTypes();
      IntPtr i = archives;
      for (i = archives; Marshal.ReadIntPtr(i) != IntPtr.Zero; i = IntPtr.Add(i, IntPtr.Size))
      {
        IntPtr ptr = Marshal.ReadIntPtr(i);
        var info = FromPtr<ArchiveInfo>(ptr);
        yield return info;
      }
    }

    /// <summary>
    /// Enable or disable following of symbolic links.
    /// </summary>
    /// <para>
    /// Some physical filesystems and archives contain files that are just pointers to other files.
    /// On the physical filesystem, opening such a link will (transparently) open the file that is pointed to.
    /// </para>
    /// <para>
    /// By default, PhysicsFS will check if a file is really a symlink during open calls and fail if it is.
    /// Otherwise, the link could take you outside the write and search paths, and compromise security.
    /// </para>
    /// <para>
    /// If you want to take that risk, call this function with a true parameter.
    /// Note that this is more for sandboxing a program's scripting language,
    /// in case untrusted scripts try to compromise the system.
    /// Generally speaking, a user could very well have a legitimate reason to set up a symlink,
    /// so unless you feel there's a specific danger in allowing them, you should permit them
    /// </para>
    /// <remarks>
    /// Symlinks are only explicitly checked when dealing with filenames in platform-independent notation.
    /// That is, when setting up your search and write paths, etc, symlinks are never checked for.
    /// </remarks>
    /// <param name="permit">true to permit symlinks, false to deny linking.</param>
    public void PermitSymbolicLinks(bool permit)
    {
      Interop.PHYSFS_permitSymbolicLinks(permit ? 1 : 0);
    }

    /// <summary>
    /// Get an enumeration of paths to available CD-ROM drives.
    /// </summary>
    /// <para>
    /// The dirs returned are platform-dependent ("D:\" on Win32, "/cdrom" or whatnot on Unix).
    /// Dirs are only returned if there is a disc ready and accessible in the drive.
    /// So if you've got two drives (D: and E:), and only E: has a disc in it, then that's all you get.
    /// If the user inserts a disc in D: and you call this function again, you get both drives.
    /// If, on a Unix box, the user unmounts a disc and remounts it elsewhere, the next call to this function will reflect that change.
    /// </para>
    /// <para>
    /// This function refers to "CD-ROM" media, but it really means "inserted disc media," such as DVD-ROM, HD-DVD, CDRW, and Blu-Ray discs.
    /// It looks for filesystems, and as such won't report an audio CD, unless there's a mounted filesystem track on it.
    /// </para>
    /// <remarks>
    /// This call may block while drives spin up. Be forewarned.
    /// </remarks>
    /// <returns>An enumeration of paths to available CD-ROM drives.</returns>
    public string[] GetCdRomDirs()
    {
      var list = new List<string>();
      GetCdRomDirsCallback((s) => list.Add(s));
      return list.ToArray();
    }

    /// <summary>
    /// Get the path where the application resides.
    /// </summary>
    /// <remarks>
    /// It is probably better to use managed methods for this.
    /// </remarks>
    /// <returns></returns>
    public string GetBaseDir()
    {
      return Marshal.PtrToStringAnsi(Interop.PHYSFS_getBaseDir());
    }

    /// <summary>
    /// Get the path where user's home directory resides.
    /// </summary>
    /// <para>
    /// Get the "user dir". This is meant to be a suggestion of where a specific user of the system can store files.
    /// On Unix, this is her home directory. On systems with no concept of multiple home directories (MacOS, win95),
    /// this will default to something like "C:\mybasedir\users\username" where "username" will either be the login name,
    /// or "default" if the platform doesn't support multiple users, either.
    /// </para>
    /// <para>
    /// You should probably use the user dir as the basis for your write dir, and also put it near the beginning of your search path.
    /// </para>
    /// <returns>String of user dir in platform-dependent notation.</returns>
    public string GetUserDir()
    {
      return Marshal.PtrToStringAnsi(Interop.PHYSFS_getUserDir());
    }

    /// <summary>
    /// Get path where PhysicsFS will allow file writing.
    /// </summary>
    /// <para>
    /// Get the current write dir. The default write dir is "".
    /// </para>
    /// <returns>String of write dir in platform-dependent notation, OR null IF NO WRITE PATH IS CURRENTLY SET</returns>
    public string GetWriteDir()
    {
      return Marshal.PtrToStringAnsi(Interop.PHYSFS_getWriteDir());
    }

    /// <summary>
    /// Tell PhysicsFS where it may write files.
    /// </summary>
    /// <para>
    /// Set a new write dir. This will override the previous setting.
    /// </para>
    /// <remarks>
    /// This call will fail(and fail to change the write dir) if the current write dir still has files open in it.
    /// </remarks>
    /// <param name="path">
    /// The new directory to be the root of the write dir, specified in platform-dependent notation.
    /// Setting to null disables the write dir, so no files can be opened for writing via PhysicsFS.
    /// </param>
    public void SetWriteDir(string path)
    {
      int err = Interop.PHYSFS_setWriteDir(path);
      ThrowException(err);
    }

    /// <summary>
    /// Add an archive or directory to the search path.
    /// </summary>
    /// <param name="newDir">Directory or archive to add to the path, in platform-dependent notation</param>
    /// <param name="appendToPath">true to append to search path, false to prepend</param>
    [Obsolete("AddToSearchPath is deprecated, please use Mount instead")]
    public void AddToSearchPath(string newDir, bool appendToPath)
    {
      int err = Interop.PHYSFS_addToSearchPath(newDir, appendToPath ? 1 : 0);
      ThrowException(err);
    }

    /// <summary>
    /// Remove a directory or archive from the search path.
    /// </summary>
    /// <para>
    /// This must be a(case-sensitive) match to a dir or archive already in the search path, specified in platform-dependent notation.
    /// </para>
    /// <para>
    /// This call will fail (and fail to remove from the path) if the element still has files open in it.
    /// </para>
    /// <param name="oldDir">	dir/archive to remove.</param>
    public void RemoveFromSearchPath(string oldDir)
    {
      int err = Interop.PHYSFS_removeFromSearchPath(oldDir);
      ThrowException(err);
    }

    /// <summary>
    /// Get the current search path.
    /// </summary>
    public string[] GetSearchPath()
    {
      var list = new List<string>();
      GetSearchPathCallback((s) => list.Add(s));
      return list.ToArray();
    }

    /// <summary>
    /// Set up sane, default paths.
    /// </summary>
    /// <para>
    /// The write dir will be set to "userdir/.organization/appName", which is created if it doesn't exist.
    /// </para>
    /// <para>
    /// The above is sufficient to make sure your program's configuration directory is separated from other clutter, and platform-independent.
    /// The period before "mygame" even hides the directory on Unix systems.
    /// </para>
    /// <para>
    /// The search path will be:
    ///   <list type="bullet">
    ///     <item>
    ///       <description>
    ///       The Write Dir (created if it doesn't exist)
    ///       </description>
    ///     </item>
    ///     <item>
    ///       <description>
    ///       The Base Dir
    ///       </description>
    ///     </item>
    ///     <item>
    ///       <description>
    ///       All found CD-ROM dirs (optionally)
    ///       </description>
    ///     </item>
    ///   </list>
    /// </para>
    /// <para>
    /// These directories are then searched for files ending with the extension <paramref name="archiveExt"/>, which,
    /// if they are valid and supported archives, will also be added to the search path.
    /// If you specified "PKG" for <paramref name="archiveExt"/>, and there's a file named data.PKG in the base dir, it'll be checked.
    /// Archives can either be appended or prepended to the search path in alphabetical order,
    /// regardless of which directories they were found in.
    /// </para>
    /// <para>
    /// All of this can be accomplished from the application, but this just does it all for you. Feel free to add more to the search path manually, too.
    /// </para>
    /// <param name="organization">Name of your company/group/etc to be used as a dirname, so keep it small, and no-frills.</param>
    /// <param name="appName">Program-specific name of your program, to separate it from other programs using PhysicsFS.</param>
    /// <param name="archiveExt">
    /// File extension used by your program to specify an archive.
    /// For example, Quake 3 uses "pk3", even though they are just zipfiles.
    /// Specify null to not dig out archives automatically.
    /// Do not specify the '.' char; If you want to look for ZIP files, specify "ZIP" and not ".ZIP" ... the archive search is case-insensitive.
    /// </param>
    /// <param name="includeCdRoms">
    /// True to include CD-ROMs in the search path, and (if <paramref name="archiveExt"/> != null) search them for archives.
    /// This may cause a significant amount of blocking while discs are accessed, and if there are no discs in the drive (or even not mounted on Unix systems),
    /// then they may not be made available anyhow. You may want to specify false and handle the disc setup yourself.
    /// </param>
    /// <param name="archivesFirst">True to prepend the archives to the search path. False to append them. Ignored if !<paramref name="archiveExt"/>.</param>
    public void SetSaneConfig(string organization, string appName, string archiveExt, bool includeCdRoms, bool archivesFirst)
    {
      int err = Interop.PHYSFS_setSaneConfig(organization, appName, archiveExt, includeCdRoms ? 1 : 0, archivesFirst ? 1 : 0);
      ThrowException(err);
    }

    /// <summary>
    /// Create a directory.
    /// </summary>
    /// <para>
    /// This is specified in platform-independent notation in relation to the write dir.
    /// All missing parent directories are also created if they don't exist.
    /// </para>
    /// <para>
    /// So if you've got the write dir set to "C:\mygame\writedir" and call Mkdir("downloads/maps")
    /// then the directories "C:\mygame\writedir\downloads" and "C:\mygame\writedir\downloads\maps" will be created if possible.
    /// If the creation of "maps" fails after we have successfully created "downloads",
    /// then the function leaves the created directory behind and reports failure.
    /// </para>
    /// <param name="dirName">New dir to create.</param>
    public void Mkdir(string dirName)
    {
      int err = Interop.PHYSFS_mkdir(dirName);
      ThrowException(err);
    }

    /// <summary>
    /// Delete a file or directory.
    /// </summary>
    /// <para><paramref name="filename"/> is specified in platform-independent notation in relation to the write dir.</para>
    /// <para>A directory must be empty before this call can delete it.</para>
    /// <para>Deleting a symlink will remove the link, not what it points to, regardless of whether you "permitSymLinks" or not.</para>
    /// <para>
    /// So if you've got the write dir set to "C:\mygame\writedir" and call Delete("downloads/maps/level1.map")
    /// then the file "C:\mygame\writedir\downloads\maps\level1.map" is removed from the physical filesystem,
    /// if it exists and the operating system permits the deletion.
    /// </para>
    /// <para>
    /// Note that on Unix systems, deleting a file may be successful,
    /// but the actual file won't be removed until all processes that have an open filehandle to it (including your program) close their handles.
    /// </para>
    /// <para>
    /// Chances are, the bits that make up the file still exist,
    /// they are just made available to be written over at a later point.
    /// Don't consider this a security method or anything. :)
    /// </para>
    /// <param name="filename">Filename to delete.</param>
    public void Delete(string filename)
    {
      int err = Interop.PHYSFS_delete(filename);
      ThrowException(err);
    }

    /// <summary>
    /// Figure out where in the search path a file resides.
    /// </summary>
    /// <para>
    /// The file is specified in platform-independent notation.
    /// The returned filename will be the element of the search path where the file was found, which may be a directory, or an archive.
    /// Even if there are multiple matches in different parts of the search path, only the first one found is used, just like when opening a file.
    /// </para>
    /// <para>
    /// So, if you look for "maps/level1.map", and C:\mygame is in your search path and C:\mygame\maps\level1.map exists, then "C:\mygame" is returned.
    /// </para>
    /// <para>If a any part of a match is a symbolic link, and you've not explicitly permitted symlinks, then it will be ignored, and the search for a match will continue.</para>
    /// <para>
    /// If you specify a fake directory that only exists as a mount point, it'll be associated with the first archive mounted there,
    /// even though that directory isn't necessarily contained in a real archive.
    /// </para>
    /// <param name="filename">File to look for.</param>
    /// <returns>String of element of search path containing the the file in question. null if not found.</returns>
    public string GetRealDir(string filename)
    {
      return Marshal.PtrToStringAnsi(Interop.PHYSFS_getRealDir(filename));
    }

    /// <summary>
    /// Determine if a file exists in the search path.
    /// </summary>
    /// <para>Reports true if there is an entry anywhere in the search path by the name of <paramref name="fname"/>.</para>
    /// <para>
    /// Note that entries that are symlinks are ignored if PhysFS.PermitSymbolicLinks(true) hasn't been called,
    /// so you might end up further down in the search path than expected.
    /// </para>
    /// <param name="fname">Filename in platform-independent notation.</param>
    /// <returns>True if filename exists. false otherwise.</returns>
    public bool Exists(string fname)
    {
      return Interop.PHYSFS_exists(fname) != 0;
    }

    /// <summary>
    /// Determine if a file in the search path is really a directory.
    /// </summary>
    /// <para>Determine if the first occurence of <paramref name="fname"/> in the search path is really a directory entry.</para>
    /// <para>Note that entries that are symlinks are ignored if PhysFS.PermitSymbolicLinks(true) hasn't been called, so you might end up further down in the search path than expected.</para>
    /// <param name="fname">Filename in platform-independent notation.</param>
    /// <returns>True if filename exists and is a directory. False otherwise.</returns>
    public bool IsDirectory(string fname)
    {
      return Interop.PHYSFS_isDirectory(fname) != 0;
    }

    /// <summary>
    /// Determine if a file in the search path is really a symbolic link.
    /// </summary>
    /// <para>Determine if the first occurence of <paramref name="fname"/> in the search path is really a symbolic link.</para>
    /// <para>Note that entries that are symlinks are ignored if PhysFS.PermitSymbolicLinks(true) hasn't been called, and as such, this function will always return false in that case.</para>
    /// <param name="fname">Filename in platform-independent notation.</param>
    /// <returns>True if filename exists and is a symlink. False otherwise.</returns>
    public bool IsSymbolicLink(string fname)
    {
      return Interop.PHYSFS_isSymbolicLink(fname) != 0;
    }

    /// <summary>
    /// Get the last modification time of a file.
    /// </summary>
    /// <para>
    /// The modtime is returned as a number of seconds since the epoch (Jan 1, 1970).
    /// The exact derivation and accuracy of this time depends on the particular archiver.
    /// If there is no reasonable way to obtain this information for a particular archiver, or there was some sort of error, this function returns (-1).
    /// </para>
    /// <param name="fname">Filename to check, in platform-independent notation.</param>
    /// <returns>Last modified time of the file. -1 if it can't be determined.</returns>
    public long GetLastModTime(string fname)
    {
      return Interop.PHYSFS_getLastModTime(fname);
    }

    /// <summary>
    /// Determine if the PhysicsFS library is initialized.
    /// </summary>
    /// <para>
    /// Once PhysFS.Init() returns successfully, this will return true.
    /// Before a successful PhysFS.Init() and after PhysFS.Deinit() returns successfully, this will return false. This function is safe to call at any time.
    /// </para>
    /// <returns>True if library is initialized, false if library is not.</returns>
    public bool IsInit()
    {
      return Interop.PHYSFS_isInit() != 0;
    }

    /// <summary>
    /// Determine if the symbolic links are permitted.
    /// </summary>
    /// <para>
    /// This reports the setting from the last call to PhysFS.PermitSymbolicLinks().
    /// If PhysFS.PermitSymbolicLinks() hasn't been called since the library was last initialized, symbolic links are implicitly disabled.
    /// </para>
    /// <returns>True if symlinks are permitted, false if not.</returns>
    public bool SymbolicLinksPermitted()
    {
      return Interop.PHYSFS_symbolicLinksPermitted() != 0;
    }

    public void SetAllocator(Allocator allocator)
    {
      int err = Interop.PHYSFS_setAllocator(allocator);
      ThrowException(err);
    }

    /// <summary>
    /// Determine a mounted archive's mountpoint.
    /// </summary>
    /// <para>
    /// You give this function the name of an archive or dir you successfully added to the search path, and it reports the location in the interpolated tree where it is mounted.
    /// Files mounted with an empty mountpoint or through PhysFS.AddToSearchPath() will report "/".
    /// </para>
    /// <param name="dir">
    /// Directory or archive previously added to the path, in platform-dependent notation.
    /// This must match the string used when adding, even if your string would also reference the same file with a different string of characters.
    /// </param>
    /// <returns>String of mount point if added to path</returns>
    public string GetMountPoint(string dir)
    {
      var s = Marshal.PtrToStringAnsi(Interop.PHYSFS_getMountPoint(dir));
      if(s == null)
      {
        throw new PhysFSException(this);
      }
      return s;
    }

    StringCallback WrapStringCallback<T>(Action<T, string> c)
    {
      return (d, s) =>
      {
        var obj = (T)GCHandle.FromIntPtr(d).Target;
        c(obj, s);
      };
    }
    
    void GetCdRomDirsCallback(StringCallback c, object data)
    {
      GCHandle objHandle = GCHandle.Alloc(data);
      Interop.PHYSFS_getCdRomDirsCallback(c, GCHandle.ToIntPtr(objHandle));
      objHandle.Free();
    }

    /// <summary>
    /// Enumerate CD-ROM directories, using an application-defined callback.
    /// </summary>
    /// <remarks>
    /// <see cref="GetCdRomDirsCallback(Action{string})"/> if you don't need to pass
    /// custom data to the callback.
    /// </remarks>
    /// <typeparam name="T">Type of data passed to callback</typeparam>
    /// <param name="c">Callback function to notify about detected drives.</param>
    /// <param name="data">Application-defined data passed to callback. Can be null.</param>
    public void GetCdRomDirsCallback<T>(Action<T, string> c, T data)
    {
      GetCdRomDirsCallback(WrapStringCallback(c), data);
    }

    /// <summary>
    /// Enumerate CD-ROM directories, using an application-defined callback.
    /// </summary>
    /// <param name="c">Callback function to notify about detected drives.</param>
    public void GetCdRomDirsCallback(Action<string> c)
    {
      Interop.PHYSFS_getCdRomDirsCallback((p, s) => c(s), IntPtr.Zero);
    }
    
    void GetSearchPathCallback(StringCallback c, object data)
    {
      GCHandle objHandle = GCHandle.Alloc(data);
      Interop.PHYSFS_getSearchPathCallback(c, GCHandle.ToIntPtr(objHandle));
      objHandle.Free();
    }

    /// <summary>
    /// Enumerate the search path, using an application-defined callback.
    /// </summary>
    /// <remarks>
    /// <see cref="GetSearchPathCallback(Action{string})"/> if you don't need to pass
    /// custom data to the callback.
    /// </remarks>
    /// <typeparam name="T">Type of data passed to callback</typeparam>
    /// <param name="c">Callback function to notify about search path elements.</param>
    /// <param name="data">Application-defined data passed to callback. Can be null.</param>
    public void GetSearchPathCallback<T>(Action<T, string> c, T data)
    {
      GetSearchPathCallback(WrapStringCallback(c), data);
    }

    /// <summary>
    /// Enumerate the search path, using an application-defined callback.
    /// </summary>
    /// <param name="c">Callback function to notify about search path elements.</param>
    public void GetSearchPathCallback(Action<string> c)
    {
      Interop.PHYSFS_getSearchPathCallback((p, s) => c(s), IntPtr.Zero);
    }
    
    void EnumerateFilesCallback(string dir, EnumFilesCallback c, object data)
    {
      GCHandle objHandle = GCHandle.Alloc(data);
      Interop.PHYSFS_enumerateFilesCallback(dir, c, GCHandle.ToIntPtr(objHandle));
      objHandle.Free();
    }

    /// <summary>
    /// Get a file listing of a search path's directory, using an application-defined callback.
    /// </summary>
    /// <remarks>
    /// <see cref="EnumerateFilesCallback(string, Action{string, string})"/> if you don't need
    /// to pass custom data to the callback.
    /// </remarks>
    /// <typeparam name="T">Type of data passed to callback</typeparam>
    /// <param name="dir">Directory, in platform-independent notation, to enumerate.</param>
    /// <param name="c">Callback function to notify about search path elements.</param>
    /// <param name="data">Application-defined data passed to callback. Can be null.</param>
    public void EnumerateFilesCallback<T>(string dir, Action<T, string, string> c, T data)
    {
      EnumerateFilesCallback(dir, (d, o, n) =>
      {
        var obj = (T)GCHandle.FromIntPtr(d).Target;
        c(obj, o, n);
      }, data);
    }

    /// <summary>
    /// Get a file listing of a search path's directory, using an application-defined callback.
    /// </summary>
    /// <param name="dir">Directory, in platform-independent notation, to enumerate.</param>
    /// <param name="c">Callback function to notify about search path elements.</param>
    public void EnumerateFilesCallback(string dir, Action<string, string> c)
    {
      Interop.PHYSFS_enumerateFilesCallback(dir, (data, origdir, fname) => c(origdir, fname), IntPtr.Zero);
    }

    public PhysFSStream OpenAppend(string file)
    {
      var handle = LowLevel.OpenAppend(file, this);
      return new PhysFSStream(this, handle, false);
    }

    public PhysFSStream OpenRead(string file)
    {
      var handle = LowLevel.OpenRead(file, this);
      return new PhysFSStream(this, handle, true);
    }

    public PhysFSStream OpenWrite(string file)
    {
      var handle = LowLevel.OpenWrite(file, this);
      return new PhysFSStream(this, handle, false);
    }

    bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        if (disposing)
        {
          Deinit();
        }

        disposed = true;
      }
    }

    public void Dispose()
    {
      Dispose(true);
    }
  }
}
