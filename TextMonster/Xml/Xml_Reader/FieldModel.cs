using System;
using System.Reflection;

namespace TextMonster.Xml.Xml_Reader
{
  internal class FieldModel
  {
    SpecifiedAccessor checkSpecified = SpecifiedAccessor.None;
    MemberInfo memberInfo;
    MemberInfo checkSpecifiedMemberInfo;
    MethodInfo checkShouldPersistMethodInfo;
    bool checkShouldPersist;
    bool readOnly = false;
    bool isProperty = false;
    Type fieldType;
    string name;
    TypeDesc fieldTypeDesc;

    internal FieldModel(string name, Type fieldType, TypeDesc fieldTypeDesc, bool checkSpecified, bool checkShouldPersist) :
      this(name, fieldType, fieldTypeDesc, checkSpecified, checkShouldPersist, false)
    {
    }
    internal FieldModel(string name, Type fieldType, TypeDesc fieldTypeDesc, bool checkSpecified, bool checkShouldPersist, bool readOnly)
    {
      this.fieldTypeDesc = fieldTypeDesc;
      this.name = name;
      this.fieldType = fieldType;
      this.checkSpecified = checkSpecified ? SpecifiedAccessor.ReadWrite : SpecifiedAccessor.None;
      this.checkShouldPersist = checkShouldPersist;
      this.readOnly = readOnly;
    }

    internal FieldModel(MemberInfo memberInfo, Type fieldType, TypeDesc fieldTypeDesc)
    {
      this.name = memberInfo.Name;
      this.fieldType = fieldType;
      this.fieldTypeDesc = fieldTypeDesc;
      this.memberInfo = memberInfo;
      this.checkShouldPersistMethodInfo = memberInfo.DeclaringType.GetMethod("ShouldSerialize" + memberInfo.Name, new Type[0]);
      this.checkShouldPersist = this.checkShouldPersistMethodInfo != null;

      FieldInfo specifiedField = memberInfo.DeclaringType.GetField(memberInfo.Name + "Specified");
      if (specifiedField != null)
      {
        if (specifiedField.FieldType != typeof(bool))
        {
          throw new InvalidOperationException(Res.GetString(Res.XmlInvalidSpecifiedType, specifiedField.Name, specifiedField.FieldType.FullName, typeof(bool).FullName));
        }
        this.checkSpecified = specifiedField.IsInitOnly ? SpecifiedAccessor.ReadOnly : SpecifiedAccessor.ReadWrite;
        this.checkSpecifiedMemberInfo = specifiedField;
      }
      else
      {
        PropertyInfo specifiedProperty = memberInfo.DeclaringType.GetProperty(memberInfo.Name + "Specified");
        if (specifiedProperty != null)
        {
          if (StructModel.CheckPropertyRead(specifiedProperty))
          {
            this.checkSpecified = specifiedProperty.CanWrite ? SpecifiedAccessor.ReadWrite : SpecifiedAccessor.ReadOnly;
            this.checkSpecifiedMemberInfo = specifiedProperty;
          }
          if (this.checkSpecified != SpecifiedAccessor.None && specifiedProperty.PropertyType != typeof(bool))
          {
            throw new InvalidOperationException(Res.GetString(Res.XmlInvalidSpecifiedType, specifiedProperty.Name, specifiedProperty.PropertyType.FullName, typeof(bool).FullName));
          }
        }
      }
      if (memberInfo is PropertyInfo)
      {
        readOnly = !((PropertyInfo)memberInfo).CanWrite;
        isProperty = true;
      }
      else if (memberInfo is FieldInfo)
      {
        readOnly = ((FieldInfo)memberInfo).IsInitOnly;
      }
    }

    internal string Name
    {
      get { return name; }
    }

    internal Type FieldType
    {
      get { return fieldType; }
    }

    internal TypeDesc FieldTypeDesc
    {
      get { return fieldTypeDesc; }
    }

    internal bool CheckShouldPersist
    {
      get { return checkShouldPersist; }
    }

    internal SpecifiedAccessor CheckSpecified
    {
      get { return checkSpecified; }
    }

    internal MemberInfo MemberInfo
    {
      get { return memberInfo; }
    }
    internal MemberInfo CheckSpecifiedMemberInfo
    {
      get { return checkSpecifiedMemberInfo; }
    }
    internal MethodInfo CheckShouldPersistMethodInfo
    {
      get { return checkShouldPersistMethodInfo; }
    }

    internal bool ReadOnly
    {
      get { return readOnly; }
    }

    internal bool IsProperty
    {
      get { return isProperty; }
    }
  }
}