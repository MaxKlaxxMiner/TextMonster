using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace TextMonster.Xml.Xml_Reader
{
  // ReSharper disable once InconsistentNaming
  internal struct Xml_CharType
  {
    internal const int SurHighStart = 55296;
    internal const int SurHighEnd = 56319;
    internal const int SurLowStart = 56320;
    internal const int SurLowEnd = 57343;
    internal const int SurMask = 64512;
    internal const int fWhitespace = 1;
    internal const int fLetter = 2;
    internal const int fNCStartNameSC = 4;
    internal const int fNCNameSC = 8;
    internal const int fCharData = 16;
    internal const int fNCNameXml4e = 32;
    internal const int fText = 64;
    internal const int fAttrValue = 128;
    private const string s_PublicIdBitmap = "␀\0ﾻ꿿\xFFFF蟿\xFFFE\x07FF";
    private const uint CharPropertiesSize = 65536U;
    private static object s_Lock;
    private static volatile unsafe byte* s_CharProperties;
    internal unsafe byte* charProperties;

    private static object StaticLock
    {
      get
      {
        if (Xml_CharType.s_Lock == null)
        {
          object obj = new object();
          Interlocked.CompareExchange<object>(ref Xml_CharType.s_Lock, obj, (object)null);
        }
        return Xml_CharType.s_Lock;
      }
    }

    public static unsafe Xml_CharType Instance
    {
      get
      {
        if ((IntPtr)Xml_CharType.s_CharProperties == IntPtr.Zero)
          Xml_CharType.InitInstance();
        return new Xml_CharType(Xml_CharType.s_CharProperties);
      }
    }

    private unsafe Xml_CharType(byte* charProperties)
    {
      this.charProperties = charProperties;
    }

    private static unsafe void InitInstance()
    {
      object staticLock = Xml_CharType.StaticLock;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(staticLock, ref lockTaken);
        if ((IntPtr)Xml_CharType.s_CharProperties != IntPtr.Zero)
          return;
        byte* positionPointer = ((UnmanagedMemoryStream)Assembly.GetExecutingAssembly().GetManifestResourceStream("XmlCharType.bin")).PositionPointer;
        Thread.MemoryBarrier();
        Xml_CharType.s_CharProperties = positionPointer;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(staticLock);
      }
    }

    public unsafe bool IsWhiteSpace(char ch)
    {
      return ((uint)this.charProperties[(int)ch] & 1U) > 0U;
    }

    public bool IsExtender(char ch)
    {
      return (int)ch == 183;
    }

    public unsafe bool IsNCNameSingleChar(char ch)
    {
      return ((uint)this.charProperties[(int)ch] & 8U) > 0U;
    }

    public unsafe bool IsStartNCNameSingleChar(char ch)
    {
      return ((uint)this.charProperties[(int)ch] & 4U) > 0U;
    }

    public bool IsNameSingleChar(char ch)
    {
      if (!this.IsNCNameSingleChar(ch))
        return (int)ch == 58;
      return true;
    }

    public bool IsStartNameSingleChar(char ch)
    {
      if (!this.IsStartNCNameSingleChar(ch))
        return (int)ch == 58;
      return true;
    }

    public unsafe bool IsCharData(char ch)
    {
      return ((uint)this.charProperties[(int)ch] & 16U) > 0U;
    }

    public bool IsPubidChar(char ch)
    {
      if ((int)ch < 128)
        return ((uint)"␀\0ﾻ꿿\xFFFF蟿\xFFFE\x07FF"[(int)ch >> 4] & (uint)(1 << ((int)ch & 15))) > 0U;
      return false;
    }

    internal unsafe bool IsTextChar(char ch)
    {
      return ((uint)this.charProperties[(int)ch] & 64U) > 0U;
    }

    internal unsafe bool IsAttributeValueChar(char ch)
    {
      return ((uint)this.charProperties[(int)ch] & 128U) > 0U;
    }

    public unsafe bool IsLetter(char ch)
    {
      return ((uint)this.charProperties[(int)ch] & 2U) > 0U;
    }

    public unsafe bool IsNCNameCharXml4e(char ch)
    {
      return ((uint)this.charProperties[(int)ch] & 32U) > 0U;
    }

    public bool IsStartNCNameCharXml4e(char ch)
    {
      if (!this.IsLetter(ch))
        return (int)ch == 95;
      return true;
    }

    public bool IsNameCharXml4e(char ch)
    {
      if (!this.IsNCNameCharXml4e(ch))
        return (int)ch == 58;
      return true;
    }

    public bool IsStartNameCharXml4e(char ch)
    {
      if (!this.IsStartNCNameCharXml4e(ch))
        return (int)ch == 58;
      return true;
    }

    public static bool IsDigit(char ch)
    {
      return Xml_CharType.InRange((int)ch, 48, 57);
    }

    public static bool IsHexDigit(char ch)
    {
      if (!Xml_CharType.InRange((int)ch, 48, 57) && !Xml_CharType.InRange((int)ch, 97, 102))
        return Xml_CharType.InRange((int)ch, 65, 70);
      return true;
    }

    internal static bool IsHighSurrogate(int ch)
    {
      return Xml_CharType.InRange(ch, 55296, 56319);
    }

    internal static bool IsLowSurrogate(int ch)
    {
      return Xml_CharType.InRange(ch, 56320, 57343);
    }

    internal static bool IsSurrogate(int ch)
    {
      return Xml_CharType.InRange(ch, 55296, 57343);
    }

    internal static int CombineSurrogateChar(int lowChar, int highChar)
    {
      return lowChar - 56320 | (highChar - 55296 << 10) + 65536;
    }

    internal static void SplitSurrogateChar(int combinedChar, out char lowChar, out char highChar)
    {
      int num = combinedChar - 65536;
      lowChar = (char)(56320 + num % 1024);
      highChar = (char)(55296 + num / 1024);
    }

    internal bool IsOnlyWhitespace(string str)
    {
      return this.IsOnlyWhitespaceWithPos(str) == -1;
    }

    internal unsafe int IsOnlyWhitespaceWithPos(string str)
    {
      if (str != null)
      {
        for (int index = 0; index < str.Length; ++index)
        {
          if (((int)this.charProperties[(int)str[index]] & 1) == 0)
            return index;
        }
      }
      return -1;
    }

    internal unsafe int IsOnlyCharData(string str)
    {
      if (str != null)
      {
        for (int index = 0; index < str.Length; ++index)
        {
          if (((int)this.charProperties[(int)str[index]] & 16) == 0)
          {
            if (index + 1 >= str.Length || !Xml_CharType.IsHighSurrogate((int)str[index]) || !Xml_CharType.IsLowSurrogate((int)str[index + 1]))
              return index;
            ++index;
          }
        }
      }
      return -1;
    }

    internal static bool IsOnlyDigits(string str, int startPos, int len)
    {
      for (int index = startPos; index < startPos + len; ++index)
      {
        if (!Xml_CharType.IsDigit(str[index]))
          return false;
      }
      return true;
    }

    internal static bool IsOnlyDigits(char[] chars, int startPos, int len)
    {
      for (int index = startPos; index < startPos + len; ++index)
      {
        if (!Xml_CharType.IsDigit(chars[index]))
          return false;
      }
      return true;
    }

    internal int IsPublicId(string str)
    {
      if (str != null)
      {
        for (int index = 0; index < str.Length; ++index)
        {
          if (!this.IsPubidChar(str[index]))
            return index;
        }
      }
      return -1;
    }

    private static bool InRange(int value, int start, int end)
    {
      return (uint)(value - start) <= (uint)(end - start);
    }
  }
}
