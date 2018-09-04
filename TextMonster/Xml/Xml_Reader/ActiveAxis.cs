using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  // whenever an element is under identity-constraint, an instance of this class will be called
  // only care about property at this time
  internal class ActiveAxis
  {
    // consider about reactivating....  the stack should be clear right??
    // just reset contextDepth & isActive....
    private int currentDepth;                       // current depth, trace the depth by myself... movetochild, movetoparent, movetoattribute
    private bool isActive;                          // not active any more after moving out context node
    private Asttree axisTree;                       // reference to the whole tree
    // for each subtree i need to keep a stack...
    private ArrayList axisStack;                    // of AxisStack

    public int CurrentDepth
    {
      get { return this.currentDepth; }
    }

    // if an instance is !IsActive, then it can be reactive and reuse
    // still need thinking.....
    internal void Reactivate()
    {
      this.isActive = true;
      this.currentDepth = -1;
    }

    internal ActiveAxis(Asttree axisTree)
    {
      this.axisTree = axisTree;                                               // only a pointer.  do i need it?
      this.currentDepth = -1;                                                 // context depth is 0 -- enforce moveToChild for the context node
      // otherwise can't deal with "." node
      this.axisStack = new ArrayList(axisTree.SubtreeArray.Count);            // defined length
      // new one stack element for each one
      for (int i = 0; i < axisTree.SubtreeArray.Count; ++i)
      {
        AxisStack stack = new AxisStack((ForwardAxis)axisTree.SubtreeArray[i], this);
        axisStack.Add(stack);
      }
      this.isActive = true;
    }

    public bool MoveToStartElement(string localname, string URN)
    {
      if (!isActive)
      {
        return false;
      }

      // for each:
      this.currentDepth++;
      bool result = false;
      for (int i = 0; i < this.axisStack.Count; ++i)
      {
        AxisStack stack = (AxisStack)this.axisStack[i];
        // special case for self tree   "." | ".//."
        if (stack.Subtree.IsSelfAxis)
        {
          if (stack.Subtree.IsDss || (this.CurrentDepth == 0))
            result = true;
          continue;
        }

        // otherwise if it's context node then return false
        if (this.CurrentDepth == 0) continue;

        if (stack.MoveToChild(localname, URN, this.currentDepth))
        {
          result = true;
          // even already know the last result is true, still need to continue...
          // run everyone once
        }
      }
      return result;
    }

    // return result doesn't have any meaning until in SelectorActiveAxis
    public virtual bool EndElement(string localname, string URN)
    {
      // need to think if the early quitting will affect reactivating....
      if (this.currentDepth == 0)
      {          // leave context node
        this.isActive = false;
        this.currentDepth--;
      }
      if (!this.isActive)
      {
        return false;
      }
      for (int i = 0; i < this.axisStack.Count; ++i)
      {
        ((AxisStack)axisStack[i]).MoveToParent(localname, URN, this.currentDepth);
      }
      this.currentDepth--;
      return false;
    }

    // Secondly field interface 
    public bool MoveToAttribute(string localname, string URN)
    {
      if (!this.isActive)
      {
        return false;
      }
      bool result = false;
      for (int i = 0; i < this.axisStack.Count; ++i)
      {
        if (((AxisStack)axisStack[i]).MoveToAttribute(localname, URN, this.currentDepth + 1))
        {  // don't change depth for attribute, but depth is add 1 
          result = true;
        }
      }
      return result;
    }
  }
}
