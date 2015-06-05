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
    /// gibt die theoretisch maximale Anzahl der verarbeitbaren Zeichen zurück (absolutes Limit)
    /// </summary>
    long LengthMaximum { get; }

    /// <summary>
    /// gibt den aktuell genutzen Arbeitsspeicher zurück
    /// </summary>
    long ByteUsedRam { get; }

    /// <summary>
    /// gibt den aktuell genutzen Speicher auf der Festplatte zurück
    /// </summary>
    long ByteUsedDrive { get; }
  }
}
