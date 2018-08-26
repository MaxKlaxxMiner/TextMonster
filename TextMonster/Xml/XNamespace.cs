using System;
using System.Threading;

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt einen XML-Namespace dar. Diese Klasse kann nicht vererbt werden.
  /// </summary>
  public sealed class XNamespace
  {
    static XHashtable<WeakReference> namespaces;
    static WeakReference refNone;
    static WeakReference refXml;
    static WeakReference refXmlns;
    readonly string namespaceName;
    readonly int hashCode;
    readonly XHashtable<XName> names;

    /// <summary>
    /// Ruft den URI (Uniform Resource Identifier) dieses Namespaces ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der den URI des Namespaces enthält.
    /// </returns>
    public string NamespaceName
    {
      get
      {
        return namespaceName;
      }
    }

    /// <summary>
    /// Ruft das <see cref="T:System.Xml.Linq.XNamespace"/>-Objekt ab, das keinem Namespace entspricht.
    /// </summary>
    /// 
    /// <returns>
    /// Der <see cref="T:System.Xml.Linq.XNamespace"/>, der keinem Namespace entspricht.
    /// </returns>
    public static XNamespace None
    {
      get
      {
        return EnsureNamespace(ref refNone, string.Empty);
      }
    }

    /// <summary>
    /// Ruft das <see cref="T:System.Xml.Linq.XNamespace"/>-Objekt ab, das dem XML-URI (http://www.w3.org/XML/1998/namespace) entspricht.
    /// </summary>
    /// 
    /// <returns>
    /// Das <see cref="T:System.Xml.Linq.XNamespace"/>, das dem XML-URI (http://www.w3.org/XML/1998/namespace) entspricht.
    /// </returns>
    public static XNamespace Xml
    {
      get
      {
        return EnsureNamespace(ref refXml, "http://www.w3.org/XML/1998/namespace");
      }
    }

    /// <summary>
    /// Ruft das <see cref="T:System.Xml.Linq.XNamespace"/>-Objekt ab, das dem xmlns-URI (http://www.w3.org/2000/xmlns/) entspricht.
    /// </summary>
    /// 
    /// <returns>
    /// Der <see cref="T:System.Xml.Linq.XNamespace"/>, der dem xmlns-URI (http://www.w3.org/2000/xmlns/) entspricht.
    /// </returns>
    public static XNamespace Xmlns
    {
      get
      {
        return EnsureNamespace(ref refXmlns, "http://www.w3.org/2000/xmlns/");
      }
    }

    XNamespace(string namespaceName)
    {
      this.namespaceName = namespaceName;
      hashCode = namespaceName.GetHashCode();
      names = new XHashtable<XName>(ExtractLocalName, 8);
    }

    /// <summary>
    /// Konvertiert eine Zeichenfolge mit einem URI (Uniform Resource Identifier) in einen <see cref="T:System.Xml.Linq.XNamespace"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Ein aus der URI-Zeichenfolge erstellter <see cref="T:System.Xml.Linq.XNamespace"/>.
    /// </returns>
    /// <param name="namespaceName">Ein <see cref="T:System.String"/>, der den Namespace-URI enthält.</param>
    [CLSCompliant(false)]
    public static implicit operator XNamespace(string namespaceName)
    {
      if (namespaceName == null)
        return null;
      return Get(namespaceName);
    }

    /// <summary>
    /// Kombiniert ein <see cref="T:System.Xml.Linq.XNamespace"/>-Objekt mit einem lokalen Namen, um einen <see cref="T:System.Xml.Linq.XName"/> zu erstellen.
    /// </summary>
    /// 
    /// <returns>
    /// Der neue <see cref="T:System.Xml.Linq.XName"/>, der aus dem Namespace und dem lokalen Namen erstellt wurde.
    /// </returns>
    /// <param name="ns">Ein <see cref="T:System.Xml.Linq.XNamespace"/>, der den Namespace enthält.</param><param name="localName">Ein <see cref="T:System.String"/>, der den lokalen Namen enthält.</param>
    public static XName operator +(XNamespace ns, string localName)
    {
      if (ns == null)
        throw new ArgumentNullException("ns");
      return ns.GetName(localName);
    }

    /// <summary>
    /// Gibt einen Wert zurück, der angibt, ob zwei Instanzen von <see cref="T:System.Xml.Linq.XNamespace"/> gleich sind.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Boolean"/>, das angibt, ob <paramref name="left"/> und <paramref name="right"/> gleich sind.
    /// </returns>
    /// <param name="left">Das erste zu vergleichende <see cref="T:System.Xml.Linq.XNamespace"/>.</param><param name="right">Das zweite zu vergleichende <see cref="T:System.Xml.Linq.XNamespace"/>.</param>
    public static bool operator ==(XNamespace left, XNamespace right)
    {
      return left == right;
    }

    /// <summary>
    /// Gibt einen Wert zurück, der angibt, ob zwei Instanzen von <see cref="T:System.Xml.Linq.XNamespace"/> ungleich sind.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Boolean"/>, das angibt, ob <paramref name="left"/> und <paramref name="right"/> ungleich sind.
    /// </returns>
    /// <param name="left">Das erste zu vergleichende <see cref="T:System.Xml.Linq.XNamespace"/>.</param><param name="right">Das zweite zu vergleichende <see cref="T:System.Xml.Linq.XNamespace"/>.</param>
    public static bool operator !=(XNamespace left, XNamespace right)
    {
      return left != right;
    }

    /// <summary>
    /// Gibt ein <see cref="T:System.Xml.Linq.XName"/>-Objekt zurück, das aus diesem <see cref="T:System.Xml.Linq.XNamespace"/> und dem angegebenen lokalen Namen erstellt wurde.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XName"/>, der aus diesem <see cref="T:System.Xml.Linq.XNamespace"/> und dem angegebenen lokalen Namen erstellt wurde.
    /// </returns>
    /// <param name="localName">Ein <see cref="T:System.String"/>, der einen lokalen Namen enthält.</param>
    public XName GetName(string localName)
    {
      if (localName == null)
        throw new ArgumentNullException("localName");
      return GetName(localName, 0, localName.Length);
    }

    /// <summary>
    /// Gibt den URI dieses <see cref="T:System.Xml.Linq.XNamespace"/> zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der URI dieses <see cref="T:System.Xml.Linq.XNamespace"/>.
    /// </returns>
    public override string ToString()
    {
      return namespaceName;
    }

    /// <summary>
    /// Ruft einen <see cref="T:System.Xml.Linq.XNamespace"/> für den angegebenen URI (Uniform Resource Identifier) ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein aus dem angegebenen URI erstellter <see cref="T:System.Xml.Linq.XNamespace"/>.
    /// </returns>
    /// <param name="namespaceName">Ein <see cref="T:System.String"/>, der einen Namespace-URI enthält.</param>
    public static XNamespace Get(string namespaceName)
    {
      if (namespaceName == null)
        throw new ArgumentNullException("namespaceName");
      return Get(namespaceName, 0, namespaceName.Length);
    }

    /// <summary>
    /// Bestimmt, ob der angegebene <see cref="T:System.Xml.Linq.XNamespace"/> und der aktuelle <see cref="T:System.Xml.Linq.XNamespace"/> gleich sind.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Boolean"/>, das angibt, ob der angegebene <see cref="T:System.Xml.Linq.XNamespace"/> und der aktuelle <see cref="T:System.Xml.Linq.XNamespace"/> gleich sind.
    /// </returns>
    /// <param name="obj">Der <see cref="T:System.Xml.Linq.XNamespace"/>, der mit dem aktuellen <see cref="T:System.Xml.Linq.XNamespace"/> verglichen werden soll.</param>
    public override bool Equals(object obj)
    {
      return ReferenceEquals(this, obj);
    }

    /// <summary>
    /// Ruft einen Hashcode für diesen <see cref="T:System.Xml.Linq.XNamespace"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Int32"/>-Wert, der den Hashcode für den <see cref="T:System.Xml.Linq.XNamespace"/> enthält.
    /// </returns>
    public override int GetHashCode()
    {
      return hashCode;
    }

    internal XName GetName(string localName, int index, int count)
    {
      XName xname;
      if (names.TryGetValue(localName, index, count, out xname))
        return xname;
      return names.Add(new XName(this, localName.Substring(index, count)));
    }

    internal static XNamespace Get(string namespaceName, int index, int count)
    {
      if (count == 0)
        return None;
      if (namespaces == null)
        Interlocked.CompareExchange(ref namespaces, new XHashtable<WeakReference>(ExtractNamespace, 32), null);
      XNamespace xnamespace;
      do
      {
        WeakReference weakReference;
        if (!namespaces.TryGetValue(namespaceName, index, count, out weakReference))
        {
          if (count == "http://www.w3.org/XML/1998/namespace".Length && string.CompareOrdinal(namespaceName, index, "http://www.w3.org/XML/1998/namespace", 0, count) == 0)
            return Xml;
          if (count == "http://www.w3.org/2000/xmlns/".Length && string.CompareOrdinal(namespaceName, index, "http://www.w3.org/2000/xmlns/", 0, count) == 0)
            return Xmlns;
          weakReference = namespaces.Add(new WeakReference(new XNamespace(namespaceName.Substring(index, count))));
        }
        xnamespace = weakReference != null ? (XNamespace)weakReference.Target : null;
      }
      while (xnamespace == null);
      return xnamespace;
    }

    static string ExtractLocalName(XName n)
    {
      return n.LocalName;
    }

    static string ExtractNamespace(WeakReference r)
    {
      XNamespace xnamespace;
      if (r == null || (xnamespace = (XNamespace)r.Target) == null)
        return null;
      return xnamespace.NamespaceName;
    }

    static XNamespace EnsureNamespace(ref WeakReference refNmsp, string namespaceName)
    {
      XNamespace xnamespace;
      while (true)
      {
        var comparand = refNmsp;
        if (comparand != null)
        {
          xnamespace = (XNamespace)comparand.Target;
          if (xnamespace != null)
            break;
        }
        Interlocked.CompareExchange(ref refNmsp, new WeakReference(new XNamespace(namespaceName)), comparand);
      }
      return xnamespace;
    }
  }
}
