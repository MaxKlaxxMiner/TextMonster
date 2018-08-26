
namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt ein XML-Element dar.
  /// </summary>
  public class XElement : XContainer, IXmlSerializable
  {
    private static IEnumerable<XElement> emptySequence;
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
      get
      {
        if (XElement.emptySequence == null)
          XElement.emptySequence = (IEnumerable<XElement>)new XElement[0];
        return XElement.emptySequence;
      }
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
        if (this.lastAttr == null)
          return (XAttribute)null;
        return this.lastAttr.next;
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
        return this.lastAttr != null;
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
        XNode xnode = this.content as XNode;
        if (xnode != null)
        {
          while (!(xnode is XElement))
          {
            xnode = xnode.next;
            if (xnode == this.content)
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
        return this.content == null;
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
        return this.lastAttr;
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
        return this.name;
      }
      set
      {
        if (value == (XName)null)
          throw new ArgumentNullException("value");
        bool flag = this.NotifyChanging((object)this, XObjectChangeEventArgs.Name);
        this.name = value;
        if (!flag)
          return;
        this.NotifyChanged((object)this, XObjectChangeEventArgs.Name);
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
        if (this.content == null)
          return string.Empty;
        string str = this.content as string;
        if (str != null)
          return str;
        StringBuilder sb = new StringBuilder();
        this.AppendText(sb);
        return sb.ToString();
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        this.RemoveNodes();
        this.Add((object)value);
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XElement"/>-Klasse mit dem angegebenen Namen.
    /// </summary>
    /// <param name="name">Ein <see cref="T:System.Xml.Linq.XName"/>, der den Namen des Elements enthält.</param>
    public XElement(XName name)
    {
      if (name == (XName)null)
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
      this.AddContentSkipNotify(content);
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
      : base((XContainer)other)
    {
      this.name = other.name;
      XAttribute other1 = other.lastAttr;
      if (other1 == null)
        return;
      do
      {
        other1 = other1.next;
        this.AppendAttributeSkipNotify(new XAttribute(other1));
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
      this.name = other.name;
      this.AddContentSkipNotify(other.content);
    }

    internal XElement()
      : this((XName)"default")
    {
    }

    internal XElement(XmlReader r)
      : this(r, LoadOptions.None)
    {
    }

    internal XElement(XmlReader r, LoadOptions o)
    {
      this.ReadElementFrom(r, o);
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
        return (string)null;
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
      return new bool?(XmlConvert.ToBoolean(element.Value.ToLower(CultureInfo.InvariantCulture)));
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
      return new int?(XmlConvert.ToInt32(element.Value));
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
      return new uint?(XmlConvert.ToUInt32(element.Value));
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
      return new long?(XmlConvert.ToInt64(element.Value));
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
      return new ulong?(XmlConvert.ToUInt64(element.Value));
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
      return new float?(XmlConvert.ToSingle(element.Value));
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
      return new double?(XmlConvert.ToDouble(element.Value));
    }

    /// <summary>
    /// Wandelt den Wert dieses <see cref="T:System.Xml.Linq.XElement"/> in einen <see cref="T:System.Decimal"/> um.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Decimal"/>, das den Inhalt dieses <see cref="T:System.Xml.Linq.XElement"/> enthält.
    /// </returns>
    /// <param name="element">Das <see cref="T:System.Xml.Linq.XElement"/>, das in <see cref="T:System.Decimal"/> umgewandelt werden soll.</param><exception cref="T:System.FormatException">Das Element enthält keinen gültigen <see cref="T:System.Decimal"/>-Wert.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="element"/>-Parameter ist null.</exception>
    public static explicit operator Decimal(XElement element)
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
    public static explicit operator Decimal?(XElement element)
    {
      if (element == null)
        return new Decimal?();
      return new Decimal?(XmlConvert.ToDecimal(element.Value));
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
      return DateTime.Parse(element.Value, (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
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
      return new DateTime?(DateTime.Parse(element.Value, (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));
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
      return new DateTimeOffset?(XmlConvert.ToDateTimeOffset(element.Value));
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
      return new TimeSpan?(XmlConvert.ToTimeSpan(element.Value));
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
      return new Guid?(XmlConvert.ToGuid(element.Value));
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
      return this.GetAncestors((XName)null, true);
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
      if (!(name != (XName)null))
        return XElement.EmptySequence;
      return this.GetAncestors(name, true);
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
      XAttribute xattribute = this.lastAttr;
      if (xattribute != null)
      {
        do
        {
          xattribute = xattribute.next;
          if (xattribute.name == name)
            return xattribute;
        }
        while (xattribute != this.lastAttr);
      }
      return (XAttribute)null;
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
      return this.GetAttributes((XName)null);
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
      if (!(name != (XName)null))
        return XAttribute.EmptySequence;
      return this.GetAttributes(name);
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
      return this.GetDescendantNodes(true);
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
      return this.GetDescendants((XName)null, true);
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
      if (!(name != (XName)null))
        return XElement.EmptySequence;
      return this.GetDescendants(name, true);
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
      string namespaceOfPrefixInScope = this.GetNamespaceOfPrefixInScope("xmlns", (XElement)null);
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
        throw new ArgumentException(Res.GetString("Argument_InvalidPrefix", new object[1]
        {
          (object) prefix
        }));
      if (prefix == "xmlns")
        return XNamespace.Xmlns;
      string namespaceOfPrefixInScope = this.GetNamespaceOfPrefixInScope(prefix, (XElement)null);
      if (namespaceOfPrefixInScope != null)
        return XNamespace.Get(namespaceOfPrefixInScope);
      if (prefix == "xml")
        return XNamespace.Xml;
      return (XNamespace)null;
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
      if (ns == (XNamespace)null)
        throw new ArgumentNullException("ns");
      string namespaceName = ns.NamespaceName;
      bool flag1 = false;
      XElement outOfScope = this;
      do
      {
        XAttribute xattribute = outOfScope.lastAttr;
        if (xattribute != null)
        {
          bool flag2 = false;
          do
          {
            xattribute = xattribute.next;
            if (xattribute.IsNamespaceDeclaration)
            {
              if (xattribute.Value == namespaceName && xattribute.Name.NamespaceName.Length != 0 && (!flag1 || this.GetNamespaceOfPrefixInScope(xattribute.Name.LocalName, outOfScope) == null))
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
        if (!flag1 || this.GetNamespaceOfPrefixInScope("xml", (XElement)null) == null)
          return "xml";
      }
      else if (namespaceName == "http://www.w3.org/2000/xmlns/")
        return "xmlns";
      return (string)null;
    }

    /// <summary>
    /// Lädt ein <see cref="T:System.Xml.Linq.XElement"/> aus einer Datei.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/> mit dem Inhalt der angegebenen Datei.
    /// </returns>
    /// <param name="uri">Eine URI-Zeichenfolge, die auf die Datei verweist, die in ein neues <see cref="T:System.Xml.Linq.XElement"/> geladen werden soll.</param>
    public static XElement Load(string uri)
    {
      return XElement.Load(uri, LoadOptions.None);
    }

    /// <summary>
    /// Lädt ein <see cref="T:System.Xml.Linq.XElement"/> aus einer Datei, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/> mit dem Inhalt der angegebenen Datei.
    /// </returns>
    /// <param name="uri">Eine URI-Zeichenfolge, die auf die Datei verweist, die in ein <see cref="T:System.Xml.Linq.XElement"/> geladen werden soll.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das Leerraumverhalten angibt und festlegt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XElement Load(string uri, LoadOptions options)
    {
      XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
      using (XmlReader reader = XmlReader.Create(uri, xmlReaderSettings))
        return XElement.Load(reader, options);
    }

    /// <summary>
    /// Erstellt mit dem angegebenen Stream eine neue <see cref="T:System.Xml.Linq.XElement"/>-Instanz.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/>-Objekt, mit dem die im Stream enthaltenen Daten gelesen werden.
    /// </returns>
    /// <param name="stream">Der Stream, der die XML-Daten enthält.</param>
    public static XElement Load(Stream stream)
    {
      return XElement.Load(stream, LoadOptions.None);
    }

    /// <summary>
    /// Erstellt mithilfe des angegebenen Streams eine neue <see cref="T:System.Xml.Linq.XElement"/>-Instanz, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/>-Objekt, mit dem die im Stream enthaltenen Daten gelesen werden.
    /// </returns>
    /// <param name="stream">Der Stream, der die XML-Daten enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>-Objekt, das angibt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XElement Load(Stream stream, LoadOptions options)
    {
      XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
      using (XmlReader reader = XmlReader.Create(stream, xmlReaderSettings))
        return XElement.Load(reader, options);
    }

    /// <summary>
    /// Lädt ein <see cref="T:System.Xml.Linq.XElement"/> aus einem <see cref="T:System.IO.TextReader"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/> mit dem XML, das aus dem angegebenen <see cref="T:System.IO.TextReader"/> gelesen wurde.
    /// </returns>
    /// <param name="textReader">Ein <see cref="T:System.IO.TextReader"/>, dessen <see cref="T:System.Xml.Linq.XElement"/>-Inhalt gelesen wird.</param>
    public static XElement Load(TextReader textReader)
    {
      return XElement.Load(textReader, LoadOptions.None);
    }

    /// <summary>
    /// Lädt ein <see cref="T:System.Xml.Linq.XElement"/> aus einem <see cref="T:System.IO.TextReader"/>, wobei optional Leerraum und Zeileninformationen beibehalten werden.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/> mit dem XML, das aus dem angegebenen <see cref="T:System.IO.TextReader"/> gelesen wurde.
    /// </returns>
    /// <param name="textReader">Ein <see cref="T:System.IO.TextReader"/>, dessen <see cref="T:System.Xml.Linq.XElement"/>-Inhalt gelesen wird.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das Leerraumverhalten angibt und festlegt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XElement Load(TextReader textReader, LoadOptions options)
    {
      XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
      using (XmlReader reader = XmlReader.Create(textReader, xmlReaderSettings))
        return XElement.Load(reader, options);
    }

    /// <summary>
    /// Lädt ein <see cref="T:System.Xml.Linq.XElement"/> aus einem <see cref="T:System.Xml.XmlReader"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/>, das die XML-Daten enthält, die aus dem angegebenen <see cref="T:System.Xml.XmlReader"/> gelesen wurden.
    /// </returns>
    /// <param name="reader">Ein <see cref="T:System.Xml.XmlReader"/>, der zum Ermitteln des Inhalts von <see cref="T:System.Xml.Linq.XElement"/> gelesen wird.</param>
    public static XElement Load(XmlReader reader)
    {
      return XElement.Load(reader, LoadOptions.None);
    }

    /// <summary>
    /// Lädt ein <see cref="T:System.Xml.Linq.XElement"/> aus einem <see cref="T:System.Xml.XmlReader"/>, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/>, das die XML-Daten enthält, die aus dem angegebenen <see cref="T:System.Xml.XmlReader"/> gelesen wurden.
    /// </returns>
    /// <param name="reader">Ein <see cref="T:System.Xml.XmlReader"/>, der zum Ermitteln des Inhalts von <see cref="T:System.Xml.Linq.XElement"/> gelesen wird.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das Leerraumverhalten angibt und festlegt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XElement Load(XmlReader reader, LoadOptions options)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (reader.MoveToContent() != XmlNodeType.Element)
        throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedNodeType", (object)XmlNodeType.Element, (object)reader.NodeType));
      XElement xelement = new XElement(reader, options);
      int num = (int)reader.MoveToContent();
      if (!reader.EOF)
        throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedEndOfFile"));
      return xelement;
    }

    /// <summary>
    /// Lädt ein <see cref="T:System.Xml.Linq.XElement"/> aus einer Zeichenfolge, die XML enthält.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/>, das aus der Zeichenfolge aufgefüllt wird, die XML enthält.
    /// </returns>
    /// <param name="text">Ein <see cref="T:System.String"/>, der XML enthält.</param>
    public static XElement Parse(string text)
    {
      return XElement.Parse(text, LoadOptions.None);
    }

    /// <summary>
    /// Lädt ein <see cref="T:System.Xml.Linq.XElement"/> aus einer Zeichenfolge, die XML enthält, wobei optional Leerraum und Zeileninformationen beibehalten werden.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/>, das aus der Zeichenfolge aufgefüllt wird, die XML enthält.
    /// </returns>
    /// <param name="text">Ein <see cref="T:System.String"/>, der XML enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das Leerraumverhalten angibt und festlegt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XElement Parse(string text, LoadOptions options)
    {
      using (StringReader stringReader = new StringReader(text))
      {
        XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
        using (XmlReader reader = XmlReader.Create((TextReader)stringReader, xmlReaderSettings))
          return XElement.Load(reader, options);
      }
    }

    /// <summary>
    /// Entfernt Knoten und Attribute aus diesem <see cref="T:System.Xml.Linq.XElement"/>.
    /// </summary>
    public void RemoveAll()
    {
      this.RemoveAttributes();
      this.RemoveNodes();
    }

    /// <summary>
    /// Entfernt die Attribute dieses <see cref="T:System.Xml.Linq.XElement"/>.
    /// </summary>
    public void RemoveAttributes()
    {
      if (this.SkipNotify())
      {
        this.RemoveAttributesSkipNotify();
      }
      else
      {
        while (this.lastAttr != null)
        {
          XAttribute xattribute = this.lastAttr.next;
          this.NotifyChanging((object)xattribute, XObjectChangeEventArgs.Remove);
          if (this.lastAttr == null || xattribute != this.lastAttr.next)
            throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
          if (xattribute != this.lastAttr)
            this.lastAttr.next = xattribute.next;
          else
            this.lastAttr = (XAttribute)null;
          xattribute.parent = (XContainer)null;
          xattribute.next = (XAttribute)null;
          this.NotifyChanged((object)xattribute, XObjectChangeEventArgs.Remove);
        }
      }
    }

    /// <summary>
    /// Ersetzt die untergeordneten Knoten und die Attribute dieses Elements durch den angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Der Inhalt, durch den die untergeordneten Knoten und die Attribute dieses Elements ersetzt werden.</param>
    public void ReplaceAll(object content)
    {
      content = XContainer.GetContentSnapshot(content);
      this.RemoveAll();
      this.Add(content);
    }

    /// <summary>
    /// Ersetzt die untergeordneten Knoten und die Attribute dieses Elements durch den angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Eine Parameterliste von Inhaltsobjekten.</param>
    public void ReplaceAll(params object[] content)
    {
      this.ReplaceAll((object)content);
    }

    /// <summary>
    /// Ersetzt die Attribute dieses Elements durch den angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Der Inhalt, durch den die Attribute dieses Elements ersetzt werden.</param>
    public void ReplaceAttributes(object content)
    {
      content = XContainer.GetContentSnapshot(content);
      this.RemoveAttributes();
      this.Add(content);
    }

    /// <summary>
    /// Ersetzt die Attribute dieses Elements durch den angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Eine Parameterliste von Inhaltsobjekten.</param>
    public void ReplaceAttributes(params object[] content)
    {
      this.ReplaceAttributes((object)content);
    }

    /// <summary>
    /// Serialisiert dieses Element in eine Datei.
    /// </summary>
    /// <param name="fileName">Ein <see cref="T:System.String"/>, der den Namen der Datei enthält.</param>
    public void Save(string fileName)
    {
      this.Save(fileName, this.GetSaveOptionsFromAnnotations());
    }

    /// <summary>
    /// Serialisiert dieses Element in eine Datei, wobei optional die Formatierung deaktiviert wird.
    /// </summary>
    /// <param name="fileName">Ein <see cref="T:System.String"/>, der den Namen der Datei enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>, das Formatierungsverhalten angibt.</param>
    public void Save(string fileName, SaveOptions options)
    {
      XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
      using (XmlWriter writer = XmlWriter.Create(fileName, xmlWriterSettings))
        this.Save(writer);
    }

    /// <summary>
    /// Gibt diesen <see cref="T:System.Xml.Linq.XElement"/> an den angegebenen <see cref="T:System.IO.Stream"/> aus.
    /// </summary>
    /// <param name="stream">Der Stream, in den dieses <see cref="T:System.Xml.Linq.XElement"/> ausgegeben werden soll.</param>
    public void Save(Stream stream)
    {
      this.Save(stream, this.GetSaveOptionsFromAnnotations());
    }

    /// <summary>
    /// Gibt dieses <see cref="T:System.Xml.Linq.XElement"/> zum angegebenen <see cref="T:System.IO.Stream"/> aus und gibt Formatierungsverhalten optional an.
    /// </summary>
    /// <param name="stream">Der Stream, in den dieses <see cref="T:System.Xml.Linq.XElement"/> ausgegeben werden soll.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>-Objekt, das das Formatierungsverhalten angibt.</param>
    public void Save(Stream stream, SaveOptions options)
    {
      XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
      using (XmlWriter writer = XmlWriter.Create(stream, xmlWriterSettings))
        this.Save(writer);
    }

    /// <summary>
    /// Serialisiert dieses Element in einem <see cref="T:System.IO.TextWriter"/>.
    /// </summary>
    /// <param name="textWriter">Ein <see cref="T:System.IO.TextWriter"/>, in den das <see cref="T:System.Xml.Linq.XElement"/> geschrieben wird.</param>
    public void Save(TextWriter textWriter)
    {
      this.Save(textWriter, this.GetSaveOptionsFromAnnotations());
    }

    /// <summary>
    /// Serialisiert dieses Element in einen <see cref="T:System.IO.TextWriter"/>, wobei optional die Formatierung deaktiviert wird.
    /// </summary>
    /// <param name="textWriter">Der <see cref="T:System.IO.TextWriter"/>, an den das XML ausgegeben werden soll.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>, das Formatierungsverhalten angibt.</param>
    public void Save(TextWriter textWriter, SaveOptions options)
    {
      XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
      using (XmlWriter writer = XmlWriter.Create(textWriter, xmlWriterSettings))
        this.Save(writer);
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
      this.WriteTo(writer);
      writer.WriteEndDocument();
    }

    /// <summary>
    /// Legt den Wert eines Attributs fest, fügt ein Attribut hinzu oder entfernt ein Attribut.
    /// </summary>
    /// <param name="name">Ein <see cref="T:System.Xml.Linq.XName"/>, der den Namen des zu ändernden Attributs enthält.</param><param name="value">Der Wert, der dem Attribut zugewiesen werden soll. Das Attribut wird entfernt, wenn der Wert null ist. Andernfalls wird der Wert in seine Zeichenfolgenentsprechung konvertiert und der <see cref="P:System.Xml.Linq.XAttribute.Value"/>-Eigenschaft des Attributs zugewiesen.</param><exception cref="T:System.ArgumentException">Der <paramref name="value"/> ist eine Instanz von <see cref="T:System.Xml.Linq.XObject"/></exception>
    public void SetAttributeValue(XName name, object value)
    {
      XAttribute a = this.Attribute(name);
      if (value == null)
      {
        if (a == null)
          return;
        this.RemoveAttribute(a);
      }
      else if (a != null)
        a.Value = XContainer.GetStringValue(value);
      else
        this.AppendAttribute(new XAttribute(name, value));
    }

    /// <summary>
    /// Legt den Wert eines untergeordneten Elements fest, fügt ein untergeordnetes Element hinzu oder entfernt ein untergeordnetes Element.
    /// </summary>
    /// <param name="name">Ein <see cref="T:System.Xml.Linq.XName"/>, der den Namen des untergeordneten Elements enthält, das geändert werden soll.</param><param name="value">Der dem untergeordneten Element zuzuweisende Wert. Das untergeordnete Element wird entfernt, wenn der Wert null ist. Andernfalls wird der Wert in seine Zeichenfolgenentsprechung konvertiert und der <see cref="P:System.Xml.Linq.XElement.Value"/>-Eigenschaft des untergeordneten Elements zugewiesen.</param><exception cref="T:System.ArgumentException">Der <paramref name="value"/> ist eine Instanz von <see cref="T:System.Xml.Linq.XObject"/></exception>
    public void SetElementValue(XName name, object value)
    {
      XElement xelement = this.Element(name);
      if (value == null)
      {
        if (xelement == null)
          return;
        this.RemoveNode((XNode)xelement);
      }
      else if (xelement != null)
        xelement.Value = XContainer.GetStringValue(value);
      else
        this.AddNode((XNode)new XElement(name, (object)XContainer.GetStringValue(value)));
    }

    /// <summary>
    /// Legt den Wert dieses Elements fest.
    /// </summary>
    /// <param name="value">Der diesem Element zuzuweisende Wert. Der Wert wird in seine Zeichenfolgenentsprechung konvertiert und der <see cref="P:System.Xml.Linq.XElement.Value"/>-Eigenschaft zugewiesen.</param><exception cref="T:System.ArgumentNullException">Die <paramref name="value"/> ist null.</exception><exception cref="T:System.ArgumentException">Der <paramref name="value"/> ist ein <see cref="T:System.Xml.Linq.XObject"/>.</exception>
    public void SetValue(object value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      this.Value = XContainer.GetStringValue(value);
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
      return (XmlSchema)null;
    }

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (this.parent != null || this.annotations != null || (this.content != null || this.lastAttr != null))
        throw new InvalidOperationException(Res.GetString("InvalidOperation_DeserializeInstance"));
      if (reader.MoveToContent() != XmlNodeType.Element)
        throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedNodeType", (object)XmlNodeType.Element, (object)reader.NodeType));
      this.ReadElementFrom(reader, LoadOptions.None);
    }

    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
      this.WriteTo(writer);
    }

    internal override void AddAttribute(XAttribute a)
    {
      if (this.Attribute(a.Name) != null)
        throw new InvalidOperationException(Res.GetString("InvalidOperation_DuplicateAttribute"));
      if (a.parent != null)
        a = new XAttribute(a);
      this.AppendAttribute(a);
    }

    internal override void AddAttributeSkipNotify(XAttribute a)
    {
      if (this.Attribute(a.Name) != null)
        throw new InvalidOperationException(Res.GetString("InvalidOperation_DuplicateAttribute"));
      if (a.parent != null)
        a = new XAttribute(a);
      this.AppendAttributeSkipNotify(a);
    }

    internal void AppendAttribute(XAttribute a)
    {
      bool flag = this.NotifyChanging((object)a, XObjectChangeEventArgs.Add);
      if (a.parent != null)
        throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
      this.AppendAttributeSkipNotify(a);
      if (!flag)
        return;
      this.NotifyChanged((object)a, XObjectChangeEventArgs.Add);
    }

    internal void AppendAttributeSkipNotify(XAttribute a)
    {
      a.parent = (XContainer)this;
      if (this.lastAttr == null)
      {
        a.next = a;
      }
      else
      {
        a.next = this.lastAttr.next;
        this.lastAttr.next = a;
      }
      this.lastAttr = a;
    }

    private bool AttributesEqual(XElement e)
    {
      XAttribute xattribute1 = this.lastAttr;
      XAttribute xattribute2 = e.lastAttr;
      if (xattribute1 != null && xattribute2 != null)
      {
        do
        {
          xattribute1 = xattribute1.next;
          xattribute2 = xattribute2.next;
          if (xattribute1.name != xattribute2.name || xattribute1.value != xattribute2.value)
            return false;
        }
        while (xattribute1 != this.lastAttr);
        return xattribute2 == e.lastAttr;
      }
      if (xattribute1 == null)
        return xattribute2 == null;
      return false;
    }

    internal override XNode CloneNode()
    {
      return (XNode)new XElement(this);
    }

    internal override bool DeepEquals(XNode node)
    {
      XElement e = node as XElement;
      if (e != null && this.name == e.name && this.ContentsEqual((XContainer)e))
        return this.AttributesEqual(e);
      return false;
    }

    private IEnumerable<XAttribute> GetAttributes(XName name)
    {
      XAttribute a = this.lastAttr;
      if (a != null)
      {
        do
        {
          a = a.next;
          if (name == (XName)null || a.name == name)
            yield return a;
        }
        while (a.parent == this && a != this.lastAttr);
      }
    }

    private string GetNamespaceOfPrefixInScope(string prefix, XElement outOfScope)
    {
      for (XElement xelement = this; xelement != outOfScope; xelement = xelement.parent as XElement)
      {
        XAttribute xattribute = xelement.lastAttr;
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
      return (string)null;
    }

    internal override int GetDeepHashCode()
    {
      int num = this.name.GetHashCode() ^ this.ContentsHashCode();
      XAttribute xattribute = this.lastAttr;
      if (xattribute != null)
      {
        do
        {
          xattribute = xattribute.next;
          num ^= xattribute.GetDeepHashCode();
        }
        while (xattribute != this.lastAttr);
      }
      return num;
    }

    private void ReadElementFrom(XmlReader r, LoadOptions o)
    {
      if (r.ReadState != ReadState.Interactive)
        throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedInteractive"));
      this.name = XNamespace.Get(r.NamespaceURI).GetName(r.LocalName);
      if ((o & LoadOptions.SetBaseUri) != LoadOptions.None)
      {
        string baseUri = r.BaseURI;
        if (baseUri != null && baseUri.Length != 0)
          this.SetBaseUri(baseUri);
      }
      IXmlLineInfo xmlLineInfo = (IXmlLineInfo)null;
      if ((o & LoadOptions.SetLineInfo) != LoadOptions.None)
      {
        xmlLineInfo = r as IXmlLineInfo;
        if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
          this.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
      }
      if (r.MoveToFirstAttribute())
      {
        do
        {
          XAttribute a = new XAttribute(XNamespace.Get(r.Prefix.Length == 0 ? string.Empty : r.NamespaceURI).GetName(r.LocalName), (object)r.Value);
          if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
            a.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
          this.AppendAttributeSkipNotify(a);
        }
        while (r.MoveToNextAttribute());
        r.MoveToElement();
      }
      if (!r.IsEmptyElement)
      {
        r.Read();
        this.ReadContentFrom(r, o);
      }
      r.Read();
    }

    internal void RemoveAttribute(XAttribute a)
    {
      bool flag = this.NotifyChanging((object)a, XObjectChangeEventArgs.Remove);
      if (a.parent != this)
        throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
      XAttribute xattribute1 = this.lastAttr;
      XAttribute xattribute2;
      while ((xattribute2 = xattribute1.next) != a)
        xattribute1 = xattribute2;
      if (xattribute1 == a)
      {
        this.lastAttr = (XAttribute)null;
      }
      else
      {
        if (this.lastAttr == a)
          this.lastAttr = xattribute1;
        xattribute1.next = a.next;
      }
      a.parent = (XContainer)null;
      a.next = (XAttribute)null;
      if (!flag)
        return;
      this.NotifyChanged((object)a, XObjectChangeEventArgs.Remove);
    }

    private void RemoveAttributesSkipNotify()
    {
      if (this.lastAttr == null)
        return;
      XAttribute xattribute1 = this.lastAttr;
      do
      {
        XAttribute xattribute2 = xattribute1.next;
        xattribute1.parent = (XContainer)null;
        xattribute1.next = (XAttribute)null;
        xattribute1 = xattribute2;
      }
      while (xattribute1 != this.lastAttr);
      this.lastAttr = (XAttribute)null;
    }

    internal void SetEndElementLineInfo(int lineNumber, int linePosition)
    {
      this.AddAnnotation((object)new LineInfoEndElementAnnotation(lineNumber, linePosition));
    }

    internal override void ValidateNode(XNode node, XNode previous)
    {
      if (node is XDocument)
        throw new ArgumentException(Res.GetString("Argument_AddNode", new object[1]
        {
          (object) XmlNodeType.Document
        }));
      if (node is XDocumentType)
        throw new ArgumentException(Res.GetString("Argument_AddNode", new object[1]
        {
          (object) XmlNodeType.DocumentType
        }));
    }
  }
}
