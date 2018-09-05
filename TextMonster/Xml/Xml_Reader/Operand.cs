namespace TextMonster.Xml.Xml_Reader
{
  internal class Operand : AstNode
  {
    private XPathResultType type;
    private object val;

    public Operand(string val)
    {
      this.type = XPathResultType.String;
      this.val = val;
    }

    public Operand(double val)
    {
      this.type = XPathResultType.Number;
      this.val = val;
    }

    public Operand(bool val)
    {
      this.type = XPathResultType.Boolean;
      this.val = val;
    }

    public override AstType Type { get { return AstType.ConstantOperand; } }
    public override XPathResultType ReturnType { get { return type; } }

    public object OperandValue { get { return val; } }
  }
}