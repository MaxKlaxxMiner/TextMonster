using System;
using System.Reflection;
using System.Threading;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializer"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSerializer
  {
    TempAssembly tempAssembly;
    bool typedSerializer;
    Type primitiveType;
    XmlMapping mapping;

    static TempAssemblyCache cache = new TempAssemblyCache();
    static volatile XmlSerializerNamespaces defaultNamespaces;
    static XmlSerializerNamespaces DefaultNamespaces
    {
      get
      {
        if (defaultNamespaces == null)
        {
          XmlSerializerNamespaces nss = new XmlSerializerNamespaces();
          nss.AddInternal("xsi", XmlSchema.InstanceNamespace);
          nss.AddInternal("xsd", XmlSchema.Namespace);
          if (defaultNamespaces == null)
          {
            defaultNamespaces = nss;
          }
        }
        return defaultNamespaces;
      }
    }

    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializer.XmlSerializer6"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlSerializer(Type type)
      : this(type, (string)null)
    {
    }

    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializer.XmlSerializer1"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlSerializer(Type type, string defaultNamespace)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      this.mapping = GetKnownMapping(type, defaultNamespace);
      if (this.mapping != null)
      {
        this.primitiveType = type;
        return;
      }
      tempAssembly = cache[defaultNamespace, type];
      if (tempAssembly == null)
      {
        lock (cache)
        {
          tempAssembly = cache[defaultNamespace, type];
          if (tempAssembly == null)
          {
            XmlSerializerImplementation contract;
            Assembly assembly = TempAssembly.LoadGeneratedAssembly(type, defaultNamespace, out contract);
            if (assembly == null)
            {
              // need to reflect and generate new serialization assembly
              XmlReflectionImporter importer = new XmlReflectionImporter(defaultNamespace);
              this.mapping = importer.ImportTypeMapping(type, null, defaultNamespace);
              tempAssembly = GenerateTempAssembly(this.mapping, type, defaultNamespace);
            }
            else
            {
              // we found the pre-generated assembly, now make sure that the assembly has the right serializer
              // try to avoid the reflection step, need to get ElementName, namespace and the Key form the type
              this.mapping = XmlReflectionImporter.GetTopLevelMapping(type, defaultNamespace);
              tempAssembly = new TempAssembly(new XmlMapping[] { this.mapping }, assembly, contract);
            }
          }
          cache.Add(defaultNamespace, type, tempAssembly);
        }
      }
      if (mapping == null)
      {
        mapping = XmlReflectionImporter.GetTopLevelMapping(type, defaultNamespace);
      }
    }

    internal static TempAssembly GenerateTempAssembly(XmlMapping xmlMapping, Type type, string defaultNamespace)
    {
      if (xmlMapping == null)
        throw new ArgumentNullException("xmlMapping");
      return new TempAssembly(new XmlMapping[] { xmlMapping }, new Type[] { type }, defaultNamespace, null, null);
    }

    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializer.Serialize5"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces)
    {
      Serialize(xmlWriter, o, namespaces, null);
    }
    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializer.Serialize6"]/*' />
    public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle)
    {
      Serialize(xmlWriter, o, namespaces, encodingStyle, null);
    }

    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializer.Serialize6"]/*' />
    public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle, string id)
    {
      try
      {
        if (primitiveType != null)
        {
          if (encodingStyle != null && encodingStyle.Length > 0)
          {
            throw new InvalidOperationException(Res.GetString(Res.XmlInvalidEncodingNotEncoded1, encodingStyle));
          }
          SerializePrimitive(xmlWriter, o, namespaces);
        }
        else if (tempAssembly == null || typedSerializer)
        {
          XmlSerializationWriter writer = CreateWriter();
          writer.Init(xmlWriter, namespaces == null || namespaces.Count == 0 ? DefaultNamespaces : namespaces, encodingStyle, id, tempAssembly);
          try
          {
            Serialize(o, writer);
          }
          finally
          {
            writer.Dispose();
          }
        }
        else
          tempAssembly.InvokeWriter(mapping, xmlWriter, o, namespaces == null || namespaces.Count == 0 ? DefaultNamespaces : namespaces, encodingStyle, id);
      }
      catch (Exception e)
      {
        if (e is ThreadAbortException || e is StackOverflowException || e is OutOfMemoryException)
        {
          throw;
        }
        if (e is TargetInvocationException)
          e = e.InnerException;
        throw new InvalidOperationException(Res.GetString(Res.XmlGenError), e);
      }
      xmlWriter.Flush();
    }


    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializer.CreateWriter"]/*' />
    ///<internalonly/>
    protected virtual XmlSerializationWriter CreateWriter() { throw new NotImplementedException(); }
    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializer.Serialize7"]/*' />
    ///<internalonly/>
    protected virtual void Serialize(object o, XmlSerializationWriter writer) { throw new NotImplementedException(); }

    static XmlTypeMapping GetKnownMapping(Type type, string ns)
    {
      if (ns != null && ns != string.Empty)
        return null;
      TypeDesc typeDesc = (TypeDesc)TypeScope.PrimtiveTypes[type];
      if (typeDesc == null)
        return null;
      ElementAccessor element = new ElementAccessor();
      element.Name = typeDesc.DataType.Name;
      XmlTypeMapping mapping = new XmlTypeMapping(null, element);
      mapping.SetKeyInternal(XmlMapping.GenerateKey(type, null, null));
      return mapping;
    }

    void SerializePrimitive(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces)
    {
      XmlSerializationPrimitiveWriter writer = new XmlSerializationPrimitiveWriter();
      writer.Init(xmlWriter, namespaces, null, null, null);
      switch (Type.GetTypeCode(primitiveType))
      {
        case TypeCode.String:
        writer.Write_string(o);
        break;
        case TypeCode.Int32:
        writer.Write_int(o);
        break;
        case TypeCode.Boolean:
        writer.Write_boolean(o);
        break;
        case TypeCode.Int16:
        writer.Write_short(o);
        break;
        case TypeCode.Int64:
        writer.Write_long(o);
        break;
        case TypeCode.Single:
        writer.Write_float(o);
        break;
        case TypeCode.Double:
        writer.Write_double(o);
        break;
        case TypeCode.Decimal:
        writer.Write_decimal(o);
        break;
        case TypeCode.DateTime:
        writer.Write_dateTime(o);
        break;
        case TypeCode.Char:
        writer.Write_char(o);
        break;
        case TypeCode.Byte:
        writer.Write_unsignedByte(o);
        break;
        case TypeCode.SByte:
        writer.Write_byte(o);
        break;
        case TypeCode.UInt16:
        writer.Write_unsignedShort(o);
        break;
        case TypeCode.UInt32:
        writer.Write_unsignedInt(o);
        break;
        case TypeCode.UInt64:
        writer.Write_unsignedLong(o);
        break;

        default:
        if (primitiveType == typeof(XmlQualifiedName))
        {
          writer.Write_QName(o);
        }
        else if (primitiveType == typeof(byte[]))
        {
          writer.Write_base64Binary(o);
        }
        else if (primitiveType == typeof(Guid))
        {
          writer.Write_guid(o);
        }
        else if (primitiveType == typeof(TimeSpan))
        {
          writer.Write_TimeSpan(o);
        }
        else
        {
          throw new InvalidOperationException(Res.GetString(Res.XmlUnxpectedType, primitiveType.FullName));
        }
        break;
      }
    }
  }
}
