using System;
using System.Collections;
using System.Threading;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollection"]/*' />
  /// <devdoc>
  ///    <para>The XmlSchemaCollection contains a set of namespace URI's.
  ///       Each namespace also have an associated private data cache
  ///       corresponding to the XML-Data Schema or W3C XML Schema.
  ///       The XmlSchemaCollection will able to load XSD and XDR schemas,
  ///       and compile them into an internal "cooked schema representation".
  ///       The Validate method then uses this internal representation for
  ///       efficient runtime validation of any given subtree.</para>
  /// </devdoc>
  [Obsolete("Use Schema.XmlSchemaSet for schema compilation and validation. http://go.microsoft.com/fwlink/?linkid=14202")]
  public sealed class XmlSchemaCollection : ICollection
  {
    private Hashtable collection;
    private NameTable nameTable;
    private SchemaNames schemaNames;
    private ReaderWriterLock wLock;
    private int timeout = Timeout.Infinite;
    private bool isThreadSafe = true;
    private ValidationEventHandler validationEventHandler = null;
    private XmlResolver xmlResolver = null;


    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollection.XmlSchemaCollection1"]/*' />
    /// <devdoc>
    ///    <para>Construct a new empty schema collection with associated NameTable.
    ///       The NameTable is used when loading schemas</para>
    /// </devdoc>
    public XmlSchemaCollection(NameTable nametable)
    {
      if (nametable == null)
      {
        throw new ArgumentNullException("nametable");
      }
      nameTable = nametable;
      collection = Hashtable.Synchronized(new Hashtable());
      xmlResolver = new XmlUrlResolver();
      isThreadSafe = true;
      if (isThreadSafe)
      {
        wLock = new ReaderWriterLock();
      }
    }

    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollection.Count"]/*' />
    /// <devdoc>
    ///    <para>Returns the number of namespaces defined in this collection
    ///       (whether or not there is an actual schema associated with those namespaces or not).</para>
    /// </devdoc>
    public int Count
    {
      get { return collection.Count; }
    }

    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollection.NameTable"]/*' />
    /// <devdoc>
    ///    <para>The default NameTable used by the XmlSchemaCollection when loading new schemas.</para>
    /// </devdoc>
    public NameTable NameTable
    {
      get { return nameTable; }
    }

    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollection.ValidationEventHandler"]/*' />
    public event ValidationEventHandler ValidationEventHandler
    {
      add { validationEventHandler += value; }
      remove { validationEventHandler -= value; }
    }

    internal XmlResolver XmlResolver
    {
      set
      {
        xmlResolver = value;
      }
    }

    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollection.IEnumerable.GetEnumerator"]/*' />
    /// <internalonly/>
    /// <devdoc>
    /// Get a IEnumerator of the XmlSchemaCollection.
    /// </devdoc>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new XmlSchemaCollectionEnumerator(collection);
    }

    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollection.GetEnumerator"]/*' />
    public XmlSchemaCollectionEnumerator GetEnumerator()
    {
      return new XmlSchemaCollectionEnumerator(collection);
    }

    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollection.ICollection.CopyTo"]/*' />
    /// <internalonly/>
    void ICollection.CopyTo(Array array, int index)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (index < 0)
        throw new ArgumentOutOfRangeException("index");
      for (XmlSchemaCollectionEnumerator e = this.GetEnumerator(); e.MoveNext(); )
      {
        if (index == array.Length && array.IsFixedSize)
        {
          throw new ArgumentOutOfRangeException("index");
        }
        array.SetValue(e.Current, index++);
      }
    }

    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollection.ICollection.IsSynchronized"]/*' />
    /// <internalonly/>
    bool ICollection.IsSynchronized
    {
      get { return true; }
    }

    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollection.ICollection.SyncRoot"]/*' />
    /// <internalonly/>
    object ICollection.SyncRoot
    {
      get { return this; }
    }

    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollection.ICollection.Count"]/*' />
    /// <internalonly/>
    int ICollection.Count
    {
      get { return collection.Count; }
    }

    internal SchemaInfo GetSchemaInfo(string ns)
    {
      XmlSchemaCollectionNode node = (XmlSchemaCollectionNode)collection[(ns != null) ? ns : string.Empty];
      return (node != null) ? node.SchemaInfo : null;
    }

    internal SchemaNames GetSchemaNames(NameTable nt)
    {
      if (nameTable != nt)
      {
        return new SchemaNames(nt);
      }
      else
      {
        if (schemaNames == null)
        {
          schemaNames = new SchemaNames(nameTable);
        }
        return schemaNames;
      }
    }

    internal XmlSchema Add(string ns, SchemaInfo schemaInfo, XmlSchema schema, bool compile)
    {
      return Add(ns, schemaInfo, schema, compile, xmlResolver);
    }

    private XmlSchema Add(string ns, SchemaInfo schemaInfo, XmlSchema schema, bool compile, XmlResolver resolver)
    {
      int errorCount = 0;
      if (schema != null)
      {
        if (schema.ErrorCount == 0 && compile)
        {
          if (!schema.CompileSchema(this, resolver, schemaInfo, ns, validationEventHandler, nameTable, true))
          {
            errorCount = 1;
          }
          ns = schema.TargetNamespace == null ? string.Empty : schema.TargetNamespace;
        }
        errorCount += schema.ErrorCount;
      }
      else
      {
        errorCount += schemaInfo.ErrorCount;
        //ns = ns == null? string.Empty : NameTable.Add(ns);
        ns = NameTable.Add(ns); //Added without checking for ns == null, since XDR cannot have null namespace
      }
      if (errorCount == 0)
      {
        XmlSchemaCollectionNode node = new XmlSchemaCollectionNode();
        node.NamespaceURI = ns;
        node.SchemaInfo = schemaInfo;
        node.Schema = schema;
        Add(ns, node);
        return schema;
      }
      return null;
    }

    private void Add(string ns, XmlSchemaCollectionNode node)
    {
      if (isThreadSafe)
        wLock.AcquireWriterLock(timeout);
      try
      {
        if (collection[ns] != null)
          collection.Remove(ns);
        collection.Add(ns, node);
      }
      finally
      {
        if (isThreadSafe)
          wLock.ReleaseWriterLock();
      }
    }

    internal ValidationEventHandler EventHandler
    {
      get
      {
        return validationEventHandler;
      }
      set
      {
        validationEventHandler = value;
      }
    }
  };
}
