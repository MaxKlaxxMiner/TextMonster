using System;

namespace TextMonster.Xml.Xml_Reader
{
  [Flags]
  public enum XmlMappingAccess
  {
    None = 0x00,
    Read = 0x01,
    Write = 0x02,
  }
}