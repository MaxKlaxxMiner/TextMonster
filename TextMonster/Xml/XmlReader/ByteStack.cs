using System;

namespace TextMonster.Xml.XmlReader
{
  internal class ByteStack
  {
    private byte[] stack;
    private int growthRate;
    private int top;
    private int size;

    public ByteStack(int growthRate)
    {
      this.growthRate = growthRate;
      top = 0;
      stack = new byte[growthRate];
      size = growthRate;
    }

    public void Push(byte data)
    {
      if (size == top)
      {
        byte[] newstack = new byte[size + growthRate];
        if (top > 0)
        {
          Buffer.BlockCopy(stack, 0, newstack, 0, top);
        }
        stack = newstack;
        size += growthRate;
      }
      stack[top++] = data;
    }

    public byte Pop()
    {
      if (top > 0)
      {
        return stack[--top];
      }
      else
      {
        return 0;
      }
    }

    public byte Peek()
    {
      if (top > 0)
      {
        return stack[top - 1];
      }
      else
      {
        return 0;
      }
    }

    public int Length
    {
      get
      {
        return top;
      }
    }
  }
}
