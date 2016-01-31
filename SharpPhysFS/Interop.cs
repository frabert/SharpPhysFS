using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace SharpPhysFS
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
    [DllImport("libdl.so", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dlopen")]
    public static extern IntPtr unix_dlopen(string filename, int flags);

    [DllImport("libdl.so", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dlsym")]
    public static extern IntPtr unix_dlsym(IntPtr handle, string symbol);

    [DllImport("libdl.so", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dlclose")]
    public static extern bool unix_dlclose(IntPtr handle);
    #endregion

    #region OSX
    [DllImport("libdyld.dylib", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dlopen")]
    public static extern IntPtr osx_dlopen(string filename, int flags);

    [DllImport("libdyld.dylib", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dlsym")]
    public static extern IntPtr osx_dlsym(IntPtr handle, string symbol);

    [DllImport("libdyld.dylib", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dlclose")]
    public static extern bool osx_dlclose(IntPtr handle);
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

    // When the callbacks are not yet initialized, instead of throwing a
    // null reference exception we explain the problem.
    // I think it makes for a more graceful failure.
    public static FnGetLinkedVersion PHYSFS_getLinkedVersion             = (ref Version v) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnInit PHYSFS_init                                     = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnDeinit PHYSFS_deinit                                 = () => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnSupportedArchiveTypes PHYSFS_supportedArchiveTypes   = () => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnFreeList PHYSFS_freeList                             = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnGetLastError PHYSFS_getLastError                     = () => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnGetLastError PHYSFS_getDirSeparator                  = () => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnPermitSymbolicLinks PHYSFS_permitSymbolicLinks       = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnSupportedArchiveTypes PHYSFS_getCdRomDirs            = () => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnGetLastError PHYSFS_getBaseDir                       = () => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnGetLastError PHYSFS_getUserDir                       = () => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnGetLastError PHYSFS_getWriteDir                      = () => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnSetWriteDir PHYSFS_setWriteDir                       = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnAddToSearchPath PHYSFS_addToSearchPath               = (a, b) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnSetWriteDir PHYSFS_removeFromSearchPath              = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnSupportedArchiveTypes PHYSFS_getSearchPath           = () => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnSetSaneConfig PHYSFS_setSaneConfig                   = (a, b, c, d, e) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnSetWriteDir PHYSFS_mkdir                             = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnSetWriteDir PHYSFS_delete                            = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnEnumerateFiles PHYSFS_getRealDir                     = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnEnumerateFiles PHYSFS_enumerateFiles                 = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnSetWriteDir PHYSFS_exists                            = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnSetWriteDir PHYSFS_isDirectory                       = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnSetWriteDir PHYSFS_isSymbolicLink                    = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnGetLastModTime PHYSFS_getLastModTime                 = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnEnumerateFiles PHYSFS_openWrite                      = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnEnumerateFiles PHYSFS_openAppend                     = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnEnumerateFiles PHYSFS_openRead                       = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnClose PHYSFS_close                                   = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnRead PHYSFS_read                                     = (a, b, c, d) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnRead PHYSFS_write                                    = (a, b, c,d ) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnClose PHYSFS_eof                                     = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnTell PHYSFS_tell                                     = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnSeek PHYSFS_seek                                     = (a, b) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnFileLength PHYSFS_fileLength                         = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnSeek PHYSFS_setBuffer                                = (a, b) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnClose PHYSFS_flush                                   = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnDeinit PHYSFS_isInit                                 = () => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnDeinit PHYSFS_symbolicLinksPermitted                 = () => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnSetAllocator PHYSFS_setAllocator                     = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnMount PHYSFS_mount                                   = (a, b, c) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnEnumerateFiles PHYSFS_getMountPoint                  = (a) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnGetCdRomDirsCallback PHYSFS_getCdRomDirsCallback     = (a, b) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnGetCdRomDirsCallback PHYSFS_getSearchPathCallback    = (a, b) => { throw new InvalidOperationException("Callbacks not initialized yet"); };
    public static FnEnumerateFilesCallback PHYSFS_enumerateFilesCallback = (a, b, c) => { throw new InvalidOperationException("Callbacks not initialized yet"); };

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
      else if(Environment.OSVersion.Platform == PlatformID.MacOSX)
      {
        loadLibrary = n => DynamicLoader.osx_dlopen(n, 1);
        loadSymbol = DynamicLoader.osx_dlsym;
        libraryName = "libphysfs.dylib";
      }
      else
      {
        loadLibrary = n => DynamicLoader.unix_dlopen(n, 1);
        loadSymbol = DynamicLoader.unix_dlsym;
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
