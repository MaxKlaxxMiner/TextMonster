namespace TextMonster.Xml.Xml_Reader
{
  internal class MembersMapping : TypeMapping
  {
    MemberMapping[] members;
    bool hasWrapperElement = true;
    bool validateRpcWrapperElement;
    bool writeAccessors = true;
    MemberMapping xmlnsMember = null;

    internal MemberMapping[] Members
    {
      get { return members; }
      set { members = value; }
    }

    internal MemberMapping XmlnsMember
    {
      get { return xmlnsMember; }
    }

    internal bool HasWrapperElement
    {
      set { hasWrapperElement = value; }
    }
  }
}