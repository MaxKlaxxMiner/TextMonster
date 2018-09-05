namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_timeNoTimeZone : Datatype_dateTimeBase
  {
    internal Datatype_timeNoTimeZone() : base(XsdDateTimeFlags.XdrTimeNoTz) { }
  }
}
