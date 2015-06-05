#region # using *.*

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextMonsterSystem.Memory;

#endregion

namespace TextMonsterTests
{
  /// <summary>
  /// Testklasse zum prüfen von den TextMemory-System
  /// </summary>
  [TestClass]
  public class TextMemoryTests
  {
    static void TestBasics(ITextMemory mem)
    {
      Assert.AreEqual(mem.Length, 0);


    }

    [TestMethod]
    public void TestMemorySimple()
    {
      using (var mem = new TextMemorySimple())
      {
        TestBasics(mem);
      }
    }
  }
}
