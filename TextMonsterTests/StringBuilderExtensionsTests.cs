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
      string[] testSource = 
      {
        "",
        "\n",
        "bla\n",
        "bla\n\n",
        "\nbla",
        "\n\nbla",
        "\n\nbla\n\n",
        "bla\nbla",
        "\nbla\nbla",
        "\nbla\nbla\n",
        "bla\nbla\nbla\n",
        "bla\nbla\nbla\nbla",
        "bla\nbla\n\nbla\nbla",
        "bla\nbla\n\n\nbla\nbla",
        "b\nla\nbla\n\n\nbla\nbla",
        "\nb\nla\nbla\n\n\nbla\nbla",
        "\nb\nla\nbla\n\n\nbla\nbla\n",
        "\nb\nla\nbla\n\n\nbla\nbla\nb",
        "\nb\nla\nbla\n\n\nbla\nbla\nb\n",
      };

      var testData = testSource.Select(x =>
      new
      {
        strLinux = new StringBuilder(x),
        strWindows = new StringBuilder(x.Replace("\n", "\r\n")),
        count = x.Split('\n').Length,
        lines = x.Split('\n'),
        linesStartLinux = new List<int>(),
        linesEndLinux = new List<int>(),
        linesStartWindows = new List<int>(),
        linesEndWindows = new List<int>(),
      }).ToArray();

      foreach (var test in testData)
      {
        test.linesStartLinux.Add(0);
        test.linesStartWindows.Add(0);

        for (int i = 1; i < test.lines.Length; i++)
        {
          int posLinux = test.linesStartLinux[i - 1] + test.lines[i - 1].Length;
          int posWindows = test.linesStartWindows[i - 1] + test.lines[i - 1].Length;
          test.linesStartLinux.Add(posLinux + 1);
          test.linesStartWindows.Add(posWindows + 2);
          test.linesEndLinux.Add(posLinux);
          test.linesEndWindows.Add(posWindows);
        }

        if (test.strLinux.Length > 0 && test.strLinux[test.strLinux.Length - 1] == '\n')
        {
          test.linesEndLinux.Add(test.strLinux.Length - 1);
          test.linesEndWindows.Add(test.strWindows.Length - 2);
        }
        else
        {
          test.linesEndLinux.Add(test.strLinux.Length);
          test.linesEndWindows.Add(test.strWindows.Length);
        }
      }

      foreach (var test in testData)
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
        for (int i = 0; i < test.lines.Length; i++)
        {
          Assert.AreEqual(test.lines[i], linesLinux[i]);
          Assert.AreEqual(test.lines[i], linesWindows[i]);
        }

        // --- GetLine() prüfen ---
        for (int i = 0; i < test.lines.Length; i++)
        {
          Assert.AreEqual(test.lines[i], test.strLinux.GetLine(i));
          Assert.AreEqual(test.lines[i], test.strWindows.GetLine(i));
        }

        // --- GetLineStart() und GetLineEnd() prüfen ---
        for (int i = 0; i < test.lines.Length; i++)
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
