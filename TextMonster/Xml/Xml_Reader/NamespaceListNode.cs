using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Temporary node to represent NamespaceList. Will be expended as a choice of symbols
  /// </summary>
  class NamespaceListNode : SyntaxTreeNode
  {
    protected NamespaceList namespaceList;
    protected object particle;

    public NamespaceListNode(NamespaceList namespaceList, object particle)
    {
      this.namespaceList = namespaceList;
      this.particle = particle;
    }

    public override SyntaxTreeNode Clone(Positions positions)
    {
      // NamespaceListNode nodes have to be removed prior to that
      throw new InvalidOperationException();
    }

    public virtual ICollection GetResolvedSymbols(SymbolsDictionary symbols)
    {
      return symbols.GetNamespaceListSymbols(namespaceList);
    }

    public override void ExpandTree(InteriorNode parent, SymbolsDictionary symbols, Positions positions)
    {
      SyntaxTreeNode replacementNode = null;
      foreach (int symbol in GetResolvedSymbols(symbols))
      {
        if (symbols.GetParticle(symbol) != particle)
        {
          symbols.IsUpaEnforced = false;
        }
        LeafNode node = new LeafNode(positions.Add(symbol, particle));
        if (replacementNode == null)
        {
          replacementNode = node;
        }
        else
        {
          InteriorNode choice = new ChoiceNode();
          choice.LeftChild = replacementNode;
          choice.RightChild = node;
          replacementNode = choice;
        }
      }
      if (parent.LeftChild == this)
      {
        parent.LeftChild = replacementNode;
      }
      else
      {
        parent.RightChild = replacementNode;
      }
    }

    public override void ConstructPos(BitSet firstpos, BitSet lastpos, BitSet[] followpos)
    {
      // NamespaceListNode nodes have to be removed prior to that
      throw new InvalidOperationException();
    }

    public override bool IsNullable
    {
      // NamespaceListNode nodes have to be removed prior to that
      get { throw new InvalidOperationException(); }
    }
  }
}