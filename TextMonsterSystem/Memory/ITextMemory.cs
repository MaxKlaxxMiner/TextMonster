#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using TextMonsterSystem.Core;

#endregion

namespace TextMonsterSystem.Memory
{
  /// <summary>
  /// Interface für TextMemory-Systeme
  /// </summary>
  public abstract class ITextMemory : IDisposable
  {
    /// <summary>
    /// merkt sich die aktuelle Speicher-Revision
    /// </summary>
    internal long memRev;

    #region # // --- Properties ---
    /// <summary>
    /// gibt die aktuelle Anzahl der gespeicherten Zeichen zurück
    /// </summary>
    public abstract long Length { get; }

    /// <summary>
    /// gibt die theoretisch maximale Anzahl der verarbeitbaren Zeichen zurück (absolutes Limit)
    /// </summary>
    public abstract long LengthLimit { get; }

    /// <summary>
    /// gibt den aktuell genutzen Arbeitsspeicher zurück
    /// </summary>
    public abstract long ByteUsedRam { get; }

    /// <summary>
    /// gibt den aktuell genutzen Speicher auf der Festplatte zurück
    /// </summary>
    public abstract long ByteUsedDrive { get; }
    #endregion

    #region # // --- Methoden ---

    #region # // --- MemoryPos() ---
    /// <summary>
    /// aktualisiert eine Speicherposition auf die aktuelle interne Revision (sofern notwendig)
    /// </summary>
    /// <param name="memPos">Speicher-Position, welche aktualisiert werden soll</param>
    public abstract void UpdateMemoryPos(MemoryPos memPos);

    /// <summary>
    /// wandelt eine Speicherposition in eine Zeichen-Position um
    /// </summary>
    /// <param name="memPos">Speicherposition, welche umgerechnet werden soll</param>
    /// <returns>entsprechende Zeichenposition</returns>
    public abstract long GetCharPos(MemoryPos memPos);

    /// <summary>
    /// wandelt eine Zeichenposition in eine Speicherposition um
    /// </summary>
    /// <param name="charPos">Zeichenposition, welche umgerechnet werden soll</param>
    /// <returns>entsprechende Speicherposition</returns>
    public abstract MemoryPos GetMemoryPos(long charPos);
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
    #endregion

    #region # // --- Clear() ---
    /// <summary>
    /// löscht alle Zeichen aus dem Speicher
    /// </summary>
    public virtual void Clear()
    {
      Remove(0, this.Length);
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
