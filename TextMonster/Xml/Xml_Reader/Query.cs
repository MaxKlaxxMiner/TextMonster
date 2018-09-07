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

    public abstract XPathNavigator Advance();

    protected static XPathNodeIterator Clone(XPathNodeIterator input)
    {
      if (input != null)
      {
        return input.Clone();
      }
      return null;
    }
  }
}
