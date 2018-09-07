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
    /// Returns true if this navigator is positioned to the same node as the "other" navigator.  Returns false
    /// if not, or if the "other" navigator is not the same type as this navigator.
    /// </summary>
    public override bool IsSamePosition(XPathNavigator other)
    {
      XPathDocumentNavigator that = other as XPathDocumentNavigator;
      if (that != null)
      {
        return this.idxCurrent == that.idxCurrent && this.pageCurrent == that.pageCurrent &&
               this.idxParent == that.idxParent && this.pageParent == that.pageParent;
      }
      return false;
    }

    /// <summary>
    /// Move to the first element child of the current node with the specified name.  Return false
    /// if the current node has no matching element children.
    /// </summary>
    public override bool MoveToChild(string localName, string namespaceURI)
    {
      if ((object)localName != (object)this.atomizedLocalName)
        this.atomizedLocalName = (localName != null) ? NameTable.Get(localName) : null;

      return XPathNodeHelper.GetElementChild(ref this.pageCurrent, ref this.idxCurrent, this.atomizedLocalName, namespaceURI);
    }

    /// <summary>
    /// Move to the first element sibling of the current node with the specified name.  Return false
    /// if the current node has no matching element siblings.
    /// </summary>
    public override bool MoveToNext(string localName, string namespaceURI)
    {
      if ((object)localName != (object)this.atomizedLocalName)
        this.atomizedLocalName = (localName != null) ? NameTable.Get(localName) : null;

      return XPathNodeHelper.GetElementSibling(ref this.pageCurrent, ref this.idxCurrent, this.atomizedLocalName, namespaceURI);
    }

    /// <summary>
    /// Move to the next element that:
    ///   1. Follows the current node in document order (includes descendants, unlike XPath following axis)
    ///   2. Precedes "end" in document order (if end is null, then all following nodes in the document are considered)
    ///   3. Has the specified QName
    /// Return false if the current node has no matching following elements.
    /// </summary>
    public override bool MoveToFollowing(string localName, string namespaceURI, XPathNavigator end)
    {
      XPathNode[] pageEnd;
      int idxEnd;

      if ((object)localName != (object)this.atomizedLocalName)
        this.atomizedLocalName = (localName != null) ? NameTable.Get(localName) : null;

      // Get node on which scan ends (null if rest of document should be scanned)
      idxEnd = GetFollowingEnd(end as XPathDocumentNavigator, false, out pageEnd);

      // If this navigator is positioned on a virtual node, then compute following of parent
      if (this.idxParent != 0)
      {
        if (!XPathNodeHelper.GetElementFollowing(ref this.pageParent, ref this.idxParent, pageEnd, idxEnd, this.atomizedLocalName, namespaceURI))
          return false;

        this.pageCurrent = this.pageParent;
        this.idxCurrent = this.idxParent;
        this.pageParent = null;
        this.idxParent = 0;
        return true;
      }

      return XPathNodeHelper.GetElementFollowing(ref this.pageCurrent, ref this.idxCurrent, pageEnd, idxEnd, this.atomizedLocalName, namespaceURI);
    }

    /// <summary>
    /// Move to the next node that:
    ///   1. Follows the current node in document order (includes descendants, unlike XPath following axis)
    ///   2. Precedes "end" in document order (if end is null, then all following nodes in the document are considered)
    ///   3. Has the specified XPathNodeType
    /// Return false if the current node has no matching following nodes.
    /// </summary>
    public override bool MoveToFollowing(XPathNodeType type, XPathNavigator end)
    {
      XPathDocumentNavigator endTiny = end as XPathDocumentNavigator;
      XPathNode[] page, pageEnd;
      int idx, idxEnd;

      // If searching for text, make sure to handle collapsed text nodes correctly
      if (type == XPathNodeType.Text || type == XPathNodeType.All)
      {
        if (this.pageCurrent[this.idxCurrent].HasCollapsedText)
        {
          // Positioned on an element with collapsed text, so return the virtual text node, assuming it's before "end"
          if (endTiny != null && this.idxCurrent == endTiny.idxParent && this.pageCurrent == endTiny.pageParent)
          {
            // "end" is positioned to a virtual attribute, namespace, or text node
            return false;
          }

          this.pageParent = this.pageCurrent;
          this.idxParent = this.idxCurrent;
          this.idxCurrent = this.pageCurrent[this.idxCurrent].Document.GetCollapsedTextNode(out this.pageCurrent);
          return true;
        }

        if (type == XPathNodeType.Text)
        {
          // Get node on which scan ends (null if rest of document should be scanned, parent if positioned on virtual node)
          idxEnd = GetFollowingEnd(endTiny, true, out pageEnd);

          // If this navigator is positioned on a virtual node, then compute following of parent
          if (this.idxParent != 0)
          {
            page = this.pageParent;
            idx = this.idxParent;
          }
          else
          {
            page = this.pageCurrent;
            idx = this.idxCurrent;
          }

          // If ending node is a virtual node, and current node is its parent, then we're done
          if (endTiny != null && endTiny.idxParent != 0 && idx == idxEnd && page == pageEnd)
            return false;

          // Get all virtual (collapsed) and physical text nodes which follow the current node
          if (!XPathNodeHelper.GetTextFollowing(ref page, ref idx, pageEnd, idxEnd))
            return false;

          if (page[idx].NodeType == XPathNodeType.Element)
          {
            // Virtualize collapsed text nodes
            this.idxCurrent = page[idx].Document.GetCollapsedTextNode(out this.pageCurrent);
            this.pageParent = page;
            this.idxParent = idx;
          }
          else
          {
            // Physical text node
            this.pageCurrent = page;
            this.idxCurrent = idx;
            this.pageParent = null;
            this.idxParent = 0;
          }
          return true;
        }
      }

      // Get node on which scan ends (null if rest of document should be scanned, parent + 1 if positioned on virtual node)
      idxEnd = GetFollowingEnd(endTiny, false, out pageEnd);

      // If this navigator is positioned on a virtual node, then compute following of parent
      if (this.idxParent != 0)
      {
        if (!XPathNodeHelper.GetContentFollowing(ref this.pageParent, ref this.idxParent, pageEnd, idxEnd, type))
          return false;

        this.pageCurrent = this.pageParent;
        this.idxCurrent = this.idxParent;
        this.pageParent = null;
        this.idxParent = 0;
        return true;
      }

      return XPathNodeHelper.GetContentFollowing(ref this.pageCurrent, ref this.idxCurrent, pageEnd, idxEnd, type);
    }

    public override object UnderlyingObject
    {
      get
      {
        // Since we don't have any underlying PUBLIC object
        //   the best one we can return is a clone of the navigator.
        // Note that it should be a clone as the user might Move the returned navigator
        //   around and thus cause unexpected behavior of the caller of this class (For example the validator)
        return this.Clone();
      }
    }

    //-----------------------------------------------
    // IXmlLineInfo
    //-----------------------------------------------

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

    /// <summary>
    /// Return true if navigator is positioned to an element having the specified name.
    /// </summary>
    public bool IsElementMatch(string localName, string namespaceURI)
    {
      if ((object)localName != (object)this.atomizedLocalName)
        this.atomizedLocalName = (localName != null) ? NameTable.Get(localName) : null;

      // Cannot be an element if parent is stored
      if (this.idxParent != 0)
        return false;

      return this.pageCurrent[this.idxCurrent].ElementMatch(this.atomizedLocalName, namespaceURI);
    }

    /// <summary>
    /// Return true if navigator is positioned to a node of the specified kind.  Whitespace/SignficantWhitespace/Text are
    /// all treated the same (i.e. they all match each other).
    /// </summary>
    public bool IsKindMatch(XPathNodeType typ)
    {
      return (((1 << (int)this.pageCurrent[this.idxCurrent].NodeType) & GetKindMask(typ)) != 0);
    }

    /// <summary>
    /// "end" is positioned on a node which terminates a following scan.  Return the page and index of "end" if it
    /// is positioned to a non-virtual node.  If "end" is positioned to a virtual node:
    ///    1. If useParentOfVirtual is true, then return the page and index of the virtual node's parent
    ///    2. If useParentOfVirtual is false, then return the page and index of the virtual node's parent + 1.
    /// </summary>
    private int GetFollowingEnd(XPathDocumentNavigator end, bool useParentOfVirtual, out XPathNode[] pageEnd)
    {
      // If ending navigator is positioned to a node in another document, then return null
      if (end != null && this.pageCurrent[this.idxCurrent].Document == end.pageCurrent[end.idxCurrent].Document)
      {

        // If the ending navigator is not positioned on a virtual node, then return its current node
        if (end.idxParent == 0)
        {
          pageEnd = end.pageCurrent;
          return end.idxCurrent;
        }

        // If the ending navigator is positioned on an attribute, namespace, or virtual text node, then use the
        // next physical node instead, as the results will be the same.
        pageEnd = end.pageParent;
        return (useParentOfVirtual) ? end.idxParent : end.idxParent + 1;
      }

      // No following, so set pageEnd to null and return an index of 0
      pageEnd = null;
      return 0;
    }
  }
}
