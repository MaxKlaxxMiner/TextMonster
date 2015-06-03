using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMonster;
using TextMonster.TextMemory;

namespace TextMonsterConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      for (int i = 0; i < 10; i++)
      {
        GC.Collect();
        long ramInit = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
        TextMemorySimple dummy = new TextMemorySimple();
      }

      TextMemorySimple test = new TextMemorySimple();

      long ramIst = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
      GC.Collect();
      long ramStart = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
      Console.WriteLine((ramStart - ramStart).ToString("#,##0"));
      GC.Collect();
      Console.ReadLine();

      for (; ; )
      {
        GC.Collect();
        ramIst = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
        if (test.mem.Count < 10) ramStart = ramIst;
        GC.Collect();
        Console.Write((ramIst - ramStart).ToString("#,##0") + " - (" + test.mem.Count.ToString("#,##0") + ") - " + test.ByteUsedRam.ToString("#,##0"));
        Console.ReadLine();
        int bis = 1;
        if (test.mem.Count < 536870900) bis = 536870900;
        try
        {
          for (int i = 0; i < bis; i++) test.mem.Add((char)i);
        }
        catch 
        {
          Console.Write((ramIst - ramStart).ToString("#,##0") + " - (" + test.mem.Count.ToString("#,##0") + ")");
          for (; ; ) Console.ReadLine();
        }
      }
    }
  }
}
