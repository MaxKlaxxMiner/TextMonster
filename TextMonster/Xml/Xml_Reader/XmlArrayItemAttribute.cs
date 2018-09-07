using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlArrayItemAttribute.uex' path='docs/doc[@for="XmlArrayItemAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
  public class XmlArrayItemAttribute : System.Attribute
  {
    string elementName;
    Type type;
    string ns;
    string dataType;
    bool nullable;
    bool nullableSpecified = false;
    XmlSchemaForm form = XmlSchemaForm.None;
    int nestingLevel;
  }
}