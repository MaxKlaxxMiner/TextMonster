using System;
using System.Globalization;
using System.IO;
using TextMonster.Xml.Xml_Reader;
// ReSharper disable InconsistentNaming

namespace TextMonster.Xml
{
  // ReSharper disable once InconsistentNaming
  public abstract class FastXmlReader : IDisposable
  {
    private const uint IsTextualNodeBitmap = 0x6018; // 00 0110 0000 0001 1000
    // 0 None, 
    // 0 Element,
    // 0 Attribute,
    // 1 Text,
    // 1 CDATA,
    // 0 EntityReference,
    // 0 Entity,
    // 0 ProcessingInstruction,
    // 0 Comment,
    // 0 Document,
    // 0 DocumentType,
    // 0 DocumentFragment,
    // 0 Notation,
    // 1 Whitespace,
    // 1 SignificantWhitespace,
    // 0 EndElement,
    // 0 EndEntity,
    // 0 XmlDeclaration

    private const uint CanReadContentAsBitmap = 0x1E1BC; // 01 1110 0001 1011 1100
    // 0 None, 
    // 0 Element,
    // 1 Attribute,
    // 1 Text,
    // 1 CDATA,
    // 1 EntityReference,
    // 0 Entity,
    // 1 ProcessingInstruction,
    // 1 Comment,
    // 0 Document,
    // 0 DocumentType,
    // 0 DocumentFragment,
    // 0 Notation,
    // 1 Whitespace,
    // 1 SignificantWhitespace,
    // 1 EndElement,
    // 1 EndEntity,
    // 0 XmlDeclaration

    private const uint HasValueBitmap = 0x2659C; // 10 0110 0101 1001 1100
    // 0 None, 
    // 0 Element,
    // 1 Attribute,
    // 1 Text,
    // 1 CDATA,
    // 0 EntityReference,
    // 0 Entity,
    // 1 ProcessingInstruction,
    // 1 Comment,
    // 0 Document,
    // 1 DocumentType,
    // 0 DocumentFragment,
    // 0 Notation,
    // 1 Whitespace,
    // 1 SignificantWhitespace,
    // 0 EndElement,
    // 0 EndEntity,
    // 1 XmlDeclaration

    //
    // Constants
    //
    internal const int DefaultBufferSize = 4096;
    private const int BiggerBufferSize = 8192;
    private const int MaxStreamLengthForDefaultBufferSize = 64 * 1024; // 64kB

    // Settings
    public virtual XmlReaderSettings Settings
    {
      get
      {
        return null;
      }
    }

    // Node Properties
    // Get the type of the current node.
    public abstract XmlNodeType NodeType { get; }

    // Gets the name of the current node, including the namespace prefix.
    public virtual string Name
    {
      get
      {
        if (Prefix.Length == 0)
        {
          return LocalName;
        }
        return NameTable.Add(string.Concat(Prefix, ":", LocalName));
      }
    }

    // Gets the name of the current node without the namespace prefix.
    public abstract string LocalName { get; }

    // Gets the namespace URN (as defined in the W3C Namespace Specification) of the current namespace scope.
    public abstract string NamespaceURI { get; }

    // Gets the namespace prefix associated with the current node.
    public abstract string Prefix { get; }

    // Gets a value indicating whether
    public virtual bool HasValue
    {
      get
      {
        return HasValueInternal(NodeType);
      }
    }

    // Gets the text value of the current node.
    public abstract string Value { get; }

    // Gets the depth of the current node in the XML element stack.
    public abstract int Depth { get; }

    // Gets the base URI of the current node.
    public abstract string BaseURI { get; }

    // Gets a value indicating whether the current node is an empty element (for example, <MyElement/>).
    public abstract bool IsEmptyElement { get; }

    // Gets a value indicating whether the current node is an attribute that was generated from the default value defined
    // in the DTD or schema.
    public virtual bool IsDefault
    {
      get
      {
        return false;
      }
    }

    // Gets the quotation mark character used to enclose the value of an attribute node.
    public virtual char QuoteChar
    {
      get
      {
        return '"';
      }
    }

    // Gets the current xml:space scope.
    public virtual XmlSpace XmlSpace
    {
      get
      {
        return XmlSpace.None;
      }
    }

    // Gets the current xml:lang scope.
    public virtual string XmlLang
    {
      get
      {
        return string.Empty;
      }
    }

    // returns the schema info interface of the reader
    public virtual IXmlSchemaInfo SchemaInfo
    {
      get
      {
        return this as IXmlSchemaInfo;
      }
    }

    // Attribute Accessors
    // The number of attributes on the current node.
    public abstract int AttributeCount { get; }

    // Gets the value of the attribute with the specified Name
    public abstract string GetAttribute(string name);

    // Gets the value of the attribute with the LocalName and NamespaceURI
    public abstract string GetAttribute(string name, string namespaceURI);

    // Gets the value of the attribute with the specified index.
    public abstract string GetAttribute(int i);

    // Moves to the attribute with the specified Name.
    public abstract bool MoveToAttribute(string name);

    // Moves to the attribute with the specified LocalName and NamespaceURI.
    public abstract bool MoveToAttribute(string name, string ns);

    // Moves to the attribute with the specified index.
    public virtual void MoveToAttribute(int i)
    {
      if (i < 0 || i >= AttributeCount)
      {
        throw new ArgumentOutOfRangeException("i");
      }
      MoveToElement();
      MoveToFirstAttribute();
      int j = 0;
      while (j < i)
      {
        MoveToNextAttribute();
        j++;
      }
    }

    // Moves to the first attribute of the current node.
    public abstract bool MoveToFirstAttribute();

    // Moves to the next attribute.
    public abstract bool MoveToNextAttribute();

    // Moves to the element that contains the current attribute node.
    public abstract bool MoveToElement();

    // Parses the attribute value into one or more Text and/or EntityReference node types.

    public abstract bool ReadAttributeValue();

    // Moving through the Stream
    // Reads the next node from the stream.

    public abstract bool Read();

    // Returns true when the XmlReader is positioned at the end of the stream.
    public abstract bool EOF { get; }

    // Closes the stream/TextReader (if CloseInput==true), changes the ReadState to Closed, and sets all the properties back to zero/empty string.
    public virtual void Close() { }

    // Returns the read state of the XmlReader.
    public abstract ReadState ReadState { get; }

    // Skips to the end tag of the current element.
    public virtual void Skip()
    {
      if (ReadState != ReadState.Interactive)
      {

        return;

      }
      SkipSubtree();
    }

    // Gets the XmlNameTable associated with the XmlReader.
    public abstract XmlNameTable NameTable { get; }

    // Resolves a namespace prefix in the current element's scope.
    public abstract string LookupNamespace(string prefix);

    // Returns true if the XmlReader can expand general entities.
    public virtual bool CanResolveEntity
    {
      get
      {
        return false;
      }
    }

    // Resolves the entity reference for nodes of NodeType EntityReference.
    public abstract void ResolveEntity();

    // Returns decoded bytes of the current base64 text content. Call this methods until it returns 0 to get all the data.
    public virtual int ReadContentAsBase64(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString(Res.Xml_ReadBinaryContentNotSupported, "ReadContentAsBase64"));
    }

    // Returns decoded bytes of the current base64 element content. Call this methods until it returns 0 to get all the data.
    public virtual int ReadElementContentAsBase64(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString(Res.Xml_ReadBinaryContentNotSupported, "ReadElementContentAsBase64"));
    }

    // Returns decoded bytes of the current binhex text content. Call this methods until it returns 0 to get all the data.
    public virtual int ReadContentAsBinHex(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString(Res.Xml_ReadBinaryContentNotSupported, "ReadContentAsBinHex"));
    }

    // Returns decoded bytes of the current binhex element content. Call this methods until it returns 0 to get all the data.
    public virtual int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString(Res.Xml_ReadBinaryContentNotSupported, "ReadElementContentAsBinHex"));
    }

    // Text streaming methods

    // Returns true if the XmlReader supports calls to ReadValueChunk.
    public virtual bool CanReadValueChunk
    {
      get
      {
        return false;
      }
    }

    public virtual int ReadValueChunk(char[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString(Res.Xml_ReadValueChunkNotSupported));
    }

    public XmlNodeType MoveToContent()
    {
      do
      {
        switch (NodeType)
        {
          case XmlNodeType.Attribute:
          MoveToElement();
          goto case XmlNodeType.Element;
          case XmlNodeType.Element:
          case XmlNodeType.EndElement:
          case XmlNodeType.CDATA:
          case XmlNodeType.Text:
          case XmlNodeType.EntityReference:
          case XmlNodeType.EndEntity:
          return NodeType;
        }
      } while (Read());
      return NodeType;
    }

    //
    // IDisposable interface
    //
    public void Dispose()
    {
      Dispose(true);
    }

    protected void Dispose(bool disposing)
    { //the boolean flag may be used by subclasses to differentiate between disposing and finalizing
      if (disposing && ReadState != ReadState.Closed)
      {
        Close();
      }
    }

    //
    // Internal methods
    //
    // Validation support
    internal virtual XmlNamespaceManager NamespaceManager
    {
      get
      {
        return null;
      }
    }

    static internal bool IsTextualNode(XmlNodeType nodeType)
    {
      return 0 != (IsTextualNodeBitmap & (1 << (int)nodeType));
    }

    static internal bool CanReadContentAs(XmlNodeType nodeType)
    {
      return 0 != (CanReadContentAsBitmap & (1 << (int)nodeType));
    }

    static internal bool HasValueInternal(XmlNodeType nodeType)
    {
      return 0 != (HasValueBitmap & (1 << (int)nodeType));
    }

    //
    // Private methods
    //
    //SkipSubTree is called whenever validation of the skipped subtree is required on a reader with XsdValidation
    private bool SkipSubtree()
    {
      MoveToElement();
      if (NodeType == XmlNodeType.Element && !IsEmptyElement)
      {
        int depth = Depth;

        while (Read() && depth < Depth)
        {
          // Nothing, just read on
        }

        // consume end tag
        if (NodeType == XmlNodeType.EndElement)
          return Read();
      }
      else
      {
        return Read();
      }

      return false;
    }

    internal Exception CreateReadContentAsException(string methodName)
    {
      return CreateReadContentAsException(methodName, NodeType, this as IXmlLineInfo);
    }

    internal Exception CreateReadElementContentAsException(string methodName)
    {
      return CreateReadElementContentAsException(methodName, NodeType, this as IXmlLineInfo);
    }

    internal bool CanReadContentAs()
    {
      return CanReadContentAs(NodeType);
    }

    private static Exception CreateReadContentAsException(string methodName, XmlNodeType nodeType, IXmlLineInfo lineInfo)
    {
      return new InvalidOperationException(AddLineInfo(Res.GetString(Res.Xml_InvalidReadContentAs, methodName, nodeType.ToString()), lineInfo));
    }

    private static Exception CreateReadElementContentAsException(string methodName, XmlNodeType nodeType, IXmlLineInfo lineInfo)
    {
      return new InvalidOperationException(AddLineInfo(Res.GetString(Res.Xml_InvalidReadElementContentAs, methodName, nodeType.ToString()), lineInfo));
    }

    static string AddLineInfo(string message, IXmlLineInfo lineInfo)
    {
      if (lineInfo != null)
      {
        var lineArgs = new string[2];
        lineArgs[0] = lineInfo.LineNumber.ToString(CultureInfo.InvariantCulture);
        lineArgs[1] = lineInfo.LinePosition.ToString(CultureInfo.InvariantCulture);
        message += " " + Res.GetString(Res.Xml_ErrorPosition, lineArgs);
      }
      return message;
    }

    internal virtual IDtdInfo DtdInfo
    {
      get
      {
        return null;
      }
    }

    // Creates an XmlReader according to the settings for parsing XML from the given stream.
    public static FastXmlReader Create(Stream input, XmlReaderSettings settings)
    {
      return Create(input, settings, string.Empty);
    }

    // Creates an XmlReader according to the settings and base Uri for parsing XML from the given stream.
    public static FastXmlReader Create(Stream input, XmlReaderSettings settings, string baseUri)
    {
      if (settings == null)
      {
        settings = new XmlReaderSettings();
      }
      return settings.CreateReader(input, null, baseUri, null);
    }

    // Creates an XmlReader according to the settings and baseUri for parsing XML from the given TextReader.
    public static FastXmlReader Create(TextReader input, XmlReaderSettings settings, string baseUri)
    {
      if (settings == null)
      {
        settings = new XmlReaderSettings();
      }
      return settings.CreateReader(input, baseUri, null);
    }

    internal static int CalcBufferSize(Stream input)
    {
      // determine the size of byte buffer
      int bufferSize = DefaultBufferSize;
      if (input.CanSeek)
      {
        long len = input.Length;
        if (len < bufferSize)
        {
          bufferSize = checked((int)len);
        }
        else if (len > MaxStreamLengthForDefaultBufferSize)
        {
          bufferSize = BiggerBufferSize;
        }
      }

      // return the byte buffer size
      return bufferSize;
    }
  }
}
