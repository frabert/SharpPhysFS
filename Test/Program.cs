using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
  class Program
  {
    static void PrintSupportedArchives()
    {
      Console.Write("Supported archive types: ");
      bool any = false;
      foreach (var archive in PhysFS.PhysFS.SupportedArchiveTypes())
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

    static Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>();

    #region Commands
    static void Help(string[] args)
    {
      Console.WriteLine("Commands:");
      foreach (var kvp in commands)
      {
        Console.WriteLine(" - {0}", kvp.Key);
      }
    }

    static void Mount(string[] args)
    {
      if (args.Length < 3)
      {
        Console.WriteLine("Usage: mount <archive> <mntpoint> <append>");
        return;
      }
      bool append;
      if (!bool.TryParse(args[2], out append))
      {
        Console.WriteLine("append can only be true or false");
      }

      PhysFS.PhysFS.Mount(args[0], args[1], append);
    }

    static void Enumerate(string[] args)
    {
      if (args.Length < 1)
      {
        Console.WriteLine("Usage: enumerate/ls <dir>");
        return;
      }

      foreach (var f in PhysFS.PhysFS.EnumerateFiles(args[0]))
      {
        Console.WriteLine(" - {0}", f);
      }
    }

    static void GetLastError(string[] args)
    {
      Console.WriteLine(PhysFS.PhysFS.GetLastError());
    }
    #endregion

    static void Main(string[] args)
    {
      try
      {
        PhysFS.PhysFS.InitializeCallbacks();
      }
      catch (PhysFS.PhysFSLibNotFound)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine("ERROR: PhysFS could not be loaded. Are you sure it is installed or a suitable module is in your working directory?");
        return;
      }
      PhysFS.PhysFS.Init("");

      var version = PhysFS.PhysFS.GetLinkedVersion();

      Console.WriteLine("SharpPhysFS Test console");
      Console.WriteLine("Loaded PhysFS version: {0}.{1}.{2}", version.major, version.minor, version.patch);
      PrintSupportedArchives();

      Console.WriteLine("Type 'help' for a list of commands");

      commands.Add("help", Help);
      commands.Add("mount", Mount);
      commands.Add("enumerate", Enumerate);
      commands.Add("ls", Enumerate);

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
          Action<string[]> cmd;
          if (commands.TryGetValue(split.First(), out cmd))
          {
            try
            {
              cmd(split.Skip(1).ToArray());
              Console.WriteLine("Done.");
            }
            catch (PhysFS.PhysFSException e)
            {
              Console.Error.WriteLine("ERROR: {0}", e.Message);
            }
          }
          else
          {
            Console.Error.WriteLine("Invalid command");
          }
        }
      }

      PhysFS.PhysFS.Deinit();
    }
  }
}
