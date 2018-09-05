using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  internal abstract class Ucs4Decoder : Decoder
  {

    internal byte[] lastBytes = new byte[4];
    internal int lastBytesCount = 0;

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
      return (count + lastBytesCount) / 4;
    }

    internal abstract int GetFullChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex);

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
      // finish a character from the bytes that were cached last time
      int i = lastBytesCount;
      if (lastBytesCount > 0)
      {
        // copy remaining bytes into the cache
        for (; lastBytesCount < 4 && byteCount > 0; lastBytesCount++)
        {
          lastBytes[lastBytesCount] = bytes[byteIndex];
          byteIndex++;
          byteCount--;
        }
        // still not enough bytes -> return
        if (lastBytesCount < 4)
        {
          return 0;
        }
        // decode 1 character from the byte cache
        i = GetFullChars(lastBytes, 0, 4, chars, charIndex);
        charIndex += i;
        lastBytesCount = 0;
      }
      else
      {
        i = 0;
      }

      // decode block of byte quadruplets
      i = GetFullChars(bytes, byteIndex, byteCount, chars, charIndex) + i;

      // cache remaining bytes that does not make up a character
      int bytesLeft = (byteCount & 0x3);
      if (bytesLeft >= 0)
      {
        for (int j = 0; j < bytesLeft; j++)
        {
          lastBytes[j] = bytes[byteIndex + byteCount - bytesLeft + j];
        }
        lastBytesCount = bytesLeft;
      }
      return i;
    }

    public override void Convert(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed)
    {
      bytesUsed = 0;
      charsUsed = 0;
      // finish a character from the bytes that were cached last time
      int i = 0;
      int lbc = lastBytesCount;
      if (lbc > 0)
      {
        // copy remaining bytes into the cache
        for (; lbc < 4 && byteCount > 0; lbc++)
        {
          lastBytes[lbc] = bytes[byteIndex];
          byteIndex++;
          byteCount--;
          bytesUsed++;
        }
        // still not enough bytes -> return
        if (lbc < 4)
        {
          lastBytesCount = lbc;
          completed = true;
          return;
        }
        // decode 1 character from the byte cache
        i = GetFullChars(lastBytes, 0, 4, chars, charIndex);
        charIndex += i;
        charCount -= i;
        charsUsed = i;

        lastBytesCount = 0;

        // if that's all that was requested -> return
        if (charCount == 0)
        {
          completed = (byteCount == 0);
          return;
        }
      }
      else
      {
        i = 0;
      }

      // modify the byte count for GetFullChars depending on how many characters were requested
      if (charCount * 4 < byteCount)
      {
        byteCount = charCount * 4;
        completed = false;
      }
      else
      {
        completed = true;
      }
      bytesUsed += byteCount;

      // decode block of byte quadruplets
      charsUsed = GetFullChars(bytes, byteIndex, byteCount, chars, charIndex) + i;

      // cache remaining bytes that does not make up a character
      int bytesLeft = (byteCount & 0x3);
      if (bytesLeft >= 0)
      {
        for (int j = 0; j < bytesLeft; j++)
        {
          lastBytes[j] = bytes[byteIndex + byteCount - bytesLeft + j];
        }
        lastBytesCount = bytesLeft;
      }
    }

    internal void Ucs4ToUTF16(uint code, char[] chars, int charIndex)
    {
      chars[charIndex] = (char)(XmlCharType.SurHighStart + (char)((code >> 16) - 1) + (char)((code >> 10) & 0x3F));
      chars[charIndex + 1] = (char)(XmlCharType.SurLowStart + (char)(code & 0x3FF));
    }
  }
}