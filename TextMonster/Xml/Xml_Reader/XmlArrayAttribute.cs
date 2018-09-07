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
  }
}