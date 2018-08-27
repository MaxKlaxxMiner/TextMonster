using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace ngMax.Xml
{
  /// <summary>
  /// Leistungs-Version vom XElement
  /// </summary>
  public sealed class X_Element : X_Container
  {
    public string name;
    X_Attribute lastAttr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void AppendAttributeSkipNotify(X_Attribute a)
    {
      a.parent = this;
      a.next = lastAttr;
      lastAttr = a;
    }

    private X_Element(X_Element other)
      : base(other)
    {
      name = other.name;
      var other1 = other.lastAttr;
      if (other1 == null) return;
      do
      {
        other1 = other1.next;
        AppendAttributeSkipNotify(new X_Attribute(other1));
      }
      while (other1 != other.lastAttr);
    }

    internal override X_Node CloneNode()
    {
      return new X_Element(this);
    }

    /// <summary>
    /// gibt ein bestimmtes Xml-Element zurück
    /// </summary>
    /// <param name="elementName">Name des Elementes</param>
    /// <returns>gefundenes Element oder null</returns>
    public X_Element Element(string elementName)
    {
      var n = content as X_Node;
      if (n == null) return null;
      do
      {
        n = n.next;
        var e = n as X_Element;
        if (e != null && e.name == elementName) return e;
      }
      while (n != content);
      return null;
    }

    /// <summary>
    /// gibt alle unteren Elemente zurück
    /// </summary>
    /// <returns>Enumerable mit allen Elementen</returns>
    public IEnumerable<X_Element> Elements()
    {
      var n = content as X_Node;
      if (n == null) yield break;
      do
      {
        n = n.next;
        var e = n as X_Element;
        if (e != null) yield return e;
      } while (n.parent == this && n != content);
    }

    /// <summary>
    /// gibt alle unteren Elemente eines bestimmten Elementes zurück
    /// </summary>
    /// <param name="elementName">Name des Hauptelementes</param>
    /// <returns>Enumerable aller unteren Elemente</returns>
    public IEnumerable<X_Element> Elements(string elementName)
    {
      var n = content as X_Node;
      if (n == null) yield break;
      do
      {
        n = n.next;
        var e = n as X_Element;
        if (e != null && e.name == elementName) yield return e;
      } while (n.parent == this && n != content);
    }

    /// <summary>
    /// gibt den Inhalt eines Elementes zurück
    /// </summary>
    /// <param name="elementName">Name des abzufragenden Elementes</param>
    /// <returns>abgefragter Inhalt oder null wenn nicht gefunden</returns>
    public string GetValue(string elementName)
    {
      var n = content as X_Node;
      if (n == null) return null;
      do
      {
        n = n.next;
        var e = n as X_Element;
        if (e != null && e.name == elementName)
        {
          return e.content as string;
        }
      }
      while (n != content);
      return null;
    }

    /// <summary>
    /// gibt den Inhalt eines Elementes als Int32-Wert zurück
    /// </summary>
    /// <param name="elementName">Name des abzufragenden Elementes</param>
    /// <param name="alternate">Alternativwert, falls das Element nicht vorhanden ist oder der Wert nicht geparst werden kann</param>
    /// <returns>gefundener Wert oder "alternate"</returns>
    public int GetValue(string elementName, int alternate)
    {
      var n = content as X_Node;
      if (n == null) return alternate;
      do
      {
        n = n.next;
        var e = n as X_Element;
        if (e != null && e.name == elementName)
        {
          int result;
          return int.TryParse(e.content as string, out result) ? result : alternate;
        }
      }
      while (n != content);
      return alternate;
    }

    /// <summary>
    /// gibt den Inhalt eines Elementes als Int32-Wert zurück
    /// </summary>
    /// <param name="elementName">Name des abzufragenden Elementes</param>
    /// <param name="alternate">Alternativwert, falls das Element nicht vorhanden ist oder der Wert nicht geparst werden kann</param>
    /// <returns>gefundener Wert oder "alternate"</returns>
    public string GetValue(string elementName, string alternate)
    {
      var n = content as X_Node;
      if (n == null) return alternate;
      do
      {
        n = n.next;
        var e = n as X_Element;
        if (e != null && e.name == elementName)
        {
          return e.content as string ?? alternate;
        }
      }
      while (n != content);
      return alternate;
    }

    /// <summary>
    /// gibt den Inhalt eines Attributes zurück
    /// </summary>
    /// <param name="attributName">Name des Attributes</param>
    /// <param name="alternate">Alternativer Wert, falls das Attribut nicht gefunden wurde</param>
    /// <returns>gefundener Wert oder "alternate"</returns>
    public string GetAttribut(string attributName, string alternate = null)
    {
      for (var attr = lastAttr; attr != null; attr = attr.next)
      {
        if (attr.name == attributName) return attr.value;
      }
      return alternate;
    }

    /// <summary>
    /// gibt den Inhalt eines Attributes als Int32-Wert zurück
    /// </summary>
    /// <param name="attributName">Name des Attributes</param>
    /// <param name="alternate">Alternativwert, falls das Attribut nicht vorhanden ist oder der Wert nicht geparst werden kann</param>
    /// <returns>gefundener Wert oder "alternate"</returns>
    public int GetAttribut(string attributName, int alternate)
    {
      for (var attr = lastAttr; attr != null; attr = attr.next)
      {
        if (attr.name == attributName)
        {
          int result;
          return int.TryParse(attr.value, out result) ? result : alternate;
        }
      }
      return alternate;
    }

    /// <summary>
    /// gibt den Inhalt eines Attributes als Int32-Wert zurück
    /// </summary>
    /// <param name="attributName">Name des Attributes</param>
    /// <param name="alternate">Alternativwert, falls das Attribut nicht vorhanden ist oder der Wert nicht geparst werden kann</param>
    /// <returns>gefundener Wert oder "alternate"</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint GetAttribut(string attributName, uint alternate)
    {
      for (var attr = lastAttr; attr != null; attr = attr.next)
      {
        if (attr.name == attributName)
        {
          uint result = 0;
          string str = attr.value;
          // ReSharper disable once ForCanBeConvertedToForeach
          for (int i = 0; i < str.Length; i++)
          {
            uint z = (uint)(str[i] - '0');
            if (z >= 10) return alternate;
            result = result * 10 + z;
          }
          return result;
        }
      }
      return alternate;
    }

    public bool HasElements
    {
      get
      {
        var n = content as X_Node;
        if (n != null)
        {
          do
          {
            if (n is X_Element) return true;
            n = n.next;
          } while (n != content);
        }
        return false;
      }
    }

    public IEnumerable<X_Element> Descendants(string name)
    {
      return name != null ? GetDescendants(name, false) : Enumerable.Empty<X_Element>();
    }

    internal IEnumerable<X_Element> GetDescendants(string name, bool self)
    {
      if (self)
      {
        var e = this;
        if (name == null || e.name == name) yield return e;
      }
      X_Node n = this;
      X_Container c = this;
      while (true)
      {
        if (c != null && c.content is X_Node)
        {
          n = ((X_Node)c.content).next;
        }
        else
        {
          while (n != this && n == n.parent.content) n = n.parent;
          if (n == this) break;
          n = n.next;
        }
        var e = n as X_Element;
        if (e != null && (name == null || e.name == name)) yield return e;
        c = e;
      }
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="name">Name des neuen Elementes</param>
    public X_Element(string name)
    {
      this.name = name;
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="name">Name des neuen Elementes</param>
    /// <param name="value">Wert, welcher als Inhalt verwendet werden soll</param>
    public X_Element(string name, string value)
    {
      this.name = name;
      content = value;
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="name">Name des neuen Elementes</param>
    /// <param name="subElement">Unter-Elemente, welche hinzugefügt werden sollen</param>
    public X_Element(string name, IEnumerable<X_Element> subElement)
    {
      this.name = name;
      foreach (var el in subElement)
      {
        AddNodeSkipNotify(el);
      }
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="r">XmlReader zum lesen der Daten</param>
    public X_Element(XmlReader r)
    {
      name = r.LocalName;
      if (r.MoveToFirstAttribute())
      {
        do
        {
          var a = new X_Attribute(r.LocalName, r.Value);
          AppendAttributeSkipNotify(a);
        }
        while (r.MoveToNextAttribute());
        r.MoveToElement();
      }
      if (!r.IsEmptyElement)
      {
        r.Read();
        ReadContentFrom(r);
      }
      r.Read();
    }

    /// <summary>
    /// gibt den Wert des Elementes zurück
    /// </summary>
    public string Value
    {
      get
      {
        if (content == null) return string.Empty;
        string str = content as string;
        if (str != null) return str;
        var sb = new StringBuilder();
        // TODO this.AppendText(sb);
        return sb.ToString();
      }
    }

    /// <summary>
    /// schreibt das Element in einen Xml-Writer Stream
    /// </summary>
    /// <param name="xWriter">Stream, wohin das Element geschrieben werden soll</param>
    public void WriteTo(XmlWriter xWriter)
    {
      xWriter.WriteStartElement(name);

      if (lastAttr != null) throw new NotSupportedException("attributes not supported");

      if (HasElements)
      {
        foreach (var el in Elements())
        {
          el.WriteTo(xWriter);
        }
      }
      else
      {
        if (content != null)
        {
          xWriter.WriteString(Value);
        }
      }

      xWriter.WriteEndElement();
    }

    /// <summary>
    /// schreibt das Element in einem StringBuilder
    /// </summary>
    /// <param name="sb">Stringbuilder, wohin das Element geschrieben werden soll</param>
    public void WriteTo(StringBuilder sb)
    {
      sb.Append('<').Append(name);

      if (lastAttr != null) throw new NotSupportedException("attributes not supported");

      if (HasElements)
      {
        sb.Append('>');
        foreach (var el in Elements())
        {
          el.WriteTo(sb);
        }
        sb.Append('<').Append('/').Append(name).Append('>');
      }
      else
      {
        string str = content as string;
        if (string.IsNullOrEmpty(str))
        {
          sb.Append('/').Append('>');
        }
        else
        {
          sb.Append('>');
          for (int i = 0; i < str.Length; i++)
          {
            if (str[i] == '&' || str[i] == '<' || str[i] == '>')
            {
              str = str.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
              break;
            }
          }
          sb.Append(str);
          sb.Append('<').Append('/').Append(name).Append('>');
        }
      }
    }

    /// <summary>
    /// gibt den Inhalt als lesbare Zeichenkette zurück
    /// </summary>
    /// <returns>lesbare Zeichenkette</returns>
    public override string ToString()
    {
      var sb = new StringBuilder();

      WriteTo(sb);

      return sb.ToString();
    }

    /// <summary>
    /// Erstellt mithilfe des angegebenen Streams eine neue <see cref="T:System.Xml.Linq.XElement"/>-Instanz, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/>-Objekt, mit dem die im Stream enthaltenen Daten gelesen werden.
    /// </returns>
    /// <param name="stream">Der Stream, der die XML-Daten enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>-Objekt, das angibt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static X_Element Load(Stream stream, LoadOptions options = LoadOptions.None)
    {
      var xmlReaderSettings = GetXmlReaderSettings(options);
      using (var reader = XmlReader.Create(stream, xmlReaderSettings))
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
    public static X_Element Load(XmlReader reader, LoadOptions options = LoadOptions.None)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (reader.MoveToContent() != XmlNodeType.Element)
        throw new InvalidOperationException("InvalidOperation_ExpectedNodeType");
      var xelement = new X_Element(reader, options);
      reader.MoveToContent();
      if (!reader.EOF)
        throw new InvalidOperationException("InvalidOperation_ExpectedEndOfFile");
      return xelement;
    }

    static XmlReaderSettings GetXmlReaderSettings(LoadOptions o)
    {
      var xmlReaderSettings = new XmlReaderSettings();
      if ((o & LoadOptions.PreserveWhitespace) == LoadOptions.None)
        xmlReaderSettings.IgnoreWhitespace = true;
      xmlReaderSettings.DtdProcessing = DtdProcessing.Parse;
      xmlReaderSettings.MaxCharactersFromEntities = 10000000L;
      xmlReaderSettings.XmlResolver = null;
      return xmlReaderSettings;
    }

    X_Element(XmlReader r, LoadOptions o = LoadOptions.None)
    {
      ReadElementFrom(r, o);
    }

    void ReadElementFrom(XmlReader r, LoadOptions o)
    {
      if (r.ReadState != ReadState.Interactive)
        throw new InvalidOperationException("InvalidOperation_ExpectedInteractive");
      name = r.LocalName;
      if (r.MoveToFirstAttribute())
      {
        do
        {
          var a = new X_Attribute(r.LocalName, r.Value);
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

    void ReadContentFrom(XmlReader r, LoadOptions o)
    {
      ReadContentFrom(r);
    }
  }
}
