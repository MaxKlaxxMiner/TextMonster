﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt ein XML-Element dar.
  /// </summary>
  public class XElement : XContainer, IXmlSerializable
  {
    static IEnumerable<XElement> emptySequence;
    internal XName name;
    internal XAttribute lastAttr;

    /// <summary>
    /// Ruft eine leere Auflistung von Elementen ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/>, das eine leere Auflistung enthält.
    /// </returns>
    public static IEnumerable<XElement> EmptySequence
    {
      get { return emptySequence ?? (emptySequence = new XElement[0]); }
    }

    /// <summary>
    /// Ruft das erste Attribut dieses Elements ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XAttribute"/>, das das erste Attribut dieses Elements enthält.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public XAttribute FirstAttribute
    {
      get
      {
        if (lastAttr == null)
          return null;
        return lastAttr.next;
      }
    }

    /// <summary>
    /// Ruft einen Wert ab, der angibt, ob dieses Element über mindestens ein Attribut verfügt.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn dieses Element über mindestens ein Attribut verfügt, andernfalls false.
    /// </returns>
    public bool HasAttributes
    {
      get
      {
        return lastAttr != null;
      }
    }

    /// <summary>
    /// Ruft einen Wert ab, der angibt, ob dieses Element über mindestens ein untergeordnetes Element verfügt.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn dieses Element über mindestens ein untergeordnetes Element verfügt, andernfalls false.
    /// </returns>
    public bool HasElements
    {
      get
      {
        var xnode = content as XNode;
        if (xnode != null)
        {
          while (!(xnode is XElement))
          {
            xnode = xnode.next;
            if (xnode == content)
              goto label_4;
          }
          return true;
        }
      label_4:
        return false;
      }
    }

    /// <summary>
    /// Ruft einen Wert ab, der angibt, ob dieses Element keinen Inhalt enthält.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn dieses Element keinen Inhalt enthält, andernfalls false.
    /// </returns>
    public bool IsEmpty
    {
      get
      {
        return content == null;
      }
    }

    /// <summary>
    /// Ruft das letzte Attribut dieses Elements ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XAttribute"/>, das das letzte Attribut dieses Elements enthält.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public XAttribute LastAttribute
    {
      get
      {
        return lastAttr;
      }
    }

    /// <summary>
    /// Ruft den Namen dieses Elements ab oder legt diesen fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XName"/>, der den Namen dieses Elements enthält.
    /// </returns>
    public XName Name
    {
      get
      {
        return name;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        bool flag = NotifyChanging(this, XObjectChangeEventArgs.Name);
        name = value;
        if (!flag)
          return;
        NotifyChanged(this, XObjectChangeEventArgs.Name);
      }
    }

    /// <summary>
    /// Ruft den Knotentyp für diesen Knoten ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Knotentyp. Für <see cref="T:System.Xml.Linq.XElement"/>-Objekte ist dieser Wert <see cref="F:System.Xml.XmlNodeType.Element"/>.
    /// </returns>
    public override XmlNodeType NodeType
    {
      get
      {
        return XmlNodeType.Element;
      }
    }

    /// <summary>
    /// Ruft den verketteten Textinhalt dieses Elements ab oder legt ihn fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der den gesamten Textinhalt dieses Elements enthält. Wenn mehrere Textknoten vorhanden sind, werden sie verkettet.
    /// </returns>
    public string Value
    {
      get
      {
        if (content == null)
          return string.Empty;
        string str = content as string;
        if (str != null)
          return str;
        var sb = new StringBuilder();
        AppendText(sb);
        return sb.ToString();
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        RemoveNodes();
        Add(value);
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XElement"/>-Klasse mit dem angegebenen Namen.
    /// </summary>
    /// <param name="name">Ein <see cref="T:System.Xml.Linq.XName"/>, der den Namen des Elements enthält.</param>
    public XElement(XName name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      this.name = name;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XElement"/>-Klasse mit dem angegebenen Namen und Inhalt.
    /// </summary>
    /// <param name="name">Ein <see cref="T:System.Xml.Linq.XName"/>, der den Elementnamen enthält.</param><param name="content">Der Inhalt des Elements.</param>
    public XElement(XName name, object content)
      : this(name)
    {
      AddContentSkipNotify(content);
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XElement"/>-Klasse mit dem angegebenen Namen und Inhalt.
    /// </summary>
    /// <param name="name">Ein <see cref="T:System.Xml.Linq.XName"/>, der den Elementnamen enthält.</param><param name="content">Der ursprüngliche Inhalt des Elements.</param>
    public XElement(XName name, params object[] content)
      : this(name, (object)content)
    {
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XElement"/>-Klasse mit einem anderen <see cref="T:System.Xml.Linq.XElement"/>-Objekt.
    /// </summary>
    /// <param name="other">Ein <see cref="T:System.Xml.Linq.XElement"/>-Objekt, aus dem kopiert werden soll.</param>
    public XElement(XElement other)
      : base(other)
    {
      name = other.name;
      var other1 = other.lastAttr;
      if (other1 == null)
        return;
      do
      {
        other1 = other1.next;
        AppendAttributeSkipNotify(new XAttribute(other1));
      }
      while (other1 != other.lastAttr);
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XElement"/>-Klasse mit einem <see cref="T:System.Xml.Linq.XStreamingElement"/>-Objekt.
    /// </summary>
    /// <param name="other">Ein <see cref="T:System.Xml.Linq.XStreamingElement"/>, das nicht ausgewertete Abfragen enthält, die zum Ermitteln des Inhalts des <see cref="T:System.Xml.Linq.XElement"/> durchlaufen werden.</param>
    public XElement(XStreamingElement other)
    {
      if (other == null)
        throw new ArgumentNullException("other");
      name = other.name;
      AddContentSkipNotify(other.content);
    }

    internal XElement()
      : this("default")
    {
    }

    internal XElement(XmlReader r, LoadOptions o = LoadOptions.None)
    {
      ReadElementFrom(r, o);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in <see cref="T:System.String"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in <see cref="T:System.String"/> umgewandelt werden soll.</param>
    public static explicit operator string(XElement element)
    {
      if (element == null)
        return null;
      return element.Value;
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in einen <see cref="T:System.Boolean"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Boolean"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in <see cref="T:System.Boolean"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Boolean"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="element"/>-Parameter ist null.</exception>
    public static explicit operator bool(XElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return XmlConvert.ToBoolean(element.Value.ToLower(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Boolean"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Boolean"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Boolean"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Boolean"/>-Wert.</exception>
    public static explicit operator bool?(XElement element)
    {
      if (element == null)
        return new bool?();
      return XmlConvert.ToBoolean(element.Value.ToLower(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in einen <see cref="T:System.Int32"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Int32"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in <see cref="T:System.Int32"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Int32"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="element"/>-Parameter ist null.</exception>
    public static explicit operator int(XElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return XmlConvert.ToInt32(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Int32"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Int32"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Int32"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Int32"/>-Wert.</exception>
    public static explicit operator int?(XElement element)
    {
      if (element == null)
        return new int?();
      return XmlConvert.ToInt32(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in einen <see cref="T:System.UInt32"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.UInt32"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in <see cref="T:System.UInt32"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.UInt32"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="element"/>-Parameter ist null.</exception>
    public static explicit operator uint(XElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return XmlConvert.ToUInt32(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.UInt32"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.UInt32"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.UInt32"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.UInt32"/>-Wert.</exception>
    public static explicit operator uint?(XElement element)
    {
      if (element == null)
        return new uint?();
      return XmlConvert.ToUInt32(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in einen <see cref="T:System.Int64"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Int64"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in <see cref="T:System.Int64"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Int64"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="element"/>-Parameter ist null.</exception>
    public static explicit operator long(XElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return XmlConvert.ToInt64(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Int64"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Int64"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Int64"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Int64"/>-Wert.</exception>
    public static explicit operator long?(XElement element)
    {
      if (element == null)
        return new long?();
      return XmlConvert.ToInt64(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in einen <see cref="T:System.UInt64"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.UInt64"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in <see cref="T:System.UInt64"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.UInt64"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="element"/>-Parameter ist null.</exception>
    public static explicit operator ulong(XElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return XmlConvert.ToUInt64(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.UInt64"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.UInt64"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.UInt64"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.UInt64"/>-Wert.</exception>
    public static explicit operator ulong?(XElement element)
    {
      if (element == null)
        return new ulong?();
      return XmlConvert.ToUInt64(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in einen <see cref="T:System.Single"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Single"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in <see cref="T:System.Single"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Single"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="element"/>-Parameter ist null.</exception>
    public static explicit operator float(XElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return XmlConvert.ToSingle(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Single"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Single"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Single"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Single"/>-Wert.</exception>
    public static explicit operator float?(XElement element)
    {
      if (element == null)
        return new float?();
      return XmlConvert.ToSingle(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in einen <see cref="T:System.Double"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Double"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in <see cref="T:System.Double"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Double"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="element"/>-Parameter ist null.</exception>
    public static explicit operator double(XElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return XmlConvert.ToDouble(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Double"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Double"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Double"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Double"/>-Wert.</exception>
    public static explicit operator double?(XElement element)
    {
      if (element == null)
        return new double?();
      return XmlConvert.ToDouble(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in einen <see cref="T:System.Decimal"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Decimal"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in <see cref="T:System.Decimal"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Decimal"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="element"/>-Parameter ist null.</exception>
    public static explicit operator decimal(XElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return XmlConvert.ToDecimal(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Decimal"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Decimal"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Decimal"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Decimal"/>-Wert.</exception>
    public static explicit operator decimal?(XElement element)
    {
      if (element == null)
        return new decimal?();
      return XmlConvert.ToDecimal(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in einen <see cref="T:System.DateTime"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.DateTime"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in <see cref="T:System.DateTime"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.DateTime"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="element"/>-Parameter ist null.</exception>
    public static explicit operator DateTime(XElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return DateTime.Parse(element.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.DateTime"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.DateTime"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.DateTime"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.DateTime"/>-Wert.</exception>
    public static explicit operator DateTime?(XElement element)
    {
      if (element == null)
        return new DateTime?();
      return DateTime.Parse(element.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XAttribute"/> in einen <see cref="T:System.DateTimeOffset"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.DateTimeOffset"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in <see cref="T:System.DateTimeOffset"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.DateTimeOffset"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="element"/>-Parameter ist null.</exception>
    public static explicit operator DateTimeOffset(XElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return XmlConvert.ToDateTimeOffset(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.DateTimeOffset"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.DateTimeOffset"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.DateTimeOffset"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.DateTimeOffset"/>-Wert.</exception>
    public static explicit operator DateTimeOffset?(XElement element)
    {
      if (element == null)
        return new DateTimeOffset?();
      return XmlConvert.ToDateTimeOffset(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in einen <see cref="T:System.TimeSpan"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.TimeSpan"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in <see cref="T:System.TimeSpan"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.TimeSpan"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="element"/>-Parameter ist null.</exception>
    public static explicit operator TimeSpan(XElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return XmlConvert.ToTimeSpan(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.TimeSpan"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.TimeSpan"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.TimeSpan"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.TimeSpan"/>-Wert.</exception>
    public static explicit operator TimeSpan?(XElement element)
    {
      if (element == null)
        return new TimeSpan?();
      return XmlConvert.ToTimeSpan(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in einen <see cref="T:System.Guid"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Guid"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in <see cref="T:System.Guid"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Guid"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="element"/>-Parameter ist null.</exception>
    public static explicit operator Guid(XElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return XmlConvert.ToGuid(element.Value);
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Guid"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Guid"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in ein <see cref="T:System.Nullable`1"/> vom Typ <see cref="T:System.Guid"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Guid"/>-Wert.</exception>
    public static explicit operator Guid?(XElement element)
    {
      if (element == null)
        return new Guid?();
      return XmlConvert.ToGuid(element.Value);
    }

    /// <summary>
    /// Gibt eine Auflistung von Elementen mit diesem Element und den übergeordneten Elementen dieses Elements zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/> von Elementen mit diesem Element und den übergeordneten Elementen dieses Elements.
    /// </returns>
    public IEnumerable<XElement> AncestorsAndSelf()
    {
      return GetAncestors(null, true);
    }

    /// <summary>
    /// Gibt eine gefilterte Auflistung von Elementen mit diesem Element und den übergeordneten Elementen dieses Elements zurück. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/> mit diesem Element und den übergeordneten Elementen dieses Elements. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </returns>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/>, mit dem eine Übereinstimmung gefunden werden soll.</param>
    public IEnumerable<XElement> AncestorsAndSelf(XName name)
    {
      if (!(name != null))
        return EmptySequence;
      return GetAncestors(name, true);
    }

    /// <summary>
    /// Gibt das <see cref="T:System.Xml.Linq.XAttribute"/> des <see cref="T:System.Xml.Linq.XElement"/> zurück, das über den angegebenen <see cref="T:System.Xml.Linq.XName"/> verfügt.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XAttribute"/>, das über den angegebenen <see cref="T:System.Xml.Linq.XName"/> verfügt. null, wenn kein Attribut mit dem angegebenen Namen vorhanden ist.
    /// </returns>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/> des abzurufenden <see cref="T:System.Xml.Linq.XAttribute"/>.</param>
    public XAttribute Attribute(XName name)
    {
      var xattribute = lastAttr;
      if (xattribute != null)
      {
        do
        {
          xattribute = xattribute.next;
          if (xattribute.name == name)
            return xattribute;
        }
        while (xattribute != lastAttr);
      }
      return null;
    }

    /// <summary>
    /// Gibt eine Auflistung von Attributen dieses Elements zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XAttribute"/> der Attribute dieses Elements.
    /// </returns>
    public IEnumerable<XAttribute> Attributes()
    {
      return GetAttributes(null);
    }

    /// <summary>
    /// Gibt eine gefilterte Auflistung der Attribute dieses Elements zurück. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XAttribute"/>, das die Attribute dieses Elements enthält. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </returns>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/>, mit dem eine Übereinstimmung gefunden werden soll.</param>
    public IEnumerable<XAttribute> Attributes(XName name)
    {
      if (!(name != null))
        return XAttribute.EmptySequence;
      return GetAttributes(name);
    }

    /// <summary>
    /// Gibt eine Auflistung von Knoten mit diesem Element und allen Nachfolgerknoten dieses Elements in Dokumentreihenfolge zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XNode"/> mit diesem Element und allen Nachfolgerknoten dieses Elements in Dokumentreihenfolge.
    /// </returns>
    public IEnumerable<XNode> DescendantNodesAndSelf()
    {
      return GetDescendantNodes(true);
    }

    /// <summary>
    /// Gibt eine Auflistung von Elementen mit diesem Element und allen Nachfolgerelementen dieses Elements in Dokumentreihenfolge zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/> von Elementen mit diesem Element und allen Nachfolgerelementen dieses Elements in Dokumentreihenfolge.
    /// </returns>
    public IEnumerable<XElement> DescendantsAndSelf()
    {
      return GetDescendants(null, true);
    }

    /// <summary>
    /// Gibt eine gefilterte Auflistung von Elementen mit diesem Element und allen Nachfolgerelementen dieses Elements in Dokumentreihenfolge zurück. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/> mit diesem Element und allen Nachfolgerelementen dieses Elements in Dokumentreihenfolge. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </returns>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/>, mit dem eine Übereinstimmung gefunden werden soll.</param>
    public IEnumerable<XElement> DescendantsAndSelf(XName name)
    {
      if (!(name != null))
        return EmptySequence;
      return GetDescendants(name, true);
    }

    /// <summary>
    /// Ruft den Standard-<see cref="T:System.Xml.Linq.XNamespace"/> dieses <see cref="T:System.Xml.Linq.XElement"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XNamespace"/>, der den Standardnamespace dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public XNamespace GetDefaultNamespace()
    {
      string namespaceOfPrefixInScope = GetNamespaceOfPrefixInScope("xmlns", null);
      if (namespaceOfPrefixInScope == null)
        return XNamespace.None;
      return XNamespace.Get(namespaceOfPrefixInScope);
    }

    /// <summary>
    /// Ruft den Namespace ab, der einem bestimmten Präfix für dieses <see cref="T:System.Xml.Linq.XElement"/> zugeordnet ist.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XNamespace"/> für den Namespace, der dem Präfix für dieses <see cref="T:System.Xml.Linq.XElement"/> zugeordnet ist.
    /// </returns>
    /// <param name="prefix">Eine Zeichenfolge, die das zu suchende Namespacepräfix enthält.</param><filterpriority>2</filterpriority>
    public XNamespace GetNamespaceOfPrefix(string prefix)
    {
      if (prefix == null)
        throw new ArgumentNullException("prefix");
      if (prefix.Length == 0)
        throw new ArgumentException("Argument_InvalidPrefix");
      if (prefix == "xmlns")
        return XNamespace.Xmlns;
      string namespaceOfPrefixInScope = GetNamespaceOfPrefixInScope(prefix, null);
      if (namespaceOfPrefixInScope != null)
        return XNamespace.Get(namespaceOfPrefixInScope);
      if (prefix == "xml")
        return XNamespace.Xml;
      return null;
    }

    /// <summary>
    /// Ruft das Präfix ab, das einem Namespace für dieses <see cref="T:System.Xml.Linq.XElement"/> zugeordnet ist.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der das Namespacepräfix enthält.
    /// </returns>
    /// <param name="ns">Ein <see cref="T:System.Xml.Linq.XNamespace"/>, der gesucht werden soll.</param><filterpriority>2</filterpriority>
    public string GetPrefixOfNamespace(XNamespace ns)
    {
      if (ns == null)
        throw new ArgumentNullException("ns");
      string namespaceName = ns.NamespaceName;
      bool flag1 = false;
      var outOfScope = this;
      do
      {
        var xattribute = outOfScope.lastAttr;
        if (xattribute != null)
        {
          bool flag2 = false;
          do
          {
            xattribute = xattribute.next;
            if (xattribute.IsNamespaceDeclaration)
            {
              if (xattribute.Value == namespaceName && xattribute.Name.NamespaceName.Length != 0 && (!flag1 || GetNamespaceOfPrefixInScope(xattribute.Name.LocalName, outOfScope) == null))
                return xattribute.Name.LocalName;
              flag2 = true;
            }
          }
          while (xattribute != outOfScope.lastAttr);
          flag1 |= flag2;
        }
        outOfScope = outOfScope.parent as XElement;
      }
      while (outOfScope != null);
      if (namespaceName == "http://www.w3.org/XML/1998/namespace")
      {
        if (!flag1 || GetNamespaceOfPrefixInScope("xml", null) == null)
          return "xml";
      }
      else if (namespaceName == "http://www.w3.org/2000/xmlns/")
        return "xmlns";
      return null;
    }

    /// <summary>
    /// Lädt ein <see cref="T:System.Xml.Linq.XElement"/> aus einer Datei, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/> mit dem Inhalt der angegebenen Datei.
    /// </returns>
    /// <param name="uri">Eine URI-Zeichenfolge, die auf die Datei verweist, die in ein <see cref="T:System.Xml.Linq.XElement"/> geladen werden soll.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das Leerraumverhalten angibt und festlegt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XElement Load(string uri, LoadOptions options = LoadOptions.None)
    {
      var xmlReaderSettings = GetXmlReaderSettings(options);
      using (var reader = XmlReader.Create(uri, xmlReaderSettings))
        return Load(reader, options);
    }

    /// <summary>
    /// Erstellt mithilfe des angegebenen Streams eine neue <see cref="T:System.Xml.Linq.XElement"/>-Instanz, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/>-Objekt, mit dem die im Stream enthaltenen Daten gelesen werden.
    /// </returns>
    /// <param name="stream">Der Stream, der die XML-Daten enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>-Objekt, das angibt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XElement Load(Stream stream, LoadOptions options = LoadOptions.None)
    {
      var xmlReaderSettings = GetXmlReaderSettings(options);
      using (var reader = XmlReader.Create(stream, xmlReaderSettings))
        return Load(reader, options);
    }

    /// <summary>
    /// Lädt ein <see cref="T:System.Xml.Linq.XElement"/> aus einem <see cref="T:System.IO.TextReader"/>, wobei optional Leerraum und Zeileninformationen beibehalten werden.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/> mit dem XML, das aus dem angegebenen <see cref="T:System.IO.TextReader"/> gelesen wurde.
    /// </returns>
    /// <param name="textReader">Ein <see cref="T:System.IO.TextReader"/>, dessen <see cref="T:System.Xml.Linq.XElement"/>-Inhalt gelesen wird.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das Leerraumverhalten angibt und festlegt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XElement Load(TextReader textReader, LoadOptions options = LoadOptions.None)
    {
      var xmlReaderSettings = GetXmlReaderSettings(options);
      using (var reader = XmlReader.Create(textReader, xmlReaderSettings))
        return Load(reader, options);
    }

    /// <summary>
    /// Lädt ein <see cref="T:System.Xml.Linq.XElement"/> aus einem <see cref="T:System.Xml.XmlReader"/>, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/>, das die XML-Daten enthält, die aus dem angegebenen <see cref="T:System.Xml.XmlReader"/> gelesen wurden.
    /// </returns>
    /// <param name="reader">Ein <see cref="T:System.Xml.XmlReader"/>, der zum Ermitteln des Inhalts von <see cref="T:System.Xml.Linq.XElement"/> gelesen wird.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das Leerraumverhalten angibt und festlegt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XElement Load(XmlReader reader, LoadOptions options = LoadOptions.None)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (reader.MoveToContent() != XmlNodeType.Element)
        throw new InvalidOperationException("InvalidOperation_ExpectedNodeType");
      var xelement = new XElement(reader, options);
      reader.MoveToContent();
      if (!reader.EOF)
        throw new InvalidOperationException("InvalidOperation_ExpectedEndOfFile");
      return xelement;
    }

    /// <summary>
    /// Lädt ein <see cref="T:System.Xml.Linq.XElement"/> aus einer Zeichenfolge, die XML enthält, wobei optional Leerraum und Zeileninformationen beibehalten werden.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/>, das aus der Zeichenfolge aufgefüllt wird, die XML enthält.
    /// </returns>
    /// <param name="text">Ein <see cref="T:System.String"/>, der XML enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das Leerraumverhalten angibt und festlegt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XElement Parse(string text, LoadOptions options = LoadOptions.None)
    {
      using (var stringReader = new StringReader(text))
      {
        var xmlReaderSettings = GetXmlReaderSettings(options);
        using (var reader = XmlReader.Create(stringReader, xmlReaderSettings))
          return Load(reader, options);
      }
    }

    /// <summary>
    /// Entfernt Knoten und Attribute aus diesem <see cref="T:System.Xml.Linq.XElement"/>.
    /// </summary>
    public void RemoveAll()
    {
      RemoveAttributes();
      RemoveNodes();
    }

    /// <summary>
    /// Entfernt die Attribute dieses <see cref="T:System.Xml.Linq.XElement"/>.
    /// </summary>
    public void RemoveAttributes()
    {
      if (SkipNotify())
      {
        RemoveAttributesSkipNotify();
      }
      else
      {
        while (lastAttr != null)
        {
          var xattribute = lastAttr.next;
          NotifyChanging(xattribute, XObjectChangeEventArgs.Remove);
          if (lastAttr == null || xattribute != lastAttr.next)
            throw new InvalidOperationException("InvalidOperation_ExternalCode");
          if (xattribute != lastAttr)
            lastAttr.next = xattribute.next;
          else
            lastAttr = null;
          xattribute.parent = null;
          xattribute.next = null;
          NotifyChanged(xattribute, XObjectChangeEventArgs.Remove);
        }
      }
    }

    /// <summary>
    /// Ersetzt die untergeordneten Knoten und die Attribute dieses Elements durch den angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Der Inhalt, durch den die untergeordneten Knoten und die Attribute dieses Elements ersetzt werden.</param>
    public void ReplaceAll(object content)
    {
      content = GetContentSnapshot(content);
      RemoveAll();
      Add(content);
    }

    /// <summary>
    /// Ersetzt die untergeordneten Knoten und die Attribute dieses Elements durch den angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Eine Parameterliste von Inhaltsobjekten.</param>
    public void ReplaceAll(params object[] content)
    {
      ReplaceAll((object)content);
    }

    /// <summary>
    /// Ersetzt die Attribute dieses Elements durch den angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Der Inhalt, durch den die Attribute dieses Elements ersetzt werden.</param>
    public void ReplaceAttributes(object content)
    {
      content = GetContentSnapshot(content);
      RemoveAttributes();
      Add(content);
    }

    /// <summary>
    /// Ersetzt die Attribute dieses Elements durch den angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Eine Parameterliste von Inhaltsobjekten.</param>
    public void ReplaceAttributes(params object[] content)
    {
      ReplaceAttributes((object)content);
    }

    /// <summary>
    /// Serialisiert dieses Element in eine Datei.
    /// </summary>
    /// <param name="fileName">Ein <see cref="T:System.String"/>, der den Namen der Datei enthält.</param>
    public void Save(string fileName)
    {
      Save(fileName, GetSaveOptionsFromAnnotations());
    }

    /// <summary>
    /// Serialisiert dieses Element in eine Datei, wobei optional die Formatierung deaktiviert wird.
    /// </summary>
    /// <param name="fileName">Ein <see cref="T:System.String"/>, der den Namen der Datei enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>, das Formatierungsverhalten angibt.</param>
    public void Save(string fileName, SaveOptions options)
    {
      var xmlWriterSettings = GetXmlWriterSettings(options);
      using (var writer = XmlWriter.Create(fileName, xmlWriterSettings))
        Save(writer);
    }

    /// <summary>
    /// Gibt diesen <see cref="T:System.Xml.Linq.XElement"/> an den angegebenen <see cref="T:System.IO.Stream"/> aus.
    /// </summary>
    /// <param name="stream">Der Stream, in den dieses <see cref="T:System.Xml.Linq.XElement"/> ausgegeben werden soll.</param>
    public void Save(Stream stream)
    {
      Save(stream, GetSaveOptionsFromAnnotations());
    }

    /// <summary>
    /// Gibt dieses <see cref="T:System.Xml.Linq.XElement"/> zum angegebenen <see cref="T:System.IO.Stream"/> aus und gibt Formatierungsverhalten optional an.
    /// </summary>
    /// <param name="stream">Der Stream, in den dieses <see cref="T:System.Xml.Linq.XElement"/> ausgegeben werden soll.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>-Objekt, das das Formatierungsverhalten angibt.</param>
    public void Save(Stream stream, SaveOptions options)
    {
      var xmlWriterSettings = GetXmlWriterSettings(options);
      using (var writer = XmlWriter.Create(stream, xmlWriterSettings))
        Save(writer);
    }

    /// <summary>
    /// Serialisiert dieses Element in einem <see cref="T:System.IO.TextWriter"/>.
    /// </summary>
    /// <param name="textWriter">Ein <see cref="T:System.IO.TextWriter"/>, in den das <see cref="T:System.Xml.Linq.XElement"/> geschrieben wird.</param>
    public void Save(TextWriter textWriter)
    {
      Save(textWriter, GetSaveOptionsFromAnnotations());
    }

    /// <summary>
    /// Serialisiert dieses Element in einen <see cref="T:System.IO.TextWriter"/>, wobei optional die Formatierung deaktiviert wird.
    /// </summary>
    /// <param name="textWriter">Der <see cref="T:System.IO.TextWriter"/>, an den das XML ausgegeben werden soll.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>, das Formatierungsverhalten angibt.</param>
    public void Save(TextWriter textWriter, SaveOptions options)
    {
      var xmlWriterSettings = GetXmlWriterSettings(options);
      using (var writer = XmlWriter.Create(textWriter, xmlWriterSettings))
        Save(writer);
    }

    /// <summary>
    /// Serialisiert dieses Element in einem <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Ein <see cref="T:System.Xml.XmlWriter"/>, in den das <see cref="T:System.Xml.Linq.XElement"/> geschrieben wird.</param>
    public void Save(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      writer.WriteStartDocument();
      WriteTo(writer);
      writer.WriteEndDocument();
    }

    /// <summary>
    /// Legt den Wert eines Attributs fest, fügt ein Attribut hinzu oder entfernt ein Attribut.
    /// </summary>
    /// <param name="name">Ein <see cref="T:System.Xml.Linq.XName"/>, der den Namen des zu ändernden Attributs enthält.</param><param name="value">Der Wert, der dem Attribut zugewiesen werden soll. Das Attribut wird entfernt, wenn der Wert null ist. Andernfalls wird der Wert in seine Zeichenfolgenentsprechung konvertiert und der <see cref="P:System.Xml.Linq.XAttribute.Value"/>-Eigenschaft des Attributs zugewiesen.</param><exception cref="T:System.ArgumentException">Der <paramref name="value"/> ist eine Instanz von <see cref="T:System.Xml.Linq.XObject"/></exception>
    public void SetAttributeValue(XName name, object value)
    {
      var a = Attribute(name);
      if (value == null)
      {
        if (a == null)
          return;
        RemoveAttribute(a);
      }
      else if (a != null)
        a.Value = GetStringValue(value);
      else
        AppendAttribute(new XAttribute(name, value));
    }

    /// <summary>
    /// Legt den Wert eines untergeordneten Elements fest, fügt ein untergeordnetes Element hinzu oder entfernt ein untergeordnetes Element.
    /// </summary>
    /// <param name="name">Ein <see cref="T:System.Xml.Linq.XName"/>, der den Namen des untergeordneten Elements enthält, das geändert werden soll.</param><param name="value">Der dem untergeordneten Element zuzuweisende Wert. Das untergeordnete Element wird entfernt, wenn der Wert null ist. Andernfalls wird der Wert in seine Zeichenfolgenentsprechung konvertiert und der <see cref="P:System.Xml.Linq.XElement.Value"/>-Eigenschaft des untergeordneten Elements zugewiesen.</param><exception cref="T:System.ArgumentException">Der <paramref name="value"/> ist eine Instanz von <see cref="T:System.Xml.Linq.XObject"/></exception>
    public void SetElementValue(XName name, object value)
    {
      var xelement = Element(name);
      if (value == null)
      {
        if (xelement == null)
          return;
        RemoveNode(xelement);
      }
      else if (xelement != null)
        xelement.Value = GetStringValue(value);
      else
        AddNode(new XElement(name, GetStringValue(value)));
    }

    /// <summary>
    /// Legt den Wert dieses Elements fest.
    /// </summary>
    /// <param name="value">Der diesem Element zuzuweisende Wert. Der Wert wird in seine Zeichenfolgenentsprechung konvertiert und der <see cref="P:System.Xml.Linq.XElement.Value"/>-Eigenschaft zugewiesen.</param><exception cref="T:System.ArgumentNullException">Die <paramref name="value"/> ist null.</exception><exception cref="T:System.ArgumentException">Der <paramref name="value"/> ist ein <see cref="T:System.Xml.Linq.XObject"/>.</exception>
    public void SetValue(object value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      Value = GetStringValue(value);
    }

    /// <summary>
    /// Schreibt dieses Element in einen <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Ein <see cref="T:System.Xml.XmlWriter"/>, in den diese Methode schreibt.</param><filterpriority>2</filterpriority>
    public override void WriteTo(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      new ElementWriter(writer).WriteElement(this);
    }

    XmlSchema IXmlSerializable.GetSchema()
    {
      return null;
    }

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (parent != null || annotations != null || (content != null || lastAttr != null))
        throw new InvalidOperationException("InvalidOperation_DeserializeInstance");
      if (reader.MoveToContent() != XmlNodeType.Element)
        throw new InvalidOperationException("InvalidOperation_ExpectedNodeType");
      ReadElementFrom(reader, LoadOptions.None);
    }

    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
      WriteTo(writer);
    }

    internal override void AddAttribute(XAttribute a)
    {
      if (Attribute(a.Name) != null)
        throw new InvalidOperationException("InvalidOperation_DuplicateAttribute");
      if (a.parent != null)
        a = new XAttribute(a);
      AppendAttribute(a);
    }

    internal override void AddAttributeSkipNotify(XAttribute a)
    {
      if (Attribute(a.Name) != null)
        throw new InvalidOperationException("InvalidOperation_DuplicateAttribute");
      if (a.parent != null)
        a = new XAttribute(a);
      AppendAttributeSkipNotify(a);
    }

    internal void AppendAttribute(XAttribute a)
    {
      bool flag = NotifyChanging(a, XObjectChangeEventArgs.Add);
      if (a.parent != null)
        throw new InvalidOperationException("InvalidOperation_ExternalCode");
      AppendAttributeSkipNotify(a);
      if (!flag)
        return;
      NotifyChanged(a, XObjectChangeEventArgs.Add);
    }

    internal void AppendAttributeSkipNotify(XAttribute a)
    {
      a.parent = this;
      if (lastAttr == null)
      {
        a.next = a;
      }
      else
      {
        a.next = lastAttr.next;
        lastAttr.next = a;
      }
      lastAttr = a;
    }

    bool AttributesEqual(XElement e)
    {
      var xattribute1 = lastAttr;
      var xattribute2 = e.lastAttr;
      if (xattribute1 != null && xattribute2 != null)
      {
        do
        {
          xattribute1 = xattribute1.next;
          xattribute2 = xattribute2.next;
          if (xattribute1.name != xattribute2.name || xattribute1.value != xattribute2.value)
            return false;
        }
        while (xattribute1 != lastAttr);
        return xattribute2 == e.lastAttr;
      }
      if (xattribute1 == null)
        return xattribute2 == null;
      return false;
    }

    internal override XNode CloneNode()
    {
      return new XElement(this);
    }

    internal override bool DeepEquals(XNode node)
    {
      var e = node as XElement;
      if (e != null && name == e.name && ContentsEqual(e))
        return AttributesEqual(e);
      return false;
    }

    IEnumerable<XAttribute> GetAttributes(XName name)
    {
      var a = lastAttr;
      if (a != null)
      {
        do
        {
          a = a.next;
          if (name == null || a.name == name)
            yield return a;
        }
        while (a.parent == this && a != lastAttr);
      }
    }

    string GetNamespaceOfPrefixInScope(string prefix, XElement outOfScope)
    {
      for (var xelement = this; xelement != outOfScope; xelement = xelement.parent as XElement)
      {
        var xattribute = xelement.lastAttr;
        if (xattribute != null)
        {
          do
          {
            xattribute = xattribute.next;
            if (xattribute.IsNamespaceDeclaration && xattribute.Name.LocalName == prefix)
              return xattribute.Value;
          }
          while (xattribute != xelement.lastAttr);
        }
      }
      return null;
    }

    internal override int GetDeepHashCode()
    {
      int num = name.GetHashCode() ^ ContentsHashCode();
      var xattribute = lastAttr;
      if (xattribute != null)
      {
        do
        {
          xattribute = xattribute.next;
          num ^= xattribute.GetDeepHashCode();
        }
        while (xattribute != lastAttr);
      }
      return num;
    }

    void ReadElementFrom(XmlReader r, LoadOptions o)
    {
      if (r.ReadState != ReadState.Interactive)
        throw new InvalidOperationException("InvalidOperation_ExpectedInteractive");
      name = XNamespace.Get(r.NamespaceURI).GetName(r.LocalName);
      if ((o & LoadOptions.SetBaseUri) != LoadOptions.None)
      {
        string baseUri = r.BaseURI;
        if (!string.IsNullOrEmpty(baseUri))
          SetBaseUri(baseUri);
      }
      var xmlLineInfo = (IXmlLineInfo)null;
      if ((o & LoadOptions.SetLineInfo) != LoadOptions.None)
      {
        xmlLineInfo = r as IXmlLineInfo;
        if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
          SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
      }
      if (r.MoveToFirstAttribute())
      {
        do
        {
          var a = new XAttribute(XNamespace.Get(r.Prefix.Length == 0 ? string.Empty : r.NamespaceURI).GetName(r.LocalName), r.Value);
          if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
            a.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
          AppendAttributeSkipNotify(a);
        }
        while (r.MoveToNextAttribute());
        r.MoveToElement();
      }
      if (!r.IsEmptyElement)
      {
        r.Read();
        ReadContentFrom(r, o);
      }
      r.Read();
    }

    internal void RemoveAttribute(XAttribute a)
    {
      bool flag = NotifyChanging(a, XObjectChangeEventArgs.Remove);
      if (a.parent != this)
        throw new InvalidOperationException("InvalidOperation_ExternalCode");
      var xattribute1 = lastAttr;
      XAttribute xattribute2;
      while ((xattribute2 = xattribute1.next) != a)
        xattribute1 = xattribute2;
      if (xattribute1 == a)
      {
        lastAttr = null;
      }
      else
      {
        if (lastAttr == a)
          lastAttr = xattribute1;
        xattribute1.next = a.next;
      }
      a.parent = null;
      a.next = null;
      if (!flag)
        return;
      NotifyChanged(a, XObjectChangeEventArgs.Remove);
    }

    void RemoveAttributesSkipNotify()
    {
      if (lastAttr == null)
        return;
      var xattribute1 = lastAttr;
      do
      {
        var xattribute2 = xattribute1.next;
        xattribute1.parent = null;
        xattribute1.next = null;
        xattribute1 = xattribute2;
      }
      while (xattribute1 != lastAttr);
      lastAttr = null;
    }

    internal void SetEndElementLineInfo(int lineNumber, int linePosition)
    {
      AddAnnotation(new LineInfoEndElementAnnotation(lineNumber, linePosition));
    }

    internal override void ValidateNode(XNode node, XNode previous)
    {
      if (node is XDocument)
        throw new ArgumentException("Argument_AddNode");
      if (node is XDocumentType)
        throw new ArgumentException("Argument_AddNode");
    }
  }
}
