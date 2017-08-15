using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace SharpPhysFS
{
  [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
  public delegate int InitDelegate();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
  public delegate void DeinitDelegate();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
  public delegate IntPtr MallocDelegate(ulong size);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
  public delegate IntPtr ReallocDelegate(IntPtr ptr, ulong size);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
  public delegate void FreeDelegate(IntPtr ptr);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
  public delegate void StringCallback(IntPtr data, string str);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
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
  public struct Allocator
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

  /// <summary>
  /// Type of a file
  /// </summary>
  public enum FileType : int
  {
    /// <summary>
    /// A normal file
    /// </summary>
    Regular = 0,

    /// <summary>
    /// A directory
    /// </summary>
    Directory,

    /// <summary>
    /// A symlink
    /// </summary>
    Symlink,

    /// <summary>
    /// Something completely different, like a device
    /// </summary>
    Other
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct FileStats
  {
    /// <summary>
    /// Size in bytes, -1 for non-files and unknown
    /// </summary>
    public long FileSize;

    /// <summary>
    /// Last modification time
    /// </summary>
    public long ModificationTime;

    /// <summary>
    /// Like <see cref="ModificationTime"/>, but for file creation time
    /// </summary>
    public long CreationTime;

    /// <summary>
    /// Like <see cref="ModificationTime"/>, but for file access time
    /// </summary>
    public long AccessTime;

    /// <summary>
    /// Type of the file
    /// </summary>
    public FileType FileType;

    /// <summary>
    /// Non-zero if read only, zero if writable
    /// </summary>
    public int ReadOnly;
  }

  static class Interop
  {
    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void PHYSFS_getLinkedVersion(ref Version v);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_init(string argv0);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_deinit();

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr PHYSFS_supportedArchiveTypes();

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void PHYSFS_freeList(IntPtr h);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr PHYSFS_getLastError();

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr PHYSFS_getDirSeparator();

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void PHYSFS_permitSymbolicLinks(int permit);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr PHYSFS_getCdRomDirs();

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr PHYSFS_getBaseDir();

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr PHYSFS_getUserDir();

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr PHYSFS_getWriteDir();

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_setWriteDir(string s);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_addToSearchPath(string s, int i);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_removeFromSearchPath(string s);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr PHYSFS_getSearchPath();

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_setSaneConfig(string s1, string s2, string s3, int i1, int i2);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_mkdir(string s);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_delete(string s);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr PHYSFS_getRealDir(string s);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr PHYSFS_enumerateFiles(string s);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_exists(string s);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_isDirectory(string s);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_isSymbolicLink(string s);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern long PHYSFS_getLastModTime(string s);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr PHYSFS_openWrite(string s);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr PHYSFS_openAppend(string s);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr PHYSFS_openRead(string s);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_close(IntPtr ptr);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern long PHYSFS_read(IntPtr ptr1, IntPtr ptr2, uint i1, uint i2);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern long PHYSFS_readBytes(IntPtr ptr1, IntPtr ptr2, ulong l);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern long PHYSFS_write(IntPtr ptr1, IntPtr ptr2, uint i1, uint i2);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern long PHYSFS_writeBytes(IntPtr ptr1, IntPtr ptr2, ulong l);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_eof(IntPtr ptr);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern long PHYSFS_tell(IntPtr ptr);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_seek(IntPtr ptr, ulong u);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern long PHYSFS_fileLength(IntPtr ptr);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_setBuffer(IntPtr ptr, ulong u);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_flush(IntPtr ptr);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_isInit();

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_symbolicLinksPermitted();

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_setAllocator(Allocator alloc);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_mount(string s1, string s2, int i);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr PHYSFS_getMountPoint(string s);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void PHYSFS_getCdRomDirsCallback(StringCallback c, IntPtr p);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void PHYSFS_getSearchPathCallback(StringCallback c, IntPtr p);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void PHYSFS_enumerateFilesCallback(string s, EnumFilesCallback c, IntPtr p);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_stat(string s, ref FileStats stat);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_mountMemory(IntPtr p1, ulong l, [MarshalAs(UnmanagedType.FunctionPtr)] Action<IntPtr> del, string fname, string mountPoint, int appendToPath);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int PHYSFS_mountHandle(IntPtr p1, string fname, string mountPoint, int appendToPath);

    [DllImport("physfs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern string PHYSFS_getPrefDir(string org, string app);
  }
}
