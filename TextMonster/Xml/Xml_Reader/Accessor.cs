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

    internal string AnyNamespaces
    {
      get { return anyNs; }
      set { anyNs = value; }
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

    internal static string EscapeName(string name)
    {
      if (name == null || name.Length == 0) return name;
      return XmlConvert.EncodeLocalName(name);
    }

    internal static string EscapeQName(string name)
    {
      if (name == null || name.Length == 0) return name;
      int colon = name.LastIndexOf(':');
      if (colon < 0)
        return XmlConvert.EncodeLocalName(name);
      else
      {
        if (colon == 0 || colon == name.Length - 1)
          throw new ArgumentException(Res.GetString(Res.Xml_InvalidNameChars, name), "name");
        return new XmlQualifiedName(XmlConvert.EncodeLocalName(name.Substring(colon + 1)), XmlConvert.EncodeLocalName(name.Substring(0, colon))).ToString();
      }
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