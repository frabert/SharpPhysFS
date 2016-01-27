using System;using System.IO;

namespace PhysFS
{
  public enum OpenMode
  {
    Append,
    Read,
    Write
  }

  public class PhysFSStream : Stream
  {
    IntPtr handle;
    bool readOnly = false;

    public PhysFSStream(string filename, OpenMode mode)
    {
      switch (mode)
      {
        case OpenMode.Append:
          handle = PhysFS.LowLevel.OpenAppend(filename);
          break;
        case OpenMode.Read:
          handle = PhysFS.LowLevel.OpenRead(filename);
          readOnly = true;
          break;
        case OpenMode.Write:
          handle = PhysFS.LowLevel.OpenAppend(filename);
          break;
      }
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
      PhysFS.LowLevel.Flush(handle);
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
        PhysFS.LowLevel.Seek(handle, (ulong)value);
      }
    }

    public long Read(byte[] buffer, uint offset, uint count)
    {
      return PhysFS.LowLevel.Read(handle, buffer, 1, count);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      return (int)Read(buffer, (uint)offset, (uint)count);
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

      PhysFS.LowLevel.Seek(handle, (ulong)(pos + offset));
      return pos + offset;
    }

    public override void SetLength(long value)
    {
      throw new NotImplementedException();
    }

    public long Write(byte[] buffer, uint offset, uint count)
    {
      return PhysFS.LowLevel.Write(handle, buffer, 1, count);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      Write(buffer, (uint)offset, (uint)count);
    }

    public override void Close()
    {
      PhysFS.LowLevel.Close(handle);
      handle = IntPtr.Zero;
      base.Close();
    }

    protected override void Dispose(bool disposing)
    {
      if(handle != IntPtr.Zero)
      {
        Close();
      }
      base.Dispose(disposing);
    }
  }
}
