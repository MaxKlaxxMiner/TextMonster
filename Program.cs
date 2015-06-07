using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMonsterSystem;
using TextMonsterSystem.Memory;

namespace TextMonsterConsole
{
  class Program
  {
    void Test1()
    {
      var t = new TextMemorySimple();
      var g = t.Insert(0, 'a');


    }

    void Run()
    {
      Test1();
    }

    static void Main()
    {
      new Program().Run();
    }
  }
}
