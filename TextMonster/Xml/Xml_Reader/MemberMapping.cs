using System.Reflection;

namespace TextMonster.Xml.Xml_Reader
{
  internal class MemberMapping : AccessorMapping
  {
    string name;
    bool checkShouldPersist;
    SpecifiedAccessor checkSpecified;
    bool isReturnValue;
    bool readOnly = false;
    int sequenceId = -1;
    MemberInfo memberInfo;
    MemberInfo checkSpecifiedMemberInfo;
    MethodInfo checkShouldPersistMethodInfo;

    internal MemberMapping() { }

    internal bool CheckShouldPersist
    {
      get { return checkShouldPersist; }
    }

    internal SpecifiedAccessor CheckSpecified
    {
      get { return checkSpecified; }
      set { checkSpecified = value; }
    }

    internal string Name
    {
      get { return name == null ? string.Empty : name; }
      set { name = value; }
    }
  }
}