using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace TextMonster.Xml.Xml_Reader
{
  internal partial class XsdValidatingReader : FastXmlReader, IXmlSchemaInfo, IXmlLineInfo, IXmlNamespaceResolver
  {

    private enum ValidatingReaderState
    {
      Init = 1,
      Read = 2,
      OnDefaultAttribute = -1,
      OnReadAttributeValue = -2,
      OnAttribute = 3,
      ClearAttributes = 4,
      ParseInlineSchema = 5,
      ReadAhead = 6,
      OnReadBinaryContent = 7,
      ReaderClosed = 8,
      EOF = 9,
    }
    //Validation
    private FastXmlReader coreReader;
    private IXmlNamespaceResolver coreReaderNSResolver;
    private IXmlNamespaceResolver thisNSResolver;
    private XmlSchemaValidator validator;
    private XmlResolver xmlResolver;
    private ValidationEventHandler validationEvent;
    private ValidatingReaderState validationState;
    private XmlValueGetter valueGetter;

    // namespace management
    XmlNamespaceManager nsManager;
    bool manageNamespaces;
    bool processInlineSchema;
    bool replayCache;

    //Current Node handling
    private ValidatingReaderNodeData cachedNode; //Used to cache current node when looking ahead or default attributes                           
    private AttributePSVIInfo attributePSVI;

    //Attributes
    int attributeCount; //Total count of attributes including default
    int coreReaderAttributeCount;
    int currentAttrIndex;
    AttributePSVIInfo[] attributePSVINodes;
    ArrayList defaultAttributes;

    //Inline Schema
    private Parser inlineSchemaParser = null;

    //Typed Value & PSVI
    private object atomicValue;
    private XmlSchemaInfo xmlSchemaInfo;

    // original string of the atomic value
    private string originalAtomicValueString;

    //cached coreReader information
    private XmlNameTable coreReaderNameTable;
    private XsdCachingReader cachingReader;

    //ReadAttributeValue TextNode
    private ValidatingReaderNodeData textNode;

    //To avoid SchemaNames creation
    private string NsXmlNs;
    private string NsXs;
    private string NsXsi;
    private string XsiType;
    private string XsiNil;
    private string XsdSchema;
    private string XsiSchemaLocation;
    private string XsiNoNamespaceSchemaLocation;

    //Underlying reader's IXmlLineInfo
    IXmlLineInfo lineInfo;

    // helpers for Read[Element]ContentAs{Base64,BinHex} methods
    ReadContentAsBinaryHelper readBinaryHelper;
    ValidatingReaderState savedState;

    //Constants
    private const int InitialAttributeCount = 8;

    static volatile Type TypeOfString;

    //Constructor
    internal XsdValidatingReader(FastXmlReader reader, XmlResolver xmlResolver, XmlReaderSettings readerSettings, XmlSchemaObject partialValidationType)
    {
      this.coreReader = reader;
      this.coreReaderNSResolver = reader as IXmlNamespaceResolver;
      this.lineInfo = reader as IXmlLineInfo;
      coreReaderNameTable = coreReader.NameTable;
      if (coreReaderNSResolver == null)
      {
        nsManager = new XmlNamespaceManager(coreReaderNameTable);
        manageNamespaces = true;
      }
      thisNSResolver = this as IXmlNamespaceResolver;
      this.xmlResolver = xmlResolver;
      this.processInlineSchema = (readerSettings.ValidationFlags & XmlSchemaValidationFlags.ProcessInlineSchema) != 0;
      Init();
      SetupValidator(readerSettings, reader, partialValidationType);
      validationEvent = readerSettings.GetEventHandler();
    }

    internal XsdValidatingReader(FastXmlReader reader, XmlResolver xmlResolver, XmlReaderSettings readerSettings)
      :
  this(reader, xmlResolver, readerSettings, null) { }

    private void Init()
    {
      validationState = ValidatingReaderState.Init;
      defaultAttributes = new ArrayList();
      currentAttrIndex = -1;
      attributePSVINodes = new AttributePSVIInfo[InitialAttributeCount];
      valueGetter = new XmlValueGetter(GetStringValue);
      TypeOfString = typeof(System.String);
      xmlSchemaInfo = new XmlSchemaInfo();

      //Add common strings to be compared to NameTable
      NsXmlNs = coreReaderNameTable.Add(XmlReservedNs.NsXmlNs);
      NsXs = coreReaderNameTable.Add(XmlReservedNs.NsXs);
      NsXsi = coreReaderNameTable.Add(XmlReservedNs.NsXsi);
      XsiType = coreReaderNameTable.Add("type");
      XsiNil = coreReaderNameTable.Add("nil");
      XsiSchemaLocation = coreReaderNameTable.Add("schemaLocation");
      XsiNoNamespaceSchemaLocation = coreReaderNameTable.Add("noNamespaceSchemaLocation");
      XsdSchema = coreReaderNameTable.Add("schema");
    }

    private void SetupValidator(XmlReaderSettings readerSettings, FastXmlReader reader, XmlSchemaObject partialValidationType)
    {
      validator = new XmlSchemaValidator(coreReaderNameTable, readerSettings.Schemas, thisNSResolver, readerSettings.ValidationFlags);
      validator.XmlResolver = this.xmlResolver;
      validator.SourceUri = XmlConvert.ToUri(reader.BaseURI); //Not using XmlResolver.ResolveUri as it checks for relative Uris,reader.BaseURI will be absolute file paths or string.Empty
      validator.ValidationEventSender = this;
      validator.ValidationEventHandler += readerSettings.GetEventHandler();
      validator.LineInfoProvider = this.lineInfo;
      validator.SetDtdSchemaInfo(reader.DtdInfo);
      if (partialValidationType != null)
      {
        validator.Initialize(partialValidationType);
      }
      else
      {
        validator.Initialize();
      }
    }

    // Settings
    public override XmlReaderSettings Settings
    {
      get
      {
        XmlReaderSettings settings = coreReader.Settings;
        if (null != settings)
          settings = settings.Clone();
        if (settings == null)
        {
          settings = new XmlReaderSettings();
        }
        settings.Schemas = validator.SchemaSet;
        settings.ValidationType = ValidationType.Schema;
        settings.ValidationFlags = validator.ValidationFlags;
        settings.ReadOnly = true;
        return settings;
      }
    }

    // Node Properties

    // Gets the type of the current node.
    public override XmlNodeType NodeType
    {
      get
      {
        if ((int)validationState < 0)
        {
          return cachedNode.NodeType;
        }
        else
        {
          XmlNodeType nodeType = coreReader.NodeType;
          //Check for significant whitespace
          if (nodeType == XmlNodeType.Whitespace && (validator.CurrentContentType == XmlSchemaContentType.TextOnly || validator.CurrentContentType == XmlSchemaContentType.Mixed))
          {
            return XmlNodeType.SignificantWhitespace;
          }
          return nodeType;
        }
      }
    }

    // Gets the name of the current node, including the namespace prefix.
    public override string Name
    {
      get
      {
        if (validationState == ValidatingReaderState.OnDefaultAttribute)
        {
          string prefix = validator.GetDefaultAttributePrefix(cachedNode.Namespace);
          if (prefix != null && prefix.Length != 0)
          {
            return string.Concat(prefix + ":" + cachedNode.LocalName);
          }
          return cachedNode.LocalName;
        }
        return coreReader.Name;
      }
    }

    // Gets the name of the current node without the namespace prefix.
    public override string LocalName
    {
      get
      {
        if ((int)validationState < 0)
        {
          return cachedNode.LocalName;
        }
        return coreReader.LocalName;
      }
    }

    // Gets the namespace URN (as defined in the W3C Namespace Specification) of the current namespace scope.
    public override string NamespaceURI
    {
      get
      {
        if ((int)validationState < 0)
        {
          return cachedNode.Namespace;
        }
        return coreReader.NamespaceURI;
      }
    }

    // Gets the namespace prefix associated with the current node.
    public override string Prefix
    {
      get
      {
        if ((int)validationState < 0)
        {
          return cachedNode.Prefix;
        }
        return coreReader.Prefix;
      }
    }

    // Gets a value indicating whether the current node can have a non-empty Value
    public override bool HasValue
    {
      get
      {
        if ((int)validationState < 0)
        {
          return true;
        }
        return coreReader.HasValue;
      }
    }

    // Gets the text value of the current node.
    public override string Value
    {
      get
      {
        if ((int)validationState < 0)
        {
          return cachedNode.RawValue;
        }
        return coreReader.Value;
      }
    }

    // Gets the depth of the current node in the XML element stack.
    public override int Depth
    {
      get
      {
        if ((int)validationState < 0)
        {
          return cachedNode.Depth;
        }
        return coreReader.Depth;
      }
    }

    // Gets the base URI of the current node.
    public override string BaseURI
    {
      get
      {
        return coreReader.BaseURI;
      }
    }

    // Gets a value indicating whether the current node is an empty element (for example, <MyElement/>).
    public override bool IsEmptyElement
    {
      get
      {
        return coreReader.IsEmptyElement;
      }
    }

    // Gets a value indicating whether the current node is an attribute that was generated from the default value defined
    // in the DTD or schema.
    public override bool IsDefault
    {
      get
      {
        if (validationState == ValidatingReaderState.OnDefaultAttribute)
        { //XSD default attributes
          return true;
        }
        return coreReader.IsDefault; //This is DTD Default attribute
      }
    }

    // Gets the quotation mark character used to enclose the value of an attribute node.
    public override char QuoteChar
    {
      get
      {
        return coreReader.QuoteChar;
      }
    }

    // Gets the current xml:space scope. 
    public override XmlSpace XmlSpace
    {
      get
      {
        return coreReader.XmlSpace;
      }
    }

    // Gets the current xml:lang scope.
    public override string XmlLang
    {
      get
      {
        return coreReader.XmlLang;
      }
    }

    public override IXmlSchemaInfo SchemaInfo
    {
      get
      {
        return this as IXmlSchemaInfo;
      }
    }

    public override int AttributeCount
    {
      get
      {
        return attributeCount;
      }
    }

    // Gets the value of the attribute with the specified Name.
    public override string GetAttribute(string name)
    {
      string attValue = coreReader.GetAttribute(name);

      if (attValue == null && attributeCount > 0)
      { //Could be default attribute
        ValidatingReaderNodeData defaultNode = GetDefaultAttribute(name, false);
        if (defaultNode != null)
        { //Default found
          attValue = defaultNode.RawValue;
        }
      }
      return attValue;
    }

    // Gets the value of the attribute with the specified LocalName and NamespaceURI.
    public override string GetAttribute(string name, string namespaceURI)
    {
      string attValue = coreReader.GetAttribute(name, namespaceURI);

      if (attValue == null && attributeCount > 0)
      { //Could be default attribute
        namespaceURI = (namespaceURI == null) ? string.Empty : coreReaderNameTable.Get(namespaceURI);
        name = coreReaderNameTable.Get(name);
        if (name == null || namespaceURI == null)
        { //Attribute not present since we did not see it
          return null;
        }
        ValidatingReaderNodeData attNode = GetDefaultAttribute(name, namespaceURI, false);
        if (attNode != null)
        {
          return attNode.RawValue;
        }
      }
      return attValue;
    }

    // Gets the value of the attribute with the specified index.
    public override string GetAttribute(int i)
    {
      if (attributeCount == 0)
      {
        return null;
      }
      if (i < coreReaderAttributeCount)
      {
        return coreReader.GetAttribute(i);
      }
      else
      {
        int defaultIndex = i - coreReaderAttributeCount;
        ValidatingReaderNodeData attNode = (ValidatingReaderNodeData)defaultAttributes[defaultIndex];
        return attNode.RawValue;
      }
    }

    // Moves to the attribute with the specified Name
    public override bool MoveToAttribute(string name)
    {

      if (coreReader.MoveToAttribute(name))
      {
        validationState = ValidatingReaderState.OnAttribute;
        attributePSVI = GetAttributePSVI(name);
        goto Found;
      }
      else if (attributeCount > 0)
      { //Default attribute
        ValidatingReaderNodeData defaultNode = GetDefaultAttribute(name, true);
        if (defaultNode != null)
        {
          validationState = ValidatingReaderState.OnDefaultAttribute;
          attributePSVI = defaultNode.AttInfo;
          cachedNode = defaultNode;
          goto Found;
        }
      }
      return false;
    Found:
      if (validationState == ValidatingReaderState.OnReadBinaryContent)
      {
        readBinaryHelper.Finish();
        validationState = savedState;
      }
      return true;
    }

    // Moves to the attribute with the specified LocalName and NamespaceURI
    public override bool MoveToAttribute(string name, string ns)
    {
      //Check atomized local name and ns
      name = coreReaderNameTable.Get(name);
      ns = ns != null ? coreReaderNameTable.Get(ns) : string.Empty;
      if (name == null || ns == null)
      { //Name or ns not found in the nameTable, then attribute is not found
        return false;
      }
      if (coreReader.MoveToAttribute(name, ns))
      {
        validationState = ValidatingReaderState.OnAttribute;
        if (inlineSchemaParser == null)
        {
          attributePSVI = GetAttributePSVI(name, ns);
        }
        else
        { //Parsing inline schema, no PSVI for schema attributes
          attributePSVI = null;
        }
        goto Found;
      }
      else
      { //Default attribute
        ValidatingReaderNodeData defaultNode = GetDefaultAttribute(name, ns, true);
        if (defaultNode != null)
        {
          attributePSVI = defaultNode.AttInfo;
          cachedNode = defaultNode;
          validationState = ValidatingReaderState.OnDefaultAttribute;
          goto Found;
        }
      }
      return false;
    Found:
      if (validationState == ValidatingReaderState.OnReadBinaryContent)
      {
        readBinaryHelper.Finish();
        validationState = savedState;
      }
      return true;
    }

    // Moves to the attribute with the specified index
    public override void MoveToAttribute(int i)
    {
      if (i < 0 || i >= attributeCount)
      {
        throw new ArgumentOutOfRangeException("i");
      }
      currentAttrIndex = i;
      if (i < coreReaderAttributeCount)
      { //reader attribute
        coreReader.MoveToAttribute(i);
        if (inlineSchemaParser == null)
        {
          attributePSVI = attributePSVINodes[i];
        }
        else
        {
          attributePSVI = null;
        }
        validationState = ValidatingReaderState.OnAttribute;
      }
      else
      { //default attribute
        int defaultIndex = i - coreReaderAttributeCount;
        cachedNode = (ValidatingReaderNodeData)defaultAttributes[defaultIndex];
        attributePSVI = cachedNode.AttInfo;
        validationState = ValidatingReaderState.OnDefaultAttribute;
      }
      if (validationState == ValidatingReaderState.OnReadBinaryContent)
      {
        readBinaryHelper.Finish();
        validationState = savedState;
      }
    }

    // Moves to the first attribute.
    public override bool MoveToFirstAttribute()
    {
      if (coreReader.MoveToFirstAttribute())
      {
        currentAttrIndex = 0;
        if (inlineSchemaParser == null)
        {
          attributePSVI = attributePSVINodes[0];
        }
        else
        {
          attributePSVI = null;
        }
        validationState = ValidatingReaderState.OnAttribute;
        goto Found;
      }
      else if (defaultAttributes.Count > 0)
      { //check for default
        cachedNode = (ValidatingReaderNodeData)defaultAttributes[0];
        attributePSVI = cachedNode.AttInfo;
        currentAttrIndex = 0;
        validationState = ValidatingReaderState.OnDefaultAttribute;
        goto Found;
      }
      return false;
    Found:
      if (validationState == ValidatingReaderState.OnReadBinaryContent)
      {
        readBinaryHelper.Finish();
        validationState = savedState;
      }
      return true;
    }

    // Moves to the next attribute.
    public override bool MoveToNextAttribute()
    {
      if (currentAttrIndex + 1 < coreReaderAttributeCount)
      {
        coreReader.MoveToNextAttribute();
        currentAttrIndex++;
        if (inlineSchemaParser == null)
        {
          attributePSVI = attributePSVINodes[currentAttrIndex];
        }
        else
        {
          attributePSVI = null;
        }
        validationState = ValidatingReaderState.OnAttribute;
        goto Found;
      }
      else if (currentAttrIndex + 1 < attributeCount)
      { //default attribute
        int defaultIndex = ++currentAttrIndex - coreReaderAttributeCount;
        cachedNode = (ValidatingReaderNodeData)defaultAttributes[defaultIndex];
        attributePSVI = cachedNode.AttInfo;
        validationState = ValidatingReaderState.OnDefaultAttribute;
        goto Found;
      }
      return false;
    Found:
      if (validationState == ValidatingReaderState.OnReadBinaryContent)
      {
        readBinaryHelper.Finish();
        validationState = savedState;
      }
      return true;
    }

    // Moves to the element that contains the current attribute node.
    public override bool MoveToElement()
    {
      if (coreReader.MoveToElement() || (int)validationState < 0)
      { //states OnDefaultAttribute or OnReadAttributeValue
        currentAttrIndex = -1;
        validationState = ValidatingReaderState.ClearAttributes;
        return true;
      }
      return false;
    }

    // Reads the next node from the stream/TextReader.
    public override bool Read()
    {
      switch (validationState)
      {
        case ValidatingReaderState.Read:
        if (coreReader.Read())
        {
          ProcessReaderEvent();
          return true;
        }
        else
        {
          validator.EndValidation();
          if (coreReader.EOF)
          {
            validationState = ValidatingReaderState.EOF;
          }
          return false;
        }

        case ValidatingReaderState.ParseInlineSchema:
        ProcessInlineSchema();
        return true;

        case ValidatingReaderState.OnAttribute:
        case ValidatingReaderState.OnDefaultAttribute:
        case ValidatingReaderState.ClearAttributes:
        case ValidatingReaderState.OnReadAttributeValue:
        ClearAttributesInfo();
        if (inlineSchemaParser != null)
        {
          validationState = ValidatingReaderState.ParseInlineSchema;
          goto case ValidatingReaderState.ParseInlineSchema;
        }
        else
        {
          validationState = ValidatingReaderState.Read;
          goto case ValidatingReaderState.Read;
        }

        case ValidatingReaderState.ReadAhead: //Will enter here on calling Skip() 
        ClearAttributesInfo();
        ProcessReaderEvent();
        validationState = ValidatingReaderState.Read;
        return true;

        case ValidatingReaderState.OnReadBinaryContent:
        validationState = savedState;
        readBinaryHelper.Finish();
        return Read();

        case ValidatingReaderState.Init:
        validationState = ValidatingReaderState.Read;
        if (coreReader.ReadState == ReadState.Interactive)
        { //If the underlying reader is already positioned on a ndoe, process it
          ProcessReaderEvent();
          return true;
        }
        else
        {
          goto case ValidatingReaderState.Read;
        }

        case ValidatingReaderState.ReaderClosed:
        case ValidatingReaderState.EOF:
        return false;

        default:
        return false;
      }
    }

    // Gets a value indicating whether XmlReader is positioned at the end of the stream/TextReader.
    public override bool EOF
    {
      get
      {
        return coreReader.EOF;
      }
    }

    // Closes the stream, changes the ReadState to Closed, and sets all the properties back to zero.
    public override void Close()
    {
      coreReader.Close();
      validationState = ValidatingReaderState.ReaderClosed;
    }

    // Returns the read state of the XmlReader.
    public override ReadState ReadState
    {
      get
      {
        return (validationState == ValidatingReaderState.Init) ? ReadState.Initial : coreReader.ReadState;
      }
    }

    // Skips to the end tag of the current element.
    public override void Skip()
    {
      switch (NodeType)
      {
        case XmlNodeType.Element:
        if (coreReader.IsEmptyElement)
        {
          break;
        }
        bool callSkipToEndElem = true;
        //If union and unionValue has been parsed till EndElement, then validator.ValidateEndElement has been called
        //Hence should not call SkipToEndElement as the current context has already been popped in the validator
        if ((xmlSchemaInfo.IsUnionType || xmlSchemaInfo.IsDefault) && coreReader is XsdCachingReader)
        {
          callSkipToEndElem = false;
        }
        coreReader.Skip();
        validationState = ValidatingReaderState.ReadAhead;
        if (callSkipToEndElem)
        {
          validator.SkipToEndElement(xmlSchemaInfo);
        }
        break;

        case XmlNodeType.Attribute:
        MoveToElement();
        goto case XmlNodeType.Element;
      }
      //For all other NodeTypes Skip() same as Read()
      Read();
      return;
    }

    // Gets the XmlNameTable associated with this implementation.
    public override XmlNameTable NameTable
    {
      get
      {
        return coreReaderNameTable;
      }
    }

    // Resolves a namespace prefix in the current element's scope.
    public override string LookupNamespace(string prefix)
    {
      return thisNSResolver.LookupNamespace(prefix);
    }

    // Resolves the entity reference for nodes of NodeType EntityReference.
    public override void ResolveEntity()
    {
      throw new InvalidOperationException();
    }

    // Parses the attribute value into one or more Text and/or EntityReference node types.
    public override bool ReadAttributeValue()
    {
      if (validationState == ValidatingReaderState.OnReadBinaryContent)
      {
        readBinaryHelper.Finish();
        validationState = savedState;
      }
      if (NodeType == XmlNodeType.Attribute)
      {
        if (validationState == ValidatingReaderState.OnDefaultAttribute)
        {
          cachedNode = CreateDummyTextNode(cachedNode.RawValue, cachedNode.Depth + 1);
          validationState = ValidatingReaderState.OnReadAttributeValue;
          return true;
        }
        return coreReader.ReadAttributeValue();
      }
      return false;
    }

    bool IXmlSchemaInfo.IsDefault
    {
      get
      {
        switch (NodeType)
        {
          case XmlNodeType.Element:
          if (!coreReader.IsEmptyElement)
          {
            GetIsDefault();
          }
          return xmlSchemaInfo.IsDefault;

          case XmlNodeType.EndElement:
          return xmlSchemaInfo.IsDefault;

          case XmlNodeType.Attribute:
          if (attributePSVI != null)
          {
            return AttributeSchemaInfo.IsDefault;
          }
          break;

          default:
          break;
        }
        return false;
      }
    }

    bool IXmlSchemaInfo.IsNil
    {
      get
      {
        switch (NodeType)
        {
          case XmlNodeType.Element:
          case XmlNodeType.EndElement:
          return xmlSchemaInfo.IsNil;

          default:
          break;
        }
        return false;
      }
    }

    XmlSchemaValidity IXmlSchemaInfo.Validity
    {
      get
      {
        switch (NodeType)
        {
          case XmlNodeType.Element:
          if (coreReader.IsEmptyElement)
          {
            return xmlSchemaInfo.Validity;
          }
          if (xmlSchemaInfo.Validity == XmlSchemaValidity.Valid)
          { //It might be valid for unions since we read ahead, but report notknown for consistency
            return XmlSchemaValidity.NotKnown;
          }
          return xmlSchemaInfo.Validity;

          case XmlNodeType.EndElement:
          return xmlSchemaInfo.Validity;

          case XmlNodeType.Attribute:
          if (attributePSVI != null)
          {
            return AttributeSchemaInfo.Validity;
          }
          break;
        }
        return XmlSchemaValidity.NotKnown;
      }
    }

    XmlSchemaSimpleType IXmlSchemaInfo.MemberType
    {
      get
      {
        switch (NodeType)
        {
          case XmlNodeType.Element:
          if (!coreReader.IsEmptyElement)
          {
            GetMemberType();
          }
          return xmlSchemaInfo.MemberType;

          case XmlNodeType.EndElement:
          return xmlSchemaInfo.MemberType;

          case XmlNodeType.Attribute:
          if (attributePSVI != null)
          {
            return AttributeSchemaInfo.MemberType;
          }
          return null;

          default:
          return null; //Text, PI, Comment etc
        }
      }
    }

    XmlSchemaType IXmlSchemaInfo.SchemaType
    {
      get
      {
        switch (NodeType)
        {
          case XmlNodeType.Element:
          case XmlNodeType.EndElement:
          return xmlSchemaInfo.SchemaType;

          case XmlNodeType.Attribute:
          if (attributePSVI != null)
          {
            return AttributeSchemaInfo.SchemaType;
          }
          return null;

          default:
          return null; //Text, PI, Comment etc
        }
      }
    }
    XmlSchemaElement IXmlSchemaInfo.SchemaElement
    {
      get
      {
        if (NodeType == XmlNodeType.Element || NodeType == XmlNodeType.EndElement)
        {
          return xmlSchemaInfo.SchemaElement;
        }
        return null;
      }
    }

    XmlSchemaAttribute IXmlSchemaInfo.SchemaAttribute
    {
      get
      {
        if (NodeType == XmlNodeType.Attribute)
        {
          if (attributePSVI != null)
          {
            return AttributeSchemaInfo.SchemaAttribute;
          }
        }
        return null;
      }
    }

    //
    // IXmlLineInfo members
    //

    public bool HasLineInfo()
    {
      return true;
    }

    public int LineNumber
    {
      get
      {
        if (lineInfo != null)
        {
          return lineInfo.LineNumber;
        }
        return 0;
      }
    }

    public int LinePosition
    {
      get
      {
        if (lineInfo != null)
        {
          return lineInfo.LinePosition;
        }
        return 0;
      }
    }

    //
    // IXmlNamespaceResolver members
    //
    IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
    {
      if (coreReaderNSResolver != null)
      {
        return coreReaderNSResolver.GetNamespacesInScope(scope);
      }
      else
      {
        return nsManager.GetNamespacesInScope(scope);
      }
    }

    string IXmlNamespaceResolver.LookupNamespace(string prefix)
    {
      if (coreReaderNSResolver != null)
      {
        return coreReaderNSResolver.LookupNamespace(prefix);
      }
      else
      {
        return nsManager.LookupNamespace(prefix);
      }
    }

    string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
    {
      if (coreReaderNSResolver != null)
      {
        return coreReaderNSResolver.LookupPrefix(namespaceName);
      }
      else
      {
        return nsManager.LookupPrefix(namespaceName);
      }
    }

    private object GetStringValue()
    {
      return coreReader.Value;
    }

    private XmlSchemaInfo AttributeSchemaInfo
    {
      get
      {
        return attributePSVI.attributeSchemaInfo;
      }
    }

    private void ProcessReaderEvent()
    {
      if (replayCache)
      { //if in replay mode, do nothing since nodes have been validated already
        //If NodeType == XmlNodeType.EndElement && if manageNamespaces, may need to pop namespace scope, since scope is not popped in ReadAheadForMemberType

        return;

      }
      switch (coreReader.NodeType)
      {
        case XmlNodeType.Element:

        ProcessElementEvent();
        break;

        case XmlNodeType.Whitespace:
        case XmlNodeType.SignificantWhitespace:
        validator.ValidateWhitespace(GetStringValue);
        break;

        case XmlNodeType.Text:          // text inside a node
        case XmlNodeType.CDATA:         // <![CDATA[...]]>
        validator.ValidateText(GetStringValue);
        break;

        case XmlNodeType.EndElement:

        ProcessEndElementEvent();
        break;

        case XmlNodeType.EntityReference:
        throw new InvalidOperationException();

        case XmlNodeType.DocumentType:
        validator.SetDtdSchemaInfo(coreReader.DtdInfo);
        break;

        default:
        break;
      }

    }

    // SxS: This function calls ValidateElement on XmlSchemaValidator which is annotated with ResourceExposure attribute.
    // Since the resource names (namespace location) are not provided directly by the user (they are read from the source
    // document) and the function does not expose any resources it is fine to suppress the SxS warning. 
    [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
    [ResourceExposure(ResourceScope.None)]
    private void ProcessElementEvent()
    {
      if (this.processInlineSchema && IsXSDRoot(coreReader.LocalName, coreReader.NamespaceURI) && coreReader.Depth > 0)
      {
        xmlSchemaInfo.Clear();
        attributeCount = coreReaderAttributeCount = coreReader.AttributeCount;
        if (!coreReader.IsEmptyElement)
        { //If its not empty schema, then parse else ignore
          inlineSchemaParser = new Parser(SchemaType.XSD, coreReaderNameTable, validator.SchemaSet.GetSchemaNames(coreReaderNameTable), validationEvent);
          inlineSchemaParser.StartParsing(coreReader, null);
          inlineSchemaParser.ParseReaderNode();
          validationState = ValidatingReaderState.ParseInlineSchema;
        }
        else
        {
          validationState = ValidatingReaderState.ClearAttributes;
        }
      }
      else
      { //Validate element

        //Clear previous data
        atomicValue = null;
        originalAtomicValueString = null;
        xmlSchemaInfo.Clear();

        if (manageNamespaces)
        {
          nsManager.PushScope();
        }
        //Find Xsi attributes that need to be processed before validating the element
        string xsiSchemaLocation = null;
        string xsiNoNamespaceSL = null;
        string xsiNil = null;
        string xsiType = null;
        if (coreReader.MoveToFirstAttribute())
        {
          do
          {
            string objectNs = coreReader.NamespaceURI;
            string objectName = coreReader.LocalName;
            if (Ref.Equal(objectNs, NsXsi))
            {
              if (Ref.Equal(objectName, XsiSchemaLocation))
              {
                xsiSchemaLocation = coreReader.Value;
              }
              else if (Ref.Equal(objectName, XsiNoNamespaceSchemaLocation))
              {
                xsiNoNamespaceSL = coreReader.Value;
              }
              else if (Ref.Equal(objectName, XsiType))
              {
                xsiType = coreReader.Value;
              }
              else if (Ref.Equal(objectName, XsiNil))
              {
                xsiNil = coreReader.Value;
              }
            }
            if (manageNamespaces && Ref.Equal(coreReader.NamespaceURI, NsXmlNs))
            {
              nsManager.AddNamespace(coreReader.Prefix.Length == 0 ? string.Empty : coreReader.LocalName, coreReader.Value);
            }

          } while (coreReader.MoveToNextAttribute());
          coreReader.MoveToElement();
        }
        validator.ValidateElement(coreReader.LocalName, coreReader.NamespaceURI, xmlSchemaInfo, xsiType, xsiNil, xsiSchemaLocation, xsiNoNamespaceSL);
        ValidateAttributes();
        validator.ValidateEndOfAttributes(xmlSchemaInfo);
        if (coreReader.IsEmptyElement)
        {
          ProcessEndElementEvent();
        }
        validationState = ValidatingReaderState.ClearAttributes;
      }
    }

    private void ProcessEndElementEvent()
    {
      atomicValue = validator.ValidateEndElement(xmlSchemaInfo);
      originalAtomicValueString = GetOriginalAtomicValueStringOfElement();
      if (xmlSchemaInfo.IsDefault)
      { //The atomicValue returned is a default value
        int depth = coreReader.Depth;
        coreReader = GetCachingReader();
        cachingReader.RecordTextNode(xmlSchemaInfo.XmlType.ValueConverter.ToString(atomicValue), originalAtomicValueString, depth + 1, 0, 0);
        cachingReader.RecordEndElementNode();
        cachingReader.SetToReplayMode();
        replayCache = true;
      }
      else if (manageNamespaces)
      {
        nsManager.PopScope();
      }
    }

    private void ValidateAttributes()
    {
      attributeCount = coreReaderAttributeCount = coreReader.AttributeCount;
      AttributePSVIInfo attributePSVI;
      int attIndex = 0;
      bool attributeInvalid = false;
      if (coreReader.MoveToFirstAttribute())
      {
        do
        {
          string localName = coreReader.LocalName;
          string ns = coreReader.NamespaceURI;

          attributePSVI = AddAttributePSVI(attIndex);
          attributePSVI.localName = localName;
          attributePSVI.namespaceUri = ns;

          if ((object)ns == (object)NsXmlNs)
          {
            attIndex++;
            continue;
          }
          attributePSVI.typedAttributeValue = validator.ValidateAttribute(localName, ns, valueGetter, attributePSVI.attributeSchemaInfo);
          if (!attributeInvalid)
          {
            attributeInvalid = attributePSVI.attributeSchemaInfo.Validity == XmlSchemaValidity.Invalid;
          }
          attIndex++;

        } while (coreReader.MoveToNextAttribute());
      }
      coreReader.MoveToElement();
      if (attributeInvalid)
      { //If any of the attributes are invalid, Need to report element's validity as invalid
        xmlSchemaInfo.Validity = XmlSchemaValidity.Invalid;
      }
      validator.GetUnspecifiedDefaultAttributes(defaultAttributes, true);
      attributeCount += defaultAttributes.Count;
    }

    private void ClearAttributesInfo()
    {
      attributeCount = 0;
      coreReaderAttributeCount = 0;
      currentAttrIndex = -1;
      defaultAttributes.Clear();
      attributePSVI = null;
    }

    private AttributePSVIInfo GetAttributePSVI(string name)
    {
      if (inlineSchemaParser != null)
      { //Parsing inline schema, no PSVI for schema attributes
        return null;
      }
      string attrLocalName;
      string attrPrefix;
      string ns;
      ValidateNames.SplitQName(name, out attrPrefix, out attrLocalName);
      attrPrefix = coreReaderNameTable.Add(attrPrefix);
      attrLocalName = coreReaderNameTable.Add(attrLocalName);

      if (attrPrefix.Length == 0)
      { //empty prefix, not qualified
        ns = string.Empty;
      }
      else
      {
        ns = thisNSResolver.LookupNamespace(attrPrefix);
      }
      return GetAttributePSVI(attrLocalName, ns);
    }

    private AttributePSVIInfo GetAttributePSVI(string localName, string ns)
    {
      AttributePSVIInfo attInfo = null;

      for (int i = 0; i < coreReaderAttributeCount; i++)
      {
        attInfo = attributePSVINodes[i];
        if (attInfo != null)
        { //Will be null for invalid attributes
          if (Ref.Equal(localName, attInfo.localName) && Ref.Equal(ns, attInfo.namespaceUri))
          {
            currentAttrIndex = i;
            return attInfo;
          }
        }
      }
      return null;
    }

    private ValidatingReaderNodeData GetDefaultAttribute(string name, bool updatePosition)
    {
      string attrLocalName;
      string attrPrefix;
      ValidateNames.SplitQName(name, out attrPrefix, out attrLocalName);

      //Atomize
      attrPrefix = coreReaderNameTable.Add(attrPrefix);
      attrLocalName = coreReaderNameTable.Add(attrLocalName);
      string ns;
      if (attrPrefix.Length == 0)
      {
        ns = string.Empty;
      }
      else
      {
        ns = thisNSResolver.LookupNamespace(attrPrefix);
      }
      return GetDefaultAttribute(attrLocalName, ns, updatePosition);
    }

    private ValidatingReaderNodeData GetDefaultAttribute(string attrLocalName, string ns, bool updatePosition)
    {
      ValidatingReaderNodeData defaultNode = null;

      for (int i = 0; i < defaultAttributes.Count; i++)
      {
        defaultNode = (ValidatingReaderNodeData)defaultAttributes[i];
        if (Ref.Equal(defaultNode.LocalName, attrLocalName) && Ref.Equal(defaultNode.Namespace, ns))
        {
          if (updatePosition)
          {
            currentAttrIndex = coreReader.AttributeCount + i;
          }
          return defaultNode;
        }
      }
      return null;
    }

    private AttributePSVIInfo AddAttributePSVI(int attIndex)
    {
      AttributePSVIInfo attInfo = attributePSVINodes[attIndex];
      if (attInfo != null)
      {
        attInfo.Reset();
        return attInfo;
      }
      if (attIndex >= attributePSVINodes.Length - 1)
      { //reached capacity of PSVIInfo array, Need to increase capacity to twice the initial
        AttributePSVIInfo[] newPSVINodes = new AttributePSVIInfo[attributePSVINodes.Length * 2];
        Array.Copy(attributePSVINodes, 0, newPSVINodes, 0, attributePSVINodes.Length);
        attributePSVINodes = newPSVINodes;
      }
      attInfo = attributePSVINodes[attIndex];
      if (attInfo == null)
      {
        attInfo = new AttributePSVIInfo();
        attributePSVINodes[attIndex] = attInfo;
      }
      return attInfo;
    }

    private bool IsXSDRoot(string localName, string ns)
    {
      return Ref.Equal(ns, NsXs) && Ref.Equal(localName, XsdSchema);
    }

    private void ProcessInlineSchema()
    {
      if (coreReader.Read())
      {
        if (coreReader.NodeType == XmlNodeType.Element)
        {
          attributeCount = coreReaderAttributeCount = coreReader.AttributeCount;
        }
        else
        { //Clear attributes info if nodeType is not element
          ClearAttributesInfo();
        }
        if (!inlineSchemaParser.ParseReaderNode())
        {
          inlineSchemaParser.FinishParsing();
          XmlSchema schema = inlineSchemaParser.XmlSchema;
          validator.AddSchema(schema);
          inlineSchemaParser = null;
          validationState = ValidatingReaderState.Read;
        }
      }
    }

    private void ReadAheadForMemberType()
    {
      while (coreReader.Read())
      {
        switch (coreReader.NodeType)
        {
          case XmlNodeType.Element:
          break;

          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          validator.ValidateText(GetStringValue);
          break;

          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
          validator.ValidateWhitespace(GetStringValue);
          break;

          case XmlNodeType.Comment:
          case XmlNodeType.ProcessingInstruction:
          break;

          case XmlNodeType.EndElement:
          atomicValue = validator.ValidateEndElement(xmlSchemaInfo); //?? pop namespaceManager scope
          originalAtomicValueString = GetOriginalAtomicValueStringOfElement();
          if (atomicValue == null)
          { //Invalid marker
            atomicValue = this;
          }
          else if (xmlSchemaInfo.IsDefault)
          { //The atomicValue returned is a default value
            cachingReader.SwitchTextNodeAndEndElement(xmlSchemaInfo.XmlType.ValueConverter.ToString(atomicValue), originalAtomicValueString);
          }
          goto breakWhile;
        }
        continue;
      breakWhile:
        break;
      }
    }

    private void GetIsDefault()
    {
      XsdCachingReader cachedReader = coreReader as XsdCachingReader;
      if (cachedReader == null && xmlSchemaInfo.HasDefaultValue)
      { //Get Isdefault
        coreReader = GetCachingReader();
        if (xmlSchemaInfo.IsUnionType && !xmlSchemaInfo.IsNil)
        { //If it also union, get the memberType as well
          ReadAheadForMemberType();
        }
        else
        {
          if (coreReader.Read())
          {
            switch (coreReader.NodeType)
            {
              case XmlNodeType.Element:
              break;

              case XmlNodeType.Text:
              case XmlNodeType.CDATA:
              validator.ValidateText(GetStringValue);
              break;

              case XmlNodeType.Whitespace:
              case XmlNodeType.SignificantWhitespace:
              validator.ValidateWhitespace(GetStringValue);
              break;

              case XmlNodeType.Comment:
              case XmlNodeType.ProcessingInstruction:
              break;

              case XmlNodeType.EndElement:
              atomicValue = validator.ValidateEndElement(xmlSchemaInfo); //?? pop namespaceManager scope
              originalAtomicValueString = GetOriginalAtomicValueStringOfElement();
              if (xmlSchemaInfo.IsDefault)
              { //The atomicValue returned is a default value
                cachingReader.SwitchTextNodeAndEndElement(xmlSchemaInfo.XmlType.ValueConverter.ToString(atomicValue), originalAtomicValueString);
              }
              break;

              default:
              break;
            }
          }
        }
        cachingReader.SetToReplayMode();
        replayCache = true;
      }
    }

    private void GetMemberType()
    {
      if (xmlSchemaInfo.MemberType != null || atomicValue == this)
      {
        return;
      }
      XsdCachingReader cachedReader = coreReader as XsdCachingReader;
      if (cachedReader == null && xmlSchemaInfo.IsUnionType && !xmlSchemaInfo.IsNil)
      {
        coreReader = GetCachingReader();
        ReadAheadForMemberType();
        cachingReader.SetToReplayMode();
        replayCache = true;
      }
    }

    private XsdCachingReader GetCachingReader()
    {
      if (cachingReader == null)
      {
        cachingReader = new XsdCachingReader(coreReader, lineInfo, new CachingEventHandler(CachingCallBack));
      }
      else
      {
        cachingReader.Reset(coreReader);
      }
      this.lineInfo = cachingReader as IXmlLineInfo;
      return cachingReader;
    }

    internal ValidatingReaderNodeData CreateDummyTextNode(string attributeValue, int depth)
    {
      if (textNode == null)
      {
        textNode = new ValidatingReaderNodeData(XmlNodeType.Text);
      }
      textNode.Depth = depth;
      textNode.RawValue = attributeValue;
      return textNode;
    }

    internal void CachingCallBack(XsdCachingReader cachingReader)
    {
      this.coreReader = cachingReader.GetCoreReader(); //re-switch the core-reader after caching reader is done
      this.lineInfo = cachingReader.GetLineInfo();
      replayCache = false;
    }

    private string GetOriginalAtomicValueStringOfElement()
    {
      if (xmlSchemaInfo.IsDefault)
      {
        XmlSchemaElement schemaElem = xmlSchemaInfo.SchemaElement;
        if (schemaElem != null)
        {
          return (schemaElem.DefaultValue != null) ? schemaElem.DefaultValue : schemaElem.FixedValue;
        }
      }
      else
      {
        return validator.GetConcatenatedValue();
      }
      return string.Empty;
    }

  }
}
