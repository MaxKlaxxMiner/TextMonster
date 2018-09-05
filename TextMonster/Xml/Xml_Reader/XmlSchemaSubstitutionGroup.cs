using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class XmlSchemaSubstitutionGroup : XmlSchemaObject
  {
    ArrayList membersList = new ArrayList();
    XmlQualifiedName examplar = XmlQualifiedName.Empty;

    [XmlIgnore]
    internal ArrayList Members
    {
      get { return membersList; }
    }

    [XmlIgnore]
    internal XmlQualifiedName Examplar
    {
      get { return examplar; }
      set { examplar = value; }
    }
  }
}
