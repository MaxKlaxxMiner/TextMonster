namespace TextMonster.Xml.Xml_Reader
{
  // only keep axis, rootNode, isAttribute, isDss inside
  // act as an element tree for the Asttree
  internal class ForwardAxis
  {
    // Axis tree
    private DoubleLinkAxis topNode;
    private DoubleLinkAxis rootNode;                // the root for reverse Axis

    // Axis tree property
    private bool isAttribute;                       // element or attribute?      "@"?
    private bool isDss;                             // has ".//" in front of it?
    private bool isSelfAxis;                        // only one node in the tree, and it's "." (self) node

    internal DoubleLinkAxis RootNode
    {
      get { return this.rootNode; }
    }

    internal DoubleLinkAxis TopNode
    {
      get { return this.topNode; }
    }

    internal bool IsAttribute
    {
      get { return this.isAttribute; }
    }

    // has ".//" in front of it?
    internal bool IsDss
    {
      get { return this.isDss; }
    }

    internal bool IsSelfAxis
    {
      get { return this.isSelfAxis; }
    }

    public ForwardAxis(DoubleLinkAxis axis, bool isdesorself)
    {
      this.isDss = isdesorself;
      this.isAttribute = Asttree.IsAttribute(axis);
      this.topNode = axis;
      this.rootNode = axis;
      while (this.rootNode.Input != null)
      {
        this.rootNode = (DoubleLinkAxis)(this.rootNode.Input);
      }
      // better to calculate it out, since it's used so often, and if the top is self then the whole tree is self
      this.isSelfAxis = Asttree.IsSelf(this.topNode);
    }
  }
}
