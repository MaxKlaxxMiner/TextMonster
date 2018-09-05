namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_month : Datatype_dateTimeBase
  {
    public override XmlTypeCode TypeCode { get { return XmlTypeCode.GMonth; } }

    internal Datatype_month() : base(XsdDateTimeFlags.GMonth) { }
  }
}
