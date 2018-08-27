namespace TextMonster.Xml.Xml_Reader
{
  // ReSharper disable once InconsistentNaming
  internal class Xml_ListConverter : XmlBaseConverter
  {
    protected XmlValueConverter atomicConverter;

    protected XmlListConverter(XmlBaseConverter atomicConverter)
      : base(atomicConverter)
    {
      this.atomicConverter = (XmlValueConverter)atomicConverter;
    }

    protected XmlListConverter(XmlBaseConverter atomicConverter, Type clrTypeDefault)
      : base(atomicConverter, clrTypeDefault)
    {
      this.atomicConverter = (XmlValueConverter)atomicConverter;
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
      return (XmlValueConverter)new XmlListConverter((XmlBaseConverter)atomicConverter);
    }

    public override object ChangeType(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (destinationType == (Type)null)
        throw new ArgumentNullException("destinationType");
      return this.ChangeListType(value, destinationType, nsResolver);
    }

    protected override object ChangeListType(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      Type type = value.GetType();
      if (destinationType == XmlBaseConverter.ObjectType)
        destinationType = this.DefaultClrType;
      if (!(value is IEnumerable) || !this.IsListType(destinationType))
        throw this.CreateInvalidClrMappingException(type, destinationType);
      if (destinationType == XmlBaseConverter.StringType)
      {
        if (type == XmlBaseConverter.StringType)
          return value;
        return (object)this.ListAsString((IEnumerable)value, nsResolver);
      }
      if (type == XmlBaseConverter.StringType)
        value = (object)this.StringAsList((string)value);
      if (destinationType.IsArray)
      {
        Type elementType = destinationType.GetElementType();
        if (elementType == XmlBaseConverter.ObjectType)
          return (object)this.ToArray<object>(value, nsResolver);
        if (type == destinationType)
          return value;
        if (elementType == XmlBaseConverter.BooleanType)
          return (object)this.ToArray<bool>(value, nsResolver);
        if (elementType == XmlBaseConverter.ByteType)
          return (object)this.ToArray<byte>(value, nsResolver);
        if (elementType == XmlBaseConverter.ByteArrayType)
          return (object)this.ToArray<byte[]>(value, nsResolver);
        if (elementType == XmlBaseConverter.DateTimeType)
          return (object)this.ToArray<DateTime>(value, nsResolver);
        if (elementType == XmlBaseConverter.DateTimeOffsetType)
          return (object)this.ToArray<DateTimeOffset>(value, nsResolver);
        if (elementType == XmlBaseConverter.DecimalType)
          return (object)this.ToArray<Decimal>(value, nsResolver);
        if (elementType == XmlBaseConverter.DoubleType)
          return (object)this.ToArray<double>(value, nsResolver);
        if (elementType == XmlBaseConverter.Int16Type)
          return (object)this.ToArray<short>(value, nsResolver);
        if (elementType == XmlBaseConverter.Int32Type)
          return (object)this.ToArray<int>(value, nsResolver);
        if (elementType == XmlBaseConverter.Int64Type)
          return (object)this.ToArray<long>(value, nsResolver);
        if (elementType == XmlBaseConverter.SByteType)
          return (object)this.ToArray<sbyte>(value, nsResolver);
        if (elementType == XmlBaseConverter.SingleType)
          return (object)this.ToArray<float>(value, nsResolver);
        if (elementType == XmlBaseConverter.StringType)
          return (object)this.ToArray<string>(value, nsResolver);
        if (elementType == XmlBaseConverter.TimeSpanType)
          return (object)this.ToArray<TimeSpan>(value, nsResolver);
        if (elementType == XmlBaseConverter.UInt16Type)
          return (object)this.ToArray<ushort>(value, nsResolver);
        if (elementType == XmlBaseConverter.UInt32Type)
          return (object)this.ToArray<uint>(value, nsResolver);
        if (elementType == XmlBaseConverter.UInt64Type)
          return (object)this.ToArray<ulong>(value, nsResolver);
        if (elementType == XmlBaseConverter.UriType)
          return (object)this.ToArray<Uri>(value, nsResolver);
        if (elementType == XmlBaseConverter.XmlAtomicValueType)
          return (object)this.ToArray<XmlAtomicValue>(value, nsResolver);
        if (elementType == XmlBaseConverter.XmlQualifiedNameType)
          return (object)this.ToArray<XmlQualifiedName>(value, nsResolver);
        if (elementType == XmlBaseConverter.XPathItemType)
          return (object)this.ToArray<XPathItem>(value, nsResolver);
        if (elementType == XmlBaseConverter.XPathNavigatorType)
          return (object)this.ToArray<XPathNavigator>(value, nsResolver);
        throw this.CreateInvalidClrMappingException(type, destinationType);
      }
      if (type == this.DefaultClrType && type != XmlBaseConverter.ObjectArrayType)
        return value;
      return (object)this.ToList(value, nsResolver);
    }

    private bool IsListType(Type type)
    {
      if (type == XmlBaseConverter.IListType || type == XmlBaseConverter.ICollectionType || (type == XmlBaseConverter.IEnumerableType || type == XmlBaseConverter.StringType))
        return true;
      return type.IsArray;
    }

    private T[] ToArray<T>(object list, IXmlNamespaceResolver nsResolver)
    {
      IList list1 = list as IList;
      if (list1 != null)
      {
        T[] objArray = new T[list1.Count];
        for (int index = 0; index < list1.Count; ++index)
          objArray[index] = (T)this.atomicConverter.ChangeType(list1[index], typeof(T), nsResolver);
        return objArray;
      }
      IEnumerable enumerable = list as IEnumerable;
      List<T> list2 = new List<T>();
      foreach (object obj in enumerable)
        list2.Add((T)this.atomicConverter.ChangeType(obj, typeof(T), nsResolver));
      return list2.ToArray();
    }

    private IList ToList(object list, IXmlNamespaceResolver nsResolver)
    {
      IList list1 = list as IList;
      if (list1 != null)
      {
        object[] objArray = new object[list1.Count];
        for (int index = 0; index < list1.Count; ++index)
          objArray[index] = this.atomicConverter.ChangeType(list1[index], XmlBaseConverter.ObjectType, nsResolver);
        return (IList)objArray;
      }
      IEnumerable enumerable = list as IEnumerable;
      List<object> list2 = new List<object>();
      foreach (object obj in enumerable)
        list2.Add(this.atomicConverter.ChangeType(obj, XmlBaseConverter.ObjectType, nsResolver));
      return (IList)list2;
    }

    private List<string> StringAsList(string value)
    {
      return new List<string>((IEnumerable<string>)Xml_Convert.SplitString(value));
    }

    private string ListAsString(IEnumerable list, IXmlNamespaceResolver nsResolver)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (object obj in list)
      {
        if (obj != null)
        {
          if (stringBuilder.Length != 0)
            stringBuilder.Append(' ');
          stringBuilder.Append(this.atomicConverter.ToString(obj, nsResolver));
        }
      }
      return stringBuilder.ToString();
    }

    private new Exception CreateInvalidClrMappingException(Type sourceType, Type destinationType)
    {
      if (sourceType == destinationType)
        return (Exception)new InvalidCastException(Res.GetString("XmlConvert_TypeListBadMapping", (object)this.XmlTypeName, (object)sourceType.Name));
      return (Exception)new InvalidCastException(Res.GetString("XmlConvert_TypeListBadMapping2", (object)this.XmlTypeName, (object)sourceType.Name, (object)destinationType.Name));
    }
  }
}
