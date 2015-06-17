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
  /// Textmonster-Klasse mit Basisfunktionalität
  /// </summary>
  public class TextMonsterSimple : ITextMonster
  {
    #region # // --- Variablen ---
    /// <summary>
    /// Klasse, welche sich den Textspeicher merkt
    /// </summary>
    ITextMemory mem;
    #endregion

    #region # // --- Konstruktor / Dispose ---
    /// <summary>
    /// Konstruktor
    /// </summary>
    public TextMonsterSimple()
    {
      mem = new TextMemorySimpleMinimal();
    }

    /// <summary>
    /// alle Ressourcen wieder frei geben
    /// </summary>
    public override void Dispose()
    {
      mem.Dispose();
    }
    #endregion

    #region # // --- Properties ---
    /// <summary>
    /// gibt die aktuelle Anzahl der gespeicherten Zeichen zurück
    /// </summary>
    public override long Length { get { return mem.Length; } }

    /// <summary>
    /// gibt die theoretisch maximale Anzahl der verarbeitbaren Zeichen zurück (absolutes Limit)
    /// </summary>
    public override long LengthLimit { get { return mem.LengthLimit; } }

    /// <summary>
    /// gibt den aktuell genutzen Arbeitsspeicher zurück
    /// </summary>
    public override long ByteUsedRam { get { return mem.ByteUsedRam; } }

    /// <summary>
    /// gibt den aktuell genutzen Speicher auf der Festplatte zurück
    /// </summary>
    public override long ByteUsedDrive { get { return mem.ByteUsedDrive; } }
    #endregion

    #region # // --- Methoden ---
    /// <summary>
    /// gibt die Speicherposition anhand einer absoluten Zeichenposition zurück
    /// </summary>
    /// <param name="charPos">Zeichenposition, welche abgefragt werden soll</param>
    /// <returns>passende Speicherposition</returns>
    public override MemoryPos GetMemoryPos(long charPos)
    {
      return mem.GetMemoryPos(charPos);
    }

    /// <summary>
    /// gibt die Speicherposition anhand einer Textposition (Zeile, Spalten) zurück
    /// </summary>
    /// <param name="textPos">Textposition mit Zeilen und Spalten, welche abgefragt werden soll</param>
    /// <returns>passende Speicherposition</returns>
    public override MemoryPos GetMemoryPos(TextPos textPos)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// gibt die Textposition (mit Zeilennummer und Spaltennummer) anhand einer Speicherposition zurück
    /// </summary>
    /// <param name="memPos">Speicherposition, welche abgefragt werden soll</param>
    /// <returns>passende Textposition</returns>
    public override TextPos GetTextPos(MemoryPos memPos)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// gibt die absolute Zeichenposition anhand einer Speicherposition zurück
    /// </summary>
    /// <param name="memPos">Speicherposition, welche abgefragt werden soll</param>
    /// <returns>passende absolute Zeichenposition</returns>
    public override long GetCharPos(MemoryPos memPos)
    {
      return mem.GetCharPos(memPos);
    }
    #endregion
  }
}
