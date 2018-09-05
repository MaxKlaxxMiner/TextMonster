using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_doubleXdr : Datatype_double
  {
    public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
    {
      double value;
      try
      {
        value = XmlConvert.ToDouble(s);
      }
      catch (Exception e)
      {
        throw new XmlSchemaException(Res.GetString(Res.Sch_InvalidValue, s), e);
      }
      if (double.IsInfinity(value) || double.IsNaN(value))
      {
        throw new XmlSchemaException(Res.Sch_InvalidValue, s);
      }
      return value;
    }
  }
}
