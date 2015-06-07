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
      var t1 = mem.Insert(0, ins[0]);
      Assert.AreEqual(t1.pos, 1);
      Assert.AreEqual(t1.GetPos(mem), 1);

      var t2 = mem.Insert(1, ins[1]);
      Assert.AreEqual(t2.pos, 2);
      Assert.AreEqual(t2.GetPos(mem), 2);
      Assert.AreEqual(t1.GetPos(mem), 1);

      var t3 = mem.Insert(0, ins[0]);
      Assert.AreEqual(t3.pos, 1);
      Assert.AreEqual(t3.GetPos(mem), 1);
      Assert.AreEqual(t2.GetPos(mem), 3);
      Assert.AreEqual(t1.GetPos(mem), 2);

      var t4 = mem.Insert(1, ins[1]);
      Assert.AreEqual(t4.pos, 2);
      Assert.AreEqual(t4.GetPos(mem), 2);
      Assert.AreEqual(t3.GetPos(mem), 1);
      Assert.AreEqual(t2.GetPos(mem), 4);
      Assert.AreEqual(t1.GetPos(mem), 3);
      Assert.AreEqual(mem.Length, 4);

      var t5 = mem.Insert(t4, ins.ToCharArray());
      Assert.AreEqual(t5.pos, 2 + ins.Length);
      Assert.AreEqual(t4.GetPos(mem), 2);
      Assert.AreEqual(t3.GetPos(mem), 1);
      Assert.AreEqual(t2.GetPos(mem), 4 + ins.Length);
      Assert.AreEqual(t1.GetPos(mem), 3 + ins.Length);
      Assert.AreEqual(mem.Length, 4 + ins.Length);
    }

    static void TestBasics(ITextMemory mem)
    {
      string testString = "Hello World";
      Assert.AreEqual(mem.Length, 0);

      TestBasicsInsert(mem, testString);
      Assert.AreEqual(mem.Length, testString.Length + 4);


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
