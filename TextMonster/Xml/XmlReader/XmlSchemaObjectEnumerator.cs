using System.Collections;

namespace TextMonster.Xml.XmlReader
{
  /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectEnumerator"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaObjectEnumerator : IEnumerator
  {
    IEnumerator enumerator;

    internal XmlSchemaObjectEnumerator(IEnumerator enumerator)
    {
      this.enumerator = enumerator;
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectEnumerator.Reset"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public void Reset()
    {
      enumerator.Reset();
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectEnumerator.MoveNext"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public bool MoveNext()
    {
      return enumerator.MoveNext();
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectEnumerator.Current"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlSchemaObject Current
    {
      get { return (XmlSchemaObject)enumerator.Current; }
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectEnumerator.IEnumerator.Reset"]/*' />
    /// <internalonly/>
    void IEnumerator.Reset()
    {
      enumerator.Reset();
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectEnumerator.IEnumerator.MoveNext"]/*' />
    /// <internalonly/>
    bool IEnumerator.MoveNext()
    {
      return enumerator.MoveNext();
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectEnumerator.IEnumerator.Current"]/*' />
    /// <internalonly/>
    object IEnumerator.Current
    {
      get { return enumerator.Current; }
    }
  }
}
