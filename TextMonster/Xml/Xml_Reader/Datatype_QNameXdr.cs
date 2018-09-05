using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_QNameXdr : Datatype_anySimpleType
  {
    static readonly Type atomicValueType = typeof(XmlQualifiedName);
    static readonly Type listValueType = typeof(XmlQualifiedName[]);

    public override XmlTokenizedType TokenizedType { get { return XmlTokenizedType.QName; } }

    public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
    {
      if (s == null || s.Length == 0)
      {
        throw new XmlSchemaException(Res.Sch_EmptyAttributeValue, string.Empty);
      }
      if (nsmgr == null)
      {
        throw new ArgumentNullException("nsmgr");
      }
      string prefix;
      try
      {
        return XmlQualifiedName.Parse(s.Trim(), nsmgr, out prefix);
      }
      catch (XmlSchemaException e)
      {
        throw e;
      }
      catch (Exception e)
      {
        throw new XmlSchemaException(Res.GetString(Res.Sch_InvalidValue, s), e);
      }
    }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }
  }
}
