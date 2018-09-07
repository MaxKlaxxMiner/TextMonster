﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;

namespace TextMonster.Xml.Xml_Reader
{
  [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
  [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
  public class XmlTextReader : FastXmlReader, IXmlLineInfo, IXmlNamespaceResolver
  {
    XmlTextReaderImpl impl;

    public XmlTextReader(string url, Stream input, XmlNameTable nt)
    {
      impl = new XmlTextReaderImpl(url, input, nt);
      impl.OuterReader = this;
    }

    public XmlTextReader(TextReader input)
    {
      impl = new XmlTextReaderImpl(input);
      impl.OuterReader = this;
    }

    public override XmlNodeType NodeType
    {
      get { return impl.NodeType; }
    }

    public override string Name
    {
      get { return impl.Name; }
    }

    public override string LocalName
    {
      get { return impl.LocalName; }
    }

    public override string NamespaceURI
    {
      get { return impl.NamespaceURI; }
    }

    public override string Prefix
    {
      get { return impl.Prefix; }
    }

    public override bool HasValue
    {
      get { return impl.HasValue; }
    }

    public override string Value
    {
      get { return impl.Value; }
    }

    public override int Depth
    {
      get { return impl.Depth; }
    }

    public override string BaseURI
    {
      get { return impl.BaseURI; }
    }

    public override bool IsEmptyElement
    {
      get { return impl.IsEmptyElement; }
    }

    public override bool IsDefault
    {
      get { return impl.IsDefault; }
    }

    public override char QuoteChar
    {
      get { return impl.QuoteChar; }
    }

    public override XmlSpace XmlSpace
    {
      get { return impl.XmlSpace; }
    }

    public override string XmlLang
    {
      get { return impl.XmlLang; }
    }

    // XmlTextReader does not override SchemaInfo, ValueType and ReadTypeValue

    public override int AttributeCount { get { return impl.AttributeCount; } }

    public override string GetAttribute(string name)
    {
      return impl.GetAttribute(name);
    }

    public override string GetAttribute(string localName, string namespaceURI)
    {
      return impl.GetAttribute(localName, namespaceURI);
    }

    public override string GetAttribute(int i)
    {
      return impl.GetAttribute(i);
    }

    public override bool MoveToAttribute(string name)
    {
      return impl.MoveToAttribute(name);
    }

    public override bool MoveToAttribute(string localName, string namespaceURI)
    {
      return impl.MoveToAttribute(localName, namespaceURI);
    }

    public override void MoveToAttribute(int i)
    {
      impl.MoveToAttribute(i);
    }

    public override bool MoveToFirstAttribute()
    {
      return impl.MoveToFirstAttribute();
    }

    public override bool MoveToNextAttribute()
    {
      return impl.MoveToNextAttribute();
    }

    public override bool MoveToElement()
    {
      return impl.MoveToElement();
    }

    public override bool ReadAttributeValue()
    {
      return impl.ReadAttributeValue();
    }

    public override bool Read()
    {
      return impl.Read();
    }

    public override bool EOF
    {
      get { return impl.EOF; }
    }

    public override void Close()
    {
      impl.Close();
    }

    public override ReadState ReadState
    {
      get { return impl.ReadState; }
    }

    public override void Skip()
    {
      impl.Skip();
    }

    public override XmlNameTable NameTable
    {
      get { return impl.NameTable; }
    }

    public override String LookupNamespace(String prefix)
    {
      string ns = impl.LookupNamespace(prefix);
      if (ns != null && ns.Length == 0)
      {
        ns = null;
      }
      return ns;
    }

    public override bool CanResolveEntity
    {
      get { return true; }
    }

    public override void ResolveEntity()
    {
      impl.ResolveEntity();
    }

    public override int ReadContentAsBase64(byte[] buffer, int index, int count)
    {
      return impl.ReadContentAsBase64(buffer, index, count);
    }

    public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
    {
      return impl.ReadElementContentAsBase64(buffer, index, count);
    }

    public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
    {
      return impl.ReadContentAsBinHex(buffer, index, count);
    }

    public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
    {
      return impl.ReadElementContentAsBinHex(buffer, index, count);
    }

    // Text streaming methods

    // XmlTextReader does do support streaming of Value (there are backwards compatibility issues when enabled)
    public override bool CanReadValueChunk
    {
      get { return false; }
    }

    public bool HasLineInfo() { return true; }

    public int LineNumber { get { return impl.LineNumber; } }

    public int LinePosition { get { return impl.LinePosition; } }

    //
    // IXmlNamespaceResolver members
    //
    IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
    {
      return impl.GetNamespacesInScope(scope);
    }

    string IXmlNamespaceResolver.LookupNamespace(string prefix)
    {
      return impl.LookupNamespace(prefix);
    }

    string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
    {
      return impl.LookupPrefix(namespaceName);
    }

    public XmlResolver XmlResolver
    {
      set { impl.XmlResolver = value; }
    }

    //
    // Internal helper methods
    //
    internal XmlTextReaderImpl Impl
    {
      get { return impl; }
    }

    internal override XmlNamespaceManager NamespaceManager
    {
      get { return impl.NamespaceManager; }
    }

    internal override IDtdInfo DtdInfo
    {
      get { return impl.DtdInfo; }
    }
  }
}
