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
      get { return role; }
    }

    public Asttree Selector
    {
      get { return selector; }
    }

    public Asttree[] Fields
    {
      get { return fields; }
    }

    private CompiledIdentityConstraint() { }
  }
}
