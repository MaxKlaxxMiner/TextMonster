using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class SchemaGraph
  {
    ArrayList empty = new ArrayList();
    XmlSchemas schemas;
    Hashtable scope;
    int items;

    internal SchemaGraph(Hashtable scope, XmlSchemas schemas)
    {
      this.scope = scope;
      schemas.Compile(null, false);
      this.schemas = schemas;
      items = 0;
      foreach (XmlSchema s in schemas)
      {
        items += s.Items.Count;
        foreach (XmlSchemaObject item in s.Items)
        {
          Depends(item);
        }
      }
    }

    internal ArrayList GetItems()
    {
      return new ArrayList(scope.Keys);
    }

    internal void AddRef(ArrayList list, XmlSchemaObject o)
    {
      if (o == null)
        return;
      if (schemas.IsReference(o))
        return;
      if (o.Parent is XmlSchema)
      {
        string ns = ((XmlSchema)o.Parent).TargetNamespace;
        if (ns == XmlSchema.Namespace)
          return;
        if (list.Contains(o))
          return;
        list.Add(o);
      }
    }

    internal ArrayList Depends(XmlSchemaObject item)
    {

      if (item.Parent is XmlSchema)
      {
        if (scope[item] != null)
          return (ArrayList)scope[item];

        ArrayList refs = new ArrayList();
        Depends(item, refs);
        scope.Add(item, refs);
        return refs;
      }
      return empty;
    }

    internal void Depends(XmlSchemaObject item, ArrayList refs)
    {
      if (item == null || scope[item] != null)
        return;

      Type t = item.GetType();
      if (typeof(XmlSchemaType).IsAssignableFrom(t))
      {
        XmlQualifiedName baseName = XmlQualifiedName.Empty;
        XmlSchemaType baseType = null;
        XmlSchemaParticle particle = null;
        XmlSchemaObjectCollection attributes = null;

        if (item is XmlSchemaComplexType)
        {
          XmlSchemaComplexType ct = (XmlSchemaComplexType)item;
          if (ct.ContentModel != null)
          {
            XmlSchemaContent content = ct.ContentModel.Content;
            if (content is XmlSchemaComplexContentRestriction)
            {
              baseName = ((XmlSchemaComplexContentRestriction)content).BaseTypeName;
              attributes = ((XmlSchemaComplexContentRestriction)content).Attributes;
            }
            else if (content is XmlSchemaSimpleContentRestriction)
            {
              XmlSchemaSimpleContentRestriction restriction = (XmlSchemaSimpleContentRestriction)content;
              if (restriction.BaseType != null)
                baseType = restriction.BaseType;
              else
                baseName = restriction.BaseTypeName;
              attributes = restriction.Attributes;
            }
            else if (content is XmlSchemaComplexContentExtension)
            {
              XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension)content;
              attributes = extension.Attributes;
              particle = extension.Particle;
              baseName = extension.BaseTypeName;
            }
            else if (content is XmlSchemaSimpleContentExtension)
            {
              XmlSchemaSimpleContentExtension extension = (XmlSchemaSimpleContentExtension)content;
              attributes = extension.Attributes;
              baseName = extension.BaseTypeName;
            }
          }
          else
          {
            attributes = ct.Attributes;
            particle = ct.Particle;
          }
          if (particle is XmlSchemaGroupRef)
          {
            XmlSchemaGroupRef refGroup = (XmlSchemaGroupRef)particle;
            particle = ((XmlSchemaGroup)schemas.Find(refGroup.RefName, typeof(XmlSchemaGroup), false)).Particle;
          }
          else if (particle is XmlSchemaGroupBase)
          {
            particle = (XmlSchemaGroupBase)particle;
          }
        }
        else if (item is XmlSchemaSimpleType)
        {
          XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType)item;
          XmlSchemaSimpleTypeContent content = simpleType.Content;
          if (content is XmlSchemaSimpleTypeRestriction)
          {
            baseType = ((XmlSchemaSimpleTypeRestriction)content).BaseType;
            baseName = ((XmlSchemaSimpleTypeRestriction)content).BaseTypeName;
          }
          else if (content is XmlSchemaSimpleTypeList)
          {
            XmlSchemaSimpleTypeList list = (XmlSchemaSimpleTypeList)content;
            if (list.ItemTypeName != null && !list.ItemTypeName.IsEmpty)
              baseName = list.ItemTypeName;
            if (list.ItemType != null)
            {
              baseType = list.ItemType;
            }
          }
          else if (content is XmlSchemaSimpleTypeRestriction)
          {
            baseName = ((XmlSchemaSimpleTypeRestriction)content).BaseTypeName;
          }
          else if (t == typeof(XmlSchemaSimpleTypeUnion))
          {
            XmlQualifiedName[] memberTypes = ((XmlSchemaSimpleTypeUnion)item).MemberTypes;

            if (memberTypes != null)
            {
              for (int i = 0; i < memberTypes.Length; i++)
              {
                XmlSchemaType type = (XmlSchemaType)schemas.Find(memberTypes[i], typeof(XmlSchemaType), false);
                AddRef(refs, type);
              }
            }
          }
        }
        if (baseType == null && !baseName.IsEmpty && baseName.Namespace != XmlSchema.Namespace)
          baseType = (XmlSchemaType)schemas.Find(baseName, typeof(XmlSchemaType), false);

        if (baseType != null)
        {
          AddRef(refs, baseType);
        }
        if (particle != null)
        {
          Depends(particle, refs);
        }
        if (attributes != null)
        {
          for (int i = 0; i < attributes.Count; i++)
          {
            Depends(attributes[i], refs);
          }
        }
      }
      else if (t == typeof(XmlSchemaElement))
      {
        XmlSchemaElement el = (XmlSchemaElement)item;
        if (!el.SubstitutionGroup.IsEmpty)
        {
          if (el.SubstitutionGroup.Namespace != XmlSchema.Namespace)
          {
            XmlSchemaElement head = (XmlSchemaElement)schemas.Find(el.SubstitutionGroup, typeof(XmlSchemaElement), false);
            AddRef(refs, head);
          }
        }
        if (!el.RefName.IsEmpty)
        {
          el = (XmlSchemaElement)schemas.Find(el.RefName, typeof(XmlSchemaElement), false);
          AddRef(refs, el);
        }
        else if (!el.SchemaTypeName.IsEmpty)
        {
          XmlSchemaType type = (XmlSchemaType)schemas.Find(el.SchemaTypeName, typeof(XmlSchemaType), false);
          AddRef(refs, type);
        }
        else
        {
          Depends(el.SchemaType, refs);
        }
      }
      else if (t == typeof(XmlSchemaGroup))
      {
        Depends(((XmlSchemaGroup)item).Particle);
      }
      else if (t == typeof(XmlSchemaGroupRef))
      {
        XmlSchemaGroup group = (XmlSchemaGroup)schemas.Find(((XmlSchemaGroupRef)item).RefName, typeof(XmlSchemaGroup), false);
        AddRef(refs, group);
      }
      else if (typeof(XmlSchemaGroupBase).IsAssignableFrom(t))
      {
        foreach (XmlSchemaObject o in ((XmlSchemaGroupBase)item).Items)
        {
          Depends(o, refs);
        }
      }
      else if (t == typeof(XmlSchemaAttributeGroupRef))
      {
        XmlSchemaAttributeGroup group = (XmlSchemaAttributeGroup)schemas.Find(((XmlSchemaAttributeGroupRef)item).RefName, typeof(XmlSchemaAttributeGroup), false);
        AddRef(refs, group);
      }
      else if (t == typeof(XmlSchemaAttributeGroup))
      {
        foreach (XmlSchemaObject o in ((XmlSchemaAttributeGroup)item).Attributes)
        {
          Depends(o, refs);
        }
      }
      else if (t == typeof(XmlSchemaAttribute))
      {
        XmlSchemaAttribute at = (XmlSchemaAttribute)item;
        if (!at.RefName.IsEmpty)
        {
          at = (XmlSchemaAttribute)schemas.Find(at.RefName, typeof(XmlSchemaAttribute), false);
          AddRef(refs, at);
        }
        else if (!at.SchemaTypeName.IsEmpty)
        {
          XmlSchemaType type = (XmlSchemaType)schemas.Find(at.SchemaTypeName, typeof(XmlSchemaType), false);
          AddRef(refs, type);
        }
        else
        {
          Depends(at.SchemaType, refs);
        }
      }
      if (typeof(XmlSchemaAnnotated).IsAssignableFrom(t))
      {
        XmlAttribute[] attrs = (XmlAttribute[])((XmlSchemaAnnotated)item).UnhandledAttributes;

        if (attrs != null)
        {
          for (int i = 0; i < attrs.Length; i++)
          {
            XmlAttribute attribute = attrs[i];
            if (attribute.LocalName == Wsdl.ArrayType && attribute.NamespaceURI == Wsdl.Namespace)
            {
              string dims;
              XmlQualifiedName qname = TypeScope.ParseWsdlArrayType(attribute.Value, out dims, item);
              XmlSchemaType type = (XmlSchemaType)schemas.Find(qname, typeof(XmlSchemaType), false);
              AddRef(refs, type);
            }
          }
        }
      }
    }
  }
}