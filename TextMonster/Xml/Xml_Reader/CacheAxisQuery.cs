using System.Collections.Generic;

namespace TextMonster.Xml.Xml_Reader
{
  internal abstract class CacheAxisQuery : BaseAxisQuery
  {
    // int count; -- we reusing it here
    protected List<XPathNavigator> outputBuffer;

    public CacheAxisQuery(Query qyInput, string name, string prefix, XPathNodeType typeTest)
      : base(qyInput, name, prefix, typeTest)
    {
      this.outputBuffer = new List<XPathNavigator>();
      this.count = 0;
    }
    protected CacheAxisQuery(CacheAxisQuery other)
      : base(other)
    {
      this.outputBuffer = new List<XPathNavigator>(other.outputBuffer);
      this.count = other.count;
    }

    public override void Reset()
    {
      this.count = 0;
    }

    public override object Evaluate(XPathNodeIterator context)
    {
      base.Evaluate(context);
      outputBuffer.Clear();
      return this;
    }

    public override XPathNavigator Advance()
    {
      if (count < outputBuffer.Count)
      {
        return outputBuffer[count++];
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
        return outputBuffer[count - 1];
      }
    }

    public override int CurrentPosition { get { return count; } }
    public override int Count { get { return outputBuffer.Count; } }
    public override QueryProps Properties { get { return QueryProps.Merge | QueryProps.Cached | QueryProps.Position | QueryProps.Count; } }
  }
}