using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal abstract class Accessor
  {
    string name;
    object defaultValue = null;
    string ns;
    TypeMapping mapping;
    bool any;
    string anyNs;
    bool topLevelInSchema;
    bool isFixed;
    bool isOptional;
    XmlSchemaForm form = XmlSchemaForm.None;

    internal Accessor() { }

    internal TypeMapping Mapping
    {
      get { return mapping; }
      set { mapping = value; }
    }

    internal object Default
    {
      get { return defaultValue; }
      set { defaultValue = value; }
    }

    internal bool HasDefault
    {
      get { return defaultValue != null && defaultValue != DBNull.Value; }
    }

    internal virtual string Name
    {
      get { return name == null ? string.Empty : name; }
      set { name = value; }
    }

    internal bool Any
    {
      get { return any; }
      set { any = value; }
    }

    internal string Namespace
    {
      get { return ns; }
      set { ns = value; }
    }

    internal XmlSchemaForm Form
    {
      get { return form; }
      set { form = value; }
    }

    internal bool IsFixed
    {
      set { isFixed = value; }
    }

    internal bool IsOptional
    {
      set { isOptional = value; }
    }

    internal bool IsTopLevelInSchema
    {
      get { return topLevelInSchema; }
      set { topLevelInSchema = value; }
    }

    internal static string UnescapeName(string name)
    {
      return XmlConvert.DecodeName(name);
    }

    internal string ToString(string defaultNs)
    {
      if (Any)
      {
        return (Namespace == null ? "##any" : Namespace) + ":" + Name;
      }
      else
      {
        return Namespace == defaultNs ? Name : Namespace + ":" + Name;
      }
    }
  }
}