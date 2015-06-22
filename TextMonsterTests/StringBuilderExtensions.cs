#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextMonsterSystem;
using TextMonsterSystem.Memory;

#endregion

namespace TextMonsterTests
{
  /// <summary>
  /// enthält Erweiterungmethoden für den StringBuilder
  /// </summary>
  public static class StringBuilderExtensions
  {
    #region # // --- kleine Tools für den StringBuilder ---
    
    #region # long Lines(this StringBuilder str) // gibt die Anzahl der Zeilen in einem Stringbuilder zurück
    /// <summary>
    /// gibt die Anzahl der Zeilen in einem Stringbuilder zurück
    /// </summary>
    /// <param name="str">StringBuilder, welcher ausgelesen werden soll</param>
    /// <returns>Anzahl der Zeilen, welche enthalten sind (mindestens 1)</returns>
    public static long Lines(this StringBuilder str)
    {
      long count = 1;
      for (int i = 0; i < str.Length; i++)
      {
        if (str[i] == '\n') count++;
      }
      return count;
    }
    #endregion

    #region # IEnumerable<string> ReadLines(this StringBuilder str) // gibt die einzelnen Zeilen zurück
    /// <summary>
    /// gibt die einzelnen Zeilen zurück
    /// </summary>
    /// <param name="str">StringBuilder, welcher ausgelesen werden soll</param>
    /// <returns>Enumerable der Zeilen, welche ausgelesen wurden</returns>
    public static IEnumerable<string> ReadLines(this StringBuilder str)
    {
      int last = 0;
      for (int i = 0; i <= str.Length; i++)
      {
        if (i == str.Length || str[i] == '\n')
        {
          int len = i - last;
          if (len > 0 && str[i - 1] == '\r') len--;
          yield return str.ToString(last, len);
          last = i + 1;
        }
      }
    }
    #endregion

    #region # string GetLine(this StringBuilder str, long lineNumber) // gibt eine bestimmte Zeile aus dem StringBuilder zurück
    /// <summary>
    /// gibt eine bestimmte Zeile aus dem StringBuilder zurück
    /// </summary>
    /// <param name="str">StringBuilder, welcher ausgelesen werden soll</param>
    /// <param name="lineNumber">Nummer der Zeile, welche zurück gegeben werden soll (beginnend bei 0)</param>
    /// <returns>ausgelesene Zeile oder "null" wenn die Zeilennummer ungültig ist</returns>
    public static string GetLine(this StringBuilder str, long lineNumber)
    {
      int last = -1;
      for (int i = 0; i < str.Length; i++)
      {
        if (str[i] == '\n')
        {
          if (lineNumber == 0)
          {
            int len = i - last - 1;
            if (len > 0 && str[i - 1] == '\r') len--;
            return str.ToString(last + 1, len);
          }
          lineNumber--;
          last = i;
        }
      }
      if (lineNumber != 0) return null;
      int len2 = str.Length - last - 1;
      if (len2 > 0 && str[str.Length - 1] == '\r') len2--;
      return str.ToString(last + 1, len2);
    }
    #endregion

    #region # int GetLineStart(this StringBuilder str, int pos) // gibt die Zeichenposition vom Anfang einer Zeile zurück
    /// <summary>
    /// gibt die Zeichenposition vom Anfang einer Zeile zurück
    /// </summary>
    /// <param name="str">StringBuilder, welcher durchsucht werden soll</param>
    /// <param name="pos">Zeichenposition, ab welcher gesucht werden soll</param>
    /// <returns>gefundene Zeichenposition am Anfang der Zeile</returns>
    public static int GetLineStart(this StringBuilder str, int pos)
    {
      while (pos > 0 && str[pos - 1] != '\n') pos--;
      return pos;
    }
    #endregion

    #region # int GetLineEnd(this StringBuilder str, int pos) // gibt die Zeichenposition vom Ende einer Zeile zurück (zeigt auf das erste Zeichen vom Zeilenumbruch)
    /// <summary>
    /// gibt die Zeichenposition vom Ende einer Zeile zurück (zeigt auf das erste Zeichen vom Zeilenumbruch)
    /// </summary>
    /// <param name="str">StringBuilder, welcher durchsucht werden soll</param>
    /// <param name="pos">Zeichenposition, ab welcher gesucht werden soll</param>
    /// <returns>gefunden Zeichenposition am Ende der Zeile</returns>
    public static int GetLineEnd(this StringBuilder str, int pos)
    {
      while (pos < str.Length && str[pos] != '\n') pos++;
      if (pos > 0 && str[pos - 1] == '\r') pos--;
      return pos;
    }
    #endregion

    #endregion

    #region # void Comp(this StringBuilder str, ITextMemory mem) // vergleicht den kompletten Inhalt mit Speichersystem
    /// <summary>
    /// vergleicht den kompletten Inhalt mit einem Speichersystem
    /// </summary>
    /// <param name="str">StringBuilder, welcher eine komplette Kopie der Daten enthält</param>
    /// <param name="mem">Speichersystem, welcher die gleichen Daten enthalten sollte</param>
    public static void Comp(this StringBuilder str, ITextMemory mem)
    {
      Assert.AreEqual(str.Length, mem.Length);
      string tmpStr = new string(mem.GetChars(0, mem.Length));
      Assert.AreEqual(str.ToString(), tmpStr);
    }
    #endregion

    #region # void Comp(this StringBuilder str, ITextMonster txt) // vergleicht den kompletten Inhalt mit einem Textmonster
    /// <summary>
    /// vergleicht den kompletten Inhalt mit einem Textmonster
    /// </summary>
    /// <param name="str">StringBuilder, welcher eine komplette Kopie der Daten enthält</param>
    /// <param name="txt">TextMonster, welcher die gleichen Daten enthalten sollte</param>
    public static void Comp(this StringBuilder str, ITextMonster txt)
    {
      // --- Inhalte Zeichenweise vergleichen ---
      Assert.AreEqual(str.Length, txt.Length);
      string tmpStr = new string(txt.GetChars(txt.GetMemoryPos(0L)).ToArray());
      Assert.AreEqual(str.ToString(), tmpStr);

      // --- Inhalte Zeilenweise vergleichen ---
      Assert.AreEqual(str.Lines(), txt.Lines);
      
    }
    #endregion

  }
}
