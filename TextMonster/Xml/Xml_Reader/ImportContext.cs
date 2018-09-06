using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  public class ImportContext
  {
    bool shareTypes;
    SchemaObjectCache cache; // cached schema top-level items
    Hashtable mappings; // XmlSchema -> SerializableMapping, XmlSchemaSimpleType -> EnumMapping, XmlSchemaComplexType -> StructMapping
    Hashtable elements; // XmlSchemaElement -> ElementAccessor
    CodeIdentifiers typeIdentifiers;

    /// <include file='doc\ImportContext.uex' path='docs/doc[@for="ImportContext.ImportContext"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public ImportContext(CodeIdentifiers identifiers, bool shareTypes)
    {
      this.typeIdentifiers = identifiers;
      this.shareTypes = shareTypes;
    }
    internal ImportContext() : this(null, false) { }

    internal SchemaObjectCache Cache
    {
      get
      {
        if (cache == null)
          cache = new SchemaObjectCache();
        return cache;
      }
    }

    internal Hashtable Elements
    {
      get
      {
        if (elements == null)
          elements = new Hashtable();
        return elements;
      }
    }

    internal Hashtable Mappings
    {
      get
      {
        if (mappings == null)
          mappings = new Hashtable();
        return mappings;
      }
    }

    /// <include file='doc\ImportContext.uex' path='docs/doc[@for="ImportContext.TypeIdentifiers"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public CodeIdentifiers TypeIdentifiers
    {
      get
      {
        if (typeIdentifiers == null)
          typeIdentifiers = new CodeIdentifiers();
        return typeIdentifiers;
      }
    }

    /// <include file='doc\ImportContext.uex' path='docs/doc[@for="ImportContext.ShareTypes"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public bool ShareTypes
    {
      get { return shareTypes; }
    }
  }
}