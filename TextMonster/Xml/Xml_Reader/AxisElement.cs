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
      get { return curNode; }
    }

    // constructor
    internal AxisElement(DoubleLinkAxis node, int depth)
    {
      curNode = node;
      rootDepth = curDepth = depth;
      isMatch = false;
    }

    internal void SetDepth(int depth)
    {
      rootDepth = curDepth = depth;
    }

    // "a/b/c"     pointer from b move to a
    // needn't change even tree structure changes
    internal void MoveToParent(int depth, ForwardAxis parent)
    {
      // "a/b/c", trying to match b (current node), but meet the end of a, so move pointer to a
      if (depth == curDepth - 1)
      {
        // really need to move the current node pointer to parent
        // what i did here is for seperating the case of IsDss or only IsChild
        // bcoz in the first case i need to expect "a" from random depth
        // -1 means it doesn't expect some specific depth (referecing the dealing to -1 in movetochild method
        // while in the second case i can't change the root depth which is 1.
        if ((curNode.Input == parent.RootNode) && (parent.IsDss))
        {
          curNode = parent.RootNode;
          rootDepth = curDepth = -1;
          return;
        }
        if (curNode.Input != null)
        {      // else cur-depth --, cur-node change
          curNode = (DoubleLinkAxis)(curNode.Input);
          curDepth--;
          return;
        }
        return;
      }
        // "a/b/c", trying to match b (current node), but meet the end of x (another child of a)
      // or maybe i matched, now move out the current node
      // or move out after failing to match attribute
      // the node i m next expecting is still the current node
      if (depth == curDepth)
      {              // after matched or [2] failed in matching attribute
        if (isMatch)
        {
          isMatch = false;
        }
      }
      // ignore
    }

    // equal & ! attribute then move
    // "a/b/c"     pointer from a move to b
    // return true if reach c and c is an element and c is the axis
    internal bool MoveToChild(string name, string URN, int depth, ForwardAxis parent)
    {
      // an attribute can never be the same as an element
      if (Asttree.IsAttribute(curNode))
      {
        return false;
      }

      // either moveToParent or moveToChild status will have to be changed into unmatch...
      if (isMatch)
      {
        isMatch = false;
      }
      if (!AxisStack.Equal(curNode.Name, curNode.Urn, name, URN))
      {
        return false;
      }
      if (curDepth == -1)
      {
        SetDepth(depth);
      }
      else if (depth > curDepth)
      {
        return false;
      }
      // matched ...  
      if (curNode == parent.TopNode)
      {
        isMatch = true;
        return true;
      }
      // move down this.curNode
      DoubleLinkAxis nowNode = (DoubleLinkAxis)(curNode.Next);
      if (Asttree.IsAttribute(nowNode))
      {
        isMatch = true;                    // for attribute 
        return false;
      }
      curNode = nowNode;
      curDepth++;
      return false;
    }

  }
}
