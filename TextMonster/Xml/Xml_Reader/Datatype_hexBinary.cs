﻿using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_hexBinary : Datatype_anySimpleType
  {
    static readonly Type atomicValueType = typeof(byte[]);
    static readonly Type listValueType = typeof(byte[][]);

    internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
    {
      return XmlMiscConverter.Create(schemaType);
    }

    internal override FacetsChecker FacetsChecker { get { return binaryFacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.HexBinary; } }

    public override Type ValueType { get { return atomicValueType; } }

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
      return Compare((byte[])value1, (byte[])value2);
    }

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = binaryFacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      byte[] byteArrayValue = null;
      try
      {
        byteArrayValue = XmlConvert.FromBinHexString(s, false);
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

      exception = binaryFacetsChecker.CheckValueFacets(byteArrayValue, this);
      if (exception != null) goto Error;

      typedValue = byteArrayValue;

      return null;

    Error:
      return exception;
    }
  }
}