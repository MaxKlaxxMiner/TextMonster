using System.Collections;
using System.Collections.Generic;

namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class XPathSortComparer : IComparer<SortKey>
  {
    private const int minSize = 3;
    private Query[] expressions;
    private IComparer[] comparers;
    private int numSorts;

    public XPathSortComparer(int size)
    {
      if (size <= 0) size = minSize;
      this.expressions = new Query[size];
      this.comparers = new IComparer[size];
    }
    public XPathSortComparer() : this(minSize) { }

    public void AddSort(Query evalQuery, IComparer comparer)
    {
      // Ajust array sizes if needed.
      if (numSorts == this.expressions.Length)
      {
        Query[] newExpressions = new Query[numSorts * 2];
        IComparer[] newComparers = new IComparer[numSorts * 2];
        for (int i = 0; i < numSorts; i++)
        {
          newExpressions[i] = this.expressions[i];
          newComparers[i] = this.comparers[i];
        }
        this.expressions = newExpressions;
        this.comparers = newComparers;
      }

      // Fixup expression to handle node-set return type:
      if (evalQuery.StaticType == XPathResultType.NodeSet || evalQuery.StaticType == XPathResultType.Any)
      {
        evalQuery = new StringFunctions(Function.FunctionType.FuncString, new Query[] { evalQuery });
      }

      this.expressions[numSorts] = evalQuery;
      this.comparers[numSorts] = comparer;
      numSorts++;
    }

    public int NumSorts { get { return numSorts; } }

    public Query Expression(int i)
    {
      return this.expressions[i];
    }

    int IComparer<SortKey>.Compare(SortKey x, SortKey y)
    {
      int result = 0;
      for (int i = 0; i < x.NumKeys; i++)
      {
        result = this.comparers[i].Compare(x[i], y[i]);
        if (result != 0)
        {
          return result;
        }
      }

      // if after all comparisions, the two sort keys are still equal, preserve the doc order
      return x.OriginalPosition - y.OriginalPosition;
    }

    internal XPathSortComparer Clone()
    {
      XPathSortComparer clone = new XPathSortComparer(this.numSorts);

      for (int i = 0; i < this.numSorts; i++)
      {
        clone.comparers[i] = this.comparers[i];
        clone.expressions[i] = (Query)this.expressions[i].Clone(); // Expressions should be cloned because Query should be cloned
      }
      clone.numSorts = this.numSorts;
      return clone;
    }
  }
}