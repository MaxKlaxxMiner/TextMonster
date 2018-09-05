using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  public abstract class XPathExpression
  {
    internal XPathExpression() { }

    public abstract string Expression { get; }

    public abstract void AddSort(object expr, IComparer comparer);

    public abstract void SetContext(XmlNamespaceManager nsManager);

    public abstract void SetContext(IXmlNamespaceResolver nsResolver);

    public static XPathExpression Compile(string xpath)
    {
      return Compile(xpath, /*nsResolver:*/null);
    }

    public static XPathExpression Compile(string xpath, IXmlNamespaceResolver nsResolver)
    {
      bool hasPrefix;
      Query query = new QueryBuilder().Build(xpath, out hasPrefix);
      CompiledXpathExpr expr = new CompiledXpathExpr(query, xpath, hasPrefix);
      if (null != nsResolver)
      {
        expr.SetContext(nsResolver);
      }
      return expr;
    }
  }
}
