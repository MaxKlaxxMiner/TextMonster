namespace TextMonster.Xml.Xml_Reader
{
  internal enum XmlAttributeFlags
  {
    Enum = 0x1,
    Array = 0x2,
    Text = 0x4,
    ArrayItems = 0x8,
    Elements = 0x10,
    Attribute = 0x20,
    Root = 0x40,
    Type = 0x80,
    AnyElements = 0x100,
    AnyAttribute = 0x200,
    ChoiceIdentifier = 0x400,
    XmlnsDeclarations = 0x800,
  }
}