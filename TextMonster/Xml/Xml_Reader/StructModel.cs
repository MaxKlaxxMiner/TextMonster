using System;
using System.Reflection;

namespace TextMonster.Xml.Xml_Reader
{
  internal class StructModel : TypeModel
  {

    internal StructModel(Type type, TypeDesc typeDesc, ModelScope scope) : base(type, typeDesc, scope) { }

    internal MemberInfo[] GetMemberInfos()
    {
      // we use to return Type.GetMembers() here, the members were returned in a different order: fields first, properties last
      // Current System.Reflection code returns members in oposite order: properties first, then fields.
      // This code make sure that returns members in the Everett order.
      MemberInfo[] members = Type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
      MemberInfo[] fieldsAndProps = new MemberInfo[members.Length];

      int cMember = 0;
      // first copy all non-property members over
      for (int i = 0; i < members.Length; i++)
      {
        if ((members[i].MemberType & MemberTypes.Property) == 0)
        {
          fieldsAndProps[cMember++] = members[i];
        }
      }
      // now copy all property members over
      for (int i = 0; i < members.Length; i++)
      {
        if ((members[i].MemberType & MemberTypes.Property) != 0)
        {
          fieldsAndProps[cMember++] = members[i];
        }
      }
      return fieldsAndProps;
    }

    internal FieldModel GetFieldModel(MemberInfo memberInfo)
    {
      FieldModel model = null;
      if (memberInfo is FieldInfo)
        model = GetFieldModel((FieldInfo)memberInfo);
      else if (memberInfo is PropertyInfo)
        model = GetPropertyModel((PropertyInfo)memberInfo);
      if (model != null)
      {
        if (model.ReadOnly && model.FieldTypeDesc.Kind != TypeKind.Collection && model.FieldTypeDesc.Kind != TypeKind.Enumerable)
          return null;
      }
      return model;
    }

    void CheckSupportedMember(TypeDesc typeDesc, MemberInfo member, Type type)
    {
      if (typeDesc == null)
        return;
      if (typeDesc.IsUnsupported)
      {
        if (typeDesc.Exception == null)
        {
          typeDesc.Exception = new NotSupportedException(Res.GetString(Res.XmlSerializerUnsupportedType, typeDesc.FullName));
        }
        throw new InvalidOperationException(Res.GetString(Res.XmlSerializerUnsupportedMember, member.DeclaringType.FullName + "." + member.Name, type.FullName), typeDesc.Exception);

      }
      CheckSupportedMember(typeDesc.BaseTypeDesc, member, type);
      CheckSupportedMember(typeDesc.ArrayElementTypeDesc, member, type);
    }

    FieldModel GetFieldModel(FieldInfo fieldInfo)
    {
      if (fieldInfo.IsStatic) return null;
      if (fieldInfo.DeclaringType != Type) return null;

      TypeDesc typeDesc = ModelScope.TypeScope.GetTypeDesc(fieldInfo.FieldType, fieldInfo, true, false);
      if (fieldInfo.IsInitOnly && typeDesc.Kind != TypeKind.Collection && typeDesc.Kind != TypeKind.Enumerable)
        return null;

      CheckSupportedMember(typeDesc, fieldInfo, fieldInfo.FieldType);
      return new FieldModel(fieldInfo, fieldInfo.FieldType, typeDesc);
    }

    FieldModel GetPropertyModel(PropertyInfo propertyInfo)
    {
      if (propertyInfo.DeclaringType != Type) return null;
      if (CheckPropertyRead(propertyInfo))
      {
        TypeDesc typeDesc = ModelScope.TypeScope.GetTypeDesc(propertyInfo.PropertyType, propertyInfo, true, false);
        // Fix for CSDMain 100492, please contact arssrvlt if you need to change this line
        if (!propertyInfo.CanWrite && typeDesc.Kind != TypeKind.Collection && typeDesc.Kind != TypeKind.Enumerable)
          return null;
        CheckSupportedMember(typeDesc, propertyInfo, propertyInfo.PropertyType);
        return new FieldModel(propertyInfo, propertyInfo.PropertyType, typeDesc);
      }
      return null;
    }

    //CheckProperty
    internal static bool CheckPropertyRead(PropertyInfo propertyInfo)
    {
      if (!propertyInfo.CanRead) return false;

      MethodInfo getMethod = propertyInfo.GetGetMethod();
      if (getMethod.IsStatic) return false;
      ParameterInfo[] parameters = getMethod.GetParameters();
      if (parameters.Length > 0) return false;
      return true;
    }
  }
}