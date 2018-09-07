using System;

namespace TextMonster.Xml.Xml_Reader
{
  // Represents the content of an XML comment.
  public class XmlComment : XmlCharacterData
  {
    protected internal XmlComment(string comment, XmlDocument doc)
      : base(comment, doc)
    {
    }

    // Gets the name of the node.
    public override String Name
    {
      get { return OwnerDocument.strCommentName; }
    }

    // Gets the name of the current node without the namespace prefix.
    public override String LocalName
    {
      get { return OwnerDocument.strCommentName; }
    }

    // Gets the type of the current node.
    public override XmlNodeType NodeType
    {
      get { return XmlNodeType.Comment; }
    }

    // Creates a duplicate of this node.
    public override XmlNode CloneNode(bool deep)
    {
      return OwnerDocument.CreateComment(Data);
    }

    // Saves the node to the specified XmlWriter.
    public override void WriteTo(XmlWriter w)
    {
      w.WriteComment(Data);
    }
  }
}
