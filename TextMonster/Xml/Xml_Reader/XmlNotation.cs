using System;

namespace TextMonster.Xml.Xml_Reader
{
  // Contains a notation declared in the DTD or schema.
  public class XmlNotation : XmlNode
  {
    String publicId;
    String systemId;
    String name;

    internal XmlNotation(String name, String publicId, String systemId, XmlDocument doc)
      : base(doc)
    {
      this.name = doc.NameTable.Add(name);
      this.publicId = publicId;
      this.systemId = systemId;
    }

    // Gets the name of the node.
    public override string Name
    {
      get { return name; }
    }

    // Gets the name of the current node without the namespace prefix.
    public override string LocalName
    {
      get { return name; }
    }

    // Gets the type of the current node.
    public override XmlNodeType NodeType
    {
      get { return XmlNodeType.Notation; }
    }

    // Throws an InvalidOperationException since Notation can not be cloned.
    public override XmlNode CloneNode(bool deep)
    {

      throw new InvalidOperationException(Res.GetString(Res.Xdom_Node_Cloning));
    }

    public override bool IsReadOnly
    {
      get
      {
        return true;        // Make notations readonly
      }
    }

    public override String OuterXml
    {
      get { return String.Empty; }
    }

    public override void WriteTo(XmlWriter w)
    {
    }
  }
}
