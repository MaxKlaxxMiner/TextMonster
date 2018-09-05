using System;
using System.Xml;

namespace TextMonster.Xml.Xml_Reader
{
  public class XmlNodeChangedEventArgs : EventArgs
  {
    private XmlNodeChangedAction action;
    private XmlNode node;
    private XmlNode oldParent;
    private XmlNode newParent;
    private string oldValue;
    private string newValue;

    public XmlNodeChangedEventArgs(XmlNode node, XmlNode oldParent, XmlNode newParent, string oldValue, string newValue, XmlNodeChangedAction action)
    {
      this.node = node;
      this.oldParent = oldParent;
      this.newParent = newParent;
      this.action = action;
      this.oldValue = oldValue;
      this.newValue = newValue;
    }

    public XmlNodeChangedAction Action { get { return action; } }

    public XmlNode Node { get { return node; } }
  }
}
