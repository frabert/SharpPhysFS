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
      PhysFS.Interop.SetUpInterop();
      PhysFS.PhysFS.Init("");
      Console.WriteLine("Hello, world");
      PhysFS.PhysFS.Mount("D:\\", "/", true);
      var x = PhysFS.PhysFS.EnumerateFiles("/");
      PhysFS.PhysFS.Deinit();
    }

  }
}
