using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlTextAttribute.uex' path='docs/doc[@for="XmlTextAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
  public class XmlTextAttribute : Attribute
  {
    Type type;
    string dataType;
  }
}
