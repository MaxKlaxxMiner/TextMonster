using System;

namespace TextMonster.Xml.Xml_Reader
{
  public abstract class XmlSchemaExternal : XmlSchemaObject
  {
    string location;
    Uri baseUri;
    XmlSchema schema;
    string id;
    XmlAttribute[] moreAttributes;
    Compositor compositor;

    [XmlAttribute("schemaLocation", DataType = "anyURI")]
    public string SchemaLocation
    {
      get { return location; }
      set { location = value; }
    }

    [XmlIgnore]
    public XmlSchema Schema
    {
      get { return schema; }
      set { schema = value; }
    }

    [XmlAttribute("id", DataType = "ID")]
    public string Id
    {
      get { return id; }
      set { id = value; }
    }

    [XmlIgnore]
    internal Uri BaseUri
    {
      get { return baseUri; }
      set { baseUri = value; }
    }

    [XmlIgnore]
    internal override string IdAttribute
    {
      get { return Id; }
      set { Id = value; }
    }

    internal override void SetUnhandledAttributes(XmlAttribute[] moreAttributes)
    {
      this.moreAttributes = moreAttributes;
    }

    internal Compositor Compositor
    {
      get
      {
        return compositor;
      }
      set
      {
        compositor = value;
      }
    }
  }
}
