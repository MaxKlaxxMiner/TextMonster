using System;
using System.Collections;

namespace TextMonster.Xml
{
  internal struct Inserter
  {
    private XContainer parent;
    private XNode previous;
    private string text;

    public Inserter(XContainer parent, XNode anchor)
    {
      this.parent = parent;
      this.previous = anchor;
      this.text = (string)null;
    }

    public void Add(object content)
    {
      this.AddContent(content);
      if (this.text == null)
        return;
      if (this.parent.content == null)
      {
        if (this.parent.SkipNotify())
          this.parent.content = (object)this.text;
        else if (this.text.Length > 0)
          this.InsertNode((XNode)new XText(this.text));
        else if (this.parent is XElement)
        {
          this.parent.NotifyChanging((object)this.parent, XObjectChangeEventArgs.Value);
          if (this.parent.content != null)
            throw new InvalidOperationException("InvalidOperation_ExternalCode");
          this.parent.content = (object)this.text;
          this.parent.NotifyChanged((object)this.parent, XObjectChangeEventArgs.Value);
        }
        else
          this.parent.content = (object)this.text;
      }
      else
      {
        if (this.text.Length <= 0)
          return;
        if (this.previous is XText && !(this.previous is XCData))
        {
          ((XText)this.previous).Value += this.text;
        }
        else
        {
          this.parent.ConvertTextToNode();
          this.InsertNode((XNode)new XText(this.text));
        }
      }
    }

    private void AddContent(object content)
    {
      if (content == null)
        return;
      XNode n = content as XNode;
      if (n != null)
      {
        this.AddNode(n);
      }
      else
      {
        string s = content as string;
        if (s != null)
        {
          this.AddString(s);
        }
        else
        {
          XStreamingElement other = content as XStreamingElement;
          if (other != null)
          {
            this.AddNode((XNode)new XElement(other));
          }
          else
          {
            object[] objArray = content as object[];
            if (objArray != null)
            {
              foreach (object content1 in objArray)
                this.AddContent(content1);
            }
            else
            {
              IEnumerable enumerable = content as IEnumerable;
              if (enumerable != null)
              {
                foreach (object content1 in enumerable)
                  this.AddContent(content1);
              }
              else
              {
                if (content is XAttribute)
                  throw new ArgumentException("Argument_AddAttribute");
                this.AddString(XContainer.GetStringValue(content));
              }
            }
          }
        }
      }
    }

    private void AddNode(XNode n)
    {
      this.parent.ValidateNode(n, this.previous);
      if (n.parent != null)
      {
        n = n.CloneNode();
      }
      else
      {
        XNode xnode = (XNode)this.parent;
        while (xnode.parent != null)
          xnode = (XNode)xnode.parent;
        if (n == xnode)
          n = n.CloneNode();
      }
      this.parent.ConvertTextToNode();
      if (this.text != null)
      {
        if (this.text.Length > 0)
        {
          if (this.previous is XText && !(this.previous is XCData))
            ((XText)this.previous).Value += this.text;
          else
            this.InsertNode((XNode)new XText(this.text));
        }
        this.text = (string)null;
      }
      this.InsertNode(n);
    }

    private void AddString(string s)
    {
      this.parent.ValidateString(s);
      this.text = this.text + s;
    }

    private void InsertNode(XNode n)
    {
      bool flag = this.parent.NotifyChanging((object)n, XObjectChangeEventArgs.Add);
      if (n.parent != null)
        throw new InvalidOperationException("InvalidOperation_ExternalCode");
      n.parent = this.parent;
      if (this.parent.content == null || this.parent.content is string)
      {
        n.next = n;
        this.parent.content = (object)n;
      }
      else if (this.previous == null)
      {
        XNode xnode = (XNode)this.parent.content;
        n.next = xnode.next;
        xnode.next = n;
      }
      else
      {
        n.next = this.previous.next;
        this.previous.next = n;
        if (this.parent.content == this.previous)
          this.parent.content = (object)n;
      }
      this.previous = n;
      if (!flag)
        return;
      this.parent.NotifyChanged((object)n, XObjectChangeEventArgs.Add);
    }
  }
}
