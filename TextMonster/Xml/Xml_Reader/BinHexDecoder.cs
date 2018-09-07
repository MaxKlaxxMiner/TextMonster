﻿using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class BinHexDecoder : IncrementalReadDecoder
  {
    //
    // Fields
    //
    byte[] buffer;
    int startIndex;
    int curIndex;
    int endIndex;
    bool hasHalfByteCached;
    byte cachedHalfByte;

    internal override bool IsFull
    {
      get
      {
        return curIndex == endIndex;
      }
    }

    internal override unsafe int Decode(char[] chars, int startPos, int len)
    {
      if (chars == null)
      {
        throw new ArgumentNullException("chars");
      }
      if (len < 0)
      {
        throw new ArgumentOutOfRangeException("len");
      }
      if (startPos < 0)
      {
        throw new ArgumentOutOfRangeException("startPos");
      }
      if (chars.Length - startPos < len)
      {
        throw new ArgumentOutOfRangeException("len");
      }

      if (len == 0)
      {
        return 0;
      }
      int bytesDecoded, charsDecoded;
      fixed (char* pChars = &chars[startPos])
      {
        fixed (byte* pBytes = &buffer[curIndex])
        {
          Decode(pChars, pChars + len, pBytes, pBytes + (endIndex - curIndex),
                  ref hasHalfByteCached, ref cachedHalfByte, out charsDecoded, out bytesDecoded);
        }
      }
      curIndex += bytesDecoded;
      return charsDecoded;
    }

    //
    // Static methods
    //
    public static unsafe byte[] Decode(char[] chars, bool allowOddChars)
    {
      if (chars == null)
      {
        throw new ArgumentNullException("chars");
      }

      int len = chars.Length;
      if (len == 0)
      {
        return new byte[0];
      }

      byte[] bytes = new byte[(len + 1) / 2];
      int bytesDecoded, charsDecoded;
      bool hasHalfByteCached = false;
      byte cachedHalfByte = 0;

      fixed (char* pChars = &chars[0])
      {
        fixed (byte* pBytes = &bytes[0])
        {
          Decode(pChars, pChars + len, pBytes, pBytes + bytes.Length, ref hasHalfByteCached, ref cachedHalfByte, out charsDecoded, out bytesDecoded);
        }
      }

      if (hasHalfByteCached && !allowOddChars)
      {
        throw new XmlException(Res.Xml_InvalidBinHexValueOddCount, new string(chars));
      }

      if (bytesDecoded < bytes.Length)
      {
        byte[] tmp = new byte[bytesDecoded];
        Array.Copy(bytes, 0, tmp, 0, bytesDecoded);
        bytes = tmp;
      }

      return bytes;
    }

    //
    // Private methods
    //

    private static unsafe void Decode(char* pChars, char* pCharsEndPos,
                                byte* pBytes, byte* pBytesEndPos,
                                ref bool hasHalfByteCached, ref byte cachedHalfByte,
                out int charsDecoded, out int bytesDecoded)
    {
      char* pChar = pChars;
      byte* pByte = pBytes;
      XmlCharType xmlCharType = XmlCharType.Instance;
      while (pChar < pCharsEndPos && pByte < pBytesEndPos)
      {
        byte halfByte;
        char ch = *pChar++;

        if (ch >= 'a' && ch <= 'f')
        {
          halfByte = (byte)(ch - 'a' + 10);
        }
        else if (ch >= 'A' && ch <= 'F')
        {
          halfByte = (byte)(ch - 'A' + 10);
        }
        else if (ch >= '0' && ch <= '9')
        {
          halfByte = (byte)(ch - '0');
        }
        else if ((xmlCharType.charProperties[ch] & XmlCharType.fWhitespace) != 0)
        { // else if ( xmlCharType.IsWhiteSpace( ch ) ) {
          continue;
        }
        else
        {
          throw new XmlException(Res.Xml_InvalidBinHexValue, new string(pChars, 0, (int)(pCharsEndPos - pChars)));
        }

        if (hasHalfByteCached)
        {
          *pByte++ = (byte)((cachedHalfByte << 4) + halfByte);
          hasHalfByteCached = false;
        }
        else
        {
          cachedHalfByte = halfByte;
          hasHalfByteCached = true;
        }
      }

      bytesDecoded = (int)(pByte - pBytes);
      charsDecoded = (int)(pChar - pChars);
    }
  }
}
