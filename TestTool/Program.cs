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
    static void CreateTestFiles(long size)
    {
      Console.WriteLine(TestFile.CreateFilePrime(TestFile.FileType.Binary, size));
      Console.WriteLine(TestFile.CreateFilePrime(TestFile.FileType.Text, size));
      Console.WriteLine(TestFile.CreateFilePrime(TestFile.FileType.Xml, size));
      Console.WriteLine(TestFile.CreateFilePrime(TestFile.FileType.Json, size));
      Console.WriteLine(TestFile.CreateFilePrime(TestFile.FileType.Tsv, size));
      Console.WriteLine(TestFile.CreateFilePrime(TestFile.FileType.Csv, size));
    }

    static void CreateTestFiles()
    {
      CreateTestFiles(1);         // 1 Byte
      CreateTestFiles(100);       // 100 Bytes
      CreateTestFiles(10000);     // 10 kByte
      CreateTestFiles(1000000);   // 1 MByte
      CreateTestFiles(100000000); // 100 MByte
    }

    static void Main(string[] args)
    {
      CreateTestFiles();
    }
  }
}
