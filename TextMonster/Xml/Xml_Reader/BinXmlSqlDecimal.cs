using System;
using System.IO;

namespace TextMonster.Xml.Xml_Reader
{
  internal struct BinXmlSqlDecimal
  {
    internal byte m_bLen;
    internal byte m_bPrec;
    internal byte m_bScale;
    internal byte m_bSign;
    internal uint m_data1;
    internal uint m_data2;
    internal uint m_data3;
    internal uint m_data4;

    public bool IsPositive
    {
      get
      {
        return (m_bSign == 0);
      }
    }

    private static readonly byte NUMERIC_MAX_PRECISION = 38;            // Maximum precision of numeric
    private static readonly byte MaxPrecision = NUMERIC_MAX_PRECISION;  // max SS precision
    private static readonly byte MaxScale = NUMERIC_MAX_PRECISION;      // max SS scale

    private static readonly int x_cNumeMax = 4;
    private static readonly long x_lInt32Base = ((long)1) << 32;      // 2**32
    private static readonly ulong x_ulInt32Base = ((ulong)1) << 32;     // 2**32
    private static readonly ulong x_ulInt32BaseForMod = x_ulInt32Base - 1;    // 2**32 - 1 (0xFFF...FF)
    internal static readonly ulong x_llMax = Int64.MaxValue;   // Max of Int64
    //private static readonly uint x_ulBase10 = 10;
    private static readonly double DUINT_BASE = (double)x_lInt32Base;     // 2**32
    private static readonly double DUINT_BASE2 = DUINT_BASE * DUINT_BASE;  // 2**64
    private static readonly double DUINT_BASE3 = DUINT_BASE2 * DUINT_BASE; // 2**96
    //private static readonly double DMAX_NUME = 1.0e+38;                  // Max value of numeric
    //private static readonly uint DBL_DIG = 17;                       // Max decimal digits of double
    //private static readonly byte x_cNumeDivScaleMin = 6;     // Minimum result scale of numeric division
    // Array of multipliers for lAdjust and Ceiling/Floor.
    private static readonly uint[] x_rgulShiftBase = new uint[9] {
            10,
            10 * 10,
            10 * 10 * 10,
            10 * 10 * 10 * 10,
            10 * 10 * 10 * 10 * 10,
            10 * 10 * 10 * 10 * 10 * 10,
            10 * 10 * 10 * 10 * 10 * 10 * 10,
            10 * 10 * 10 * 10 * 10 * 10 * 10 * 10,
            10 * 10 * 10 * 10 * 10 * 10 * 10 * 10 * 10
        };

    public BinXmlSqlDecimal(byte[] data, int offset, bool trim)
    {
      byte b = data[offset];
      switch (b)
      {
        case 7: m_bLen = 1; break;
        case 11: m_bLen = 2; break;
        case 15: m_bLen = 3; break;
        case 19: m_bLen = 4; break;
        default: throw new XmlException(Res.XmlBinary_InvalidSqlDecimal, (string[])null);
      }
      m_bPrec = data[offset + 1];
      m_bScale = data[offset + 2];
      m_bSign = 0 == data[offset + 3] ? (byte)1 : (byte)0;
      m_data1 = UIntFromByteArray(data, offset + 4);
      m_data2 = (m_bLen > 1) ? UIntFromByteArray(data, offset + 8) : 0;
      m_data3 = (m_bLen > 2) ? UIntFromByteArray(data, offset + 12) : 0;
      m_data4 = (m_bLen > 3) ? UIntFromByteArray(data, offset + 16) : 0;
      if (m_bLen == 4 && m_data4 == 0)
        m_bLen = 3;
      if (m_bLen == 3 && m_data3 == 0)
        m_bLen = 2;
      if (m_bLen == 2 && m_data2 == 0)
        m_bLen = 1;
      if (trim)
      {
        TrimTrailingZeros();
      }
    }

    public void Write(Stream strm)
    {
      strm.WriteByte((byte)(this.m_bLen * 4 + 3));
      strm.WriteByte(this.m_bPrec);
      strm.WriteByte(this.m_bScale);
      strm.WriteByte(0 == this.m_bSign ? (byte)1 : (byte)0);
      WriteUI4(this.m_data1, strm);
      if (this.m_bLen > 1)
      {
        WriteUI4(this.m_data2, strm);
        if (this.m_bLen > 2)
        {
          WriteUI4(this.m_data3, strm);
          if (this.m_bLen > 3)
          {
            WriteUI4(this.m_data4, strm);
          }
        }
      }
    }

    private void WriteUI4(uint val, Stream strm)
    {
      strm.WriteByte((byte)(val & 0xFF));
      strm.WriteByte((byte)((val >> 8) & 0xFF));
      strm.WriteByte((byte)((val >> 16) & 0xFF));
      strm.WriteByte((byte)((val >> 24) & 0xFF));
    }

    private static uint UIntFromByteArray(byte[] data, int offset)
    {
      int val = (data[offset]) << 0;
      val |= (data[offset + 1]) << 8;
      val |= (data[offset + 2]) << 16;
      val |= (data[offset + 3]) << 24;
      return unchecked((uint)val);
    }

    // check whether is zero
    private bool FZero()
    {
      return (m_data1 == 0) && (m_bLen <= 1);
    }
    // Store data back from rguiData[] to m_data*
    private void StoreFromWorkingArray(uint[] rguiData)
    {
      m_data1 = rguiData[0];
      m_data2 = rguiData[1];
      m_data3 = rguiData[2];
      m_data4 = rguiData[3];
    }

    // Find the case where we overflowed 10**38, but not 2**128
    private bool FGt10_38(uint[] rglData)
    {
      //Debug.Assert(rglData.Length == 4, "rglData.Length == 4", "Wrong array length: " + rglData.Length.ToString(CultureInfo.InvariantCulture));
      return rglData[3] >= 0x4b3b4ca8L && ((rglData[3] > 0x4b3b4ca8L) || (rglData[2] > 0x5a86c47aL) || (rglData[2] == 0x5a86c47aL) && (rglData[1] >= 0x098a2240L));
    }


    // Multi-precision one super-digit divide in place.
    // U = U / D,
    // R = U % D
    // Length of U can decrease
    private static void MpDiv1(uint[] rgulU,      // InOut| U
                               ref int ciulU,      // InOut| # of digits in U
                               uint iulD,       // In    | D
                               out uint iulR        // Out    | R
                               )
    {
      uint ulCarry = 0;
      ulong dwlAccum;
      ulong ulD = (ulong)iulD;
      int idU = ciulU;

      while (idU > 0)
      {
        idU--;
        dwlAccum = (((ulong)ulCarry) << 32) + (ulong)(rgulU[idU]);
        rgulU[idU] = (uint)(dwlAccum / ulD);
        ulCarry = (uint)(dwlAccum - (ulong)rgulU[idU] * ulD);  // (ULONG) (dwlAccum % iulD)
      }

      iulR = ulCarry;
      MpNormalize(rgulU, ref ciulU);
    }
    // Normalize multi-precision number - remove leading zeroes
    private static void MpNormalize(uint[] rgulU,      // In   | Number
                                    ref int ciulU       // InOut| # of digits
                                    )
    {
      while (ciulU > 1 && rgulU[ciulU - 1] == 0)
        ciulU--;
    }

    //    AdjustScale()
    //
    //    Adjust number of digits to the right of the decimal point.
    //    A positive adjustment increases the scale of the numeric value
    //    while a negative adjustment decreases the scale.  When decreasing
    //    the scale for the numeric value, the remainder is checked and
    //    rounded accordingly.
    //
    internal void AdjustScale(int digits, bool fRound)
    {
      uint ulRem;                  //Remainder when downshifting
      uint ulShiftBase;            //What to multiply by to effect scale adjust
      bool fNeedRound = false;     //Do we really need to round?
      byte bNewScale, bNewPrec;
      int lAdjust = digits;

      //If downshifting causes truncation of data
      if (lAdjust + m_bScale < 0)
        throw new XmlException(Res.SqlTypes_ArithTruncation, (string)null);

      //If uphifting causes scale overflow
      if (lAdjust + m_bScale > NUMERIC_MAX_PRECISION)
        throw new XmlException(Res.SqlTypes_ArithOverflow, (string)null);

      bNewScale = (byte)(lAdjust + m_bScale);
      bNewPrec = (byte)(Math.Min(NUMERIC_MAX_PRECISION, Math.Max(1, lAdjust + m_bPrec)));
      if (lAdjust > 0)
      {
        m_bScale = bNewScale;
        m_bPrec = bNewPrec;
        while (lAdjust > 0)
        {
          //if lAdjust>=9, downshift by 10^9 each time, otherwise by the full amount
          if (lAdjust >= 9)
          {
            ulShiftBase = x_rgulShiftBase[8];
            lAdjust -= 9;
          }
          else
          {
            ulShiftBase = x_rgulShiftBase[lAdjust - 1];
            lAdjust = 0;
          }

          MultByULong(ulShiftBase);
        }
      }
      else if (lAdjust < 0)
      {
        do
        {
          if (lAdjust <= -9)
          {
            ulShiftBase = x_rgulShiftBase[8];
            lAdjust += 9;
          }
          else
          {
            ulShiftBase = x_rgulShiftBase[-lAdjust - 1];
            lAdjust = 0;
          }

          ulRem = DivByULong(ulShiftBase);
        } while (lAdjust < 0);

        // Do we really need to round?
        fNeedRound = (ulRem >= ulShiftBase / 2);
        m_bScale = bNewScale;
        m_bPrec = bNewPrec;
      }

      // After adjusting, if the result is 0 and remainder is less than 5,
      // set the sign to be positive and return.
      if (fNeedRound && fRound)
      {
        // If remainder is 5 or above, increment/decrement by 1.
        AddULong(1);
      }
      else if (FZero())
        this.m_bSign = 0;
    }
    //    AddULong()
    //
    //    Add ulAdd to this numeric.  The result will be returned in *this.
    //
    //    Parameters:
    //        this    - IN Operand1 & OUT Result
    //        ulAdd    - IN operand2.
    //
    private void AddULong(uint ulAdd)
    {
      ulong dwlAccum = (ulong)ulAdd;
      int iData;                  // which UI4 in this we are on
      int iDataMax = (int)m_bLen; // # of UI4s in this
      uint[] rguiData = new uint[4] { m_data1, m_data2, m_data3, m_data4 };

      // Add, starting at the LS UI4 until out of UI4s or no carry
      iData = 0;
      do
      {
        dwlAccum += (ulong)rguiData[iData];
        rguiData[iData] = (uint)dwlAccum;       // equivalent to mod x_dwlBaseUI4
        dwlAccum >>= 32;                        // equivalent to dwlAccum /= x_dwlBaseUI4;
        if (0 == dwlAccum)
        {
          StoreFromWorkingArray(rguiData);
          return;
        }

        iData++;
      } while (iData < iDataMax);

      // Either overflowed
      if (iData == x_cNumeMax)
        throw new XmlException(Res.SqlTypes_ArithOverflow, (string)null);

      // Or need to extend length by 1 UI4
      rguiData[iData] = (uint)dwlAccum;
      m_bLen++;
      if (FGt10_38(rguiData))
        throw new XmlException(Res.SqlTypes_ArithOverflow, (string)null);

      StoreFromWorkingArray(rguiData);
    }
    // multiply by a long integer
    private void MultByULong(uint uiMultiplier)
    {
      int iDataMax = m_bLen; // How many UI4s currently in *this
      ulong dwlAccum = 0;       // accumulated sum
      ulong dwlNextAccum = 0;   // accumulation past dwlAccum
      int iData;              // which UI4 in *This we are on.
      uint[] rguiData = new uint[4] { m_data1, m_data2, m_data3, m_data4 };

      for (iData = 0; iData < iDataMax; iData++)
      {
        ulong ulTemp = (ulong)rguiData[iData];

        dwlNextAccum = ulTemp * (ulong)uiMultiplier;
        dwlAccum += dwlNextAccum;
        if (dwlAccum < dwlNextAccum)        // Overflow of int64 add
          dwlNextAccum = x_ulInt32Base;   // how much to add to dwlAccum after div x_dwlBaseUI4
        else
          dwlNextAccum = 0;

        rguiData[iData] = (uint)dwlAccum;           // equivalent to mod x_dwlBaseUI4
        dwlAccum = (dwlAccum >> 32) + dwlNextAccum; // equivalent to div x_dwlBaseUI4
      }

      // If any carry,
      if (dwlAccum != 0)
      {
        // Either overflowed
        if (iDataMax == x_cNumeMax)
          throw new XmlException(Res.SqlTypes_ArithOverflow, (string)null);

        // Or extend length by one uint
        rguiData[iDataMax] = (uint)dwlAccum;
        m_bLen++;
      }

      if (FGt10_38(rguiData))
        throw new XmlException(Res.SqlTypes_ArithOverflow, (string)null);

      StoreFromWorkingArray(rguiData);
    }
    //    DivByULong()
    //
    //    Divide numeric value by a ULONG.  The result will be returned
    //    in the dividend *this.
    //
    //    Parameters:
    //        this        - IN Dividend & OUT Result
    //        ulDivisor    - IN Divisor
    //    Returns:        - OUT Remainder
    //
    internal uint DivByULong(uint iDivisor)
    {
      ulong dwlDivisor = (ulong)iDivisor;
      ulong dwlAccum = 0;           //Accumulated sum
      uint ulQuotientCur = 0;      // Value of the current UI4 of the quotient
      bool fAllZero = true;    // All of the quotient (so far) has been 0
      int iData;              //Which UI4 currently on

      // Check for zero divisor.
      if (dwlDivisor == 0)
        throw new XmlException(Res.SqlTypes_DivideByZero, (string)null);

      // Copy into array, so that we can iterate through the data
      uint[] rguiData = new uint[4] { m_data1, m_data2, m_data3, m_data4 };

      // Start from the MS UI4 of quotient, divide by divisor, placing result
      //        in quotient and carrying the remainder.
      //DEVNOTE DWORDLONG sufficient accumulator since:
      //        Accum < Divisor <= 2^32 - 1    at start each loop
      //                                    initially,and mod end previous loop
      //        Accum*2^32 < 2^64 - 2^32
      //                                    multiply both side by 2^32 (x_dwlBaseUI4)
      //        Accum*2^32 + m_rgulData < 2^64
      //                                    rglData < 2^32
      for (iData = m_bLen; iData > 0; iData--)
      {
        dwlAccum = (dwlAccum << 32) + (ulong)(rguiData[iData - 1]); // dwlA*x_dwlBaseUI4 + rglData

        //Update dividend to the quotient.
        ulQuotientCur = (uint)(dwlAccum / dwlDivisor);
        rguiData[iData - 1] = ulQuotientCur;

        //Remainder to be carried to the next lower significant byte.
        dwlAccum = dwlAccum % dwlDivisor;

        // While current part of quotient still 0, reduce length
        fAllZero = fAllZero && (ulQuotientCur == 0);
        if (fAllZero)
          m_bLen--;
      }

      StoreFromWorkingArray(rguiData);

      // If result is 0, preserve sign but set length to 5
      if (fAllZero)
        m_bLen = 1;

      // return the remainder
      return (uint)dwlAccum;
    }

    //Determine the number of uints needed for a numeric given a precision
    //Precision        Length
    //    0            invalid
    //    1-9            1
    //    10-19        2
    //    20-28        3
    //    29-38        4
    // The array in Shiloh. Listed here for comparison.
    //private static readonly byte[] rgCLenFromPrec = new byte[] {5,5,5,5,5,5,5,5,5,9,9,9,9,9,
    //    9,9,9,9,9,13,13,13,13,13,13,13,13,13,17,17,17,17,17,17,17,17,17,17};
    private static readonly byte[] rgCLenFromPrec = new byte[] {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4
        };
    private static byte CLenFromPrec(byte bPrec)
    {
      return rgCLenFromPrec[bPrec - 1];
    }

    private static char ChFromDigit(uint uiDigit)
    {
      return (char)(uiDigit + '0');
    }

    public Decimal ToDecimal()
    {
      if ((int)m_data4 != 0 || m_bScale > 28)
        throw new XmlException(Res.SqlTypes_ArithOverflow, (string)null);

      return new Decimal((int)m_data1, (int)m_data2, (int)m_data3, !IsPositive, m_bScale);
    }

    void TrimTrailingZeros()
    {
      uint[] rgulNumeric = new uint[4] { m_data1, m_data2, m_data3, m_data4 };
      int culLen = m_bLen;
      uint ulRem; //Remainder of a division by x_ulBase10, i.e.,least significant digit

      // special-case 0
      if (culLen == 1 && rgulNumeric[0] == 0)
      {
        m_bScale = 0;
        return;
      }

      while (m_bScale > 0 && (culLen > 1 || rgulNumeric[0] != 0))
      {
        MpDiv1(rgulNumeric, ref culLen, 10, out ulRem);
        if (ulRem == 0)
        {
          m_data1 = rgulNumeric[0];
          m_data2 = rgulNumeric[1];
          m_data3 = rgulNumeric[2];
          m_data4 = rgulNumeric[3];
          m_bScale--;
        }
        else
        {
          break;
        }
      }
      if (m_bLen == 4 && m_data4 == 0)
        m_bLen = 3;
      if (m_bLen == 3 && m_data3 == 0)
        m_bLen = 2;
      if (m_bLen == 2 && m_data2 == 0)
        m_bLen = 1;
    }

    public override String ToString()
    {
      // Make local copy of data to avoid modifying input.
      uint[] rgulNumeric = new uint[4] { m_data1, m_data2, m_data3, m_data4 };
      int culLen = m_bLen;
      char[] pszTmp = new char[NUMERIC_MAX_PRECISION + 1];   //Local Character buffer to hold
      //the decimal digits, from the
      //lowest significant to highest significant

      int iDigits = 0;//Number of significant digits
      uint ulRem; //Remainder of a division by x_ulBase10, i.e.,least significant digit

      // Build the final numeric string by inserting the sign, reversing
      // the order and inserting the decimal number at the correct position

      //Retrieve each digit from the lowest significant digit
      while (culLen > 1 || rgulNumeric[0] != 0)
      {
        MpDiv1(rgulNumeric, ref culLen, 10, out ulRem);
        //modulo x_ulBase10 is the lowest significant digit
        pszTmp[iDigits++] = ChFromDigit(ulRem);
      }

      // if scale of the number has not been
      // reached pad remaining number with zeros.
      while (iDigits <= m_bScale)
      {
        pszTmp[iDigits++] = ChFromDigit(0);
      }

      bool fPositive = IsPositive;

      // Increment the result length if negative (need to add '-')
      int uiResultLen = fPositive ? iDigits : iDigits + 1;

      // Increment the result length if scale > 0 (need to add '.')
      if (m_bScale > 0)
        uiResultLen++;

      char[] szResult = new char[uiResultLen];
      int iCurChar = 0;

      if (!fPositive)
        szResult[iCurChar++] = '-';

      while (iDigits > 0)
      {
        if (iDigits-- == m_bScale)
          szResult[iCurChar++] = '.';
        szResult[iCurChar++] = pszTmp[iDigits];
      }

      return new String(szResult);
    }

  }
}
