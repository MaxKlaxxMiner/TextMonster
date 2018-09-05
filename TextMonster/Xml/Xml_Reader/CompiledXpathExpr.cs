using System.Collections;

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

    public override void AddSort(object expr, IComparer comparer)
    {
      // sort makes sense only when we are dealing with a query that
      // returns a nodeset.
      Query evalExpr;
      if (expr is string)
      {
        evalExpr = new QueryBuilder().Build((string)expr, out needContext); // this will throw if expr is invalid
      }
      else if (expr is CompiledXpathExpr)
      {
        evalExpr = ((CompiledXpathExpr)expr).QueryTree;
      }
      else
      {
        throw XPathException.Create(Res.Xp_BadQueryObject);
      }
      SortQuery sortQuery = query as SortQuery;
      if (sortQuery == null)
      {
        query = sortQuery = new SortQuery(query);
      }
      sortQuery.AddSort(evalExpr, comparer);
    }

    public virtual void AddSort(object expr, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType)
    {
      AddSort(expr, new XPathComparerHelper(order, caseOrder, lang, dataType));
    }

    public virtual XPathExpression Clone()
    {
      return new CompiledXpathExpr(Query.Clone(query), expr, needContext);
    }

    public virtual void SetContext(XmlNamespaceManager nsManager)
    {
      SetContext((IXmlNamespaceResolver)nsManager);
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

    public virtual XPathResultType ReturnType { get { return query.StaticType; } }

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
      public virtual int CompareDocument(string baseUri, string nextbaseUri)
      {
        return string.CompareOrdinal(baseUri, nextbaseUri);
      }
    }
  }
}