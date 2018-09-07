using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_duration : Datatype_anySimpleType
  {
    static readonly Type atomicValueType = typeof(TimeSpan);
    static readonly Type listValueType = typeof(TimeSpan[]);

    internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
    {
      return XmlMiscConverter.Create(schemaType);
    }

    internal override FacetsChecker FacetsChecker { get { return durationFacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.Duration; } }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet { get { return XmlSchemaWhiteSpace.Collapse; } }

    internal override int Compare(object value1, object value2)
    {
      return ((TimeSpan)value1).CompareTo(value2);
    }


    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;
      typedValue = null;

      if (s == null || s.Length == 0)
      {
        return new XmlSchemaException(Res.Sch_EmptyAttributeValue, string.Empty);
      }

      exception = durationFacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      TimeSpan timeSpanValue;
      exception = XmlConvert.TryToTimeSpan(s, out timeSpanValue);
      if (exception != null) goto Error;

      exception = durationFacetsChecker.CheckValueFacets(timeSpanValue, this);
      if (exception != null) goto Error;

      typedValue = timeSpanValue;

      return null;

    Error:
      return exception;
    }
  }
}
