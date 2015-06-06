#region # using *.*

using System;

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
    /// gibt die Anzahl der gespeicherten Zeichen als veränderbare MemoryPos zurück
    /// </summary>
    MemoryPos LengthP { get; }

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
  }
}
