using System;

namespace SharpPhysFS
{
  public partial class PhysFS
  {
    public static class LowLevel
    {
      public static IntPtr OpenWrite(string filename, PhysFS physFS)
      {
        var val = Interop.PHYSFS_openWrite(filename);
        if (val == null)
          throw new PhysFSException(physFS);
        return val;
      }

      public static IntPtr OpenAppend(string filename, PhysFS physFS)
      {
        var val = Interop.PHYSFS_openAppend(filename);
        if (val == null)
          throw new PhysFSException(physFS);
        return val;
      }

      public static IntPtr OpenRead(string filename, PhysFS physFS)
      {
        var val = Interop.PHYSFS_openRead(filename);
        if (val == null)
          throw new PhysFSException(physFS);
        return val;
      }

      public static void Close(IntPtr file, PhysFS physFS)
      {
        int err = Interop.PHYSFS_close(file);
        physFS.ThrowException(err);
      }

      public static long Read(IntPtr file, byte[] buffer, uint objSize, uint objCount)
      {
        unsafe
        {
          fixed (void* ptr = buffer)
          {
            return Interop.PHYSFS_read(file, (IntPtr)ptr, objSize, objCount);
          }
        }
      }

      public static long Write(IntPtr file, byte[] buffer, uint objSize, uint objCount)
      {
        unsafe
        {
          fixed (void* ptr = buffer)
          {
            return Interop.PHYSFS_write(file, (IntPtr)ptr, objSize, objCount);
          }
        }
      }

      public static bool EOF(IntPtr file)
      {
        return Interop.PHYSFS_eof(file) != 0;
      }

      public static long Tell(IntPtr file)
      {
        return Interop.PHYSFS_tell(file);
      }

      public static void Seek(IntPtr file, ulong pos, PhysFS physFS)
      {
        int err = Interop.PHYSFS_seek(file, pos);
        physFS.ThrowException(err);
      }

      public static long FileLength(IntPtr file)
      {
        return Interop.PHYSFS_fileLength(file);
      }

      public static void SetBuffer(IntPtr file, ulong bufSize, PhysFS physFS)
      {
        int err = Interop.PHYSFS_setBuffer(file, bufSize);
        physFS.ThrowException(err);
      }

      public static void Flush(IntPtr file, PhysFS physFS)
      {
        int err = Interop.PHYSFS_flush(file);
        physFS.ThrowException(err);
      }
    }
  }
}
