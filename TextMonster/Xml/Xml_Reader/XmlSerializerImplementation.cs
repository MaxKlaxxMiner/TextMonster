using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializerImplementation"]/*' />
  ///<internalonly/>
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public abstract class XmlSerializerImplementation
  {
    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializerImplementation.Writer"]/*' />
    public virtual XmlSerializationWriter Writer { get { throw new NotSupportedException(); } }

    /// <include file='doc\XmlSerializer.uex' path='docs/doc[@for="XmlSerializerImplementation.WriteMethods"]/*' />
    public virtual Hashtable WriteMethods { get { throw new NotSupportedException(); } }
  }
}
