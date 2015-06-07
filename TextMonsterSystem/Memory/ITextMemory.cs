#region # using *.*

using System;
using System.Collections.Generic;

#endregion

namespace TextMonsterSystem.Memory
{
  /// <summary>
  /// Interface für TextMemory-Systeme
  /// </summary>
  public interface ITextMemory : IDisposable
  {
    /// <summary>
    /// gibt die aktuelle Anzahl der gespeicherten Zeichen zurück
    /// </summary>
    long Length { get; }

    /// <summary>
    /// gibt die theoretisch maximale Anzahl der verarbeitbaren Zeichen zurück (absolutes Limit)
    /// </summary>
    long LengthLimit { get; }

    /// <summary>
    /// gibt den aktuell genutzen Arbeitsspeicher zurück
    /// </summary>
    long ByteUsedRam { get; }

    /// <summary>
    /// gibt den aktuell genutzen Speicher auf der Festplatte zurück
    /// </summary>
    long ByteUsedDrive { get; }

    /// <summary>
    /// aktualisiert eine Speicherposition auf die aktuelle interne Revision (sofern notwendig)
    /// </summary>
    /// <param name="memPos">Speicher-Position, welche aktualisiert werden soll</param>
    void UpdateMemoryPos(ref MemoryPos memPos);

    /// <summary>
    /// fügt ein einzelnes Zeichen in den Speicher ein
    /// </summary>
    /// <param name="offset">Startposition, wo das Zeichen eingefügt werden soll</param>
    /// <param name="value">das Zeichen, welches eingefügt werden soll</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichens</returns>
    MemoryPos Insert(long offset, char value);

    /// <summary>
    /// fügt ein Zeichenarray in den Speicher ein
    /// </summary>
    /// <param name="offset">Startposition, wo das Zeichenarray eingefügt werden soll</param>
    /// <param name="values">die Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichenarrays</returns>
    MemoryPos Insert(long offset, char[] values);

    /// <summary>
    /// fügt eine Liste von Zeichen in den Speicher ein
    /// </summary>
    /// <param name="offset">Startposition, wo die Zeichen eingefügt werden sollen</param>
    /// <param name="values">Enumerable der Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende der eingefügten Zeichen</returns>
    MemoryPos Insert(long offset, IEnumerable<char> values);

    /// <summary>
    /// fügt ein einzelnes Zeichen in den Speicher ein
    /// </summary>
    /// <param name="offset">Startposition, wo das Zeichen eingefügt werden soll</param>
    /// <param name="value">das Zeichen, welches eingefügt werden soll</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichens</returns>
    MemoryPos Insert(MemoryPos offset, char value);

    /// <summary>
    /// fügt ein Zeichenarray in den Speicher ein
    /// </summary>
    /// <param name="offset">Startposition, wo das Zeichenarray eingefügt werden soll</param>
    /// <param name="values">die Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichenarrays</returns>
    MemoryPos Insert(MemoryPos offset, char[] values);

    /// <summary>
    /// fügt eine Liste von Zeichen in den Speicher ein
    /// </summary>
    /// <param name="offset">Startposition, wo die Zeichen eingefügt werden sollen</param>
    /// <param name="values">Enumerable der Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende der eingefügten Zeichen</returns>
    MemoryPos Insert(MemoryPos offset, IEnumerable<char> values);
  }
}
