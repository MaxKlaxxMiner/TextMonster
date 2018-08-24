#region # using *.*
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassCanBeSealed.Global
// ReSharper disable UnusedMember.Global
#endregion

namespace TextMonster
{
  /// <summary>
  /// Klasse zum schnellen einlesen einer Datei in den Arbeitsspeicher (im 64-Bit System keine Größenbegrenzung)
  /// </summary>
  public unsafe class UnsafeReader : IDisposable
  {
    #region # // --- Variablen ---
    /// <summary>
    /// merkt sich den Zeiger auf den entsprechenden Speicherbereich (Größe: memLength + BufSize)
    /// </summary>
    public readonly IntPtr memPointer;

    /// <summary>
    /// merkt sich die Anzahl der gespeicherten Bytes
    /// </summary>
    public readonly long memLength;

    /// <summary>
    /// Größe des Buffers, welche zum einlesen der Daten verwendet wird
    /// </summary>
    public const int BufSize = 65536;

    /// <summary>
    /// Zeichen, welche automatisch zum auffüllen mehrfach am Ende des Speichers angehangen werden, um bei unvollständigen Daten immer ein Ende zu garantieren
    /// </summary>
    public const string FillStopChars = "# [Fill-Bytes] \r\n\t'`%\";:< >()&?!=.,+-*/\\1234567890";

    /// <summary>
    /// gibt an, ob der Speicher schon freigegeben wurde
    /// </summary>
    public bool IsDisposed { get; private set; }
    #endregion

    #region # // --- Konstruktor ---
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="readFilename">Name der Datei, welch komplett eingelesen werden soll</param>
    /// <param name="statusPos">optional: Methode zum ausgeben der aktuellen Leseposition</param>
    public UnsafeReader(string readFilename, Action<long, long> statusPos = null) : this(File.OpenRead(readFilename), statusPos) { }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="readStream">Stream, woraus die Daten gelesen werden sollen</param>
    /// <param name="statusPos">optional: Methode zum ausgeben der aktuellen Leseposition</param>
    /// <param name="autoClose">optional: gibt an, ob der Stream nach dem einlesen automatisch geschlossen werden soll (default: true)</param>
    public UnsafeReader(Stream readStream, Action<long, long> statusPos = null, bool autoClose = true)
    {
      try
      {
        // --- Gesamtgröße der Daten addieren, falls der Stream eine Längenangabe unterstützt ---
        memLength = readStream.CanSeek ? Math.Max(BufSize, readStream.Length - readStream.Position) : BufSize;

        // --- ersten Hauptspeicher reservieren ---
        memPointer = Marshal.AllocHGlobal((IntPtr)(memLength + BufSize));
        if (memPointer == IntPtr.Zero) { IsDisposed = true; throw new OutOfMemoryException(); }

        // --- Daten des Streams einlesen und in den Speicherbereich kopieren ---
        long memPos = 0;
        var buf = new byte[BufSize];
        fixed (byte* bufP = buf)
        {
          int tick = 0;
          int lese;
          while ((lese = readStream.Read(buf, 0, BufSize)) > 0)
          {
            // --- Speicherbereich vergrößern, falls notwendig) ---
            if (memPos + lese > memLength)
            {
              long addLength = Math.Min(memLength * 3 + BufSize - 1, 1024L * 1024L * 1024L); // 3x Vergrößerung pro Schritt, max. jedoch 1 GB vergrößern
              memLength += addLength / BufSize * BufSize;
              memPointer = Marshal.ReAllocHGlobal(memPointer, (IntPtr)(memLength + BufSize));
              if (memPointer == IntPtr.Zero) { IsDisposed = true; throw new OutOfMemoryException(); }
            }

            // --- gelesene Daten in den Speicher kopieren ---
            UnsafeHelper.MemMove((byte*)memPointer + memPos, bufP, lese);
            memPos += lese;

            // --- Statusmeldungen ausgeben (mit Timer gesichert) ---
            if (Environment.TickCount > tick)
            {
              if (statusPos != null) statusPos(memPos, memLength);
              tick = Environment.TickCount + 100;
            }
          }

          // --- Ende des Speicherbereiches mit Stop-Zeichen befüllen ---
          for (int i = 0; i < BufSize; i++) bufP[i] = (byte)FillStopChars[i % FillStopChars.Length];
          UnsafeHelper.MemMove((byte*)memPointer + memPos, bufP, BufSize);
        }

        // --- Endgröße vergleichen und ggf. anpassen ---
        if (memLength != memPos)
        {
          memLength = memPos;
          memPointer = Marshal.ReAllocHGlobal(memPointer, (IntPtr)(memLength + BufSize));
          if (memPointer == IntPtr.Zero) { IsDisposed = true; throw new OutOfMemoryException(); }
        }

        if (statusPos != null) statusPos(memPos, memPos);
      }
      finally
      {
        if (autoClose) readStream.Close();
      }
    }
    #endregion

    #region # // --- Dispose ---
    /// <summary>
    /// gibt den benutzen Speicher wieder frei
    /// </summary>
    public void Dispose()
    {
      if (IsDisposed) return;
      Marshal.FreeHGlobal(memPointer);
      IsDisposed = true;
    }

    /// <summary>
    /// zusätzlicher Destructor, falls vergessen wurde den Speicher mit Dispose wieder freizugeben
    /// </summary>
    ~UnsafeReader()
    {
      Dispose();
    }
    #endregion

    #region # // --- Abfrage-Methoden ---
    /// <summary>
    /// gibt einen Teil des Speicherbereiches als eigenes Byte-Array zurück
    /// </summary>
    /// <param name="offset">Startposition im Speichern</param>
    /// <param name="count">Anzahl der Bytes, welche zurück gegeben werden sollen</param>
    /// <returns>fertiges Byte-Array mit den entsprechenden Daten</returns>
    public byte[] GetBytes(long offset, int count)
    {
      if (offset < 0 || offset > memLength + BufSize || count < 0 || offset + count > memLength + BufSize) throw new ArgumentOutOfRangeException();

      var output = new byte[count];

      fixed (byte* p = output)
      {
        UnsafeHelper.MemMove(p, (byte*)memPointer + offset, count);
      }

      return output;
    }

    /// <summary>
    /// vergleicht im Speicher eine bestimmte Byte-Kette und gibt true zurück, falls diese identisch ist
    /// </summary>
    /// <param name="offset">Startposition im Speicherbereich</param>
    /// <param name="compareBytes">Bytes, welche verglichen werden sollen</param>
    /// <returns>true, wenn die Bytes identisch sind, sonst false</returns>
    public bool CompareTo(long offset, byte[] compareBytes)
    {
      if (compareBytes == null || compareBytes.Length == 0) return true; // keine Daten, welche verglichen wurden -> Inhalt "stimmt überein"

      if (offset < 0 || offset > memLength || offset + compareBytes.Length > memLength) throw new ArgumentOutOfRangeException();

      return UnsafeHelper.CompareBytes((byte*)memPointer + offset, compareBytes);
    }

    /// <summary>
    /// erwartet eine bestimmt Bytekette im Speicher und gibt die Anzahl der entsprechenden Bytes zurück.
    /// Falls nicht gefunden, wird eine Fehlermeldung erzeugt.
    /// </summary>
    /// <param name="offset">Startposition im Speicherbereich</param>
    /// <param name="value">Byte-Kette, welche verglichen werden soll</param>
    /// <returns>Anzahl der Bytes, welche übersprungen werden können</returns>
    public long SkipKnown(long offset, byte[] value)
    {
      if (!CompareTo(offset, value))
      {
        throw new Exception("Byte-Kette im Speicher [" + offset.ToString("N0") + "] stimmt nicht überein");
      }

      return value.Length;
    }

    /// <summary>
    /// erwartet eine bestimmte Zeichenkette im Speicher (UTF8-kodiert) und gibt die Anzahl der entsprechenden Bytes zurück.
    /// Falls nicht gefunden, wird eine Fehlermeldung erzeugt.
    /// </summary>
    /// <param name="offset">Startposition im Speicherbereich</param>
    /// <param name="value">Zeichenkette, welche verglichen werden soll</param>
    /// <returns>Anzahl der Bytes, welche übersprungen werden können</returns>
    public long SkipKnown(long offset, string value)
    {
      if (string.IsNullOrEmpty(value)) return 0;

      var checkBytes = Encoding.UTF8.GetBytes(value);

      if (!CompareTo(offset, checkBytes))
      {
        string txt = Encoding.UTF8.GetString(GetBytes(offset, checkBytes.Length + 50));
        int p = -1;
        for (int i = value.Length - 1; i >= 0; i--) if (txt[i] != value[i]) p = i;

        throw new Exception("Zeichenkette im Speicher [" + offset.ToString("N0") + "] stimmt nicht überein: \"" + value + "\" (" + p + ")");
      }

      return checkBytes.Length;
    }

    /// <summary>
    /// Zählt die Anzahl der Zeilen ("\n")
    /// </summary>
    /// <param name="offset">optionale Start-Position</param>
    /// <param name="length">optionale Länge des zu prüfenden Bereiches</param>
    /// <returns>Anzahl der vorhandenen Zeilen</returns>
    public int CountLines(long offset = 0, long length = long.MaxValue)
    {
      long end = length == long.MaxValue ? memLength : offset + length;
      if (offset < 0 || offset > memLength || end < 0 || end > memLength || offset > end) throw new ArgumentOutOfRangeException();

      int count = 0;
      var p = (byte*)memPointer;
      while (offset <= end)
      {
        int nextLine = UnsafeHelper.IndexOfConst(p + offset, (byte)'\n') + 1;
        count++;
        offset += nextLine;
      }
      count--; // letzte Zeile ausserhalb der Bereiches wieder abziehen

      return count;
    }
    #endregion
  }
}
