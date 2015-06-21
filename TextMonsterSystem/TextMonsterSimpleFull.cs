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
  /// Textmonster-Klasse mit Basisfunktionalität
  /// </summary>
  public sealed class TextMonsterSimpleFull : ITextMonster
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
    public TextMonsterSimpleFull()
    {
      mem = new TextMemorySimpleFull();
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
    /// gibt die Anzahl der Zeilen zurück
    /// </summary>
    public override long Lines { get { return 1L; } }

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

    #region # // --- GetPos() ---
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
    /// gibt die Textposition (mit Zeilennummer und Spaltennummer) anhand einer absoluten Zeichenposition zurück
    /// </summary>
    /// <param name="charPos">absolute Zeichenposition, welche abgefragt werden soll</param>
    /// <returns>passende Textposition</returns>
    public override TextPos GetTextPos(long charPos)
    {
      return GetTextPos(mem.GetMemoryPos(charPos));
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

    /// <summary>
    /// gibt die absolute Zeichenposition anhand einer Textposition (mit Zeilennummer und Spaltennummer) zurück
    /// </summary>
    /// <param name="textPos">Textposition, welche abgefragt werden soll</param>
    /// <returns>passende absolute Zeichenposition</returns>
    public override long GetCharPos(TextPos textPos)
    {
      return mem.GetCharPos(GetMemoryPos(textPos));
    }
    #endregion

    #region # // --- Insert() ---
    /// <summary>
    /// fügt ein einzelnes Zeichen in den Speicher ein
    /// </summary>
    /// <param name="memPos">Startposition, wo das Zeichen eingefügt werden soll</param>
    /// <param name="value">das Zeichen, welches eingefügt werden soll</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichens</returns>
    public override MemoryPos Insert(MemoryPos memPos, char value)
    {
      return mem.Insert(memPos, value);
    }

    /// <summary>
    /// fügt ein einzelnes Zeichen in den Speicher ein
    /// </summary>
    /// <param name="charPos">Zeichenposition, wo das Zeichen eingefügt werden soll</param>
    /// <param name="value">das Zeichen, welches eingefügt werden soll</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichens</returns>
    public override MemoryPos Insert(long charPos, char value)
    {
      return mem.Insert(charPos, value);
    }

    /// <summary>
    /// fügt eine Liste von Zeichen in den Speicher ein
    /// </summary>
    /// <param name="memPos">Startposition, wo die Zeichen eingefügt werden sollen</param>
    /// <param name="values">Enumerable der Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende der eingefügten Zeichen</returns>
    public override MemoryPos Insert(MemoryPos memPos, IEnumerable<char> values)
    {
      return mem.Insert(memPos, values);
    }

    /// <summary>
    /// fügt eine Liste von Zeichen in den Speicher ein
    /// </summary>
    /// <param name="charPos">Startposition, wo die Zeichen eingefügt werden sollen</param>
    /// <param name="values">Enumerable der Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende der eingefügten Zeichen</returns>
    public override MemoryPos Insert(long charPos, IEnumerable<char> values)
    {
      return mem.Insert(charPos, values);
    }
    #endregion

    #region # // --- Remove() ---
    /// <summary>
    /// löscht betimmte Zeichen aus dem Speicher
    /// </summary>
    /// <param name="memPosStart">Startposition, wo Daten im Speicher gelöscht werden sollen</param>
    /// <param name="memPosEnd">Endposition, bis zu den Daten, welche Daten gelöscht werden sollen</param>
    /// <returns>Länge der Daten, welche gelöscht wurden</returns>
    public override void Remove(MemoryPos memPosStart, MemoryPos memPosEnd)
    {
      mem.Remove(memPosStart, memPosEnd);
    }

    /// <summary>
    /// löscht bestimmte Zeichen aus dem Speicher
    /// </summary>
    /// <param name="charPos">Startposition, wo Zeichen im Speicher gelöscht werden sollen</param>
    /// <param name="length">Anzahl der Zeichen, welche gelöscht werden sollen</param>
    public override void Remove(long charPos, long length)
    {
      mem.Remove(charPos, length);
    }
    #endregion

    #region # // --- GetChars() ---
    /// <summary>
    /// gibt die Zeichen aus dem Speicher zurück
    /// </summary>
    /// <param name="memPosStart">Startposition, wo die Zeichen im Speicher gelesen werden sollen</param>
    /// <param name="memPosEnd">Endposition, der Zeichen im Speicher (exklusive)</param>
    /// <returns>Enumerable der entsprechenden Zeichen</returns>
    public override IEnumerable<char> GetChars(MemoryPos memPosStart, MemoryPos memPosEnd)
    {
      return mem.GetChars(memPosStart, memPosEnd);
    }

    /// <summary>
    /// gibt die Zeichen aus dem Speicher zurück
    /// </summary>
    /// <param name="charPos">Startposition, wo die Zeichen im Speicher gelesen werden sollen</param>
    /// <param name="length"></param>
    /// <returns></returns>
    public override char[] GetChars(long charPos, long length)
    {
      return mem.GetChars(charPos, length);
    }
    #endregion

    #region # // --- GetLine() ---
    /// <summary>
    /// gibt den Anfang einer Zeile zurück
    /// </summary>
    /// <param name="memPos">Speicherposition ab welcher gesucht werden soll</param>
    /// <returns>Speicherposition auf den Anfang der Zeile</returns>
    public override MemoryPos GetLineStart(MemoryPos memPos)
    {
      return mem.GetLineStart(memPos);
    }

    /// <summary>
    /// gibt das Ende einer Zeile zurück
    /// </summary>
    /// <param name="memPos">Speicherposition ab welcher gesucht werden soll</param>
    /// <returns>Speicherposition auf das Ende der Zeile (hinter dem letzten Zeichen)</returns>
    public override MemoryPos GetLineEnd(MemoryPos memPos)
    {
      return mem.GetLineEnd(memPos);
    }

    /// <summary>
    /// gibt Position einer gesamten Zeile zurück
    /// </summary>
    /// <param name="memPos">Speicherposition ab welcher gesucht werden soll</param>
    /// <returns>Position der gesamten Zeile (Anfang und Ende der Zeile)</returns>
    public override LinePos GetLine(MemoryPos memPos)
    {
      return new LinePos { lineStart = mem.GetLineStart(memPos), lineEnd = mem.GetLineEnd(memPos) };
    }
    #endregion

    #endregion

  }
}
