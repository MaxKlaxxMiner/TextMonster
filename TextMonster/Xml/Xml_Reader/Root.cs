namespace TextMonster.Xml.Xml_Reader
{
  internal class Root : AstNode
  {
    public override AstType Type { get { return AstType.Root; } }
    public override XPathResultType ReturnType { get { return XPathResultType.NodeSet; } }
  }
}
