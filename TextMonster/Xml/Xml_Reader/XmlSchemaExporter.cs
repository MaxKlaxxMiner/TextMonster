using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaExporter.uex' path='docs/doc[@for="XmlSchemaExporter"]/*' />
  ///<internalonly/>
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaExporter
  {
    internal const XmlSchemaForm elementFormDefault = XmlSchemaForm.Qualified;
    internal const XmlSchemaForm attributeFormDefault = XmlSchemaForm.Unqualified;

    XmlSchemas schemas;
    Hashtable elements = new Hashtable();   // ElementAccessor -> XmlSchemaElement
    Hashtable attributes = new Hashtable();   // AttributeAccessor -> XmlSchemaElement
    Hashtable types = new Hashtable();      // StructMapping/EnumMapping -> XmlSchemaComplexType/XmlSchemaSimpleType
    Hashtable references = new Hashtable();   // TypeMappings to keep track of circular references via anonymous types
    bool needToExportRoot;
    TypeScope scope;

    /// <include file='doc\XmlSchemaExporter.uex' path='docs/doc[@for="XmlSchemaExporter.XmlSchemaExporter"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlSchemaExporter(XmlSchemas schemas)
    {
      this.schemas = schemas;
    }

    void CheckForDuplicateType(TypeMapping mapping, string newNamespace)
    {
      if (mapping.IsAnonymousType)
        return;
      string newTypeName = mapping.TypeName;
      XmlSchema schema = schemas[newNamespace];
      if (schema != null)
      {
        foreach (XmlSchemaObject o in schema.Items)
        {
          XmlSchemaType type = o as XmlSchemaType;
          if (type != null && type.Name == newTypeName)
            throw new InvalidOperationException(Res.GetString(Res.XmlDuplicateTypeName, newTypeName, newNamespace));

        }
      }
    }

    XmlSchema AddSchema(string targetNamespace)
    {
      XmlSchema schema = new XmlSchema();
      schema.TargetNamespace = string.IsNullOrEmpty(targetNamespace) ? null : targetNamespace;

#pragma warning disable 429  // Unreachable code:  the default values are constant, so will never be Unqualified
      schema.ElementFormDefault = elementFormDefault == XmlSchemaForm.Unqualified ? XmlSchemaForm.None : elementFormDefault;
      schema.AttributeFormDefault = attributeFormDefault == XmlSchemaForm.Unqualified ? XmlSchemaForm.None : attributeFormDefault;
#pragma warning restore 429
      schemas.Add(schema);

      return schema;
    }

    void AddSchemaItem(XmlSchemaObject item, string ns, string referencingNs)
    {
      XmlSchema schema = schemas[ns];
      if (schema == null)
      {
        schema = AddSchema(ns);
      }

      if (item is XmlSchemaElement)
      {
        XmlSchemaElement e = (XmlSchemaElement)item;
        if (e.Form == XmlSchemaForm.Unqualified)
          throw new InvalidOperationException(Res.GetString(Res.XmlIllegalForm, e.Name));
        e.Form = XmlSchemaForm.None;
      }
      else if (item is XmlSchemaAttribute)
      {
        XmlSchemaAttribute a = (XmlSchemaAttribute)item;
        if (a.Form == XmlSchemaForm.Unqualified)
          throw new InvalidOperationException(Res.GetString(Res.XmlIllegalForm, a.Name));
        a.Form = XmlSchemaForm.None;
      }
      schema.Items.Add(item);
      AddSchemaImport(ns, referencingNs);
    }

    void AddSchemaImport(string ns, string referencingNs)
    {
      if (referencingNs == null) return;
      if (NamespacesEqual(ns, referencingNs)) return;
      XmlSchema schema = schemas[referencingNs];
      if (schema == null)
      {
        schema = AddSchema(referencingNs);
      }
      if (FindImport(schema, ns) == null)
      {
        XmlSchemaImport import = new XmlSchemaImport();
        if (ns != null && ns.Length > 0)
          import.Namespace = ns;
        schema.Includes.Add(import);
      }
    }

    static bool NamespacesEqual(string ns1, string ns2)
    {
      if (ns1 == null || ns1.Length == 0)
        return (ns2 == null || ns2.Length == 0);
      else
        return ns1 == ns2;
    }

    bool SchemaContainsItem(XmlSchemaObject item, string ns)
    {
      XmlSchema schema = schemas[ns];
      if (schema != null)
      {
        return schema.Items.Contains(item);
      }
      return false;
    }

    XmlSchemaImport FindImport(XmlSchema schema, string ns)
    {
      foreach (object item in schema.Includes)
      {
        if (item is XmlSchemaImport)
        {
          XmlSchemaImport import = (XmlSchemaImport)item;
          if (NamespacesEqual(import.Namespace, ns))
          {
            return import;
          }
        }
      }
      return null;
    }

    void ExportElementMapping(XmlSchemaElement element, Mapping mapping, string ns, bool isAny)
    {
      if (mapping is ArrayMapping)
        ExportArrayMapping((ArrayMapping)mapping, ns, element);
      else if (mapping is PrimitiveMapping)
      {
        PrimitiveMapping pm = (PrimitiveMapping)mapping;
        if (pm.IsAnonymousType)
        {
          element.SchemaType = ExportAnonymousPrimitiveMapping(pm);
        }
        else
        {
          element.SchemaTypeName = ExportPrimitiveMapping(pm, ns);
        }
      }
      else if (mapping is StructMapping)
      {
        ExportStructMapping((StructMapping)mapping, ns, element);
      }
      else if (mapping is MembersMapping)
        element.SchemaType = ExportMembersMapping((MembersMapping)mapping, ns);
      else if (mapping is SpecialMapping)
        ExportSpecialMapping((SpecialMapping)mapping, ns, isAny, element);
      else if (mapping is NullableMapping)
      {
        ExportElementMapping(element, ((NullableMapping)mapping).BaseMapping, ns, isAny);
      }
      else
        throw new ArgumentException(Res.GetString(Res.XmlInternalError), "mapping");
    }

    XmlQualifiedName ExportNonXsdPrimitiveMapping(PrimitiveMapping mapping, string ns)
    {
      XmlSchemaSimpleType type = (XmlSchemaSimpleType)mapping.TypeDesc.DataType;
      if (!SchemaContainsItem(type, UrtTypes.Namespace))
      {
        AddSchemaItem(type, UrtTypes.Namespace, ns);
      }
      else
      {
        AddSchemaImport(mapping.Namespace, ns);
      }
      return new XmlQualifiedName(mapping.TypeDesc.DataType.Name, UrtTypes.Namespace);
    }

    XmlSchemaType ExportSpecialMapping(SpecialMapping mapping, string ns, bool isAny, XmlSchemaElement element)
    {
      switch (mapping.TypeDesc.Kind)
      {
        case TypeKind.Node:
        {
          XmlSchemaComplexType type = new XmlSchemaComplexType();
          type.IsMixed = mapping.TypeDesc.IsMixed;
          XmlSchemaSequence seq = new XmlSchemaSequence();
          XmlSchemaAny any = new XmlSchemaAny();
          if (isAny)
          {
            type.AnyAttribute = new XmlSchemaAnyAttribute();
            type.IsMixed = true;
            any.MaxOccurs = decimal.MaxValue;
          }
          seq.Items.Add(any);
          type.Particle = seq;
          if (element != null)
            element.SchemaType = type;
          return type;
        }
        case TypeKind.Serializable:
        {
          SerializableMapping serializableMapping = (SerializableMapping)mapping;
          if (serializableMapping.IsAny)
          {
            XmlSchemaComplexType type = new XmlSchemaComplexType();
            type.IsMixed = mapping.TypeDesc.IsMixed;
            XmlSchemaSequence seq = new XmlSchemaSequence();
            XmlSchemaAny any = new XmlSchemaAny();
            if (isAny)
            {
              type.AnyAttribute = new XmlSchemaAnyAttribute();
              type.IsMixed = true;
              any.MaxOccurs = decimal.MaxValue;
            }
            if (serializableMapping.NamespaceList.Length > 0)
              any.Namespace = serializableMapping.NamespaceList;
            any.ProcessContents = XmlSchemaContentProcessing.Lax;

            if (serializableMapping.Schemas != null)
            {
              foreach (XmlSchema schema in serializableMapping.Schemas.Schemas())
              {
                if (schema.TargetNamespace != XmlSchema.Namespace)
                {
                  schemas.Add(schema, true);
                  AddSchemaImport(schema.TargetNamespace, ns);
                }
              }
            }
            seq.Items.Add(any);
            type.Particle = seq;
            if (element != null)
              element.SchemaType = type;
            return type;
          }
          else if (serializableMapping.XsiType != null || serializableMapping.XsdType != null)
          {
            XmlSchemaType type = serializableMapping.XsdType;
            // for performance reasons we need to postpone merging of the serializable schemas
            foreach (XmlSchema schema in serializableMapping.Schemas.Schemas())
            {
              if (schema.TargetNamespace != XmlSchema.Namespace)
              {
                schemas.Add(schema, true);
                AddSchemaImport(schema.TargetNamespace, ns);
                if (!serializableMapping.XsiType.IsEmpty && serializableMapping.XsiType.Namespace == schema.TargetNamespace)
                  type = (XmlSchemaType)schema.SchemaTypes[serializableMapping.XsiType];
              }
            }
            if (element != null)
            {
              element.SchemaTypeName = serializableMapping.XsiType;
              if (element.SchemaTypeName.IsEmpty)
                element.SchemaType = type;
            }
            // check for duplicate top-level elements XmlAttributes
            serializableMapping.CheckDuplicateElement(element, ns);
            return type;
          }
          else if (serializableMapping.Schema != null)
          {
            // this is the strongly-typed DataSet
            XmlSchemaComplexType type = new XmlSchemaComplexType();
            XmlSchemaAny any = new XmlSchemaAny();
            XmlSchemaSequence seq = new XmlSchemaSequence();
            seq.Items.Add(any);
            type.Particle = seq;
            string anyNs = serializableMapping.Schema.TargetNamespace;
            any.Namespace = anyNs == null ? "" : anyNs;
            XmlSchema existingSchema = schemas[anyNs];
            if (existingSchema == null)
            {
              schemas.Add(serializableMapping.Schema);
            }
            else if (existingSchema != serializableMapping.Schema)
            {
              throw new InvalidOperationException(Res.GetString(Res.XmlDuplicateNamespace, anyNs));
            }
            if (element != null)
              element.SchemaType = type;

            // check for duplicate top-level elements XmlAttributes
            serializableMapping.CheckDuplicateElement(element, ns);
            return type;
          }
          else
          {
            // DataSet
            XmlSchemaComplexType type = new XmlSchemaComplexType();
            XmlSchemaElement schemaElement = new XmlSchemaElement();
            schemaElement.RefName = new XmlQualifiedName("schema", XmlSchema.Namespace);
            XmlSchemaSequence seq = new XmlSchemaSequence();
            seq.Items.Add(schemaElement);
            seq.Items.Add(new XmlSchemaAny());
            type.Particle = seq;
            AddSchemaImport(XmlSchema.Namespace, ns);
            if (element != null)
              element.SchemaType = type;
            return type;
          }
        }
        default:
          throw new ArgumentException(Res.GetString(Res.XmlInternalError), "mapping");
      }
    }

    XmlSchemaType ExportMembersMapping(MembersMapping mapping, string ns)
    {
      XmlSchemaComplexType type = new XmlSchemaComplexType();
      ExportTypeMembers(type, mapping.Members, mapping.TypeName, ns, false, false);

      if (mapping.XmlnsMember != null)
      {
        AddXmlnsAnnotation(type, mapping.XmlnsMember.Name);
      }

      return type;
    }

    XmlSchemaType ExportAnonymousPrimitiveMapping(PrimitiveMapping mapping)
    {
      if (mapping is EnumMapping)
      {
        return ExportEnumMapping((EnumMapping)mapping, null);
      }
      else
      {
        throw new InvalidOperationException(Res.GetString(Res.XmlInternalErrorDetails, "Unsuported anonymous mapping type: " + mapping.ToString()));
      }
    }

    XmlQualifiedName ExportPrimitiveMapping(PrimitiveMapping mapping, string ns)
    {
      XmlQualifiedName qname;
      if (mapping is EnumMapping)
      {
        XmlSchemaType type = ExportEnumMapping((EnumMapping)mapping, ns);
        qname = new XmlQualifiedName(type.Name, mapping.Namespace);
      }
      else
      {
        if (mapping.TypeDesc.IsXsdType)
        {
          qname = new XmlQualifiedName(mapping.TypeDesc.DataType.Name, XmlSchema.Namespace);
        }
        else
        {
          qname = ExportNonXsdPrimitiveMapping(mapping, ns);
        }
      }
      return qname;
    }

    void ExportArrayMapping(ArrayMapping mapping, string ns, XmlSchemaElement element)
    {
      // some of the items in the linked list differ only by CLR type. We don't need to
      // export different schema types for these. Look further down the list for another
      // entry with the same elements. If there is one, it will be exported later so
      // just return its name now.

      ArrayMapping currentMapping = mapping;
      while (currentMapping.Next != null)
      {
        currentMapping = currentMapping.Next;
      }
      XmlSchemaComplexType type = (XmlSchemaComplexType)types[currentMapping];
      if (type == null)
      {
        CheckForDuplicateType(currentMapping, currentMapping.Namespace);
        type = new XmlSchemaComplexType();
        if (!mapping.IsAnonymousType)
        {
          type.Name = mapping.TypeName;
          AddSchemaItem(type, mapping.Namespace, ns);
        }
        if (!currentMapping.IsAnonymousType)
          types.Add(currentMapping, type);
        XmlSchemaSequence seq = new XmlSchemaSequence();
        ExportElementAccessors(seq, mapping.Elements, true, false, mapping.Namespace);
        if (seq.Items.Count > 0)
        {
          if (seq.Items[0] is XmlSchemaChoice)
          {
            type.Particle = (XmlSchemaChoice)seq.Items[0];
          }
          else
          {
            type.Particle = seq;
          }
        }
      }
      else
      {
        AddSchemaImport(mapping.Namespace, ns);
      }
      if (element != null)
      {
        if (mapping.IsAnonymousType)
        {
          element.SchemaType = type;
        }
        else
        {
          element.SchemaTypeName = new XmlQualifiedName(type.Name, mapping.Namespace);
        }
      }
    }

    void ExportElementAccessors(XmlSchemaGroupBase group, ElementAccessor[] accessors, bool repeats, bool valueTypeOptional, string ns)
    {
      if (accessors.Length == 0) return;
      if (accessors.Length == 1)
      {
        ExportElementAccessor(group, accessors[0], repeats, valueTypeOptional, ns);
      }
      else
      {
        XmlSchemaChoice choice = new XmlSchemaChoice();
        choice.MaxOccurs = repeats ? decimal.MaxValue : 1;
        choice.MinOccurs = repeats ? 0 : 1;
        for (int i = 0; i < accessors.Length; i++)
          ExportElementAccessor(choice, accessors[i], false, valueTypeOptional, ns);

        if (choice.Items.Count > 0) group.Items.Add(choice);
      }
    }

    void ExportAttributeAccessor(XmlSchemaComplexType type, AttributeAccessor accessor, bool valueTypeOptional, string ns)
    {
      if (accessor == null) return;
      XmlSchemaObjectCollection attributes;

      if (type.ContentModel != null)
      {
        if (type.ContentModel.Content is XmlSchemaComplexContentRestriction)
          attributes = ((XmlSchemaComplexContentRestriction)type.ContentModel.Content).Attributes;
        else if (type.ContentModel.Content is XmlSchemaComplexContentExtension)
          attributes = ((XmlSchemaComplexContentExtension)type.ContentModel.Content).Attributes;
        else if (type.ContentModel.Content is XmlSchemaSimpleContentExtension)
          attributes = ((XmlSchemaSimpleContentExtension)type.ContentModel.Content).Attributes;
        else
          throw new InvalidOperationException(Res.GetString(Res.XmlInvalidContent, type.ContentModel.Content.GetType().Name));
      }
      else
      {
        attributes = type.Attributes;
      }

      if (accessor.IsSpecialXmlNamespace)
      {

        // add <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
        AddSchemaImport(XmlReservedNs.NsXml, ns);

        // generate <xsd:attribute ref="xml:lang" use="optional" />
        XmlSchemaAttribute attribute = new XmlSchemaAttribute();
        attribute.Use = XmlSchemaUse.Optional;
        attribute.RefName = new XmlQualifiedName(accessor.Name, XmlReservedNs.NsXml);
        attributes.Add(attribute);
      }
      else if (accessor.Any)
      {
        if (type.ContentModel == null)
        {
          type.AnyAttribute = new XmlSchemaAnyAttribute();
        }
        else
        {
          XmlSchemaContent content = type.ContentModel.Content;
          if (content is XmlSchemaComplexContentExtension)
          {
            XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension)content;
            extension.AnyAttribute = new XmlSchemaAnyAttribute();
          }
          else if (content is XmlSchemaComplexContentRestriction)
          {
            XmlSchemaComplexContentRestriction restriction = (XmlSchemaComplexContentRestriction)content;
            restriction.AnyAttribute = new XmlSchemaAnyAttribute();
          }
          else if (type.ContentModel.Content is XmlSchemaSimpleContentExtension)
          {
            XmlSchemaSimpleContentExtension extension = (XmlSchemaSimpleContentExtension)content;
            extension.AnyAttribute = new XmlSchemaAnyAttribute();
          }
        }
      }
      else
      {
        XmlSchemaAttribute attribute = new XmlSchemaAttribute();
        attribute.Use = XmlSchemaUse.None;
        if (!accessor.HasDefault && !valueTypeOptional && accessor.Mapping.TypeDesc.IsValueType)
        {
          attribute.Use = XmlSchemaUse.Required;
        }
        attribute.Name = accessor.Name;
        if (accessor.Namespace == null || accessor.Namespace == ns)
        {
          // determine the form attribute value
          XmlSchema schema = schemas[ns];
          if (schema == null)
            attribute.Form = accessor.Form == attributeFormDefault ? XmlSchemaForm.None : accessor.Form;
          else
          {
            attribute.Form = accessor.Form == schema.AttributeFormDefault ? XmlSchemaForm.None : accessor.Form;
          }
          attributes.Add(attribute);
        }
        else
        {
          // we are going to add the attribute to the top-level items. "use" attribute should not be set
          if (this.attributes[accessor] == null)
          {
            attribute.Use = XmlSchemaUse.None;
            attribute.Form = accessor.Form;
            AddSchemaItem(attribute, accessor.Namespace, ns);
            this.attributes.Add(accessor, accessor);
          }
          XmlSchemaAttribute refAttribute = new XmlSchemaAttribute();
          refAttribute.Use = XmlSchemaUse.None;
          refAttribute.RefName = new XmlQualifiedName(accessor.Name, accessor.Namespace);
          attributes.Add(refAttribute);
          AddSchemaImport(accessor.Namespace, ns);
        }
        if (accessor.Mapping is PrimitiveMapping)
        {
          PrimitiveMapping pm = (PrimitiveMapping)accessor.Mapping;
          if (pm.IsList)
          {
            // create local simple type for the list-like attributes
            XmlSchemaSimpleType dataType = new XmlSchemaSimpleType();
            XmlSchemaSimpleTypeList list = new XmlSchemaSimpleTypeList();
            if (pm.IsAnonymousType)
            {
              list.ItemType = (XmlSchemaSimpleType)ExportAnonymousPrimitiveMapping(pm);
            }
            else
            {
              list.ItemTypeName = ExportPrimitiveMapping(pm, accessor.Namespace == null ? ns : accessor.Namespace);
            }
            dataType.Content = list;
            attribute.SchemaType = dataType;
          }
          else
          {
            if (pm.IsAnonymousType)
            {
              attribute.SchemaType = (XmlSchemaSimpleType)ExportAnonymousPrimitiveMapping(pm);
            }
            else
            {
              attribute.SchemaTypeName = ExportPrimitiveMapping(pm, accessor.Namespace == null ? ns : accessor.Namespace);
            }
          }
        }
        else if (!(accessor.Mapping is SpecialMapping))
          throw new InvalidOperationException(Res.GetString(Res.XmlInternalError));

        if (accessor.HasDefault)
        {
          attribute.DefaultValue = ExportDefaultValue(accessor.Mapping, accessor.Default);
        }
      }
    }

    void ExportElementAccessor(XmlSchemaGroupBase group, ElementAccessor accessor, bool repeats, bool valueTypeOptional, string ns)
    {
      if (accessor.Any && accessor.Name.Length == 0)
      {
        XmlSchemaAny any = new XmlSchemaAny();
        any.MinOccurs = 0;
        any.MaxOccurs = repeats ? decimal.MaxValue : 1;
        if (accessor.Namespace != null && accessor.Namespace.Length > 0 && accessor.Namespace != ns)
          any.Namespace = accessor.Namespace;
        group.Items.Add(any);
      }
      else
      {
        XmlSchemaElement element = (XmlSchemaElement)elements[accessor];
        int minOccurs = repeats || accessor.HasDefault || (!accessor.IsNullable && !accessor.Mapping.TypeDesc.IsValueType) || valueTypeOptional ? 0 : 1;
        decimal maxOccurs = repeats || accessor.IsUnbounded ? decimal.MaxValue : 1;

        if (element == null)
        {
          element = new XmlSchemaElement();
          element.IsNillable = accessor.IsNullable;
          element.Name = accessor.Name;
          if (accessor.HasDefault)
            element.DefaultValue = ExportDefaultValue(accessor.Mapping, accessor.Default);

          if (accessor.IsTopLevelInSchema)
          {
            elements.Add(accessor, element);
            element.Form = accessor.Form;
            AddSchemaItem(element, accessor.Namespace, ns);
          }
          else
          {
            element.MinOccurs = minOccurs;
            element.MaxOccurs = maxOccurs;
            // determine the form attribute value
            XmlSchema schema = schemas[ns];
            if (schema == null)
              element.Form = accessor.Form == elementFormDefault ? XmlSchemaForm.None : accessor.Form;
            else
            {
              element.Form = accessor.Form == schema.ElementFormDefault ? XmlSchemaForm.None : accessor.Form;
            }
          }
          ExportElementMapping(element, (TypeMapping)accessor.Mapping, accessor.Namespace, accessor.Any);
        }
        if (accessor.IsTopLevelInSchema)
        {
          XmlSchemaElement refElement = new XmlSchemaElement();
          refElement.RefName = new XmlQualifiedName(accessor.Name, accessor.Namespace);
          refElement.MinOccurs = minOccurs;
          refElement.MaxOccurs = maxOccurs;
          group.Items.Add(refElement);
          AddSchemaImport(accessor.Namespace, ns);
        }
        else
        {
          group.Items.Add(element);
        }
      }
    }

    static internal string ExportDefaultValue(TypeMapping mapping, object value)
    {

      if (!(mapping is PrimitiveMapping))
        // should throw, but it will be a breaking change;
        return null;

      if (value == null || value == DBNull.Value)
        return null;

      if (mapping is EnumMapping)
      {
        EnumMapping em = (EnumMapping)mapping;

        // check the validity of the value
        ConstantMapping[] c = em.Constants;
        if (em.IsFlags)
        {
          string[] names = new string[c.Length];
          long[] ids = new long[c.Length];
          Hashtable values = new Hashtable();
          for (int i = 0; i < c.Length; i++)
          {
            names[i] = c[i].XmlName;
            ids[i] = 1 << i;
            values.Add(c[i].Name, ids[i]);
          }
          long val = XmlCustomFormatter.ToEnum((string)value, values, em.TypeName, false);
          return val != 0 ? XmlCustomFormatter.FromEnum(val, names, ids, mapping.TypeDesc.FullName) : null;
        }
        else
        {
          for (int i = 0; i < c.Length; i++)
          {
            if (c[i].Name == (string)value)
            {
              return c[i].XmlName;
            }
          }
          return null; // unknown value
        }
      }

      PrimitiveMapping pm = (PrimitiveMapping)mapping;

      if (!pm.TypeDesc.HasCustomFormatter)
      {
        if (pm.TypeDesc.FormatterName == "String")
          return (string)value;

        Type formatter = typeof(XmlConvert);
        System.Reflection.MethodInfo format = formatter.GetMethod("ToString", new Type[] { pm.TypeDesc.Type });
        if (format != null)
          return (string)format.Invoke(formatter, new Object[] { value });
      }
      else
      {
        string defaultValue = XmlCustomFormatter.FromDefaultValue(value, pm.TypeDesc.FormatterName);
        if (defaultValue == null)
          throw new InvalidOperationException(Res.GetString(Res.XmlInvalidDefaultValue, value.ToString(), pm.TypeDesc.Name));
        return defaultValue;
      }
      throw new InvalidOperationException(Res.GetString(Res.XmlInvalidDefaultValue, value.ToString(), pm.TypeDesc.Name));
    }

    void ExportRootIfNecessary(TypeScope typeScope)
    {
      if (!needToExportRoot)
        return;
      foreach (TypeMapping mapping in typeScope.TypeMappings)
      {
        if (mapping is StructMapping && mapping.TypeDesc.IsRoot)
        {
          ExportDerivedMappings((StructMapping)mapping);
        }
        else if (mapping is ArrayMapping)
        {
          ExportArrayMapping((ArrayMapping)mapping, mapping.Namespace, null);
        }
        else if (mapping is SerializableMapping)
        {
          ExportSpecialMapping((SerializableMapping)mapping, mapping.Namespace, false, null);
        }
      }
    }

    XmlQualifiedName ExportStructMapping(StructMapping mapping, string ns, XmlSchemaElement element)
    {
      if (mapping.TypeDesc.IsRoot)
      {
        needToExportRoot = true;
        return XmlQualifiedName.Empty;
      }
      if (mapping.IsAnonymousType)
      {
        if (references[mapping] != null)
          throw new InvalidOperationException(Res.GetString(Res.XmlCircularReference2, mapping.TypeDesc.Name, "AnonymousType", "false"));
        references[mapping] = mapping;
      }
      XmlSchemaComplexType type = (XmlSchemaComplexType)types[mapping];
      if (type == null)
      {
        if (!mapping.IncludeInSchema) throw new InvalidOperationException(Res.GetString(Res.XmlCannotIncludeInSchema, mapping.TypeDesc.Name));
        CheckForDuplicateType(mapping, mapping.Namespace);
        type = new XmlSchemaComplexType();
        if (!mapping.IsAnonymousType)
        {
          type.Name = mapping.TypeName;
          AddSchemaItem(type, mapping.Namespace, ns);
          types.Add(mapping, type);
        }
        type.IsAbstract = mapping.TypeDesc.IsAbstract;
        bool openModel = mapping.IsOpenModel;
        if (mapping.BaseMapping != null && mapping.BaseMapping.IncludeInSchema)
        {
          if (mapping.BaseMapping.IsAnonymousType)
          {
            throw new InvalidOperationException(Res.GetString(Res.XmlAnonymousBaseType, mapping.TypeDesc.Name, mapping.BaseMapping.TypeDesc.Name, "AnonymousType", "false"));
          }
          if (mapping.HasSimpleContent)
          {
            XmlSchemaSimpleContent model = new XmlSchemaSimpleContent();
            XmlSchemaSimpleContentExtension extension = new XmlSchemaSimpleContentExtension();
            extension.BaseTypeName = ExportStructMapping(mapping.BaseMapping, mapping.Namespace, null);
            model.Content = extension;
            type.ContentModel = model;
          }
          else
          {
            XmlSchemaComplexContentExtension extension = new XmlSchemaComplexContentExtension();
            extension.BaseTypeName = ExportStructMapping(mapping.BaseMapping, mapping.Namespace, null);
            XmlSchemaComplexContent model = new XmlSchemaComplexContent();
            model.Content = extension;
            model.IsMixed = XmlSchemaImporter.IsMixed((XmlSchemaComplexType)types[mapping.BaseMapping]);
            type.ContentModel = model;
          }
          openModel = false;
        }
        ExportTypeMembers(type, mapping.Members, mapping.TypeName, mapping.Namespace, mapping.HasSimpleContent, openModel);
        ExportDerivedMappings(mapping);
        if (mapping.XmlnsMember != null)
        {
          AddXmlnsAnnotation(type, mapping.XmlnsMember.Name);
        }
      }
      else
      {
        AddSchemaImport(mapping.Namespace, ns);
      }
      if (mapping.IsAnonymousType)
      {
        references[mapping] = null;
        if (element != null)
          element.SchemaType = type;
        return XmlQualifiedName.Empty;
      }
      else
      {
        XmlQualifiedName qname = new XmlQualifiedName(type.Name, mapping.Namespace);
        if (element != null) element.SchemaTypeName = qname;
        return qname;
      }
    }

    void ExportTypeMembers(XmlSchemaComplexType type, MemberMapping[] members, string name, string ns, bool hasSimpleContent, bool openModel)
    {
      XmlSchemaGroupBase seq = new XmlSchemaSequence();
      TypeMapping textMapping = null;

      for (int i = 0; i < members.Length; i++)
      {
        MemberMapping member = members[i];
        if (member.Ignore)
          continue;
        if (member.Text != null)
        {
          if (textMapping != null)
          {
            throw new InvalidOperationException(Res.GetString(Res.XmlIllegalMultipleText, name));
          }
          textMapping = member.Text.Mapping;
        }
        if (member.Elements.Length > 0)
        {
          bool repeats = member.TypeDesc.IsArrayLike &&
                         !(member.Elements.Length == 1 && member.Elements[0].Mapping is ArrayMapping);

          bool valueTypeOptional = member.CheckSpecified != SpecifiedAccessor.None || member.CheckShouldPersist;
          ExportElementAccessors(seq, member.Elements, repeats, valueTypeOptional, ns);
        }
      }

      if (seq.Items.Count > 0)
      {
        if (type.ContentModel != null)
        {
          if (type.ContentModel.Content is XmlSchemaComplexContentRestriction)
            ((XmlSchemaComplexContentRestriction)type.ContentModel.Content).Particle = seq;
          else if (type.ContentModel.Content is XmlSchemaComplexContentExtension)
            ((XmlSchemaComplexContentExtension)type.ContentModel.Content).Particle = seq;
          else
            throw new InvalidOperationException(Res.GetString(Res.XmlInvalidContent, type.ContentModel.Content.GetType().Name));
        }
        else
        {
          type.Particle = seq;
        }
      }
      if (textMapping != null)
      {
        if (hasSimpleContent)
        {
          if (textMapping is PrimitiveMapping && seq.Items.Count == 0)
          {
            PrimitiveMapping pm = (PrimitiveMapping)textMapping;
            if (pm.IsList)
            {
              type.IsMixed = true;
            }
            else
            {
              if (pm.IsAnonymousType)
              {
                throw new InvalidOperationException(Res.GetString(Res.XmlAnonymousBaseType, textMapping.TypeDesc.Name, pm.TypeDesc.Name, "AnonymousType", "false"));
              }
              // Create simpleContent
              XmlSchemaSimpleContent model = new XmlSchemaSimpleContent();
              XmlSchemaSimpleContentExtension ex = new XmlSchemaSimpleContentExtension();
              model.Content = ex;
              type.ContentModel = model;
              ex.BaseTypeName = ExportPrimitiveMapping(pm, ns);
            }
          }
        }
        else
        {
          type.IsMixed = true;
        }
      }
      bool anyAttribute = false;
      for (int i = 0; i < members.Length; i++)
      {
        AttributeAccessor accessor = members[i].Attribute;
        if (accessor != null)
        {
          ExportAttributeAccessor(type, members[i].Attribute, members[i].CheckSpecified != SpecifiedAccessor.None || members[i].CheckShouldPersist, ns);
          if (members[i].Attribute.Any)
            anyAttribute = true;
        }
      }
      if (openModel && !anyAttribute)
      {
        AttributeAccessor any = new AttributeAccessor();
        any.Any = true;
        ExportAttributeAccessor(type, any, false, ns);
      }
    }

    void ExportDerivedMappings(StructMapping mapping)
    {
      if (mapping.IsAnonymousType)
        return;
      for (StructMapping derived = mapping.DerivedMappings; derived != null; derived = derived.NextDerivedMapping)
      {
        if (derived.IncludeInSchema) ExportStructMapping(derived, derived.Namespace, null);
      }
    }

    XmlSchemaType ExportEnumMapping(EnumMapping mapping, string ns)
    {
      if (!mapping.IncludeInSchema) throw new InvalidOperationException(Res.GetString(Res.XmlCannotIncludeInSchema, mapping.TypeDesc.Name));
      XmlSchemaSimpleType dataType = (XmlSchemaSimpleType)types[mapping];
      if (dataType == null)
      {
        CheckForDuplicateType(mapping, mapping.Namespace);
        dataType = new XmlSchemaSimpleType();
        dataType.Name = mapping.TypeName;
        if (!mapping.IsAnonymousType)
        {
          types.Add(mapping, dataType);
          AddSchemaItem(dataType, mapping.Namespace, ns);
        }
        XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
        restriction.BaseTypeName = new XmlQualifiedName("string", XmlSchema.Namespace);

        for (int i = 0; i < mapping.Constants.Length; i++)
        {
          ConstantMapping constant = mapping.Constants[i];
          XmlSchemaEnumerationFacet enumeration = new XmlSchemaEnumerationFacet();
          enumeration.Value = constant.XmlName;
          restriction.Facets.Add(enumeration);
        }
        if (!mapping.IsFlags)
        {
          dataType.Content = restriction;
        }
        else
        {
          XmlSchemaSimpleTypeList list = new XmlSchemaSimpleTypeList();
          XmlSchemaSimpleType enumType = new XmlSchemaSimpleType();
          enumType.Content = restriction;
          list.ItemType = enumType;
          dataType.Content = list;
        }
      }
      if (!mapping.IsAnonymousType)
      {
        AddSchemaImport(mapping.Namespace, ns);
      }
      return dataType;
    }

    void AddXmlnsAnnotation(XmlSchemaComplexType type, string xmlnsMemberName)
    {
      XmlSchemaAnnotation annotation = new XmlSchemaAnnotation();
      XmlSchemaAppInfo appinfo = new XmlSchemaAppInfo();

      XmlDocument d = new XmlDocument();
      XmlElement e = d.CreateElement("keepNamespaceDeclarations");
      if (xmlnsMemberName != null)
        e.InsertBefore(d.CreateTextNode(xmlnsMemberName), null);
      appinfo.Markup = new XmlNode[] { e };
      annotation.Items.Add(appinfo);
      type.Annotation = annotation;
    }
  }
}