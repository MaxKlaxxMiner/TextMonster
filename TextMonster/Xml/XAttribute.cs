using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt ein XML-Attribut dar.
  /// </summary>
  public class XAttribute : XObject
  {
    private static IEnumerable<XAttribute> emptySequence;
    internal XAttribute next;
    internal XName name;
    internal string value;

    /// <summary>
    /// Ruft eine leere Auflistung von Attributen ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XAttribute"/>, das eine leere Auflistung enthält.
    /// </returns>
    public static IEnumerable<XAttribute> EmptySequence
    {
      get
      {
        if (XAttribute.emptySequence == null)
          XAttribute.emptySequence = (IEnumerable<XAttribute>)new XAttribute[0];
        return XAttribute.emptySequence;
      }
    }

    /// <summary>
    /// Bestimmt, ob dieses Attribut eine Namespacedeklaration ist.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn dieses Attribut eine Namespacedeklaration ist, andernfalls false.
    /// </returns>
    public bool IsNamespaceDeclaration
    {
      get
      {
        string namespaceName = this.name.NamespaceName;
        if (namespaceName.Length == 0)
          return this.name.LocalName == "xmlns";
        return namespaceName == "http://www.w3.org/2000/xmlns/";
      }
    }

    /// <summary>
    /// Ruft den erweiterten Namen dieses Attributs ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XName"/>, der den Namen dieses Attributs enthält.
    /// </returns>
    public XName Name
    {
      get
      {
        return this.name;
      }
    }

    /// <summary>
    /// Ruft das nächste Attribut des übergeordneten Elements ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XAttribute"/>, das das nächste Attribut des übergeordneten Elements enthält.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public XAttribute NextAttribute
    {
      get
      {
        if (this.parent == null || ((XElement)this.parent).lastAttr == this)
          return (XAttribute)null;
        return this.next;
      }
    }

    /// <summary>
    /// Ruft den Knotentyp für diesen Knoten ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Knotentyp. Für <see cref="T:System.Xml.Linq.XAttribute"/>-Objekte ist dieser Wert <see cref="F:System.Xml.XmlNodeType.Attribute"/>.
    /// </returns>
    public override XmlNodeType NodeType
    {
      get
      {
        return XmlNodeType.Attribute;
      }
    }

    /// <summary>
    /// Ruft das vorherige Attribut des übergeordneten Elements ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XAttribute"/>, das das vorherige Attribut des übergeordneten Elements enthält.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public XAttribute PreviousAttribute
    {
      get
      {
        if (this.parent == null)
          return (XAttribute)null;
        XAttribute xattribute = ((XElement)this.parent).lastAttr;
        while (xattribute.next != this)
          xattribute = xattribute.next;
        if (xattribute == ((XElement)this.parent).lastAttr)
          return (XAttribute)null;
        return xattribute;
      }
    }

    /// <summary>
    /// Ruft den Wert des Attributs ab oder legt diesen fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der den Wert dieses Attributs enthält.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">Beim Festlegen ist <paramref name="value"/> gleich null.</exception>
    public string Value
    {
      get
      {
        return this.value;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        XAttribute.ValidateAttribute(this.name, value);
        bool flag = this.NotifyChanging((object)this, XObjectChangeEventArgs.Value);
        this.value = value;
        if (!flag)
          return;
        this.NotifyChanged((object)this, XObjectChangeEventArgs.Value);
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XAttribute"/>-Klasse mit dem angegebenen Namen und Wert.
    /// </summary>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/> des Attributs.</param><param name="value">Ein <see cref="T:System.Object"/>, das den Wert des Attributs enthält.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="name"/>-Parameter oder der <paramref name="value"/>-Parameter ist null.</exception>
    public XAttribute(XName name, object value)
    {
      if (name == (XName)null)
        throw new ArgumentNullException("name");
      if (value == null)
        throw new ArgumentNullException("value");
      string stringValue = XContainer.GetStringValue(value);
      XAttribute.ValidateAttribute(name, stringValue);
      this.name = name;
      this.value = stringValue;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XAttribute"/>-Klasse mit einem anderen <see cref="T:System.Xml.Linq.XAttribute"/>-Objekt.
    /// </summary>
    /// <param name="other">Ein <see cref="T:System.Xml.Linq.XAttribute"/>-Objekt, aus dem kopiert werden soll.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="other"/>-Parameter ist null.</exception>
    public XAttribute(XAttribute other)
    {
      if (other == null)
        throw new ArgumentNullException("other");
      this.name = other.name;
      this.value = other.value;
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in <see cref="T:System.String"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Eine <see cref="T:System.String"/>, die den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.String"/> umgewandelt werden soll.</param>
    public static explicit operator string(XAttribute attribute)
    {
      if (attribute == null)
        return (string)null;
      return attribute.value;
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.Boolean"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Boolean"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.Boolean"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Boolean"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator bool(XAttribute attribute)
    {
      if (attribute == null)
        throw new ArgumentNullException("attribute");
      return XmlConvert.ToBoolean(attribute.value.ToLower(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Boolean"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Boolean"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Boolean"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Boolean"/>-Wert.</exception>
    public static explicit operator bool?(XAttribute attribute)
    {
      if (attribute == null)
        return new bool?();
      return new bool?(XmlConvert.ToBoolean(attribute.value.ToLower(CultureInfo.InvariantCulture)));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in <see cref="T:System.Int32"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Int32"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.Int32"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Int32"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator int(XAttribute attribute)
    {
      if (attribute == null)
        throw new ArgumentNullException("attribute");
      return XmlConvert.ToInt32(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Int32"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Int32"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Int32"/> umgewandelt werden soll.</param>
    public static explicit operator int?(XAttribute attribute)
    {
      if (attribute == null)
        return new int?();
      return new int?(XmlConvert.ToInt32(attribute.value));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.UInt32"/>-Wert um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.UInt32"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in einen <see cref="T:System.UInt32"/>-Wert umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.UInt32"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator uint(XAttribute attribute)
    {
      if (attribute == null)
        throw new ArgumentNullException("attribute");
      return XmlConvert.ToUInt32(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.UInt32"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.UInt32"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.UInt32"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.UInt32"/>-Wert.</exception>
    public static explicit operator uint?(XAttribute attribute)
    {
      if (attribute == null)
        return new uint?();
      return new uint?(XmlConvert.ToUInt32(attribute.value));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in <see cref="T:System.Int64"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Int64"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.Int64"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Int64"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator long(XAttribute attribute)
    {
      if (attribute == null)
        throw new ArgumentNullException("attribute");
      return XmlConvert.ToInt64(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Int64"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Int64"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Int64"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Int64"/>-Wert.</exception>
    public static explicit operator long?(XAttribute attribute)
    {
      if (attribute == null)
        return new long?();
      return new long?(XmlConvert.ToInt64(attribute.value));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.UInt64"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.UInt64"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.UInt64"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.UInt64"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator ulong(XAttribute attribute)
    {
      if (attribute == null)
        throw new ArgumentNullException("attribute");
      return XmlConvert.ToUInt64(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.UInt64"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.UInt64"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.UInt64"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.UInt64"/>-Wert.</exception>
    public static explicit operator ulong?(XAttribute attribute)
    {
      if (attribute == null)
        return new ulong?();
      return new ulong?(XmlConvert.ToUInt64(attribute.value));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.Single"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Single"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.Single"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Single"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator float(XAttribute attribute)
    {
      if (attribute == null)
        throw new ArgumentNullException("attribute");
      return XmlConvert.ToSingle(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Single"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Single"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Single"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Single"/>-Wert.</exception>
    public static explicit operator float?(XAttribute attribute)
    {
      if (attribute == null)
        return new float?();
      return new float?(XmlConvert.ToSingle(attribute.value));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.Double"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Double"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.Double"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Double"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator double(XAttribute attribute)
    {
      if (attribute == null)
        throw new ArgumentNullException("attribute");
      return XmlConvert.ToDouble(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.Nullable`1"/>-Wert vom Typ <see cref="T:System.Double"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Double"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in einen <see cref="T:System.Nullable`1"/>-Wert vom Typ <see cref="T:System.Double"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Double"/>-Wert.</exception>
    public static explicit operator double?(XAttribute attribute)
    {
      if (attribute == null)
        return new double?();
      return new double?(XmlConvert.ToDouble(attribute.value));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.Decimal"/>-Wert um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Decimal"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in einen <see cref="T:System.Decimal"/>-Wert umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Decimal"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator Decimal(XAttribute attribute)
    {
      if (attribute == null)
        throw new ArgumentNullException("attribute");
      return XmlConvert.ToDecimal(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Decimal"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Decimal"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Decimal"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Decimal"/>-Wert.</exception>
    public static explicit operator Decimal?(XAttribute attribute)
    {
      if (attribute == null)
        return new Decimal?();
      return new Decimal?(XmlConvert.ToDecimal(attribute.value));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.DateTime"/>-Wert um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.DateTime"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.DateTime"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.DateTime"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator DateTime(XAttribute attribute)
    {
      if (attribute == null)
        throw new ArgumentNullException("attribute");
      return DateTime.Parse(attribute.value, (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.DateTime"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.DateTime"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.DateTime"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.DateTime"/>-Wert.</exception>
    public static explicit operator DateTime?(XAttribute attribute)
    {
      if (attribute == null)
        return new DateTime?();
      return new DateTime?(DateTime.Parse(attribute.value, (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.DateTimeOffset"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.DateTimeOffset"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in einen <see cref="T:System.DateTimeOffset"/>-Wert umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.DateTimeOffset"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator DateTimeOffset(XAttribute attribute)
    {
      if (attribute == null)
        throw new ArgumentNullException("attribute");
      return XmlConvert.ToDateTimeOffset(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.Nullable`1"/>-Wert vom Typ <see cref="T:System.DateTimeOffset"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.DateTimeOffset"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in einen <see cref="T:System.Nullable`1"/>-Wert vom Typ <see cref="T:System.DateTimeOffset"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.DateTimeOffset"/>-Wert.</exception>
    public static explicit operator DateTimeOffset?(XAttribute attribute)
    {
      if (attribute == null)
        return new DateTimeOffset?();
      return new DateTimeOffset?(XmlConvert.ToDateTimeOffset(attribute.value));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in eine <see cref="T:System.TimeSpan"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.TimeSpan"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.TimeSpan"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.TimeSpan"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator TimeSpan(XAttribute attribute)
    {
      if (attribute == null)
        throw new ArgumentNullException("attribute");
      return XmlConvert.ToTimeSpan(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.Nullable`1"/>-Wert vom Typ <see cref="T:System.TimeSpan"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.TimeSpan"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.TimeSpan"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.TimeSpan"/>-Wert.</exception>
    public static explicit operator TimeSpan?(XAttribute attribute)
    {
      if (attribute == null)
        return new TimeSpan?();
      return new TimeSpan?(XmlConvert.ToTimeSpan(attribute.value));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.Guid"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Guid"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.Guid"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Guid"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator Guid(XAttribute attribute)
    {
      if (attribute == null)
        throw new ArgumentNullException("attribute");
      return XmlConvert.ToGuid(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.Nullable`1"/>-Wert vom Typ <see cref="T:System.Guid"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Guid"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Guid"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Guid"/>-Wert.</exception>
    public static explicit operator Guid?(XAttribute attribute)
    {
      if (attribute == null)
        return new Guid?();
      return new Guid?(XmlConvert.ToGuid(attribute.value));
    }

    /// <summary>
    /// Entfernt dieses Attribut aus seinem übergeordneten Element.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Das übergeordnete Element ist null.</exception>
    public void Remove()
    {
      if (this.parent == null)
        throw new InvalidOperationException("InvalidOperation_MissingParent");
      ((XElement)this.parent).RemoveAttribute(this);
    }

    /// <summary>
    /// Legt den Wert dieses Attributs fest.
    /// </summary>
    /// <param name="value">Der Wert, der diesem Attribut zugewiesen werden soll.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="value"/>-Parameter ist null.</exception><exception cref="T:System.ArgumentException">Der <paramref name="value"/> ist ein <see cref="T:System.Xml.Linq.XObject"/>.</exception>
    public void SetValue(object value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      this.Value = XContainer.GetStringValue(value);
    }

    /// <summary>
    /// Konvertiert das aktuelle <see cref="T:System.Xml.Linq.XAttribute"/>-Objekt in eine Zeichenfolgendarstellung.
    /// </summary>
    /// 
    /// <returns>
    /// Eine <see cref="T:System.String"/>, die die XML-Textdarstellung eines Attributs und seines Werts enthält.
    /// </returns>
    public override string ToString()
    {
      using (StringWriter stringWriter = new StringWriter((IFormatProvider)CultureInfo.InvariantCulture))
      {
        using (XmlWriter xmlWriter = XmlWriter.Create((TextWriter)stringWriter, new XmlWriterSettings()
        {
          ConformanceLevel = ConformanceLevel.Fragment
        }))
          xmlWriter.WriteAttributeString(this.GetPrefixOfNamespace(this.name.Namespace), this.name.LocalName, this.name.NamespaceName, this.value);
        return stringWriter.ToString().Trim();
      }
    }

    internal int GetDeepHashCode()
    {
      return this.name.GetHashCode() ^ this.value.GetHashCode();
    }

    internal string GetPrefixOfNamespace(XNamespace ns)
    {
      string namespaceName = ns.NamespaceName;
      if (namespaceName.Length == 0)
        return string.Empty;
      if (this.parent != null)
        return ((XElement)this.parent).GetPrefixOfNamespace(ns);
      if (namespaceName == "http://www.w3.org/XML/1998/namespace")
        return "xml";
      if (namespaceName == "http://www.w3.org/2000/xmlns/")
        return "xmlns";
      return (string)null;
    }

    private static void ValidateAttribute(XName name, string value)
    {
      string namespaceName = name.NamespaceName;
      if (namespaceName == "http://www.w3.org/2000/xmlns/")
      {
        if (value.Length == 0)
          throw new ArgumentException("Argument_NamespaceDeclarationPrefixed");
        if (value == "http://www.w3.org/XML/1998/namespace")
        {
          if (name.LocalName != "xml")
            throw new ArgumentException("Argument_NamespaceDeclarationXml");
        }
        else
        {
          if (value == "http://www.w3.org/2000/xmlns/")
            throw new ArgumentException("Argument_NamespaceDeclarationXmlns");
          string localName = name.LocalName;
          if (localName == "xml")
            throw new ArgumentException("Argument_NamespaceDeclarationXml");
          if (localName == "xmlns")
            throw new ArgumentException("Argument_NamespaceDeclarationXmlns");
        }
      }
      else
      {
        if (namespaceName.Length != 0 || !(name.LocalName == "xmlns"))
          return;
        if (value == "http://www.w3.org/XML/1998/namespace")
          throw new ArgumentException("Argument_NamespaceDeclarationXml");
        if (value == "http://www.w3.org/2000/xmlns/")
          throw new ArgumentException("Argument_NamespaceDeclarationXmlns");
      }
    }
  }
}
