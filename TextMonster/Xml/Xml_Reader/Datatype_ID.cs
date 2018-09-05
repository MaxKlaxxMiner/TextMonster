namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_ID : Datatype_NCName
  {

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.Id; } }

    public override XmlTokenizedType TokenizedType { get { return XmlTokenizedType.ID; } }
  }
}
