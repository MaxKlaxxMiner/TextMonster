using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class XmlSchemaCollectionNode
  {
    private String namespaceUri;
    private SchemaInfo schemaInfo;
    private XmlSchema schema;

    internal String NamespaceURI
    {
      get { return namespaceUri; }
      set { namespaceUri = value; }
    }

    internal SchemaInfo SchemaInfo
    {
      get { return schemaInfo; }
      set { schemaInfo = value; }
    }

    internal XmlSchema Schema
    {
      get { return schema; }
      set { schema = value; }
    }
  }
}
