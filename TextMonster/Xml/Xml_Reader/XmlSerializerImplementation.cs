namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializerImplementation"]/*' />
  ///<internalonly/>
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public abstract class XmlSerializerImplementation
  {
    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializerImplementation.Reader"]/*' />
    public virtual XmlSerializationReader Reader { get { throw new NotSupportedException(); } }
    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializerImplementation.Writer"]/*' />
    public virtual XmlSerializationWriter Writer { get { throw new NotSupportedException(); } }
    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializerImplementation.ReadMethods"]/*' />
    public virtual Hashtable ReadMethods { get { throw new NotSupportedException(); } }
    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializerImplementation.WriteMethods"]/*' />
    public virtual Hashtable WriteMethods { get { throw new NotSupportedException(); } }
    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializerImplementation.TypedSerializers"]/*' />
    public virtual Hashtable TypedSerializers { get { throw new NotSupportedException(); } }
    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializerImplementation.CanSerialize"]/*' />
    public virtual bool CanSerialize(Type type) { throw new NotSupportedException(); }
    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializerImplementation.GetSerializer"]/*' />
    public virtual XmlSerializer GetSerializer(Type type) { throw new NotSupportedException(); }
  }
}
