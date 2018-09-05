namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_tokenV1Compat : Datatype_normalizedStringV1Compat
  {
    public override XmlTypeCode TypeCode { get { return XmlTypeCode.Token; } }
  }
}
