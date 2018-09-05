using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class UTF16Decoder : System.Text.Decoder
  {
    private bool bigEndian;
    private int lastByte;
    private const int CharSize = 2;

    public UTF16Decoder(bool bigEndian)
    {
      this.lastByte = -1;
      this.bigEndian = bigEndian;
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
      return GetCharCount(bytes, index, count, false);
    }

    public override int GetCharCount(byte[] bytes, int index, int count, bool flush)
    {
      int byteCount = count + ((lastByte >= 0) ? 1 : 0);
      if (flush && (byteCount % CharSize != 0))
      {
        throw new ArgumentException(Res.GetString(Res.Enc_InvalidByteInEncoding, new object[1] { -1 }), (string)null);
      }
      return byteCount / CharSize;
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
      int charCount = GetCharCount(bytes, byteIndex, byteCount);

      if (lastByte >= 0)
      {
        if (byteCount == 0)
        {
          return charCount;
        }
        int nextByte = bytes[byteIndex++];
        byteCount--;

        chars[charIndex++] = bigEndian
          ? (char)(lastByte << 8 | nextByte)
          : (char)(nextByte << 8 | lastByte);
        lastByte = -1;
      }

      if ((byteCount & 1) != 0)
      {
        lastByte = bytes[byteIndex + --byteCount];
      }

      // use the fast BlockCopy if possible
      if (bigEndian == BitConverter.IsLittleEndian)
      {
        int byteEnd = byteIndex + byteCount;
        if (bigEndian)
        {
          while (byteIndex < byteEnd)
          {
            int hi = bytes[byteIndex++];
            int lo = bytes[byteIndex++];
            chars[charIndex++] = (char)(hi << 8 | lo);
          }
        }
        else
        {
          while (byteIndex < byteEnd)
          {
            int lo = bytes[byteIndex++];
            int hi = bytes[byteIndex++];
            chars[charIndex++] = (char)(hi << 8 | lo);
          }
        }
      }
      else
      {
        Buffer.BlockCopy(bytes, byteIndex, chars, charIndex * CharSize, byteCount);
      }
      return charCount;
    }

    public override void Convert(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed)
    {
      charsUsed = 0;
      bytesUsed = 0;

      if (lastByte >= 0)
      {
        if (byteCount == 0)
        {
          completed = true;
          return;
        }
        int nextByte = bytes[byteIndex++];
        byteCount--;
        bytesUsed++;

        chars[charIndex++] = bigEndian
          ? (char)(lastByte << 8 | nextByte)
          : (char)(nextByte << 8 | lastByte);
        charCount--;
        charsUsed++;
        lastByte = -1;
      }

      if (charCount * CharSize < byteCount)
      {
        byteCount = charCount * CharSize;
        completed = false;
      }
      else
      {
        completed = true;
      }

      if (bigEndian == BitConverter.IsLittleEndian)
      {
        int i = byteIndex;
        int byteEnd = i + (byteCount & ~0x1);
        if (bigEndian)
        {
          while (i < byteEnd)
          {
            int hi = bytes[i++];
            int lo = bytes[i++];
            chars[charIndex++] = (char)(hi << 8 | lo);
          }
        }
        else
        {
          while (i < byteEnd)
          {
            int lo = bytes[i++];
            int hi = bytes[i++];
            chars[charIndex++] = (char)(hi << 8 | lo);
          }
        }
      }
      else
      {
        Buffer.BlockCopy(bytes, byteIndex, chars, charIndex * CharSize, (int)(byteCount & ~0x1));
      }
      charsUsed += byteCount / CharSize;
      bytesUsed += byteCount;

      if ((byteCount & 1) != 0)
      {
        lastByte = bytes[byteIndex + byteCount - 1];
      }
    }
  }
}