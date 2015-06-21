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
  /// Tests für die Erweiterungmethoden des StringBuilders
  /// </summary>
  [TestClass]
  public class StringBuilderExtensionsTests
  {
    [TestMethod]
    public void TestStringBuilderLines()
    {
      foreach (var test in TestData.TestLines)
      {
        Assert.AreEqual(test.count, test.strLinux.Lines());
        Assert.AreEqual(test.count, test.strWindows.Lines());

        string[] linesLinux = test.strLinux.ReadLines().ToArray();
        string[] linesWindows = test.strWindows.ReadLines().ToArray();
        Assert.AreEqual(test.count, linesLinux.Length);
        Assert.AreEqual(test.count, linesWindows.Length);

        Assert.IsNull(test.strLinux.GetLine(-1));
        Assert.IsNull(test.strWindows.GetLine(-1));
        Assert.IsNull(test.strLinux.GetLine(test.count));
        Assert.IsNull(test.strWindows.GetLine(test.count));

        // --- ReadLines() prüfen ---
        for (int i = 0; i < test.count; i++)
        {
          Assert.AreEqual(test.linesRaw[i], linesLinux[i]);
          Assert.AreEqual(test.linesRaw[i], linesWindows[i]);
        }

        // --- GetLine() prüfen ---
        for (int i = 0; i < test.count; i++)
        {
          Assert.AreEqual(test.linesRaw[i], test.strLinux.GetLine(i));
          Assert.AreEqual(test.linesRaw[i], test.strWindows.GetLine(i));
        }

        // --- GetLineStart() und GetLineEnd() prüfen ---
        for (int i = 0; i < test.count; i++)
        {
          for (int c = test.linesStartLinux[i]; c <= test.linesEndLinux[i]; c++)
          {
            Assert.AreEqual(test.linesStartLinux[i], test.strLinux.GetLineStart(c));
            Assert.AreEqual(test.linesEndLinux[i], test.strLinux.GetLineEnd(c));
          }
          int to = Math.Min(test.strWindows.Length, test.linesEndWindows[i] + 1);
          for (int c = test.linesStartWindows[i]; c <= to; c++)
          {
            Assert.AreEqual(test.linesStartWindows[i], test.strWindows.GetLineStart(c));
            Assert.AreEqual(test.linesEndWindows[i], test.strWindows.GetLineEnd(c));
          }
        }
      }
    }
  }
}
