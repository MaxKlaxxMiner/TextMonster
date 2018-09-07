using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_char : Datatype_anySimpleType
  {
    static readonly Type atomicValueType = typeof(char);
    static readonly Type listValueType = typeof(char[]);

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override int Compare(object value1, object value2)
    {
      // this should be culture sensitive - comparing values
      return ((char)value1).CompareTo(value2);
    }

    public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
    {
      try
      {
        return XmlConvert.ToChar(s);
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

      char charValue;
      exception = XmlConvert.TryToChar(s, out charValue);
      if (exception != null) goto Error;

      typedValue = charValue;

      return null;

    Error:
      return exception;
    }
  }
}
