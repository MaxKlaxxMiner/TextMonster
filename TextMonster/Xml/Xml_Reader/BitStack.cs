using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Manages a stack of bits.  Exposes push, pop, and peek operations.
  /// </summary>
  internal class BitStack
  {
    private uint[] bitStack;
    private int stackPos;
    private uint curr;

    /// <summary>
    /// Initialize stack.
    /// </summary>
    public BitStack()
    {
      // Set sentinel bit in 1st position.  As bits are shifted onto this.curr, this sentinel
      // bit shifts to the left.  When it's about to overflow, this.curr will be pushed
      // onto an unsigned int stack and the sentinel bit will be reset to 0x1.
      curr = 0x1;
    }

    /// <summary>
    /// Push a 0 or 1 bit onto the stack.
    /// </summary>
    public void PushBit(bool bit)
    {
      if ((curr & 0x80000000) != 0)
      {
        // If sentinel bit has reached the last position, push this.curr
        PushCurr();
      }

      // Shift new bit onto this.curr (which must have at least one open position)
      curr = (curr << 1) | (bit ? 1u : 0u);
    }

    /// <summary>
    /// Pop the top bit from the stack and return it.
    /// </summary>
    public bool PopBit()
    {
      bool bit;

      // Shift rightmost bit from this.curr
      bit = (curr & 0x1) != 0;

      curr >>= 1;

      if (curr == 0x1)
      {
        // If sentinel bit has reached the rightmost position, pop this.curr
        PopCurr();
      }

      return bit;
    }

    /// <summary>
    /// Return the top bit on the stack without pushing or popping.
    /// </summary>
    public bool PeekBit()
    {
      return (curr & 0x1) != 0;
    }

    /// <summary>
    /// this.curr has enough space for 31 bits (minus 1 for sentinel bit).  Once this space is
    /// exhausted, a uint stack is created to handle the overflow.
    /// </summary>
    private void PushCurr()
    {
      int len;

      if (bitStack == null)
      {
        bitStack = new uint[16];
      }

      // Push current unsigned int (which has been filled) onto a stack
      // and initialize this.curr to be used for future pushes.
      bitStack[stackPos++] = curr;
      curr = 0x1;

      // Resize stack if necessary
      len = bitStack.Length;
      if (stackPos >= len)
      {
        uint[] bitStackNew = new uint[2 * len];
        Array.Copy(bitStack, bitStackNew, len);
        bitStack = bitStackNew;
      }
    }

    /// <summary>
    /// If all bits have been popped from this.curr, then pop the previous uint value from the stack in
    /// order to provide another 31 bits.
    /// </summary>
    private void PopCurr()
    {
      if (stackPos > 0)
        curr = bitStack[--stackPos];
    }
  }
}
