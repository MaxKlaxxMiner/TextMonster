using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  sealed class DfaContentValidator : ContentValidator
  {
    int[][] transitionTable;
    SymbolsDictionary symbols;

    /// <summary>
    /// Algorithm 3.5 Construction of a DFA from a regular expression
    /// </summary>
    internal DfaContentValidator(
        int[][] transitionTable, SymbolsDictionary symbols,
        XmlSchemaContentType contentType, bool isOpen, bool isEmptiable)
      : base(contentType, isOpen, isEmptiable)
    {
      this.transitionTable = transitionTable;
      this.symbols = symbols;
    }

    public override void InitValidation(ValidationState context)
    {
      context.CurrentState.State = 0;
      context.HasMatched = transitionTable[0][symbols.Count] > 0;
    }

    /// <summary>
    /// Algorithm 3.1 Simulating a DFA
    /// </summary>
    public override object ValidateElement(XmlQualifiedName name, ValidationState context, out int errorCode)
    {
      int symbol = symbols[name];
      int state = transitionTable[context.CurrentState.State][symbol];
      errorCode = 0;
      if (state != -1)
      {
        context.CurrentState.State = state;
        context.HasMatched = transitionTable[context.CurrentState.State][symbols.Count] > 0;
        return symbols.GetParticle(symbol); // OK
      }
      if (IsOpen && context.HasMatched)
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
      if (!context.HasMatched)
      {
        return false;
      }
      return true;
    }

    public override ArrayList ExpectedElements(ValidationState context, bool isRequiredOnly)
    {
      ArrayList names = null;
      int[] transition = transitionTable[context.CurrentState.State];
      if (transition != null)
      {
        for (int i = 0; i < transition.Length - 1; i++)
        {
          if (transition[i] != -1)
          {
            if (names == null)
            {
              names = new ArrayList();
            }
            XmlSchemaParticle p = (XmlSchemaParticle)symbols.GetParticle(i);
            if (p == null)
            {
              string s = symbols.NameOf(i);
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
      int[] transition = transitionTable[context.CurrentState.State];
      if (transition != null)
      {
        for (int i = 0; i < transition.Length - 1; i++)
        {
          if (transition[i] != -1)
          {
            XmlSchemaParticle p = (XmlSchemaParticle)symbols.GetParticle(i);
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
