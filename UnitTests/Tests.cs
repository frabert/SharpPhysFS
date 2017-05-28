using System;
using System.IO;
using System.Linq;
using Xunit;
using SharpPhysFS;

namespace UnitTests
{
  public class Tests
  {
    [Fact]
    void IsInit()
    {
      using (var pfs = new PhysFS(""))
        Assert.True(pfs.IsInit(), "PhysFS was not initialized");
    }

    [Theory]
    [InlineData(2, 1, 0)]
    void VersionCheck(byte major, byte minor, byte patch)
    {
      using (var pfs = new PhysFS(""))
        Assert.Equal(new SharpPhysFS.Version() { major = major, minor = minor, patch = patch }, pfs.GetLinkedVersion());
    }

    [Fact]
    void DirSeparator()
    {
      using (var pfs = new PhysFS(""))
      {
        Assert.NotNull(pfs.GetDirSeparator());
        Assert.NotEqual("", pfs.GetDirSeparator());
      }
    }

    [Fact]
    void PermitSymbolicLinks()
    {
      using (var pfs = new PhysFS(""))
      {
        Assert.False(pfs.SymbolicLinksPermitted());
        pfs.PermitSymbolicLinks(true);
        Assert.True(pfs.SymbolicLinksPermitted());
        pfs.PermitSymbolicLinks(false);
        Assert.False(pfs.SymbolicLinksPermitted());
      }
    }

    [Fact]
    void Mounting()
    {
      using (var pfs = new PhysFS(""))
      {
        Assert.Empty(pfs.GetSearchPath());
        pfs.Mount("./", "/", false);
        Assert.Equal(new string[] { "./" }, pfs.GetSearchPath());
        Assert.Equal("/", pfs.GetMountPoint("./"));
        Assert.True(pfs.IsDirectory("/"));

        pfs.Mount("../", "foo", true);
        Assert.Equal(new string[] { "./", "../", }, pfs.GetSearchPath());
        Assert.Equal("foo/", pfs.GetMountPoint("../"));
        Assert.True(pfs.IsDirectory("/foo"));

        pfs.Mount("../../", "bar", false);
        Assert.Equal(new string[] { "../../", "./", "../", }, pfs.GetSearchPath());
        Assert.Equal("bar/", pfs.GetMountPoint("../../"));
        Assert.True(pfs.IsDirectory("/bar"));

        pfs.RemoveFromSearchPath("../");
        Assert.Equal(new string[] { "../../", "./", }, pfs.GetSearchPath());
      }
    }

    [Fact]
    void FileEnumeration()
    {
      using (var pfs = new PhysFS(""))
      {
        pfs.Mount("./", "/", false);

        var effectiveFiles = Directory.GetFiles("./").Select(x => Path.GetFileName(x)).ToArray();
        Array.Sort(effectiveFiles);
        var enumeratedFiles = pfs.EnumerateFiles("/");
        Array.Sort(enumeratedFiles);

        Assert.Equal(effectiveFiles, enumeratedFiles);
      }
    }

    [Fact]
    void DriveEnumeration()
    {
      using(var pfs = new PhysFS(""))
      {
        var effectiveCdDrives = DriveInfo.GetDrives()
          .Where(x => x.DriveType == DriveType.CDRom)
          .Select(x => x.RootDirectory.FullName)
          .ToArray();

        var enumeratedCdDrives = pfs.GetCdRomDirs();

        Array.Sort(effectiveCdDrives);
        Array.Sort(enumeratedCdDrives);

        Assert.Equal(effectiveCdDrives, enumeratedCdDrives);
      }
    }

    [Fact]
    void UserDirectory()
    {
      using(var pfs = new PhysFS(""))
      {
        var userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var pfsUserDirectory = pfs.GetUserDir();
        Assert.Equal(Path.GetPathRoot(userDirectory), Path.GetPathRoot(pfsUserDirectory));
      }
    }

    [Fact]
    void DirectoryManipulation()
    {
      using(var pfs = new PhysFS(""))
      {
        pfs.SetWriteDir("./");
        Assert.Equal("./", pfs.GetWriteDir());

        pfs.Mkdir("hello");
        Assert.True(Directory.Exists("./hello"));

        pfs.Delete("hello");
        Assert.False(Directory.Exists("./hello"));
      }
    }

    [Fact]
    void FileManipulation()
    {
      using (var pfs = new PhysFS(""))
      {
        pfs.SetWriteDir("./");
        pfs.Mount("./", "/", true);
        
        using(var sw = new StreamWriter(pfs.OpenWrite("foo")))
        {
          sw.Write("hello, world! èòàùã こんにちは世界 你好世界");
        }

        Assert.True(File.Exists("./foo"));

        var fileContent = File.ReadAllText("./foo");
        using(var sr = new StreamReader(pfs.OpenRead("foo")))
        {
          Assert.Equal(fileContent, sr.ReadToEnd());
        }

        using (var sw = new StreamWriter(pfs.OpenAppend("foo")))
        {
          sw.Write("foo");
        }
        Assert.Equal(fileContent + "foo", File.ReadAllText("./foo"));

        pfs.Delete("foo");
        Assert.False(File.Exists("./foo"));
      }
    }
  }
}
