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
      while(true)
      {
        Console.Write("> ");
        var input = Console.ReadLine();
      }

      PhysFS.PhysFS.Deinit();
    }
  }
}
