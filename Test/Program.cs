using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
  class Program
  {
    static void Main(string[] args)
    {
      PhysFS.PhysFS.InitializeCallbacks();
      PhysFS.PhysFS.Init("");
      var ver = PhysFS.PhysFS.GetLinkedVersion();
      Console.WriteLine("{0}.{1}.{2}", ver.major, ver.minor, ver.patch);
      PhysFS.PhysFS.Mount("D:\\", "/", true);
      var x = PhysFS.PhysFS.EnumerateFiles("/");
      PhysFS.PhysFS.Deinit();
      Console.ReadLine();
    }

  }
}
