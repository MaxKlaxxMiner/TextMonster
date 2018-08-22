#region # using *.*
using System.Runtime.CompilerServices;
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
  }
}
