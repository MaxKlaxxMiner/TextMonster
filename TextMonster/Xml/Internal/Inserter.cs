using System;
using System.Collections;
// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull

namespace TextMonster.Xml
{
  internal struct Inserter
  {
    readonly X_Container parent;
    X_Node previous;
    string text;

    public Inserter(X_Container parent, X_Node anchor)
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
          InsertNode(new X_Text(text));
        else if (parent is X_Element)
        {
          parent.NotifyChanging(parent, X_ObjectChangeEventArgs.Value);
          if (parent.content != null)
            throw new InvalidOperationException("InvalidOperation_ExternalCode");
          parent.content = text;
          parent.NotifyChanged(parent, X_ObjectChangeEventArgs.Value);
        }
        else
          parent.content = text;
      }
      else
      {
        if (text.Length <= 0)
          return;
        if (previous is X_Text && !(previous is X_CData))
        {
          ((X_Text)previous).Value += text;
        }
        else
        {
          parent.ConvertTextToNode();
          InsertNode(new X_Text(text));
        }
      }
    }

    void AddContent(object content)
    {
      if (content == null)
        return;
      var n = content as X_Node;
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
          var other = content as X_StreamingElement;
          if (other != null)
          {
            AddNode(new X_Element(other));
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
                if (content is X_Attribute)
                  throw new ArgumentException("Argument_AddAttribute");
                AddString(X_Container.GetStringValue(content));
              }
            }
          }
        }
      }
    }

    void AddNode(X_Node n)
    {
      parent.ValidateNode(n, previous);
      if (n.parent != null)
      {
        n = n.CloneNode();
      }
      else
      {
        var xnode = (X_Node)parent;
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
          if (previous is X_Text && !(previous is X_CData))
            ((X_Text)previous).Value += text;
          else
            InsertNode(new X_Text(text));
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

    void InsertNode(X_Node n)
    {
      bool flag = parent.NotifyChanging(n, X_ObjectChangeEventArgs.Add);
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
        var xnode = (X_Node)parent.content;
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
      parent.NotifyChanged(n, X_ObjectChangeEventArgs.Add);
    }
  }
}
