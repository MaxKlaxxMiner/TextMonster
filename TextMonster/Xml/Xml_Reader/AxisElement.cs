namespace TextMonster.Xml.Xml_Reader
{
  // stack element class
  // this one needn't change, even the parameter in methods
  internal class AxisElement
  {
    internal DoubleLinkAxis curNode;                // current under-checking node during navigating
    internal int rootDepth;                         // root depth -- contextDepth + 1 if ! isDss; context + {1...} if isDss
    internal int curDepth;                          // current depth
    internal bool isMatch;                          // current is already matching or waiting for matching

    internal DoubleLinkAxis CurNode
    {
      get { return this.curNode; }
    }

    // constructor
    internal AxisElement(DoubleLinkAxis node, int depth)
    {
      this.curNode = node;
      this.rootDepth = this.curDepth = depth;
      this.isMatch = false;
    }

    internal void SetDepth(int depth)
    {
      this.rootDepth = this.curDepth = depth;
      return;
    }

    // "a/b/c"     pointer from b move to a
    // needn't change even tree structure changes
    internal void MoveToParent(int depth, ForwardAxis parent)
    {
      // "a/b/c", trying to match b (current node), but meet the end of a, so move pointer to a
      if (depth == this.curDepth - 1)
      {
        // really need to move the current node pointer to parent
        // what i did here is for seperating the case of IsDss or only IsChild
        // bcoz in the first case i need to expect "a" from random depth
        // -1 means it doesn't expect some specific depth (referecing the dealing to -1 in movetochild method
        // while in the second case i can't change the root depth which is 1.
        if ((this.curNode.Input == parent.RootNode) && (parent.IsDss))
        {
          this.curNode = parent.RootNode;
          this.rootDepth = this.curDepth = -1;
          return;
        }
        else if (this.curNode.Input != null)
        {      // else cur-depth --, cur-node change
          this.curNode = (DoubleLinkAxis)(this.curNode.Input);
          this.curDepth--;
          return;
        }
        else return;
      }
      // "a/b/c", trying to match b (current node), but meet the end of x (another child of a)
      // or maybe i matched, now move out the current node
      // or move out after failing to match attribute
      // the node i m next expecting is still the current node
      else if (depth == this.curDepth)
      {              // after matched or [2] failed in matching attribute
        if (this.isMatch)
        {
          this.isMatch = false;
        }
      }
      return;                                         // this node is still what i am expecting
      // ignore
    }

    // equal & ! attribute then move
    // "a/b/c"     pointer from a move to b
    // return true if reach c and c is an element and c is the axis
    internal bool MoveToChild(string name, string URN, int depth, ForwardAxis parent)
    {
      // an attribute can never be the same as an element
      if (Asttree.IsAttribute(this.curNode))
      {
        return false;
      }

      // either moveToParent or moveToChild status will have to be changed into unmatch...
      if (this.isMatch)
      {
        this.isMatch = false;
      }
      if (!AxisStack.Equal(this.curNode.Name, this.curNode.Urn, name, URN))
      {
        return false;
      }
      if (this.curDepth == -1)
      {
        SetDepth(depth);
      }
      else if (depth > this.curDepth)
      {
        return false;
      }
      // matched ...  
      if (this.curNode == parent.TopNode)
      {
        this.isMatch = true;
        return true;
      }
      // move down this.curNode
      DoubleLinkAxis nowNode = (DoubleLinkAxis)(this.curNode.Next);
      if (Asttree.IsAttribute(nowNode))
      {
        this.isMatch = true;                    // for attribute 
        return false;
      }
      this.curNode = nowNode;
      this.curDepth++;
      return false;
    }

  }
}
