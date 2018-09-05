namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_NMTOKEN : Datatype_token
  {

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.NmToken; } }

    public override XmlTokenizedType TokenizedType { get { return XmlTokenizedType.NMTOKEN; } }
  }
}
