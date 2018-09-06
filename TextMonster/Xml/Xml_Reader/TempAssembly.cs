using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  internal class TempAssembly
  {
    internal const string GeneratedAssemblyNamespace = "Microsoft.Xml.Serialization.GeneratedAssembly";
    Assembly assembly;
    bool pregeneratedAssmbly = false;
    XmlSerializerImplementation contract = null;
    Hashtable writerMethods;
    Hashtable readerMethods;
    TempMethodDictionary methods;
    static object[] emptyObjectArray = new object[0];
    Hashtable assemblies = new Hashtable();
    static volatile FileIOPermission fileIOPermission;

    internal class TempMethod
    {
      internal MethodInfo writeMethod;
      internal MethodInfo readMethod;
      internal string name;
      internal string ns;
      internal bool isSoap;
      internal string methodKey;
    }

    internal TempAssembly(XmlMapping[] xmlMappings, Type[] types, string defaultNamespace, string location, Evidence evidence)
    {
      bool containsSoapMapping = false;
      for (int i = 0; i < xmlMappings.Length; i++)
      {
        xmlMappings[i].CheckShallow();
        if (xmlMappings[i].IsSoap)
        {
          containsSoapMapping = true;
        }
      }

      // We will make best effort to use RefEmit for assembly generation
      bool fallbackToCSharpAssemblyGeneration = false;

      if (!containsSoapMapping && !TempAssembly.UseLegacySerializerGeneration)
      {
        try
        {
          assembly = GenerateRefEmitAssembly(xmlMappings, types, defaultNamespace, evidence);
        }
        // Only catch and handle known failures with RefEmit
        catch (CodeGeneratorConversionException)
        {
          fallbackToCSharpAssemblyGeneration = true;
        }
        // Add other known exceptions here...
        //
      }
      else
      {
        fallbackToCSharpAssemblyGeneration = true;
      }

      InitAssemblyMethods(xmlMappings);
    }

    internal TempAssembly(XmlMapping[] xmlMappings, Assembly assembly, XmlSerializerImplementation contract)
    {
      this.assembly = assembly;
      InitAssemblyMethods(xmlMappings);
      this.contract = contract;
      pregeneratedAssmbly = true;
    }

    internal static bool UseLegacySerializerGeneration
    {
      get
      {
        return true;
      }
    }

    internal XmlSerializerImplementation Contract
    {
      get
      {
        if (contract == null)
        {
          contract = (XmlSerializerImplementation)Activator.CreateInstance(GetTypeFromAssembly(this.assembly, "XmlSerializerContract"));
        }
        return contract;
      }
    }

    internal void InitAssemblyMethods(XmlMapping[] xmlMappings)
    {
      methods = new TempMethodDictionary();
      for (int i = 0; i < xmlMappings.Length; i++)
      {
        TempMethod method = new TempMethod();
        method.isSoap = xmlMappings[i].IsSoap;
        method.methodKey = xmlMappings[i].Key;
        XmlTypeMapping xmlTypeMapping = xmlMappings[i] as XmlTypeMapping;
        if (xmlTypeMapping != null)
        {
          method.name = xmlTypeMapping.ElementName;
          method.ns = xmlTypeMapping.Namespace;
        }
        methods.Add(xmlMappings[i].Key, method);
      }
    }

    /// <devdoc>
    ///    <para>
    ///    Attempts to load pre-generated serialization assembly.
    ///    First check for the [XmlSerializerAssembly] attribute
    ///    </para>
    /// </devdoc>
    // SxS: This method does not take any resource name and does not expose any resources to the caller.
    // It's OK to suppress the SxS warning.
    [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
    [ResourceExposure(ResourceScope.None)]
    internal static Assembly LoadGeneratedAssembly(Type type, string defaultNamespace, out XmlSerializerImplementation contract)
    {
      Assembly serializer = null;
      contract = null;
      string serializerName = null;

      // Packaged apps do not support loading generated serializers.
      return null;
    }

    static void Log(string message, EventLogEntryType type)
    {
      new EventLogPermission(PermissionState.Unrestricted).Assert();
      EventLog.WriteEntry("XmlSerializer", message, type);
    }

    static AssemblyName GetName(Assembly assembly, bool copyName)
    {
      PermissionSet perms = new PermissionSet(PermissionState.None);
      perms.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
      perms.Assert();
      return assembly.GetName(copyName);
    }


    static bool IsSerializerVersionMatch(Assembly serializer, Type type, string defaultNamespace, string location)
    {
      if (serializer == null)
        return false;
      object[] attrs = serializer.GetCustomAttributes(typeof(XmlSerializerVersionAttribute), false);
      if (attrs.Length != 1)
        return false;

      XmlSerializerVersionAttribute assemblyInfo = (XmlSerializerVersionAttribute)attrs[0];
      // we found out dated pre-generate assembly
      // 
      if (assemblyInfo.ParentAssemblyId == GenerateAssemblyId(type) && assemblyInfo.Namespace == defaultNamespace)
        return true;
      return false;
    }

    // SxS: This method does not take any resource name and does not expose any resources to the caller.
    // It's OK to suppress the SxS warning.
    [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
    [ResourceExposure(ResourceScope.None)]
    static string GenerateAssemblyId(Type type)
    {
      Module[] modules = type.Assembly.GetModules();
      ArrayList list = new ArrayList();
      for (int i = 0; i < modules.Length; i++)
      {
        list.Add(modules[i].ModuleVersionId.ToString());
      }
      list.Sort();
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < list.Count; i++)
      {
        sb.Append(list[i].ToString());
        sb.Append(",");
      }
      return sb.ToString();
    }

    internal static Assembly GenerateAssembly(XmlMapping[] xmlMappings, Type[] types, string defaultNamespace, Evidence evidence, XmlSerializerCompilerParameters parameters, Assembly assembly, Hashtable assemblies)
    {
      return null;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts", Justification = "It is safe because the serialization assembly is generated by the framework code, not by the user.")]
    internal static Assembly GenerateRefEmitAssembly(XmlMapping[] xmlMappings, Type[] types, string defaultNamespace, Evidence evidence)
    {
      return null;
    }

    // SxS: This method does not take any resource name and does not expose any resources to the caller.
    // It's OK to suppress the SxS warning.
    [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
    [ResourceExposure(ResourceScope.None)]
    static MethodInfo GetMethodFromType(Type type, string methodName, Assembly assembly)
    {
      MethodInfo method = type.GetMethod(methodName);
      if (method != null)
        return method;

      MissingMethodException missingMethod = new MissingMethodException(type.FullName, methodName);
      if (assembly != null)
      {
        throw new InvalidOperationException(Res.GetString(Res.XmlSerializerExpired, assembly.FullName, assembly.CodeBase), missingMethod);
      }
      throw missingMethod;
    }

    internal static Type GetTypeFromAssembly(Assembly assembly, string typeName)
    {
      typeName = GeneratedAssemblyNamespace + "." + typeName;
      Type type = assembly.GetType(typeName);
      if (type == null) throw new InvalidOperationException(Res.GetString(Res.XmlMissingType, typeName, assembly.FullName));
      return type;
    }

    internal bool CanRead(XmlMapping mapping, FastXmlReader xmlReader)
    {
      if (mapping == null)
        return false;

      if (mapping.Accessor.Any)
      {
        return true;
      }
      TempMethod method = methods[mapping.Key];
      return xmlReader.IsStartElement(method.name, method.ns);
    }

    string ValidateEncodingStyle(string encodingStyle, string methodKey)
    {
      if (encodingStyle != null && encodingStyle.Length > 0)
      {
        if (methods[methodKey].isSoap)
        {
          if (encodingStyle != Soap.Encoding && encodingStyle != Soap12.Encoding)
          {
            throw new InvalidOperationException(Res.GetString(Res.XmlInvalidEncoding3, encodingStyle, Soap.Encoding, Soap12.Encoding));
          }
        }
        else
        {
          throw new InvalidOperationException(Res.GetString(Res.XmlInvalidEncodingNotEncoded1, encodingStyle));
        }
      }
      else
      {
        if (methods[methodKey].isSoap)
        {
          encodingStyle = Soap.Encoding;
        }
      }
      return encodingStyle;
    }

    internal static FileIOPermission FileIOPermission
    {
      get
      {
        if (fileIOPermission == null)
          fileIOPermission = new FileIOPermission(PermissionState.Unrestricted);
        return fileIOPermission;
      }
    }

    internal object InvokeReader(XmlMapping mapping, FastXmlReader xmlReader, XmlDeserializationEvents events, string encodingStyle)
    {
      XmlSerializationReader reader = null;
      try
      {
        encodingStyle = ValidateEncodingStyle(encodingStyle, mapping.Key);
        reader = Contract.Reader;
        reader.Init(xmlReader, events, encodingStyle, this);
        if (methods[mapping.Key].readMethod == null)
        {
          if (readerMethods == null)
          {
            readerMethods = Contract.ReadMethods;
          }
          string methodName = (string)readerMethods[mapping.Key];
          if (methodName == null)
          {
            throw new InvalidOperationException(Res.GetString(Res.XmlNotSerializable, mapping.Accessor.Name));
          }
          methods[mapping.Key].readMethod = GetMethodFromType(reader.GetType(), methodName, pregeneratedAssmbly ? this.assembly : null);
        }
        return methods[mapping.Key].readMethod.Invoke(reader, emptyObjectArray);
      }
      catch (SecurityException e)
      {
        throw new InvalidOperationException(Res.GetString(Res.XmlNoPartialTrust), e);
      }
      finally
      {
        if (reader != null)
          reader.Dispose();
      }
    }

    internal void InvokeWriter(XmlMapping mapping, XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle, string id)
    {
      XmlSerializationWriter writer = null;
      try
      {
        encodingStyle = ValidateEncodingStyle(encodingStyle, mapping.Key);
        writer = Contract.Writer;
        writer.Init(xmlWriter, namespaces, encodingStyle, id, this);
        if (methods[mapping.Key].writeMethod == null)
        {
          if (writerMethods == null)
          {
            writerMethods = Contract.WriteMethods;
          }
          string methodName = (string)writerMethods[mapping.Key];
          if (methodName == null)
          {
            throw new InvalidOperationException(Res.GetString(Res.XmlNotSerializable, mapping.Accessor.Name));
          }
          methods[mapping.Key].writeMethod = GetMethodFromType(writer.GetType(), methodName, pregeneratedAssmbly ? assembly : null);
        }
        methods[mapping.Key].writeMethod.Invoke(writer, new object[] { o });
      }
      catch (SecurityException e)
      {
        throw new InvalidOperationException(Res.GetString(Res.XmlNoPartialTrust), e);
      }
      finally
      {
        if (writer != null)
          writer.Dispose();
      }
    }

    internal Assembly GetReferencedAssembly(string name)
    {
      return assemblies != null && name != null ? (Assembly)assemblies[name] : null;
    }

    internal bool NeedAssembyResolve
    {
      get { return assemblies != null && assemblies.Count > 0; }
    }

    internal sealed class TempMethodDictionary : DictionaryBase
    {
      internal TempMethod this[string key]
      {
        get
        {
          return (TempMethod)Dictionary[key];
        }
      }
      internal void Add(string key, TempMethod value)
      {
        Dictionary.Add(key, value);
      }
    }
  }

  internal class CodeGeneratorConversionException : Exception
  {

    private Type sourceType;
    private Type targetType;
    private bool isAddress;
    private string reason;

    public CodeGeneratorConversionException(Type sourceType, Type targetType, bool isAddress, string reason)
      : base()
    {

      this.sourceType = sourceType;
      this.targetType = targetType;
      this.isAddress = isAddress;
      this.reason = reason;
    }
  }

  sealed class XmlSerializerCompilerParameters
  {
    bool needTempDirAccess;
    CompilerParameters parameters;
    XmlSerializerCompilerParameters(CompilerParameters parameters, bool needTempDirAccess)
    {
      this.needTempDirAccess = needTempDirAccess;
      this.parameters = parameters;
    }

    internal bool IsNeedTempDirAccess { get { return this.needTempDirAccess; } }
    internal CompilerParameters CodeDomParameters { get { return this.parameters; } }

    internal static XmlSerializerCompilerParameters Create(string location)
    {
      return null;
    }

    internal static XmlSerializerCompilerParameters Create(CompilerParameters parameters, bool needTempDirAccess)
    {
      return new XmlSerializerCompilerParameters(parameters, needTempDirAccess);
    }
  }

  /// <include file='doc\XmlSerializerVersionAttribute.uex' path='docs/doc[@for="XmlSerializerVersionAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.Assembly)]
  public sealed class XmlSerializerVersionAttribute : System.Attribute
  {
    string mvid;
    string serializerVersion;
    string ns;
    Type type;

    /// <include file='doc\XmlSerializerVersionAttribute.uex' path='docs/doc[@for="XmlSerializerVersionAttribute.XmlSerializerVersionAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlSerializerVersionAttribute()
    {
    }

    /// <include file='doc\XmlSerializerVersionAttribute.uex' path='docs/doc[@for="XmlSerializerVersionAttribute.XmlSerializerAssemblyAttribute1"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlSerializerVersionAttribute(Type type)
    {
      this.type = type;
    }

    /// <include file='doc\XmlSerializerVersionAttribute.uex' path='docs/doc[@for="XmlSerializerVersionAttribute.ParentAssemblyId"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string ParentAssemblyId
    {
      get { return mvid; }
      set { mvid = value; }
    }

    /// <include file='doc\XmlSerializerVersionAttribute.uex' path='docs/doc[@for="XmlSerializerVersionAttribute.ParentAssemblyId"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string Version
    {
      get { return serializerVersion; }
      set { serializerVersion = value; }
    }


    /// <include file='doc\XmlSerializerVersionAttribute.uex' path='docs/doc[@for="XmlSerializerVersionAttribute.Namespace"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string Namespace
    {
      get { return ns; }
      set { ns = value; }
    }

    /// <include file='doc\XmlSerializerVersionAttribute.uex' path='docs/doc[@for="XmlSerializerVersionAttribute.TypeName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public Type Type
    {
      get { return type; }
      set { type = value; }
    }
  }

  internal class Soap12
  {
    private Soap12() { }
    internal const string Encoding = "http://www.w3.org/2003/05/soap-encoding";
    internal const string RpcNamespace = "http://www.w3.org/2003/05/soap-rpc";
    internal const string RpcResult = "result";
  }

  /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlDeserializationEvents"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public struct XmlDeserializationEvents
  {
    XmlNodeEventHandler onUnknownNode;
    XmlAttributeEventHandler onUnknownAttribute;
    XmlElementEventHandler onUnknownElement;
    UnreferencedObjectEventHandler onUnreferencedObject;
    internal object sender;

    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlDeserializationEvents.OnUnknownNode"]/*' />
    public XmlNodeEventHandler OnUnknownNode
    {
      get
      {
        return onUnknownNode;
      }

      set
      {
        onUnknownNode = value;
      }
    }

    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlDeserializationEvents.OnUnknownAttribute"]/*' />
    public XmlAttributeEventHandler OnUnknownAttribute
    {
      get
      {
        return onUnknownAttribute;
      }
      set
      {
        onUnknownAttribute = value;
      }
    }

    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlDeserializationEvents.OnUnknownElement"]/*' />
    public XmlElementEventHandler OnUnknownElement
    {
      get
      {
        return onUnknownElement;
      }
      set
      {
        onUnknownElement = value;
      }
    }

    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlDeserializationEvents.OnUnreferencedObject"]/*' />
    public UnreferencedObjectEventHandler OnUnreferencedObject
    {
      get
      {
        return onUnreferencedObject;
      }
      set
      {
        onUnreferencedObject = value;
      }
    }
  }

  /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlNodeEventHandler"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public delegate void XmlNodeEventHandler(object sender, XmlNodeEventArgs e);

  /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlAttributeEventHandler"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public delegate void XmlAttributeEventHandler(object sender, XmlAttributeEventArgs e);

  /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlAttributeEventArgs"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlAttributeEventArgs : EventArgs
  {
    object o;
    XmlAttribute attr;
    string qnames;
    int lineNumber;
    int linePosition;


    internal XmlAttributeEventArgs(XmlAttribute attr, int lineNumber, int linePosition, object o, string qnames)
    {
      this.attr = attr;
      this.o = o;
      this.qnames = qnames;
      this.lineNumber = lineNumber;
      this.linePosition = linePosition;
    }


    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlAttributeEventArgs.ObjectBeingDeserialized"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public object ObjectBeingDeserialized
    {
      get { return o; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlAttributeEventArgs.Attr"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlAttribute Attr
    {
      get { return attr; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlAttributeEventArgs.LineNumber"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Gets the current line number.
    ///    </para>
    /// </devdoc>
    public int LineNumber
    {
      get { return lineNumber; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlAttributeEventArgs.LinePosition"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Gets the current line position.
    ///    </para>
    /// </devdoc>
    public int LinePosition
    {
      get { return linePosition; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlAttributeEventArgs.Attributes"]/*' />
    /// <devdoc>
    ///    <para>
    ///       List the qnames of attributes expected in the current context.
    ///    </para>
    /// </devdoc>
    public string ExpectedAttributes
    {
      get { return qnames == null ? string.Empty : qnames; }
    }
  }

  /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlElementEventHandler"]/*' />
  public delegate void XmlElementEventHandler(object sender, XmlElementEventArgs e);

  /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlElementEventArgs"]/*' />
  public class XmlElementEventArgs : EventArgs
  {
    object o;
    XmlElement elem;
    string qnames;
    int lineNumber;
    int linePosition;

    internal XmlElementEventArgs(XmlElement elem, int lineNumber, int linePosition, object o, string qnames)
    {
      this.elem = elem;
      this.o = o;
      this.qnames = qnames;
      this.lineNumber = lineNumber;
      this.linePosition = linePosition;
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlElementEventArgs.ObjectBeingDeserialized"]/*' />
    public object ObjectBeingDeserialized
    {
      get { return o; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlElementEventArgs.Attr"]/*' />
    public XmlElement Element
    {
      get { return elem; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlElementEventArgs.LineNumber"]/*' />
    public int LineNumber
    {
      get { return lineNumber; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlElementEventArgs.LinePosition"]/*' />
    public int LinePosition
    {
      get { return linePosition; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlAttributeEventArgs.ExpectedElements"]/*' />
    /// <devdoc>
    ///    <para>
    ///       List of qnames of elements expected in the current context.
    ///    </para>
    /// </devdoc>
    public string ExpectedElements
    {
      get { return qnames == null ? string.Empty : qnames; }
    }
  }

  /// <include file='doc\_Events.uex' path='docs/doc[@for="UnreferencedObjectEventHandler"]/*' />
  public delegate void UnreferencedObjectEventHandler(object sender, UnreferencedObjectEventArgs e);

  /// <include file='doc\_Events.uex' path='docs/doc[@for="UnreferencedObjectEventArgs"]/*' />
  public class UnreferencedObjectEventArgs : EventArgs
  {
    object o;
    string id;

    /// <include file='doc\_Events.uex' path='docs/doc[@for="UnreferencedObjectEventArgs.UnreferencedObjectEventArgs"]/*' />
    public UnreferencedObjectEventArgs(object o, string id)
    {
      this.o = o;
      this.id = id;
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="UnreferencedObjectEventArgs.UnreferencedObject"]/*' />
    public object UnreferencedObject
    {
      get { return o; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="UnreferencedObjectEventArgs.UnreferencedId"]/*' />
    public string UnreferencedId
    {
      get { return id; }
    }
  }

  /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlNodeEventArgs"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlNodeEventArgs : EventArgs
  {
    object o;
    XmlNode xmlNode;
    int lineNumber;
    int linePosition;


    internal XmlNodeEventArgs(XmlNode xmlNode, int lineNumber, int linePosition, object o)
    {
      this.o = o;
      this.xmlNode = xmlNode;
      this.lineNumber = lineNumber;
      this.linePosition = linePosition;
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlNodeEventArgs.ObjectBeingDeserialized"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public object ObjectBeingDeserialized
    {
      get { return o; }
    }


    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlNodeEventArgs.NodeType"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlNodeType NodeType
    {
      get { return xmlNode.NodeType; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlNodeEventArgs.Name"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string Name
    {
      get { return xmlNode.Name; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlNodeEventArgs.LocalName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string LocalName
    {
      get { return xmlNode.LocalName; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlNodeEventArgs.NamespaceURI"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string NamespaceURI
    {
      get { return xmlNode.NamespaceURI; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlNodeEventArgs.Text"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string Text
    {
      get { return xmlNode.Value; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlNodeEventArgs.LineNumber"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Gets the current line number.
    ///    </para>
    /// </devdoc>
    public int LineNumber
    {
      get { return lineNumber; }
    }

    /// <include file='doc\_Events.uex' path='docs/doc[@for="XmlNodeEventArgs.LinePosition"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Gets the current line position.
    ///    </para>
    /// </devdoc>
    public int LinePosition
    {
      get { return linePosition; }
    }
  }
}
