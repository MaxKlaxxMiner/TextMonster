namespace TextMonster.Xml.Xml_Reader
{
  internal enum TypeFlags
  {
    None = 0x0,
    Abstract = 0x1,
    Reference = 0x2,
    Special = 0x4,
    CanBeAttributeValue = 0x8,
    CanBeTextValue = 0x10,
    CanBeElementValue = 0x20,
    HasCustomFormatter = 0x40,
    AmbiguousDataType = 0x80,
    IgnoreDefault = 0x200,
    HasIsEmpty = 0x400,
    HasDefaultConstructor = 0x800,
    XmlEncodingNotRequired = 0x1000,
    UseReflection = 0x4000,
    CollapseWhitespace = 0x8000,
    OptionalValue = 0x10000,
    CtorInaccessible = 0x20000,
    UsePrivateImplementation = 0x40000,
    GenericInterface = 0x80000,
    Unsupported = 0x100000,
  }
}