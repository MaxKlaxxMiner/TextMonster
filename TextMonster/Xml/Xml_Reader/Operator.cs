namespace TextMonster.Xml.Xml_Reader
{
  internal class Operator : AstNode
  {
    public enum Op
    { // order is alligned with XPathOperator
      INVALID,
      /*Logical   */
      OR,
      AND,
      /*Equality  */
      EQ,
      NE,
      /*Relational*/
      LT,
      LE,
      GT,
      GE,
      /*Arithmetic*/
      PLUS,
      MINUS,
      MUL,
      DIV,
      MOD,
      /*Union     */
      UNION,
    };

    private Op opType;
    private AstNode opnd1;
    private AstNode opnd2;

    public Operator(Op op, AstNode opnd1, AstNode opnd2)
    {
      this.opType = op;
      this.opnd1 = opnd1;
      this.opnd2 = opnd2;
    }

    public override AstType Type { get { return AstType.Operator; } }
    public override XPathResultType ReturnType
    {
      get
      {
        if (opType <= Op.GE)
        {
          return XPathResultType.Boolean;
        }
        if (opType <= Op.MOD)
        {
          return XPathResultType.Number;
        }
        return XPathResultType.NodeSet;
      }
    }
  }
}