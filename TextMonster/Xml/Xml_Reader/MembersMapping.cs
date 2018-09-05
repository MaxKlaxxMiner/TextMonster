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
      set { xmlnsMember = value; }
    }

    internal bool HasWrapperElement
    {
      get { return hasWrapperElement; }
      set { hasWrapperElement = value; }
    }

    internal bool ValidateRpcWrapperElement
    {
      get { return validateRpcWrapperElement; }
      set { validateRpcWrapperElement = value; }
    }

    internal bool WriteAccessors
    {
      get { return writeAccessors; }
      set { writeAccessors = value; }
    }
  }
}