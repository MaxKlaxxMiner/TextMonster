using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace TextMonster.Xml.Xml_Reader
{
  internal class SerializableMapping : SpecialMapping
  {
    XmlSchema schema;
    Type type;
    bool needSchema = true;

    // new implementation of the IXmlSerializable
    MethodInfo getSchemaMethod;
    XmlQualifiedName xsiType;
    XmlSchemaType xsdType;
    XmlSchemaSet schemas;
    bool any;
    string namespaces;

    SerializableMapping baseMapping;
    SerializableMapping derivedMappings;
    SerializableMapping nextDerivedMapping;
    SerializableMapping next; // all mappings with the same qname

    internal SerializableMapping() { }
    internal SerializableMapping(MethodInfo getSchemaMethod, bool any, string ns)
    {
      this.getSchemaMethod = getSchemaMethod;
      this.any = any;
      this.Namespace = ns;
      needSchema = getSchemaMethod != null;
    }

    internal SerializableMapping(XmlQualifiedName xsiType, XmlSchemaSet schemas)
    {
      this.xsiType = xsiType;
      this.schemas = schemas;
      this.TypeName = xsiType.Name;
      this.Namespace = xsiType.Namespace;
      needSchema = false;
    }

    internal void SetBaseMapping(SerializableMapping mapping)
    {
      baseMapping = mapping;
      if (baseMapping != null)
      {
        nextDerivedMapping = baseMapping.derivedMappings;
        baseMapping.derivedMappings = this;
        if (this == nextDerivedMapping)
        {
          throw new InvalidOperationException(Res.GetString(Res.XmlCircularDerivation, TypeDesc.FullName));
        }
      }
    }

    internal bool IsAny
    {
      get
      {
        if (any)
          return true;
        if (getSchemaMethod == null)
          return false;
        if (needSchema && typeof(XmlSchemaType).IsAssignableFrom(getSchemaMethod.ReturnType))
          return false;
        RetrieveSerializableSchema();
        return any;
      }
    }

    internal string NamespaceList
    {
      get
      {
        RetrieveSerializableSchema();
        if (namespaces == null)
        {
          if (schemas != null)
          {
            StringBuilder anyNamespaces = new StringBuilder();
            foreach (XmlSchema s in schemas.Schemas())
            {
              if (s.TargetNamespace != null && s.TargetNamespace.Length > 0)
              {
                if (anyNamespaces.Length > 0)
                  anyNamespaces.Append(" ");
                anyNamespaces.Append(s.TargetNamespace);
              }
            }
            namespaces = anyNamespaces.ToString();
          }
          else
          {
            namespaces = string.Empty;
          }
        }
        return namespaces;
      }
    }

    internal SerializableMapping DerivedMappings
    {
      get
      {
        return derivedMappings;
      }
    }

    internal SerializableMapping NextDerivedMapping
    {
      get
      {
        return nextDerivedMapping;
      }
    }

    internal SerializableMapping Next
    {
      get { return next; }
      set { next = value; }
    }

    internal Type Type
    {
      get { return type; }
      set { type = value; }
    }

    internal XmlSchemaSet Schemas
    {
      get
      {
        RetrieveSerializableSchema();
        return schemas;
      }
    }

    internal XmlSchema Schema
    {
      get
      {
        RetrieveSerializableSchema();
        return schema;
      }
    }

    internal XmlQualifiedName XsiType
    {
      get
      {
        if (!needSchema)
          return xsiType;
        if (getSchemaMethod == null)
          return null;
        if (typeof(XmlSchemaType).IsAssignableFrom(getSchemaMethod.ReturnType))
          return null;
        RetrieveSerializableSchema();
        return xsiType;
      }
    }

    internal XmlSchemaType XsdType
    {
      get
      {
        RetrieveSerializableSchema();
        return xsdType;
      }
    }

    internal static void ValidationCallbackWithErrorCode(object sender, ValidationEventArgs args)
    {
      // 
      if (args.Severity == XmlSeverityType.Error)
        throw new InvalidOperationException(Res.GetString(Res.XmlSerializableSchemaError, typeof(IXmlSerializable).Name, args.Message));
    }

    internal void CheckDuplicateElement(XmlSchemaElement element, string elementNs)
    {
      if (element == null)
        return;

      // only check duplicate definitions for top-level element
      if (element.Parent == null || !(element.Parent is XmlSchema))
        return;

      XmlSchemaObjectTable elements = null;
      if (Schema != null && Schema.TargetNamespace == elementNs)
      {
        XmlSchemas.Preprocess(Schema);
        elements = Schema.Elements;
      }
      else if (Schemas != null)
      {
        elements = Schemas.GlobalElements;
      }
      else
      {
        return;
      }
      foreach (XmlSchemaElement e in elements.Values)
      {
        if (e.Name == element.Name && e.QualifiedName.Namespace == elementNs)
        {
          if (Match(e, element))
            return;
          // XmlSerializableRootDupName=Cannot reconcile schema for '{0}'. Please use [XmlRoot] attribute to change name or namepace of the top-level element to avoid duplicate element declarations: element name='{1} namespace='{2}'.
          throw new InvalidOperationException(Res.GetString(Res.XmlSerializableRootDupName, getSchemaMethod.DeclaringType.FullName, e.Name, elementNs));
        }
      }
    }

    bool Match(XmlSchemaElement e1, XmlSchemaElement e2)
    {
      if (e1.IsNillable != e2.IsNillable)
        return false;
      if (e1.RefName != e2.RefName)
        return false;
      if (e1.SchemaType != e2.SchemaType)
        return false;
      if (e1.SchemaTypeName != e2.SchemaTypeName)
        return false;
      if (e1.MinOccurs != e2.MinOccurs)
        return false;
      if (e1.MaxOccurs != e2.MaxOccurs)
        return false;
      if (e1.IsAbstract != e2.IsAbstract)
        return false;
      if (e1.DefaultValue != e2.DefaultValue)
        return false;
      if (e1.SubstitutionGroup != e2.SubstitutionGroup)
        return false;
      return true;
    }

    void RetrieveSerializableSchema()
    {
      if (needSchema)
      {
        needSchema = false;
        if (getSchemaMethod != null)
        {
          // get the type info
          if (schemas == null)
            schemas = new XmlSchemaSet();
          object typeInfo = getSchemaMethod.Invoke(null, new object[] { schemas });
          xsiType = XmlQualifiedName.Empty;

          if (typeInfo != null)
          {
            if (typeof(XmlSchemaType).IsAssignableFrom(getSchemaMethod.ReturnType))
            {
              xsdType = (XmlSchemaType)typeInfo;
              // check if type is named
              xsiType = xsdType.QualifiedName;
            }
            else if (typeof(XmlQualifiedName).IsAssignableFrom(getSchemaMethod.ReturnType))
            {
              xsiType = (XmlQualifiedName)typeInfo;
              if (xsiType.IsEmpty)
              {
                throw new InvalidOperationException(Res.GetString(Res.XmlGetSchemaEmptyTypeName, type.FullName, getSchemaMethod.Name));
              }
            }
            else
            {
              throw new InvalidOperationException(Res.GetString(Res.XmlGetSchemaMethodReturnType, type.Name, getSchemaMethod.Name, typeof(XmlSchemaProviderAttribute).Name, typeof(XmlQualifiedName).FullName));
            }
          }
          else
          {
            any = true;
          }

          // make sure that user-specified schemas are valid
          schemas.ValidationEventHandler += new ValidationEventHandler(ValidationCallbackWithErrorCode);
          schemas.Compile();
          // at this point we verified that the information returned by the IXmlSerializable is valid
          // Now check to see if the type was referenced before:
          // 
          if (!xsiType.IsEmpty)
          {
            // try to find the type in the schemas collection
            if (xsiType.Namespace != XmlSchema.Namespace)
            {
              ArrayList srcSchemas = (ArrayList)schemas.Schemas(xsiType.Namespace);

              if (srcSchemas.Count == 0)
              {
                throw new InvalidOperationException(Res.GetString(Res.XmlMissingSchema, xsiType.Namespace));
              }
              if (srcSchemas.Count > 1)
              {
                throw new InvalidOperationException(Res.GetString(Res.XmlGetSchemaInclude, xsiType.Namespace, getSchemaMethod.DeclaringType.FullName, getSchemaMethod.Name));
              }
              XmlSchema s = (XmlSchema)srcSchemas[0];
              if (s == null)
              {
                throw new InvalidOperationException(Res.GetString(Res.XmlMissingSchema, xsiType.Namespace));
              }
              xsdType = (XmlSchemaType)s.SchemaTypes[xsiType];
              if (xsdType == null)
              {
                throw new InvalidOperationException(Res.GetString(Res.XmlGetSchemaTypeMissing, getSchemaMethod.DeclaringType.FullName, getSchemaMethod.Name, xsiType.Name, xsiType.Namespace));
              }
              xsdType = xsdType.Redefined != null ? xsdType.Redefined : xsdType;
            }
          }
        }
        else
        {
          IXmlSerializable serializable = (IXmlSerializable)Activator.CreateInstance(type);
          schema = serializable.GetSchema();

          if (schema != null)
          {
            if (schema.Id == null || schema.Id.Length == 0) throw new InvalidOperationException(Res.GetString(Res.XmlSerializableNameMissing1, type.FullName));
          }
        }
      }
    }
  }
}