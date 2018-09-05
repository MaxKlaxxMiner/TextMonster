using System;

namespace TextMonster.Xml.Xml_Reader
{
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false)]
  public class XmlArrayAttribute : System.Attribute
  {
    string elementName;
    string ns;
    bool nullable;
    XmlSchemaForm form = XmlSchemaForm.None;
    int order = -1;

    public string ElementName
    {
      get { return elementName == null ? string.Empty : elementName; }
      set { elementName = value; }
    }

    public string Namespace
    {
      get { return ns; }
      set { ns = value; }
    }

    public bool IsNullable
    {
      get { return nullable; }
    }

    public XmlSchemaForm Form
    {
      get { return form; }
      set { form = value; }
    }

    public int Order
    {
      get { return order; }
    }
  }
}