using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_yearMonthDuration : Datatype_duration
  {

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

      XsdDuration duration;
      exception = XsdDuration.TryParse(s, XsdDuration.DurationType.YearMonthDuration, out duration);
      if (exception != null) goto Error;

      TimeSpan timeSpanValue;

      exception = duration.TryToTimeSpan(XsdDuration.DurationType.YearMonthDuration, out timeSpanValue);
      if (exception != null) goto Error;

      exception = durationFacetsChecker.CheckValueFacets(timeSpanValue, this);
      if (exception != null) goto Error;

      typedValue = timeSpanValue;

      return null;

    Error:
      return exception;
    }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.YearMonthDuration; } }

  }
}
