using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class XmlSchemaObjectComparer : IComparer
  {
    QNameComparer comparer = new QNameComparer();
    public int Compare(object o1, object o2)
    {
      return comparer.Compare(NameOf((XmlSchemaObject)o1), NameOf((XmlSchemaObject)o2));
    }

    internal static XmlQualifiedName NameOf(XmlSchemaObject o)
    {
      if (o is XmlSchemaAttribute)
      {
        return ((XmlSchemaAttribute)o).QualifiedName;
      }
      else if (o is XmlSchemaAttributeGroup)
      {
        return ((XmlSchemaAttributeGroup)o).QualifiedName;
      }
      else if (o is XmlSchemaComplexType)
      {
        return ((XmlSchemaComplexType)o).QualifiedName;
      }
      else if (o is XmlSchemaSimpleType)
      {
        return ((XmlSchemaSimpleType)o).QualifiedName;
      }
      else if (o is XmlSchemaElement)
      {
        return ((XmlSchemaElement)o).QualifiedName;
      }
      else if (o is XmlSchemaGroup)
      {
        return ((XmlSchemaGroup)o).QualifiedName;
      }
      else if (o is XmlSchemaGroupRef)
      {
        return ((XmlSchemaGroupRef)o).RefName;
      }
      else if (o is XmlSchemaNotation)
      {
        return ((XmlSchemaNotation)o).QualifiedName;
      }
      else if (o is XmlSchemaSequence)
      {
        XmlSchemaSequence s = (XmlSchemaSequence)o;
        if (s.Items.Count == 0)
          return new XmlQualifiedName(".sequence", Namespace(o));
        return NameOf(s.Items[0]);
      }
      else if (o is XmlSchemaAll)
      {
        XmlSchemaAll a = (XmlSchemaAll)o;
        if (a.Items.Count == 0)
          return new XmlQualifiedName(".all", Namespace(o));
        return NameOf(a.Items);
      }
      else if (o is XmlSchemaChoice)
      {
        XmlSchemaChoice c = (XmlSchemaChoice)o;
        if (c.Items.Count == 0)
          return new XmlQualifiedName(".choice", Namespace(o));
        return NameOf(c.Items);
      }
      else if (o is XmlSchemaAny)
      {
        return new XmlQualifiedName("*", SchemaObjectWriter.ToString(((XmlSchemaAny)o).NamespaceList));
      }
      else if (o is XmlSchemaIdentityConstraint)
      {
        return ((XmlSchemaIdentityConstraint)o).QualifiedName;
      }
      return new XmlQualifiedName("?", Namespace(o));
    }

    internal static XmlQualifiedName NameOf(XmlSchemaObjectCollection items)
    {
      ArrayList list = new ArrayList();

      for (int i = 0; i < items.Count; i++)
      {
        list.Add(NameOf(items[i]));
      }
      list.Sort(new QNameComparer());
      return (XmlQualifiedName)list[0];
    }

    internal static string Namespace(XmlSchemaObject o)
    {
      while (o != null && !(o is XmlSchema))
      {
        o = o.Parent;
      }
      return o == null ? "" : ((XmlSchema)o).TargetNamespace;
    }
  }
}