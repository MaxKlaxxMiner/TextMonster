using System;
using System.Collections;
using System.IO;
using System.Threading;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [XmlRoot("schema", Namespace = XmlSchema.Namespace)]
  public class XmlSchema : XmlSchemaObject
  {

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.Namespace"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public const string Namespace = XmlReservedNs.NsXs;

    XmlSchemaForm attributeFormDefault = XmlSchemaForm.None;
    XmlSchemaForm elementFormDefault = XmlSchemaForm.None;
    XmlSchemaDerivationMethod blockDefault = XmlSchemaDerivationMethod.None;
    XmlSchemaDerivationMethod finalDefault = XmlSchemaDerivationMethod.None;
    string targetNs;
    string version;
    XmlSchemaObjectCollection includes = new XmlSchemaObjectCollection();
    XmlSchemaObjectCollection items = new XmlSchemaObjectCollection();
    string id;
    XmlAttribute[] moreAttributes;

    // compiled info
    bool isCompiled = false;
    bool isCompiledBySet = false;
    bool isPreprocessed = false;
    bool isRedefined = false;
    int errorCount = 0;
    XmlSchemaObjectTable attributes;
    XmlSchemaObjectTable attributeGroups = new XmlSchemaObjectTable();
    XmlSchemaObjectTable elements = new XmlSchemaObjectTable();
    XmlSchemaObjectTable types = new XmlSchemaObjectTable();
    XmlSchemaObjectTable groups = new XmlSchemaObjectTable();
    XmlSchemaObjectTable notations = new XmlSchemaObjectTable();
    XmlSchemaObjectTable identityConstraints = new XmlSchemaObjectTable();

    static int globalIdCounter = -1;
    ArrayList importedSchemas;
    ArrayList importedNamespaces;

    int schemaId = -1; //Not added to a set
    Uri baseUri;
    bool isChameleon;
    Hashtable ids = new Hashtable();
    XmlDocument document;
    XmlNameTable nameTable;

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.Read"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static XmlSchema Read(TextReader reader, ValidationEventHandler validationEventHandler)
    {
      return Read(new XmlTextReader(reader), validationEventHandler);
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.Read2"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static XmlSchema Read(FastXmlReader reader, ValidationEventHandler validationEventHandler)
    {
      XmlNameTable nameTable = reader.NameTable;
      Parser parser = new Parser(SchemaType.XSD, nameTable, new SchemaNames(nameTable), validationEventHandler);
      try
      {
        parser.Parse(reader, null);
      }
      catch (XmlSchemaException e)
      {
        if (validationEventHandler != null)
        {
          validationEventHandler(null, new ValidationEventArgs(e));
        }
        else
        {
          throw e;
        }
        return null;
      }
      return parser.XmlSchema;
    }

    internal bool CompileSchema(XmlSchemaCollection xsc, XmlResolver resolver, SchemaInfo schemaInfo, string ns, ValidationEventHandler validationEventHandler, XmlNameTable nameTable, bool CompileContentModel)
    {
      return false;
    }

    internal void CompileSchemaInSet(XmlNameTable nameTable, ValidationEventHandler eventHandler, XmlSchemaCompilationSettings compilationSettings)
    {
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.AttributeFormDefault"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("attributeFormDefault"), DefaultValue(XmlSchemaForm.None)]
    public XmlSchemaForm AttributeFormDefault
    {
      get { return attributeFormDefault; }
      set { attributeFormDefault = value; }
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.BlockDefault"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("blockDefault"), DefaultValue(XmlSchemaDerivationMethod.None)]
    public XmlSchemaDerivationMethod BlockDefault
    {
      get { return blockDefault; }
      set { blockDefault = value; }
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.FinalDefault"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("finalDefault"), DefaultValue(XmlSchemaDerivationMethod.None)]
    public XmlSchemaDerivationMethod FinalDefault
    {
      get { return finalDefault; }
      set { finalDefault = value; }
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.ElementFormDefault"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("elementFormDefault"), DefaultValue(XmlSchemaForm.None)]
    public XmlSchemaForm ElementFormDefault
    {
      get { return elementFormDefault; }
      set { elementFormDefault = value; }
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.TargetNamespace"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("targetNamespace", DataType = "anyURI")]
    public string TargetNamespace
    {
      get { return targetNs; }
      set { targetNs = value; }
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.Version"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("version", DataType = "token")]
    public string Version
    {
      get { return version; }
      set { version = value; }
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.Includes"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlElement("include", typeof(XmlSchemaInclude)),
     XmlElement("import", typeof(XmlSchemaImport)),
     XmlElement("redefine", typeof(XmlSchemaRedefine))]
    public XmlSchemaObjectCollection Includes
    {
      get { return includes; }
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.Items"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlElement("annotation", typeof(XmlSchemaAnnotation)),
     XmlElement("attribute", typeof(XmlSchemaAttribute)),
     XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroup)),
     XmlElement("complexType", typeof(XmlSchemaComplexType)),
     XmlElement("simpleType", typeof(XmlSchemaSimpleType)),
     XmlElement("element", typeof(XmlSchemaElement)),
     XmlElement("group", typeof(XmlSchemaGroup)),
     XmlElement("notation", typeof(XmlSchemaNotation))]
    public XmlSchemaObjectCollection Items
    {
      get { return items; }
    }

    [XmlIgnore]
    internal bool IsCompiledBySet
    {
      set { isCompiledBySet = value; }
    }

    [XmlIgnore]
    internal bool IsPreprocessed
    {
      get { return isPreprocessed; }
      set { isPreprocessed = value; }
    }

    [XmlIgnore]
    internal bool IsRedefined
    {
      get { return isRedefined; }
      set { isRedefined = value; }
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.Attributes"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlIgnore]
    public XmlSchemaObjectTable Attributes
    {
      get
      {
        if (attributes == null)
        {
          attributes = new XmlSchemaObjectTable();
        }
        return attributes;
      }
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.AttributeGroups"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlIgnore]
    public XmlSchemaObjectTable AttributeGroups
    {
      get
      {
        if (attributeGroups == null)
        {
          attributeGroups = new XmlSchemaObjectTable();
        }
        return attributeGroups;
      }
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.SchemaTypes"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlIgnore]
    public XmlSchemaObjectTable SchemaTypes
    {
      get
      {
        if (types == null)
        {
          types = new XmlSchemaObjectTable();
        }
        return types;
      }
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.Elements"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlIgnore]
    public XmlSchemaObjectTable Elements
    {
      get
      {
        if (elements == null)
        {
          elements = new XmlSchemaObjectTable();
        }
        return elements;
      }
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.Id"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("id", DataType = "ID")]
    public string Id
    {
      get { return id; }
      set { id = value; }
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.Groups"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlIgnore]
    public XmlSchemaObjectTable Groups
    {
      get { return groups; }
    }

    /// <include file='doc\XmlSchema.uex' path='docs/doc[@for="XmlSchema.Notations"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlIgnore]
    public XmlSchemaObjectTable Notations
    {
      get { return notations; }
    }

    [XmlIgnore]
    internal XmlSchemaObjectTable IdentityConstraints
    {
      get { return identityConstraints; }
    }

    [XmlIgnore]
    internal Uri BaseUri
    {
      get { return baseUri; }
      set
      {
        baseUri = value;
      }
    }

    [XmlIgnore]
    // Please be careful with this property. Since it lazy initialized and its value depends on a global state
    //   if it gets called on multiple schemas in a different order the schemas will end up with different IDs
    //   Unfortunately the IDs are used to sort the schemas in the schema set and thus changing the IDs might change
    //   the order which would be a breaking change!!
    // Simply put if you are planning to add or remove a call to this getter you need to be extra carefull
    //   or better don't do it at all.
    internal int SchemaId
    {
      get
      {
        if (schemaId == -1)
        {
          schemaId = Interlocked.Increment(ref globalIdCounter);
        }
        return schemaId;
      }
    }

    [XmlIgnore]
    internal bool IsChameleon
    {
      get { return isChameleon; }
      set { isChameleon = value; }
    }

    [XmlIgnore]
    internal Hashtable Ids
    {
      get { return ids; }
    }

    [XmlIgnore]
    internal XmlDocument Document
    {
      get { if (document == null) document = new XmlDocument(); return document; }
    }

    [XmlIgnore]
    internal int ErrorCount
    {
      get { return errorCount; }
      set { errorCount = value; }
    }

    internal XmlSchema DeepClone()
    {
      XmlSchema that = new XmlSchema();
      that.attributeFormDefault = this.attributeFormDefault;
      that.elementFormDefault = this.elementFormDefault;
      that.blockDefault = this.blockDefault;
      that.finalDefault = this.finalDefault;
      that.targetNs = this.targetNs;
      that.version = this.version;
      that.isPreprocessed = this.isPreprocessed;
      //that.IsProcessing           = this.IsProcessing; //Not sure if this is needed

      //Clone its Items
      for (int i = 0; i < this.items.Count; ++i)
      {
        XmlSchemaObject newItem;

        XmlSchemaComplexType complexType;
        XmlSchemaElement element;
        XmlSchemaGroup group;

        if ((complexType = items[i] as XmlSchemaComplexType) != null)
        {
          newItem = complexType.Clone(this);
        }
        else if ((element = items[i] as XmlSchemaElement) != null)
        {
          newItem = element.Clone(this);
        }
        else if ((group = items[i] as XmlSchemaGroup) != null)
        {
          newItem = group.Clone(this);
        }
        else
        {
          newItem = items[i].Clone();
        }
        that.Items.Add(newItem);
      }

      //Clone Includes
      for (int i = 0; i < this.includes.Count; ++i)
      {
        XmlSchemaExternal newInclude = (XmlSchemaExternal)this.includes[i].Clone();
        that.Includes.Add(newInclude);
      }
      that.Namespaces = this.Namespaces;
      //that.includes               = this.includes; //Need to verify this is OK for redefines
      that.BaseUri = this.BaseUri;
      return that;
    }

    [XmlIgnore]
    internal override string IdAttribute
    {
      get { return Id; }
      set { Id = value; }
    }

    internal override void SetUnhandledAttributes(XmlAttribute[] moreAttributes)
    {
      this.moreAttributes = moreAttributes;
    }
    internal override void AddAnnotation(XmlSchemaAnnotation annotation)
    {
      items.Add(annotation);
    }

    internal ArrayList ImportedSchemas
    {
      get
      {
        if (importedSchemas == null)
        {
          importedSchemas = new ArrayList();
        }
        return importedSchemas;
      }
    }

    internal ArrayList ImportedNamespaces
    {
      get
      {
        if (importedNamespaces == null)
        {
          importedNamespaces = new ArrayList();
        }
        return importedNamespaces;
      }
    }

    internal void GetExternalSchemasList(IList extList, XmlSchema schema)
    {
      if (extList.Contains(schema))
      {
        return;
      }
      extList.Add(schema);
      for (int i = 0; i < schema.Includes.Count; ++i)
      {
        XmlSchemaExternal ext = (XmlSchemaExternal)schema.Includes[i];
        if (ext.Schema != null)
        {
          GetExternalSchemasList(extList, ext.Schema);
        }
      }
    }
  }
}
