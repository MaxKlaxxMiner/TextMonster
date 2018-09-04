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
      get { return this.tableDim; }
    }

    internal ConstraintStruct(CompiledIdentityConstraint constraint)
    {
      this.constraint = constraint;
      this.tableDim = constraint.Fields.Length;
      this.axisFields = new ArrayList();              // empty fields
      this.axisSelector = new SelectorActiveAxis(constraint.Selector, this);
      if (this.constraint.Role != CompiledIdentityConstraint.ConstraintRole.Keyref)
      {
        this.qualifiedTable = new Hashtable();
      }
    }

  }
}
