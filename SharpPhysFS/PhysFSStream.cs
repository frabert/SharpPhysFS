using System;
using System.IO;

namespace SharpPhysFS
{
  public class PhysFSStream : Stream
  {
    IntPtr handle;
    bool readOnly = false;
    PhysFS physFS;

    internal PhysFSStream(PhysFS pfs, IntPtr ptr, bool readOnly)
    {
      handle = ptr;
      this.readOnly = readOnly;
      physFS = pfs;
    }

    public override bool CanRead
    {
      get
      {
        return true;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return true;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return !readOnly;
      }
    }

    public override void Flush()
    {
      PhysFS.LowLevel.Flush(handle, physFS);
    }

    public override long Length
    {
      get
      {
        return PhysFS.LowLevel.FileLength(handle);
      }
    }

    public override long Position
    {
      get
      {
        return PhysFS.LowLevel.Tell(handle);
      }
      set
      {
        PhysFS.LowLevel.Seek(handle, (ulong)value, physFS);
      }
    }

    public long Read(byte[] buffer, int offset, uint count)
    {
      return PhysFS.LowLevel.Read(handle, buffer, 1, count, offset);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      return (int)Read(buffer, offset, (uint)count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      long pos = 0;
      if (origin == SeekOrigin.Begin)
        pos = 0;
      else if (origin == SeekOrigin.Current)
        pos = PhysFS.LowLevel.Tell(handle);
      else
        pos = PhysFS.LowLevel.FileLength(handle);

      PhysFS.LowLevel.Seek(handle, (ulong)(pos + offset), physFS);
      return pos + offset;
    }

    public long Write(byte[] buffer, uint offset, uint count)
    {
      return PhysFS.LowLevel.Write(handle, buffer, 1, count);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      Write(buffer, (uint)offset, (uint)count);
    }

    public override void SetLength(long value)
    {
      throw new NotImplementedException();
    }

    protected override void Dispose(bool disposing)
    {
      if(disposing)
      {
        if (handle != IntPtr.Zero)
        {
          PhysFS.LowLevel.Close(handle, physFS);
          handle = IntPtr.Zero;
        }
      }
      base.Dispose(disposing);
    }
  }
}
