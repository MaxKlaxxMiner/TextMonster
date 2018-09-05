using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class BooleanFunctions : ValueQuery
  {
    Query arg;
    Function.FunctionType funcType;

    public BooleanFunctions(Function.FunctionType funcType, Query arg)
    {
      this.arg = arg;
      this.funcType = funcType;
    }
    private BooleanFunctions(BooleanFunctions other)
      : base(other)
    {
      this.arg = Clone(other.arg);
      this.funcType = other.funcType;
    }

    public override void SetXsltContext(XsltContext context)
    {
      if (arg != null)
      {
        arg.SetXsltContext(context);
      }
    }

    public override object Evaluate(XPathNodeIterator nodeIterator)
    {
      switch (funcType)
      {
        case Function.FunctionType.FuncBoolean: return toBoolean(nodeIterator);
        case Function.FunctionType.FuncNot: return Not(nodeIterator);
        case Function.FunctionType.FuncTrue: return true;
        case Function.FunctionType.FuncFalse: return false;
        case Function.FunctionType.FuncLang: return Lang(nodeIterator);
      }
      return false;
    }

    internal static bool toBoolean(double number)
    {
      return number != 0 && !double.IsNaN(number);
    }
    internal static bool toBoolean(string str)
    {
      return str.Length > 0;
    }

    internal bool toBoolean(XPathNodeIterator nodeIterator)
    {
      object result = arg.Evaluate(nodeIterator);
      if (result is XPathNodeIterator) return arg.Advance() != null;
      if (result is string) return toBoolean((string)result);
      if (result is double) return toBoolean((double)result);
      if (result is bool) return (bool)result;
      return true;
    }

    public override XPathResultType StaticType { get { return XPathResultType.Boolean; } }

    private bool Not(XPathNodeIterator nodeIterator)
    {
      return !(bool)arg.Evaluate(nodeIterator);
    }

    private bool Lang(XPathNodeIterator nodeIterator)
    {
      string str = arg.Evaluate(nodeIterator).ToString();
      string lang = nodeIterator.Current.XmlLang;
      return (
        lang.StartsWith(str, StringComparison.OrdinalIgnoreCase) &&
        (lang.Length == str.Length || lang[str.Length] == '-')
        );
    }

    public override XPathNodeIterator Clone() { return new BooleanFunctions(this); }

    public override void PrintQuery(XmlWriter w)
    {
      w.WriteStartElement(this.GetType().Name);
      w.WriteAttributeString("name", funcType.ToString());
      if (arg != null)
      {
        arg.PrintQuery(w);
      }
      w.WriteEndElement();
    }
  }
}