namespace TextMonster.Xml.Xml_Reader
{
  internal enum AttributeMatchState
  {
    AttributeFound,
    AnyIdAttributeFound,
    UndeclaredElementAndAttribute,
    UndeclaredAttribute,
    AnyAttributeLax,
    AnyAttributeSkip,
    ProhibitedAnyAttribute,
    ProhibitedAttribute,
    AttributeNameMismatch,
    ValidateAttributeInvalidCall,
  }
}
