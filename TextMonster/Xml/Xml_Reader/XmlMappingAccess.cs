using System;

namespace TextMonster.Xml.Xml_Reader
{
  [Flags]
  public enum XmlMappingAccess
  {
    Read = 0x01,
    Write = 0x02,
  }
}