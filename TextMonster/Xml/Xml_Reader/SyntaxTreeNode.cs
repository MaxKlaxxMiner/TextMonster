namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Base class for the systax tree nodes
  /// </summary>
  abstract class SyntaxTreeNode
  {
    /// <summary>
    /// Expand NamesapceListNode and RangeNode nodes. All other nodes
    /// </summary>
    public abstract void ExpandTree(InteriorNode parent, SymbolsDictionary symbols, Positions positions);

    /// <summary>
    /// Clone the syntaxTree. We need to pass symbolsByPosition because leaf nodes have to add themselves to it.
    /// </summary>
    public abstract SyntaxTreeNode Clone(Positions positions);

    /// <summary>
    /// From a regular expression to a DFA
    /// Compilers by Aho, Sethi, Ullman.
    /// ISBN 0-201-10088-6, p135 
    /// Construct firstpos, lastpos and calculate followpos 
    /// </summary>
    public abstract void ConstructPos(BitSet firstpos, BitSet lastpos, BitSet[] followpos);

    /// <summary>
    /// Returns nullable property that is being used by ConstructPos
    /// </summary>
    public abstract bool IsNullable { get; }

    /// <summary>
    /// Returns true if node is a range node
    /// </summary>
    public virtual bool IsRangeNode
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Print syntax tree
    /// </summary>
#if DEBUG        
        public abstract void Dump(StringBuilder bb, SymbolsDictionary symbols, Positions positions);
#endif
  }
}