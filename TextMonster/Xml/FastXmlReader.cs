using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
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

    // returns the type of the current node
    public virtual Type ValueType
    {
      get
      {
        return typeof(string);
      }
    }

    // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
    // and returns the content as the most appropriate type (by default as string). Stops at start tags and end tags.
    public virtual object ReadContentAsObject()
    {
      if (!CanReadContentAs())
      {
        throw CreateReadContentAsException("ReadContentAsObject");
      }
      return InternalReadContentAsString();
    }

    // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
    // and converts the content to a boolean. Stops at start tags and end tags.
    public virtual bool ReadContentAsBoolean()
    {
      if (!CanReadContentAs())
      {
        throw CreateReadContentAsException("ReadContentAsBoolean");
      }
      try
      {
        return XmlConvert.ToBoolean(InternalReadContentAsString());
      }
      catch (FormatException e)
      {
        throw new XmlException(Res.Xml_ReadContentAsFormatException, "Boolean", e, this as IXmlLineInfo);
      }
    }

    // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
    // and converts the content to a DateTime. Stops at start tags and end tags.
    public virtual DateTime ReadContentAsDateTime()
    {
      if (!CanReadContentAs())
      {
        throw CreateReadContentAsException("ReadContentAsDateTime");
      }
      try
      {
        return XmlConvert.ToDateTime(InternalReadContentAsString(), XmlDateTimeSerializationMode.RoundtripKind);
      }
      catch (FormatException e)
      {
        throw new XmlException(Res.Xml_ReadContentAsFormatException, "DateTime", e, this as IXmlLineInfo);
      }
    }

    // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
    // and converts the content to a double. Stops at start tags and end tags.
    public virtual double ReadContentAsDouble()
    {
      if (!CanReadContentAs())
      {
        throw CreateReadContentAsException("ReadContentAsDouble");
      }
      try
      {
        return XmlConvert.ToDouble(InternalReadContentAsString());
      }
      catch (FormatException e)
      {
        throw new XmlException(Res.Xml_ReadContentAsFormatException, "Double", e, this as IXmlLineInfo);
      }
    }

    // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
    // and converts the content to a float. Stops at start tags and end tags.
    public virtual float ReadContentAsFloat()
    {
      if (!CanReadContentAs())
      {
        throw CreateReadContentAsException("ReadContentAsFloat");
      }
      try
      {
        return XmlConvert.ToSingle(InternalReadContentAsString());
      }
      catch (FormatException e)
      {
        throw new XmlException(Res.Xml_ReadContentAsFormatException, "Float", e, this as IXmlLineInfo);
      }
    }

    // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
    // and converts the content to a decimal. Stops at start tags and end tags.
    public virtual decimal ReadContentAsDecimal()
    {
      if (!CanReadContentAs())
      {
        throw CreateReadContentAsException("ReadContentAsDecimal");
      }
      try
      {
        return XmlConvert.ToDecimal(InternalReadContentAsString());
      }
      catch (FormatException e)
      {
        throw new XmlException(Res.Xml_ReadContentAsFormatException, "Decimal", e, this as IXmlLineInfo);
      }
    }

    // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
    // and converts the content to an int. Stops at start tags and end tags.
    public virtual int ReadContentAsInt()
    {
      if (!CanReadContentAs())
      {
        throw CreateReadContentAsException("ReadContentAsInt");
      }
      try
      {
        return XmlConvert.ToInt32(InternalReadContentAsString());
      }
      catch (FormatException e)
      {
        throw new XmlException(Res.Xml_ReadContentAsFormatException, "Int", e, this as IXmlLineInfo);
      }
    }

    // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
    // and converts the content to a long. Stops at start tags and end tags.
    public virtual long ReadContentAsLong()
    {
      if (!CanReadContentAs())
      {
        throw CreateReadContentAsException("ReadContentAsLong");
      }
      try
      {
        return XmlConvert.ToInt64(InternalReadContentAsString());
      }
      catch (FormatException e)
      {
        throw new XmlException(Res.Xml_ReadContentAsFormatException, "Long", e, this as IXmlLineInfo);
      }
    }

    // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
    // and returns the content as a string. Stops at start tags and end tags.
    public virtual string ReadContentAsString()
    {
      if (!CanReadContentAs())
      {
        throw CreateReadContentAsException("ReadContentAsString");
      }
      return InternalReadContentAsString();
    }

    // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
    // and converts the content to the requested type. Stops at start tags and end tags.
    public virtual object ReadContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
    {
      if (!CanReadContentAs())
      {
        throw CreateReadContentAsException("ReadContentAs");
      }

      string strContentValue = InternalReadContentAsString();
      if (returnType == typeof(string))
      {
        return strContentValue;
      }
      try
      {
        return XmlUntypedConverter.Untyped.ChangeType(strContentValue, returnType, namespaceResolver ?? this as IXmlNamespaceResolver);
      }
      catch (FormatException e)
      {
        throw new XmlException(Res.Xml_ReadContentAsFormatException, returnType.ToString(), e, this as IXmlLineInfo);
      }
      catch (InvalidCastException e)
      {
        throw new XmlException(Res.Xml_ReadContentAsFormatException, returnType.ToString(), e, this as IXmlLineInfo);
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

    // Binary content access methods
    // Returns true if the reader supports call to ReadContentAsBase64, ReadElementContentAsBase64, ReadContentAsBinHex and ReadElementContentAsBinHex.
    public virtual bool CanReadBinaryContent
    {
      get
      {
        return false;
      }
    }

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

    // Returns a chunk of the value of the current node. Call this method in a loop to get all the data. 
    // Use this method to get a streaming access to the value of the current node.
    public virtual int ReadValueChunk(char[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString(Res.Xml_ReadValueChunkNotSupported));
    }

    // Virtual helper methods
    // Reads the contents of an element as a string. Stops of comments, PIs or entity references.
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public virtual string ReadString()
    {
      if (ReadState != ReadState.Interactive)
      {
        return string.Empty;
      }
      MoveToElement();
      if (NodeType == XmlNodeType.Element)
      {
        if (IsEmptyElement)
        {
          return string.Empty;
        }
        if (!Read())
        {
          throw new InvalidOperationException(Res.GetString(Res.Xml_InvalidOperation));
        }
        if (NodeType == XmlNodeType.EndElement)
        {
          return string.Empty;
        }
      }
      string result = string.Empty;
      while (IsTextualNode(NodeType))
      {
        result += Value;
        if (!Read())
        {
          break;
        }
      }
      return result;
    }

    // Checks whether the current node is a content (non-whitespace text, CDATA, Element, EndElement, EntityReference
    // or EndEntity) node. If the node is not a content node, then the method skips ahead to the next content node or 
    // end of file. Skips over nodes of type ProcessingInstruction, DocumentType, Comment, Whitespace and SignificantWhitespace.
    public virtual XmlNodeType MoveToContent()
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

    // Reads to the next sibling of the current element with the given LocalName and NamespaceURI.
    public virtual bool ReadToNextSibling(string localName, string namespaceURI)
    {
      if (string.IsNullOrEmpty(localName))
      {
        throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
      }
      if (namespaceURI == null)
      {
        throw new ArgumentNullException("namespaceURI");
      }

      // atomize local name and namespace
      localName = NameTable.Add(localName);
      namespaceURI = NameTable.Add(namespaceURI);

      // find the next sibling
      XmlNodeType nt;
      do
      {
        if (!SkipSubtree())
        {
          break;
        }
        nt = NodeType;
        if (nt == XmlNodeType.Element && Ref.Equal(localName, LocalName) && Ref.Equal(namespaceURI, NamespaceURI))
        {
          return true;
        }
      } while (nt != XmlNodeType.EndElement && !EOF);
      return false;
    }

    // Returns the inner content (including markup) of an element or attribute as a string.
    public virtual string ReadInnerXml()
    {
      if (ReadState != ReadState.Interactive)
      {
        return string.Empty;
      }
      if (NodeType != XmlNodeType.Attribute && NodeType != XmlNodeType.Element)
      {
        Read();
        return string.Empty;
      }

      StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
      XmlWriter xtw = CreateWriterForInnerOuterXml(sw);

      try
      {
        if (NodeType == XmlNodeType.Attribute)
        {
          ((XmlTextWriter)xtw).QuoteChar = QuoteChar;
          WriteAttributeValue(xtw);
        }
        if (NodeType == XmlNodeType.Element)
        {
          WriteNode(xtw, false);
        }
      }
      finally
      {
        xtw.Close();
      }
      return sw.ToString();
    }

    // Writes the content (inner XML) of the current node into the provided XmlWriter.
    private void WriteNode(XmlWriter xtw, bool defattr)
    {
      Debug.Assert(xtw is XmlTextWriter);
      int d = NodeType == XmlNodeType.None ? -1 : Depth;
      while (Read() && (d < Depth))
      {
        switch (NodeType)
        {
          case XmlNodeType.Element:
          xtw.WriteStartElement(Prefix, LocalName, NamespaceURI);
          ((XmlTextWriter)xtw).QuoteChar = QuoteChar;
          xtw.WriteAttributes(this, defattr);
          if (IsEmptyElement)
          {
            xtw.WriteEndElement();
          }
          break;
          case XmlNodeType.Text:
          xtw.WriteString(Value);
          break;
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
          xtw.WriteWhitespace(Value);
          break;
          case XmlNodeType.CDATA:
          xtw.WriteCData(Value);
          break;
          case XmlNodeType.EntityReference:
          xtw.WriteEntityRef(Name);
          break;
          case XmlNodeType.XmlDeclaration:
          case XmlNodeType.ProcessingInstruction:
          xtw.WriteProcessingInstruction(Name, Value);
          break;
          case XmlNodeType.DocumentType:
          xtw.WriteDocType(Name, GetAttribute("PUBLIC"), GetAttribute("SYSTEM"), Value);
          break;
          case XmlNodeType.Comment:
          xtw.WriteComment(Value);
          break;
          case XmlNodeType.EndElement:
          xtw.WriteFullEndElement();
          break;
        }
      }
      if (d == Depth && NodeType == XmlNodeType.EndElement)
      {
        Read();
      }
    }

    // Writes the attribute into the provided XmlWriter.
    private void WriteAttributeValue(XmlWriter xtw)
    {
      string attrName = Name;
      while (ReadAttributeValue())
      {
        if (NodeType == XmlNodeType.EntityReference)
        {
          xtw.WriteEntityRef(Name);
        }
        else
        {
          xtw.WriteString(Value);
        }
      }
      MoveToAttribute(attrName);
    }

    // Returns the current element and its descendants or an attribute as a string.
    public virtual string ReadOuterXml()
    {
      if (ReadState != ReadState.Interactive)
      {
        return string.Empty;
      }
      if (NodeType != XmlNodeType.Attribute && NodeType != XmlNodeType.Element)
      {
        Read();
        return string.Empty;
      }

      StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
      XmlWriter xtw = CreateWriterForInnerOuterXml(sw);

      try
      {
        if (NodeType == XmlNodeType.Attribute)
        {
          xtw.WriteStartAttribute(Prefix, LocalName, NamespaceURI);
          WriteAttributeValue(xtw);
          xtw.WriteEndAttribute();
        }
        else
        {
          xtw.WriteNode(this, false);
        }
      }
      finally
      {
        xtw.Close();
      }
      return sw.ToString();
    }

    private XmlWriter CreateWriterForInnerOuterXml(StringWriter sw)
    {
      XmlTextWriter w = new XmlTextWriter(sw);
      // This is a V1 hack; we can put a custom implementation of ReadOuterXml on XmlTextReader/XmlValidatingReader
      SetNamespacesFlag(w);
      return w;
    }

    void SetNamespacesFlag(XmlTextWriter xtw)
    {
      XmlTextReader tr = this as XmlTextReader;
      if (tr != null)
      {
        xtw.Namespaces = tr.Namespaces;
      }
      else
      {
#pragma warning disable 618
        XmlValidatingReader vr = this as XmlValidatingReader;
        if (vr != null)
        {
          xtw.Namespaces = vr.Namespaces;
        }
      }
#pragma warning restore 618
    }

    // Returns an XmlReader that will read only the current element and its descendants and then go to EOF state.
    public virtual FastXmlReader ReadSubtree()
    {
      if (NodeType != XmlNodeType.Element)
      {
        throw new InvalidOperationException(Res.GetString(Res.Xml_ReadSubtreeNotOnElement));
      }
      return new XmlSubtreeReader(this);
    }

    // Returns true when the current node has any attributes.
    public virtual bool HasAttributes
    {
      get
      {
        return AttributeCount > 0;
      }
    }

    //
    // IDisposable interface
    //
    public void Dispose()
    {
      Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
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

    private void CheckElement(string localName, string namespaceURI)
    {
      if (string.IsNullOrEmpty(localName))
      {
        throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
      }
      if (namespaceURI == null)
      {
        throw new ArgumentNullException("namespaceURI");
      }
      if (NodeType != XmlNodeType.Element)
      {
        throw new XmlException(Res.Xml_InvalidNodeType, NodeType.ToString(), this as IXmlLineInfo);
      }
      if (LocalName != localName || NamespaceURI != namespaceURI)
      {
        throw new XmlException(Res.Xml_ElementNotFoundNs, new[] { localName, namespaceURI }, this as IXmlLineInfo);
      }
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

    internal string InternalReadContentAsString()
    {
      string value = string.Empty;
      StringBuilder sb = null;
      do
      {
        switch (NodeType)
        {
          case XmlNodeType.Attribute:
          return Value;
          case XmlNodeType.Text:
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
          case XmlNodeType.CDATA:
          // merge text content
          if (value.Length == 0)
          {
            value = Value;
          }
          else
          {
            if (sb == null)
            {
              sb = new StringBuilder();
              sb.Append(value);
            }
            sb.Append(Value);
          }
          break;
          case XmlNodeType.ProcessingInstruction:
          case XmlNodeType.Comment:
          case XmlNodeType.EndEntity:
          // skip comments, pis and end entity nodes
          break;
          case XmlNodeType.EntityReference:
          if (CanResolveEntity)
          {
            ResolveEntity();
            break;
          }
          goto default;
          default:
          goto ReturnContent;
        }
      } while (AttributeCount != 0 ? ReadAttributeValue() : Read());

    ReturnContent:
      return (sb == null) ? value : sb.ToString();
    }

    private bool SetupReadElementContentAsXxx(string methodName)
    {
      if (NodeType != XmlNodeType.Element)
      {
        throw CreateReadElementContentAsException(methodName);
      }

      bool isEmptyElement = IsEmptyElement;

      // move to content or beyond the empty element
      Read();

      if (isEmptyElement)
      {
        return false;
      }

      XmlNodeType nodeType = NodeType;
      if (nodeType == XmlNodeType.EndElement)
      {
        Read();
        return false;
      }
      if (nodeType == XmlNodeType.Element)
      {
        throw new XmlException(Res.Xml_MixedReadElementContentAs, string.Empty, this as IXmlLineInfo);
      }
      return true;
    }

    private void FinishReadElementContentAsXxx()
    {
      if (NodeType != XmlNodeType.EndElement)
      {
        throw new XmlException(Res.Xml_InvalidNodeType, NodeType.ToString());
      }
      Read();
    }

    internal bool IsDefaultInternal
    {
      get
      {
        if (IsDefault)
        {
          return true;
        }
        IXmlSchemaInfo schemaInfo = SchemaInfo;
        if (schemaInfo != null && schemaInfo.IsDefault)
        {
          return true;
        }
        return false;
      }
    }

    internal virtual IDtdInfo DtdInfo
    {
      get
      {
        return null;
      }
    }

    internal static ConformanceLevel GetV1ConformanceLevel(FastXmlReader reader)
    {
      XmlTextReaderImpl tri = GetXmlTextReaderImpl(reader);
      return tri != null ? tri.V1ComformanceLevel : ConformanceLevel.Document;
    }

    private static XmlTextReaderImpl GetXmlTextReaderImpl(FastXmlReader reader)
    {
      XmlTextReaderImpl tri = reader as XmlTextReaderImpl;
      if (tri != null)
      {
        return tri;
      }

      XmlTextReader tr = reader as XmlTextReader;
      if (tr != null)
      {
        return tr.Impl;
      }

      XmlValidatingReaderImpl vri = reader as XmlValidatingReaderImpl;
      if (vri != null)
      {
        return vri.ReaderImpl;
      }
#pragma warning disable 618
      XmlValidatingReader vr = reader as XmlValidatingReader;
#pragma warning restore 618
      if (vr != null)
      {
        return vr.Impl.ReaderImpl;
      }
      return null;
    }

    //
    // Static methods for creating readers
    //

    // Creates an XmlReader according to the settings for parsing XML from the given Uri.
    [ResourceConsumption(ResourceScope.Machine)]
    [ResourceExposure(ResourceScope.Machine)]
    public static FastXmlReader Create(string inputUri, XmlReaderSettings settings)
    {
      return Create(inputUri, settings, null);
    }

    // Creates an XmlReader according to the settings and parser context for parsing XML from the given Uri.
    [ResourceConsumption(ResourceScope.Machine)]
    [ResourceExposure(ResourceScope.Machine)]
    private static FastXmlReader Create(string inputUri, XmlReaderSettings settings, XmlParserContext inputContext)
    {
      if (settings == null)
      {
        settings = new XmlReaderSettings();
      }
      return settings.CreateReader(inputUri, inputContext);
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

    // Creates an XmlReader according to the settings wrapped over the given reader.
    public static FastXmlReader Create(FastXmlReader reader, XmlReaderSettings settings)
    {
      if (settings == null)
      {
        settings = new XmlReaderSettings();
      }
      return settings.CreateReader(reader);
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
