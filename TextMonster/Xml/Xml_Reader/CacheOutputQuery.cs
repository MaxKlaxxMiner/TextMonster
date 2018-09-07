using System.Collections.Generic;
using System.Diagnostics;

namespace TextMonster.Xml.Xml_Reader
{
  internal abstract class CacheOutputQuery : Query
  {
    internal Query input;
    // int count; -- we reusing it here
    protected List<XPathNavigator> outputBuffer;

    public CacheOutputQuery(Query input)
    {
      this.input = input;
      this.outputBuffer = new List<XPathNavigator>();
      this.count = 0;
    }
    protected CacheOutputQuery(CacheOutputQuery other)
      : base(other)
    {
      this.input = Clone(other.input);
      this.outputBuffer = new List<XPathNavigator>(other.outputBuffer);
      this.count = other.count;
    }

    public override void Reset()
    {
      this.count = 0;
    }

    public override void SetXsltContext(XsltContext context)
    {
      input.SetXsltContext(context);
    }

    public override object Evaluate(XPathNodeIterator context)
    {
      outputBuffer.Clear();
      count = 0;
      return input.Evaluate(context);// This is trick. IDQuery needs this value. Otherwise we would return this.
      // All subclasses should and would anyway override thismethod and return this.
    }

    public override XPathNavigator Advance()
    {
      Debug.Assert(0 <= count && count <= outputBuffer.Count);
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
        Debug.Assert(0 <= count && count <= outputBuffer.Count);
        if (count == 0)
        {
          return null;
        }
        return outputBuffer[count - 1];
      }
    }

    public override XPathResultType StaticType { get { return XPathResultType.NodeSet; } }
    public override int CurrentPosition { get { return count; } }
    public override int Count { get { return outputBuffer.Count; } }

    public override void PrintQuery(XmlWriter w)
    {
      w.WriteStartElement(this.GetType().Name);
      input.PrintQuery(w);
      w.WriteEndElement();
    }
  }
}
