namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Library of XPathNode helper routines.
  /// </summary>
  internal abstract class XPathNodeHelper
  {

    /// <summary>
    /// Return chain of namespace nodes.  If specified node has no local namespaces, then 0 will be
    /// returned.  Otherwise, the first node in the chain is guaranteed to be a local namespace (its
    /// parent is this node).  Subsequent nodes may not have the same node as parent, so the caller will
    /// need to test the parent in order to terminate a search that processes only local namespaces.
    /// </summary>
    public static int GetLocalNamespaces(XPathNode[] pageElem, int idxElem, out XPathNode[] pageNmsp)
    {
      if (pageElem[idxElem].HasNamespaceDecls)
      {
        // Only elements have namespace nodes
        return pageElem[idxElem].Document.LookupNamespaces(pageElem, idxElem, out pageNmsp);
      }
      pageNmsp = null;
      return 0;
    }

    /// <summary>
    /// Return chain of in-scope namespace nodes for nodes of type Element.  Nodes in the chain might not
    /// have this element as their parent.  Since the xmlns:xml namespace node is always in scope, this
    /// method will never return 0 if the specified node is an element.
    /// </summary>
    public static int GetInScopeNamespaces(XPathNode[] pageElem, int idxElem, out XPathNode[] pageNmsp)
    {
      XPathDocument doc;

      // Only elements have namespace nodes
      if (pageElem[idxElem].NodeType == XPathNodeType.Element)
      {
        doc = pageElem[idxElem].Document;

        // Walk ancestors, looking for an ancestor that has at least one namespace declaration
        while (!pageElem[idxElem].HasNamespaceDecls)
        {
          idxElem = pageElem[idxElem].GetParent(out pageElem);
          if (idxElem == 0)
          {
            // There are no namespace nodes declared on ancestors, so return xmlns:xml node
            return doc.GetXmlNamespaceNode(out pageNmsp);
          }
        }
        // Return chain of in-scope namespace nodes
        return doc.LookupNamespaces(pageElem, idxElem, out pageNmsp);
      }
      pageNmsp = null;
      return 0;
    }

    /// <summary>
    /// Return the first content-typed child of the specified node.  If the node has no children, or
    /// if the node is not content-typed, then do not set pageNode or idxNode and return false.
    /// </summary>
    public static bool GetContentChild(ref XPathNode[] pageNode, ref int idxNode)
    {
      XPathNode[] page = pageNode;
      int idx = idxNode;

      if (page[idx].HasContentChild)
      {
        GetChild(ref page, ref idx);

        // Skip past attribute children
        while (page[idx].NodeType == XPathNodeType.Attribute)
        {
          idx = page[idx].GetSibling(out page);
        }

        pageNode = page;
        idxNode = idx;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Return the next content-typed sibling of the specified node.  If the node has no siblings, or
    /// if the node is not content-typed, then do not set pageNode or idxNode and return false.
    /// </summary>
    public static bool GetContentSibling(ref XPathNode[] pageNode, ref int idxNode)
    {
      XPathNode[] page = pageNode;
      int idx = idxNode;

      if (!page[idx].IsAttrNmsp)
      {
        idx = page[idx].GetSibling(out page);
        if (idx != 0)
        {
          pageNode = page;
          idxNode = idx;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Return the parent of the specified node.  If the node has no parent, do not set pageNode
    /// or idxNode and return false.
    /// </summary>
    public static bool GetParent(ref XPathNode[] pageNode, ref int idxNode)
    {
      XPathNode[] page = pageNode;
      int idx = idxNode;

      idx = page[idx].GetParent(out page);
      if (idx != 0)
      {
        pageNode = page;
        idxNode = idx;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Return a location integer that can be easily compared with other locations from the same document
    /// in order to determine the relative document order of two nodes.
    /// </summary>
    public static int GetLocation(XPathNode[] pageNode, int idxNode)
    {
      return (pageNode[0].PageInfo.PageNumber << 16) | idxNode;
    }

    /// <summary>
    /// Scan all nodes that follow the current node in document order, but precede the ending node in document order.
    /// Return two types of nodes with non-null text:
    ///   1. Element parents of collapsed text nodes (since it is the element parent that has the collapsed text)
    ///   2. Non-collapsed text nodes
    /// If no such node exists, then do not set pageCurrent or idxCurrent and return false.
    /// </summary>
    public static bool GetTextFollowing(ref XPathNode[] pageCurrent, ref int idxCurrent, XPathNode[] pageEnd, int idxEnd)
    {
      XPathNode[] page = pageCurrent;
      int idx = idxCurrent;

      // Since nodes are laid out in document order on pages, scan them sequentially
      // rather than following sibling/child/parent links.
      idx++;
      do
      {
        if ((object)page == (object)pageEnd && idx <= idxEnd)
        {
          // Only scan to termination point
          while (idx != idxEnd)
          {
            if (page[idx].IsText || (page[idx].NodeType == XPathNodeType.Element && page[idx].HasCollapsedText))
              goto FoundNode;
            idx++;
          }
          break;
        }
        else
        {
          // Scan all nodes in the page
          while (idx < page[0].PageInfo.NodeCount)
          {
            if (page[idx].IsText || (page[idx].NodeType == XPathNodeType.Element && page[idx].HasCollapsedText))
              goto FoundNode;
            idx++;
          }
        }

        page = page[0].PageInfo.NextPage;
        idx = 1;
      }
      while (page != null);

      return false;

    FoundNode:
      // Found match
      pageCurrent = page;
      idxCurrent = idx;
      return true;
    }

    /// <summary>
    /// Get the next non-virtual (not collapsed text, not namespaces) node that follows the specified node in document order,
    /// but is not a descendant.  If no such node exists, then do not set pageNode or idxNode and return false.
    /// </summary>
    public static bool GetNonDescendant(ref XPathNode[] pageNode, ref int idxNode)
    {
      XPathNode[] page = pageNode;
      int idx = idxNode;

      // Get page, idx at which to end sequential scan of nodes
      do
      {
        // If the current node has a sibling,
        if (page[idx].HasSibling)
        {
          // Then that is the first non-descendant
          pageNode = page;
          idxNode = page[idx].GetSibling(out pageNode);
          return true;
        }

        // Otherwise, try finding a sibling at the parent level
        idx = page[idx].GetParent(out page);
      }
      while (idx != 0);

      return false;
    }

    /// <summary>
    /// Return the page and index of the first child (attribute or content) of the specified node.
    /// </summary>
    private static void GetChild(ref XPathNode[] pageNode, ref int idxNode)
    {
      if (++idxNode >= pageNode.Length)
      {
        // Child is first node on next page
        pageNode = pageNode[0].PageInfo.NextPage;
        idxNode = 1;
      }
      // Else child is next node on this page
    }
  }
}
