using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPhysFS
{
  public partial class PhysFS
  {
    public static class LowLevel
    {
      public static IntPtr OpenWrite(string filename, PhysFS physFS)
      {
        var val = physFS.interop.PHYSFS_openWrite(filename);
        if (val == null)
          throw new PhysFSException(physFS);
        return val;
      }

      public static IntPtr OpenAppend(string filename, PhysFS physFS)
      {
        var val = physFS.interop.PHYSFS_openAppend(filename);
        if (val == null)
          throw new PhysFSException(physFS);
        return val;
      }

      public static IntPtr OpenRead(string filename, PhysFS physFS)
      {
        var val = physFS.interop.PHYSFS_openRead(filename);
        if (val == null)
          throw new PhysFSException(physFS);
        return val;
      }

      public static void Close(IntPtr file, PhysFS physFS)
      {
        int err = physFS.interop.PHYSFS_close(file);
        physFS.ThrowException(err);
      }

      public static long Read(IntPtr file, byte[] buffer, uint objSize, uint objCount, PhysFS physFS)
      {
        unsafe
        {
          fixed (void* ptr = buffer)
          {
            return physFS.interop.PHYSFS_read(file, (IntPtr)ptr, objSize, objCount);
          }
        }
      }

      public static long Write(IntPtr file, byte[] buffer, uint objSize, uint objCount, PhysFS physFS)
      {
        unsafe
        {
          fixed (void* ptr = buffer)
          {
            return physFS.interop.PHYSFS_write(file, (IntPtr)ptr, objSize, objCount);
          }
        }
      }

      public static bool EOF(IntPtr file, PhysFS physFS)
      {
        return physFS.interop.PHYSFS_eof(file) != 0;
      }

      public static long Tell(IntPtr file, PhysFS physFS)
      {
        return physFS.interop.PHYSFS_tell(file);
      }

      public static void Seek(IntPtr file, ulong pos, PhysFS physFS)
      {
        int err = physFS.interop.PHYSFS_seek(file, pos);
        physFS.ThrowException(err);
      }

      public static long FileLength(IntPtr file, PhysFS physFS)
      {
        return physFS.interop.PHYSFS_fileLength(file);
      }

      public static void SetBuffer(IntPtr file, ulong bufSize, PhysFS physFS)
      {
        int err = physFS.interop.PHYSFS_setBuffer(file, bufSize);
        physFS.ThrowException(err);
      }

      public static void Flush(IntPtr file, PhysFS physFS)
      {
        int err = physFS.interop.PHYSFS_flush(file);
        physFS.ThrowException(err);
      }
    }
  }
}
