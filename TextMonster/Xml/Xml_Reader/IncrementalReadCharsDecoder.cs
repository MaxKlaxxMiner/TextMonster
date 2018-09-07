using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class IncrementalReadCharsDecoder : IncrementalReadDecoder
  {
    char[] buffer;
    int startIndex;
    int curIndex;
    int endIndex;

    internal override int DecodedCount
    {
      get
      {
        return curIndex - startIndex;
      }
    }

    internal override bool IsFull
    {
      get
      {
        return curIndex == endIndex;
      }
    }

    internal override int Decode(char[] chars, int startPos, int len)
    {
      int copyCount = endIndex - curIndex;
      if (copyCount > len)
      {
        copyCount = len;
      }
      Buffer.BlockCopy(chars, startPos * 2, buffer, curIndex * 2, copyCount * 2);
      curIndex += copyCount;

      return copyCount;
    }

    internal override int Decode(string str, int startPos, int len)
    {
      int copyCount = endIndex - curIndex;
      if (copyCount > len)
      {
        copyCount = len;
      }
      str.CopyTo(startPos, buffer, curIndex, copyCount);
      curIndex += copyCount;

      return copyCount;
    }

    internal override void Reset()
    {
    }

    internal override void SetNextOutputBuffer(Array buffer, int index, int count)
    {
      this.buffer = (char[])buffer;
      this.startIndex = index;
      this.curIndex = index;
      this.endIndex = index + count;
    }
  }
}
