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
    static void TestBasicsInsert(ITextMemory mem, string ins)
    {
      long startLength = mem.Length;

      var t1 = mem.Insert(0, ins[0]);
      Assert.AreEqual(mem.GetCharPos(t1), 1);
      Assert.AreEqual(mem.Length, startLength + 1);

      var t2 = mem.Insert(1, ins[1]);
      Assert.AreEqual(mem.GetCharPos(t1), 1);
      Assert.AreEqual(mem.GetCharPos(t2), 2);
      Assert.AreEqual(mem.Length, startLength + 2);

      var t3 = mem.Insert(0, ins[0]);
      Assert.AreEqual(mem.GetCharPos(t3), 1);
      Assert.AreEqual(mem.GetCharPos(t1), 2);
      Assert.AreEqual(mem.GetCharPos(t2), 3);
      Assert.AreEqual(mem.Length, startLength + 3);

      var t4 = mem.Insert(1, ins[1]);
      Assert.AreEqual(mem.GetCharPos(t3), 1);
      Assert.AreEqual(mem.GetCharPos(t4), 2);
      Assert.AreEqual(mem.GetCharPos(t1), 3);
      Assert.AreEqual(mem.GetCharPos(t2), 4);
      Assert.AreEqual(mem.Length, startLength + 4);

      var t5 = mem.Insert(t4, ins);
      Assert.AreEqual(mem.GetCharPos(t3), 1);
      Assert.AreEqual(mem.GetCharPos(t4), 2);
      Assert.AreEqual(mem.GetCharPos(t5), 2 + ins.Length);
      Assert.AreEqual(mem.GetCharPos(t1), 3 + ins.Length);
      Assert.AreEqual(mem.GetCharPos(t2), 4 + ins.Length);
      Assert.AreEqual(mem.Length, startLength + 4 + ins.Length);
    }

    static void TestBasics(ITextMemory mem)
    {
      string testString = "Héllo Wörld";

      for (long count = 1; count <= 10; count++)
      {
        TestBasicsInsert(mem, testString);
        Assert.AreEqual(mem.Length, (4 + testString.Length) * count);
      }


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
