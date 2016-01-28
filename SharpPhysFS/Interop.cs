using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace PhysFS
{
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate int InitDelegate();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void DeinitDelegate();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate IntPtr MallocDelegate(ulong size);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate IntPtr ReallocDelegate(IntPtr ptr, ulong size);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void FreeDelegate(IntPtr ptr);

  public delegate void StringCallback(IntPtr data, string str);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void EnumFilesCallback(IntPtr data, string origdir, string fname);

  [StructLayout(LayoutKind.Sequential)]
  public struct ArchiveInfo
  {
    [MarshalAs(UnmanagedType.LPStr)]
    public string extension;

    [MarshalAs(UnmanagedType.LPStr)]
    public string description;

    [MarshalAs(UnmanagedType.LPStr)]
    public string author;

    [MarshalAs(UnmanagedType.LPStr)]
    public string url;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct Version
  {
    public byte major;
    public byte minor;
    public byte patch;
  }

  [StructLayout(LayoutKind.Sequential)]
  public class Allocator
  {
    [MarshalAs(UnmanagedType.FunctionPtr)]
    public InitDelegate Init;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public DeinitDelegate Deinit;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public MallocDelegate Malloc;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public ReallocDelegate Realloc;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public FreeDelegate Free;
  }

  static class DynamicLoader
  {
    #region Windows
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    public static extern bool FreeLibrary(IntPtr hModule);
    #endregion

    #region Unix
    [DllImport("libdl.so", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr dlopen(string filename, int flags);

    [DllImport("libdl.so", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr dlsym(IntPtr handle, string symbol);

    [DllImport("libdl.so", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool dlclose(IntPtr handle);
    #endregion
  }

  static class Interop
  {
    public static bool init = false;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FnGetLinkedVersion(ref Version v);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FnInit(string argv0);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FnDeinit();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FnClose(IntPtr ptr);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr FnSupportedArchiveTypes();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FnFreeList(IntPtr h);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr FnGetLastError();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FnPermitSymbolicLinks(int permit);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FnSetWriteDir(string s);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FnAddToSearchPath(string s, int i);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FnSetSaneConfig(string s1, string s2, string s3, int i1, int i2);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr FnEnumerateFiles(string s);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long FnGetLastModTime(string s);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long FnRead(IntPtr ptr1, IntPtr ptr2, uint i1, uint i2);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long FnTell(IntPtr ptr);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FnSeek(IntPtr ptr, ulong u);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long FnFileLength(IntPtr ptr);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FnSetAllocator(Allocator alloc);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FnMount(string s1, string s2, int i);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FnGetCdRomDirsCallback(StringCallback c, IntPtr ptr);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FnEnumerateFilesCallback(string s, EnumFilesCallback c, IntPtr ptr);

    public static FnGetLinkedVersion PHYSFS_getLinkedVersion;
    public static FnInit PHYSFS_init;
    public static FnDeinit PHYSFS_deinit;
    public static FnSupportedArchiveTypes PHYSFS_supportedArchiveTypes;
    public static FnFreeList PHYSFS_freeList;
    public static FnGetLastError PHYSFS_getLastError;
    public static FnGetLastError PHYSFS_getDirSeparator;
    public static FnPermitSymbolicLinks PHYSFS_permitSymbolicLinks;
    public static FnSupportedArchiveTypes PHYSFS_getCdRomDirs;
    public static FnGetLastError PHYSFS_getBaseDir;
    public static FnGetLastError PHYSFS_getUserDir;
    public static FnGetLastError PHYSFS_getWriteDir;
    public static FnSetWriteDir PHYSFS_setWriteDir;
    public static FnAddToSearchPath PHYSFS_addToSearchPath;
    public static FnSetWriteDir PHYSFS_removeFromSearchPath;
    public static FnSupportedArchiveTypes PHYSFS_getSearchPath;
    public static FnSetSaneConfig PHYSFS_setSaneConfig;
    public static FnSetWriteDir PHYSFS_mkdir;
    public static FnSetWriteDir PHYSFS_delete;
    public static FnEnumerateFiles PHYSFS_getRealDir;
    public static FnEnumerateFiles PHYSFS_enumerateFiles;
    public static FnSetWriteDir PHYSFS_exists;
    public static FnSetWriteDir PHYSFS_isDirectory;
    public static FnSetWriteDir PHYSFS_isSymbolicLink;
    public static FnGetLastModTime PHYSFS_getLastModTime;
    public static FnEnumerateFiles PHYSFS_openWrite;
    public static FnEnumerateFiles PHYSFS_openAppend;
    public static FnEnumerateFiles PHYSFS_openRead;
    public static FnClose PHYSFS_close;
    public static FnRead PHYSFS_read;
    public static FnRead PHYSFS_write;
    public static FnClose PHYSFS_eof;
    public static FnTell PHYSFS_tell;
    public static FnSeek PHYSFS_seek;
    public static FnFileLength PHYSFS_fileLength;
    public static FnSeek PHYSFS_setBuffer;
    public static FnClose PHYSFS_flush;
    public static FnDeinit PHYSFS_isInit;
    public static FnDeinit PHYSFS_symbolicLinksPermitted;
    public static FnSetAllocator PHYSFS_setAllocator;
    public static FnMount PHYSFS_mount;
    public static FnEnumerateFiles PHYSFS_getMountPoint;
    public static FnGetCdRomDirsCallback PHYSFS_getCdRomDirsCallback;
    public static FnGetCdRomDirsCallback PHYSFS_getSearchPathCallback;
    public static FnEnumerateFilesCallback PHYSFS_enumerateFilesCallback;

    public static void SetUpInterop()
    {
      /* This method is used to dynamically load the physfs
       * library. It works by detecting the current platform
       * and deciding whether to use LoadLibrary/GetProcAddr
       * on Windows or dlopen/dlsym on Linux and OSX.
       * The the class is then scanned using reflection
       * to populate all the callbacks with the right function
       * pointers from the loaded library
       */

      Func<string, IntPtr> loadLibrary;
      Func<IntPtr, string, IntPtr> loadSymbol;
      string libraryName;

      IntPtr library;

      if (Environment.OSVersion.Platform == PlatformID.Win32NT)
      {
        loadLibrary = DynamicLoader.LoadLibrary;
        loadSymbol = DynamicLoader.GetProcAddress;
        libraryName = "physfs.dll";
      }
      else
      {
        loadLibrary = n => DynamicLoader.dlopen(n, 1);
        loadSymbol = DynamicLoader.dlsym;
        libraryName = "libphysfs.so";
      }

      library = loadLibrary(libraryName);
      if (library == IntPtr.Zero)
      {
        throw new PhysFSLibNotFound();
      }

      var fields = typeof(Interop).GetFields();

      foreach(var field in fields.Where(x => x.Name.StartsWith("PHYSFS_")))
      {
        var funcPtr = loadSymbol(library, field.Name);
        var del = Marshal.GetDelegateForFunctionPointer(funcPtr, field.FieldType);

        field.SetValue(null, del);
      }

      init = true;
    }
  }
}
