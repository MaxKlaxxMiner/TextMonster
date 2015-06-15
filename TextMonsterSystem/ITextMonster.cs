using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMonsterSystem.Memory;

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
    /// gibt den aktuell genutzen Arbeitsspeicher zurück
    /// </summary>
    public abstract long ByteUsedRam { get; }

    /// <summary>
    /// gibt den aktuell genutzen Speicher auf der Festplatte zurück
    /// </summary>
    public abstract long ByteUsedDrive { get; }
    #endregion

    #region # --- Methoden ---
    /// <summary>
    /// alle Ressourcen wieder frei geben
    /// </summary>
    public abstract void Dispose();
    #endregion
  }
}