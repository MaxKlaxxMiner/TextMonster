using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_dateTimeBase : Datatype_anySimpleType
  {
    static readonly Type atomicValueType = typeof(DateTime);
    static readonly Type listValueType = typeof(DateTime[]);
    private XsdDateTimeFlags dateTimeFlags;

    internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
    {
      return XmlDateTimeConverter.Create(schemaType);
    }

    internal override FacetsChecker FacetsChecker { get { return dateTimeFacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.DateTime; } }

    internal Datatype_dateTimeBase()
    {
    }

    internal Datatype_dateTimeBase(XsdDateTimeFlags dateTimeFlags)
    {
      this.dateTimeFlags = dateTimeFlags;
    }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet { get { return XmlSchemaWhiteSpace.Collapse; } }

    internal override RestrictionFlags ValidRestrictionFlags
    {
      get
      {
        return RestrictionFlags.Pattern |
               RestrictionFlags.Enumeration |
               RestrictionFlags.WhiteSpace |
               RestrictionFlags.MinExclusive |
               RestrictionFlags.MinInclusive |
               RestrictionFlags.MaxExclusive |
               RestrictionFlags.MaxInclusive;
      }
    }

    internal override int Compare(object value1, object value2)
    {
      DateTime dateTime1 = (DateTime)value1;
      DateTime dateTime2 = (DateTime)value2;
      if (dateTime1.Kind == DateTimeKind.Unspecified || dateTime2.Kind == DateTimeKind.Unspecified)
      { //If either of them are unspecified, do not convert zones
        return dateTime1.CompareTo(dateTime2);
      }
      dateTime1 = dateTime1.ToUniversalTime();
      return dateTime1.CompareTo(dateTime2.ToUniversalTime());
    }

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;
      typedValue = null;

      exception = dateTimeFacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      XsdDateTime dateTime;
      if (!XsdDateTime.TryParse(s, dateTimeFlags, out dateTime))
      {
        exception = new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, dateTimeFlags.ToString()));
        goto Error;
      }

      DateTime dateTimeValue = DateTime.MinValue;
      try
      {
        dateTimeValue = (DateTime)dateTime;
      }
      catch (ArgumentException e)
      {
        exception = e;
        goto Error;
      }

      exception = dateTimeFacetsChecker.CheckValueFacets(dateTimeValue, this);
      if (exception != null) goto Error;

      typedValue = dateTimeValue;

      return null;

    Error:
      return exception;
    }
  }
}
