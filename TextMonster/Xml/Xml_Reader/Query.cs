using System.Diagnostics;

namespace TextMonster.Xml.Xml_Reader
{
  // See comments to QueryBuilder.Props
  // Not all of them are used currently


  // Turn off DebuggerDisplayAttribute. in subclasses of Query.
  // Calls to Current in the XPathNavigator.DebuggerDisplayProxy may change state or throw
  [DebuggerDisplay("{ToString()}")]
  internal abstract class Query : ResetableIterator
  {
    public Query() { }
    protected Query(Query other) : base(other) { }

    // -- XPathNodeIterator --
    //public abstract XPathNodeIterator Clone();
    //public abstract XPathNavigator Current { get; }
    //public abstract int CurrentPosition { get; }
    public override bool MoveNext() { return Advance() != null; }
    public override int Count
    {
      get
      {
        // Query can be ordered in reverse order. So we can't assume like base.Count that last node has greatest position.
        if (count == -1)
        {
          Query clone = (Query)this.Clone();
          clone.Reset();
          count = 0;
          while (clone.MoveNext()) count++;
        }
        return count;
      }
    }

    // ------------- ResetableIterator -----------
    // It's importent that Query is resetable. This fact is used in several plases: 
    // 1. In LogicalExpr: "foo = bar"
    // 2. In SelectionOperator.Reset().
    // In all this cases Reset() means restart iterate through the same nodes. 
    // So reset shouldn't clean bufferes in query. This should be done in set context.
    //public abstract void Reset();

    // -------------------- Query ------------------
    public virtual void SetXsltContext(XsltContext context) { }

    public abstract object Evaluate(XPathNodeIterator nodeIterator);
    public abstract XPathNavigator Advance();

    public virtual XPathNavigator MatchNode(XPathNavigator current)
    {
      throw XPathException.Create(Res.Xp_InvalidPattern);
    }

    public abstract XPathResultType StaticType { get; }

    // ----------------- Helper methods -------------
    public static Query Clone(Query input)
    {
      if (input != null)
      {
        return (Query)input.Clone();
      }
      return null;
    }

    protected static XPathNodeIterator Clone(XPathNodeIterator input)
    {
      if (input != null)
      {
        return input.Clone();
      }
      return null;
    }

    public static XmlNodeOrder CompareNodes(XPathNavigator l, XPathNavigator r)
    {
      XmlNodeOrder cmp = l.ComparePosition(r);
      if (cmp == XmlNodeOrder.Unknown)
      {
        XPathNavigator copy = l.Clone();
        copy.MoveToRoot();
        string baseUriL = copy.BaseURI;
        if (!copy.MoveTo(r))
        {
          copy = r.Clone();
        }
        copy.MoveToRoot();
        string baseUriR = copy.BaseURI;
        int cmpBase = string.CompareOrdinal(baseUriL, baseUriR);
        cmp = (
            cmpBase < 0 ? XmlNodeOrder.Before :
            cmpBase > 0 ? XmlNodeOrder.After :
          /*default*/   XmlNodeOrder.Unknown
        );
      }
      return cmp;
    }

    [Conditional("DEBUG")]
    public static void AssertQuery(Query query)
    {
      Debug.Assert(query != null, "AssertQuery(): query == null");
      if (query is FunctionQuery) return; // Temp Fix. Functions (as document()) return now unordered sequences
      query = Clone(query);
      XPathNavigator last = null;
      XPathNavigator curr;
      int querySize = query.Clone().Count;
      int actualSize = 0;
      while ((curr = query.Advance()) != null)
      {
        if (curr.GetType().ToString() == "Microsoft.VisualStudio.Modeling.StoreNavigator") return;
        if (curr.GetType().ToString() == "DataDocumentXPathNavigator") return;
        Debug.Assert(curr == query.Current, "AssertQuery(): query.Advance() != query.Current");
        if (last != null)
        {
          if (last.NodeType == XPathNodeType.Namespace && curr.NodeType == XPathNodeType.Namespace)
          {
            // NamespaceQuery reports namsespaces in mixed order.
            // Ignore this for now. 
            // It seams that this doesn't breake other queries becasue NS can't have children
          }
          else
          {
            XmlNodeOrder cmp = CompareNodes(last, curr);
            Debug.Assert(cmp == XmlNodeOrder.Before, "AssertQuery(): Wrong node order");
          }
        }
        last = curr.Clone();
        actualSize++;
      }
      Debug.Assert(actualSize == querySize, "AssertQuery(): actualSize != querySize");
    }

    public virtual void PrintQuery(XmlWriter w)
    {
      w.WriteElementString(this.GetType().Name, string.Empty);
    }
  }
}
