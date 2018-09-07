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

    public virtual void PrintQuery(XmlWriter w)
    {
      w.WriteElementString(this.GetType().Name, string.Empty);
    }
  }
}
