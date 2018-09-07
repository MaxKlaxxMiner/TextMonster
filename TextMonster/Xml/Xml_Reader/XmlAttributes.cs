using System;
using System.Reflection;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlAttributes
  {
    XmlElementAttributes xmlElements = new XmlElementAttributes();
    XmlArrayItemAttributes xmlArrayItems = new XmlArrayItemAttributes();
    XmlAnyElementAttributes xmlAnyElements = new XmlAnyElementAttributes();
    XmlArrayAttribute xmlArray;
    XmlAttributeAttribute xmlAttribute;
    XmlTextAttribute xmlText;
    XmlEnumAttribute xmlEnum;
    bool xmlIgnore;
    bool xmlns;
    object xmlDefaultValue = null;
    XmlRootAttribute xmlRoot;
    XmlTypeAttribute xmlType;
    XmlAnyAttributeAttribute xmlAnyAttribute;
    XmlChoiceIdentifierAttribute xmlChoiceIdentifier;
    static volatile Type ignoreAttributeType;

    private static Type IgnoreAttribute
    {
      get
      {
        if (ignoreAttributeType == null)
        {
          ignoreAttributeType = typeof(object).Assembly.GetType("System.XmlIgnoreMemberAttribute");
          if (ignoreAttributeType == null)
          {
            ignoreAttributeType = typeof(XmlIgnoreAttribute);
          }
        }
        return ignoreAttributeType;
      }
    }

    /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlAttributes1"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlAttributes(ICustomAttributeProvider provider)
    {
      object[] attrs = provider.GetCustomAttributes(false);

      // most generic <any/> matches everithig 
      XmlAnyElementAttribute wildcard = null;
      for (int i = 0; i < attrs.Length; i++)
      {
        if (attrs[i] is XmlIgnoreAttribute || attrs[i] is ObsoleteAttribute || attrs[i].GetType() == IgnoreAttribute)
        {
          xmlIgnore = true;
          break;
        }
        else if (attrs[i] is XmlElementAttribute)
        {
          this.xmlElements.Add((XmlElementAttribute)attrs[i]);
        }
        else if (attrs[i] is XmlArrayItemAttribute)
        {
          this.xmlArrayItems.Add((XmlArrayItemAttribute)attrs[i]);
        }
        else if (attrs[i] is XmlAnyElementAttribute)
        {
          XmlAnyElementAttribute any = (XmlAnyElementAttribute)attrs[i];
          if ((any.Name == null || any.Name.Length == 0) && any.NamespaceSpecified && any.Namespace == null)
          {
            // ignore duplicate wildcards
            wildcard = any;
          }
          else
          {
            this.xmlAnyElements.Add((XmlAnyElementAttribute)attrs[i]);
          }
        }
        else if (attrs[i] is DefaultValueAttribute)
        {
          this.xmlDefaultValue = ((DefaultValueAttribute)attrs[i]).Value;
        }
        else if (attrs[i] is XmlAttributeAttribute)
        {
          this.xmlAttribute = (XmlAttributeAttribute)attrs[i];
        }
        else if (attrs[i] is XmlArrayAttribute)
        {
          this.xmlArray = (XmlArrayAttribute)attrs[i];
        }
        else if (attrs[i] is XmlTextAttribute)
        {
          this.xmlText = (XmlTextAttribute)attrs[i];
        }
        else if (attrs[i] is XmlEnumAttribute)
        {
          this.xmlEnum = (XmlEnumAttribute)attrs[i];
        }
        else if (attrs[i] is XmlRootAttribute)
        {
          this.xmlRoot = (XmlRootAttribute)attrs[i];
        }
        else if (attrs[i] is XmlTypeAttribute)
        {
          this.xmlType = (XmlTypeAttribute)attrs[i];
        }
        else if (attrs[i] is XmlAnyAttributeAttribute)
        {
          this.xmlAnyAttribute = (XmlAnyAttributeAttribute)attrs[i];
        }
        else if (attrs[i] is XmlChoiceIdentifierAttribute)
        {
          this.xmlChoiceIdentifier = (XmlChoiceIdentifierAttribute)attrs[i];
        }
        else if (attrs[i] is XmlNamespaceDeclarationsAttribute)
        {
          this.xmlns = true;
        }
      }
      if (xmlIgnore)
      {
        this.xmlElements.Clear();
        this.xmlArrayItems.Clear();
        this.xmlAnyElements.Clear();
        this.xmlDefaultValue = null;
        this.xmlAttribute = null;
        this.xmlArray = null;
        this.xmlText = null;
        this.xmlEnum = null;
        this.xmlType = null;
        this.xmlAnyAttribute = null;
        this.xmlChoiceIdentifier = null;
        this.xmlns = false;
      }
      else
      {
        if (wildcard != null)
        {
          this.xmlAnyElements.Add(wildcard);
        }
      }
    }

    /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlIgnore"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public bool XmlIgnore
    {
      get { return xmlIgnore; }
    }
  }
}