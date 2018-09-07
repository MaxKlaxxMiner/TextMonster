using System;

namespace TextMonster.Xml.Xml_Reader
{
  // Represents an element.
  public class XmlElement : XmlLinkedNode
  {
    XmlName name;
    XmlAttributeCollection attributes;
    XmlLinkedNode lastChild; // == this for empty elements otherwise it is the last child

    internal XmlElement(XmlName name, bool empty, XmlDocument doc)
      : base(doc)
    {
      this.parentNode = null;
      if (!doc.IsLoading)
      {
        XmlDocument.CheckName(name.Prefix);
        XmlDocument.CheckName(name.LocalName);
      }
      if (name.LocalName.Length == 0)
        throw new ArgumentException(Res.GetString(Res.Xdom_Empty_LocalName));
      this.name = name;
      if (empty)
      {
        this.lastChild = this;
      }
    }

    protected internal XmlElement(string prefix, string localName, string namespaceURI, XmlDocument doc)
      : this(doc.AddXmlName(prefix, localName, namespaceURI, null), true, doc)
    {
    }

    internal XmlName XmlName
    {
      get { return name; }
      set { name = value; }
    }

    // Creates a duplicate of this node.
    public override XmlNode CloneNode(bool deep)
    {
      XmlDocument doc = OwnerDocument;
      bool OrigLoadingStatus = doc.IsLoading;
      doc.IsLoading = true;
      XmlElement element = doc.CreateElement(Prefix, LocalName, NamespaceURI);
      doc.IsLoading = OrigLoadingStatus;
      if (element.IsEmpty != this.IsEmpty)
        element.IsEmpty = this.IsEmpty;

      if (HasAttributes)
      {
        foreach (XmlAttribute attr in Attributes)
        {
          XmlAttribute newAttr = (XmlAttribute)(attr.CloneNode(true));
          if (attr is XmlUnspecifiedAttribute && attr.Specified == false)
            ((XmlUnspecifiedAttribute)newAttr).SetSpecified(false);
          element.Attributes.InternalAppendAttribute(newAttr);
        }
      }
      if (deep)
        element.CopyChildren(doc, this, deep);

      return element;
    }

    // Gets the name of the node.
    public override string Name
    {
      get { return name.Name; }
    }

    // Gets the name of the current node without the namespace prefix.
    public override string LocalName
    {
      get { return name.LocalName; }
    }

    // Gets the namespace URI of this node.
    public override string NamespaceURI
    {
      get { return name.NamespaceURI; }
    }

    // Gets or sets the namespace prefix of this node.
    public override string Prefix
    {
      get { return name.Prefix; }
    }

    // Gets the type of the current node.
    public override XmlNodeType NodeType
    {
      get { return XmlNodeType.Element; }
    }

    public override XmlNode ParentNode
    {
      get
      {
        return this.parentNode;
      }
    }

    // Gets the XmlDocument that contains this node.
    public override XmlDocument OwnerDocument
    {
      get
      {
        return name.OwnerDocument;
      }
    }

    internal override bool IsContainer
    {
      get { return true; }
    }

    //the function is provided only at Load time to speed up Load process
    internal override XmlNode AppendChildForLoad(XmlNode newChild, XmlDocument doc)
    {
      XmlNodeChangedEventArgs args = doc.GetInsertEventArgsForLoad(newChild, this);

      if (args != null)
        doc.BeforeEvent(args);

      XmlLinkedNode newNode = (XmlLinkedNode)newChild;

      if (lastChild == null
          || lastChild == this)
      { // if LastNode == null 
        newNode.next = newNode;
        lastChild = newNode; // LastNode = newNode;
        newNode.SetParentForLoad(this);
      }
      else
      {
        XmlLinkedNode refNode = lastChild; // refNode = LastNode;
        newNode.next = refNode.next;
        refNode.next = newNode;
        lastChild = newNode; // LastNode = newNode;
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

    // Gets or sets whether the element does not have any children.
    public bool IsEmpty
    {
      get
      {
        return lastChild == this;
      }

      set
      {
        if (value)
        {
          if (lastChild != this)
          {
            RemoveAllChildren();
            lastChild = this;
          }
        }
        else
        {
          if (lastChild == this)
          {
            lastChild = null;
          }
        }

      }
    }

    internal override XmlLinkedNode LastNode
    {
      get
      {
        return lastChild == this ? null : lastChild;
      }

      set
      {
        lastChild = value;
      }
    }

    internal override bool IsValidChildType(XmlNodeType type)
    {
      switch (type)
      {
        case XmlNodeType.Element:
        case XmlNodeType.Text:
        case XmlNodeType.EntityReference:
        case XmlNodeType.Comment:
        case XmlNodeType.Whitespace:
        case XmlNodeType.SignificantWhitespace:
        case XmlNodeType.ProcessingInstruction:
        case XmlNodeType.CDATA:
        return true;

        default:
        return false;
      }
    }


    // Gets a XmlAttributeCollection containing the list of attributes for this node.
    public override XmlAttributeCollection Attributes
    {
      get
      {
        if (attributes == null)
        {
          lock (OwnerDocument.objLock)
          {
            if (attributes == null)
            {
              attributes = new XmlAttributeCollection(this);
            }
          }
        }

        return attributes;
      }
    }

    // Gets a value indicating whether the current node
    // has any attributes.
    public virtual bool HasAttributes
    {
      get
      {
        if (this.attributes == null)
          return false;
        else
          return this.attributes.Count > 0;
      }
    }

    // Returns the value for the attribute with the specified name.
    public virtual string GetAttribute(string name)
    {
      XmlAttribute attr = GetAttributeNode(name);
      if (attr != null)
        return attr.Value;
      return String.Empty;
    }

    // Returns the XmlAttribute with the specified name.
    public virtual XmlAttribute GetAttributeNode(string name)
    {
      if (HasAttributes)
        return Attributes[name];
      return null;
    }

    // Adds the specified XmlAttribute.
    public virtual XmlAttribute SetAttributeNode(XmlAttribute newAttr)
    {
      if (newAttr.OwnerElement != null)
        throw new InvalidOperationException(Res.GetString(Res.Xdom_Attr_InUse));
      return (XmlAttribute)Attributes.SetNamedItem(newAttr);
    }

    public virtual bool HasAttribute(string name)
    {
      return GetAttributeNode(name) != null;
    }

    // Saves the current node to the specified XmlWriter.
    public override void WriteTo(XmlWriter w)
    {

      if (GetType() == typeof(XmlElement))
      {
        // Use the non-recursive version (for XmlElement only)
        WriteElementTo(w, this);
      }
      else
      {
        // Use the (potentially) recursive version
        WriteStartElement(w);

        if (IsEmpty)
        {
          w.WriteEndElement();
        }
        else
        {
          WriteContentTo(w);
          w.WriteFullEndElement();
        }
      }
    }

    // This method is copied from Linq.ElementWriter.WriteElement but adapted to DOM
    private static void WriteElementTo(XmlWriter writer, XmlElement e)
    {
      XmlNode root = e;
      XmlNode n = e;
      while (true)
      {
        e = n as XmlElement;
        // Only use the inlined write logic for XmlElement, not for derived classes
        if (e != null && e.GetType() == typeof(XmlElement))
        {
          // Write the element
          e.WriteStartElement(writer);
          // Write the element's content
          if (e.IsEmpty)
          {
            // No content; use a short end element <a />
            writer.WriteEndElement();
          }
          else if (e.lastChild == null)
          {
            // No actual content; use a full end element <a></a>
            writer.WriteFullEndElement();
          }
          else
          {
            // There are child node(s); move to first child
            n = e.FirstChild;
            continue;
          }
        }
        else
        {
          // Use virtual dispatch (might recurse)
          n.WriteTo(writer);
        }
        // Go back to the parent after writing the last child
        while (n != root && n == n.ParentNode.LastChild)
        {
          n = n.ParentNode;
          writer.WriteFullEndElement();
        }
        if (n == root)
          break;
        n = n.NextSibling;
      }
    }

    // Writes the start of the element (and its attributes) to the specified writer
    private void WriteStartElement(XmlWriter w)
    {
      w.WriteStartElement(Prefix, LocalName, NamespaceURI);

      if (HasAttributes)
      {
        XmlAttributeCollection attrs = Attributes;
        for (int i = 0; i < attrs.Count; i += 1)
        {
          XmlAttribute attr = attrs[i];
          attr.WriteTo(w);
        }
      }
    }

    // Saves all the children of the node to the specified XmlWriter.
    public virtual void WriteContentTo(XmlWriter w)
    {
      for (XmlNode node = FirstChild; node != null; node = node.NextSibling)
      {
        node.WriteTo(w);
      }
    }

    // Removes all attributes from the element.
    public virtual void RemoveAllAttributes()
    {
      if (HasAttributes)
      {
        attributes.RemoveAll();
      }
    }

    // Removes all the children and/or attributes
    // of the current node.
    public override void RemoveAll()
    {
      //remove all the children
      base.RemoveAll();
      //remove all the attributes
      RemoveAllAttributes();
    }

    internal void RemoveAllChildren()
    {
      base.RemoveAll();
    }

    public override string InnerText
    {
      get
      {
        return base.InnerText;
      }
      set
      {
        XmlLinkedNode linkedNode = LastNode;
        if (linkedNode != null && //there is one child
            linkedNode.NodeType == XmlNodeType.Text && //which is text node
            linkedNode.next == linkedNode) // and it is the only child 
        {
          //this branch is for perf reason, event fired when TextNode.Value is changed.
          linkedNode.Value = value;
        }
        else
        {
          RemoveAllChildren();
          AppendChild(OwnerDocument.CreateTextNode(value));
        }
      }
    }

    public override XmlNode NextSibling
    {
      get
      {
        if (this.parentNode != null
            && this.parentNode.LastNode != this)
          return next;
        return null;
      }
    }

    internal override void SetParent(XmlNode node)
    {
      this.parentNode = node;
    }
  }
}
