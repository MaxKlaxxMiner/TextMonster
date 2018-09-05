using System;
using System.Collections;
using System.Collections.Generic;

namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class SortQuery : Query
  {
    private List<SortKey> results;
    private XPathSortComparer comparer;
    private Query qyInput;

    public SortQuery(Query qyInput)
    {
      this.results = new List<SortKey>();
      this.comparer = new XPathSortComparer();
      this.qyInput = qyInput;
      count = 0;
    }
    private SortQuery(SortQuery other)
      : base(other)
    {
      this.results = new List<SortKey>(other.results);
      this.comparer = other.comparer.Clone();
      this.qyInput = Clone(other.qyInput);
      count = 0;
    }

    public override void Reset() { count = 0; }

    public override void SetXsltContext(XsltContext xsltContext)
    {
      qyInput.SetXsltContext(xsltContext);
      if (
        qyInput.StaticType != XPathResultType.NodeSet &&
        qyInput.StaticType != XPathResultType.Any
        )
      {
        throw XPathException.Create(Res.Xp_NodeSetExpected);
      }
    }

    private void BuildResultsList()
    {
      Int32 numSorts = this.comparer.NumSorts;

      XPathNavigator eNext;
      while ((eNext = qyInput.Advance()) != null)
      {
        SortKey key = new SortKey(numSorts, /*originalPosition:*/this.results.Count, eNext.Clone());

        for (Int32 j = 0; j < numSorts; j++)
        {
          key[j] = this.comparer.Expression(j).Evaluate(qyInput);
        }

        results.Add(key);
      }
      results.Sort(this.comparer);
    }

    public override object Evaluate(XPathNodeIterator context)
    {
      qyInput.Evaluate(context);
      this.results.Clear();
      BuildResultsList();
      count = 0;
      return this;
    }

    public override XPathNavigator Advance()
    {
      if (count < this.results.Count)
      {
        return this.results[count++].Node;
      }
      return null;
    }

    public override XPathNavigator Current
    {
      get
      {
        if (count == 0)
        {
          return null;
        }
        return results[count - 1].Node;
      }
    }

    internal void AddSort(Query evalQuery, IComparer comparer)
    {
      this.comparer.AddSort(evalQuery, comparer);
    }

    public override XPathNodeIterator Clone() { return new SortQuery(this); }

    public override XPathResultType StaticType { get { return XPathResultType.NodeSet; } }
    public override int CurrentPosition { get { return count; } }
    public override int Count { get { return results.Count; } }
    public override QueryProps Properties { get { return QueryProps.Cached | QueryProps.Position | QueryProps.Count; } }

    public override void PrintQuery(XmlWriter w)
    {
      w.WriteStartElement(this.GetType().Name);
      qyInput.PrintQuery(w);
      w.WriteElementString("XPathSortComparer", "... PrintTree() not implemented ...");
      w.WriteEndElement();
    }
  }
}