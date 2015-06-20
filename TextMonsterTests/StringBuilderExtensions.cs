﻿#region # using *.*

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
    #region # void Comp(this StringBuilder str, ITextMemory mem) // vergleicht den kompletten Inhalt mit Speichersystem
    /// <summary>
    /// vergleicht den kompletten Inhalt mit einem Speichersystem
    /// </summary>
    /// <param name="str">StringBuilder, welcher eine komplette Kopie der Daten enthält</param>
    /// <param name="mem">Speichersystem, welcher die gleichen Daten enthalten sollte</param>
    public static void Comp(this StringBuilder str, ITextMemory mem)
    {
      Assert.AreEqual(str.Length, mem.Length);

      if (mem is TextMemorySimpleMinimal)
      {
        var tmp = (TextMemorySimpleMinimal)mem;
        string tmpStrIntern = new string(tmp.mem.ToArray());
        Assert.AreEqual(str.ToString(), tmpStrIntern);
      }

      string tmpStr = new string(mem.GetChars(0, mem.Length));
      Assert.AreEqual(str.ToString(), tmpStr);
    }
    #endregion

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

  }
}
