using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class StructMapping : TypeMapping, INameScope
  {
    MemberMapping[] members;
    StructMapping baseMapping;
    StructMapping derivedMappings;
    StructMapping nextDerivedMapping;
    MemberMapping xmlnsMember = null;
    bool hasSimpleContent;
    bool openModel;
    bool isSequence;
    NameTableScope elements;
    NameTableScope attributes;
    CodeIdentifiers scope;

    internal StructMapping BaseMapping
    {
      get { return baseMapping; }
      set
      {
        baseMapping = value;
        if (!IsAnonymousType && baseMapping != null)
        {
          nextDerivedMapping = baseMapping.derivedMappings;
          baseMapping.derivedMappings = this;
        }
        if (value.isSequence && !isSequence)
        {
          isSequence = true;
          if (baseMapping.IsSequence)
          {
            for (StructMapping derived = derivedMappings; derived != null; derived = derived.NextDerivedMapping)
            {
              derived.SetSequence();
            }
          }
        }
      }
    }

    internal StructMapping DerivedMappings
    {
      get { return derivedMappings; }
    }

    internal NameTableScope LocalElements
    {
      get
      {
        if (elements == null)
          elements = new NameTableScope();
        return elements;
      }
    }

    object INameScope.this[string name, string ns]
    {
      get
      {
        object named = LocalElements[name, ns];
        if (named != null)
          return named;
        if (baseMapping != null)
          return ((INameScope)baseMapping)[name, ns];
        return null;
      }
      set
      {
        LocalElements[name, ns] = value;
      }
    }
    internal StructMapping NextDerivedMapping
    {
      get { return nextDerivedMapping; }
    }

    internal bool HasSimpleContent
    {
      get { return hasSimpleContent; }
    }

    internal MemberMapping[] Members
    {
      get { return members; }
      set { members = value; }
    }

    internal MemberMapping XmlnsMember
    {
      get { return xmlnsMember; }
    }

    internal bool IsOpenModel
    {
      get { return openModel; }
    }

    internal CodeIdentifiers Scope
    {
      get
      {
        if (scope == null)
          scope = new CodeIdentifiers();
        return scope;
      }
      set { scope = value; }
    }

    internal MemberMapping FindDeclaringMapping(MemberMapping member, out StructMapping declaringMapping, string parent)
    {
      declaringMapping = null;
      if (BaseMapping != null)
      {
        MemberMapping baseMember = BaseMapping.FindDeclaringMapping(member, out declaringMapping, parent);
        if (baseMember != null) return baseMember;
      }
      if (members == null) return null;

      for (int i = 0; i < members.Length; i++)
      {
        if (members[i].Name == member.Name)
        {
          if (members[i].TypeDesc != member.TypeDesc)
            throw new InvalidOperationException(Res.GetString(Res.XmlHiddenMember, parent, member.Name, member.TypeDesc.FullName, this.TypeName, members[i].Name, members[i].TypeDesc.FullName));
          else if (!members[i].Match(member))
          {
            throw new InvalidOperationException(Res.GetString(Res.XmlInvalidXmlOverride, parent, member.Name, this.TypeName, members[i].Name));
          }
          declaringMapping = this;
          return members[i];
        }
      }
      return null;
    }

    internal void SetSequence()
    {
      if (TypeDesc.IsRoot)
        return;

      StructMapping start = this;

      // find first mapping that does not have the sequence set
      while (!start.BaseMapping.IsSequence && start.BaseMapping != null && !start.BaseMapping.TypeDesc.IsRoot)
        start = start.BaseMapping;

      start.IsSequence = true;
      for (StructMapping derived = start.DerivedMappings; derived != null; derived = derived.NextDerivedMapping)
      {
        derived.SetSequence();
      }
    }

    internal bool IsSequence
    {
      get { return isSequence && !TypeDesc.IsRoot; }
      set { isSequence = value; }
    }
  }
}