using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  internal class SafeAsciiDecoder : Decoder
  {

    public SafeAsciiDecoder()
    {
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
      return count;
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
      int i = byteIndex;
      int j = charIndex;
      while (i < byteIndex + byteCount)
      {
        chars[j++] = (char)bytes[i++];
      }
      return byteCount;
    }

    public override void Convert(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed)
    {
      if (charCount < byteCount)
      {
        byteCount = charCount;
        completed = false;
      }
      else
      {
        completed = true;
      }

      int i = byteIndex;
      int j = charIndex;
      int byteEndIndex = byteIndex + byteCount;

      while (i < byteEndIndex)
      {
        chars[j++] = (char)bytes[i++];
      }

      charsUsed = byteCount;
      bytesUsed = byteCount;
    }
  }
}