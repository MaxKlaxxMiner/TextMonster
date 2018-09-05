using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_QName : Datatype_anySimpleType
  {
    static readonly Type atomicValueType = typeof(XmlQualifiedName);
    static readonly Type listValueType = typeof(XmlQualifiedName[]);

    internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
    {
      return XmlMiscConverter.Create(schemaType);
    }

    internal override FacetsChecker FacetsChecker { get { return qnameFacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.QName; } }

    public override XmlTokenizedType TokenizedType { get { return XmlTokenizedType.QName; } }

    internal override RestrictionFlags ValidRestrictionFlags
    {
      get
      {
        return RestrictionFlags.Length |
               RestrictionFlags.MinLength |
               RestrictionFlags.MaxLength |
               RestrictionFlags.Pattern |
               RestrictionFlags.Enumeration |
               RestrictionFlags.WhiteSpace;
      }
    }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet { get { return XmlSchemaWhiteSpace.Collapse; } }

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      if (s == null || s.Length == 0)
      {
        return new XmlSchemaException(Res.Sch_EmptyAttributeValue, string.Empty);
      }

      exception = qnameFacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      XmlQualifiedName qname = null;
      try
      {
        string prefix;
        qname = XmlQualifiedName.Parse(s, nsmgr, out prefix);
      }
      catch (ArgumentException e)
      {
        exception = e;
        goto Error;
      }
      catch (XmlException e)
      {
        exception = e;
        goto Error;
      }

      exception = qnameFacetsChecker.CheckValueFacets(qname, this);
      if (exception != null) goto Error;

      typedValue = qname;

      return null;

    Error:
      return exception;
    }
  }
}
