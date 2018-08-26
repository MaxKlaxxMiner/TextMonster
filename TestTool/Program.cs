#region # using *.*
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TextMonster;
using TextMonster.Xml;

#endregion

namespace TestTool
{
  class Program
  {
    #region # // --- CreateTestFiles ---
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
    #endregion

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
      else if (testData is int)
      {
        Console.WriteLine(" total count:  {0:N0}", (int)testData);
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

    #region # // --- SpeedCheckBinary ---
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

    static void SpeedCheckBinary()
    {
      SpeedCheck("ReadTest()", ReadTest);
      SpeedCheck("ReadTestNative()", ReadTestNative);
      Console.Clear();
      for (int i = 0; i < 5; i++)
      {
        SpeedCheck("ReadTest()", ReadTest);
        SpeedCheck("ReadTestNative()", ReadTestNative);
      }
    }
    #endregion

    #region # // --- SpeedCheckTextLines ---
    static object LinesCount(string fileName)
    {
      int lineCount = 0;
      var mem = new MemoryStream(fullFile);
      using (var sr = new StreamReader(mem))
      {
        while (sr.ReadLine() != null) lineCount++;
      }
      return lineCount;
    }

    static object LinesCount2(string fileName)
    {
      int lineCount = 0;
      var mem = new MemoryStream(fullFile);
      using (var sr = new StreamReader(mem, UnsafeHelper.Latin1, false, 65536))
      {
        while (sr.ReadLine() != null) lineCount++;
      }
      return lineCount;
    }

    static object LinesCountRaw(string fileName)
    {
      int lineCount = 0;
      var buf = fullFile;
      for (int i = 0; i < buf.Length; i++)
      {
        if (buf[i] != '\n') continue;
        lineCount++;
      }
      return lineCount;
    }

    static unsafe int LinesCountRawXInternal(byte* buf, int last)
    {
      int lineCount = 0;
      int p = 0;
      for (; p < last; )
      {
        int next = UnsafeHelper.IndexOfConst(buf + p, (byte)'\n');
        p += next + 1;
        lineCount++;
      }
      return lineCount;
    }

    static unsafe object LinesCountRawX(string fileName)
    {
      fixed (byte* bufP = &fullFile[0])
      {
        int last = fullFile.Length;
        while (last > 0 && bufP[last] != '\n') last--;
        if (last == 0 && bufP[0] != '\n') return 0;
        return LinesCountRawXInternal(bufP, last);
      }
    }

    static byte[] fullFile;
    static void SpeedCheckTextLines()
    {
      fullFile = File.ReadAllBytes(TestFile.CreateFilePrime(TestFile.FileType.Xml, 100000000));

      SpeedCheck("LinesCount() - StreamReader", LinesCount);
      SpeedCheck("LinesCount2() - StreamReader optimized", LinesCount2);
      SpeedCheck("LinesCountRaw() - DirectBytes", LinesCountRaw);
      SpeedCheck("LinesCountRawX() - x64-Search", LinesCountRawX);
      for (int i = 0; i < 3; i++)
      {
        SpeedCheck("LinesCount() - StreamReader", LinesCount);
        SpeedCheck("LinesCount2() - StreamReader optimized", LinesCount2);
        SpeedCheck("LinesCountRaw() - DirectBytes", LinesCountRaw);
        SpeedCheck("LinesCountRawX() - x64-Search", LinesCountRawX);
      }
    }
    #endregion

    static object ParseXml1(string fileName)
    {
      int sum = 0;
      using (var rdat = File.OpenRead(fileName))
      {
        var xel = XElement.Load(rdat);
        foreach (var x in xel.Elements())
        {
          int count = x.Attribute("count").Value.Length;
          int value = x.Value.Length;
          sum += count + value;
        }
      }
      return sum;
    }

    static object ParseXml2(string fileName)
    {
      int sum = 0;
      using (var rdat = File.OpenRead(fileName))
      {
        var xel = X_Element.Load(rdat);
        foreach (var x in xel.Elements())
        {
          int count = x.Attribute("count").Value.Length;
          int value = x.Value.Length;
          sum += count + value;
        }
      }
      return sum;
    }

    static void SpeedCheckXmlParser()
    {
      //fullFile = File.ReadAllBytes(TestFile.CreateFilePrime(TestFile.FileType.Xml, 100000000));

      //SpeedCheck("ParseXml1() - XElement", ParseXml1);
      SpeedCheck("ParseXml2() - XElement", ParseXml2);
    }

    static void Main(string[] args)
    {
      //CreateTestFiles();

      //SpeedCheckBinary();
      //SpeedCheckTextLines();
      SpeedCheckXmlParser();
    }
  }
}
