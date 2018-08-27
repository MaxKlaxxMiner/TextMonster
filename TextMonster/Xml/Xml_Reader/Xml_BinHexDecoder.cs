namespace TextMonster.Xml.Xml_Reader
{
  // ReSharper disable once InconsistentNaming
  internal class Xml_BinHexDecoder : IncrementalReadDecoder
  {
    private byte[] buffer;
    private int startIndex;
    private int curIndex;
    private int endIndex;
    private bool hasHalfByteCached;
    private byte cachedHalfByte;

    internal override int DecodedCount
    {
      get
      {
        return this.curIndex - this.startIndex;
      }
    }

    internal override bool IsFull
    {
      get
      {
        return this.curIndex == this.endIndex;
      }
    }

    internal override unsafe int Decode(char[] chars, int startPos, int len)
    {
      if (chars == null)
        throw new ArgumentNullException("chars");
      if (len < 0)
        throw new ArgumentOutOfRangeException("len");
      if (startPos < 0)
        throw new ArgumentOutOfRangeException("startPos");
      if (chars.Length - startPos < len)
        throw new ArgumentOutOfRangeException("len");
      if (len == 0)
        return 0;
      int num1;
      int num2;
      fixed (char* chPtr = &chars[startPos])
        fixed (byte* numPtr = &this.buffer[this.curIndex])
        {
          IntPtr num3 = (IntPtr) chPtr;
          IntPtr num4 = (IntPtr) len * 2;
          IntPtr num5 = num3 + num4;
          IntPtr num6 = (IntPtr) numPtr;
          int num7 = this.endIndex - this.curIndex;
          IntPtr num8 = num6 + num7;
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          bool& hasHalfByteCached = @this.hasHalfByteCached;
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          byte& cachedHalfByte = @this.cachedHalfByte;
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          int& charsDecoded = @num1;
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          int& bytesDecoded = @num2;
          Xml_BinHexDecoder.Decode((char*) num3, (char*) num5, (byte*) num6, (byte*) num8, hasHalfByteCached, cachedHalfByte, charsDecoded, bytesDecoded);
        }
      this.curIndex = this.curIndex + num2;
      return num1;
    }

    internal override unsafe int Decode(string str, int startPos, int len)
    {
      if (str == null)
        throw new ArgumentNullException("str");
      if (len < 0)
        throw new ArgumentOutOfRangeException("len");
      if (startPos < 0)
        throw new ArgumentOutOfRangeException("startPos");
      if (str.Length - startPos < len)
        throw new ArgumentOutOfRangeException("len");
      if (len == 0)
        return 0;
      string str1 = str;
      char* chPtr = (char*) str1;
      if ((IntPtr) chPtr != IntPtr.Zero)
        chPtr += RuntimeHelpers.OffsetToStringData;
      int num1;
      int num2;
      fixed (byte* numPtr = &this.buffer[this.curIndex])
      {
        IntPtr num3 = (IntPtr) (chPtr + startPos);
        IntPtr num4 = (IntPtr) (chPtr + startPos + len);
        IntPtr num5 = (IntPtr) numPtr;
        int num6 = this.endIndex - this.curIndex;
        IntPtr num7 = num5 + num6;
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        bool& hasHalfByteCached = @this.hasHalfByteCached;
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        byte& cachedHalfByte = @this.cachedHalfByte;
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        int& charsDecoded = @num1;
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        int& bytesDecoded = @num2;
        Xml_BinHexDecoder.Decode((char*) num3, (char*) num4, (byte*) num5, (byte*) num7, hasHalfByteCached, cachedHalfByte, charsDecoded, bytesDecoded);
      }
      str1 = (string) null;
      this.curIndex = this.curIndex + num2;
      return num1;
    }

    internal override void Reset()
    {
      this.hasHalfByteCached = false;
      this.cachedHalfByte = (byte)0;
    }

    internal override void SetNextOutputBuffer(Array buffer, int index, int count)
    {
      this.buffer = (byte[])buffer;
      this.startIndex = index;
      this.curIndex = index;
      this.endIndex = index + count;
    }

    public static unsafe byte[] Decode(char[] chars, bool allowOddChars)
    {
      if (chars == null)
        throw new ArgumentNullException("chars");
      int length1 = chars.Length;
      if (length1 == 0)
        return new byte[0];
      byte[] numArray1 = new byte[(length1 + 1) / 2];
      bool flag = false;
      byte num1 = (byte) 0;
      int length2;
      fixed (char* chPtr = &chars[0])
        fixed (byte* numPtr = &numArray1[0])
        {
          IntPtr num2 = (IntPtr) chPtr;
          IntPtr num3 = (IntPtr) length1 * 2;
          IntPtr num4 = num2 + num3;
          IntPtr num5 = (IntPtr) numPtr;
          int length3 = numArray1.Length;
          IntPtr num6 = num5 + length3;
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          bool& hasHalfByteCached = @flag;
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          byte& cachedHalfByte = @num1;
          int num7;
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          int& charsDecoded = @num7;
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          int& bytesDecoded = @length2;
          Xml_BinHexDecoder.Decode((char*) num2, (char*) num4, (byte*) num5, (byte*) num6, hasHalfByteCached, cachedHalfByte, charsDecoded, bytesDecoded);
        }
      if (flag && !allowOddChars)
        throw new XmlException("Xml_InvalidBinHexValueOddCount", new string(chars));
      if (length2 < numArray1.Length)
      {
        byte[] numArray2 = new byte[length2];
        Array.Copy((Array) numArray1, 0, (Array) numArray2, 0, length2);
        numArray1 = numArray2;
      }
      return numArray1;
    }

    private static unsafe void Decode(char* pChars, char* pCharsEndPos, byte* pBytes, byte* pBytesEndPos, ref bool hasHalfByteCached, ref byte cachedHalfByte, out int charsDecoded, out int bytesDecoded)
    {
      char* chPtr = pChars;
      byte* numPtr = pBytes;
      XmlCharType instance = XmlCharType.Instance;
      while (chPtr < pCharsEndPos && numPtr < pBytesEndPos)
      {
        char ch = *chPtr++;
        byte num;
        if ((int)ch >= 97 && (int)ch <= 102)
          num = (byte)((int)ch - 97 + 10);
        else if ((int)ch >= 65 && (int)ch <= 70)
          num = (byte)((int)ch - 65 + 10);
        else if ((int)ch >= 48 && (int)ch <= 57)
        {
          num = (byte)((uint)ch - 48U);
        }
        else
        {
          if (((int)instance.charProperties[(int)ch] & 1) == 0)
            throw new XmlException("Xml_InvalidBinHexValue", new string(pChars, 0, (int)(pCharsEndPos - pChars)));
          continue;
        }
        if (hasHalfByteCached)
        {
          *numPtr++ = (byte)(((uint)cachedHalfByte << 4) + (uint)num);
          hasHalfByteCached = false;
        }
        else
        {
          cachedHalfByte = num;
          hasHalfByteCached = true;
        }
      }
      bytesDecoded = (int)(numPtr - pBytes);
      charsDecoded = (int)(chPtr - pChars);
    }
  }
}
