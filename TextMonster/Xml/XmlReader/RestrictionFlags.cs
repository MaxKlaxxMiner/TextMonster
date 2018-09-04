using System;

namespace TextMonster.Xml.XmlReader
{
  [Flags]
  internal enum RestrictionFlags
  {
    Length = 0x0001,
    MinLength = 0x0002,
    MaxLength = 0x0004,
    Pattern = 0x0008,
    Enumeration = 0x0010,
    WhiteSpace = 0x0020,
    MaxInclusive = 0x0040,
    MaxExclusive = 0x0080,
    MinInclusive = 0x0100,
    MinExclusive = 0x0200,
    TotalDigits = 0x0400,
    FractionDigits = 0x0800,
  }
}
