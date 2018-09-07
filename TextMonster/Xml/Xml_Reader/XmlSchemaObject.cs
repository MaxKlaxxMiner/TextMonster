﻿using System.Security.Permissions;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaObject.uex' path='docs/doc[@for="XmlSchemaObject"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
  public abstract class XmlSchemaObject
  {
    int lineNum = 0;
    int linePos = 0;
    string sourceUri;
    XmlSerializerNamespaces namespaces;
    XmlSchemaObject parent;

    //internal
    bool isProcessing; //Indicates whether this object is currently being processed

    /// <include file='doc\XmlSchemaObject.uex' path='docs/doc[@for="XmlSchemaObject.LineNum"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlIgnore]
    public int LineNumber
    {
      get { return lineNum; }
      set { lineNum = value; }
    }

    /// <include file='doc\XmlSchemaObject.uex' path='docs/doc[@for="XmlSchemaObject.LinePos"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlIgnore]
    public int LinePosition
    {
      get { return linePos; }
      set { linePos = value; }
    }

    /// <include file='doc\XmlSchemaObject.uex' path='docs/doc[@for="XmlSchemaObject.SourceUri"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlIgnore]
    public string SourceUri
    {
      get { return sourceUri; }
      set { sourceUri = value; }
    }

    /// <include file='doc\XmlSchemaObject.uex' path='docs/doc[@for="XmlSchemaObject.Parent"]/*' />
    [XmlIgnore]
    public XmlSchemaObject Parent
    {
      get { return parent; }
      set { parent = value; }
    }

    /// <include file='doc\XmlSchemaObject.uex' path='docs/doc[@for="XmlSchemaObject.Namespaces"]/*' />
    [XmlNamespaceDeclarations]
    public XmlSerializerNamespaces Namespaces
    {
      get
      {
        if (namespaces == null)
          namespaces = new XmlSerializerNamespaces();
        return namespaces;
      }
      set { namespaces = value; }
    }

    internal void OnAdd(XmlSchemaObjectCollection container, object item) { }
    internal void OnRemove(XmlSchemaObjectCollection container, object item) { }
    internal void OnClear(XmlSchemaObjectCollection container) { }

    [XmlIgnore]
    internal virtual string IdAttribute
    {
      get { return null; }
      set { }
    }

    internal virtual void SetUnhandledAttributes(XmlAttribute[] moreAttributes) { }
    internal virtual void AddAnnotation(XmlSchemaAnnotation annotation) { }

    [XmlIgnore]
    internal virtual string NameAttribute
    {
      get { return null; }
      set { }
    }

    [XmlIgnore]
    internal bool IsProcessing
    {
      get
      {
        return isProcessing;
      }
      set
      {
        isProcessing = value;
      }
    }

    internal virtual XmlSchemaObject Clone()
    {
      return (XmlSchemaObject)MemberwiseClone();
    }
  }
}
