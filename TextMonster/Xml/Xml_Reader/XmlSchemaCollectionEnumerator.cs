using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollectionEnumerator"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public sealed class XmlSchemaCollectionEnumerator : IEnumerator
  {
    private IDictionaryEnumerator enumerator;

    internal XmlSchemaCollectionEnumerator(Hashtable collection)
    {
      enumerator = collection.GetEnumerator();
    }

    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollectionEnumerator.IEnumerator.Reset"]/*' />
    /// <internalonly/>
    void IEnumerator.Reset()
    {
      enumerator.Reset();
    }

    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollectionEnumerator.IEnumerator.MoveNext"]/*' />
    /// <internalonly/>
    bool IEnumerator.MoveNext()
    {
      return enumerator.MoveNext();
    }

    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollectionEnumerator.MoveNext"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public bool MoveNext()
    {
      return enumerator.MoveNext();
    }

    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollectionEnumerator.IEnumerator.Current"]/*' />
    /// <internalonly/>
    object IEnumerator.Current
    {
      get { return this.Current; }
    }

    /// <include file='doc\XmlSchemaCollection.uex' path='docs/doc[@for="XmlSchemaCollectionEnumerator.Current"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlSchema Current
    {
      get
      {
        XmlSchemaCollectionNode n = (XmlSchemaCollectionNode)enumerator.Value;
        if (n != null)
          return n.Schema;
        else
          return null;
      }
    }

    internal XmlSchemaCollectionNode CurrentNode
    {
      get
      {
        XmlSchemaCollectionNode n = (XmlSchemaCollectionNode)enumerator.Value;
        return n;
      }
    }
  }
}
