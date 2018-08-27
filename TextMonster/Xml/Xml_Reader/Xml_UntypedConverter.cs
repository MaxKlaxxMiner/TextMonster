namespace TextMonster.Xml.Xml_Reader
{
  // ReSharper disable once InconsistentNaming
  internal class Xml_UntypedConverter : XmlListConverter
  {
    public static readonly XmlValueConverter Untyped = (XmlValueConverter)new Xml_UntypedConverter(new Xml_UntypedConverter(), false);
    public static readonly XmlValueConverter UntypedList = (XmlValueConverter)new Xml_UntypedConverter(new Xml_UntypedConverter(), true);
    private bool allowListToList;

    protected Xml_UntypedConverter()
      : base((XmlSchemaType)DatatypeImplementation.UntypedAtomicType)
    {
    }

    protected Xml_UntypedConverter(Xml_UntypedConverter atomicConverter, bool allowListToList)
      : base((XmlBaseConverter)atomicConverter, allowListToList ? XmlBaseConverter.StringArrayType : XmlBaseConverter.StringType)
    {
      this.allowListToList = allowListToList;
    }

    public override bool ToBoolean(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      return Xml_Convert.ToBoolean(value);
    }

    public override bool ToBoolean(object value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (value.GetType() == XmlBaseConverter.StringType)
        return Xml_Convert.ToBoolean((string)value);
      return (bool)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.BooleanType, (IXmlNamespaceResolver)null);
    }

    public override DateTime ToDateTime(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      return XmlBaseConverter.UntypedAtomicToDateTime(value);
    }

    public override DateTime ToDateTime(object value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (value.GetType() == XmlBaseConverter.StringType)
        return XmlBaseConverter.UntypedAtomicToDateTime((string)value);
      return (DateTime)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.DateTimeType, (IXmlNamespaceResolver)null);
    }

    public override DateTimeOffset ToDateTimeOffset(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      return XmlBaseConverter.UntypedAtomicToDateTimeOffset(value);
    }

    public override DateTimeOffset ToDateTimeOffset(object value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (value.GetType() == XmlBaseConverter.StringType)
        return XmlBaseConverter.UntypedAtomicToDateTimeOffset((string)value);
      return (DateTimeOffset)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.DateTimeOffsetType, (IXmlNamespaceResolver)null);
    }

    public override Decimal ToDecimal(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      return Xml_Convert.ToDecimal(value);
    }

    public override Decimal ToDecimal(object value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (value.GetType() == XmlBaseConverter.StringType)
        return Xml_Convert.ToDecimal((string)value);
      return (Decimal)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.DecimalType, (IXmlNamespaceResolver)null);
    }

    public override double ToDouble(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      return Xml_Convert.ToDouble(value);
    }

    public override double ToDouble(object value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (value.GetType() == XmlBaseConverter.StringType)
        return Xml_Convert.ToDouble((string)value);
      return (double)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.DoubleType, (IXmlNamespaceResolver)null);
    }

    public override int ToInt32(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      return Xml_Convert.ToInt32(value);
    }

    public override int ToInt32(object value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (value.GetType() == XmlBaseConverter.StringType)
        return Xml_Convert.ToInt32((string)value);
      return (int)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.Int32Type, (IXmlNamespaceResolver)null);
    }

    public override long ToInt64(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      return Xml_Convert.ToInt64(value);
    }

    public override long ToInt64(object value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (value.GetType() == XmlBaseConverter.StringType)
        return Xml_Convert.ToInt64((string)value);
      return (long)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.Int64Type, (IXmlNamespaceResolver)null);
    }

    public override float ToSingle(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      return Xml_Convert.ToSingle(value);
    }

    public override float ToSingle(object value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (value.GetType() == XmlBaseConverter.StringType)
        return Xml_Convert.ToSingle((string)value);
      return (float)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.SingleType, (IXmlNamespaceResolver)null);
    }

    public override string ToString(bool value)
    {
      return Xml_Convert.ToString(value);
    }

    public override string ToString(DateTime value)
    {
      return XmlBaseConverter.DateTimeToString(value);
    }

    public override string ToString(DateTimeOffset value)
    {
      return XmlBaseConverter.DateTimeOffsetToString(value);
    }

    public override string ToString(Decimal value)
    {
      return Xml_Convert.ToString(value);
    }

    public override string ToString(double value)
    {
      return Xml_Convert.ToString(value);
    }

    public override string ToString(int value)
    {
      return Xml_Convert.ToString(value);
    }

    public override string ToString(long value)
    {
      return Xml_Convert.ToString(value);
    }

    public override string ToString(float value)
    {
      return Xml_Convert.ToString(value);
    }

    public override string ToString(string value, IXmlNamespaceResolver nsResolver)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      return value;
    }

    public override string ToString(object value, IXmlNamespaceResolver nsResolver)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      Type type = value.GetType();
      if (type == XmlBaseConverter.BooleanType)
        return Xml_Convert.ToString((bool)value);
      if (type == XmlBaseConverter.ByteType)
        return Xml_Convert.ToString((byte)value);
      if (type == XmlBaseConverter.ByteArrayType)
        return XmlBaseConverter.Base64BinaryToString((byte[])value);
      if (type == XmlBaseConverter.DateTimeType)
        return XmlBaseConverter.DateTimeToString((DateTime)value);
      if (type == XmlBaseConverter.DateTimeOffsetType)
        return XmlBaseConverter.DateTimeOffsetToString((DateTimeOffset)value);
      if (type == XmlBaseConverter.DecimalType)
        return Xml_Convert.ToString((Decimal)value);
      if (type == XmlBaseConverter.DoubleType)
        return Xml_Convert.ToString((double)value);
      if (type == XmlBaseConverter.Int16Type)
        return Xml_Convert.ToString((short)value);
      if (type == XmlBaseConverter.Int32Type)
        return Xml_Convert.ToString((int)value);
      if (type == XmlBaseConverter.Int64Type)
        return Xml_Convert.ToString((long)value);
      if (type == XmlBaseConverter.SByteType)
        return Xml_Convert.ToString((sbyte)value);
      if (type == XmlBaseConverter.SingleType)
        return Xml_Convert.ToString((float)value);
      if (type == XmlBaseConverter.StringType)
        return (string)value;
      if (type == XmlBaseConverter.TimeSpanType)
        return XmlBaseConverter.DurationToString((TimeSpan)value);
      if (type == XmlBaseConverter.UInt16Type)
        return Xml_Convert.ToString((ushort)value);
      if (type == XmlBaseConverter.UInt32Type)
        return Xml_Convert.ToString((uint)value);
      if (type == XmlBaseConverter.UInt64Type)
        return Xml_Convert.ToString((ulong)value);
      if (XmlBaseConverter.IsDerivedFrom(type, XmlBaseConverter.UriType))
        return XmlBaseConverter.AnyUriToString((Uri)value);
      if (type == XmlBaseConverter.XmlAtomicValueType)
        return (string)((XPathItem)value).ValueAs(XmlBaseConverter.StringType, nsResolver);
      if (XmlBaseConverter.IsDerivedFrom(type, XmlBaseConverter.XmlQualifiedNameType))
        return XmlBaseConverter.QNameToString((XmlQualifiedName)value, nsResolver);
      return (string)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.StringType, nsResolver);
    }

    public override object ChangeType(bool value, Type destinationType)
    {
      if (destinationType == (Type)null)
        throw new ArgumentNullException("destinationType");
      if (destinationType == XmlBaseConverter.ObjectType)
        destinationType = this.DefaultClrType;
      if (destinationType == XmlBaseConverter.StringType)
        return (object)Xml_Convert.ToString(value);
      return this.ChangeTypeWildcardSource((object)(bool)(value ? 1 : 0), destinationType, (IXmlNamespaceResolver)null);
    }

    public override object ChangeType(DateTime value, Type destinationType)
    {
      if (destinationType == (Type)null)
        throw new ArgumentNullException("destinationType");
      if (destinationType == XmlBaseConverter.ObjectType)
        destinationType = this.DefaultClrType;
      if (destinationType == XmlBaseConverter.StringType)
        return (object)XmlBaseConverter.DateTimeToString(value);
      return this.ChangeTypeWildcardSource((object)value, destinationType, (IXmlNamespaceResolver)null);
    }

    public override object ChangeType(DateTimeOffset value, Type destinationType)
    {
      if (destinationType == (Type)null)
        throw new ArgumentNullException("destinationType");
      if (destinationType == XmlBaseConverter.ObjectType)
        destinationType = this.DefaultClrType;
      if (destinationType == XmlBaseConverter.StringType)
        return (object)XmlBaseConverter.DateTimeOffsetToString(value);
      return this.ChangeTypeWildcardSource((object)value, destinationType, (IXmlNamespaceResolver)null);
    }

    public override object ChangeType(Decimal value, Type destinationType)
    {
      if (destinationType == (Type)null)
        throw new ArgumentNullException("destinationType");
      if (destinationType == XmlBaseConverter.ObjectType)
        destinationType = this.DefaultClrType;
      if (destinationType == XmlBaseConverter.StringType)
        return (object)Xml_Convert.ToString(value);
      return this.ChangeTypeWildcardSource((object)value, destinationType, (IXmlNamespaceResolver)null);
    }

    public override object ChangeType(double value, Type destinationType)
    {
      if (destinationType == (Type)null)
        throw new ArgumentNullException("destinationType");
      if (destinationType == XmlBaseConverter.ObjectType)
        destinationType = this.DefaultClrType;
      if (destinationType == XmlBaseConverter.StringType)
        return (object)Xml_Convert.ToString(value);
      return this.ChangeTypeWildcardSource((object)value, destinationType, (IXmlNamespaceResolver)null);
    }

    public override object ChangeType(int value, Type destinationType)
    {
      if (destinationType == (Type)null)
        throw new ArgumentNullException("destinationType");
      if (destinationType == XmlBaseConverter.ObjectType)
        destinationType = this.DefaultClrType;
      if (destinationType == XmlBaseConverter.StringType)
        return (object)Xml_Convert.ToString(value);
      return this.ChangeTypeWildcardSource((object)value, destinationType, (IXmlNamespaceResolver)null);
    }

    public override object ChangeType(long value, Type destinationType)
    {
      if (destinationType == (Type)null)
        throw new ArgumentNullException("destinationType");
      if (destinationType == XmlBaseConverter.ObjectType)
        destinationType = this.DefaultClrType;
      if (destinationType == XmlBaseConverter.StringType)
        return (object)Xml_Convert.ToString(value);
      return this.ChangeTypeWildcardSource((object)value, destinationType, (IXmlNamespaceResolver)null);
    }

    public override object ChangeType(float value, Type destinationType)
    {
      if (destinationType == (Type)null)
        throw new ArgumentNullException("destinationType");
      if (destinationType == XmlBaseConverter.ObjectType)
        destinationType = this.DefaultClrType;
      if (destinationType == XmlBaseConverter.StringType)
        return (object)Xml_Convert.ToString(value);
      return this.ChangeTypeWildcardSource((object)value, destinationType, (IXmlNamespaceResolver)null);
    }

    public override object ChangeType(string value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (destinationType == (Type)null)
        throw new ArgumentNullException("destinationType");
      if (destinationType == XmlBaseConverter.ObjectType)
        destinationType = this.DefaultClrType;
      if (destinationType == XmlBaseConverter.BooleanType)
        return (object)(bool)(Xml_Convert.ToBoolean(value) ? 1 : 0);
      if (destinationType == XmlBaseConverter.ByteType)
        return (object)XmlBaseConverter.Int32ToByte(Xml_Convert.ToInt32(value));
      if (destinationType == XmlBaseConverter.ByteArrayType)
        return (object)XmlBaseConverter.StringToBase64Binary(value);
      if (destinationType == XmlBaseConverter.DateTimeType)
        return (object)XmlBaseConverter.UntypedAtomicToDateTime(value);
      if (destinationType == XmlBaseConverter.DateTimeOffsetType)
        return (object)XmlBaseConverter.UntypedAtomicToDateTimeOffset(value);
      if (destinationType == XmlBaseConverter.DecimalType)
        return (object)Xml_Convert.ToDecimal(value);
      if (destinationType == XmlBaseConverter.DoubleType)
        return (object)Xml_Convert.ToDouble(value);
      if (destinationType == XmlBaseConverter.Int16Type)
        return (object)XmlBaseConverter.Int32ToInt16(Xml_Convert.ToInt32(value));
      if (destinationType == XmlBaseConverter.Int32Type)
        return (object)Xml_Convert.ToInt32(value);
      if (destinationType == XmlBaseConverter.Int64Type)
        return (object)Xml_Convert.ToInt64(value);
      if (destinationType == XmlBaseConverter.SByteType)
        return (object)XmlBaseConverter.Int32ToSByte(Xml_Convert.ToInt32(value));
      if (destinationType == XmlBaseConverter.SingleType)
        return (object)Xml_Convert.ToSingle(value);
      if (destinationType == XmlBaseConverter.TimeSpanType)
        return (object)XmlBaseConverter.StringToDuration(value);
      if (destinationType == XmlBaseConverter.UInt16Type)
        return (object)XmlBaseConverter.Int32ToUInt16(Xml_Convert.ToInt32(value));
      if (destinationType == XmlBaseConverter.UInt32Type)
        return (object)XmlBaseConverter.Int64ToUInt32(Xml_Convert.ToInt64(value));
      if (destinationType == XmlBaseConverter.UInt64Type)
        return (object)XmlBaseConverter.DecimalToUInt64(Xml_Convert.ToDecimal(value));
      if (destinationType == XmlBaseConverter.UriType)
        return (object)Xml_Convert.ToUri(value);
      if (destinationType == XmlBaseConverter.XmlAtomicValueType)
        return (object)new XmlAtomicValue(this.SchemaType, value);
      if (destinationType == XmlBaseConverter.XmlQualifiedNameType)
        return (object)XmlBaseConverter.StringToQName(value, nsResolver);
      if (destinationType == XmlBaseConverter.XPathItemType)
        return (object)new XmlAtomicValue(this.SchemaType, value);
      if (destinationType == XmlBaseConverter.StringType)
        return (object)value;
      return this.ChangeTypeWildcardSource((object)value, destinationType, nsResolver);
    }

    public override object ChangeType(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (destinationType == (Type)null)
        throw new ArgumentNullException("destinationType");
      Type type = value.GetType();
      if (destinationType == XmlBaseConverter.ObjectType)
        destinationType = this.DefaultClrType;
      if (destinationType == XmlBaseConverter.BooleanType && type == XmlBaseConverter.StringType)
        return (object)(bool)(Xml_Convert.ToBoolean((string)value) ? 1 : 0);
      if (destinationType == XmlBaseConverter.ByteType && type == XmlBaseConverter.StringType)
        return (object)XmlBaseConverter.Int32ToByte(Xml_Convert.ToInt32((string)value));
      if (destinationType == XmlBaseConverter.ByteArrayType && type == XmlBaseConverter.StringType)
        return (object)XmlBaseConverter.StringToBase64Binary((string)value);
      if (destinationType == XmlBaseConverter.DateTimeType && type == XmlBaseConverter.StringType)
        return (object)XmlBaseConverter.UntypedAtomicToDateTime((string)value);
      if (destinationType == XmlBaseConverter.DateTimeOffsetType && type == XmlBaseConverter.StringType)
        return (object)XmlBaseConverter.UntypedAtomicToDateTimeOffset((string)value);
      if (destinationType == XmlBaseConverter.DecimalType && type == XmlBaseConverter.StringType)
        return (object)Xml_Convert.ToDecimal((string)value);
      if (destinationType == XmlBaseConverter.DoubleType && type == XmlBaseConverter.StringType)
        return (object)Xml_Convert.ToDouble((string)value);
      if (destinationType == XmlBaseConverter.Int16Type && type == XmlBaseConverter.StringType)
        return (object)XmlBaseConverter.Int32ToInt16(Xml_Convert.ToInt32((string)value));
      if (destinationType == XmlBaseConverter.Int32Type && type == XmlBaseConverter.StringType)
        return (object)Xml_Convert.ToInt32((string)value);
      if (destinationType == XmlBaseConverter.Int64Type && type == XmlBaseConverter.StringType)
        return (object)Xml_Convert.ToInt64((string)value);
      if (destinationType == XmlBaseConverter.SByteType && type == XmlBaseConverter.StringType)
        return (object)XmlBaseConverter.Int32ToSByte(Xml_Convert.ToInt32((string)value));
      if (destinationType == XmlBaseConverter.SingleType && type == XmlBaseConverter.StringType)
        return (object)Xml_Convert.ToSingle((string)value);
      if (destinationType == XmlBaseConverter.TimeSpanType && type == XmlBaseConverter.StringType)
        return (object)XmlBaseConverter.StringToDuration((string)value);
      if (destinationType == XmlBaseConverter.UInt16Type && type == XmlBaseConverter.StringType)
        return (object)XmlBaseConverter.Int32ToUInt16(Xml_Convert.ToInt32((string)value));
      if (destinationType == XmlBaseConverter.UInt32Type && type == XmlBaseConverter.StringType)
        return (object)XmlBaseConverter.Int64ToUInt32(Xml_Convert.ToInt64((string)value));
      if (destinationType == XmlBaseConverter.UInt64Type && type == XmlBaseConverter.StringType)
        return (object)XmlBaseConverter.DecimalToUInt64(Xml_Convert.ToDecimal((string)value));
      if (destinationType == XmlBaseConverter.UriType && type == XmlBaseConverter.StringType)
        return (object)Xml_Convert.ToUri((string)value);
      if (destinationType == XmlBaseConverter.XmlAtomicValueType)
      {
        if (type == XmlBaseConverter.StringType)
          return (object)new XmlAtomicValue(this.SchemaType, (string)value);
        if (type == XmlBaseConverter.XmlAtomicValueType)
          return (object)(XmlAtomicValue)value;
      }
      if (destinationType == XmlBaseConverter.XmlQualifiedNameType && type == XmlBaseConverter.StringType)
        return (object)XmlBaseConverter.StringToQName((string)value, nsResolver);
      if (destinationType == XmlBaseConverter.XPathItemType)
      {
        if (type == XmlBaseConverter.StringType)
          return (object)new XmlAtomicValue(this.SchemaType, (string)value);
        if (type == XmlBaseConverter.XmlAtomicValueType)
          return (object)(XmlAtomicValue)value;
      }
      if (destinationType == XmlBaseConverter.StringType)
        return (object)this.ToString(value, nsResolver);
      if (destinationType == XmlBaseConverter.XmlAtomicValueType)
        return (object)new XmlAtomicValue(this.SchemaType, this.ToString(value, nsResolver));
      if (destinationType == XmlBaseConverter.XPathItemType)
        return (object)new XmlAtomicValue(this.SchemaType, this.ToString(value, nsResolver));
      if (type == XmlBaseConverter.XmlAtomicValueType)
        return ((XPathItem)value).ValueAs(destinationType, nsResolver);
      return this.ChangeListType(value, destinationType, nsResolver);
    }

    private object ChangeTypeWildcardDestination(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      if (value.GetType() == XmlBaseConverter.XmlAtomicValueType)
        return ((XPathItem)value).ValueAs(destinationType, nsResolver);
      return this.ChangeListType(value, destinationType, nsResolver);
    }

    private object ChangeTypeWildcardSource(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      if (destinationType == XmlBaseConverter.XmlAtomicValueType)
        return (object)new XmlAtomicValue(this.SchemaType, this.ToString(value, nsResolver));
      if (destinationType == XmlBaseConverter.XPathItemType)
        return (object)new XmlAtomicValue(this.SchemaType, this.ToString(value, nsResolver));
      return this.ChangeListType(value, destinationType, nsResolver);
    }

    protected override object ChangeListType(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      Type type = value.GetType();
      if (this.atomicConverter != null && (this.allowListToList || !(type != XmlBaseConverter.StringType) || !(destinationType != XmlBaseConverter.StringType)))
        return base.ChangeListType(value, destinationType, nsResolver);
      if (this.SupportsType(type))
        throw new InvalidCastException(Res.GetString("XmlConvert_TypeToString", (object)this.XmlTypeName, (object)type.Name));
      if (this.SupportsType(destinationType))
        throw new InvalidCastException(Res.GetString("XmlConvert_TypeFromString", (object)this.XmlTypeName, (object)destinationType.Name));
      throw base.CreateInvalidClrMappingException(type, destinationType);
    }

    private bool SupportsType(Type clrType)
    {
      return clrType == XmlBaseConverter.BooleanType || clrType == XmlBaseConverter.ByteType || (clrType == XmlBaseConverter.ByteArrayType || clrType == XmlBaseConverter.DateTimeType) || (clrType == XmlBaseConverter.DateTimeOffsetType || clrType == XmlBaseConverter.DecimalType || (clrType == XmlBaseConverter.DoubleType || clrType == XmlBaseConverter.Int16Type)) || (clrType == XmlBaseConverter.Int32Type || clrType == XmlBaseConverter.Int64Type || (clrType == XmlBaseConverter.SByteType || clrType == XmlBaseConverter.SingleType) || (clrType == XmlBaseConverter.TimeSpanType || clrType == XmlBaseConverter.UInt16Type || (clrType == XmlBaseConverter.UInt32Type || clrType == XmlBaseConverter.UInt64Type))) || (clrType == XmlBaseConverter.UriType || clrType == XmlBaseConverter.XmlQualifiedNameType);
    }
  }
}
