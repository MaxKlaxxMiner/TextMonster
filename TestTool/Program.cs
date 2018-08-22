#region # using *.*
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
      Console.Clear();
    }

    static void SpeedCheck(string name, Func<string, object> method)
    {
      long oldMem = GC.GetTotalMemory(true);
      string fileName = TestFile.CreateFilePrime(TestFile.FileType.Xml, 100000000);
      var stop = Stopwatch.StartNew();
      var testData = method(fileName);
      stop.Stop();
      long newMem = GC.GetTotalMemory(true);
      if (testData == null) throw new Exception();

      Console.WriteLine();
      Console.WriteLine("  SpeedCheck:  100 MB XML - {0}", name);
      Console.WriteLine();
      if (testData is long)
      {
        Console.WriteLine(" total size:   {0:N2} MByte", (long)testData / 1048576.0);
        Console.WriteLine();
      }
      else
      {
        Console.WriteLine("  used memory: {0:N2} MByte", (newMem - oldMem) / 1048576.0);
        Console.WriteLine();
      }
      Console.WriteLine("  read time:   {0:N2} ms", 1000.0 / Stopwatch.Frequency * stop.ElapsedTicks);
      Console.WriteLine();
    }

    const int BufSize = 4096;

    static object ReadTest(string fileName)
    {
      var buf = new byte[BufSize];
      long bytesTotal = 0;

      using (var rdat = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, BufSize))
      {
        for (; ; )
        {
          int readBytes = rdat.Read(buf, 0, buf.Length);
          if (readBytes == 0) break;

          bytesTotal += readBytes;
        }
      }

      return bytesTotal;
    }

    static object ReadTestNative(string fileName)
    {
      var buf = new byte[BufSize];
      long bytesTotal = 0;

      var handle = Win32.CreateFileW(fileName, Win32.GenericRead, Win32.FileShareRead, IntPtr.Zero, Win32.OpenAlways, 0, IntPtr.Zero);
      for (; ; )
      {
        int readBytes;
        if (!Win32.ReadFile(handle, buf, buf.Length, out readBytes, IntPtr.Zero)) throw new IOException();
        if (readBytes == 0) break;

        bytesTotal += readBytes;
      }

      Win32.CloseHandle(handle);
      return bytesTotal;
    }

    static void Main(string[] args)
    {
      CreateTestFiles();

      SpeedCheck("ReadTest()", ReadTest);
      SpeedCheck("ReadTestNative()", ReadTestNative);
      Console.Clear();
      for (int i = 0; i < 5; i++)
      {
        SpeedCheck("ReadTest()", ReadTest);
        SpeedCheck("ReadTestNative()", ReadTestNative);
      }
    }
  }
}
