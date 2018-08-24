#region # using *.*

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMember.Global
#endregion

namespace TextMonster
{
  /// <summary>
  /// Hilfmethoden auf Unsafe-Basis
  /// </summary>
  public unsafe static class UnsafeHelper
  {
    #region # // --- IndexOf ---
    /// <summary>
    /// sucht nach einem bestimmten Wert und gibt die entsprechende Offset-Position zurück.
    /// Achtung: das Byte muss(!) im Speicherbereich enthalten sein, da kein vorheriger Abbruch möglich ist!
    /// </summary>
    /// <param name="buf">Adresse, wo die Suche begonnen werden soll</param>
    /// <param name="b">Wert, welcher gesucht werden soll</param>
    /// <returns>Offset-Position, wo der Wert gefunden wurde</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int IndexOf(byte* buf, byte b)
    {
      if (*buf == b) return 0; // Quick-Check
      var p = (ulong*)buf;
      ulong val = *p++;
      for (; ; )
      {
        int d = _IndexOf8(val, b);
        if (d != 0) return (int)((byte*)p - buf + d);
        ulong sub = _IndexOfByteSub(b);
        for (; ; )
        {
          val = *p;
          if (_IndexOfHasByteSub(val, sub)) break;
          p++;
        }
        p++;
      }
    }

    /// <summary>
    /// gleiche wie IndexOf(), jedoch sollte der gesuchte Wert eine Konstante sein (bei Suchlängen unter 100 Bytes ist diese Variante bis 20% schneller)
    /// Achtung: das Byte muss(!) im Speicherbereich enthalten sein, da kein vorheriger Abbruch möglich ist!
    /// </summary>
    /// <param name="buf">Adresse, wo die Suche begonnen werden soll</param>
    /// <param name="b">Wert, welcher gesucht werden soll (sollte im optimalen Fall eine Konstante sein)</param>
    /// <returns>Offset-Position, wo der Wert gefunden wurde</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfConst(byte* buf, byte b)
    {
#if DEBUG
      return IndexOf(buf, b); // ist im Debug-Modus ca. 200% schneller nur im Release-Modus ca. 10-15% langsamer
#else
      if (b < (byte)' ')
      {
        var p = (ulong*)buf;
        for (; ; )
        {
          ulong val = *p++;
          if (!_IndexOfHasByteLow(val, b)) continue;

          int d = _IndexOf8(val, b);
          if (d != 0) return (int)((byte*)p - buf + d);
        }
      }
      else
      {
        var p = (ulong*)buf;
        ulong val = *p++;
        for (; ; )
        {
          int d = _IndexOf8(val, b);
          if (d != 0) return (int)((byte*)p - buf + d);
          for (; ; )
          {
            val = *p;
            if (_IndexOfHasByte(val, b)) break;
            p++;
          }
          p++;
        }
      }
#endif
    }

    /// <summary>
    /// (langsamer: 1,2x bis 5,0x fache längere Suche als IndexOf) sucht nach einem bestimmten Wert und gibt die entsprechende Offset-Position zurück oder -1 wenn nicht innerhalb des angegeben Bereiches gefunden wurde
    /// </summary>
    /// <param name="buf">Adresse, wo die Suche begonnen werden soll</param>
    /// <param name="b">Wert, welcher gesucht werden soll (sollte im optimalen Fall eine Konstante sein)</param>
    /// <param name="bufLength">maximale Länge des zu durchsuchenden Buffers</param>
    /// <returns>Offset-Position, wo der Wert gefunden wurde oder -1 wenn nicht gefunden</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfLimit(byte* buf, byte b, int bufLength)
    {
      int p = 0;
      while (buf[p] != b)
      {
        p++;
        if (p > bufLength) return -1;
      }
      return p;
    }

    /// <summary>
    /// sucht in einem ulong-Wert nach einem bestimmten Byte und gibt die Position minus 8 zurück oder 0 wenn nicht gefunden
    /// </summary>
    /// <param name="val">ulong-Wert, welcher durchsucht werden soll</param>
    /// <param name="b">Wert, welcher gesucht wird (sollte im optimalen Fall eine Konstante sein)</param>
    /// <returns>Position im Wert minus 8 oder 0 wenn nicht gefunden (-8 bis -1 oder 0)</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int _IndexOf8(ulong val, byte b)
    {
      if ((val & 0xffUL << 0) == (ulong)b << 0) return 0 - 8;
      if ((val & 0xffUL << 8) == (ulong)b << 8) return 1 - 8;
      if ((val & 0xffUL << 16) == (ulong)b << 16) return 2 - 8;
      if ((val & 0xffUL << 24) == (ulong)b << 24) return 3 - 8;
      if ((val & 0xffUL << 32) == (ulong)b << 32) return 4 - 8;
      if ((val & 0xffUL << 40) == (ulong)b << 40) return 5 - 8;
      if ((val & 0xffUL << 48) == (ulong)b << 48) return 6 - 8;
      if ((val & 0xffUL << 56) == (ulong)b << 56) return 7 - 8;
      return 0;
    }

    /// <summary>
    /// prüft, ob in einem ulong-Wert ein bestimmtes Byte enthalten ist (Achtung: falsch-positive Ergebnisse möglich)
    /// </summary>
    /// <param name="val">ulong-Wert, welcher durchsucht werden soll</param>
    /// <param name="b">Wert, welcher gesucht wird (sollte im optimalen Fall eine Konstante sein)</param>
    /// <returns>true wenn der gesuchte Wert oder ein ähnlicher Wert gefunden wurde</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool _IndexOfHasByte(ulong val, byte b)
    {
      ulong sub = _IndexOfByteSub(b);

      return _IndexOfHasByteSub(val, sub);
    }

    /// <summary>
    /// prüft, ob in einem ulong-Wert ein bestimmtes Byte enthalten ist (Achtung: falsch-positive Ergebnisse möglich)
    /// </summary>
    /// <param name="val">ulong-Wert, welcher durchsucht werden soll</param>
    /// <param name="b">Wert, welcher gesucht wird (sollte im optimalen Fall eine (kleine) Konstante sein)</param>
    /// <returns>true wenn der gesuchte Wert oder ein ähnlicher Wert gefunden wurde</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool _IndexOfHasByteLow(ulong val, byte b)
    {
      ulong sub = _IndexOfByteSub((byte)(b + 1));

      return ((val - sub) & 0x8080808080808080) != 0;
    }

    /// <summary>
    /// verketten ein Byte 8-fach in einem ulong-Wert
    /// </summary>
    /// <param name="b">Byte-Wert, welcher verkettet werden soll</param>
    /// <returns>fertiger Wert</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ulong _IndexOfByteSub(byte b)
    {
      return b | (ulong)b << 8 | (ulong)b << 16 | (ulong)b << 24 | (ulong)b << 32 | (ulong)b << 40 | (ulong)b << 48 | (ulong)b << 56;
    }

    /// <summary>
    /// prüft, ob in einem ulong-Wert ein bestimmtes Byte enthalten ist (Achtung: falsch-positive Ergebnisse möglich)
    /// </summary>
    /// <param name="val">ulong-Wert, welcher durchsucht werden soll</param>
    /// <param name="bSub">Kombi-Wert, welcher gesucht wird</param>
    /// <returns>true wenn der gesuchte Wert oder ein ähnlicher Wert gefunden wurde</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool _IndexOfHasByteSub(ulong val, ulong bSub)
    {
      return ((val - (bSub - 0x0101010101010101) & 0x7e7e7e7e7e7e7e7e) - 0x0101010101010101 & 0x8080808080808080) != 0;
    }
    #endregion

    #region # // --- MemMove ---
    /// <summary>
    /// kopiert einen Speicherbereich in einen anderen
    /// </summary>
    /// <param name="dest">Zieladresse, wohin der Bereich kopiert werden soll</param>
    /// <param name="src">Quelladresse, woher der Bereich gelesen werden soll</param>
    /// <param name="len">Anzahl der Bytes, welche kopiert werden sollen (muss größergleich 0 sein!)</param>
    public static void MemMove(byte* dest, byte* src, long len)
    {
      long offset = len & 15;
      switch (offset)
      {
        case 1: *dest = *src; break;
        case 2: *(short*)dest = *(short*)src; break;
        case 3: *(short*)dest = *(short*)src; *(dest + 2) = *(src + 2); break;
        case 4: *(int*)dest = *(int*)src; break;
        case 5: *(int*)dest = *(int*)src; *(dest + 4) = *(src + 4); break;
        case 6: *(int*)dest = *(int*)src; *(short*)(dest + 4) = *(short*)(src + 4); break;
        case 7: *(int*)dest = *(int*)src; *(int*)(dest + 3) = *(int*)(src + 3); break;
        case 8: *(long*)dest = *(long*)src; break;
        case 9: *(long*)dest = *(long*)src; *(dest + 8) = *(src + 8); break;
        case 10: *(long*)dest = *(long*)src; *(short*)(dest + 8) = *(short*)(src + 8); break;
        case 11: *(long*)dest = *(long*)src; *(int*)(dest + 7) = *(int*)(src + 7); break;
        case 12: *(long*)dest = *(long*)src; *(int*)(dest + 8) = *(int*)(src + 8); break;
        case 13: *(long*)dest = *(long*)src; *(long*)(dest + 5) = *(long*)(src + 5); break;
        case 14: *(long*)dest = *(long*)src; *(long*)(dest + 6) = *(long*)(src + 6); break;
        case 15: *(long*)dest = *(long*)src; *(long*)(dest + 7) = *(long*)(src + 7); break;
      }
      for (; offset != len; offset += 16)
      {
        *(long*)(dest + offset) = *(long*)(src + offset);
        *(long*)(dest + offset + 8) = *(long*)(src + offset + 8);
      }
    }
    #endregion

    #region # // --- CompareBytes ---
    /// <summary>
    /// vergleicht zwei Byte-Arrays gibt true zurück, wenn diese übereinstimmen, oder false wenn etwas nicht gepasst hat (es nie eine Exception geworfen)
    /// </summary>
    /// <param name="sourceBytes">Quelldaten, welche geprüft werden sollen</param>
    /// <param name="sourceOffset">Startposition in den Quelldaten</param>
    /// <param name="compareBytes">die zu vergleichenden Bytes</param>
    /// <param name="length">Länge der zu überprüfenden Bytes (muss kleinergleich compareBytes.Length sein)</param>
    /// <returns>true, wenn die Bytekette übereinstimmt oder false, wenn die Bytekette nicht gleich order fehlerhaft ist</returns>
    public static bool CompareBytes(byte[] sourceBytes, int sourceOffset, byte[] compareBytes, int length = 0)
    {
      if (sourceBytes == null || compareBytes == null) return false; // Null-Check
      if (length == 0) length = compareBytes.Length;
      if (sourceOffset < 0 || sourceOffset + length > sourceBytes.Length || length > compareBytes.Length || length <= 0) return false; // Bound-Checks

      fixed (byte* sourceP = &sourceBytes[sourceOffset])
      fixed (byte* compareP = &compareBytes[0])
      {
        return CompareBytesInternal(sourceP, compareP, length);
      }
    }

    /// <summary>
    /// vergleicht zwei Byte-Arrays gibt true zurück, wenn diese übereinstimmen, oder false wenn etwas nicht gepasst hat (es nie eine Exception geworfen)
    /// </summary>
    /// <param name="source">Quelldaten, welche geprüft werden sollen</param>
    /// <param name="compareBytes">die zu vergleichenden Bytes</param>
    /// <param name="length">Länge der zu überprüfenden Bytes (muss kleinergleich compareBytes.Length sein)</param>
    /// <returns>true, wenn die Bytekette übereinstimmt oder false, wenn die Bytekette nicht gleich order fehlerhaft ist</returns>
    public static bool CompareBytes(byte* source, byte[] compareBytes, int length = 0)
    {
      if (compareBytes == null) return false; // Null-Check
      if (length == 0) length = compareBytes.Length;
      if (length > compareBytes.Length || length <= 0) return false; // Bound-Checks

      fixed (byte* compareP = &compareBytes[0])
      {
        return CompareBytesInternal(source, compareP, length);
      }
    }

    /// <summary>
    /// vergleicht zwei Speicherbereiche
    /// </summary>
    /// <param name="src">Zeiger auf Quelldaten, welche verglichen werden sollen</param>
    /// <param name="cmp">Zeiger auf Vergleichsdaten, welche verglichen werden sollen</param>
    /// <param name="len">Länge der zu vergleichen Daten in Bytes</param>
    /// <returns>true, wenn alle Bytes übereinstimmen</returns>
    static bool CompareBytesInternal(byte* src, byte* cmp, int len)
    {
      int i = 0;

      // --- 64-Bit Schnellvergleich ---
      len -= sizeof(long);
      for (; i <= len; i += sizeof(long))
      {
        if (*(long*)(src + i) != *(long*)(cmp + i)) return false;
      }
      len += sizeof(long);

      // --- restliche Bytes vergleichen ---
      for (; i < len; i++)
      {
        if (src[i] != cmp[i]) return false;
      }

      return true; // keinen Unterschied gefunden
    }
    #endregion

    #region # // --- GetString ---
    /// <summary>
    /// Latin1-Zeichensatz (wie in MySql)
    /// </summary>
    public static readonly Encoding Latin1 = Encoding.GetEncoding("ISO-8859-1");

    /// <summary>
    /// merkt sich den UTF8-Decoder
    /// </summary>
    static readonly Decoder Utf8Decoder = Encoding.UTF8.GetDecoder();

    /// <summary>
    /// schnelle Methode um einen leeren String zu erstellen
    /// </summary>
    public static readonly Func<int, string> FastAllocateString = GenFastAllocateString();

    /// <summary>
    /// gibt die Methode "string.FastAllocateString" zurück
    /// </summary>
    /// <returns>Delegate auf die Methode</returns>
    static Func<int, string> GenFastAllocateString()
    {
      try
      {
        return (Func<int, string>)Delegate.CreateDelegate(typeof(Func<int, string>), typeof(string).GetMethod("FastAllocateString", BindingFlags.NonPublic | BindingFlags.Static));
      }
      catch
      {
        return count => new string('\0', count); // Fallback 
      }
    }

    /// <summary>
    /// prüft, ob in einem bestimmten Speicherbereich Utf8-Zeichen bzw. andere Sonderzeichen (größer als 0x7f) vorhanden sind
    /// </summary>
    /// <param name="buf">Buffer, welcher geprüft werden soll</param>
    /// <param name="bytes">Anzahl der zu überprüfenden Bytes</param>
    /// <returns>true, wenn Utf8- oder andere Sonderzeichen gefunden wurden</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckUtf8Chars(byte* buf, int bytes)
    {
      int bytes8 = bytes & 0x7fffffff - 7;
      int p;
      for (p = 0; p < bytes8; p += 8)
      {
        if ((*(ulong*)(buf + p) & 0x8080808080808080) != 0) return true;
      }
      for (; p < bytes; p++)
      {
        if ((buf[p] & 0x80) != 0) return true;
      }
      return false; // keine Utf8-Zeichen gefunden
    }

    /// <summary>
    /// dekodiert einen Utf8-String
    /// </summary>
    /// <param name="buf">Buffer, welcher ausgelesen werden soll</param>
    /// <param name="bytes">Anzahl der Bytes, welche gelesen werden sollen</param>
    /// <returns>fertig dekodierte Zeichenkette</returns>
    public static string GetUtf8String(byte* buf, int bytes)
    {
      if (bytes <= 0) return "";
      if (!CheckUtf8Chars(buf, bytes)) return GetLatin1String(buf, bytes);

      int chars = Utf8Decoder.GetCharCount(buf, bytes, false);
      string output = FastAllocateString(chars);
      fixed (char* cP = output)
      {
        Utf8Decoder.GetChars(buf, bytes, cP, chars, false);
      }
      return output;
    }

    /// <summary>
    /// dekodiert einen Latin1-String
    /// </summary>
    /// <param name="buf">Buffer, welcher ausgelesen werden soll</param>
    /// <param name="bytes">Anzahl der Bytes, welche gelesen werden sollen</param>
    /// <returns>fertig dekodierte Zeichenkette</returns>
    public static string GetLatin1String(byte* buf, int bytes)
    {
      if (bytes <= 0) return "";
      string output = FastAllocateString(bytes);
      fixed (char* cP = output)
      {
        for (int i = 0; i < bytes; i++) cP[i] = (char)buf[i];
      }
      return output;
    }
    #endregion
  }
}
