using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal abstract partial class XmlRawWriter : XmlWriter
  {
    //
    // Fields
    //
    // base64 converter
    protected XmlRawWriterBase64Encoder base64Encoder;

    // namespace resolver
    protected IXmlNamespaceResolver resolver;

    //
    // XmlWriter implementation
    //

    // Raw writers do not have to track whether this is a well-formed document.
    public override void WriteStartDocument()
    {
      throw new InvalidOperationException(Res.GetString(Res.Xml_InvalidOperation));
    }

    public override void WriteEndDocument()
    {
      throw new InvalidOperationException(Res.GetString(Res.Xml_InvalidOperation));
    }

    public override void WriteDocType(string name, string pubid, string sysid, string subset)
    {

    }

    // Raw writers do not have to keep a stack of element names.
    public override void WriteEndElement()
    {
      throw new InvalidOperationException(Res.GetString(Res.Xml_InvalidOperation));
    }

    // Raw writers do not have to keep a stack of element names.
    public override void WriteFullEndElement()
    {
      throw new InvalidOperationException(Res.GetString(Res.Xml_InvalidOperation));
    }

    // By default, convert base64 value to string and call WriteString.
    public override void WriteBase64(byte[] buffer, int index, int count)
    {
      if (base64Encoder == null)
      {
        base64Encoder = new XmlRawWriterBase64Encoder(this);
      }
      // Encode will call WriteRaw to write out the encoded characters
      base64Encoder.Encode(buffer, index, count);
    }

    public override WriteState WriteState
    {
      get
      {
        throw new InvalidOperationException(Res.GetString(Res.Xml_InvalidOperation));
      }
    }

    // Forward call to WriteString(string).
    public override void WriteCData(string text)
    {
      WriteString(text);
    }

    // Forward call to WriteString(string).
    public override void WriteCharEntity(char ch)
    {
      WriteString(new string(new char[] { ch }));
    }

    // Forward call to WriteString(string).
    public override void WriteSurrogateCharEntity(char lowChar, char highChar)
    {
      WriteString(new string(new char[] { lowChar, highChar }));
    }

    // Forward call to WriteString(string).
    public override void WriteWhitespace(string ws)
    {
      WriteString(ws);
    }

    // Forward call to WriteString(string).
    public override void WriteChars(char[] buffer, int index, int count)
    {
      WriteString(new string(buffer, index, count));
    }

    // Forward call to WriteString(string).
    public override void WriteRaw(char[] buffer, int index, int count)
    {
      WriteString(new string(buffer, index, count));
    }

    // Forward call to WriteString(string).
    public override void WriteRaw(string data)
    {
      WriteString(data);
    }

    // Override in order to handle Xml simple typed values and to pass resolver for QName values
    public override void WriteValue(object value)
    {
      if (value == null)
      {
        throw new ArgumentNullException("value");
      }
      WriteString(XmlUntypedConverter.Untyped.ToString(value, resolver));
    }

    // Override in order to handle Xml simple typed values and to pass resolver for QName values
    public override void WriteValue(string value)
    {
      WriteString(value);
    }

    public override void WriteValue(DateTimeOffset value)
    {
      // For compatibility with custom writers, XmlWriter writes DateTimeOffset as DateTime. 
      // Our internal writers should use the DateTimeOffset-String conversion from XmlConvert.
      WriteString(XmlConvert.ToString(value));
    }

    // Get and set the namespace resolver that's used by this RawWriter to resolve prefixes.
    internal virtual IXmlNamespaceResolver NamespaceResolver
    {
      set
      {
        resolver = value;
      }
    }

    // Write the xml declaration.  This must be the first call.
    internal virtual void WriteXmlDeclaration(XmlStandalone standalone)
    {

    }
    internal virtual void WriteXmlDeclaration(string xmldecl)
    {

    }

    // Called after an element's attributes have been enumerated, but before any children have been
    // enumerated.  This method must always be called, even for empty elements.

    internal abstract void StartElementContent();

    // Called before a root element is written (before the WriteStartElement call)
    //   the conformanceLevel specifies the current conformance level the writer is operating with.
    internal virtual void OnRootElement(ConformanceLevel conformanceLevel) { }

    // WriteEndElement() and WriteFullEndElement() overloads, in which caller gives the full name of the
    // element, so that raw writers do not need to keep a stack of element names.  This method should
    // always be called instead of WriteEndElement() or WriteFullEndElement() without parameters.

    internal abstract void WriteEndElement(string prefix, string localName, string ns);

    internal virtual void WriteFullEndElement(string prefix, string localName, string ns)
    {
      WriteEndElement(prefix, localName, ns);
    }

    internal abstract void WriteNamespaceDeclaration(string prefix, string ns);

    // When true, the XmlWellFormedWriter will call:
    //      1) WriteStartNamespaceDeclaration
    //      2) any method that can be used to write out a value of an attribute: WriteString, WriteChars, WriteRaw, WriteCharEntity... 
    //      3) WriteEndNamespaceDeclaration
    // instead of just a single WriteNamespaceDeclaration call. 
    //
    // This feature will be supported by raw writers serializing to text that wish to preserve the attribute value escaping and entities.
    internal virtual bool SupportsNamespaceDeclarationInChunks
    {
      get
      {
        return false;
      }
    }

    internal virtual void WriteStartNamespaceDeclaration(string prefix)
    {
      throw new NotSupportedException();
    }

    internal virtual void WriteEndNamespaceDeclaration()
    {
      throw new NotSupportedException();
    }

    // This is called when the remainder of a base64 value should be output.
    internal void WriteEndBase64()
    {
      // The Flush will call WriteRaw to write out the rest of the encoded characters
      base64Encoder.Flush();
    }

    internal void Close(WriteState currentState)
    {
      Close();
    }

  }
}
