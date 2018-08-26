using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
// ReSharper disable UnusedMember.Global

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt ein XML-Attribut dar.
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class X_Attribute : X_Object
  {
    static IEnumerable<X_Attribute> emptySequence;
    internal X_Attribute next;
    internal readonly X_Name name;
    internal string value;

    /// <summary>
    /// Ruft eine leere Auflistung von Attributen ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XAttribute"/>, das eine leere Auflistung enthält.
    /// </returns>
    public static IEnumerable<X_Attribute> EmptySequence
    {
      get { return emptySequence ?? (emptySequence = new X_Attribute[0]); }
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
        string namespaceName = name.NamespaceName;
        if (namespaceName.Length == 0)
          return name.LocalName == "xmlns";
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
    public X_Name Name
    {
      get
      {
        return name;
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
    public X_Attribute NextAttribute
    {
      get
      {
        if (parent == null || ((X_Element)parent).lastAttr == this)
          return null;
        return next;
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
    public X_Attribute PreviousAttribute
    {
      get
      {
        if (parent == null)
          return null;
        var xattribute = ((X_Element)parent).lastAttr;
        while (xattribute.next != this)
          xattribute = xattribute.next;
        if (xattribute == ((X_Element)parent).lastAttr)
          return null;
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
        return value;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        ValidateAttribute(name, value);
        bool flag = NotifyChanging(this, X_ObjectChangeEventArgs.Value);
        this.value = value;
        if (!flag)
          return;
        NotifyChanged(this, X_ObjectChangeEventArgs.Value);
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XAttribute"/>-Klasse mit dem angegebenen Namen und Wert.
    /// </summary>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/> des Attributs.</param><param name="value">Ein <see cref="T:System.Object"/>, das den Wert des Attributs enthält.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="name"/>-Parameter oder der <paramref name="value"/>-Parameter ist null.</exception>
    public X_Attribute(X_Name name, object value)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (value == null)
        throw new ArgumentNullException("value");
      string stringValue = X_Container.GetStringValue(value);
      ValidateAttribute(name, stringValue);
      this.name = name;
      this.value = stringValue;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XAttribute"/>-Klasse mit einem anderen <see cref="T:System.Xml.Linq.XAttribute"/>-Objekt.
    /// </summary>
    /// <param name="other">Ein <see cref="T:System.Xml.Linq.XAttribute"/>-Objekt, aus dem kopiert werden soll.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="other"/>-Parameter ist null.</exception>
    public X_Attribute(X_Attribute other)
    {
      if (other == null)
        throw new ArgumentNullException("other");
      name = other.name;
      value = other.value;
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in <see cref="T:System.String"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Eine <see cref="T:System.String"/>, die den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.String"/> umgewandelt werden soll.</param>
    public static explicit operator string(X_Attribute attribute)
    {
      if (attribute == null)
        return null;
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
    public static explicit operator bool(X_Attribute attribute)
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
    public static explicit operator bool?(X_Attribute attribute)
    {
      if (attribute == null)
        return new bool?();
      return XmlConvert.ToBoolean(attribute.value.ToLower(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in <see cref="T:System.Int32"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Int32"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.Int32"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Int32"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator int(X_Attribute attribute)
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
    public static explicit operator int?(X_Attribute attribute)
    {
      if (attribute == null)
        return new int?();
      return XmlConvert.ToInt32(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.UInt32"/>-Wert um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.UInt32"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in einen <see cref="T:System.UInt32"/>-Wert umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.UInt32"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator uint(X_Attribute attribute)
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
    public static explicit operator uint?(X_Attribute attribute)
    {
      if (attribute == null)
        return new uint?();
      return XmlConvert.ToUInt32(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in <see cref="T:System.Int64"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Int64"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.Int64"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Int64"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator long(X_Attribute attribute)
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
    public static explicit operator long?(X_Attribute attribute)
    {
      if (attribute == null)
        return new long?();
      return XmlConvert.ToInt64(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.UInt64"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.UInt64"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.UInt64"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.UInt64"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator ulong(X_Attribute attribute)
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
    public static explicit operator ulong?(X_Attribute attribute)
    {
      if (attribute == null)
        return new ulong?();
      return XmlConvert.ToUInt64(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.Single"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Single"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.Single"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Single"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator float(X_Attribute attribute)
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
    public static explicit operator float?(X_Attribute attribute)
    {
      if (attribute == null)
        return new float?();
      return XmlConvert.ToSingle(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.Double"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Double"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.Double"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Double"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator double(X_Attribute attribute)
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
    public static explicit operator double?(X_Attribute attribute)
    {
      if (attribute == null)
        return new double?();
      return XmlConvert.ToDouble(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.Decimal"/>-Wert um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Decimal"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in einen <see cref="T:System.Decimal"/>-Wert umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Decimal"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator decimal(X_Attribute attribute)
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
    public static explicit operator decimal?(X_Attribute attribute)
    {
      if (attribute == null)
        return new decimal?();
      return XmlConvert.ToDecimal(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.DateTime"/>-Wert um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.DateTime"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.DateTime"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.DateTime"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator DateTime(X_Attribute attribute)
    {
      if (attribute == null)
        throw new ArgumentNullException("attribute");
      return DateTime.Parse(attribute.value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.DateTime"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.DateTime"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.DateTime"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.DateTime"/>-Wert.</exception>
    public static explicit operator DateTime?(X_Attribute attribute)
    {
      if (attribute == null)
        return new DateTime?();
      return DateTime.Parse(attribute.value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.DateTimeOffset"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.DateTimeOffset"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in einen <see cref="T:System.DateTimeOffset"/>-Wert umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.DateTimeOffset"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator DateTimeOffset(X_Attribute attribute)
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
    public static explicit operator DateTimeOffset?(X_Attribute attribute)
    {
      if (attribute == null)
        return new DateTimeOffset?();
      return XmlConvert.ToDateTimeOffset(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in eine <see cref="T:System.TimeSpan"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.TimeSpan"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.TimeSpan"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.TimeSpan"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator TimeSpan(X_Attribute attribute)
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
    public static explicit operator TimeSpan?(X_Attribute attribute)
    {
      if (attribute == null)
        return new TimeSpan?();
      return XmlConvert.ToTimeSpan(attribute.value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.Guid"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Guid"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XAttribute"/> enthält.
    /// </returns>
    /// <param name="attribute">Das <see cref="T:System.Xml.Linq.XAttribute"/>, das in <see cref="T:System.Guid"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Attribut enthält keinen gültigen <see cref="T:System.Guid"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="attribute"/>-Parameter ist null.</exception>
    public static explicit operator Guid(X_Attribute attribute)
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
    public static explicit operator Guid?(X_Attribute attribute)
    {
      if (attribute == null)
        return new Guid?();
      return XmlConvert.ToGuid(attribute.value);
    }

    /// <summary>
    /// Entfernt dieses Attribut aus seinem übergeordneten Element.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Das übergeordnete Element ist null.</exception>
    public void Remove()
    {
      if (parent == null)
        throw new InvalidOperationException("InvalidOperation_MissingParent");
      ((X_Element)parent).RemoveAttribute(this);
    }

    /// <summary>
    /// Legt den Wert dieses Attributs fest.
    /// </summary>
    /// <param name="value">Der Wert, der diesem Attribut zugewiesen werden soll.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="value"/>-Parameter ist null.</exception><exception cref="T:System.ArgumentException">Der <paramref name="value"/> ist ein <see cref="T:System.Xml.Linq.XObject"/>.</exception>
    public void SetValue(object value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      Value = X_Container.GetStringValue(value);
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
      using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
      {
        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
        {
          ConformanceLevel = ConformanceLevel.Fragment
        }))
          xmlWriter.WriteAttributeString(GetPrefixOfNamespace(name.Namespace), name.LocalName, name.NamespaceName, value);
        return stringWriter.ToString().Trim();
      }
    }

    internal int GetDeepHashCode()
    {
      return name.GetHashCode() ^ value.GetHashCode();
    }

    internal string GetPrefixOfNamespace(X_Namespace ns)
    {
      string namespaceName = ns.NamespaceName;
      if (namespaceName.Length == 0)
        return string.Empty;
      if (parent != null)
        return ((X_Element)parent).GetPrefixOfNamespace(ns);
      if (namespaceName == "http://www.w3.org/XML/1998/namespace")
        return "xml";
      if (namespaceName == "http://www.w3.org/2000/xmlns/")
        return "xmlns";
      return null;
    }

    static void ValidateAttribute(X_Name name, string value)
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
        if (namespaceName.Length != 0 || name.LocalName != "xmlns")
          return;
        if (value == "http://www.w3.org/XML/1998/namespace")
          throw new ArgumentException("Argument_NamespaceDeclarationXml");
        if (value == "http://www.w3.org/2000/xmlns/")
          throw new ArgumentException("Argument_NamespaceDeclarationXmlns");
      }
    }
  }
}
