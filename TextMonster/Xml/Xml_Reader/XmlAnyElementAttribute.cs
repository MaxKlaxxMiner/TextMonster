using System;

namespace TextMonster.Xml.Xml_Reader
{
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
  public class XmlAnyElementAttribute : System.Attribute
  {
    string name;
    string ns;
    int order = -1;
    bool nsSpecified;

    public string Name
    {
      get { return name == null ? string.Empty : name; }
    }

    public string Namespace
    {
      get { return ns; }
    }

    public int Order
    {
      get { return order; }
    }

    internal bool NamespaceSpecified
    {
      get { return nsSpecified; }
    }
  }
}
