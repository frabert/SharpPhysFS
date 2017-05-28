using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpPhysFS;

namespace Test
{
  class Program
  {
    static PhysFS physFS;

    static void PrintSupportedArchives()
    {
      Console.Write("Supported archive types: ");
      bool any = false;
      foreach (var archive in physFS.SupportedArchiveTypes())
      {
        any = true;
        Console.WriteLine("\n - {0}: {1}", archive.extension, archive.description);
        Console.WriteLine("   Written by {0}", archive.author);
        Console.Write("   {0}", archive.url);
      }
      if (!any)
      {
        Console.WriteLine("NONE.");
      }
      else
      {
        Console.WriteLine();
      }
    }

    static IEnumerable<string> ParseInput(string input)
    {
      var sb = new StringBuilder();
      bool openString = false;
      foreach (var c in input)
      {
        if (char.IsWhiteSpace(c))
        {
          if (!openString)
          {
            if (sb.ToString() != "")
            {
              yield return sb.ToString();
            }
            sb.Clear();
            continue;
          }
          else
          {
            sb.Append(c);
          }
        }

        if (c == '"')
        {
          if (sb.ToString() != "")
          {
            yield return sb.ToString();
          }

          sb.Clear();

          openString = !openString;
        }
        else
        {
          sb.Append(c);
        }
      }

      if (sb.ToString() != "")
      {
        yield return sb.ToString();
      }
    }

    static Dictionary<string, Func<string[], bool>> commands = new Dictionary<string, Func<string[], bool>>();

    #region Commands
    static bool Help(string[] args)
    {
      Console.WriteLine("Commands:");
      foreach (var kvp in commands)
      {
        Console.WriteLine(" - {0}", kvp.Key);
      }
      return true;
    }

    static bool Mount(string[] args)
    {
      if (args.Length < 3)
      {
        Console.WriteLine("Usage: mount <archive> <mntpoint> <append>");
        return false;
      }
      bool append;
      if (!bool.TryParse(args[2], out append))
      {
        Console.WriteLine("append can only be true or false");
      }

      physFS.Mount(args[0], args[1], append);
      return true;
    }

    static bool Enumerate(string[] args)
    {
      if (args.Length < 1)
      {
        Console.WriteLine("Usage: enumerate/ls <dir>");
        return false;
      }

      foreach (var f in physFS.EnumerateFiles(args[0]))
      {
        Console.WriteLine(" - {0}", f);
      }
      return true;
    }

    static bool GetLastError(string[] args)
    {
      Console.WriteLine(physFS.GetLastError());
      return true;
    }

    static bool GetDirSeparator(string[] args)
    {
      Console.WriteLine(physFS.GetDirSeparator());
      return true;
    }

    static bool GetCdRomDirectories(string[] args)
    {
      foreach(var d in physFS.GetCdRomDirs())
      {
        Console.WriteLine(" - {0}", d);
      }
      return true;
    }

    static bool GetSearchPath(string[] args)
    {
      foreach (var d in physFS.GetSearchPath())
      {
        Console.WriteLine(" - {0}", d);
      }
      return true;
    }

    static bool GetBaseDirectory(string[] args)
    {
      Console.WriteLine(physFS.GetBaseDir());
      return true;
    }

    static bool GetUserDirectory(string[] args)
    {
      Console.WriteLine(physFS.GetUserDir());
      return true;
    }

    static bool GetWriteDirectory(string[] args)
    {
      Console.WriteLine(physFS.GetWriteDir());
      return true;
    }

    static bool SetWriteDirectory(string[] args)
    {
      if (args.Length < 1)
      {
        Console.WriteLine("Usage: setwritedir <dir>");
        return false;
      }
      physFS.SetWriteDir(args[0]);
      return true;
    }

    static bool PermitSymlinks(string[] args)
    {
      if (args.Length < 1)
      {
        Console.WriteLine("Usage: permitsymlinks <true/false>");
        return false;
      }
      bool permit;
      if (!bool.TryParse(args[0], out permit))
      {
        Console.WriteLine("Usage: permitsymlinks <true/false>");
      }
      physFS.PermitSymbolicLinks(permit);
      return true;
    }

    static bool SetSaneConfig(string[] args)
    {
      if(args.Length < 5)
      {
        Console.WriteLine("Usage: setsaneconfig <org> <appName> <arcExt> <includeCdRoms> <archivesFirst>");
        return false;
      }
      bool includeCdRoms, archivesFirst;
      if(bool.TryParse(args[3], out includeCdRoms) && bool.TryParse(args[4], out archivesFirst))
      {
        physFS.SetSaneConfig(args[0], args[1], args[2], includeCdRoms, archivesFirst);
      }
      else
      {
        Console.WriteLine("Usage: setsaneconfig <org> <appName> <arcExt> <includeCdRoms> <archivesFirst>");
      }
      return true;
    }

    static bool MkDir(string[] args)
    {
      if (args.Length < 1)
      {
        Console.WriteLine("Usage: mkdir <dir>");
        return false;
      }
      physFS.Mkdir(args[0]);
      return true;
    }

    static bool Delete(string[] args)
    {
      if (args.Length < 1)
      {
        Console.WriteLine("Usage: delete <dir>");
        return false;
      }
      physFS.Delete(args[0]);
      return true;
    }

    static bool GetRealDir(string[] args)
    {
      if (args.Length < 1)
      {
        Console.WriteLine("Usage: getrealdir <dir>");
        return false;
      }
      Console.WriteLine(physFS.GetRealDir(args[0]));
      return true;
    }

    static bool Exists(string[] args)
    {
      if (args.Length < 1)
      {
        Console.WriteLine("Usage: exists <file>");
        return false;
      }
      Console.WriteLine(physFS.Exists(args[0]));
      return true;
    }

    static bool IsDir(string[] args)
    {
      if (args.Length < 1)
      {
        Console.WriteLine("Usage: isdir <path>");
        return false;
      }
      Console.WriteLine(physFS.IsDirectory(args[0]));
      return true;
    }

    static bool IsSymlink(string[] args)
    {
      if (args.Length < 1)
      {
        Console.WriteLine("Usage: issymlink <path>");
        return false;
      }
      Console.WriteLine(physFS.IsSymbolicLink(args[0]));
      return true;
    }

    static bool Cat(string[] args)
    {
      if (args.Length < 1)
      {
        Console.WriteLine("Usage: cat <file>");
        return false;
      }

      using (var stream = physFS.OpenRead(args[0]))
      using (var reader = new System.IO.StreamReader(stream))
      {
        Console.WriteLine(reader.ReadToEnd());
      }
      return true;
    }

    static bool FileLength(string[] args)
    {
      if (args.Length < 1)
      {
        Console.WriteLine("Usage: filelength <file>");
        return false;
      }
      using (var stream = physFS.OpenRead(args[0]))
      {
        Console.WriteLine(stream.Length);
      }
      return true;
    }

    static bool GetMountPoint(string[] args)
    {
      if (args.Length < 1)
      {
        Console.WriteLine("Usage: getmountpoint <file>");
        return false;
      }
      Console.WriteLine(physFS.GetMountPoint(args[0]));
      return true;
    }

    #endregion

    static void Main(string[] args)
    {
      try
      {
        physFS = new PhysFS("");
      }
      catch (PhysFSLibNotFound)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine("ERROR: PhysFS could not be loaded. Are you sure it is installed or a suitable module is in your working directory?");
        return;
      }

      var version = physFS.GetLinkedVersion();

      Console.WriteLine("SharpPhysFS Test console");
      Console.WriteLine("Loaded PhysFS version: {0}.{1}.{2}", version.major, version.minor, version.patch);
      PrintSupportedArchives();

      Console.WriteLine("Type 'help' for a list of commands");

      commands.Add("help", Help);
      commands.Add("mount", Mount);
      commands.Add("enumerate", Enumerate);
      commands.Add("ls", Enumerate);
      commands.Add("getdirsep", GetDirSeparator);
      commands.Add("getcdromdirs", GetCdRomDirectories);
      commands.Add("getsearchpath", GetSearchPath);
      commands.Add("getbasedir", GetBaseDirectory);
      commands.Add("getuserdir", GetUserDirectory);
      commands.Add("getwritedir", GetWriteDirectory);
      commands.Add("setwritedir", SetWriteDirectory);
      commands.Add("permitsymlinks", PermitSymlinks);
      commands.Add("setsaneconfig", SetSaneConfig);
      commands.Add("mkdir", MkDir);
      commands.Add("delete", Delete);
      commands.Add("getrealdir", GetRealDir);
      commands.Add("exists", Exists);
      commands.Add("isdir", IsDir);
      commands.Add("issymlink", IsSymlink);
      commands.Add("cat", Cat);
      commands.Add("filelength", FileLength);
      commands.Add("getmountpoint", GetMountPoint);

      while (true)
      {
        Console.Write("> ");
        var input = Console.ReadLine();
        var split = ParseInput(input);
        if (split.Count() == 0)
        {
          continue;
        }

        if (split.First() == "quit")
        {
          break;
        }
        else
        {
          Func<string[], bool> cmd;
          if (commands.TryGetValue(split.First(), out cmd))
          {
            try
            {
              if(cmd(split.Skip(1).ToArray()))
              {
                Console.WriteLine("Done.");
              }
            }
            catch (PhysFS.PhysFSException e)
            {
              Console.ForegroundColor = ConsoleColor.Red;
              Console.Error.WriteLine("ERROR: {0}", e.Message);
              Console.ForegroundColor = ConsoleColor.Gray;
            }
          }
          else
          {
            Console.Error.WriteLine("Invalid command");
          }
        }
      }

      physFS.Dispose();
    }
  }
}
