using System;
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

  static class Interop
  {
    const string DLL_NAME = "physfs.dll";

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void PHYSFS_getLinkedVersion(ref Version ver);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_init(string argv0);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_deinit();

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr PHYSFS_supportedArchiveTypes();

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void PHYSFS_freeList(IntPtr listVar);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr PHYSFS_getLastError();

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern string PHYSFS_getDirSeparator();

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void PHYSFS_permitSymbolicLinks(int allow);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static unsafe extern IntPtr PHYSFS_getCdRomDirs();

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern string PHYSFS_getBaseDir();

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern string PHYSFS_getUserDir();

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern string PHYSFS_getWriteDir();

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_setWriteDir(string newDir);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_addToSearchPath(string newDir, int appendToPath);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_removeFromSearchPath(string oldDir);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static unsafe extern IntPtr PHYSFS_getSearchPath();

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_setSaneConfig(string organization, string appName, string archiveExt, int includeCdRoms, int archivesFirst);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_mkdir(string dirName);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_delete(string filename);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern string PHYSFS_getRealDir(string filename);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static unsafe extern IntPtr PHYSFS_enumerateFiles(string dir);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_exists(string fname);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_isDirectory(string fname);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_isSymbolicLink(string fname);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern long PHYSFS_getLastModTime(string filename);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr PHYSFS_openWrite(string filename);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr PHYSFS_openAppend(string filename);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr PHYSFS_openRead(string filename);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_close(IntPtr handle);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern long PHYSFS_read(IntPtr handle, IntPtr buffer, uint objSize, uint objCount);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern long PHYSFS_write(IntPtr handle, IntPtr buffer, uint objSize, uint objCount);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_eof(IntPtr handle);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern long PHYSFS_tell(IntPtr handle);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_seek(IntPtr handle, ulong pos);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern long PHYSFS_fileLength(IntPtr handle);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_setBuffer(IntPtr handle, ulong bufsize);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_flush(IntPtr handle);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern short PHYSFS_swapSLE16(short val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort PHYSFS_swapULE16(ushort val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_swapSLE32(int val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint PHYSFS_swapULE32(uint val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern long PHYSFS_swapSLE64(long val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong PHYSFS_swapULE64(ulong val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern short PHYSFS_swapSBE16(short val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort PHYSFS_swapUBE16(ushort val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_swapSBE32(int val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint PHYSFS_swapUBE32(uint val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern long PHYSFS_swapSBE64(long val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong PHYSFS_swapUBE64(ulong val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_readSLE16(IntPtr file, ref short val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_readULE16(IntPtr file, ref ushort val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_readSBE16(IntPtr file, ref short val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_readUBE16(IntPtr file, ref ushort val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_readSLE32(IntPtr file, ref int val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_readULE32(IntPtr file, ref uint val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_readSBE32(IntPtr file, ref int val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_readUBE32(IntPtr file, ref uint val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_readSLE64(IntPtr file, ref long val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_readULE64(IntPtr file, ref ulong val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_readSBE64(IntPtr file, ref long val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_readUBE64(IntPtr file, ref ulong val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_writeSLE16(IntPtr file, short val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_writeULE16(IntPtr file, ushort val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_writeSBE16(IntPtr file, short val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_writeUBE16(IntPtr file, ushort val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_writeSLE32(IntPtr file, int val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_writeULE32(IntPtr file, uint val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_writeSBE32(IntPtr file, int val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_writeUBE32(IntPtr file, uint val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_writeSLE64(IntPtr file, long val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_writeULE64(IntPtr file, ulong val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_writeSBE64(IntPtr file, long val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_writeUBE64(IntPtr file, ulong val);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_isInit();

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_symbolicLinksPermitted();

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_setAllocator(Allocator allocator);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int PHYSFS_mount(string newDir, string mountPoint, int appendToPath);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern string PHYSFS_getMountPoint(string dir);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void PHYSFS_getCdRomDirsCallback(StringCallback c, IntPtr d);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void PHYSFS_getSearchPathCallback(StringCallback c, IntPtr d);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void PHYSFS_enumerateFilesCallback(string dir, EnumFilesCallback c, IntPtr d);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static unsafe extern void PHYSFS_utf8FromUcs4(uint* src, char* dst, ulong len);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static unsafe extern void PHYSFS_utf8ToUcs4(string src, uint* dst, ulong len);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static unsafe extern void PHYSFS_utf8FromUcs2(ushort* src, char* dst, ulong len);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static unsafe extern void PHYSFS_utf8ToUcs2(string src, ushort* dst, ulong len);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static unsafe extern void PHYSFS_utf8FromLatin1(string src, char* dst, ulong len);
  }
}
