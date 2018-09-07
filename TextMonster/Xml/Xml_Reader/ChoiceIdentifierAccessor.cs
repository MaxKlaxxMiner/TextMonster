using System.Reflection;

namespace TextMonster.Xml.Xml_Reader
{
  internal class ChoiceIdentifierAccessor : Accessor
  {
    string memberName;
    string[] memberIds;
    MemberInfo memberInfo;

    internal string MemberName
    {
      get { return memberName; }
      set { memberName = value; }
    }

    internal string[] MemberIds
    {
      get { return memberIds; }
      set { memberIds = value; }
    }

    internal MemberInfo MemberInfo
    {
      get { return memberInfo; }
    }
  }
}