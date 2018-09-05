using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_anyURI : Datatype_anySimpleType
  {
    static readonly Type atomicValueType = typeof(Uri);
    static readonly Type listValueType = typeof(Uri[]);

    internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
    {
      return XmlMiscConverter.Create(schemaType);
    }

    internal override FacetsChecker FacetsChecker { get { return stringFacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.AnyUri; } }

    public override Type ValueType { get { return atomicValueType; } }

    internal override bool HasValueFacets
    {
      get
      {
        return true; //Built-in facet to check validity of Uri
      }
    }
    internal override Type ListValueType { get { return listValueType; } }

    internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet { get { return XmlSchemaWhiteSpace.Collapse; } }

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

    internal override int Compare(object value1, object value2)
    {
      return ((Uri)value1).Equals((Uri)value2) ? 0 : -1;
    }

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = stringFacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      Uri uri;
      exception = XmlConvert.TryToUri(s, out uri);
      if (exception != null) goto Error;

      string stringValue = uri.OriginalString;
      exception = ((StringFacetsChecker)stringFacetsChecker).CheckValueFacets(stringValue, this, false);
      if (exception != null) goto Error;

      typedValue = uri;

      return null;

    Error:
      return exception;
    }
  }
}
