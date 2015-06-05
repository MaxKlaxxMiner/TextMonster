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
    [TestMethod]
    public void TestMethod1()
    {
      var test = new TextMemorySimple();

      test.mem.Add('a');

    }
  }
}
