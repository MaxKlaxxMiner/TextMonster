using System;

namespace TextMonster.Xml.Xml_Reader
{
  [Flags]
  public enum CodeGenerationOptions
  {
    [XmlEnum("properties")]
    GenerateProperties = 0x1,


    [XmlEnum("order")]
    GenerateOrder = 0x08,

    [XmlEnum("enableDataBinding")]
    EnableDataBinding = 0x10,
  }
}