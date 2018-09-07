﻿using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class XmlAnyConverter : XmlBaseConverter
  {
    protected XmlAnyConverter(XmlTypeCode typeCode)
      : base(typeCode)
    {
    }

    public static readonly XmlValueConverter Item = new XmlAnyConverter(XmlTypeCode.Item);
    public static readonly XmlValueConverter AnyAtomic = new XmlAnyConverter(XmlTypeCode.AnyAtomicType);

    #region AUTOGENERATED_XMLANYCONVERTER

    //-----------------------------------------------
    // ToBoolean
    //-----------------------------------------------

    public override bool ToBoolean(object value)
    {
      if (value == null) throw new ArgumentNullException("value");

      Type sourceType = value.GetType();

      if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value).ValueAsBoolean;

      return (bool)ChangeTypeWildcardDestination(value, BooleanType, null);
    }


    //-----------------------------------------------
    // ToDateTime
    //-----------------------------------------------

    public override DateTime ToDateTime(object value)
    {
      if (value == null) throw new ArgumentNullException("value");

      Type sourceType = value.GetType();

      if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value).ValueAsDateTime;

      return (DateTime)ChangeTypeWildcardDestination(value, DateTimeType, null);
    }

    //-----------------------------------------------
    // ToDateTimeOffset
    //-----------------------------------------------


    //-----------------------------------------------
    // ToDecimal
    //-----------------------------------------------

    public override decimal ToDecimal(object value)
    {
      if (value == null) throw new ArgumentNullException("value");

      Type sourceType = value.GetType();

      if (sourceType == XmlAtomicValueType) return ((decimal)((XmlAtomicValue)value).ValueAs(DecimalType));

      return (decimal)ChangeTypeWildcardDestination(value, DecimalType, null);
    }


    //-----------------------------------------------
    // ToDouble
    //-----------------------------------------------

    public override double ToDouble(object value)
    {
      if (value == null) throw new ArgumentNullException("value");

      Type sourceType = value.GetType();

      if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value).ValueAsDouble;

      return (double)ChangeTypeWildcardDestination(value, DoubleType, null);
    }


    //-----------------------------------------------
    // ToInt32
    //-----------------------------------------------

    public override int ToInt32(object value)
    {
      if (value == null) throw new ArgumentNullException("value");

      Type sourceType = value.GetType();

      if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value).ValueAsInt;

      return (int)ChangeTypeWildcardDestination(value, Int32Type, null);
    }


    //-----------------------------------------------
    // ToInt64
    //-----------------------------------------------

    public override long ToInt64(object value)
    {
      if (value == null) throw new ArgumentNullException("value");

      Type sourceType = value.GetType();

      if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value).ValueAsLong;

      return (long)ChangeTypeWildcardDestination(value, Int64Type, null);
    }


    //-----------------------------------------------
    // ToSingle
    //-----------------------------------------------

    public override float ToSingle(object value)
    {
      if (value == null) throw new ArgumentNullException("value");

      Type sourceType = value.GetType();

      if (sourceType == XmlAtomicValueType) return ((float)((XmlAtomicValue)value).ValueAs(SingleType));

      return (float)ChangeTypeWildcardDestination(value, SingleType, null);
    }


    //-----------------------------------------------
    // ToString
    //-----------------------------------------------

    // This converter does not support conversions to String.


    //-----------------------------------------------
    // ChangeType
    //-----------------------------------------------

    public override object ChangeType(bool value, Type destinationType)
    {
      if (destinationType == null) throw new ArgumentNullException("destinationType");

      if (destinationType == ObjectType) destinationType = DefaultClrType;
      if (destinationType == XmlAtomicValueType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean), (bool)value));

      return ChangeTypeWildcardSource(value, destinationType, null);
    }

    public override object ChangeType(DateTime value, Type destinationType)
    {
      if (destinationType == null) throw new ArgumentNullException("destinationType");

      if (destinationType == ObjectType) destinationType = DefaultClrType;
      if (destinationType == XmlAtomicValueType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DateTime), (DateTime)value));

      return ChangeTypeWildcardSource(value, destinationType, null);
    }

    public override object ChangeType(double value, Type destinationType)
    {
      if (destinationType == null) throw new ArgumentNullException("destinationType");

      if (destinationType == ObjectType) destinationType = DefaultClrType;
      if (destinationType == XmlAtomicValueType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double), (double)value));

      return ChangeTypeWildcardSource(value, destinationType, null);
    }

    public override object ChangeType(int value, Type destinationType)
    {
      if (destinationType == null) throw new ArgumentNullException("destinationType");

      if (destinationType == ObjectType) destinationType = DefaultClrType;
      if (destinationType == XmlAtomicValueType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Int), (int)value));

      return ChangeTypeWildcardSource(value, destinationType, null);
    }

    public override object ChangeType(long value, Type destinationType)
    {
      if (destinationType == null) throw new ArgumentNullException("destinationType");

      if (destinationType == ObjectType) destinationType = DefaultClrType;
      if (destinationType == XmlAtomicValueType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Long), (long)value));

      return ChangeTypeWildcardSource(value, destinationType, null);
    }

    public override object ChangeType(string value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      if (value == null) throw new ArgumentNullException("value");
      if (destinationType == null) throw new ArgumentNullException("destinationType");

      if (destinationType == ObjectType) destinationType = DefaultClrType;
      if (destinationType == XmlAtomicValueType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), (string)value));

      return ChangeTypeWildcardSource(value, destinationType, nsResolver);
    }

    public override object ChangeType(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      if (value == null) throw new ArgumentNullException("value");
      if (destinationType == null) throw new ArgumentNullException("destinationType");

      Type sourceType = value.GetType();

      if (destinationType == ObjectType) destinationType = DefaultClrType;
      if (destinationType == BooleanType)
      {
        if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value).ValueAsBoolean;
      }
      if (destinationType == DateTimeType)
      {
        if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value).ValueAsDateTime;
      }
      if (destinationType == DateTimeOffsetType)
      {
        if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value).ValueAs(DateTimeOffsetType);
      }
      if (destinationType == DecimalType)
      {
        if (sourceType == XmlAtomicValueType) return ((decimal)((XmlAtomicValue)value).ValueAs(DecimalType));
      }
      if (destinationType == DoubleType)
      {
        if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value).ValueAsDouble;
      }
      if (destinationType == Int32Type)
      {
        if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value).ValueAsInt;
      }
      if (destinationType == Int64Type)
      {
        if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value).ValueAsLong;
      }
      if (destinationType == SingleType)
      {
        if (sourceType == XmlAtomicValueType) return ((float)((XmlAtomicValue)value).ValueAs(SingleType));
      }
      if (destinationType == XmlAtomicValueType)
      {
        if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value);
        if (sourceType == BooleanType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean), (bool)value));
        if (sourceType == ByteType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedByte), value));
        if (sourceType == ByteArrayType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Base64Binary), value));
        if (sourceType == DateTimeType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DateTime), (DateTime)value));
        if (sourceType == DateTimeOffsetType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DateTime), (DateTimeOffset)value));
        if (sourceType == DecimalType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Decimal), value));
        if (sourceType == DoubleType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double), (double)value));
        if (sourceType == Int16Type) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Short), value));
        if (sourceType == Int32Type) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Int), (int)value));
        if (sourceType == Int64Type) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Long), (long)value));
        if (sourceType == SByteType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Byte), value));
        if (sourceType == SingleType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Float), value));
        if (sourceType == StringType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), (string)value));
        if (sourceType == TimeSpanType) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Duration), value));
        if (sourceType == UInt16Type) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedShort), value));
        if (sourceType == UInt32Type) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedInt), value));
        if (sourceType == UInt64Type) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedLong), value));
        if (IsDerivedFrom(sourceType, UriType)) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.AnyUri), value));
        if (IsDerivedFrom(sourceType, XmlQualifiedNameType)) return (new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.QName), value, nsResolver));
      }
      if (destinationType == XPathItemType)
      {
        if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value);
        if (IsDerivedFrom(sourceType, XPathNavigatorType)) return ((XPathNavigator)value);
      }
      if (destinationType == XPathNavigatorType)
      {
        if (IsDerivedFrom(sourceType, XPathNavigatorType)) return ToNavigator((XPathNavigator)value);
      }
      if (destinationType == XPathItemType) return ((XPathItem)this.ChangeType(value, XmlAtomicValueType, nsResolver));
      if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value).ValueAs(destinationType, nsResolver);

      return ChangeListType(value, destinationType, nsResolver);
    }


    //-----------------------------------------------
    // Helpers
    //-----------------------------------------------

    private object ChangeTypeWildcardDestination(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      Type sourceType = value.GetType();

      if (sourceType == XmlAtomicValueType) return ((XmlAtomicValue)value).ValueAs(destinationType, nsResolver);

      return ChangeListType(value, destinationType, nsResolver);
    }
    private object ChangeTypeWildcardSource(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      if (destinationType == XPathItemType) return ((XPathItem)this.ChangeType(value, XmlAtomicValueType, nsResolver));

      return ChangeListType(value, destinationType, nsResolver);
    }
    #endregion

    /// <summary>
    /// Throw an exception if nodes are not allowed by this converter.
    /// </summary>
    private XPathNavigator ToNavigator(XPathNavigator nav)
    {
      if (TypeCode != XmlTypeCode.Item)
        throw CreateInvalidClrMappingException(XPathNavigatorType, XPathNavigatorType);

      return nav;
    }
  }
}
