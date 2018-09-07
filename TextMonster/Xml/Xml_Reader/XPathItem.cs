using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Base class for XPathNavigator and XmlAtomicValue.
  /// </summary>
  public abstract class XPathItem
  {
    /// <summary>
    /// Typed and untyped value accessors.
    /// </summary>
    public abstract string Value { get; }

    public object ValueAs(Type returnType) { return ValueAs(returnType, null); }
    public abstract object ValueAs(Type returnType, IXmlNamespaceResolver nsResolver);
  }
}
