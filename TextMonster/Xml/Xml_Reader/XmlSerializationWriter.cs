using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter"]/*' />
  ///<internalonly/>
  public abstract class XmlSerializationWriter : XmlSerializationGeneratedCode
  {
    XmlWriter w;
    XmlSerializerNamespaces namespaces;
    int tempNamespacePrefix;
    Hashtable usedPrefixes;
    Hashtable references;
    string idBase;
    int nextId;
    Hashtable typeEntries;
    ArrayList referencesToWrite;
    Hashtable objectsInUse;
    string aliasBase = "q";
    bool soap12;
    bool escapeName = true;

    // this method must be called before any generated serialization methods are called
    internal void Init(XmlWriter w, XmlSerializerNamespaces namespaces, string encodingStyle, string idBase, TempAssembly tempAssembly)
    {
      this.w = w;
      this.namespaces = namespaces;
      this.soap12 = (encodingStyle == Soap12.Encoding);
      this.idBase = idBase;
      Init(tempAssembly);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.EscapeName"]/*' />
    protected bool EscapeName
    {
      get
      {
        return escapeName;
      }
      set
      {
        escapeName = value;
      }
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.Writer"]/*' />
    protected XmlWriter Writer
    {
      get
      {
        return w;
      }
      set
      {
        w = value;
      }
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.Namespaces"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    protected ArrayList Namespaces
    {
      get
      {
        return namespaces == null ? null : namespaces.NamespaceList;
      }
      set
      {
        if (value == null)
        {
          namespaces = null;
        }
        else
        {
          XmlQualifiedName[] qnames = (XmlQualifiedName[])value.ToArray(typeof(XmlQualifiedName));
          namespaces = new XmlSerializerNamespaces(qnames);
        }
      }
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.FromByteArrayBase64"]/*' />
    protected static byte[] FromByteArrayBase64(byte[] value)
    {
      // Unlike other "From" functions that one is just a place holder for automatic code generation.
      // The reason is performance and memory consumption for (potentially) big 64base-encoded chunks
      // And it is assumed that the caller generates the code that will distinguish between byte[] and string return types
      //
      return value;
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.ResolveDynamicAssembly"]/*' />
    ///<internalonly/>
    protected static Assembly ResolveDynamicAssembly(string assemblyFullName)
    {
      return DynamicAssemblies.Get(assemblyFullName);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.FromByteArrayHex"]/*' />
    protected static string FromByteArrayHex(byte[] value)
    {
      return XmlCustomFormatter.FromByteArrayHex(value);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.FromDateTime"]/*' />
    protected static string FromDateTime(DateTime value)
    {
      return XmlCustomFormatter.FromDateTime(value);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.FromDate"]/*' />
    protected static string FromDate(DateTime value)
    {
      return XmlCustomFormatter.FromDate(value);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.FromTime"]/*' />
    protected static string FromTime(DateTime value)
    {
      return XmlCustomFormatter.FromTime(value);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.FromChar"]/*' />
    protected static string FromChar(char value)
    {
      return XmlCustomFormatter.FromChar(value);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.FromEnum"]/*' />
    protected static string FromEnum(long value, string[] values, long[] ids)
    {
      return XmlCustomFormatter.FromEnum(value, values, ids, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.FromEnum1"]/*' />
    protected static string FromEnum(long value, string[] values, long[] ids, string typeName)
    {
      return XmlCustomFormatter.FromEnum(value, values, ids, typeName);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.FromXmlName"]/*' />
    protected static string FromXmlName(string name)
    {
      return XmlCustomFormatter.FromXmlName(name);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.FromXmlNCName"]/*' />
    protected static string FromXmlNCName(string ncName)
    {
      return XmlCustomFormatter.FromXmlNCName(ncName);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.FromXmlNmToken"]/*' />
    protected static string FromXmlNmToken(string nmToken)
    {
      return XmlCustomFormatter.FromXmlNmToken(nmToken);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.FromXmlNmTokens"]/*' />
    protected static string FromXmlNmTokens(string nmTokens)
    {
      return XmlCustomFormatter.FromXmlNmTokens(nmTokens);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteXsiType"]/*' />
    protected void WriteXsiType(string name, string ns)
    {
      WriteAttribute("type", XmlSchema.InstanceNamespace, GetQualifiedName(name, ns));
    }

    XmlQualifiedName GetPrimitiveTypeName(Type type)
    {
      return GetPrimitiveTypeName(type, true);
    }

    XmlQualifiedName GetPrimitiveTypeName(Type type, bool throwIfUnknown)
    {
      XmlQualifiedName qname = GetPrimitiveTypeNameInternal(type);
      if (throwIfUnknown && qname == null)
        throw CreateUnknownTypeException(type);
      return qname;
    }

    internal static XmlQualifiedName GetPrimitiveTypeNameInternal(Type type)
    {
      string typeName;
      string typeNs = XmlSchema.Namespace;

      switch (Type.GetTypeCode(type))
      {
        case TypeCode.String: typeName = "string"; break;
        case TypeCode.Int32: typeName = "int"; break;
        case TypeCode.Boolean: typeName = "boolean"; break;
        case TypeCode.Int16: typeName = "short"; break;
        case TypeCode.Int64: typeName = "long"; break;
        case TypeCode.Single: typeName = "float"; break;
        case TypeCode.Double: typeName = "double"; break;
        case TypeCode.Decimal: typeName = "decimal"; break;
        case TypeCode.DateTime: typeName = "dateTime"; break;
        case TypeCode.Byte: typeName = "unsignedByte"; break;
        case TypeCode.SByte: typeName = "byte"; break;
        case TypeCode.UInt16: typeName = "unsignedShort"; break;
        case TypeCode.UInt32: typeName = "unsignedInt"; break;
        case TypeCode.UInt64: typeName = "unsignedLong"; break;
        case TypeCode.Char:
        typeName = "char";
        typeNs = UrtTypes.Namespace;
        break;
        default:
        if (type == typeof(XmlQualifiedName)) typeName = "QName";
        else if (type == typeof(byte[])) typeName = "base64Binary";
        else if (type == typeof(Guid))
        {
          typeName = "guid";
          typeNs = UrtTypes.Namespace;
        }
        else if (type == typeof(XmlNode[]))
        {
          typeName = Soap.UrType;
        }
        else
          return null;
        break;
      }
      return new XmlQualifiedName(typeName, typeNs);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteTypedPrimitive"]/*' />
    protected void WriteTypedPrimitive(string name, string ns, object o, bool xsiType)
    {
      string value = null;
      string type;
      string typeNs = XmlSchema.Namespace;
      bool writeRaw = true;
      bool writeDirect = false;
      Type t = o.GetType();
      bool wroteStartElement = false;

      switch (Type.GetTypeCode(t))
      {
        case TypeCode.String:
        value = (string)o;
        type = "string";
        writeRaw = false;
        break;
        case TypeCode.Int32:
        value = XmlConvert.ToString((int)o);
        type = "int";
        break;
        case TypeCode.Boolean:
        value = XmlConvert.ToString((bool)o);
        type = "boolean";
        break;
        case TypeCode.Int16:
        value = XmlConvert.ToString((short)o);
        type = "short";
        break;
        case TypeCode.Int64:
        value = XmlConvert.ToString((long)o);
        type = "long";
        break;
        case TypeCode.Single:
        value = XmlConvert.ToString((float)o);
        type = "float";
        break;
        case TypeCode.Double:
        value = XmlConvert.ToString((double)o);
        type = "double";
        break;
        case TypeCode.Decimal:
        value = XmlConvert.ToString((decimal)o);
        type = "decimal";
        break;
        case TypeCode.DateTime:
        value = FromDateTime((DateTime)o);
        type = "dateTime";
        break;
        case TypeCode.Char:
        value = FromChar((char)o);
        type = "char";
        typeNs = UrtTypes.Namespace;
        break;
        case TypeCode.Byte:
        value = XmlConvert.ToString((byte)o);
        type = "unsignedByte";
        break;
        case TypeCode.SByte:
        value = XmlConvert.ToString((sbyte)o);
        type = "byte";
        break;
        case TypeCode.UInt16:
        value = XmlConvert.ToString((UInt16)o);
        type = "unsignedShort";
        break;
        case TypeCode.UInt32:
        value = XmlConvert.ToString((UInt32)o);
        type = "unsignedInt";
        break;
        case TypeCode.UInt64:
        value = XmlConvert.ToString((UInt64)o);
        type = "unsignedLong";
        break;

        default:
        if (t == typeof(XmlQualifiedName))
        {
          type = "QName";
          // need to write start element ahead of time to establish context
          // for ns definitions by FromXmlQualifiedName
          wroteStartElement = true;
          if (name == null)
            w.WriteStartElement(type, typeNs);
          else
            w.WriteStartElement(name, ns);
          value = FromXmlQualifiedName((XmlQualifiedName)o, false);
        }
        else if (t == typeof(byte[]))
        {
          value = String.Empty;
          writeDirect = true;
          type = "base64Binary";
        }
        else if (t == typeof(Guid))
        {
          value = XmlConvert.ToString((Guid)o);
          type = "guid";
          typeNs = UrtTypes.Namespace;
        }
        else if (typeof(XmlNode[]).IsAssignableFrom(t))
        {
          if (name == null)
            w.WriteStartElement(Soap.UrType, XmlSchema.Namespace);
          else
            w.WriteStartElement(name, ns);

          XmlNode[] xmlNodes = (XmlNode[])o;
          for (int i = 0; i < xmlNodes.Length; i++)
          {
            if (xmlNodes[i] == null)
              continue;
            xmlNodes[i].WriteTo(w);
          }
          w.WriteEndElement();
          return;
        }
        else
          throw CreateUnknownTypeException(t);
        break;
      }
      if (!wroteStartElement)
      {
        if (name == null)
          w.WriteStartElement(type, typeNs);
        else
          w.WriteStartElement(name, ns);
      }

      if (xsiType) WriteXsiType(type, typeNs);

      if (value == null)
      {
        w.WriteAttributeString("nil", XmlSchema.InstanceNamespace, "true");
      }
      else if (writeDirect)
      {
        // only one type currently writes directly to XML stream
        XmlCustomFormatter.WriteArrayBase64(w, (byte[])o, 0, ((byte[])o).Length);
      }
      else if (writeRaw)
      {
        w.WriteRaw(value);
      }
      else
        w.WriteString(value);
      w.WriteEndElement();
    }

    string GetQualifiedName(string name, string ns)
    {
      if (ns == null || ns.Length == 0) return name;
      string prefix = w.LookupPrefix(ns);
      if (prefix == null)
      {
        if (ns == XmlReservedNs.NsXml)
        {
          prefix = "xml";
        }
        else
        {
          prefix = NextPrefix();
          WriteAttribute("xmlns", prefix, null, ns);
        }
      }
      else if (prefix.Length == 0)
      {
        return name;
      }
      return prefix + ":" + name;
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.FromXmlQualifiedName"]/*' />
    protected string FromXmlQualifiedName(XmlQualifiedName xmlQualifiedName)
    {
      return FromXmlQualifiedName(xmlQualifiedName, true);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.FromXmlQualifiedName"]/*' />
    protected string FromXmlQualifiedName(XmlQualifiedName xmlQualifiedName, bool ignoreEmpty)
    {
      if (xmlQualifiedName == null) return null;
      if (xmlQualifiedName.IsEmpty && ignoreEmpty) return null;
      return GetQualifiedName(EscapeName ? XmlConvert.EncodeLocalName(xmlQualifiedName.Name) : xmlQualifiedName.Name, xmlQualifiedName.Namespace);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteStartElement"]/*' />
    protected void WriteStartElement(string name)
    {
      WriteStartElement(name, null, null, false, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteStartElement1"]/*' />
    protected void WriteStartElement(string name, string ns)
    {
      WriteStartElement(name, ns, null, false, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteStartElement4"]/*' />
    protected void WriteStartElement(string name, string ns, bool writePrefixed)
    {
      WriteStartElement(name, ns, null, writePrefixed, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteStartElement2"]/*' />
    protected void WriteStartElement(string name, string ns, object o)
    {
      WriteStartElement(name, ns, o, false, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteStartElement3"]/*' />
    protected void WriteStartElement(string name, string ns, object o, bool writePrefixed)
    {
      WriteStartElement(name, ns, o, writePrefixed, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteStartElement5"]/*' />
    protected void WriteStartElement(string name, string ns, object o, bool writePrefixed, XmlSerializerNamespaces xmlns)
    {
      if (o != null && objectsInUse != null)
      {
        if (objectsInUse.ContainsKey(o)) throw new InvalidOperationException(Res.GetString(Res.XmlCircularReference, o.GetType().FullName));
        objectsInUse.Add(o, o);
      }

      string prefix = null;
      bool needEmptyDefaultNamespace = false;
      if (namespaces != null)
      {
        foreach (string alias in namespaces.Namespaces.Keys)
        {
          string aliasNs = (string)namespaces.Namespaces[alias];

          if (alias.Length > 0 && aliasNs == ns)
            prefix = alias;
          if (alias.Length == 0)
          {
            if (aliasNs == null || aliasNs.Length == 0)
              needEmptyDefaultNamespace = true;
            if (ns != aliasNs)
              writePrefixed = true;
          }
        }
        usedPrefixes = ListUsedPrefixes(namespaces.Namespaces, aliasBase);
      }
      if (writePrefixed && prefix == null && ns != null && ns.Length > 0)
      {
        prefix = w.LookupPrefix(ns);
        if (prefix == null || prefix.Length == 0)
        {
          prefix = NextPrefix();
        }
      }
      if (prefix == null && xmlns != null)
      {
        prefix = xmlns.LookupPrefix(ns);
      }
      if (needEmptyDefaultNamespace && prefix == null && ns != null && ns.Length != 0)
        prefix = NextPrefix();
      w.WriteStartElement(prefix, name, ns);
      if (namespaces != null)
      {
        foreach (string alias in namespaces.Namespaces.Keys)
        {
          string aliasNs = (string)namespaces.Namespaces[alias];
          if (alias.Length == 0 && (aliasNs == null || aliasNs.Length == 0))
            continue;
          if (aliasNs == null || aliasNs.Length == 0)
          {
            if (alias.Length > 0)
              throw new InvalidOperationException(Res.GetString(Res.XmlInvalidXmlns, alias));
            WriteAttribute("xmlns", alias, null, aliasNs);
          }
          else
          {
            if (w.LookupPrefix(aliasNs) == null)
            {
              // write the default namespace declaration only if we have not written it already, over wise we just ignore one provided by the user
              if (prefix == null && alias.Length == 0)
                break;
              WriteAttribute("xmlns", alias, null, aliasNs);
            }
          }
        }
      }
      WriteNamespaceDeclarations(xmlns);
    }

    Hashtable ListUsedPrefixes(Hashtable nsList, string prefix)
    {
      Hashtable qnIndexes = new Hashtable();
      int prefixLength = prefix.Length;
      const string MaxInt32 = "2147483647";
      foreach (string alias in namespaces.Namespaces.Keys)
      {
        string name;
        if (alias.Length > prefixLength)
        {
          name = alias;
          int nameLength = name.Length;
          if (name.Length > prefixLength && name.Length <= prefixLength + MaxInt32.Length && name.StartsWith(prefix, StringComparison.Ordinal))
          {
            bool numeric = true;
            for (int j = prefixLength; j < name.Length; j++)
            {
              if (!Char.IsDigit(name, j))
              {
                numeric = false;
                break;
              }
            }
            if (numeric)
            {
              Int64 index = Int64.Parse(name.Substring(prefixLength), CultureInfo.InvariantCulture);
              if (index <= Int32.MaxValue)
              {
                Int32 newIndex = (Int32)index;
                if (!qnIndexes.ContainsKey(newIndex))
                {
                  qnIndexes.Add(newIndex, newIndex);
                }
              }
            }
          }
        }
      }
      if (qnIndexes.Count > 0)
      {
        return qnIndexes;
      }
      return null;
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteNullTagEncoded"]/*' />
    protected void WriteNullTagEncoded(string name)
    {
      WriteNullTagEncoded(name, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteNullTagEncoded1"]/*' />
    protected void WriteNullTagEncoded(string name, string ns)
    {
      if (name == null || name.Length == 0)
        return;
      WriteStartElement(name, ns, null, true);
      w.WriteAttributeString("nil", XmlSchema.InstanceNamespace, "true");
      w.WriteEndElement();
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteNullTagLiteral"]/*' />
    protected void WriteNullTagLiteral(string name)
    {
      WriteNullTagLiteral(name, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteNullTag1"]/*' />
    protected void WriteNullTagLiteral(string name, string ns)
    {
      if (name == null || name.Length == 0)
        return;
      WriteStartElement(name, ns, null, false);
      w.WriteAttributeString("nil", XmlSchema.InstanceNamespace, "true");
      w.WriteEndElement();
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteEmptyTag"]/*' />
    protected void WriteEmptyTag(string name)
    {
      WriteEmptyTag(name, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteEmptyTag1"]/*' />
    protected void WriteEmptyTag(string name, string ns)
    {
      if (name == null || name.Length == 0)
        return;
      WriteStartElement(name, ns, null, false);
      w.WriteEndElement();
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteEndElement"]/*' />
    protected void WriteEndElement()
    {
      w.WriteEndElement();
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteEndElement1"]/*' />
    protected void WriteEndElement(object o)
    {
      w.WriteEndElement();

      if (o != null && objectsInUse != null)
      {
#if DEBUG
                    // use exception in the place of Debug.Assert to avoid throwing asserts from a server process such as aspnet_ewp.exe
                    if (!objectsInUse.ContainsKey(o)) throw new InvalidOperationException(Res.GetString(Res.XmlInternalErrorDetails, "missing stack object of type " + o.GetType().FullName));
#endif

        objectsInUse.Remove(o);
      }
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteSerializable"]/*' />
    protected void WriteSerializable(IXmlSerializable serializable, string name, string ns, bool isNullable)
    {
      WriteSerializable(serializable, name, ns, isNullable, true);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteSerializable1"]/*' />
    protected void WriteSerializable(IXmlSerializable serializable, string name, string ns, bool isNullable, bool wrapped)
    {
      if (serializable == null)
      {
        if (isNullable) WriteNullTagLiteral(name, ns);
        return;
      }
      if (wrapped)
      {
        w.WriteStartElement(name, ns);
      }
      serializable.WriteXml(w);
      if (wrapped)
      {
        w.WriteEndElement();
      }
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteNullableStringEncoded"]/*' />
    protected void WriteNullableStringEncoded(string name, string ns, string value, XmlQualifiedName xsiType)
    {
      if (value == null)
        WriteNullTagEncoded(name, ns);
      else
        WriteElementString(name, ns, value, xsiType);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteNullableStringLiteral"]/*' />
    protected void WriteNullableStringLiteral(string name, string ns, string value)
    {
      if (value == null)
        WriteNullTagLiteral(name, ns);
      else
        WriteElementString(name, ns, value, null);
    }


    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteNullableStringEncodedRaw"]/*' />
    protected void WriteNullableStringEncodedRaw(string name, string ns, string value, XmlQualifiedName xsiType)
    {
      if (value == null)
        WriteNullTagEncoded(name, ns);
      else
        WriteElementStringRaw(name, ns, value, xsiType);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteNullableStringEncodedRaw1"]/*' />
    protected void WriteNullableStringEncodedRaw(string name, string ns, byte[] value, XmlQualifiedName xsiType)
    {
      if (value == null)
        WriteNullTagEncoded(name, ns);
      else
        WriteElementStringRaw(name, ns, value, xsiType);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteNullableStringLiteralRaw"]/*' />
    protected void WriteNullableStringLiteralRaw(string name, string ns, string value)
    {
      if (value == null)
        WriteNullTagLiteral(name, ns);
      else
        WriteElementStringRaw(name, ns, value, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteNullableStringLiteralRaw1"]/*' />
    protected void WriteNullableStringLiteralRaw(string name, string ns, byte[] value)
    {
      if (value == null)
        WriteNullTagLiteral(name, ns);
      else
        WriteElementStringRaw(name, ns, value, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteNullableQualifiedNameEncoded"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    protected void WriteNullableQualifiedNameEncoded(string name, string ns, XmlQualifiedName value, XmlQualifiedName xsiType)
    {
      if (value == null)
        WriteNullTagEncoded(name, ns);
      else
        WriteElementQualifiedName(name, ns, value, xsiType);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteNullableQualifiedNameLiteral"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    protected void WriteNullableQualifiedNameLiteral(string name, string ns, XmlQualifiedName value)
    {
      if (value == null)
        WriteNullTagLiteral(name, ns);
      else
        WriteElementQualifiedName(name, ns, value, null);
    }


    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementEncoded"]/*' />
    protected void WriteElementEncoded(XmlNode node, string name, string ns, bool isNullable, bool any)
    {
      if (node == null)
      {
        if (isNullable) WriteNullTagEncoded(name, ns);
        return;
      }
      WriteElement(node, name, ns, isNullable, any);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementLiteral"]/*' />
    protected void WriteElementLiteral(XmlNode node, string name, string ns, bool isNullable, bool any)
    {
      if (node == null)
      {
        if (isNullable) WriteNullTagLiteral(name, ns);
        return;
      }
      WriteElement(node, name, ns, isNullable, any);
    }

    private void WriteElement(XmlNode node, string name, string ns, bool isNullable, bool any)
    {
      if (typeof(XmlAttribute).IsAssignableFrom(node.GetType()))
        throw new InvalidOperationException(Res.GetString(Res.XmlNoAttributeHere));
      if (node is XmlDocument)
      {
        node = ((XmlDocument)node).DocumentElement;
        if (node == null)
        {
          if (isNullable) WriteNullTagEncoded(name, ns);
          return;
        }
      }
      if (any)
      {
        if (node is XmlElement && name != null && name.Length > 0)
        {
          // need to check against schema
          if (node.LocalName != name || node.NamespaceURI != ns)
            throw new InvalidOperationException(Res.GetString(Res.XmlElementNameMismatch, node.LocalName, node.NamespaceURI, name, ns));
        }
      }
      else
        w.WriteStartElement(name, ns);

      node.WriteTo(w);

      if (!any)
        w.WriteEndElement();
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.CreateUnknownTypeException"]/*' />
    protected Exception CreateUnknownTypeException(object o)
    {
      return CreateUnknownTypeException(o.GetType());
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.CreateUnknownTypeException1"]/*' />
    protected Exception CreateUnknownTypeException(Type type)
    {
      if (typeof(IXmlSerializable).IsAssignableFrom(type)) return new InvalidOperationException(Res.GetString(Res.XmlInvalidSerializable, type.FullName));
      TypeDesc typeDesc = new TypeScope().GetTypeDesc(type);
      if (!typeDesc.IsStructLike) return new InvalidOperationException(Res.GetString(Res.XmlInvalidUseOfType, type.FullName));
      return new InvalidOperationException(Res.GetString(Res.XmlUnxpectedType, type.FullName));
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.CreateMismatchChoiceException"]/*' />
    protected Exception CreateMismatchChoiceException(string value, string elementName, string enumValue)
    {
      // Value of {0} mismatches the type of {1}, you need to set it to {2}.
      return new InvalidOperationException(Res.GetString(Res.XmlChoiceMismatchChoiceException, elementName, value, enumValue));
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.CreateUnknownAnyElementException"]/*' />
    protected Exception CreateUnknownAnyElementException(string name, string ns)
    {
      return new InvalidOperationException(Res.GetString(Res.XmlUnknownAnyElement, name, ns));
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.CreateInvalidChoiceIdentifierValueException"]/*' />
    protected Exception CreateInvalidChoiceIdentifierValueException(string type, string identifier)
    {
      return new InvalidOperationException(Res.GetString(Res.XmlInvalidChoiceIdentifierValue, type, identifier));
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.CreateChoiceIdentifierValueException"]/*' />
    protected Exception CreateChoiceIdentifierValueException(string value, string identifier, string name, string ns)
    {
      // XmlChoiceIdentifierMismatch=Value '{0}' of the choice identifier '{1}' does not match element '{2}' from namespace '{3}'.
      return new InvalidOperationException(Res.GetString(Res.XmlChoiceIdentifierMismatch, value, identifier, name, ns));
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.CreateInvalidEnumValueException"]/*' />
    protected Exception CreateInvalidEnumValueException(object value, string typeName)
    {
      return new InvalidOperationException(Res.GetString(Res.XmlUnknownConstant, value, typeName));
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.CreateInvalidAnyTypeException"]/*' />
    protected Exception CreateInvalidAnyTypeException(object o)
    {
      return CreateInvalidAnyTypeException(o.GetType());
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.CreateInvalidAnyTypeException1"]/*' />
    protected Exception CreateInvalidAnyTypeException(Type type)
    {
      return new InvalidOperationException(Res.GetString(Res.XmlIllegalAnyElement, type.FullName));
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteReferencingElement"]/*' />
    protected void WriteReferencingElement(string n, string ns, object o)
    {
      WriteReferencingElement(n, ns, o, false);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteReferencingElement1"]/*' />
    protected void WriteReferencingElement(string n, string ns, object o, bool isNullable)
    {
      if (o == null)
      {
        if (isNullable) WriteNullTagEncoded(n, ns);
        return;
      }
      WriteStartElement(n, ns, null, true);
      if (soap12)
        w.WriteAttributeString("ref", Soap12.Encoding, GetId(o, true));
      else
        w.WriteAttributeString("href", "#" + GetId(o, true));
      w.WriteEndElement();
    }

    bool IsIdDefined(object o)
    {
      if (references != null) return references.Contains(o);
      else return false;
    }

    string GetId(object o, bool addToReferencesList)
    {
      if (references == null)
      {
        references = new Hashtable();
        referencesToWrite = new ArrayList();
      }
      string id = (string)references[o];
      if (id == null)
      {
        id = idBase + "id" + (++nextId).ToString(CultureInfo.InvariantCulture);
        references.Add(o, id);
        if (addToReferencesList) referencesToWrite.Add(o);
      }
      return id;
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteId"]/*' />
    protected void WriteId(object o)
    {
      WriteId(o, true);
    }

    void WriteId(object o, bool addToReferencesList)
    {
      if (soap12)
        w.WriteAttributeString("id", Soap12.Encoding, GetId(o, addToReferencesList));
      else
        w.WriteAttributeString("id", GetId(o, addToReferencesList));
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteXmlAttribute1"]/*' />
    protected void WriteXmlAttribute(XmlNode node)
    {
      WriteXmlAttribute(node, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteXmlAttribute2"]/*' />
    protected void WriteXmlAttribute(XmlNode node, object container)
    {
      XmlAttribute attr = node as XmlAttribute;
      if (attr == null) throw new InvalidOperationException(Res.GetString(Res.XmlNeedAttributeHere));
      if (attr.Value != null)
      {
        if (attr.NamespaceURI == Wsdl.Namespace && attr.LocalName == Wsdl.ArrayType)
        {
          string dims;
          XmlQualifiedName qname = TypeScope.ParseWsdlArrayType(attr.Value, out dims, (container is XmlSchemaObject) ? (XmlSchemaObject)container : null);

          string value = FromXmlQualifiedName(qname, true) + dims;

          //<xsd:attribute xmlns:q3="s0" wsdl:arrayType="q3:FoosBase[]" xmlns:q4="http://schemas.xmlsoap.org/soap/encoding/" ref="q4:arrayType" />
          WriteAttribute(Wsdl.ArrayType, Wsdl.Namespace, value);
        }
        else
        {
          WriteAttribute(attr.Name, attr.NamespaceURI, attr.Value);
        }
      }
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteAttribute"]/*' />
    protected void WriteAttribute(string localName, string ns, string value)
    {
      if (value == null) return;
      if (localName == "xmlns" || localName.StartsWith("xmlns:", StringComparison.Ordinal))
      {
        ;
      }
      else
      {
        int colon = localName.IndexOf(':');
        if (colon < 0)
        {
          if (ns == XmlReservedNs.NsXml)
          {
            string prefix = w.LookupPrefix(ns);
            if (prefix == null || prefix.Length == 0)
              prefix = "xml";
            w.WriteAttributeString(prefix, localName, ns, value);
          }
          else
          {
            w.WriteAttributeString(localName, ns, value);
          }
        }
        else
        {
          string prefix = localName.Substring(0, colon);
          w.WriteAttributeString(prefix, localName.Substring(colon + 1), ns, value);
        }
      }
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteAttribute0"]/*' />
    protected void WriteAttribute(string localName, string ns, byte[] value)
    {
      if (value == null) return;
      if (localName == "xmlns" || localName.StartsWith("xmlns:", StringComparison.Ordinal))
      {
        ;
      }
      else
      {
        int colon = localName.IndexOf(':');
        if (colon < 0)
        {
          if (ns == XmlReservedNs.NsXml)
          {
            string prefix = w.LookupPrefix(ns);
            if (prefix == null || prefix.Length == 0)
              prefix = "xml";
            w.WriteStartAttribute("xml", localName, ns);
          }
          else
          {
            w.WriteStartAttribute(null, localName, ns);
          }
        }
        else
        {
          string prefix = localName.Substring(0, colon);
          prefix = w.LookupPrefix(ns);
          w.WriteStartAttribute(prefix, localName.Substring(colon + 1), ns);
        }
        XmlCustomFormatter.WriteArrayBase64(w, value, 0, value.Length);
        w.WriteEndAttribute();
      }
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteAttribute1"]/*' />
    protected void WriteAttribute(string localName, string value)
    {
      if (value == null) return;
      w.WriteAttributeString(localName, null, value);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteAttribute01"]/*' />
    protected void WriteAttribute(string localName, byte[] value)
    {
      if (value == null) return;

      w.WriteStartAttribute(null, localName, (string)null);
      XmlCustomFormatter.WriteArrayBase64(w, value, 0, value.Length);
      w.WriteEndAttribute();
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteAttribute2"]/*' />
    protected void WriteAttribute(string prefix, string localName, string ns, string value)
    {
      if (value == null) return;
      w.WriteAttributeString(prefix, localName, null, value);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteValue"]/*' />
    protected void WriteValue(string value)
    {
      if (value == null) return;
      w.WriteString(value);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteValue01"]/*' />
    protected void WriteValue(byte[] value)
    {
      if (value == null) return;
      XmlCustomFormatter.WriteArrayBase64(w, value, 0, value.Length);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteStartDocument"]/*' />
    protected void WriteStartDocument()
    {
      if (w.WriteState == WriteState.Start)
      {
        w.WriteStartDocument();
      }
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementString"]/*' />
    protected void WriteElementString(String localName, String value)
    {
      WriteElementString(localName, null, value, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementString1"]/*' />
    protected void WriteElementString(String localName, String ns, String value)
    {
      WriteElementString(localName, ns, value, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementString2"]/*' />
    protected void WriteElementString(String localName, String value, XmlQualifiedName xsiType)
    {
      WriteElementString(localName, null, value, xsiType);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementString3"]/*' />
    protected void WriteElementString(String localName, String ns, String value, XmlQualifiedName xsiType)
    {
      if (value == null) return;
      if (xsiType == null)
        w.WriteElementString(localName, ns, value);
      else
      {
        w.WriteStartElement(localName, ns);
        WriteXsiType(xsiType.Name, xsiType.Namespace);
        w.WriteString(value);
        w.WriteEndElement();
      }
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementStringRaw"]/*' />
    protected void WriteElementStringRaw(String localName, String value)
    {
      WriteElementStringRaw(localName, null, value, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementStringRaw0"]/*' />
    protected void WriteElementStringRaw(String localName, byte[] value)
    {
      WriteElementStringRaw(localName, null, value, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementStringRaw1"]/*' />
    protected void WriteElementStringRaw(String localName, String ns, String value)
    {
      WriteElementStringRaw(localName, ns, value, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementStringRaw01"]/*' />
    protected void WriteElementStringRaw(String localName, String ns, byte[] value)
    {
      WriteElementStringRaw(localName, ns, value, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementStringRaw2"]/*' />
    protected void WriteElementStringRaw(String localName, String value, XmlQualifiedName xsiType)
    {
      WriteElementStringRaw(localName, null, value, xsiType);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementStringRaw02"]/*' />
    protected void WriteElementStringRaw(String localName, byte[] value, XmlQualifiedName xsiType)
    {
      WriteElementStringRaw(localName, null, value, xsiType);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementStringRaw3"]/*' />
    protected void WriteElementStringRaw(String localName, String ns, String value, XmlQualifiedName xsiType)
    {
      if (value == null) return;
      w.WriteStartElement(localName, ns);
      if (xsiType != null)
        WriteXsiType(xsiType.Name, xsiType.Namespace);
      w.WriteRaw(value);
      w.WriteEndElement();
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementStringRaw03"]/*' />
    protected void WriteElementStringRaw(String localName, String ns, byte[] value, XmlQualifiedName xsiType)
    {
      if (value == null) return;
      w.WriteStartElement(localName, ns);
      if (xsiType != null)
        WriteXsiType(xsiType.Name, xsiType.Namespace);
      XmlCustomFormatter.WriteArrayBase64(w, value, 0, value.Length);
      w.WriteEndElement();
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteRpcResult"]/*' />
    protected void WriteRpcResult(string name, string ns)
    {
      if (!soap12) return;
      WriteElementQualifiedName(Soap12.RpcResult, Soap12.RpcNamespace, new XmlQualifiedName(name, ns), null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementQualifiedName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    protected void WriteElementQualifiedName(String localName, XmlQualifiedName value)
    {
      WriteElementQualifiedName(localName, null, value, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementQualifiedName1"]/*' />
    protected void WriteElementQualifiedName(string localName, XmlQualifiedName value, XmlQualifiedName xsiType)
    {
      WriteElementQualifiedName(localName, null, value, xsiType);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementQualifiedName2"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    protected void WriteElementQualifiedName(String localName, String ns, XmlQualifiedName value)
    {
      WriteElementQualifiedName(localName, ns, value, null);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteElementQualifiedName3"]/*' />
    protected void WriteElementQualifiedName(string localName, string ns, XmlQualifiedName value, XmlQualifiedName xsiType)
    {
      if (value == null) return;
      if (value.Namespace == null || value.Namespace.Length == 0)
      {
        WriteStartElement(localName, ns, null, true);
        WriteAttribute("xmlns", "");
      }
      else
        w.WriteStartElement(localName, ns);
      if (xsiType != null)
        WriteXsiType(xsiType.Name, xsiType.Namespace);
      w.WriteString(FromXmlQualifiedName(value, false));
      w.WriteEndElement();
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.AddWriteCallback"]/*' />
    protected void AddWriteCallback(Type type, string typeName, string typeNs, XmlSerializationWriteCallback callback)
    {
      TypeEntry entry = new TypeEntry();
      entry.typeName = typeName;
      entry.typeNs = typeNs;
      entry.type = type;
      entry.callback = callback;
      typeEntries[type] = entry;
    }

    void WriteArray(string name, string ns, object o, Type type)
    {
      Type elementType = TypeScope.GetArrayElementType(type, null);
      string typeName;
      string typeNs;

      StringBuilder arrayDims = new StringBuilder();
      if (!soap12)
      {
        while ((elementType.IsArray || typeof(IEnumerable).IsAssignableFrom(elementType)) && GetPrimitiveTypeName(elementType, false) == null)
        {
          elementType = TypeScope.GetArrayElementType(elementType, null);
          arrayDims.Append("[]");
        }
      }

      if (elementType == typeof(object))
      {
        typeName = Soap.UrType;
        typeNs = XmlSchema.Namespace;
      }
      else
      {
        TypeEntry entry = GetTypeEntry(elementType);
        if (entry != null)
        {
          typeName = entry.typeName;
          typeNs = entry.typeNs;
        }
        else if (soap12)
        {
          XmlQualifiedName qualName = GetPrimitiveTypeName(elementType, false);
          if (qualName != null)
          {
            typeName = qualName.Name;
            typeNs = qualName.Namespace;
          }
          else
          {
            Type elementBaseType = elementType.BaseType;
            while (elementBaseType != null)
            {
              entry = GetTypeEntry(elementBaseType);
              if (entry != null) break;
              elementBaseType = elementBaseType.BaseType;
            }
            if (entry != null)
            {
              typeName = entry.typeName;
              typeNs = entry.typeNs;
            }
            else
            {
              typeName = Soap.UrType;
              typeNs = XmlSchema.Namespace;
            }
          }
        }
        else
        {
          XmlQualifiedName qualName = GetPrimitiveTypeName(elementType);
          typeName = qualName.Name;
          typeNs = qualName.Namespace;
        }
      }

      if (arrayDims.Length > 0)
        typeName = typeName + arrayDims.ToString();

      if (soap12 && name != null && name.Length > 0)
        WriteStartElement(name, ns, null, false);
      else
        WriteStartElement(Soap.Array, Soap.Encoding, null, true);

      WriteId(o, false);

      if (type.IsArray)
      {
        Array a = (Array)o;
        int arrayLength = a.Length;
        if (soap12)
        {
          w.WriteAttributeString("itemType", Soap12.Encoding, GetQualifiedName(typeName, typeNs));
          w.WriteAttributeString("arraySize", Soap12.Encoding, arrayLength.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
          w.WriteAttributeString("arrayType", Soap.Encoding, GetQualifiedName(typeName, typeNs) + "[" + arrayLength.ToString(CultureInfo.InvariantCulture) + "]");
        }
        for (int i = 0; i < arrayLength; i++)
        {
          WritePotentiallyReferencingElement("Item", "", a.GetValue(i), elementType, false, true);
        }
      }
      else
      {
#if DEBUG
                    // use exception in the place of Debug.Assert to avoid throwing asserts from a server process such as aspnet_ewp.exe
                    if (!typeof(IEnumerable).IsAssignableFrom(type)) throw new InvalidOperationException(Res.GetString(Res.XmlInternalErrorDetails, "not array like type " + type.FullName));
#endif

        int arrayLength = typeof(ICollection).IsAssignableFrom(type) ? ((ICollection)o).Count : -1;
        if (soap12)
        {
          w.WriteAttributeString("itemType", Soap12.Encoding, GetQualifiedName(typeName, typeNs));
          if (arrayLength >= 0)
            w.WriteAttributeString("arraySize", Soap12.Encoding, arrayLength.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
          string brackets = arrayLength >= 0 ? "[" + arrayLength + "]" : "[]";
          w.WriteAttributeString("arrayType", Soap.Encoding, GetQualifiedName(typeName, typeNs) + brackets);
        }
        IEnumerator e = ((IEnumerable)o).GetEnumerator();
        if (e != null)
        {
          while (e.MoveNext())
          {
            WritePotentiallyReferencingElement("Item", "", e.Current, elementType, false, true);
          }
        }
      }
      w.WriteEndElement();
    }
    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WritePotentiallyReferencingElement"]/*' />
    protected void WritePotentiallyReferencingElement(string n, string ns, object o)
    {
      WritePotentiallyReferencingElement(n, ns, o, null, false, false);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WritePotentiallyReferencingElement1"]/*' />
    protected void WritePotentiallyReferencingElement(string n, string ns, object o, Type ambientType)
    {
      WritePotentiallyReferencingElement(n, ns, o, ambientType, false, false);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WritePotentiallyReferencingElement2"]/*' />
    protected void WritePotentiallyReferencingElement(string n, string ns, object o, Type ambientType, bool suppressReference)
    {
      WritePotentiallyReferencingElement(n, ns, o, ambientType, suppressReference, false);
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WritePotentiallyReferencingElement3"]/*' />
    protected void WritePotentiallyReferencingElement(string n, string ns, object o, Type ambientType, bool suppressReference, bool isNullable)
    {
      if (o == null)
      {
        if (isNullable) WriteNullTagEncoded(n, ns);
        return;
      }
      Type t = o.GetType();
      if (Convert.GetTypeCode(o) == TypeCode.Object && !(o is Guid) && (t != typeof(XmlQualifiedName)) && !(o is XmlNode[]) && (t != typeof(byte[])))
      {
        if ((suppressReference || soap12) && !IsIdDefined(o))
        {
          WriteReferencedElement(n, ns, o, ambientType);
        }
        else
        {
          if (n == null)
          {
            TypeEntry entry = GetTypeEntry(t);
            WriteReferencingElement(entry.typeName, entry.typeNs, o, isNullable);
          }
          else
            WriteReferencingElement(n, ns, o, isNullable);
        }
      }
      else
      {
        // Enums always write xsi:type, so don't write it again here.
        bool needXsiType = t != ambientType && !t.IsEnum;
        TypeEntry entry = GetTypeEntry(t);
        if (entry != null)
        {
          if (n == null)
            WriteStartElement(entry.typeName, entry.typeNs, null, true);
          else
            WriteStartElement(n, ns, null, true);

          if (needXsiType) WriteXsiType(entry.typeName, entry.typeNs);
          entry.callback(o);
          w.WriteEndElement();
        }
        else
        {
          WriteTypedPrimitive(n, ns, o, needXsiType);
        }
      }
    }


    void WriteReferencedElement(object o, Type ambientType)
    {
      WriteReferencedElement(null, null, o, ambientType);
    }

    void WriteReferencedElement(string name, string ns, object o, Type ambientType)
    {
      if (name == null) name = String.Empty;
      Type t = o.GetType();
      if (t.IsArray || typeof(IEnumerable).IsAssignableFrom(t))
      {
        WriteArray(name, ns, o, t);
      }
      else
      {
        TypeEntry entry = GetTypeEntry(t);
        if (entry == null) throw CreateUnknownTypeException(t);
        WriteStartElement(name.Length == 0 ? entry.typeName : name, ns == null ? entry.typeNs : ns, null, true);
        WriteId(o, false);
        if (ambientType != t) WriteXsiType(entry.typeName, entry.typeNs);
        entry.callback(o);
        w.WriteEndElement();
      }
    }

    TypeEntry GetTypeEntry(Type t)
    {
      if (typeEntries == null)
      {
        typeEntries = new Hashtable();
        InitCallbacks();
      }
      return (TypeEntry)typeEntries[t];
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.InitCallbacks"]/*' />
    protected abstract void InitCallbacks();

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteReferencedElements"]/*' />
    protected void WriteReferencedElements()
    {
      if (referencesToWrite == null) return;

      for (int i = 0; i < referencesToWrite.Count; i++)
      {
        WriteReferencedElement(referencesToWrite[i], null);
      }
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.TopLevelElement"]/*' />
    protected void TopLevelElement()
    {
      objectsInUse = new Hashtable();
    }

    /// <include file='doc\XmlSerializationWriter.uex' path='docs/doc[@for="XmlSerializationWriter.WriteNamespaceDeclarations"]/*' />
    ///<internalonly/>
    protected void WriteNamespaceDeclarations(XmlSerializerNamespaces xmlns)
    {
      if (xmlns != null)
      {
        foreach (DictionaryEntry entry in xmlns.Namespaces)
        {
          string prefix = (string)entry.Key;
          string ns = (string)entry.Value;
          if (namespaces != null)
          {
            string oldNs = namespaces.Namespaces[prefix] as string;
            if (oldNs != null && oldNs != ns)
            {
              throw new InvalidOperationException(Res.GetString(Res.XmlDuplicateNs, prefix, ns));
            }
          }
          string oldPrefix = (ns == null || ns.Length == 0) ? null : Writer.LookupPrefix(ns);

          if (oldPrefix == null || oldPrefix != prefix)
          {
            WriteAttribute("xmlns", prefix, null, ns);
          }
        }
      }
      namespaces = null;
    }

    string NextPrefix()
    {
      if (usedPrefixes == null)
      {
        return aliasBase + (++tempNamespacePrefix);
      }
      while (usedPrefixes.ContainsKey(++tempNamespacePrefix)) { ;}
      return aliasBase + tempNamespacePrefix;
    }

    internal class TypeEntry
    {
      internal XmlSerializationWriteCallback callback;
      internal string typeNs;
      internal string typeName;
      internal Type type;
    }
  }
}
