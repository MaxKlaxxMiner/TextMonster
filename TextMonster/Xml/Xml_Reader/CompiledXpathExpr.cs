namespace TextMonster.Xml.Xml_Reader
{
  internal class CompiledXpathExpr : XPathExpression
  {
    Query query;
    string expr;
    bool needContext;

    internal CompiledXpathExpr(Query query, string expression, bool needContext)
    {
      this.query = query;
      this.expr = expression;
      this.needContext = needContext;
    }

    internal Query QueryTree
    {
      get
      {
        if (needContext)
        {
          throw XPathException.Create(Res.Xp_NoContext);
        }
        return query;
      }
    }

    public override string Expression
    {
      get { return expr; }
    }

    public override void SetContext(IXmlNamespaceResolver nsResolver)
    {
      XsltContext xsltContext = nsResolver as XsltContext;
      if (xsltContext == null)
      {
        if (nsResolver == null)
        {
          nsResolver = new XmlNamespaceManager(new NameTable());
        }
        xsltContext = new UndefinedXsltContext(nsResolver);
      }
      query.SetXsltContext(xsltContext);

      needContext = false;
    }

    private class UndefinedXsltContext : XsltContext
    {
      private IXmlNamespaceResolver nsResolver;

      public UndefinedXsltContext(IXmlNamespaceResolver nsResolver)
        : base(/*dummy*/false)
      {
        this.nsResolver = nsResolver;
      }
      //----- Namespace support -----
      public override string DefaultNamespace
      {
        get { return string.Empty; }
      }
      public override string LookupNamespace(string prefix)
      {
        if (prefix.Length == 0)
        {
          return string.Empty;
        }
        string ns = this.nsResolver.LookupNamespace(prefix);
        if (ns == null)
        {
          throw XPathException.Create(Res.XmlUndefinedAlias, prefix);
        }
        return ns;
      }
      //----- XsltContext support -----
      public override IXsltContextVariable ResolveVariable(string prefix, string name)
      {
        throw XPathException.Create(Res.Xp_UndefinedXsltContext);
      }
      public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
      {
        throw XPathException.Create(Res.Xp_UndefinedXsltContext);
      }
      public override bool Whitespace { get { return false; } }
      public override bool PreserveWhitespace(XPathNavigator node) { return false; }
    }
  }
}