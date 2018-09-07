using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// XDocument follows the XPath/XQuery data model.  All nodes in the tree reference the document,
  /// and the document references the root node of the tree.  All namespaces are stored out-of-line,
  /// in an Element --> In-Scope-Namespaces map.
  /// </summary>
  public class XPathDocument : IXPathNavigable
  {
    private XPathNode[] pageText, pageRoot, pageXmlNmsp;
    private int idxText, idxRoot, idxXmlNmsp;
    private NameTable nameTable;
    private bool hasLineInfo;
    private Dictionary<XPathNodeRef, XPathNodeRef> mapNmsp;
    private Dictionary<string, XPathNodeRef> idValueMap;

    /// <summary>
    /// Flags that control Load behavior.
    /// </summary>
    internal enum LoadFlags
    {
      None = 0,
      AtomizeNames = 1,       // Do not assume that names passed to XPathDocumentBuilder have been pre-atomized, and atomize them
      Fragment = 2,           // Create a document with no document node
    }

    /// <summary>
    /// Create a new document and load the content from the Uri, with whitespace handling controlled according to "space".
    /// </summary>
    [ResourceConsumption(ResourceScope.Machine)]
    [ResourceExposure(ResourceScope.Machine)]
    public XPathDocument(string uri, XmlSpace space)
    {
      XmlTextReaderImpl reader = SetupReader(new XmlTextReaderImpl(uri));

      try
      {
        LoadFromReader(reader, space);
      }
      finally
      {
        reader.Close();
      }
    }

    /// <summary>
    /// Create a writer that can be used to create nodes in this document.  The root node will be assigned "baseUri", and flags
    /// can be passed to indicate that names should be atomized by the builder and/or a fragment should be created.
    /// </summary>
    internal void LoadFromReader(FastXmlReader reader, XmlSpace space)
    {
      XPathDocumentBuilder builder;
      IXmlLineInfo lineInfo;
      string xmlnsUri;
      bool topLevelReader;
      int initialDepth;

      if (reader == null)
        throw new ArgumentNullException("reader");

      // Determine line number provider
      lineInfo = reader as IXmlLineInfo;
      if (lineInfo == null || !lineInfo.HasLineInfo())
        lineInfo = null;
      this.hasLineInfo = (lineInfo != null);

      this.nameTable = reader.NameTable;
      builder = new XPathDocumentBuilder(this, lineInfo, reader.BaseURI, LoadFlags.None);

      try
      {
        // Determine whether reader is in initial state
        topLevelReader = (reader.ReadState == ReadState.Initial);
        initialDepth = reader.Depth;

        // Get atomized xmlns uri
        xmlnsUri = this.nameTable.Get(XmlReservedNs.NsXmlNs);

        // Read past Initial state; if there are no more events then load is complete
        if (topLevelReader && !reader.Read())
          return;

        // Read all events
        do
        {
          // If reader began in intermediate state, return when all siblings have been read
          if (!topLevelReader && reader.Depth < initialDepth)
            return;

          switch (reader.NodeType)
          {
            case XmlNodeType.Element:
            {
              bool isEmptyElement = reader.IsEmptyElement;

              builder.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.BaseURI);

              // Add attribute and namespace nodes to element
              while (reader.MoveToNextAttribute())
              {
                string namespaceUri = reader.NamespaceURI;

                if ((object)namespaceUri == (object)xmlnsUri)
                {
                  if (reader.Prefix.Length == 0)
                  {
                    // Default namespace declaration "xmlns"
                    builder.WriteNamespaceDeclaration(string.Empty, reader.Value);
                  }
                  else
                  {
                    builder.WriteNamespaceDeclaration(reader.LocalName, reader.Value);
                  }
                }
                else
                {
                  builder.WriteStartAttribute(reader.Prefix, reader.LocalName, namespaceUri);
                  builder.WriteString(reader.Value, TextBlockType.Text);
                  builder.WriteEndAttribute();
                }
              }

              if (isEmptyElement)
                builder.WriteEndElement(true);
              break;
            }

            case XmlNodeType.EndElement:
            builder.WriteEndElement(false);
            break;

            case XmlNodeType.Text:
            case XmlNodeType.CDATA:
            builder.WriteString(reader.Value, TextBlockType.Text);
            break;

            case XmlNodeType.SignificantWhitespace:
            if (reader.XmlSpace == XmlSpace.Preserve)
              builder.WriteString(reader.Value, TextBlockType.SignificantWhitespace);
            else
              // Significant whitespace without xml:space="preserve" is not significant in XPath/XQuery data model
              goto case XmlNodeType.Whitespace;
            break;

            case XmlNodeType.Whitespace:
            // We intentionally ignore the reader.XmlSpace property here and blindly trust
            //   the reported node type. If the reported information is not in sync
            //   (in this case if the reader.XmlSpace == Preserve) then we make the choice
            //   to trust the reported node type. Since we have no control over the input reader
            //   we can't even assert here.

            // Always filter top-level whitespace
            if (space == XmlSpace.Preserve && (!topLevelReader || reader.Depth != 0))
              builder.WriteString(reader.Value, TextBlockType.Whitespace);
            break;

            case XmlNodeType.Comment:
            builder.WriteComment(reader.Value);
            break;

            case XmlNodeType.ProcessingInstruction:
            builder.WriteProcessingInstruction(reader.LocalName, reader.Value, reader.BaseURI);
            break;

            case XmlNodeType.EntityReference:
            reader.ResolveEntity();
            break;

            case XmlNodeType.DocumentType:
            // Create ID tables
            IDtdInfo info = reader.DtdInfo;
            if (info != null)
              builder.CreateIdTables(info);
            break;

            case XmlNodeType.EndEntity:
            case XmlNodeType.None:
            case XmlNodeType.XmlDeclaration:
            break;
          }
        }
        while (reader.Read());
      }
      finally
      {
        builder.Close();
      }
    }

    /// <summary>
    /// Return the name table used to atomize all name parts (local name, namespace uri, prefix).
    /// </summary>
    internal NameTable NameTable
    {
      get { return this.nameTable; }
    }

    /// <summary>
    /// Set the page and index where the singleton collapsed text node is stored.
    /// </summary>
    internal void SetCollapsedTextNode(XPathNode[] pageText, int idxText)
    {
      this.pageText = pageText;
      this.idxText = idxText;
    }

    /// <summary>
    /// Return the root node of the document.  This may not be a node of type XPathNodeType.Root if this
    /// is a document fragment.
    /// </summary>
    internal int GetRootNode(out XPathNode[] pageRoot)
    {
      pageRoot = this.pageRoot;
      return this.idxRoot;
    }

    /// <summary>
    /// Set the page and index where the root node is stored.
    /// </summary>
    internal void SetRootNode(XPathNode[] pageRoot, int idxRoot)
    {
      this.pageRoot = pageRoot;
      this.idxRoot = idxRoot;
    }

    /// <summary>
    /// Set the page and index where the implicit xmlns:xml node is stored.
    /// </summary>
    internal void SetXmlNamespaceNode(XPathNode[] pageXmlNmsp, int idxXmlNmsp)
    {
      this.pageXmlNmsp = pageXmlNmsp;
      this.idxXmlNmsp = idxXmlNmsp;
    }

    /// <summary>
    /// Associate a namespace node with an element.
    /// </summary>
    internal void AddNamespace(XPathNode[] pageElem, int idxElem, XPathNode[] pageNmsp, int idxNmsp)
    {
      if (this.mapNmsp == null)
        this.mapNmsp = new Dictionary<XPathNodeRef, XPathNodeRef>();

      this.mapNmsp.Add(new XPathNodeRef(pageElem, idxElem), new XPathNodeRef(pageNmsp, idxNmsp));
    }

    /// <summary>
    /// Add an element indexed by ID value.
    /// </summary>
    internal void AddIdElement(string id, XPathNode[] pageElem, int idxElem)
    {
      if (this.idValueMap == null)
        this.idValueMap = new Dictionary<string, XPathNodeRef>();

      if (!this.idValueMap.ContainsKey(id))
        this.idValueMap.Add(id, new XPathNodeRef(pageElem, idxElem));
    }

    /// <summary>
    /// Set properties on the reader so that it is backwards-compatible with V1.
    /// </summary>
    private XmlTextReaderImpl SetupReader(XmlTextReaderImpl reader)
    {
      reader.EntityHandling = EntityHandling.ExpandEntities;
      reader.XmlValidatingReaderCompatibilityMode = true;
      return reader;
    }
  }
}
