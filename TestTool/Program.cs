#region # using *.*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace TestTool
{
  class Program
  {
    static void Main(string[] args)
    {
      long len = 1;
      for (; ; )
      {
        Console.Write("create {0:N2} kByte Files:", len / 1024.0);
        Console.ReadLine();
        Console.WriteLine(TestFile.CreateFilePrime(TestFile.FileType.Binary, len));
        Console.WriteLine(TestFile.CreateFilePrime(TestFile.FileType.Text, len));
        Console.WriteLine(TestFile.CreateFilePrime(TestFile.FileType.Xml, len));
        Console.WriteLine(TestFile.CreateFilePrime(TestFile.FileType.Json, len));
        Console.WriteLine(TestFile.CreateFilePrime(TestFile.FileType.Tsv, len));
        Console.WriteLine(TestFile.CreateFilePrime(TestFile.FileType.Csv, len));
        len *= 10;
      }
    }
  }
}
