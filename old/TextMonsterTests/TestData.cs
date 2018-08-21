#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace TextMonsterTests
{
  /// <summary>
  /// Klasse enthält sämtliche Testdaten
  /// </summary>
  public static class TestData
  {
    #region # string[] TestStrings
    public readonly static string[] TestStrings = new[]
    {
      "ha",
      " ha",
      "ha ",
      " ha ",
      "Öl",
      " Öl",
      "Öl ",
      " Öl ",
      "Héllo Wörld",
      "\tWas du heute kannst besorgen, das verschiebe nicht auf morgen!\r\n",
      "Zeilen Linux\n",
      "\nZeilen Linux",
      "Zeilen\nLinux",
      "\nZeilen\nLinux\n",
      "Zeilen Windows\r\n",
      "\r\nZeilen Windows",
      "Zeilen\r\nWindows",
      "\r\nZeilen\r\nWindows\r\n",
    };
    #endregion

    #region # TestLine[] TestLines
    static string[] testLinesSource = 
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

    public struct TestLine
    {
      public StringBuilder strLinux;
      public StringBuilder strWindows;
      public int count;
      public string[] linesRaw;
      public List<int> linesStartLinux;
      public List<int> linesEndLinux;
      public List<int> linesStartWindows;
      public List<int> linesEndWindows;
    }

    public static TestLine[] TestLines
    {
      get
      {
        var testData = testLinesSource.Concat(testLinesSource.Select(x => x.Replace('a', 'ä'))).Select(x =>
                       new TestLine
                       {
                         strLinux = new StringBuilder(x),
                         strWindows = new StringBuilder(x.Replace("\n", "\r\n")),
                         count = x.Split('\n').Length,
                         linesRaw = x.Split('\n'),
                         linesStartLinux = new List<int>(),
                         linesEndLinux = new List<int>(),
                         linesStartWindows = new List<int>(),
                         linesEndWindows = new List<int>(),
                       }).ToArray();

        foreach (var test in testData)
        {
          test.linesStartLinux.Add(0);
          test.linesStartWindows.Add(0);

          for (int i = 1; i < test.linesRaw.Length; i++)
          {
            int posLinux = test.linesStartLinux[i - 1] + test.linesRaw[i - 1].Length;
            int posWindows = test.linesStartWindows[i - 1] + test.linesRaw[i - 1].Length;
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

        return testData;
      }
    }
    #endregion
  }
}
