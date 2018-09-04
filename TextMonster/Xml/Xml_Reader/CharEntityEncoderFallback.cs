using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  internal class CharEntityEncoderFallback : EncoderFallback
  {
    private CharEntityEncoderFallbackBuffer fallbackBuffer;

    private int[] textContentMarks;
    private int endMarkPos;
    private int curMarkPos;
    private int startOffset;

    internal CharEntityEncoderFallback()
    {
    }

    public override EncoderFallbackBuffer CreateFallbackBuffer()
    {
      if (fallbackBuffer == null)
      {
        fallbackBuffer = new CharEntityEncoderFallbackBuffer(this);
      }
      return fallbackBuffer;
    }

    public override int MaxCharCount
    {
      get
      {
        return 12;
      }
    }

    internal int StartOffset
    {
      get
      {
        return startOffset;
      }
      set
      {
        startOffset = value;
      }
    }

    internal void Reset(int[] textContentMarks, int endMarkPos)
    {
      this.textContentMarks = textContentMarks;
      this.endMarkPos = endMarkPos;
      curMarkPos = 0;
    }

    internal bool CanReplaceAt(int index)
    {
      int mPos = curMarkPos;
      int charPos = startOffset + index;
      while (mPos < endMarkPos && charPos >= textContentMarks[mPos + 1])
      {
        mPos++;
      }
      curMarkPos = mPos;

      return (mPos & 1) != 0;
    }
  }
}
