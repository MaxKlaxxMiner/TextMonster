using System.CodeDom.Compiler;
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
      set { checkShouldPersist = value; }
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
      set { checkSpecifiedMemberInfo = value; }
    }

    internal MethodInfo CheckShouldPersistMethodInfo
    {
      set { checkShouldPersistMethodInfo = value; }
    }

    internal bool IsReturnValue
    {
      set { isReturnValue = value; }
    }

    internal bool ReadOnly
    {
      set { readOnly = value; }
    }

    internal bool IsSequence
    {
      get { return sequenceId >= 0; }
    }

    internal int SequenceId
    {
      get { return sequenceId; }
      set { sequenceId = value; }
    }

    string GetNullableType(TypeDesc td)
    {
      // SOAP encoded arrays not mapped to Nullable<T> since they always derive from soapenc:Array
      if (td.IsMappedType || (!td.IsValueType && (Elements[0].IsSoap || td.ArrayElementTypeDesc == null)))
        return td.FullName;
      if (td.ArrayElementTypeDesc != null)
      {
        return GetNullableType(td.ArrayElementTypeDesc) + "[]";
      }
      return "System.Nullable`1[" + td.FullName + "]";
    }

    internal MemberMapping Clone()
    {
      return new MemberMapping(this);
    }

    internal string GetTypeName(CodeDomProvider codeProvider)
    {
      if (IsNeedNullable && codeProvider.Supports(GeneratorSupport.GenericTypeReference))
      {
        return GetNullableType(TypeDesc);
      }
      return TypeDesc.FullName;
    }
  }
}