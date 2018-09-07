using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class ConstraintStruct
  {
    // for each constraint
    internal CompiledIdentityConstraint constraint;     // pointer to constraint
    internal SelectorActiveAxis axisSelector;
    internal ArrayList axisFields;                     // Add tableDim * LocatedActiveAxis in a loop
    internal Hashtable qualifiedTable;                 // Checking confliction
    internal Hashtable keyrefTable;                    // several keyref tables having connections to this one is possible
    private int tableDim;                               // dimension of table = numbers of fields;

    internal int TableDim
    {
      get { return tableDim; }
    }

    internal ConstraintStruct(CompiledIdentityConstraint constraint)
    {
      this.constraint = constraint;
      tableDim = constraint.Fields.Length;
      axisFields = new ArrayList();              // empty fields
      axisSelector = new SelectorActiveAxis(constraint.Selector, this);
      if (this.constraint.Role != CompiledIdentityConstraint.ConstraintRole.Keyref)
      {
        qualifiedTable = new Hashtable();
      }
    }

  }
}
