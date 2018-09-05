using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_floatXdr : Datatype_float
  {
    public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
    {
      float value;
      try
      {
        value = XmlConvert.ToSingle(s);
      }
      catch (Exception e)
      {
        throw new XmlSchemaException(Res.GetString(Res.Sch_InvalidValue, s), e);
      }
      if (float.IsInfinity(value) || float.IsNaN(value))
      {
        throw new XmlSchemaException(Res.Sch_InvalidValue, s);
      }
      return value;
    }
  }
}
