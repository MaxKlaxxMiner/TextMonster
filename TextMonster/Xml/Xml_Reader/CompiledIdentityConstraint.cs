namespace TextMonster.Xml.Xml_Reader
{
  internal class CompiledIdentityConstraint
  {
    internal XmlQualifiedName name = XmlQualifiedName.Empty;
    private ConstraintRole role;
    private Asttree selector;
    private Asttree[] fields;
    internal XmlQualifiedName refer = XmlQualifiedName.Empty;

    public enum ConstraintRole
    {
      Unique,
      Key,
      Keyref
    }

    public ConstraintRole Role
    {
      get { return this.role; }
    }

    public Asttree Selector
    {
      get { return this.selector; }
    }

    public Asttree[] Fields
    {
      get { return this.fields; }
    }

    private CompiledIdentityConstraint() { }
  }
}
