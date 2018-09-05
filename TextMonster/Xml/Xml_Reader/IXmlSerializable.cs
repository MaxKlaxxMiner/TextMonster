namespace TextMonster.Xml.Xml_Reader
{
  public interface IXmlSerializable
  {
    /// <include file='doc\IXmlSerializable.uex' path='docs/doc[@for="IXmlSerializable.GetSchema"]/*' />
    XmlSchema GetSchema();
    /// <include file='doc\IXmlSerializable.uex' path='docs/doc[@for="IXmlSerializable.ReadXml"]/*' />
    void ReadXml(FastXmlReader reader);
    /// <include file='doc\IXmlSerializable.uex' path='docs/doc[@for="IXmlSerializable.WriteXml"]/*' />
    void WriteXml(XmlWriter writer);
  }
}