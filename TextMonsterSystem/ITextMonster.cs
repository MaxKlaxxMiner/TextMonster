using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMonsterSystem.Core;
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
    /// gibt die Speicherposition anhand einer absoluten Zeichenposition zurück
    /// </summary>
    /// <param name="charPos">Zeichenposition, welche abgefragt werden soll</param>
    /// <returns>passende Speicherposition</returns>
    public abstract MemoryPos GetMemoryPos(long charPos);

    /// <summary>
    /// gibt die Speicherposition anhand einer Textposition (Zeile, Spalten) zurück
    /// </summary>
    /// <param name="textPos">Textposition mit Zeilen und Spalten, welche abgefragt werden soll</param>
    /// <returns>passende Speicherposition</returns>
    public abstract MemoryPos GetMemoryPos(TextPos textPos);

    /// <summary>
    /// gibt die Textposition (mit Zeilennummer und Spaltennummer) anhand einer absoluten Zeichenposition zurück
    /// </summary>
    /// <param name="charPos">absolute Zeichenposition, welche abgefragt werden soll</param>
    /// <returns>passende Textposition</returns>
    public virtual TextPos GetTextPos(long charPos)
    {
      return GetTextPos(GetMemoryPos(charPos));
    }

    /// <summary>
    /// gibt die Textposition (mit Zeilennummer und Spaltennummer) anhand einer Speicherposition zurück
    /// </summary>
    /// <param name="memPos">Speicherposition, welche abgefragt werden soll</param>
    /// <returns>passende Textposition</returns>
    public abstract TextPos GetTextPos(MemoryPos memPos);

    /// <summary>
    /// gibt die absolute Zeichenposition anhand einer Speicherposition zurück
    /// </summary>
    /// <param name="memPos">Speicherposition, welche abgefragt werden soll</param>
    /// <returns>passende absolute Zeichenposition</returns>
    public abstract long GetCharPos(MemoryPos memPos);

    /// <summary>
    /// gibt die absolute Zeichenposition anhand einer Textposition (mit Zeilennummer und Spaltennummer) zurück
    /// </summary>
    /// <param name="textPos">Textposition, welche abgefragt werden soll</param>
    /// <returns>passende absolute Zeichenposition</returns>
    public virtual long GetCharPos(TextPos textPos)
    {
      return GetCharPos(GetMemoryPos(textPos));
    }

    /// <summary>
    /// alle Ressourcen wieder frei geben
    /// </summary>
    public abstract void Dispose();
    #endregion
  }
}