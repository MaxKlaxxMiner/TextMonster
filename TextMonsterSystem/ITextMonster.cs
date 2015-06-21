#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMonsterSystem.Core;
using TextMonsterSystem.Memory;

#endregion

namespace TextMonsterSystem
{
  /// <summary>
  /// interface bzw. Basisklasse für alle Textmonster
  /// </summary>
  public abstract class ITextMonster : IDisposable
  {
    #region # --- Properties ---
    /// <summary>
    /// gibt die aktuelle Anzahl der gespeicherten Zeichen zurück
    /// </summary>
    public abstract long Length { get; }

    /// <summary>
    /// gibt die theoretisch maximale Anzahl der verarbeitbaren Zeichen zurück (absolutes Limit)
    /// </summary>
    public abstract long LengthLimit { get; }

    /// <summary>
    /// gibt die Anzahl der Zeilen zurück
    /// </summary>
    public abstract long Lines { get; }

    /// <summary>
    /// gibt den aktuell genutzen Arbeitsspeicher zurück
    /// </summary>
    public abstract long ByteUsedRam { get; }

    /// <summary>
    /// gibt den aktuell genutzen Speicher auf der Festplatte zurück
    /// </summary>
    public abstract long ByteUsedDrive { get; }
    #endregion

    #region # --- Methoden ---

    #region # // --- GetPos() ---
    /// <summary>
    /// gibt die Speicherposition anhand einer absoluten Zeichenposition zurück
    /// </summary>
    /// <param name="charPos">Zeichenposition, welche abgefragt werden soll</param>
    /// <returns>passende Speicherposition</returns>
    public abstract MemoryPos GetMemoryPos(long charPos);

    /// <summary>
    /// gibt die absolute Zeichenposition anhand einer Speicherposition zurück
    /// </summary>
    /// <param name="memPos">Speicherposition, welche abgefragt werden soll</param>
    /// <returns>passende absolute Zeichenposition</returns>
    public abstract long GetCharPos(MemoryPos memPos);
    #endregion

    #region # // --- Insert() ---
    /// <summary>
    /// fügt ein einzelnes Zeichen in den Speicher ein
    /// </summary>
    /// <param name="memPos">Startposition, wo das Zeichen eingefügt werden soll</param>
    /// <param name="value">das Zeichen, welches eingefügt werden soll</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichens</returns>
    public abstract MemoryPos Insert(MemoryPos memPos, char value);

    /// <summary>
    /// fügt ein einzelnes Zeichen in den Speicher ein
    /// </summary>
    /// <param name="charPos">Zeichenposition, wo das Zeichen eingefügt werden soll</param>
    /// <param name="value">das Zeichen, welches eingefügt werden soll</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichens</returns>
    public virtual MemoryPos Insert(long charPos, char value)
    {
      return Insert(GetMemoryPos(charPos), value);
    }

    /// <summary>
    /// fügt eine Liste von Zeichen in den Speicher ein
    /// </summary>
    /// <param name="memPos">Startposition, wo die Zeichen eingefügt werden sollen</param>
    /// <param name="values">Enumerable der Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende der eingefügten Zeichen</returns>
    public virtual MemoryPos Insert(MemoryPos memPos, IEnumerable<char> values)
    {
      return values.Aggregate(memPos, Insert);
    }

    /// <summary>
    /// fügt eine Liste von Zeichen in den Speicher ein
    /// </summary>
    /// <param name="charPos">Startposition, wo die Zeichen eingefügt werden sollen</param>
    /// <param name="values">Enumerable der Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende der eingefügten Zeichen</returns>
    public virtual MemoryPos Insert(long charPos, IEnumerable<char> values)
    {
      return Insert(GetMemoryPos(charPos), values);
    }
    #endregion

    #region # // --- Remove() ---
    /// <summary>
    /// löscht betimmte Zeichen aus dem Speicher
    /// </summary>
    /// <param name="memPosStart">Startposition, wo Daten im Speicher gelöscht werden sollen</param>
    /// <param name="memPosEnd">Endposition, bis zu den Daten, welche Daten gelöscht werden sollen</param>
    /// <returns>Länge der Daten, welche gelöscht wurden</returns>
    public abstract void Remove(MemoryPos memPosStart, MemoryPos memPosEnd);

    /// <summary>
    /// löscht bestimmte Zeichen aus dem Speicher
    /// </summary>
    /// <param name="charPos">Startposition, wo Zeichen im Speicher gelöscht werden sollen</param>
    /// <param name="length">Anzahl der Zeichen, welche gelöscht werden sollen</param>
    public virtual void Remove(long charPos, long length)
    {
      Remove(GetMemoryPos(charPos), GetMemoryPos(charPos + length));
    }
    #endregion

    #region # // --- GetChars() ---
    /// <summary>
    /// gibt die Zeichen aus dem Speicher zurück
    /// </summary>
    /// <param name="memPosStart">Startposition, wo die Zeichen im Speicher gelesen werden sollen</param>
    /// <param name="memPosEnd">Endposition, der Zeichen im Speicher (exklusive)</param>
    /// <returns>Enumerable der entsprechenden Zeichen</returns>
    public abstract IEnumerable<char> GetChars(MemoryPos memPosStart, MemoryPos memPosEnd);

    /// <summary>
    /// gibt die Zeichen aus dem Speicher zurück
    /// </summary>
    /// <param name="charPos">Startposition, wo die Zeichen im Speicher gelesen werden sollen</param>
    /// <param name="length"></param>
    /// <returns></returns>
    public virtual char[] GetChars(long charPos, long length)
    {
      return GetChars(GetMemoryPos(charPos), GetMemoryPos(charPos + length)).ToArray();
    }
    #endregion

    #region # // --- GetLine() ---
    /// <summary>
    /// gibt den Anfang einer Zeile zurück
    /// </summary>
    /// <param name="memPos">Speicherposition ab welcher gesucht werden soll</param>
    /// <returns>Speicherposition auf den Anfang der Zeile</returns>
    public abstract MemoryPos GetLineStart(MemoryPos memPos);

    /// <summary>
    /// gibt das Ende einer Zeile zurück
    /// </summary>
    /// <param name="memPos">Speicherposition ab welcher gesucht werden soll</param>
    /// <returns>Speicherposition auf das Ende der Zeile (hinter dem letzten Zeichen)</returns>
    public abstract MemoryPos GetLineEnd(MemoryPos memPos);

    /// <summary>
    /// gibt Position einer gesamten Zeile zurück
    /// </summary>
    /// <param name="memPos">Speicherposition ab welcher gesucht werden soll</param>
    /// <returns>Position der gesamten Zeile (Anfang und Ende der Zeile)</returns>
    public virtual LinePos GetLine(MemoryPos memPos)
    {
      return new LinePos { lineStart = GetLineStart(memPos), lineEnd = GetLineEnd(memPos) };
    }

    /// <summary>
    /// gibt die Position der nachfolgenden Zeile zurück
    /// </summary>
    /// <param name="linePos">vorherige Zeilenposition</param>
    /// <returns>nachfolgende Zeilenposition</returns>
    public virtual LinePos GetNextLine(LinePos linePos)
    {
      if (!linePos.Valid) return LinePos.InvalidPos;

      long p = GetCharPos(linePos.lineEnd) + 1;
      if (p > Length) return LinePos.InvalidPos;
      if (p < Length && GetChars(p, 1).First() == '\r') p++;

      MemoryPos nextLineStart = GetMemoryPos(p);
      return new LinePos { lineStart = nextLineStart, lineEnd = GetLineEnd(nextLineStart) };
    }

    /// <summary>
    /// gibt die Position der vorhergehenden Zeile zurück
    /// </summary>
    /// <param name="linePos">bisherige Zeilenposition</param>
    /// <returns>vorhergehende Zeilenposition</returns>
    public virtual LinePos GetPrevLine(LinePos linePos)
    {
      if (!linePos.Valid) return LinePos.InvalidPos;

      long p = GetCharPos(linePos.lineStart) - 1;
      if (p < 0) return LinePos.InvalidPos;
      if (p > 0 && GetChars(p - 1, 1).First() == '\r') p--;

      MemoryPos prevLineEnd = GetMemoryPos(p);
      return new LinePos { lineStart = GetLineStart(prevLineEnd), lineEnd = prevLineEnd };
    }

    /// <summary>
    /// gibt die Position einer bestimmten Zeile zurück
    /// </summary>
    /// <param name="lineNumber">Zeilennummer, welche zurück gegeben werden soll (beginnend bei 0)</param>
    /// <returns>entsprechende Zeilenposition</returns>
    public virtual LinePos GetLine(long lineNumber)
    {
      if (lineNumber < 0) return LinePos.InvalidPos;
      LinePos linePos = GetLine(GetMemoryPos(0L));
      while (lineNumber > 0 && linePos.Valid)
      {
        linePos = GetNextLine(linePos);
        lineNumber--;
      }
      return linePos;
    }
    #endregion

    #region # // --- Dispose() ---
    /// <summary>
    /// alle Ressourcen wieder frei geben
    /// </summary>
    public abstract void Dispose();
    #endregion

    #endregion

  }
}