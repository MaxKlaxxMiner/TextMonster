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
    #region # static void TestBasicsInsert(ITextMemory mem, string ins, StringBuilder debugString)
    static void TestBasicsInsert(ITextMemory mem, string ins, StringBuilder debugString)
    {
      long startLength = mem.Length;

      debugString.Insert(0, ins[0]);
      var t1 = mem.Insert(0, ins[0]);
      Assert.AreEqual(mem.GetCharPos(t1), 1);
      Assert.AreEqual(mem.Length, startLength + 1);
      debugString.Comp(mem);

      debugString.Insert(1, ins[1]);
      var t2 = mem.Insert(1, ins[1]);
      Assert.AreEqual(mem.GetCharPos(t1), 1);
      Assert.AreEqual(mem.GetCharPos(t2), 2);
      Assert.AreEqual(mem.Length, startLength + 2);
      debugString.Comp(mem);

      debugString.Insert(0, ins[0]);
      var t3 = mem.Insert(0, ins[0]);
      Assert.AreEqual(mem.GetCharPos(t3), 1);
      Assert.AreEqual(mem.GetCharPos(t1), 2);
      Assert.AreEqual(mem.GetCharPos(t2), 3);
      Assert.AreEqual(mem.Length, startLength + 3);
      debugString.Comp(mem);

      debugString.Insert(1, ins[1]);
      var t4 = mem.Insert(1, ins[1]);
      Assert.AreEqual(mem.GetCharPos(t3), 1);
      Assert.AreEqual(mem.GetCharPos(t4), 2);
      Assert.AreEqual(mem.GetCharPos(t1), 3);
      Assert.AreEqual(mem.GetCharPos(t2), 4);
      Assert.AreEqual(mem.Length, startLength + 4);
      debugString.Comp(mem);

      debugString.Insert(2, ins);
      var t5 = mem.Insert(t4, ins);
      Assert.AreEqual(mem.GetCharPos(t3), 1);
      Assert.AreEqual(mem.GetCharPos(t4), 2);
      Assert.AreEqual(mem.GetCharPos(t5), 2 + ins.Length);
      Assert.AreEqual(mem.GetCharPos(t1), 3 + ins.Length);
      Assert.AreEqual(mem.GetCharPos(t2), 4 + ins.Length);
      Assert.AreEqual(mem.Length, startLength + 4 + ins.Length);
      debugString.Comp(mem);

      debugString.Insert(2 + ins.Length, ins);
      var t6 = mem.Insert(t5, ins);
      Assert.AreEqual(mem.GetCharPos(t3), 1);
      Assert.AreEqual(mem.GetCharPos(t4), 2);
      Assert.AreEqual(mem.GetCharPos(t5), 2 + ins.Length);
      Assert.AreEqual(mem.GetCharPos(t6), 2 + ins.Length * 2);
      Assert.AreEqual(mem.GetCharPos(t1), 3 + ins.Length * 2);
      Assert.AreEqual(mem.GetCharPos(t2), 4 + ins.Length * 2);
      Assert.AreEqual(mem.Length, startLength + 4 + ins.Length * 2);
      debugString.Comp(mem);
    }
    #endregion

    #region # static void TestBasicsRemove(ITextMemory mem, string ins, StringBuilder debugString)
    static void TestBasicsRemove(ITextMemory mem, string ins, StringBuilder debugString)
    {
      debugString.Insert(debugString.Length - 2, '#');
      var t2 = mem.Insert(mem.Length - 2, '#');
      Assert.AreEqual(mem.GetCharPos(t2), debugString.Length - 2);
      debugString.Comp(mem);

      debugString.Insert(2, '#');
      var t1 = mem.Insert(2, '#');
      Assert.AreEqual(mem.GetCharPos(t1), 3);
      Assert.AreEqual(mem.GetCharPos(t2), debugString.Length - 2);
      debugString.Comp(mem);

      debugString.Remove(debugString.Length / 4, debugString.Length / 2);
      mem.Remove(mem.Length / 4, mem.Length / 2);
      Assert.AreEqual(mem.GetCharPos(t1), 3);
      Assert.AreEqual(mem.GetCharPos(t2), debugString.Length - 2);
      debugString.Comp(mem);

      debugString.Remove(0, 1);
      mem.Remove(0, 1);
      debugString.Comp(mem);

      debugString.Remove(2, ins.Length);
      mem.Remove(2, ins.Length);
      debugString.Comp(mem);

      debugString.Remove(ins.Length + 2, 4);
      mem.Remove(ins.Length + 2, 4);
      Assert.AreEqual(mem.GetCharPos(t1), 2);
      Assert.AreEqual(mem.GetCharPos(t2), debugString.ToString().LastIndexOf('#') + 1);
      debugString.Comp(mem);

      debugString.Remove(0, 2 + ins.Length);
      mem.Remove(0, 2 + ins.Length);
      Assert.AreEqual(mem.GetCharPos(t1), 0);
      Assert.AreEqual(mem.GetCharPos(t2), debugString.ToString().LastIndexOf('#') + 1);
      debugString.Comp(mem);

      debugString.Remove(debugString.Length - 2, 2);
      mem.Remove(mem.Length - 2, 2);
      debugString.Comp(mem);

      debugString.Remove(ins.Length, debugString.Length - ins.Length * 2 - 1);
      mem.Remove(ins.Length, mem.Length - ins.Length * 2 - 1);
      Assert.AreEqual(mem.GetCharPos(t1), 0);
      Assert.AreEqual(mem.GetCharPos(t2), debugString.ToString().LastIndexOf('#') + 1);
      debugString.Comp(mem);

      debugString.Remove(ins.Length, ins.Length + 1);
      mem.Remove(ins.Length, ins.Length + 1);
      Assert.AreEqual(mem.GetCharPos(t1), 0);
      Assert.AreEqual(mem.GetCharPos(t2), debugString.Length);
      debugString.Comp(mem);
    }
    #endregion

    #region # static void TestBasics(ITextMemory mem)
    static void TestBasics(ITextMemory mem)
    {
      string[] testStrings = new[]
      {
        "ha",
        "HaHa",
        "Öl",
        "Héllo Wörld",
        "Was du heute kannst besorgen, das verschiebe nicht auf morgen!"
      };

      StringBuilder debugString = new StringBuilder();

      foreach (string testString in testStrings)
      {
        // --- Insert testen ---
        for (long count = 1; count <= 10; count++)
        {
          TestBasicsInsert(mem, testString, debugString);
          Assert.AreEqual(mem.Length, (4 + testString.Length * 2) * count);
          debugString.Comp(mem);
        }

        // --- Remove testen ---
        TestBasicsRemove(mem, testString, debugString);
        debugString.Comp(mem);

        mem.Clear();
        debugString.Clear();
      }
    }
    #endregion

    [TestMethod]
    public void TestMemorySimpleMinimal()
    {
      using (var mem = new TextMemorySimpleMinimal())
      {
        TestBasics(mem);
      }
    }

    [TestMethod]
    public void TestMemorySimpleFull()
    {
      using (var mem = new TextMemorySimpleFull())
      {
        TestBasics(mem);
      }
    }
  }
}
