﻿namespace TextMonster.Xml.Xml_Reader
{
  internal abstract class AstNode
  {
    public enum AstType
    {
      Axis,
      Operator,
      Filter,
      ConstantOperand,
      Function,
      Group,
      Root,
      Variable,
      Error
    };

    public abstract AstType Type { get; }
    public abstract XPathResultType ReturnType { get; }
  }
}