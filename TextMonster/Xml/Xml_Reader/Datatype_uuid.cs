using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_uuid : Datatype_anySimpleType
  {
    static readonly Type atomicValueType = typeof(Guid);
    static readonly Type listValueType = typeof(Guid[]);

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override RestrictionFlags ValidRestrictionFlags { get { return 0; } }

    internal override int Compare(object value1, object value2)
    {
      return ((Guid)value1).Equals(value2) ? 0 : -1;
    }

    public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
    {
      try
      {
        return XmlConvert.ToGuid(s);
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

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      Guid guid;
      exception = XmlConvert.TryToGuid(s, out guid);
      if (exception != null) goto Error;

      typedValue = guid;

      return null;

    Error:
      return exception;
    }
  }
}
