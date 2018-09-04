using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class AxisStack
  {
    // property
    private ArrayList stack;                            // of AxisElement
    private ForwardAxis subtree;                        // reference to the corresponding subtree
    private ActiveAxis parent;

    internal ForwardAxis Subtree
    {
      get { return this.subtree; }
    }

    internal int Length
    {                               // stack length
      get { return this.stack.Count; }
    }

    // instructor
    public AxisStack(ForwardAxis faxis, ActiveAxis parent)
    {
      this.subtree = faxis;
      this.stack = new ArrayList();
      this.parent = parent;       // need to use its contextdepth each time....

      // improvement:
      // if ! isDss, there has nothing to do with Push/Pop, only one copy each time will be kept
      // if isDss, push and pop each time....
      if (!faxis.IsDss)
      {                // keep an instance
        this.Push(1);              // context depth + 1
      }
      // else just keep stack empty
    }

    // method
    internal void Push(int depth)
    {
      AxisElement eaxis = new AxisElement(this.subtree.RootNode, depth);
      this.stack.Add(eaxis);
    }

    internal void Pop()
    {
      this.stack.RemoveAt(Length - 1);
    }

    // used in the beginning of .//  and MoveToChild
    // didn't consider Self, only consider name
    internal static bool Equal(string thisname, string thisURN, string name, string URN)
    {
      // which means "b" in xpath, no namespace should be specified
      if (thisURN == null)
      {
        if (!((URN == null) || (URN.Length == 0)))
        {
          return false;
        }
      }
      // != "*"
      else if ((thisURN.Length != 0) && (thisURN != URN))
      {
        return false;
      }
      // != "a:*" || "*"
      if ((thisname.Length != 0) && (thisname != name))
      {
        return false;
      }
      return true;
    }

    // "a/b/c"     pointer from b move to a
    // needn't change even tree structure changes
    internal void MoveToParent(string name, string URN, int depth)
    {
      if (this.subtree.IsSelfAxis)
      {
        return;
      }

      for (int i = 0; i < this.stack.Count; ++i)
      {
        ((AxisElement)stack[i]).MoveToParent(depth, this.subtree);
      }

      // in ".//"'s case, since each time you push one new element while match, why not pop one too while match?
      if (this.subtree.IsDss && Equal(this.subtree.RootNode.Name, this.subtree.RootNode.Urn, name, URN))
      {
        Pop();
      }   // only the last one
    }

    // "a/b/c"     pointer from a move to b
    // return true if reach c
    internal bool MoveToChild(string name, string URN, int depth)
    {
      bool result = false;
      // push first
      if (this.subtree.IsDss && Equal(this.subtree.RootNode.Name, this.subtree.RootNode.Urn, name, URN))
      {
        Push(-1);
      }
      for (int i = 0; i < this.stack.Count; ++i)
      {
        if (((AxisElement)stack[i]).MoveToChild(name, URN, depth, this.subtree))
        {
          result = true;
        }
      }
      return result;
    }

    // attribute can only at the topaxis part
    // dealing with attribute only here, didn't go into stack element at all
    // stack element only deal with moving the pointer around elements
    internal bool MoveToAttribute(string name, string URN, int depth)
    {
      if (!this.subtree.IsAttribute)
      {
        return false;
      }
      if (!Equal(this.subtree.TopNode.Name, this.subtree.TopNode.Urn, name, URN))
      {
        return false;
      }

      bool result = false;

      // no stack element for single attribute, so dealing with it seperately
      if (this.subtree.TopNode.Input == null)
      {
        return (this.subtree.IsDss || (depth == 1));
      }

      for (int i = 0; i < this.stack.Count; ++i)
      {
        AxisElement eaxis = (AxisElement)this.stack[i];
        if ((eaxis.isMatch) && (eaxis.CurNode == this.subtree.TopNode.Input))
        {
          result = true;
        }
      }
      return result;
    }

  }
}
