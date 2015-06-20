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
        "\nbla",
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
        lines = x.Split('\n')
      }).ToArray();

      foreach (var test in testData)
      {
        Assert.AreEqual(test.count, test.strLinux.Lines());
        Assert.AreEqual(test.count, test.strWindows.Lines());

        string[] linesLinux = test.strLinux.ReadLines().ToArray();
        string[] linesWindows = test.strWindows.ReadLines().ToArray();
        Assert.AreEqual(test.count, linesLinux.Length);
        Assert.AreEqual(test.count, linesWindows.Length);

        for (int i = 0; i < test.lines.Length; i++)
        {
          Assert.AreEqual(test.lines[i], linesLinux[i]);
          Assert.AreEqual(test.lines[i], linesWindows[i]);
        }
      }
    }
  }
}
