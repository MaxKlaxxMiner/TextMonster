using System;
using System.Collections;
using System.Reflection;

namespace TextMonster.Xml.Xml_Reader
{
  internal class EnumModel : TypeModel
  {
    ConstantModel[] constants;

    internal EnumModel(Type type, TypeDesc typeDesc, ModelScope scope) : base(type, typeDesc, scope) { }

    internal ConstantModel[] Constants
    {
      get
      {
        if (constants == null)
        {
          ArrayList list = new ArrayList();
          FieldInfo[] fields = Type.GetFields();
          for (int i = 0; i < fields.Length; i++)
          {
            FieldInfo field = fields[i];
            ConstantModel constant = GetConstantModel(field);
            if (constant != null) list.Add(constant);
          }
          constants = (ConstantModel[])list.ToArray(typeof(ConstantModel));
        }
        return constants;
      }

    }

    ConstantModel GetConstantModel(FieldInfo fieldInfo)
    {
      if (fieldInfo.IsSpecialName) return null;
      return new ConstantModel(fieldInfo, ((IConvertible)fieldInfo.GetValue(null)).ToInt64(null));
    }
  }
}