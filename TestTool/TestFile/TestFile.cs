#region # using *.*
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
#endregion

namespace TestTool
{
  /// <summary>
  /// Klasse zum erstellen und bereitstellen von Test-Dateien
  /// </summary>
  public class TestFile
  {
    #region # public enum FileType // Typ der Datei
    /// <summary>
    /// Typ der Datei
    /// </summary>
    public enum FileType
    {
      /// <summary>
      /// binäres Datenformat
      /// </summary>
      Binary,
      /// <summary>
      /// normaler Text
      /// </summary>
      Text,
      /// <summary>
      /// XML-Format
      /// </summary>
      Xml,
      /// <summary>
      /// JSON-Format
      /// </summary>
      Json,
      /// <summary>
      /// tabulator separated values (mit \t)
      /// </summary>
      Tsv,
      /// <summary>
      /// comma separated values (mit ;)
      /// </summary>
      Csv
    }
    #endregion

    #region # static IEnumerable<long> GeneratePrimesSlow() // generiert (theoretisch) unbegrenzt viele Primzahlen
    /// <summary>
    /// generiert (theoretisch) unbegrenzt viele Primzahlen
    /// </summary>
    /// <returns>Enumerable der Primzahlen</returns>
    static IEnumerable<long> GeneratePrimesSlow()
    {
      yield return 2;
      yield return 3;
      var knownPrimes = new List<long> { 3 };
      int searchTo = 0;
      for (long n = 5; ; n += 2)
      {
        bool noPrime = false;
        for (int i = 0; i < searchTo; i++)
        {
          if (n % knownPrimes[i] == 0)
          {
            noPrime = true;
            break;
          }
        }
        if (noPrime) continue;
        if (knownPrimes[searchTo] * knownPrimes[searchTo] == n)
        {
          searchTo++;
          continue;
        }
        yield return n;
        knownPrimes.Add(n);
      }
    }
    #endregion

    #region # static IEnumerable<long> GeneratePrimesFast() // generiert (theoretisch) unbegrenzt viele Primzahlen mit hoher Geschwindigkeit
    /// <summary>
    /// generiert (theoretisch) unbegrenzt viele Primzahlen mit hoher Geschwindigkeit
    /// </summary>
    /// <returns>Enumerable der Primzahlen</returns>
    static IEnumerable<long> GeneratePrimesFast()
    {
      yield return 2;
      var map = new ulong[10];
      long maxValue = map.Length * 128L;
      for (long n = 3; ; n += 2)
      {
        if ((map[n >> 7] & (1UL << (int)(n >> 1 & 0x3f))) == 0)
        {
          yield return n;
          long s = n * n;
          if (s > maxValue)
          {
            n += 2;
            for (; n < maxValue; n += 2)
            {
              if ((map[n >> 7] & (1UL << (int)(n >> 1 & 0x3f))) == 0) yield return n;
            }
            var newMap = new ulong[map.Length * 100];
            Array.Copy(map, newMap, map.Length);
            map = newMap;
            maxValue = map.Length * 128L;
            FillMap(map, maxValue);
            continue;
          }
          for (; s < maxValue; s += n << 1)
          {
            map[s >> 7] |= 1UL << (int)(s >> 1 & 0x3f);
          }
        }
      }
    }

    static void FillMap(ulong[] map, long maxValue)
    {
      foreach (var p in GeneratePrimesFast())
      {
        if (p == 2) continue;
        long s = p * p;
        if (s > maxValue) break;
        FillMapInternal(map, maxValue, s, p << 1);
      }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void FillMapInternal(ulong[] map, long maxValue, long s, long p)
    {
      for (; s < maxValue; s += p)
      {
        map[s >> 7] |= 1UL << (int)(s >> 1 & 0x3f);
      }
    }
    #endregion

    /// <summary>
    /// erstellt eine Test-Datei gefüllt mit Primzahlen (sofern noch nicht vorhanden)
    /// </summary>
    /// <param name="type">Datei-Typ, welcher verwendet werden soll</param>
    /// <param name="length">gewünschte Länge der Datei in Bytes</param>
    /// <returns>Name der Testdatei, welche erstellt wurde</returns>
    public static string CreateFilePrime(FileType type, long length)
    {
      string name = new DirectoryInfo("../TestFiles").FullName;
      Directory.CreateDirectory(name);
      name = name.TrimEnd('/', '\\') + "/primes_" + type + "_" + length;

      switch (type)
      {
        #region # case FileType.Binary:
        case FileType.Binary:
        {
          name += ".bin";
          if (!File.Exists(name)) // nur neu erstellen, falls noch nicht vorhanden
          {
            using (var wdat = File.Create(name))
            {
              long pos = 0;
              long count = 0;
              foreach (long prime in GeneratePrimesFast())
              {
                count++;
                wdat.Write(BitConverter.GetBytes(count), 0, sizeof(long)); pos += sizeof(long);
                wdat.Write(BitConverter.GetBytes(prime), 0, sizeof(long)); pos += sizeof(long);
                if (pos >= length) break; // Schleife abbrechen, wenn Länge erreicht wurde
              }
            }
          }
          return name;
        }
        #endregion
        #region # case FileType.Text:
        case FileType.Text:
        {
          name += ".txt";
          if (!File.Exists(name)) // nur neu erstellen, falls noch nicht vorhanden
          {
            using (var wdat = File.Create(name))
            {
              long pos = 0;
              long count = 0;
              foreach (long prime in GeneratePrimesFast())
              {
                count++;
                var buf = Encoding.ASCII.GetBytes(count + ": " + prime + "\r\n");
                wdat.Write(buf, 0, buf.Length);
                pos += buf.Length;
                if (pos >= length) break; // Schleife abbrechen, wenn Länge erreicht wurde
              }
            }
          }
          return name;
        }
        #endregion
        #region # case FileType.Xml:
        case FileType.Xml:
        {
          name += ".xml";
          if (!File.Exists(name)) // nur neu erstellen, falls noch nicht vorhanden
          {
            using (var wdat = File.Create(name))
            {
              long pos = 0;
              long count = 0;
              string tmp = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<primeNumbers>\r\n";
              wdat.Write(Encoding.ASCII.GetBytes(tmp), 0, tmp.Length);
              pos += tmp.Length;
              foreach (long prime in GeneratePrimesFast())
              {
                count++;
                var buf = Encoding.ASCII.GetBytes("  <primeNumber count=\"" + count + "\">" + prime + "</primeNumber>\r\n");
                wdat.Write(buf, 0, buf.Length);
                pos += buf.Length;
                if (pos >= length) break; // Schleife abbrechen, wenn Länge erreicht wurde
              }
              tmp = "</primeNumbers>\r\n";
              wdat.Write(Encoding.ASCII.GetBytes(tmp), 0, tmp.Length);
            }
          }
          return name;
        }
        #endregion
        #region # case FileType.Json:
        case FileType.Json:
        {
          name += ".json";
          if (!File.Exists(name)) // nur neu erstellen, falls noch nicht vorhanden
          {
            using (var wdat = File.Create(name))
            {
              long pos = 0;
              long count = 0;
              string tmp = "{\r\n  \"primeNumbers\": [\r\n";
              wdat.Write(Encoding.ASCII.GetBytes(tmp), 0, tmp.Length);
              pos += tmp.Length;
              foreach (long prime in GeneratePrimesFast())
              {
                count++;
                var buf = Encoding.ASCII.GetBytes("    { \"count\": " + count + ", \"primeNumber\": " + prime + " },\r\n");
                wdat.Write(buf, 0, buf.Length);
                pos += buf.Length;
                if (pos >= length) break; // Schleife abbrechen, wenn Länge erreicht wurde
              }
              wdat.Position -= 3;
              tmp = "\r\n  ]\r\n}\r\n";
              wdat.Write(Encoding.ASCII.GetBytes(tmp), 0, tmp.Length);
            }
          }
          return name;
        }
        #endregion
        #region # case FileType.Tsv:
        case FileType.Tsv:
        {
          name += ".tsv";
          if (!File.Exists(name)) // nur neu erstellen, falls noch nicht vorhanden
          {
            using (var wdat = File.Create(name))
            {
              long pos = 0;
              long count = 0;
              foreach (long prime in GeneratePrimesFast())
              {
                count++;
                var buf = Encoding.ASCII.GetBytes(count + "\t" + prime + "\r\n");
                wdat.Write(buf, 0, buf.Length);
                pos += buf.Length;
                if (pos >= length) break; // Schleife abbrechen, wenn Länge erreicht wurde
              }
            }
          }
          return name;
        }
        #endregion
        #region # case FileType.Csv:
        case FileType.Csv:
        {
          name += ".csv";
          if (!File.Exists(name)) // nur neu erstellen, falls noch nicht vorhanden
          {
            using (var wdat = File.Create(name))
            {
              long pos = 0;
              long count = 0;
              string tmp = "count;primeNumber\r\n";
              wdat.Write(Encoding.ASCII.GetBytes(tmp), 0, tmp.Length);
              pos += tmp.Length;
              foreach (long prime in GeneratePrimesFast())
              {
                count++;
                var buf = Encoding.ASCII.GetBytes(count + ";" + prime + "\r\n");
                wdat.Write(buf, 0, buf.Length);
                pos += buf.Length;
                if (pos >= length) break; // Schleife abbrechen, wenn Länge erreicht wurde
              }
            }
          }
          return name;
        }
        #endregion
        default: throw new NotSupportedException("filetype unknown: " + type);
      }
    }
  }
}
