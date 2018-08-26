using System;
using System.Runtime.Serialization;
using System.Xml;

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt den Namen eines XML-Elements oder -Attributs dar.
  /// </summary>
  [Serializable]
  public sealed class XName : IEquatable<XName>, ISerializable
  {
    private XNamespace ns;
    private string localName;
    private int hashCode;

    /// <summary>
    /// Ruft den lokalen (nicht qualifizierten) Teil des Namens ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der den lokalen (nicht qualifizierten) Teil des Namens enthält.
    /// </returns>
    public string LocalName
    {
      get
      {
        return this.localName;
      }
    }

    /// <summary>
    /// Ruft den Namespaceteil des vollqualifizierten Namens ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XNamespace"/>, der den Namespaceteil des Namens enthält.
    /// </returns>
    public XNamespace Namespace
    {
      get
      {
        return this.ns;
      }
    }

    /// <summary>
    /// Gibt den URI des <see cref="T:System.Xml.Linq.XNamespace"/> für diesen <see cref="T:System.Xml.Linq.XName"/> zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der URI des <see cref="T:System.Xml.Linq.XNamespace"/> für diesen <see cref="T:System.Xml.Linq.XName"/>.
    /// </returns>
    public string NamespaceName
    {
      get
      {
        return this.ns.NamespaceName;
      }
    }

    internal XName(XNamespace ns, string localName)
    {
      this.ns = ns;
      this.localName = XmlConvert.VerifyNCName(localName);
      this.hashCode = ns.GetHashCode() ^ localName.GetHashCode();
    }

    /// <summary>
    /// Konvertiert eine als erweiterter XML-Name (d. h. {namespace}localname) formatierte Zeichenfolge in ein <see cref="T:System.Xml.Linq.XName"/>-Objekt.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XName"/>-Objekt, das aus dem erweiterten Namen erstellt wurde.
    /// </returns>
    /// <param name="expandedName">Eine Zeichenfolge, die einen erweiterten XML-Namen im Format {namespace}localname enthält.</param>
    [CLSCompliant(false)]
    public static implicit operator XName(string expandedName)
    {
      if (expandedName == null)
        return (XName)null;
      return XName.Get(expandedName);
    }

    /// <summary>
    /// Gibt einen Wert zurück, der angibt, ob zwei Instanzen von <see cref="T:System.Xml.Linq.XName"/> gleich sind.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn <paramref name="left"/> und <paramref name="right"/> gleich sind, andernfalls false.
    /// </returns>
    /// <param name="left">Das erste zu vergleichende <see cref="T:System.Xml.Linq.XName"/>.</param><param name="right">Das zweite zu vergleichende <see cref="T:System.Xml.Linq.XName"/>.</param>
    public static bool operator ==(XName left, XName right)
    {
      return left == right;
    }

    /// <summary>
    /// Gibt einen Wert zurück, der angibt, ob zwei Instanzen von <see cref="T:System.Xml.Linq.XName"/> ungleich sind.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn <paramref name="left"/> und <paramref name="right"/> ungleich sind, andernfalls false.
    /// </returns>
    /// <param name="left">Das erste zu vergleichende <see cref="T:System.Xml.Linq.XName"/>.</param><param name="right">Das zweite zu vergleichende <see cref="T:System.Xml.Linq.XName"/>.</param>
    public static bool operator !=(XName left, XName right)
    {
      return left != right;
    }

    /// <summary>
    /// Gibt den erweiterten XML-Namen im Format {namespace}localname zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Eine <see cref="T:System.String"/>, die den erweiterten XML-Namen im Format {namespace}localname enthält.
    /// </returns>
    public override string ToString()
    {
      if (this.ns.NamespaceName.Length == 0)
        return this.localName;
      return "{" + this.ns.NamespaceName + "}" + this.localName;
    }

    /// <summary>
    /// Ruft ein <see cref="T:System.Xml.Linq.XName"/>-Objekt aus einem erweiterten Namen ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XName"/>-Objekt, das aus dem erweiterten Namen erstellt wurde.
    /// </returns>
    /// <param name="expandedName">Eine <see cref="T:System.String"/>, die einen erweiterten XML-Namen im Format {namespace}localname enthält.</param>
    public static XName Get(string expandedName)
    {
      if (expandedName == null)
        throw new ArgumentNullException("expandedName");
      if (expandedName.Length == 0)
        throw new ArgumentException(Res.GetString("Argument_InvalidExpandedName", new object[1]
        {
          (object) expandedName
        }));
      if ((int)expandedName[0] != 123)
        return XNamespace.None.GetName(expandedName);
      int num = expandedName.LastIndexOf('}');
      if (num <= 1 || num == expandedName.Length - 1)
        throw new ArgumentException(Res.GetString("Argument_InvalidExpandedName", new object[1]
        {
          (object) expandedName
        }));
      return XNamespace.Get(expandedName, 1, num - 1).GetName(expandedName, num + 1, expandedName.Length - num - 1);
    }

    /// <summary>
    /// Ruft ein <see cref="T:System.Xml.Linq.XName"/>-Objekt aus einem lokalen Namen und einem Namespace ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein aus dem angegebenen lokalen Namen und Namespace erstelltes <see cref="T:System.Xml.Linq.XName"/>-Objekt.
    /// </returns>
    /// <param name="localName">Ein lokaler (nicht qualifizierter) Name.</param><param name="namespaceName">Ein XML-Namespace.</param>
    public static XName Get(string localName, string namespaceName)
    {
      return XNamespace.Get(namespaceName).GetName(localName);
    }

    /// <summary>
    /// Bestimmt, ob der angegebene <see cref="T:System.Xml.Linq.XName"/> und dieser <see cref="T:System.Xml.Linq.XName"/> gleich sind.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn der angegebene <see cref="T:System.Xml.Linq.XName"/> und der aktuelle <see cref="T:System.Xml.Linq.XName"/> gleich sind, andernfalls false.
    /// </returns>
    /// <param name="obj">Der <see cref="T:System.Xml.Linq.XName"/>, der mit dem aktuellen <see cref="T:System.Xml.Linq.XName"/> verglichen werden soll.</param>
    public override bool Equals(object obj)
    {
      return this == obj;
    }

    /// <summary>
    /// Ruft einen Hashcode für diesen <see cref="T:System.Xml.Linq.XName"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Int32"/>, das den Hashcode für den <see cref="T:System.Xml.Linq.XName"/> enthält.
    /// </returns>
    public override int GetHashCode()
    {
      return this.hashCode;
    }

    bool IEquatable<XName>.Equals(XName other)
    {
      return this == other;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException("info");
      info.AddValue("name", (object)this.ToString());
      info.SetType(typeof(NameSerializer));
    }
  }
}
