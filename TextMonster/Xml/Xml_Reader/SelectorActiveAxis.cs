using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  // exist for optimization purpose
  // ActiveAxis plus
  // 1. overload endelement function from parent to return result
  // 2. combine locatedactiveaxis and keysequence more closely
  // 3. enable locatedactiveaxis reusing (the most important optimization point)
  // 4. enable ks adding to hashtable right after moving out selector node (to enable 3)
  // 5. will modify locatedactiveaxis class accordingly
  // 6. taking care of updating ConstraintStruct.axisFields
  // 7. remove constraintTable from ConstraintStruct
  // 8. still need centralized locatedactiveaxis for movetoattribute purpose
  internal class SelectorActiveAxis : ActiveAxis
  {
    private ConstraintStruct cs;            // pointer of constraintstruct, to enable 6
    private ArrayList KSs;                  // stack of KSStruct, will not become less 
    private int KSpointer = 0;              // indicate current stack top (next available element);

    public int lastDepth
    {
      get { return (KSpointer == 0) ? -1 : ((KsStruct)KSs[KSpointer - 1]).depth; }
    }

    public SelectorActiveAxis(Asttree axisTree, ConstraintStruct cs)
      : base(axisTree)
    {
      this.KSs = new ArrayList();
      this.cs = cs;
    }

    public override bool EndElement(string localname, string URN)
    {
      base.EndElement(localname, URN);
      if (KSpointer > 0 && this.CurrentDepth == lastDepth)
      {
        return true;
        // next step PopPS, and insert into hash
      }
      return false;
    }

    // update constraintStruct.axisFields as well, if it's new LocatedActiveAxis
    public int PushKS(int errline, int errcol)
    {
      // new KeySequence each time
      KeySequence ks = new KeySequence(cs.TableDim, errline, errcol);

      // needs to clear KSStruct before using
      KsStruct kss;
      if (KSpointer < KSs.Count)
      {
        // reuse, clear up KSs.KSpointer
        kss = (KsStruct)KSs[KSpointer];
        kss.ks = ks;
        // reactivate LocatedActiveAxis
        for (int i = 0; i < cs.TableDim; i++)
        {
          kss.fields[i].Reactivate(ks);               // reassociate key sequence
        }
      }
      else
      { // "==", new
        kss = new KsStruct(ks, cs.TableDim);
        for (int i = 0; i < cs.TableDim; i++)
        {
          kss.fields[i] = new LocatedActiveAxis(cs.constraint.Fields[i], ks, i);
          cs.axisFields.Add(kss.fields[i]);          // new, add to axisFields
        }
        KSs.Add(kss);
      }

      kss.depth = this.CurrentDepth - 1;

      return (KSpointer++);
    }

    public KeySequence PopKS()
    {
      return ((KsStruct)KSs[--KSpointer]).ks;
    }

  }
}
