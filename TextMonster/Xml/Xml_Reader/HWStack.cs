using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class HWStack : ICloneable
  {
    internal HWStack(int GrowthRate) : this(GrowthRate, int.MaxValue) { }

    internal HWStack(int GrowthRate, int limit)
    {
      this.growthRate = GrowthRate;
      this.used = 0;
      this.stack = new Object[GrowthRate];
      this.size = GrowthRate;
      this.limit = limit;
    }

    internal Object Push()
    {
      if (this.used == this.size)
      {
        if (this.limit <= this.used)
        {
          throw new XmlException(Res.Xml_StackOverflow, string.Empty);
        }
        Object[] newstack = new Object[this.size + this.growthRate];
        if (this.used > 0)
        {
          System.Array.Copy(this.stack, 0, newstack, 0, this.used);
        }
        this.stack = newstack;
        this.size += this.growthRate;
      }
      return this.stack[this.used++];
    }

    internal Object Pop()
    {
      if (0 < this.used)
      {
        this.used--;
        Object result = this.stack[this.used];
        return result;
      }
      return null;
    }

    internal object Peek()
    {
      return this.used > 0 ? this.stack[this.used - 1] : null;
    }

    internal void AddToTop(object o)
    {
      if (this.used > 0)
      {
        this.stack[this.used - 1] = o;
      }
    }

    internal Object this[int index]
    {
      get
      {
        if (index >= 0 && index < this.used)
        {
          Object result = this.stack[index];
          return result;
        }
        else
        {
          throw new IndexOutOfRangeException();
        }
      }
      set
      {
        if (index >= 0 && index < this.used)
        {
          this.stack[index] = value;
        }
        else
        {
          throw new IndexOutOfRangeException();
        }
      }
    }

    internal int Length
    {
      get { return this.used; }
    }

    //
    // ICloneable
    //

    private HWStack(object[] stack, int growthRate, int used, int size)
    {
      this.stack = stack;
      this.growthRate = growthRate;
      this.used = used;
      this.size = size;
    }

    public object Clone()
    {
      return new HWStack((object[])this.stack.Clone(), this.growthRate, this.used, this.size);
    }

    private Object[] stack;
    private int growthRate;
    private int used;
    private int size;
    private int limit;
  };
}
