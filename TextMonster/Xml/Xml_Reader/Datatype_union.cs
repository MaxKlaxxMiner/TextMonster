using System;

namespace TextMonster.Xml.Xml_Reader
{
  //Union datatype
  internal class Datatype_union : Datatype_anySimpleType
  {
    static readonly Type atomicValueType = typeof(object);
    static readonly Type listValueType = typeof(object[]);
    XmlSchemaSimpleType[] types;

    internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
    {
      return XmlUnionConverter.Create(schemaType);
    }

    internal Datatype_union(XmlSchemaSimpleType[] types)
    {
      this.types = types;
    }

    internal override int Compare(object value1, object value2)
    {
      XsdSimpleValue simpleValue1 = value1 as XsdSimpleValue;
      XsdSimpleValue simpleValue2 = value2 as XsdSimpleValue;

      if (simpleValue1 == null || simpleValue2 == null)
      {
        return -1;
      }
      XmlSchemaType schemaType1 = simpleValue1.XmlType;
      XmlSchemaType schemaType2 = simpleValue2.XmlType;

      if (schemaType1 == schemaType2)
      {
        XmlSchemaDatatype datatype = schemaType1.Datatype;
        return datatype.Compare(simpleValue1.TypedValue, simpleValue2.TypedValue);
      }
      return -1;
    }

    public override Type ValueType { get { return atomicValueType; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.AnyAtomicType; } }

    internal override FacetsChecker FacetsChecker { get { return unionFacetsChecker; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal bool HasAtomicMembers()
    {
      for (int i = 0; i < types.Length; ++i)
      {
        if (types[i].Datatype.Variety == XmlSchemaDatatypeVariety.List)
        {
          return false;
        }
      }
      return true;
    }

    internal bool IsUnionBaseOf(DatatypeImplementation derivedType)
    {
      for (int i = 0; i < types.Length; ++i)
      {
        if (derivedType.IsDerivedFrom(types[i].Datatype))
        {
          return true;
        }
      }
      return false;
    }

    internal override Exception TryParseValue(string s, NameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;
      XmlSchemaSimpleType memberType = null;

      typedValue = null;

      exception = unionFacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      //Parse string to CLR value
      for (int i = 0; i < types.Length; ++i)
      {
        exception = types[i].Datatype.TryParseValue(s, nameTable, nsmgr, out typedValue);
        if (exception == null)
        {
          memberType = types[i];
          break;
        }
      }
      if (memberType == null)
      {
        exception = new XmlSchemaException(Res.Sch_UnionFailedEx, s);
        goto Error;
      }

      typedValue = new XsdSimpleValue(memberType, typedValue);
      exception = unionFacetsChecker.CheckValueFacets(typedValue, this);
      if (exception != null) goto Error;

      return null;

    Error:
      return exception;
    }

    internal override Exception TryParseValue(object value, NameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;
      if (value == null)
      {
        throw new ArgumentNullException("value");
      }
      typedValue = null;
      string s = value as string;
      if (s != null)
      {
        return TryParseValue(s, nameTable, nsmgr, out typedValue);
      }

      object valueToCheck = null;
      XmlSchemaSimpleType memberType = null;
      for (int i = 0; i < types.Length; ++i)
      {
        if (types[i].Datatype.TryParseValue(value, nameTable, nsmgr, out valueToCheck) == null)
        { //no error
          memberType = types[i];
          break;
        }
      }
      if (valueToCheck == null)
      {
        exception = new XmlSchemaException(Res.Sch_UnionFailedEx, value.ToString());
        goto Error;
      }
      try
      {
        if (this.HasLexicalFacets)
        {
          string s1 = (string)this.ValueConverter.ChangeType(valueToCheck, typeof(System.String), nsmgr); //Using value here to avoid info loss
          exception = unionFacetsChecker.CheckLexicalFacets(ref s1, this);
          if (exception != null) goto Error;
        }
        typedValue = new XsdSimpleValue(memberType, valueToCheck);
        if (this.HasValueFacets)
        {
          exception = unionFacetsChecker.CheckValueFacets(typedValue, this);
          if (exception != null) goto Error;
        }
        return null;
      }
      catch (FormatException e)
      { //Catching for exceptions thrown by ValueConverter
        exception = e;
      }
      catch (InvalidCastException e)
      { //Catching for exceptions thrown by ValueConverter
        exception = e;
      }
      catch (OverflowException e)
      { //Catching for exceptions thrown by ValueConverter
        exception = e;
      }
      catch (ArgumentException e)
      { //Catching for exceptions thrown by ValueConverter
        exception = e;
      }

    Error:
      return exception;
    }
  }
}
