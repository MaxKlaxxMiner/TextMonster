#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextMonsterSystem;

#endregion

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
      Assert.IsFalse(txt.LengthLimit < 1048576L, "LengthLimit zu klein < 1 MB");
      Assert.IsFalse(txt.LengthLimit > 1048576L * 16777216L, "LengthLimit zu groß > 16 TB");
    }

#if DEBUG
    [TestMethod]
    public void TestSimpleMinimal()
    {
      using (var test = new TextMonsterSimpleMinimal())
      {
        TestBasics(test);
      }
    }
#endif

    [TestMethod]
    public void TestSimpleFull()
    {
      using (var test = new TextMonsterSimpleFull())
      {
        TestBasics(test);
      }
    }
  }
}
