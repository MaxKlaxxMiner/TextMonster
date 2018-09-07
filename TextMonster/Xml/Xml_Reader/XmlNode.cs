using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  // Represents a single node in the document.
  [DebuggerDisplay("{debuggerDisplayProxy}")]
  public abstract class XmlNode : ICloneable, IEnumerable, IXPathNavigable
  {
    internal XmlNode parentNode; //this pointer is reused to save the userdata information, need to prevent internal user access the pointer directly.

    internal XmlNode()
    {
    }

    internal XmlNode(XmlDocument doc)
    {
      if (doc == null)
        throw new ArgumentException(Res.GetString(Res.Xdom_Node_Null_Doc));
      this.parentNode = doc;
    }

    // Gets the name of the node.
    public abstract string Name
    {
      get;
    }

    // Gets or sets the value of the node.
    public virtual string Value
    {
      get { return null; }
      set { throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Res.GetString(Res.Xdom_Node_SetVal), NodeType.ToString())); }
    }

    // Gets the type of the current node.
    public abstract XmlNodeType NodeType
    {
      get;
    }

    // Gets the parent of this node (for nodes that can have parents).
    public virtual XmlNode ParentNode
    {
      get
      {
        Debug.Assert(parentNode != null);

        if (parentNode.NodeType != XmlNodeType.Document)
        {
          return parentNode;
        }

        // Linear lookup through the children of the document
        XmlLinkedNode firstChild = parentNode.FirstChild as XmlLinkedNode;
        if (firstChild != null)
        {
          XmlLinkedNode node = firstChild;
          do
          {
            if (node == this)
            {
              return parentNode;
            }
            node = node.next;
          }
          while (node != null
                 && node != firstChild);
        }
        return null;
      }
    }

    // Gets all children of this node.
    public XmlNodeList ChildNodes
    {
      get { return new XmlChildNodes(this); }
    }

    // Gets the node immediately preceding this node.
    public virtual XmlNode PreviousSibling
    {
      get { return null; }
    }

    // Gets the node immediately following this node.
    public virtual XmlNode NextSibling
    {
      get { return null; }
    }

    // Gets a XmlAttributeCollection containing the attributes
    // of this node.
    public virtual XmlAttributeCollection Attributes
    {
      get { return null; }
    }

    // Gets the XmlDocument that contains this node.
    public virtual XmlDocument OwnerDocument
    {
      get
      {
        Debug.Assert(parentNode != null);
        if (parentNode.NodeType == XmlNodeType.Document)
          return (XmlDocument)parentNode;
        return parentNode.OwnerDocument;
      }
    }

    // Gets the first child of this node.
    public XmlNode FirstChild
    {
      get
      {
        XmlLinkedNode linkedNode = LastNode;
        if (linkedNode != null)
          return linkedNode.next;

        return null;
      }
    }

    // Gets the last child of this node.
    public XmlNode LastChild
    {
      get { return LastNode; }
    }

    internal virtual bool IsContainer
    {
      get { return false; }
    }

    internal virtual XmlLinkedNode LastNode
    {
      get { return null; }
      set { }
    }

    internal bool AncestorNode(XmlNode node)
    {
      XmlNode n = this.ParentNode;

      while (n != null && n != this)
      {
        if (n == node)
          return true;
        n = n.ParentNode;
      }

      return false;
    }

    public virtual XmlNode InsertBefore(XmlNode newChild, XmlNode refChild)
    {
      if (this == newChild || AncestorNode(newChild))
        throw new ArgumentException(Res.GetString(Res.Xdom_Node_Insert_Child));

      if (refChild == null)
        return AppendChild(newChild);

      if (!IsContainer)
        throw new InvalidOperationException(Res.GetString(Res.Xdom_Node_Insert_Contain));

      if (refChild.ParentNode != this)
        throw new ArgumentException(Res.GetString(Res.Xdom_Node_Insert_Path));

      if (newChild == refChild)
        return newChild;

      XmlDocument childDoc = newChild.OwnerDocument;
      XmlDocument thisDoc = OwnerDocument;
      if (childDoc != null && childDoc != thisDoc && childDoc != this)
        throw new ArgumentException(Res.GetString(Res.Xdom_Node_Insert_Context));

      if (!CanInsertBefore(newChild, refChild))
        throw new InvalidOperationException(Res.GetString(Res.Xdom_Node_Insert_Location));

      if (newChild.ParentNode != null)
        newChild.ParentNode.RemoveChild(newChild);

      // special case for doc-fragment.
      if (newChild.NodeType == XmlNodeType.DocumentFragment)
      {
        XmlNode first = newChild.FirstChild;
        XmlNode node = first;
        if (node != null)
        {
          newChild.RemoveChild(node);
          InsertBefore(node, refChild);
          // insert the rest of the children after this one.
          InsertAfter(newChild, node);
        }
        return first;
      }

      if (!(newChild is XmlLinkedNode) || !IsValidChildType(newChild.NodeType))
        throw new InvalidOperationException(Res.GetString(Res.Xdom_Node_Insert_TypeConflict));

      XmlLinkedNode newNode = (XmlLinkedNode)newChild;
      XmlLinkedNode refNode = (XmlLinkedNode)refChild;

      string newChildValue = newChild.Value;
      XmlNodeChangedEventArgs args = GetEventArgs(newChild, newChild.ParentNode, this, newChildValue, newChildValue, XmlNodeChangedAction.Insert);

      if (args != null)
        BeforeEvent(args);

      if (refNode == FirstChild)
      {
        newNode.next = refNode;
        LastNode.next = newNode;
        newNode.SetParent(this);

        if (newNode.IsText)
        {
          if (refNode.IsText)
          {
            NestTextNodes(newNode, refNode);
          }
        }
      }
      else
      {
        XmlLinkedNode prevNode = (XmlLinkedNode)refNode.PreviousSibling;

        newNode.next = refNode;
        prevNode.next = newNode;
        newNode.SetParent(this);

        if (prevNode.IsText)
        {
          if (newNode.IsText)
          {
            NestTextNodes(prevNode, newNode);
            if (refNode.IsText)
            {
              NestTextNodes(newNode, refNode);
            }
          }
          else
          {
            if (refNode.IsText)
            {
              UnnestTextNodes(prevNode, refNode);
            }
          }
        }
        else
        {
          if (newNode.IsText)
          {
            if (refNode.IsText)
            {
              NestTextNodes(newNode, refNode);
            }
          }
        }
      }

      if (args != null)
        AfterEvent(args);

      return newNode;
    }

    // Inserts the specified node immediately after the specified reference node.
    public virtual XmlNode InsertAfter(XmlNode newChild, XmlNode refChild)
    {
      if (this == newChild || AncestorNode(newChild))
        throw new ArgumentException(Res.GetString(Res.Xdom_Node_Insert_Child));

      if (refChild == null)
        return PrependChild(newChild);

      if (!IsContainer)
        throw new InvalidOperationException(Res.GetString(Res.Xdom_Node_Insert_Contain));

      if (refChild.ParentNode != this)
        throw new ArgumentException(Res.GetString(Res.Xdom_Node_Insert_Path));

      if (newChild == refChild)
        return newChild;

      XmlDocument childDoc = newChild.OwnerDocument;
      XmlDocument thisDoc = OwnerDocument;
      if (childDoc != null && childDoc != thisDoc && childDoc != this)
        throw new ArgumentException(Res.GetString(Res.Xdom_Node_Insert_Context));

      if (!CanInsertAfter(newChild, refChild))
        throw new InvalidOperationException(Res.GetString(Res.Xdom_Node_Insert_Location));

      if (newChild.ParentNode != null)
        newChild.ParentNode.RemoveChild(newChild);

      // special case for doc-fragment.
      if (newChild.NodeType == XmlNodeType.DocumentFragment)
      {
        XmlNode last = refChild;
        XmlNode first = newChild.FirstChild;
        XmlNode node = first;
        while (node != null)
        {
          XmlNode next = node.NextSibling;
          newChild.RemoveChild(node);
          InsertAfter(node, last);
          last = node;
          node = next;
        }
        return first;
      }

      if (!(newChild is XmlLinkedNode) || !IsValidChildType(newChild.NodeType))
        throw new InvalidOperationException(Res.GetString(Res.Xdom_Node_Insert_TypeConflict));

      XmlLinkedNode newNode = (XmlLinkedNode)newChild;
      XmlLinkedNode refNode = (XmlLinkedNode)refChild;

      string newChildValue = newChild.Value;
      XmlNodeChangedEventArgs args = GetEventArgs(newChild, newChild.ParentNode, this, newChildValue, newChildValue, XmlNodeChangedAction.Insert);

      if (args != null)
        BeforeEvent(args);

      if (refNode == LastNode)
      {
        newNode.next = refNode.next;
        refNode.next = newNode;
        LastNode = newNode;
        newNode.SetParent(this);

        if (refNode.IsText)
        {
          if (newNode.IsText)
          {
            NestTextNodes(refNode, newNode);
          }
        }
      }
      else
      {
        XmlLinkedNode nextNode = refNode.next;

        newNode.next = nextNode;
        refNode.next = newNode;
        newNode.SetParent(this);

        if (refNode.IsText)
        {
          if (newNode.IsText)
          {
            NestTextNodes(refNode, newNode);
            if (nextNode.IsText)
            {
              NestTextNodes(newNode, nextNode);
            }
          }
          else
          {
            if (nextNode.IsText)
            {
              UnnestTextNodes(refNode, nextNode);
            }
          }
        }
        else
        {
          if (newNode.IsText)
          {
            if (nextNode.IsText)
            {
              NestTextNodes(newNode, nextNode);
            }
          }
        }
      }


      if (args != null)
        AfterEvent(args);

      return newNode;
    }

    public virtual XmlNode RemoveChild(XmlNode oldChild)
    {
      if (!IsContainer)
        throw new InvalidOperationException(Res.GetString(Res.Xdom_Node_Remove_Contain));

      if (oldChild.ParentNode != this)
        throw new ArgumentException(Res.GetString(Res.Xdom_Node_Remove_Child));

      XmlLinkedNode oldNode = (XmlLinkedNode)oldChild;

      string oldNodeValue = oldNode.Value;
      XmlNodeChangedEventArgs args = GetEventArgs(oldNode, this, null, oldNodeValue, oldNodeValue, XmlNodeChangedAction.Remove);

      if (args != null)
        BeforeEvent(args);

      XmlLinkedNode lastNode = LastNode;

      if (oldNode == FirstChild)
      {
        if (oldNode == lastNode)
        {
          LastNode = null;
          oldNode.next = null;
          oldNode.SetParent(null);
        }
        else
        {
          XmlLinkedNode nextNode = oldNode.next;

          if (nextNode.IsText)
          {
            if (oldNode.IsText)
            {
              UnnestTextNodes(oldNode, nextNode);
            }
          }

          lastNode.next = nextNode;
          oldNode.next = null;
          oldNode.SetParent(null);
        }
      }
      else
      {
        if (oldNode == lastNode)
        {
          XmlLinkedNode prevNode = (XmlLinkedNode)oldNode.PreviousSibling;
          prevNode.next = oldNode.next;
          LastNode = prevNode;
          oldNode.next = null;
          oldNode.SetParent(null);
        }
        else
        {
          XmlLinkedNode prevNode = (XmlLinkedNode)oldNode.PreviousSibling;
          XmlLinkedNode nextNode = oldNode.next;

          if (nextNode.IsText)
          {
            if (prevNode.IsText)
            {
              NestTextNodes(prevNode, nextNode);
            }
            else
            {
              if (oldNode.IsText)
              {
                UnnestTextNodes(oldNode, nextNode);
              }
            }
          }

          prevNode.next = nextNode;
          oldNode.next = null;
          oldNode.SetParent(null);
        }
      }

      if (args != null)
        AfterEvent(args);

      return oldChild;
    }

    // Adds the specified node to the beginning of the list of children of this node.
    public virtual XmlNode PrependChild(XmlNode newChild)
    {
      return InsertBefore(newChild, FirstChild);
    }

    // Adds the specified node to the end of the list of children of this node.
    public virtual XmlNode AppendChild(XmlNode newChild)
    {
      XmlDocument thisDoc = OwnerDocument;
      if (thisDoc == null)
      {
        thisDoc = this as XmlDocument;
      }
      if (!IsContainer)
        throw new InvalidOperationException(Res.GetString(Res.Xdom_Node_Insert_Contain));

      if (this == newChild || AncestorNode(newChild))
        throw new ArgumentException(Res.GetString(Res.Xdom_Node_Insert_Child));

      if (newChild.ParentNode != null)
        newChild.ParentNode.RemoveChild(newChild);

      XmlDocument childDoc = newChild.OwnerDocument;
      if (childDoc != null && childDoc != thisDoc && childDoc != this)
        throw new ArgumentException(Res.GetString(Res.Xdom_Node_Insert_Context));

      // special case for doc-fragment.
      if (newChild.NodeType == XmlNodeType.DocumentFragment)
      {
        XmlNode first = newChild.FirstChild;
        XmlNode node = first;
        while (node != null)
        {
          XmlNode next = node.NextSibling;
          newChild.RemoveChild(node);
          AppendChild(node);
          node = next;
        }
        return first;
      }

      if (!(newChild is XmlLinkedNode) || !IsValidChildType(newChild.NodeType))
        throw new InvalidOperationException(Res.GetString(Res.Xdom_Node_Insert_TypeConflict));


      if (!CanInsertAfter(newChild, LastChild))
        throw new InvalidOperationException(Res.GetString(Res.Xdom_Node_Insert_Location));

      string newChildValue = newChild.Value;
      XmlNodeChangedEventArgs args = GetEventArgs(newChild, newChild.ParentNode, this, newChildValue, newChildValue, XmlNodeChangedAction.Insert);

      if (args != null)
        BeforeEvent(args);

      XmlLinkedNode refNode = LastNode;
      XmlLinkedNode newNode = (XmlLinkedNode)newChild;

      if (refNode == null)
      {
        newNode.next = newNode;
        LastNode = newNode;
        newNode.SetParent(this);
      }
      else
      {
        newNode.next = refNode.next;
        refNode.next = newNode;
        LastNode = newNode;
        newNode.SetParent(this);

        if (refNode.IsText)
        {
          if (newNode.IsText)
          {
            NestTextNodes(refNode, newNode);
          }
        }
      }

      if (args != null)
        AfterEvent(args);

      return newNode;
    }

    //the function is provided only at Load time to speed up Load process
    internal virtual XmlNode AppendChildForLoad(XmlNode newChild, XmlDocument doc)
    {
      XmlNodeChangedEventArgs args = doc.GetInsertEventArgsForLoad(newChild, this);

      if (args != null)
        doc.BeforeEvent(args);

      XmlLinkedNode refNode = LastNode;
      XmlLinkedNode newNode = (XmlLinkedNode)newChild;

      if (refNode == null)
      {
        newNode.next = newNode;
        LastNode = newNode;
        newNode.SetParentForLoad(this);
      }
      else
      {
        newNode.next = refNode.next;
        refNode.next = newNode;
        LastNode = newNode;
        if (refNode.IsText
            && newNode.IsText)
        {
          NestTextNodes(refNode, newNode);
        }
        else
        {
          newNode.SetParentForLoad(this);
        }
      }

      if (args != null)
        doc.AfterEvent(args);

      return newNode;
    }

    internal virtual bool IsValidChildType(XmlNodeType type)
    {
      return false;
    }

    internal virtual bool CanInsertBefore(XmlNode newChild, XmlNode refChild)
    {
      return true;
    }

    internal virtual bool CanInsertAfter(XmlNode newChild, XmlNode refChild)
    {
      return true;
    }

    // Gets a value indicating whether this node has any child nodes.
    public bool HasChildNodes
    {
      get { return LastNode != null; }
    }

    // Creates a duplicate of this node.
    public abstract XmlNode CloneNode(bool deep);

    internal void CopyChildren(XmlDocument doc, XmlNode container, bool deep)
    {
      for (XmlNode child = container.FirstChild; child != null; child = child.NextSibling)
      {
        AppendChildForLoad(child.CloneNode(deep), doc);
      }
    }

    public virtual string NamespaceURI
    {
      get { return string.Empty; }
    }

    // Gets or sets the namespace prefix of this node.
    public virtual string Prefix
    {
      get { return string.Empty; }
    }

    // Gets the name of the node without the namespace prefix.
    public abstract string LocalName
    {
      get;
    }

    // Microsoft extensions

    // Gets a value indicating whether the node is read-only.
    public virtual bool IsReadOnly
    {
      get
      {
        return HasReadOnlyParent(this);
      }
    }

    internal static bool HasReadOnlyParent(XmlNode n)
    {
      while (n != null)
      {
        switch (n.NodeType)
        {
          case XmlNodeType.EntityReference:
          case XmlNodeType.Entity:
          return true;

          case XmlNodeType.Attribute:
          n = ((XmlAttribute)n).OwnerElement;
          break;

          default:
          n = n.ParentNode;
          break;
        }
      }
      return false;
    }

    object ICloneable.Clone()
    {
      return this.CloneNode(true);
    }

    // Provides a simple ForEach-style iteration over the
    // collection of nodes in this XmlNamedNodeMap.
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new XmlChildEnumerator(this);
    }

    public IEnumerator GetEnumerator()
    {
      return new XmlChildEnumerator(this);
    }

    private void AppendChildText(StringBuilder builder)
    {
      for (XmlNode child = FirstChild; child != null; child = child.NextSibling)
      {
        if (child.FirstChild == null)
        {
          if (child.NodeType == XmlNodeType.Text || child.NodeType == XmlNodeType.CDATA
              || child.NodeType == XmlNodeType.Whitespace || child.NodeType == XmlNodeType.SignificantWhitespace)
            builder.Append(child.InnerText);
        }
        else
        {
          child.AppendChildText(builder);
        }
      }
    }

    // Gets or sets the concatenated values of the node and
    // all its children.
    public virtual string InnerText
    {
      get
      {
        XmlNode fc = FirstChild;
        if (fc == null)
        {
          return string.Empty;
        }
        if (fc.NextSibling == null)
        {
          XmlNodeType nodeType = fc.NodeType;
          switch (nodeType)
          {
            case XmlNodeType.Text:
            case XmlNodeType.CDATA:
            case XmlNodeType.Whitespace:
            case XmlNodeType.SignificantWhitespace:
            return fc.Value;
          }
        }
        StringBuilder builder = new StringBuilder();
        AppendChildText(builder);
        return builder.ToString();
      }

      set
      {
        XmlNode firstChild = FirstChild;
        if (firstChild != null  //there is one child
            && firstChild.NextSibling == null // and exactly one
            && firstChild.NodeType == XmlNodeType.Text)//which is a text node
        {
          //this branch is for perf reason and event fired when TextNode.Value is changed
          firstChild.Value = value;
        }
        else
        {
          RemoveAll();
          AppendChild(OwnerDocument.CreateTextNode(value));
        }
      }
    }

    // Gets the markup representing this node and all its children.
    public virtual string OuterXml
    {
      get
      {
        StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
        XmlDOMTextWriter xw = new XmlDOMTextWriter(sw);
        try
        {
          WriteTo(xw);
        }
        finally
        {
          xw.Close();
        }
        return sw.ToString();
      }
    }

    public virtual IXmlSchemaInfo SchemaInfo
    {
      get
      {
        return XmlDocument.NotKnownSchemaInfo;
      }
    }

    public virtual String BaseURI
    {
      get
      {
        XmlNode curNode = this.ParentNode; //save one while loop since if going to here, the nodetype of this node can't be document, entity and entityref
        while (curNode != null)
        {
          XmlNodeType nt = curNode.NodeType;
          //EntityReference's children come from the dtd where they are defined.
          //we need to investigate the same thing for entity's children if they are defined in an external dtd file.
          if (nt == XmlNodeType.EntityReference)
            return ((XmlEntityReference)curNode).ChildBaseURI;
          if (nt == XmlNodeType.Document
              || nt == XmlNodeType.Entity
              || nt == XmlNodeType.Attribute)
            return curNode.BaseURI;
          curNode = curNode.ParentNode;
        }
        return String.Empty;
      }
    }

    // Saves the current node to the specified XmlWriter.
    public abstract void WriteTo(XmlWriter w);

    public virtual void RemoveAll()
    {
      XmlNode child = FirstChild;
      XmlNode sibling = null;

      while (child != null)
      {
        sibling = child.NextSibling;
        RemoveChild(child);
        child = sibling;
      }
    }

    internal XmlDocument Document
    {
      get
      {
        if (NodeType == XmlNodeType.Document)
          return (XmlDocument)this;
        return OwnerDocument;
      }
    }

    internal virtual void SetParent(XmlNode node)
    {
      if (node == null)
      {
        this.parentNode = OwnerDocument;
      }
      else
      {
        this.parentNode = node;
      }
    }

    internal virtual void SetParentForLoad(XmlNode node)
    {
      this.parentNode = node;
    }

    internal static void SplitName(string name, out string prefix, out string localName)
    {
      int colonPos = name.IndexOf(':'); // ordinal compare
      if (-1 == colonPos || 0 == colonPos || name.Length - 1 == colonPos)
      {
        prefix = string.Empty;
        localName = name;
      }
      else
      {
        prefix = name.Substring(0, colonPos);
        localName = name.Substring(colonPos + 1);
      }
    }

    internal XmlNode FindChild(XmlNodeType type)
    {
      for (XmlNode child = FirstChild; child != null; child = child.NextSibling)
      {
        if (child.NodeType == type)
        {
          return child;
        }
      }
      return null;
    }

    internal virtual XmlNodeChangedEventArgs GetEventArgs(XmlNode node, XmlNode oldParent, XmlNode newParent, string oldValue, string newValue, XmlNodeChangedAction action)
    {
      XmlDocument doc = OwnerDocument;
      if (doc != null)
      {
        if (!doc.IsLoading)
        {
          if (((newParent != null && newParent.IsReadOnly) || (oldParent != null && oldParent.IsReadOnly)))
            throw new InvalidOperationException(Res.GetString(Res.Xdom_Node_Modify_ReadOnly));
        }
        return doc.GetEventArgs(node, oldParent, newParent, oldValue, newValue, action);
      }
      return null;
    }

    internal virtual void BeforeEvent(XmlNodeChangedEventArgs args)
    {
      if (args != null)
        OwnerDocument.BeforeEvent(args);
    }

    internal virtual void AfterEvent(XmlNodeChangedEventArgs args)
    {
      if (args != null)
        OwnerDocument.AfterEvent(args);
    }

    internal virtual XmlSpace XmlSpace
    {
      get
      {
        XmlNode node = this;
        XmlElement elem = null;
        do
        {
          elem = node as XmlElement;
          if (elem != null && elem.HasAttribute("xml:space"))
          {
            switch (XmlConvert.TrimString(elem.GetAttribute("xml:space")))
            {
              case "default":
              return XmlSpace.Default;
              case "preserve":
              return XmlSpace.Preserve;
              default:
              //should we throw exception if value is otherwise?
              break;
            }
          }
          node = node.ParentNode;
        }
        while (node != null);
        return XmlSpace.None;
      }
    }

    internal virtual String XmlLang
    {
      get
      {
        XmlNode node = this;
        XmlElement elem = null;
        do
        {
          elem = node as XmlElement;
          if (elem != null)
          {
            if (elem.HasAttribute("xml:lang"))
              return elem.GetAttribute("xml:lang");
          }
          node = node.ParentNode;
        } while (node != null);
        return String.Empty;
      }
    }

    internal virtual bool IsText
    {
      get
      {
        return false;
      }
    }

    internal static void NestTextNodes(XmlNode prevNode, XmlNode nextNode)
    {
      Debug.Assert(prevNode.IsText);
      Debug.Assert(nextNode.IsText);

      nextNode.parentNode = prevNode;
    }

    internal static void UnnestTextNodes(XmlNode prevNode, XmlNode nextNode)
    {
      Debug.Assert(prevNode.IsText);
      Debug.Assert(nextNode.IsText);

      nextNode.parentNode = prevNode.ParentNode;
    }
    private object debuggerDisplayProxy { get { return new DebuggerDisplayXmlNodeProxy(this); } }
  }
}
