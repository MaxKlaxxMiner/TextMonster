using System;
using System.Globalization;

namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class OperandQuery : ValueQuery
  {
    internal object val;

    public OperandQuery(object val)
    {
      this.val = val;
    }

    public override object Evaluate(XPathNodeIterator nodeIterator)
    {
      return val;
    }
    public override XPathResultType StaticType { get { return GetXPathType(val); } }
    public override XPathNodeIterator Clone() { return this; }

    public override void PrintQuery(XmlWriter w)
    {
      w.WriteStartElement(this.GetType().Name);
      w.WriteAttributeString("value", Convert.ToString(val, CultureInfo.InvariantCulture));
      w.WriteEndElement();
    }
  }
}