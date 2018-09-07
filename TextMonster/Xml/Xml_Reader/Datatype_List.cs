using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  //List type
  internal class Datatype_List : Datatype_anySimpleType
  {
    DatatypeImplementation itemType;
    int minListSize;

    internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
    {
      XmlSchemaType listItemType = null;
      XmlSchemaSimpleType simpleType;
      XmlSchemaComplexType complexType;
      complexType = schemaType as XmlSchemaComplexType;

      if (complexType != null)
      {
        do
        {
          simpleType = complexType.BaseXmlSchemaType as XmlSchemaSimpleType;
          if (simpleType != null)
          {
            break;
          }
          complexType = complexType.BaseXmlSchemaType as XmlSchemaComplexType;
        } while (complexType != null && complexType != XmlSchemaComplexType.AnyType);
      }
      else
      {
        simpleType = schemaType as XmlSchemaSimpleType;
      }
      if (simpleType != null)
      {
        do
        {
          XmlSchemaSimpleTypeList listType = simpleType.Content as XmlSchemaSimpleTypeList;
          if (listType != null)
          {
            listItemType = listType.BaseItemType;
            break;
          }
          simpleType = simpleType.BaseXmlSchemaType as XmlSchemaSimpleType;
        } while (simpleType != null && simpleType != DatatypeImplementation.AnySimpleType);
      }

      if (listItemType == null)
      { //Get built-in simple type for the typecode
        listItemType = DatatypeImplementation.GetSimpleTypeFromTypeCode(schemaType.Datatype.TypeCode);
      }

      return XmlListConverter.Create(listItemType.ValueConverter);
    }

    internal Datatype_List(DatatypeImplementation type, int minListSize)
    {
      this.itemType = type;
      this.minListSize = minListSize;
    }

    internal override int Compare(object value1, object value2)
    {
      System.Array arr1 = (System.Array)value1;
      System.Array arr2 = (System.Array)value2;

      int length = arr1.Length;
      if (length != arr2.Length)
      {
        return -1;
      }
      XmlAtomicValue[] atomicValues1 = arr1 as XmlAtomicValue[];
      if (atomicValues1 != null)
      {
        XmlAtomicValue[] atomicValues2 = arr2 as XmlAtomicValue[];
        XmlSchemaType xmlType1;
        for (int i = 0; i < atomicValues1.Length; i++)
        {
          xmlType1 = atomicValues1[i].XmlType;
          if (xmlType1 != atomicValues2[i].XmlType || !xmlType1.Datatype.IsEqual(atomicValues1[i].TypedValue, atomicValues2[i].TypedValue))
          {
            return -1;
          }
        }
        return 0;
      }
      else
      {
        for (int i = 0; i < arr1.Length; i++)
        {
          if (itemType.Compare(arr1.GetValue(i), arr2.GetValue(i)) != 0)
          {
            return -1;
          }
        }
        return 0;
      }
    }

    public override Type ValueType { get { return ListValueType; } }

    public override XmlTokenizedType TokenizedType { get { return itemType.TokenizedType; } }

    internal override Type ListValueType { get { return itemType.ListValueType; } }

    internal override FacetsChecker FacetsChecker { get { return listFacetsChecker; } }

    public override XmlTypeCode TypeCode
    {
      get
      {
        return itemType.TypeCode;
      }
    }

    internal DatatypeImplementation ItemType { get { return itemType; } }

    internal override Exception TryParseValue(object value, XmlNameTable nameTable, IXmlNamespaceResolver namespaceResolver, out object typedValue)
    {
      Exception exception;
      if (value == null)
      {
        throw new ArgumentNullException("value");
      }
      string s = value as string;
      typedValue = null;
      if (s != null)
      {
        return TryParseValue(s, nameTable, namespaceResolver, out typedValue);
      }

      try
      {
        object valueToCheck = this.ValueConverter.ChangeType(value, this.ValueType, namespaceResolver);
        Array valuesToCheck = valueToCheck as Array;

        bool checkItemLexical = itemType.HasLexicalFacets;
        bool checkItemValue = itemType.HasValueFacets;
        object item;
        FacetsChecker itemFacetsChecker = itemType.FacetsChecker;
        XmlValueConverter itemValueConverter = itemType.ValueConverter;

        for (int i = 0; i < valuesToCheck.Length; i++)
        {
          item = valuesToCheck.GetValue(i);
          if (checkItemLexical)
          {
            string s1 = (string)itemValueConverter.ChangeType(item, typeof(System.String), namespaceResolver);
            exception = itemFacetsChecker.CheckLexicalFacets(ref s1, itemType);
            if (exception != null) goto Error;
          }
          if (checkItemValue)
          {
            exception = itemFacetsChecker.CheckValueFacets(item, itemType);
            if (exception != null) goto Error;
          }
        }

        //Check facets on the list itself
        if (this.HasLexicalFacets)
        {
          string s1 = (string)this.ValueConverter.ChangeType(valueToCheck, typeof(System.String), namespaceResolver);
          exception = listFacetsChecker.CheckLexicalFacets(ref s1, this);
          if (exception != null) goto Error;
        }
        if (this.HasValueFacets)
        {
          exception = listFacetsChecker.CheckValueFacets(valueToCheck, this);
          if (exception != null) goto Error;
        }
        typedValue = valueToCheck;
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

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = listFacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      ArrayList values = new ArrayList();
      object array;
      if (itemType.Variety == XmlSchemaDatatypeVariety.Union)
      {
        object unionTypedValue;
        string[] splitString = XmlConvert.SplitString(s);
        for (int i = 0; i < splitString.Length; ++i)
        {
          //Parse items in list according to the itemType
          exception = itemType.TryParseValue(splitString[i], nameTable, nsmgr, out unionTypedValue);
          if (exception != null) goto Error;

          XsdSimpleValue simpleValue = (XsdSimpleValue)unionTypedValue;
          values.Add(new XmlAtomicValue(simpleValue.XmlType, simpleValue.TypedValue, nsmgr));
        }
        array = values.ToArray(typeof(XmlAtomicValue));
      }
      else
      { //Variety == List or Atomic
        string[] splitString = XmlConvert.SplitString(s);
        for (int i = 0; i < splitString.Length; ++i)
        {
          exception = itemType.TryParseValue(splitString[i], nameTable, nsmgr, out typedValue);
          if (exception != null) goto Error;

          values.Add(typedValue);
        }
        array = values.ToArray(itemType.ValueType);
      }
      if (values.Count < minListSize)
      {
        return new XmlSchemaException(Res.Sch_EmptyAttributeValue, string.Empty);
      }

      exception = listFacetsChecker.CheckValueFacets(array, this);
      if (exception != null) goto Error;

      typedValue = array;

      return null;

    Error:
      return exception;
    }

  }
}
