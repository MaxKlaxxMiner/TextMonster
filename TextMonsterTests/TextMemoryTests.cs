#region # using *.*

using System.Text;
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
    static void TestBasicsInsert(ITextMemory mem, string ins, StringBuilder debugString)
    {
      long startLength = mem.Length;

      debugString.Insert(0, ins[0]);
      var t1 = mem.Insert(0, ins[0]);
      Assert.AreEqual(mem.GetCharPos(t1), 1);
      Assert.AreEqual(mem.Length, startLength + 1);

      debugString.Insert(1, ins[1]);
      var t2 = mem.Insert(1, ins[1]);
      Assert.AreEqual(mem.GetCharPos(t1), 1);
      Assert.AreEqual(mem.GetCharPos(t2), 2);
      Assert.AreEqual(mem.Length, startLength + 2);

      debugString.Insert(0, ins[0]);
      var t3 = mem.Insert(0, ins[0]);
      Assert.AreEqual(mem.GetCharPos(t3), 1);
      Assert.AreEqual(mem.GetCharPos(t1), 2);
      Assert.AreEqual(mem.GetCharPos(t2), 3);
      Assert.AreEqual(mem.Length, startLength + 3);

      debugString.Insert(1, ins[1]);
      var t4 = mem.Insert(1, ins[1]);
      Assert.AreEqual(mem.GetCharPos(t3), 1);
      Assert.AreEqual(mem.GetCharPos(t4), 2);
      Assert.AreEqual(mem.GetCharPos(t1), 3);
      Assert.AreEqual(mem.GetCharPos(t2), 4);
      Assert.AreEqual(mem.Length, startLength + 4);

      debugString.Insert(2, ins);
      var t5 = mem.Insert(t4, ins);
      Assert.AreEqual(mem.GetCharPos(t3), 1);
      Assert.AreEqual(mem.GetCharPos(t4), 2);
      Assert.AreEqual(mem.GetCharPos(t5), 2 + ins.Length);
      Assert.AreEqual(mem.GetCharPos(t1), 3 + ins.Length);
      Assert.AreEqual(mem.GetCharPos(t2), 4 + ins.Length);
      Assert.AreEqual(mem.Length, startLength + 4 + ins.Length);

      debugString.Insert(2 + ins.Length, ins);
      var t6 = mem.Insert(t5, ins);
      Assert.AreEqual(mem.GetCharPos(t3), 1);
      Assert.AreEqual(mem.GetCharPos(t4), 2);
      Assert.AreEqual(mem.GetCharPos(t5), 2 + ins.Length);
      Assert.AreEqual(mem.GetCharPos(t6), 2 + ins.Length * 2);
      Assert.AreEqual(mem.GetCharPos(t1), 3 + ins.Length * 2);
      Assert.AreEqual(mem.GetCharPos(t2), 4 + ins.Length * 2);
      Assert.AreEqual(mem.Length, startLength + 4 + ins.Length * 2);
    }

    static void TestBasics(ITextMemory mem, StringBuilder debugString)
    {
      string testString = "Héllo Wörld";

      for (long count = 1; count <= 10; count++)
      {
        TestBasicsInsert(mem, testString, debugString);
        Assert.AreEqual(mem.Length, (4 + testString.Length * 2) * count);
      }

      if (mem is TextMemorySimple)
      {
        var tmp = (TextMemorySimple)mem;
        string tmpStr = new string(tmp.mem.ToArray());
        Assert.AreEqual(debugString.ToString(), tmpStr);
      }

    }

    [TestMethod]
    public void TestMemorySimple()
    {
      StringBuilder debugString = new StringBuilder();
      using (var mem = new TextMemorySimple())
      {
        TestBasics(mem, debugString);
      }
    }
  }
}
