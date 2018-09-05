using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_string : Datatype_anySimpleType
  {
    internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
    {
      return XmlStringConverter.Create(schemaType);
    }

    internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet { get { return XmlSchemaWhiteSpace.Preserve; } }

    internal override FacetsChecker FacetsChecker { get { return stringFacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.String; } }

    public override XmlTokenizedType TokenizedType { get { return XmlTokenizedType.CDATA; } }

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

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = stringFacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      exception = stringFacetsChecker.CheckValueFacets(s, this);
      if (exception != null) goto Error;

      typedValue = s;
      return null;

    Error:
      return exception;
    }
  }
}
