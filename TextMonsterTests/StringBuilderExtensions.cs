#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextMonsterSystem.Memory;

#endregion

namespace TextMonsterTests
{
  /// <summary>
  /// enthält Erweiterungmethoden für den StringBuilder
  /// </summary>
  public static class StringBuilderExtensions
  {
    #region # public static void Comp(this StringBuilder str, ITextMemory mem) // fügt einen kompletten Test mit einem SpeicherSystem und vergleicht den kompletten Inhalt
    /// <summary>
    /// fügt einen kompletten Test mit einem SpeicherSystem und vergleicht den kompletten Inhalt
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

  }
}
