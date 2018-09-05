using System;

namespace TextMonster.Xml.Xml_Reader
{
  [Flags]
  public enum XmlSchemaValidationFlags
  {
    None = 0x0000,
    ProcessInlineSchema = 0x0001,
    ProcessSchemaLocation = 0x0002,
    ReportValidationWarnings = 0x0004,
    ProcessIdentityConstraints = 0x0008,
    AllowXmlAttributes = 0x0010,
  }
}
