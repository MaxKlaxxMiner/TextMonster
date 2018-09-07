using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// This is the default XPath/XQuery data model cache implementation.  It will be used whenever
  /// the user does not supply his own XPathNavigator implementation.
  /// </summary>
  internal sealed class XPathDocumentNavigator : XPathNavigator, IXmlLineInfo
  {
    private XPathNode[] pageCurrent;
    private XPathNode[] pageParent;
    private int idxCurrent;
    private int idxParent;
    private string atomizedLocalName;


    //-----------------------------------------------
    // Constructors
    //-----------------------------------------------

    /// <summary>
    /// Create a new navigator positioned on the specified current node.  If the current node is a namespace or a collapsed
    /// text node, then the parent is a virtualized parent (may be different than .Parent on the current node).
    /// </summary>
    public XPathDocumentNavigator(XPathNode[] pageCurrent, int idxCurrent, XPathNode[] pageParent, int idxParent)
    {
      this.pageCurrent = pageCurrent;
      this.pageParent = pageParent;
      this.idxCurrent = idxCurrent;
      this.idxParent = idxParent;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    public XPathDocumentNavigator(XPathDocumentNavigator nav)
      : this(nav.pageCurrent, nav.idxCurrent, nav.pageParent, nav.idxParent)
    {
      this.atomizedLocalName = nav.atomizedLocalName;
    }


    //-----------------------------------------------
    // XPathItem
    //-----------------------------------------------

    /// <summary>
    /// Get the string value of the current node, computed using data model dm:string-value rules.
    /// If the node has a typed value, return the string representation of the value.  If the node
    /// is not a parent type (comment, text, pi, etc.), get its simple text value.  Otherwise,
    /// concatenate all text node descendants of the current node.
    /// </summary>
    public override string Value
    {
      get
      {
        string value;
        XPathNode[] page, pageEnd;
        int idx, idxEnd;

        // Try to get the pre-computed string value of the node
        value = this.pageCurrent[this.idxCurrent].Value;
        if (value != null)
          return value;

        // If current node is collapsed text, then parent element has a simple text value
        if (this.idxParent != 0)
        {
          return this.pageParent[this.idxParent].Value;
        }

        // Must be node with complex content, so concatenate the string values of all text descendants
        string s = string.Empty;
        StringBuilder bldr = null;

        // Get all text nodes which follow the current node in document order, but which are still descendants
        page = pageEnd = this.pageCurrent;
        idx = idxEnd = this.idxCurrent;
        if (!XPathNodeHelper.GetNonDescendant(ref pageEnd, ref idxEnd))
        {
          pageEnd = null;
          idxEnd = 0;
        }

        while (XPathNodeHelper.GetTextFollowing(ref page, ref idx, pageEnd, idxEnd))
        {
          if (s.Length == 0)
          {
            s = page[idx].Value;
          }
          else
          {
            if (bldr == null)
            {
              bldr = new StringBuilder();
              bldr.Append(s);
            }
            bldr.Append(page[idx].Value);
          }
        }

        return (bldr != null) ? bldr.ToString() : s;
      }
    }


    //-----------------------------------------------
    // XPathNavigator
    //-----------------------------------------------

    /// <summary>
    /// Create a copy of this navigator, positioned to the same node in the tree.
    /// </summary>
    public override XPathNavigator Clone()
    {
      return new XPathDocumentNavigator(this.pageCurrent, this.idxCurrent, this.pageParent, this.idxParent);
    }

    /// <summary>
    /// Get the XPath node type of the current node.
    /// </summary>
    public override XPathNodeType NodeType
    {
      get { return this.pageCurrent[this.idxCurrent].NodeType; }
    }

    /// <summary>
    /// Get the local name portion of the current node's name.
    /// </summary>
    public override string LocalName
    {
      get { return this.pageCurrent[this.idxCurrent].LocalName; }
    }

    /// <summary>
    /// Get the namespace portion of the current node's name.
    /// </summary>
    public override string NamespaceURI
    {
      get { return this.pageCurrent[this.idxCurrent].NamespaceUri; }
    }

    /// <summary>
    /// Get the name of the current node.
    /// </summary>
    public override string Name
    {
      get { return this.pageCurrent[this.idxCurrent].Name; }
    }

    /// <summary>
    /// Return the xml name table which was used to atomize all prefixes, local-names, and
    /// namespace uris in the document.
    /// </summary>
    public override XmlNameTable NameTable
    {
      get { return this.pageCurrent[this.idxCurrent].Document.NameTable; }
    }

    /// <summary>
    /// Position the navigator on the namespace within the specified scope.  If no matching namespace
    /// can be found, return false.
    /// </summary>
    public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
    {
      XPathNode[] page;
      int idx;

      if (namespaceScope == XPathNamespaceScope.Local)
      {
        // Get local namespaces only
        idx = XPathNodeHelper.GetLocalNamespaces(this.pageCurrent, this.idxCurrent, out page);
      }
      else
      {
        // Get all in-scope namespaces
        idx = XPathNodeHelper.GetInScopeNamespaces(this.pageCurrent, this.idxCurrent, out page);
      }

      while (idx != 0)
      {
        // Don't include the xmlns:xml namespace node if scope is ExcludeXml
        if (namespaceScope != XPathNamespaceScope.ExcludeXml || !page[idx].IsXmlNamespaceNode)
        {
          this.pageParent = this.pageCurrent;
          this.idxParent = this.idxCurrent;
          this.pageCurrent = page;
          this.idxCurrent = idx;
          return true;
        }

        // Skip past xmlns:xml
        idx = page[idx].GetSibling(out page);
      }

      return false;
    }

    /// <summary>
    /// Position the navigator on the next namespace within the specified scope.  If no matching namespace
    /// can be found, return false.
    /// </summary>
    public override bool MoveToNextNamespace(XPathNamespaceScope scope)
    {
      XPathNode[] page = this.pageCurrent, pageParent;
      int idx = this.idxCurrent, idxParent;

      // If current node is not a namespace node, return false
      if (page[idx].NodeType != XPathNodeType.Namespace)
        return false;

      while (true)
      {
        // Get next namespace sibling
        idx = page[idx].GetSibling(out page);

        // If there are no more nodes, return false
        if (idx == 0)
          return false;

        switch (scope)
        {
          case XPathNamespaceScope.Local:
          // Once parent changes, there are no longer any local namespaces
          idxParent = page[idx].GetParent(out pageParent);
          if (idxParent != this.idxParent || (object)pageParent != (object)this.pageParent)
            return false;
          break;

          case XPathNamespaceScope.ExcludeXml:
          // If node is xmlns:xml, then skip it
          if (page[idx].IsXmlNamespaceNode)
            continue;
          break;
        }

        // Found a matching next namespace node, so return it
        break;
      }

      this.pageCurrent = page;
      this.idxCurrent = idx;
      return true;
    }

    /// <summary>
    /// If the current node is an attribute or namespace (not content), return false.  Otherwise,
    /// move to the next content node.  Return false if there are no more content nodes.
    /// </summary>
    public override bool MoveToNext()
    {
      return XPathNodeHelper.GetContentSibling(ref this.pageCurrent, ref this.idxCurrent);
    }

    /// <summary>
    /// Move to the first content-typed child of the current node.  Return false if the current
    /// node has no content children.
    /// </summary>
    public override bool MoveToFirstChild()
    {
      if (this.pageCurrent[this.idxCurrent].HasCollapsedText)
      {
        // Virtualize collapsed text nodes
        this.pageParent = this.pageCurrent;
        this.idxParent = this.idxCurrent;
        this.idxCurrent = this.pageCurrent[this.idxCurrent].Document.GetCollapsedTextNode(out this.pageCurrent);
        return true;
      }

      return XPathNodeHelper.GetContentChild(ref this.pageCurrent, ref this.idxCurrent);
    }

    /// <summary>
    /// Position the navigator on the parent of the current node.  If the current node has no parent,
    /// return false.
    /// </summary>
    public override bool MoveToParent()
    {
      if (this.idxParent != 0)
      {
        // 1. For attribute nodes, element parent is always stored in order to make node-order
        //    comparison simpler.
        // 2. For namespace nodes, parent is always stored in navigator in order to virtualize
        //    XPath 1.0 namespaces.
        // 3. For collapsed text nodes, element parent is always stored in navigator.
        this.pageCurrent = this.pageParent;
        this.idxCurrent = this.idxParent;
        this.pageParent = null;
        this.idxParent = 0;
        return true;
      }

      return XPathNodeHelper.GetParent(ref this.pageCurrent, ref this.idxCurrent);
    }

    /// <summary>
    /// Position this navigator to the same position as the "other" navigator.  If the "other" navigator
    /// is not of the same type as this navigator, then return false.
    /// </summary>
    public override bool MoveTo(XPathNavigator other)
    {
      XPathDocumentNavigator that = other as XPathDocumentNavigator;
      if (that != null)
      {
        this.pageCurrent = that.pageCurrent;
        this.idxCurrent = that.idxCurrent;
        this.pageParent = that.pageParent;
        this.idxParent = that.idxParent;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Return true if line number information is recorded in the cache.
    /// </summary>
    public bool HasLineInfo()
    {
      return this.pageCurrent[this.idxCurrent].Document.HasLineInfo;
    }

    /// <summary>
    /// Return the source line number of the current node.
    /// </summary>
    public int LineNumber
    {
      get
      {
        // If the current node is a collapsed text node, then return parent element's line number
        if (this.idxParent != 0 && NodeType == XPathNodeType.Text)
          return this.pageParent[this.idxParent].LineNumber;

        return this.pageCurrent[this.idxCurrent].LineNumber;
      }
    }

    /// <summary>
    /// Return the source line position of the current node.
    /// </summary>
    public int LinePosition
    {
      get
      {
        // If the current node is a collapsed text node, then get position from parent element
        if (this.idxParent != 0 && NodeType == XPathNodeType.Text)
          return this.pageParent[this.idxParent].CollapsedLinePosition;

        return this.pageCurrent[this.idxCurrent].LinePosition;
      }
    }
  }
}
