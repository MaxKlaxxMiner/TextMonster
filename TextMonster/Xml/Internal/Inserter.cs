using System;
using System.Collections;
// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull

namespace TextMonster.Xml
{
  internal struct Inserter
  {
    readonly XContainer parent;
    XNode previous;
    string text;

    public Inserter(XContainer parent, XNode anchor)
    {
      this.parent = parent;
      previous = anchor;
      text = null;
    }

    public void Add(object content)
    {
      AddContent(content);
      if (text == null)
        return;
      if (parent.content == null)
      {
        if (parent.SkipNotify())
          parent.content = text;
        else if (text.Length > 0)
          InsertNode(new XText(text));
        else if (parent is XElement)
        {
          parent.NotifyChanging(parent, XObjectChangeEventArgs.Value);
          if (parent.content != null)
            throw new InvalidOperationException("InvalidOperation_ExternalCode");
          parent.content = text;
          parent.NotifyChanged(parent, XObjectChangeEventArgs.Value);
        }
        else
          parent.content = text;
      }
      else
      {
        if (text.Length <= 0)
          return;
        if (previous is XText && !(previous is XcData))
        {
          ((XText)previous).Value += text;
        }
        else
        {
          parent.ConvertTextToNode();
          InsertNode(new XText(text));
        }
      }
    }

    void AddContent(object content)
    {
      if (content == null)
        return;
      var n = content as XNode;
      if (n != null)
      {
        AddNode(n);
      }
      else
      {
        string s = content as string;
        if (s != null)
        {
          AddString(s);
        }
        else
        {
          var other = content as XStreamingElement;
          if (other != null)
          {
            AddNode(new XElement(other));
          }
          else
          {
            var objArray = content as object[];
            if (objArray != null)
            {
              foreach (var content1 in objArray)
                AddContent(content1);
            }
            else
            {
              var enumerable = content as IEnumerable;
              if (enumerable != null)
              {
                foreach (var content1 in enumerable)
                  AddContent(content1);
              }
              else
              {
                if (content is XAttribute)
                  throw new ArgumentException("Argument_AddAttribute");
                AddString(XContainer.GetStringValue(content));
              }
            }
          }
        }
      }
    }

    void AddNode(XNode n)
    {
      parent.ValidateNode(n, previous);
      if (n.parent != null)
      {
        n = n.CloneNode();
      }
      else
      {
        var xnode = (XNode)parent;
        while (xnode.parent != null)
          xnode = xnode.parent;
        if (n == xnode)
          n = n.CloneNode();
      }
      parent.ConvertTextToNode();
      if (text != null)
      {
        if (text.Length > 0)
        {
          if (previous is XText && !(previous is XcData))
            ((XText)previous).Value += text;
          else
            InsertNode(new XText(text));
        }
        text = null;
      }
      InsertNode(n);
    }

    void AddString(string s)
    {
      parent.ValidateString(s);
      text = text + s;
    }

    void InsertNode(XNode n)
    {
      bool flag = parent.NotifyChanging(n, XObjectChangeEventArgs.Add);
      if (n.parent != null)
        throw new InvalidOperationException("InvalidOperation_ExternalCode");
      n.parent = parent;
      if (parent.content == null || parent.content is string)
      {
        n.next = n;
        parent.content = n;
      }
      else if (previous == null)
      {
        var xnode = (XNode)parent.content;
        n.next = xnode.next;
        xnode.next = n;
      }
      else
      {
        n.next = previous.next;
        previous.next = n;
        if (parent.content == previous)
          parent.content = n;
      }
      previous = n;
      if (!flag)
        return;
      parent.NotifyChanged(n, XObjectChangeEventArgs.Add);
    }
  }
}
