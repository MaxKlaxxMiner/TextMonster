using System;
using System.Collections;
using System.Collections.Generic;

namespace TextMonster.Xml.Xml_Reader
{
  sealed class RangeContentValidator : ContentValidator
  {
    BitSet firstpos;
    BitSet[] followpos;
    BitSet positionsWithRangeTerminals;
    SymbolsDictionary symbols;
    Positions positions;
    int minMaxNodesCount;
    int endMarkerPos;

    internal RangeContentValidator(
        BitSet firstpos, BitSet[] followpos, SymbolsDictionary symbols, Positions positions, int endMarkerPos, XmlSchemaContentType contentType, bool isEmptiable, BitSet positionsWithRangeTerminals, int minmaxNodesCount)
      : base(contentType, false, isEmptiable)
    {
      this.firstpos = firstpos;
      this.followpos = followpos;
      this.symbols = symbols;
      this.positions = positions;
      this.positionsWithRangeTerminals = positionsWithRangeTerminals;
      this.minMaxNodesCount = minmaxNodesCount;
      this.endMarkerPos = endMarkerPos;
    }

    public override void InitValidation(ValidationState context)
    {
      List<RangePositionInfo> runningPositions = context.RunningPositions;
      if (runningPositions != null)
      {
        runningPositions.Clear();
      }
      else
      {
        runningPositions = new List<RangePositionInfo>();
        context.RunningPositions = runningPositions;
      }
      RangePositionInfo rposInfo = new RangePositionInfo();
      rposInfo.curpos = firstpos.Clone();

      rposInfo.rangeCounters = new decimal[minMaxNodesCount];
      runningPositions.Add(rposInfo);
      context.CurrentState.NumberOfRunningPos = 1;
      context.HasMatched = rposInfo.curpos.Get(endMarkerPos);
    }

    public override object ValidateElement(XmlQualifiedName name, ValidationState context, out int errorCode)
    {
      errorCode = 0;
      int symbol = symbols[name];
      bool hasSeenFinalPosition = false;
      List<RangePositionInfo> runningPositions = context.RunningPositions;
      int matchCount = context.CurrentState.NumberOfRunningPos;
      int k = 0;
      RangePositionInfo rposInfo;

      int pos = -1;
      int firstMatchedIndex = -1;
      bool matched = false;

      while (k < matchCount)
      { //we are looking for the first match in the list of bitsets
        rposInfo = runningPositions[k];
        BitSet curpos = rposInfo.curpos;
        for (int matchpos = curpos.NextSet(-1); matchpos != -1; matchpos = curpos.NextSet(matchpos))
        { //In all sets, have to scan all positions because of Disabled UPA possibility
          if (symbol == positions[matchpos].symbol)
          {
            pos = matchpos;
            if (firstMatchedIndex == -1)
            { // get the first match for this symbol
              firstMatchedIndex = k;
            }
            matched = true;
            break;
          }
        }
        if (matched && positions[pos].particle is XmlSchemaElement)
        { //We found a match in the list, break at that bitset
          break;
        }
        else
        {
          k++;
        }
      }

      if (k == matchCount && pos != -1)
      { // we did find a match but that was any and hence continued ahead for element
        k = firstMatchedIndex;
      }
      if (k < matchCount)
      { //There is a match
        if (k != 0)
        { //If the first bitset itself matched, then no need to remove anything
          runningPositions.RemoveRange(0, k); //Delete entries from 0 to k-1
        }
        matchCount = matchCount - k;
        k = 0; // Since we re-sized the array
        while (k < matchCount)
        {
          rposInfo = runningPositions[k];
          matched = rposInfo.curpos.Get(pos); //Look for the bitset that matches the same position as pos
          if (matched)
          { //If match found, get the follow positions of the current matched position
            rposInfo.curpos = followpos[pos]; //Note that we are copying the same counters of the current position to that of the follow position
            runningPositions[k] = rposInfo;
            k++;
          }
          else
          { //Clear the current pos and get another position from the list to start matching
            matchCount--;
            if (matchCount > 0)
            {
              RangePositionInfo lastrpos = runningPositions[matchCount];
              runningPositions[matchCount] = runningPositions[k];
              runningPositions[k] = lastrpos;
            }
          }
        }
      }
      else
      { //There is no match
        matchCount = 0;
      }

      if (matchCount > 0)
      {
        if (matchCount >= 10000)
        {
          context.TooComplex = true;
          matchCount /= 2;
        }

        for (k = matchCount - 1; k >= 0; k--)
        {
          int j = k;
          BitSet currentRunningPosition = runningPositions[k].curpos;
          hasSeenFinalPosition = hasSeenFinalPosition || currentRunningPosition.Get(endMarkerPos); //Accepting position reached if the current position BitSet contains the endPosition
          while (matchCount < 10000 && currentRunningPosition.Intersects(positionsWithRangeTerminals))
          {
            //Now might add 2 more positions to followpos 
            //1. nextIteration of the rangeNode, which is firstpos of its parent's leftChild
            //2. Followpos of the range node

            BitSet countingPosition = currentRunningPosition.Clone();
            countingPosition.And(positionsWithRangeTerminals);
            int cPos = countingPosition.NextSet(-1); //Get the first position where leaf range node appears
            LeafRangeNode lrNode = positions[cPos].particle as LeafRangeNode; //For a position with leaf range node, the particle is the node itself

            rposInfo = runningPositions[j];
            if (matchCount + 2 >= runningPositions.Count)
            {
              runningPositions.Add(new RangePositionInfo());
              runningPositions.Add(new RangePositionInfo());
            }
            RangePositionInfo newRPosInfo = runningPositions[matchCount];
            if (newRPosInfo.rangeCounters == null)
            {
              newRPosInfo.rangeCounters = new decimal[minMaxNodesCount];
            }
            Array.Copy(rposInfo.rangeCounters, 0, newRPosInfo.rangeCounters, 0, rposInfo.rangeCounters.Length);
            decimal count = ++newRPosInfo.rangeCounters[lrNode.Pos];

            if (count == lrNode.Max)
            {
              newRPosInfo.curpos = followpos[cPos]; //since max has been reached, Get followposition of range node
              newRPosInfo.rangeCounters[lrNode.Pos] = 0; //reset counter
              runningPositions[matchCount] = newRPosInfo;
              j = matchCount++;
            }
            else if (count < lrNode.Min)
            {
              newRPosInfo.curpos = lrNode.NextIteration;
              runningPositions[matchCount] = newRPosInfo;
              matchCount++;
              break;
            }
            else
            { // min <= count < max
              newRPosInfo.curpos = lrNode.NextIteration; //set currentpos to firstpos of node which has the range
              runningPositions[matchCount] = newRPosInfo;
              j = matchCount + 1;
              newRPosInfo = runningPositions[j];
              if (newRPosInfo.rangeCounters == null)
              {
                newRPosInfo.rangeCounters = new decimal[minMaxNodesCount];
              }
              Array.Copy(rposInfo.rangeCounters, 0, newRPosInfo.rangeCounters, 0, rposInfo.rangeCounters.Length);
              newRPosInfo.curpos = followpos[cPos];
              newRPosInfo.rangeCounters[lrNode.Pos] = 0;
              runningPositions[j] = newRPosInfo;
              matchCount += 2;
            }
            currentRunningPosition = runningPositions[j].curpos;
            hasSeenFinalPosition = hasSeenFinalPosition || currentRunningPosition.Get(endMarkerPos);
          }
        }
        context.HasMatched = hasSeenFinalPosition;
        context.CurrentState.NumberOfRunningPos = matchCount;
        return positions[pos].particle;
      } //matchcount > 0
      errorCode = -1;
      context.NeedValidateChildren = false;
      return null;
    }

    public override bool CompleteValidation(ValidationState context)
    {
      return context.HasMatched;
    }

    public override ArrayList ExpectedElements(ValidationState context, bool isRequiredOnly)
    {
      ArrayList names = null;
      BitSet expectedPos;
      if (context.RunningPositions != null)
      {
        List<RangePositionInfo> runningPositions = context.RunningPositions;
        expectedPos = new BitSet(positions.Count);
        for (int i = context.CurrentState.NumberOfRunningPos - 1; i >= 0; i--)
        {
          expectedPos.Or(runningPositions[i].curpos);
        }
        for (int pos = expectedPos.NextSet(-1); pos != -1; pos = expectedPos.NextSet(pos))
        {
          if (names == null)
          {
            names = new ArrayList();
          }
          int symbol = positions[pos].symbol;
          if (symbol >= 0)
          { //non range nodes
            XmlSchemaParticle p = positions[pos].particle as XmlSchemaParticle;
            if (p == null)
            {
              string s = symbols.NameOf(positions[pos].symbol);
              if (s.Length != 0)
              {
                names.Add(s);
              }
            }
            else
            {
              string s = p.NameString;
              if (!names.Contains(s))
              {
                names.Add(s);
              }
            }
          }
        }
      }
      return names;
    }

    public override ArrayList ExpectedParticles(ValidationState context, bool isRequiredOnly, XmlSchemaSet schemaSet)
    {
      ArrayList particles = new ArrayList();
      BitSet expectedPos;
      if (context.RunningPositions != null)
      {
        List<RangePositionInfo> runningPositions = context.RunningPositions;
        expectedPos = new BitSet(positions.Count);
        for (int i = context.CurrentState.NumberOfRunningPos - 1; i >= 0; i--)
        {
          expectedPos.Or(runningPositions[i].curpos);
        }
        for (int pos = expectedPos.NextSet(-1); pos != -1; pos = expectedPos.NextSet(pos))
        {
          int symbol = positions[pos].symbol;
          if (symbol >= 0)
          { //non range nodes
            XmlSchemaParticle p = positions[pos].particle as XmlSchemaParticle;
            if (p == null)
            {
              continue;
            }
            AddParticleToExpected(p, schemaSet, particles);
          }
        }
      }
      return particles;
    }
  }
}
