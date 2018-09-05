namespace TextMonster.Xml.Xml_Reader
{
  internal class Group : AstNode
  {
    private AstNode groupNode;

    public Group(AstNode groupNode)
    {
      this.groupNode = groupNode;
    }
    public override AstType Type { get { return AstType.Group; } }
    public override XPathResultType ReturnType { get { return XPathResultType.NodeSet; } }

    public AstNode GroupNode { get { return groupNode; } }
  }
}