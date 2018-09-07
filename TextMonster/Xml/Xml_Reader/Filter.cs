namespace TextMonster.Xml.Xml_Reader
{
  internal class Filter : AstNode
  {
    private AstNode input;
    private AstNode condition;

    public Filter(AstNode input, AstNode condition)
    {
      this.input = input;
      this.condition = condition;
    }

    public override AstType Type { get { return AstType.Filter; } }
    public override XPathResultType ReturnType { get { return XPathResultType.NodeSet; } }
  }
}