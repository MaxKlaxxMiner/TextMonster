using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  sealed class ParticleContentValidator : ContentValidator
  {
    SymbolsDictionary symbols;
    Positions positions;
    Stack stack;                        // parsing context
    SyntaxTreeNode contentNode;         // content model points to syntax tree
    bool isPartial;                     // whether the closure applies to partial or the whole node that is on top of the stack
    int minMaxNodesCount;
    bool enableUpaCheck;

    public ParticleContentValidator(XmlSchemaContentType contentType)
      : this(contentType, true)
    {
    }

    public ParticleContentValidator(XmlSchemaContentType contentType, bool enableUpaCheck)
      : base(contentType)
    {
      this.enableUpaCheck = enableUpaCheck;
    }

    public override void InitValidation(ValidationState context)
    {
      // ParticleContentValidator cannot be used during validation
      throw new InvalidOperationException();
    }

    public override object ValidateElement(XmlQualifiedName name, ValidationState context, out int errorCode)
    {
      // ParticleContentValidator cannot be used during validation
      throw new InvalidOperationException();
    }

    public override bool CompleteValidation(ValidationState context)
    {
      // ParticleContentValidator cannot be used during validation
      throw new InvalidOperationException();
    }

    public void Start()
    {
      symbols = new SymbolsDictionary();
      positions = new Positions();
      stack = new Stack();
    }

    public void OpenGroup()
    {
      stack.Push(null);
    }

    public void CloseGroup()
    {
      SyntaxTreeNode node = (SyntaxTreeNode)stack.Pop();
      if (node == null)
      {
        return;
      }
      if (stack.Count == 0)
      {
        contentNode = node;
        isPartial = false;
      }
      else
      {
        // some collapsing to do...
        InteriorNode inNode = (InteriorNode)stack.Pop();
        if (inNode != null)
        {
          inNode.RightChild = node;
          node = inNode;
          isPartial = true;
        }
        else
        {
          isPartial = false;
        }
        stack.Push(node);
      }
    }

    public bool Exists(XmlQualifiedName name)
    {
      if (symbols.Exists(name))
      {
        return true;
      }
      return false;
    }

    public void AddName(XmlQualifiedName name, object particle)
    {
      AddLeafNode(new LeafNode(positions.Add(symbols.AddName(name, particle), particle)));
    }

    public void AddNamespaceList(NamespaceList namespaceList, object particle)
    {
      symbols.AddNamespaceList(namespaceList, particle, false);
      AddLeafNode(new NamespaceListNode(namespaceList, particle));
    }

    private void AddLeafNode(SyntaxTreeNode node)
    {
      if (stack.Count > 0)
      {
        InteriorNode inNode = (InteriorNode)stack.Pop();
        if (inNode != null)
        {
          inNode.RightChild = node;
          node = inNode;
        }
      }
      stack.Push(node);
      isPartial = true;
    }

    public void AddChoice()
    {
      SyntaxTreeNode node = (SyntaxTreeNode)stack.Pop();
      InteriorNode choice = new ChoiceNode();
      choice.LeftChild = node;
      stack.Push(choice);
    }

    public void AddSequence()
    {
      SyntaxTreeNode node = (SyntaxTreeNode)stack.Pop();
      InteriorNode sequence = new SequenceNode();
      sequence.LeftChild = node;
      stack.Push(sequence);
    }

    public void AddStar()
    {
      Closure(new StarNode());
    }

    public void AddPlus()
    {
      Closure(new PlusNode());
    }

    public void AddQMark()
    {
      Closure(new QmarkNode());
    }

    public void AddLeafRange(decimal min, decimal max)
    {
      LeafRangeNode rNode = new LeafRangeNode(min, max);
      int pos = positions.Add(-2, rNode);
      rNode.Pos = pos;

      InteriorNode sequence = new SequenceNode();
      sequence.RightChild = rNode;
      Closure(sequence);
      minMaxNodesCount++;
    }

    private void Closure(InteriorNode node)
    {
      if (stack.Count > 0)
      {
        SyntaxTreeNode topNode = (SyntaxTreeNode)stack.Pop();
        InteriorNode inNode = topNode as InteriorNode;
        if (isPartial && inNode != null)
        {
          // need to reach in and wrap right hand side of element.
          // and n remains the same.
          node.LeftChild = inNode.RightChild;
          inNode.RightChild = node;
        }
        else
        {
          // wrap terminal or any node
          node.LeftChild = topNode;
          topNode = node;
        }
        stack.Push(topNode);
      }
      else if (contentNode != null)
      { //If there is content to wrap
        // wrap whole content
        node.LeftChild = contentNode;
        contentNode = node;
      }
    }

    public ContentValidator Finish()
    {
      return Finish(true);
    }

    public ContentValidator Finish(bool useDFA)
    {
      if (contentNode == null)
      {
        if (ContentType == XmlSchemaContentType.Mixed)
        {
          string ctype = IsOpen ? "Any" : "TextOnly";
          return IsOpen ? ContentValidator.Any : ContentValidator.TextOnly;
        }
        else
        {
          return ContentValidator.Empty;
        }
      }

      // Add end marker
      InteriorNode contentRoot = new SequenceNode();
      contentRoot.LeftChild = contentNode;
      LeafNode endMarker = new LeafNode(positions.Add(symbols.AddName(XmlQualifiedName.Empty, null), null));
      contentRoot.RightChild = endMarker;

      // Eliminate NamespaceListNode(s) and RangeNode(s)
      contentNode.ExpandTree(contentRoot, symbols, positions);

      // calculate followpos
      int symbolsCount = symbols.Count;
      int positionsCount = positions.Count;
      BitSet firstpos = new BitSet(positionsCount);
      BitSet lastpos = new BitSet(positionsCount);
      BitSet[] followpos = new BitSet[positionsCount];
      for (int i = 0; i < positionsCount; i++)
      {
        followpos[i] = new BitSet(positionsCount);
      }
      contentRoot.ConstructPos(firstpos, lastpos, followpos);
      if (minMaxNodesCount > 0)
      { //If the tree has any terminal range nodes
        BitSet positionsWithRangeTerminals;
        BitSet[] minMaxFollowPos = CalculateTotalFollowposForRangeNodes(firstpos, followpos, out positionsWithRangeTerminals);

        if (enableUpaCheck)
        {
          CheckCMUPAWithLeafRangeNodes(GetApplicableMinMaxFollowPos(firstpos, positionsWithRangeTerminals, minMaxFollowPos));
          for (int i = 0; i < positionsCount; i++)
          {
            CheckCMUPAWithLeafRangeNodes(GetApplicableMinMaxFollowPos(followpos[i], positionsWithRangeTerminals, minMaxFollowPos));
          }
        }
        return new RangeContentValidator(firstpos, followpos, symbols, positions, endMarker.Pos, this.ContentType, contentRoot.LeftChild.IsNullable, positionsWithRangeTerminals, minMaxNodesCount);
      }
      else
      {
        int[][] transitionTable = null;
        // if each symbol has unique particle we are golden
        if (!symbols.IsUpaEnforced)
        {
          if (enableUpaCheck)
          {
            // multiple positions that match the same symbol have different particles, but they never follow the same position
            CheckUniqueParticleAttribution(firstpos, followpos);
          }
        }
        else if (useDFA)
        {
          // Can return null if the number of states reaches higher than 8192 / positionsCount
          transitionTable = BuildTransitionTable(firstpos, followpos, endMarker.Pos);
        }
        if (transitionTable != null)
        {
          return new DfaContentValidator(transitionTable, symbols, this.ContentType, this.IsOpen, contentRoot.LeftChild.IsNullable);
        }
        else
        {
          return new NfaContentValidator(firstpos, followpos, symbols, positions, endMarker.Pos, this.ContentType, this.IsOpen, contentRoot.LeftChild.IsNullable);
        }
      }
    }

    private BitSet[] CalculateTotalFollowposForRangeNodes(BitSet firstpos, BitSet[] followpos, out BitSet posWithRangeTerminals)
    {
      int positionsCount = positions.Count; //terminals
      posWithRangeTerminals = new BitSet(positionsCount);

      //Compute followpos for each range node
      //For any range node that is surrounded by an outer range node, its follow positions will include those of the outer range node
      BitSet[] minmaxFollowPos = new BitSet[minMaxNodesCount];
      int localMinMaxNodesCount = 0;

      for (int i = positionsCount - 1; i >= 0; i--)
      {
        Position p = positions[i];
        if (p.symbol == -2)
        { //P is a LeafRangeNode
          LeafRangeNode lrNode = p.particle as LeafRangeNode;
          BitSet tempFollowPos = new BitSet(positionsCount);
          tempFollowPos.Clear();
          tempFollowPos.Or(followpos[i]); //Add the followpos of the range node
          if (lrNode.Min != lrNode.Max)
          { //If they are the same, then followpos cannot include the firstpos
            tempFollowPos.Or(lrNode.NextIteration); //Add the nextIteration of the range node (this is the firstpos of its parent's leftChild)
          }

          //For each position in the bitset, if it is a outer range node (pos > i), then add its followpos as well to the current node's followpos
          for (int pos = tempFollowPos.NextSet(-1); pos != -1; pos = tempFollowPos.NextSet(pos))
          {
            if (pos > i)
            {
              Position p1 = positions[pos];
              if (p1.symbol == -2)
              {
                LeafRangeNode lrNode1 = p1.particle as LeafRangeNode;
                tempFollowPos.Or(minmaxFollowPos[lrNode1.Pos]);
              }
            }
          }
          //set the followpos built to the index in the BitSet[]
          minmaxFollowPos[localMinMaxNodesCount] = tempFollowPos;
          lrNode.Pos = localMinMaxNodesCount++;
          posWithRangeTerminals.Set(i);
        }
      }
      return minmaxFollowPos;
    }

    private void CheckCMUPAWithLeafRangeNodes(BitSet curpos)
    {
      object[] symbolMatches = new object[symbols.Count];
      for (int pos = curpos.NextSet(-1); pos != -1; pos = curpos.NextSet(pos))
      {
        Position currentPosition = positions[pos];
        int symbol = currentPosition.symbol;
        if (symbol >= 0)
        { //its not a range position
          if (symbolMatches[symbol] != null)
          {
            throw new UpaException(symbolMatches[symbol], currentPosition.particle);
          }
          else
          {
            symbolMatches[symbol] = currentPosition.particle;
          }
        }
      }
    }

    //For each position, this method calculates the additional follows of any range nodes that need to be added to its followpos
    //((ab?)2-4)c, Followpos of a is b as well as that of node R(2-4) = c
    private BitSet GetApplicableMinMaxFollowPos(BitSet curpos, BitSet posWithRangeTerminals, BitSet[] minmaxFollowPos)
    {
      if (curpos.Intersects(posWithRangeTerminals))
      {
        BitSet newSet = new BitSet(positions.Count); //Doing work again 
        newSet.Or(curpos);
        newSet.And(posWithRangeTerminals);
        curpos = curpos.Clone();
        for (int pos = newSet.NextSet(-1); pos != -1; pos = newSet.NextSet(pos))
        {
          LeafRangeNode lrNode = positions[pos].particle as LeafRangeNode;
          curpos.Or(minmaxFollowPos[lrNode.Pos]);
        }
      }
      return curpos;
    }

    private void CheckUniqueParticleAttribution(BitSet firstpos, BitSet[] followpos)
    {
      CheckUniqueParticleAttribution(firstpos);
      for (int i = 0; i < positions.Count; i++)
      {
        CheckUniqueParticleAttribution(followpos[i]);
      }
    }

    private void CheckUniqueParticleAttribution(BitSet curpos)
    {
      // particles will be attributed uniquely if the same symbol never poins to two different ones
      object[] particles = new object[symbols.Count];
      for (int pos = curpos.NextSet(-1); pos != -1; pos = curpos.NextSet(pos))
      {
        // if position can follow
        int symbol = positions[pos].symbol;
        if (particles[symbol] == null)
        {
          // set particle for the symbol
          particles[symbol] = positions[pos].particle;
        }
        else if (particles[symbol] != positions[pos].particle)
        {
          throw new UpaException(particles[symbol], positions[pos].particle);
        }
        // two different position point to the same symbol and particle - that's OK
      }
    }

    /// <summary>
    /// Algorithm 3.5 Construction of a DFA from a regular expression
    /// </summary>
    private int[][] BuildTransitionTable(BitSet firstpos, BitSet[] followpos, int endMarkerPos)
    {
      const int TimeConstant = 8192; //(MaxStates * MaxPositions should be a constant) 
      int positionsCount = positions.Count;
      int MaxStatesCount = TimeConstant / positionsCount;
      int symbolsCount = symbols.Count;

      // transition table (Dtran in the book)
      ArrayList transitionTable = new ArrayList();

      // state lookup table (Dstate in the book)
      Hashtable stateTable = new Hashtable();

      // Add empty set that would signal an error
      stateTable.Add(new BitSet(positionsCount), -1);

      // lists unmarked states
      Queue unmarked = new Queue();

      // initially, the only unmarked state in Dstates is firstpo(root) 
      int state = 0;
      unmarked.Enqueue(firstpos);
      stateTable.Add(firstpos, 0);
      transitionTable.Add(new int[symbolsCount + 1]);

      // while there is an umnarked state T in Dstates do begin
      while (unmarked.Count > 0)
      {
        BitSet statePosSet = (BitSet)unmarked.Dequeue(); // all positions that constitute DFA state 
        int[] transition = (int[])transitionTable[state];
        if (statePosSet[endMarkerPos])
        {
          transition[symbolsCount] = 1;   // accepting
        }

        // for each input symbol a do begin
        for (int symbol = 0; symbol < symbolsCount; symbol++)
        {
          // let U be the set of positions that are in followpos(p)
          //       for some position p in T
          //       such that the symbol at position p is a
          BitSet newset = new BitSet(positionsCount);
          for (int pos = statePosSet.NextSet(-1); pos != -1; pos = statePosSet.NextSet(pos))
          {
            if (symbol == positions[pos].symbol)
            {
              newset.Or(followpos[pos]);
            }
          }

          // if U is not empty and is not in Dstates then
          //      add U as an unmarked state to Dstates
          object lookup = stateTable[newset];
          if (lookup != null)
          {
            transition[symbol] = (int)lookup;
          }
          else
          {
            // construct new state
            int newState = stateTable.Count - 1;
            if (newState >= MaxStatesCount)
            {
              return null;
            }
            unmarked.Enqueue(newset);
            stateTable.Add(newset, newState);
            transitionTable.Add(new int[symbolsCount + 1]);
            transition[symbol] = newState;
          }
        }
        state++;
      }
      // now convert transition table to array
      return (int[][])transitionTable.ToArray(typeof(int[]));
    }
  }
}
