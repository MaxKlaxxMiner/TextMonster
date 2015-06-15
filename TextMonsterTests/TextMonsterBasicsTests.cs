using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextMonsterSystem;

namespace TextMonsterTests
{
  /// <summary>
  /// Testklasse zum prüfen von Basis-Funktionen der TextMonster-Klassen
  /// </summary>
  [TestClass]
  public class TextMonsterBasicsTests
  {
    static void TestBasics(ITextMonster txt)
    {
      
    }

    [TestMethod]
    public void TestSimple()
    {
      using (var test = new TextMonsterSimple())
      {
        TestBasics(test);
      }
    }
  }
}
