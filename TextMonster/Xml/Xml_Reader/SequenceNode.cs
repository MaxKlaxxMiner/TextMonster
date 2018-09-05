using System.Collections.Generic;

namespace TextMonster.Xml.Xml_Reader
{
  sealed class SequenceNode : InteriorNode
  {

    struct SequenceConstructPosContext
    {
      public SequenceNode this_;
      public BitSet firstpos;
      public BitSet lastpos;
      public BitSet lastposLeft;
      public BitSet firstposRight;

      public SequenceConstructPosContext(SequenceNode node, BitSet firstpos, BitSet lastpos)
      {
        this_ = node;
        this.firstpos = firstpos;
        this.lastpos = lastpos;

        lastposLeft = null;
        firstposRight = null;
      }
    }

    public override void ConstructPos(BitSet firstpos, BitSet lastpos, BitSet[] followpos)
    {

      Stack<SequenceConstructPosContext> contextStack = new Stack<SequenceConstructPosContext>();
      SequenceConstructPosContext context = new SequenceConstructPosContext(this, firstpos, lastpos);

      while (true)
      {
        SequenceNode this_ = context.this_;
        context.lastposLeft = new BitSet(lastpos.Count);
        if (this_.LeftChild is SequenceNode)
        {
          contextStack.Push(context);
          context = new SequenceConstructPosContext((SequenceNode)this_.LeftChild, context.firstpos, context.lastposLeft);
          continue;
        }
        this_.LeftChild.ConstructPos(context.firstpos, context.lastposLeft, followpos);

        ProcessRight:
        context.firstposRight = new BitSet(firstpos.Count);
        this_.RightChild.ConstructPos(context.firstposRight, context.lastpos, followpos);

        if (this_.LeftChild.IsNullable && !this_.RightChild.IsRangeNode)
        {
          context.firstpos.Or(context.firstposRight);
        }
        if (this_.RightChild.IsNullable)
        {
          context.lastpos.Or(context.lastposLeft);
        }
        for (int pos = context.lastposLeft.NextSet(-1); pos != -1; pos = context.lastposLeft.NextSet(pos))
        {
          followpos[pos].Or(context.firstposRight);
        }
        if (this_.RightChild.IsRangeNode)
        { //firstpos is leftchild.firstpos as the or with firstposRight has not been done as it is a rangenode
          ((LeafRangeNode)this_.RightChild).NextIteration = context.firstpos.Clone();
        }

        if (contextStack.Count == 0)
          break;

        context = contextStack.Pop();
        this_ = context.this_;
        goto ProcessRight;
      }
    }

    public override bool IsNullable
    {
      get
      {
        SyntaxTreeNode n;
        SequenceNode this_ = this;
        do
        {
          if (this_.RightChild.IsRangeNode && ((LeafRangeNode)this_.RightChild).Min == 0)
            return true;
          if (!this_.RightChild.IsNullable && !this_.RightChild.IsRangeNode)
            return false;
          n = this_.LeftChild;
          this_ = n as SequenceNode;
        }
        while (this_ != null);
        return n.IsNullable;
      }
    }

    public override void ExpandTree(InteriorNode parent, SymbolsDictionary symbols, Positions positions)
    {
      ExpandTreeNoRecursive(parent, symbols, positions);
    }
  }
}