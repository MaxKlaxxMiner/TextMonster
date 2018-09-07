using System.Runtime.CompilerServices;

namespace TextMonster
{
  /// <summary>
  /// Klasse zum berechnen von Crc64-Schlüsseln (FNV)
  /// </summary>
  public static class Crc64
  {
    /// <summary>
    /// Crc64 Startwert
    /// </summary>
    public const ulong Start = 0xcbf29ce484222325u;
    /// <summary>
    /// Crc64 Multiplikator
    /// </summary>
    public const ulong Mul = 0x100000001b3;

    /// <summary>
    /// erstellt eine Crc64-Prüfsumme
    /// </summary>
    /// <param name="wert">Datenwert, welcher einberechnet werden soll</param>
    /// <param name="offset">Startposition im Array</param>
    /// <param name="length">Länge der Daten</param>
    /// <returns>berechneter Crc64-Wert</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetCrc64(this char[] wert, int offset, int length)
    {
      ulong crc64 = Start;
      for (int i = 0; i < length; i++)
      {
        crc64 = (crc64 ^ wert[offset++]) * Mul;
      }
      return crc64;
    }

    /// <summary>
    /// erstellt eine Crc64-Prüfsumme
    /// </summary>
    /// <param name="wert">Datenwert, welcher einberechnet werden soll</param>
    /// <returns>berechneter Crc64-Wert</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetCrc64(this string wert)
    {
      ulong crc64 = Start;
      for (int i = 0; i < wert.Length; i++)
      {
        crc64 = (crc64 ^ wert[i]) * Mul;
      }
      return crc64;
    }
  }
}
