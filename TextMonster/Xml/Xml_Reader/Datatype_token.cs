namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_token : Datatype_normalizedString
  {
    public override XmlTypeCode TypeCode { get { return XmlTypeCode.Token; } }
    internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet { get { return XmlSchemaWhiteSpace.Collapse; } }
  }
}
