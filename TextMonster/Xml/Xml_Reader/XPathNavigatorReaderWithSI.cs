namespace TextMonster.Xml.Xml_Reader
{
  internal class XPathNavigatorReaderWithSI : XPathNavigatorReader, IXmlSchemaInfo
  {
    internal XPathNavigatorReaderWithSI(XPathNavigator navToRead, IXmlLineInfo xli, IXmlSchemaInfo xsi)
      : base(navToRead, xli, xsi)
    {
    }

    //-----------------------------------------------
    // IXmlSchemaInfo
    //-----------------------------------------------

    public virtual XmlSchemaValidity Validity { get { return IsReading ? this.schemaInfo.Validity : XmlSchemaValidity.NotKnown; } }
    public override bool IsDefault { get { return IsReading ? this.schemaInfo.IsDefault : false; } }
    public virtual bool IsNil { get { return IsReading ? this.schemaInfo.IsNil : false; } }
    public virtual XmlSchemaSimpleType MemberType { get { return IsReading ? this.schemaInfo.MemberType : null; } }
    public virtual XmlSchemaType SchemaType { get { return IsReading ? this.schemaInfo.SchemaType : null; } }
    public virtual XmlSchemaElement SchemaElement { get { return IsReading ? this.schemaInfo.SchemaElement : null; } }
    public virtual XmlSchemaAttribute SchemaAttribute { get { return IsReading ? this.schemaInfo.SchemaAttribute : null; } }
  }
}
