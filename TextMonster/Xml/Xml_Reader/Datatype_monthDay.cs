namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_monthDay : Datatype_dateTimeBase
  {
    public override XmlTypeCode TypeCode { get { return XmlTypeCode.GMonthDay; } }

    internal Datatype_monthDay() : base(XsdDateTimeFlags.GMonthDay) { }
  }
}
