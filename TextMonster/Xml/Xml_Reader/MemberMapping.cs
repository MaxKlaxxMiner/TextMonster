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

    MemberMapping(MemberMapping mapping)
      : base(mapping)
    {
      this.name = mapping.name;
      this.checkShouldPersist = mapping.checkShouldPersist;
      this.checkSpecified = mapping.checkSpecified;
      this.isReturnValue = mapping.isReturnValue;
      this.readOnly = mapping.readOnly;
      this.sequenceId = mapping.sequenceId;
      this.memberInfo = mapping.memberInfo;
      this.checkSpecifiedMemberInfo = mapping.checkSpecifiedMemberInfo;
      this.checkShouldPersistMethodInfo = mapping.checkShouldPersistMethodInfo;
    }

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

    internal MemberInfo MemberInfo
    {
      get { return memberInfo; }
      set { memberInfo = value; }
    }

    internal MemberInfo CheckSpecifiedMemberInfo
    {
      get { return checkSpecifiedMemberInfo; }
    }

    internal MemberMapping Clone()
    {
      return new MemberMapping(this);
    }
  }
}