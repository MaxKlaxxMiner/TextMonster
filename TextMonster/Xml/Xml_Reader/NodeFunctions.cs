namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class NodeFunctions : ValueQuery
  {
    Query arg = null;
    Function.FunctionType funcType;
    XsltContext xsltContext;

    public NodeFunctions(Function.FunctionType funcType, Query arg)
    {
      this.funcType = funcType;
      this.arg = arg;
    }

    public override void SetXsltContext(XsltContext context)
    {
      this.xsltContext = context.Whitespace ? context : null;
      if (arg != null)
      {
        arg.SetXsltContext(context);
      }
    }

    private XPathNavigator EvaluateArg(XPathNodeIterator context)
    {
      if (arg == null)
      {
        return context.Current;
      }
      arg.Evaluate(context);
      return arg.Advance();
    }

    public override object Evaluate(XPathNodeIterator context)
    {
      XPathNavigator argVal;

      switch (funcType)
      {
        case Function.FunctionType.FuncPosition:
          return (double)context.CurrentPosition;
        case Function.FunctionType.FuncLast:
          return (double)context.Count;
        case Function.FunctionType.FuncNameSpaceUri:
          argVal = EvaluateArg(context);
          if (argVal != null)
          {
            return argVal.NamespaceURI;
          }
          break;
        case Function.FunctionType.FuncLocalName:
          argVal = EvaluateArg(context);
          if (argVal != null)
          {
            return argVal.LocalName;
          }
          break;
        case Function.FunctionType.FuncName:
          argVal = EvaluateArg(context);
          if (argVal != null)
          {
            return argVal.Name;
          }
          break;
        case Function.FunctionType.FuncCount:
          arg.Evaluate(context);
          int count = 0;
          if (xsltContext != null)
          {
            XPathNavigator nav;
            while ((nav = arg.Advance()) != null)
            {
              if (nav.NodeType != XPathNodeType.Whitespace || xsltContext.PreserveWhitespace(nav))
              {
                count++;
              }
            }
          }
          else
          {
            while (arg.Advance() != null)
            {
              count++;
            }
          }
          return (double)count;
      }
      return string.Empty;
    }

    public override XPathResultType StaticType { get { return Function.ReturnTypes[(int)funcType]; } }

    public override XPathNodeIterator Clone()
    {
      NodeFunctions method = new NodeFunctions(funcType, Clone(arg));
      method.xsltContext = this.xsltContext;
      return method;
    }

    public override void PrintQuery(XmlWriter w)
    {
      w.WriteStartElement(this.GetType().Name);
      w.WriteAttributeString("name", funcType.ToString());
      if (arg != null)
      {
        arg.PrintQuery(w);
      }
      w.WriteEndElement();
    }
  }
}