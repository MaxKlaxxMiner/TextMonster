using System;
using System.Reflection;

namespace TextMonster.Xml.Xml_Reader
{
  internal class StructModel : TypeModel
  {

    internal StructModel(Type type, TypeDesc typeDesc, ModelScope scope) : base(type, typeDesc, scope) { }

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