using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  internal class XmlListConverter : XmlBaseConverter
  {
    protected XmlValueConverter atomicConverter;

    protected XmlListConverter(XmlBaseConverter atomicConverter)
      : base(atomicConverter)
    {
      this.atomicConverter = atomicConverter;
    }

    protected XmlListConverter(XmlBaseConverter atomicConverter, Type clrTypeDefault)
      : base(atomicConverter, clrTypeDefault)
    {
      this.atomicConverter = atomicConverter;
    }

    protected XmlListConverter(XmlSchemaType schemaType)
      : base(schemaType)
    {
    }

    public static XmlValueConverter Create(XmlValueConverter atomicConverter)
    {
      if (atomicConverter == XmlUntypedConverter.Untyped)
        return XmlUntypedConverter.UntypedList;

      if (atomicConverter == XmlAnyConverter.Item)
        return XmlAnyListConverter.ItemList;

      if (atomicConverter == XmlAnyConverter.AnyAtomic)
        return XmlAnyListConverter.AnyAtomicList;

      return new XmlListConverter((XmlBaseConverter)atomicConverter);
    }


    //-----------------------------------------------
    // ChangeType
    //-----------------------------------------------

    public override object ChangeType(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      if (value == null) throw new ArgumentNullException("value");
      if (destinationType == null) throw new ArgumentNullException("destinationType");

      return ChangeListType(value, destinationType, nsResolver);
    }


    //------------------------------------------------------------------------
    // Helpers
    //------------------------------------------------------------------------

    protected override object ChangeListType(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      Type sourceType = value.GetType();

      if (destinationType == ObjectType) destinationType = DefaultClrType;

      // Input value must support IEnumerable and destination type should be IEnumerable, ICollection, IList, Type[], or String
      if (!(value is IEnumerable) || !IsListType(destinationType))
        throw CreateInvalidClrMappingException(sourceType, destinationType);

      // Handle case where destination type is a string
      if (destinationType == StringType)
      {
        // Conversion from string to string is a no-op
        if (sourceType == StringType)
          return value;

        // Convert from list to string
        return ListAsString((IEnumerable)value, nsResolver);
      }

      // Handle case where source type is a string
      // Tokenize string and create a list out of resulting string tokens
      if (sourceType == StringType)
        value = StringAsList((string)value);

      if (destinationType.IsArray)
      {
        // Convert from source value to strongly-typed array; special-case each possible item type for performance
        Type itemTypeDst = destinationType.GetElementType();

        // Converting from object[] to object[] is not necessarily a no-op (i.e. xs:int* stored as an object[]
        // containing String values will need to be converted to an object[] containing Int32 values).
        if (itemTypeDst == ObjectType) return ToArray<object>(value, nsResolver);

        // For all types except object[], sourceType = destinationType is a no-op conversion
        if (sourceType == destinationType) return value;

        // Otherwise, iterate over values in source list, convert them to output item type, and store them in result array
        if (itemTypeDst == BooleanType) return ToArray<bool>(value, nsResolver);
        if (itemTypeDst == ByteType) return ToArray<byte>(value, nsResolver);
        if (itemTypeDst == ByteArrayType) return ToArray<byte[]>(value, nsResolver);
        if (itemTypeDst == DateTimeType) return ToArray<DateTime>(value, nsResolver);
        if (itemTypeDst == DateTimeOffsetType) return ToArray<DateTimeOffset>(value, nsResolver);
        if (itemTypeDst == DecimalType) return ToArray<decimal>(value, nsResolver);
        if (itemTypeDst == DoubleType) return ToArray<double>(value, nsResolver);
        if (itemTypeDst == Int16Type) return ToArray<short>(value, nsResolver);
        if (itemTypeDst == Int32Type) return ToArray<int>(value, nsResolver);
        if (itemTypeDst == Int64Type) return ToArray<long>(value, nsResolver);
        if (itemTypeDst == SByteType) return ToArray<sbyte>(value, nsResolver);
        if (itemTypeDst == SingleType) return ToArray<float>(value, nsResolver);
        if (itemTypeDst == StringType) return ToArray<string>(value, nsResolver);
        if (itemTypeDst == TimeSpanType) return ToArray<TimeSpan>(value, nsResolver);
        if (itemTypeDst == UInt16Type) return ToArray<ushort>(value, nsResolver);
        if (itemTypeDst == UInt32Type) return ToArray<uint>(value, nsResolver);
        if (itemTypeDst == UInt64Type) return ToArray<ulong>(value, nsResolver);
        if (itemTypeDst == UriType) return ToArray<Uri>(value, nsResolver);
        if (itemTypeDst == XmlAtomicValueType) return ToArray<XmlAtomicValue>(value, nsResolver);
        if (itemTypeDst == XmlQualifiedNameType) return ToArray<XmlQualifiedName>(value, nsResolver);
        if (itemTypeDst == XPathItemType) return ToArray<XPathItem>(value, nsResolver);
        if (itemTypeDst == XPathNavigatorType) return ToArray<XPathNavigator>(value, nsResolver);

        throw CreateInvalidClrMappingException(sourceType, destinationType);
      }

      // Destination type is IList, ICollection or IEnumerable
      // If source value is an array of values having the default representation, then conversion is a no-op
      if (sourceType == DefaultClrType && sourceType != ObjectArrayType)
        return value;

      return ToList(value, nsResolver);
    }

    /// <summary>
    /// Return true if "type" is one of the following:
    ///   1. IList, ICollection, IEnumerable
    ///   2. A strongly-typed array
    ///   3. A string
    /// </summary>
    private bool IsListType(Type type)
    {
      // IsClrListType returns true if "type" is one of the list interfaces
      if (type == IListType || type == ICollectionType || type == IEnumerableType || type == StringType)
        return true;

      return type.IsArray;
    }

    /// <summary>
    /// Convert "list" to an array of type T by iterating over each item in "list" and converting it to type "T"
    /// by invoking the atomic converter.
    /// </summary>
    private T[] ToArray<T>(object list, IXmlNamespaceResolver nsResolver)
    {
      // IList --> Array<T>
      IList listSrc = list as IList;
      if (listSrc != null)
      {
        T[] arrDst = new T[listSrc.Count];

        for (int i = 0; i < listSrc.Count; i++)
          arrDst[i] = (T)this.atomicConverter.ChangeType(listSrc[i], typeof(T), nsResolver);

        return arrDst;
      }

      // IEnumerable --> Array<T>
      IEnumerable enumSrc = list as IEnumerable;

      List<T> listDst = new List<T>();
      foreach (object value in enumSrc)
        listDst.Add((T)this.atomicConverter.ChangeType(value, typeof(T), nsResolver));

      return listDst.ToArray();
    }

    /// <summary>
    /// Convert "list" to an IList containing items in the atomic type's default representation.
    /// </summary>
    private IList ToList(object list, IXmlNamespaceResolver nsResolver)
    {
      // IList --> object[]
      IList listSrc = list as IList;
      if (listSrc != null)
      {
        object[] arrDst = new object[listSrc.Count];

        for (int i = 0; i < listSrc.Count; i++)
          arrDst[i] = this.atomicConverter.ChangeType(listSrc[i], ObjectType, nsResolver);

        return arrDst;
      }

      // IEnumerable --> List<object>
      IEnumerable enumSrc = list as IEnumerable;

      List<object> listDst = new List<object>();
      foreach (object value in enumSrc)
        listDst.Add(this.atomicConverter.ChangeType(value, ObjectType, nsResolver));

      return listDst;
    }

    /// <summary>
    /// Tokenize "value" by splitting it on whitespace characters.  Insert tokens into an ArrayList and return the list.
    /// </summary>
    private List<string> StringAsList(string value)
    {
      return new List<string>(XmlConvert.SplitString(value));
    }

    /// <summary>
    /// Convert a list to a corresponding list of strings.  Then concatenate the strings, which adjacent values delimited
    /// by a space character.
    /// </summary>
    private string ListAsString(IEnumerable list, IXmlNamespaceResolver nsResolver)
    {
      StringBuilder bldr = new StringBuilder();

      foreach (object value in list)
      {
        // skip null values
        if (value != null)
        {
          // Separate values by single space character
          if (bldr.Length != 0)
            bldr.Append(' ');

          // Append string value of next item in the list
          bldr.Append(this.atomicConverter.ToString(value, nsResolver));
        }
      }

      return bldr.ToString();
    }

    /// <summary>
    /// Create an InvalidCastException for cases where either "destinationType" or "sourceType" is not a supported CLR representation
    /// for this Xml type.
    /// </summary>
    private new Exception CreateInvalidClrMappingException(Type sourceType, Type destinationType)
    {
      if (sourceType == destinationType)
        return new InvalidCastException(Res.GetString(Res.XmlConvert_TypeListBadMapping, XmlTypeName, sourceType.Name));

      return new InvalidCastException(Res.GetString(Res.XmlConvert_TypeListBadMapping2, XmlTypeName, sourceType.Name, destinationType.Name));
    }
  }
}
