namespace TextMonster.Xml.Xml_Reader
{
  internal enum ValidatorState
  {
    None,
    Start,
    TopLevelAttribute,
    TopLevelTextOrWS,
    Element,
    Attribute,
    EndOfAttributes,
    Text,
    Whitespace,
    EndElement,
    SkipToEndElement,
    Finish,
  }
}
