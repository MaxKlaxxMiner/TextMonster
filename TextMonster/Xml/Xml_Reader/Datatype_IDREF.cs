namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_IDREF : Datatype_NCName
  {

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.Idref; } }

    public override XmlTokenizedType TokenizedType { get { return XmlTokenizedType.IDREF; } }
  }
}
