using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  sealed class NfaContentValidator : ContentValidator
  {
    BitSet firstpos;
    BitSet[] followpos;
    SymbolsDictionary symbols;
    Positions positions;
    int endMarkerPos;

    internal NfaContentValidator(
        BitSet firstpos, BitSet[] followpos, SymbolsDictionary symbols, Positions positions, int endMarkerPos,
        XmlSchemaContentType contentType, bool isOpen, bool isEmptiable)
      : base(contentType, isOpen, isEmptiable)
    {
      this.firstpos = firstpos;
      this.followpos = followpos;
      this.symbols = symbols;
      this.positions = positions;
      this.endMarkerPos = endMarkerPos;
    }

    public override void InitValidation(ValidationState context)
    {
      context.CurPos[0] = firstpos.Clone();
      context.CurPos[1] = new BitSet(firstpos.Count);
      context.CurrentState.CurPosIndex = 0;
    }

    /// <summary>
    /// Algorithm 3.4 Simulation of an NFA
    /// </summary>
    public override object ValidateElement(XmlQualifiedName name, ValidationState context, out int errorCode)
    {
      BitSet curpos = context.CurPos[context.CurrentState.CurPosIndex];
      int next = (context.CurrentState.CurPosIndex + 1) % 2;
      BitSet nextpos = context.CurPos[next];
      nextpos.Clear();
      int symbol = symbols[name];
      object particle = null;
      errorCode = 0;
      for (int pos = curpos.NextSet(-1); pos != -1; pos = curpos.NextSet(pos))
      {
        // if position can follow
        if (symbol == positions[pos].symbol)
        {
          nextpos.Or(followpos[pos]);
          particle = positions[pos].particle; //Between element and wildcard, element will be in earlier pos than wildcard since we add the element nodes to the list of positions first
          break;                              // and then ExpandTree for the namespace nodes which adds the wildcards to the positions list
        }
      }
      if (!nextpos.IsEmpty)
      {
        context.CurrentState.CurPosIndex = next;
        return particle;
      }
      if (IsOpen && curpos[endMarkerPos])
      {
        // XDR allows any well-formed contents after matched.
        return null;
      }
      context.NeedValidateChildren = false;
      errorCode = -1;
      return null; // will never be here

    }

    public override bool CompleteValidation(ValidationState context)
    {
      if (!context.CurPos[context.CurrentState.CurPosIndex][endMarkerPos])
      {
        return false;
      }
      return true;
    }

    public override ArrayList ExpectedElements(ValidationState context, bool isRequiredOnly)
    {
      ArrayList names = null;
      BitSet curpos = context.CurPos[context.CurrentState.CurPosIndex];
      for (int pos = curpos.NextSet(-1); pos != -1; pos = curpos.NextSet(pos))
      {
        if (names == null)
        {
          names = new ArrayList();
        }
        XmlSchemaParticle p = (XmlSchemaParticle)positions[pos].particle;
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
      return names;
    }

    public override ArrayList ExpectedParticles(ValidationState context, bool isRequiredOnly, XmlSchemaSet schemaSet)
    {
      ArrayList particles = new ArrayList();
      BitSet curpos = context.CurPos[context.CurrentState.CurPosIndex];
      for (int pos = curpos.NextSet(-1); pos != -1; pos = curpos.NextSet(pos))
      {
        XmlSchemaParticle p = (XmlSchemaParticle)positions[pos].particle;
        if (p == null)
        {
          continue;
        }
        else
        {
          AddParticleToExpected(p, schemaSet, particles);
        }
      }
      return particles;
    }
  }
}
