using System.Xml;

namespace TextMonster.Xml
{
  internal struct ElementWriter
  {
    readonly XmlWriter writer;

    public ElementWriter(XmlWriter writer)
    {
      this.writer = writer;
    }

    public void WriteElement(X_Element e)
    {
      PushAncestors(e);
      var xelement = e;
      var xnode = (X_Node)e;
      while (true)
      {
        e = xnode as X_Element;
        if (e != null)
        {
          WriteStartElement(e);
          if (e.content == null)
          {
            WriteEndElement();
          }
          else
          {
            string text = e.content as string;
            if (text != null)
            {
              writer.WriteString(text);
              WriteFullEndElement();
            }
            else
            {
              xnode = ((X_Node)e.content).next;
              continue;
            }
          }
        }
        else
          xnode.WriteTo(writer);
        while (xnode != xelement && xnode == xnode.parent.content)
        {
          xnode = xnode.parent;
          WriteFullEndElement();
        }
        if (xnode != xelement)
          xnode = xnode.next;
        else
          break;
      }
    }

    static void PushAncestors(X_Element e)
    {
    label_0:
      X_Attribute xattribute;
      do
      {
        e = e.parent as X_Element;
        if (e != null)
          xattribute = e.lastAttr;
        else
          goto label_5;
      }
      while (xattribute == null);
      goto label_2;
    label_5:
      return;
    label_2:
      do
      {
        xattribute = xattribute.next;
      }
      while (xattribute != e.lastAttr);
      goto label_0;
    }

    static void PushElement(X_Element e)
    {
      var xattribute = e.lastAttr;
      if (xattribute == null)
        return;
      do
      {
        xattribute = xattribute.next;
      }
      while (xattribute != e.lastAttr);
    }

    void WriteEndElement()
    {
      writer.WriteEndElement();
    }

    void WriteFullEndElement()
    {
      writer.WriteFullEndElement();
    }

    void WriteStartElement(X_Element e)
    {
      PushElement(e);
      writer.WriteStartElement(string.Empty, e.Name, string.Empty);
      var xattribute = e.lastAttr;
      if (xattribute == null)
        return;
      do
      {
        xattribute = xattribute.next;
        string localName = xattribute.Name;
        writer.WriteAttributeString(string.Empty, localName, string.Empty, xattribute.Value);
      }
      while (xattribute != e.lastAttr);
    }
  }
}
