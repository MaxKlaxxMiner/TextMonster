using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Ucs4Decoder2143 : Ucs4Decoder
  {

    internal override int GetFullChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
      uint code;
      int i, j;

      byteCount += byteIndex;

      for (i = byteIndex, j = charIndex; i + 3 < byteCount; )
      {
        code = (uint)((bytes[i + 1] << 24) | (bytes[i] << 16) | (bytes[i + 3] << 8) | bytes[i + 2]);
        if (code > 0x10FFFF)
        {
          throw new ArgumentException(Res.GetString(Res.Enc_InvalidByteInEncoding, new object[1] { i }), (string)null);
        }
        else if (code > 0xFFFF)
        {
          Ucs4ToUTF16(code, chars, j);
          j++;
        }
        else
        {
          if (XmlCharType.IsSurrogate((int)code))
          {
            throw new XmlException(Res.Xml_InvalidCharInThisEncoding, string.Empty);
          }
          else
          {
            chars[j] = (char)code;
          }
        }
        j++;
        i += 4;
      }
      return j - charIndex;
    }
  }
}