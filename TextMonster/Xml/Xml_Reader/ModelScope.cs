using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class ModelScope
  {
    TypeScope typeScope;
    Hashtable models = new Hashtable();
    Hashtable arrayModels = new Hashtable();

    internal ModelScope(TypeScope typeScope)
    {
      this.typeScope = typeScope;
    }

    internal TypeScope TypeScope
    {
      get { return typeScope; }
    }

    internal TypeModel GetTypeModel(Type type)
    {
      return GetTypeModel(type, true);
    }

    internal TypeModel GetTypeModel(Type type, bool directReference)
    {
      TypeModel model = (TypeModel)models[type];
      if (model != null) return model;
      TypeDesc typeDesc = typeScope.GetTypeDesc(type, null, directReference);

      switch (typeDesc.Kind)
      {
        case TypeKind.Enum:
          model = new EnumModel(type, typeDesc, this);
          break;
        case TypeKind.Primitive:
          model = new PrimitiveModel(type, typeDesc, this);
          break;
        case TypeKind.Array:
        case TypeKind.Collection:
        case TypeKind.Enumerable:
          model = new ArrayModel(type, typeDesc, this);
          break;
        case TypeKind.Root:
        case TypeKind.Class:
        case TypeKind.Struct:
          model = new StructModel(type, typeDesc, this);
          break;
        default:
          if (!typeDesc.IsSpecial) throw new NotSupportedException(Res.GetString(Res.XmlUnsupportedTypeKind, type.FullName));
          model = new SpecialModel(type, typeDesc, this);
          break;
      }

      models.Add(type, model);
      return model;
    }

    internal ArrayModel GetArrayModel(Type type)
    {
      TypeModel model = (TypeModel)arrayModels[type];
      if (model == null)
      {
        model = GetTypeModel(type);
        if (!(model is ArrayModel))
        {
          TypeDesc typeDesc = typeScope.GetArrayTypeDesc(type);
          model = new ArrayModel(type, typeDesc, this);
        }
        arrayModels.Add(type, model);
      }
      return (ArrayModel)model;
    }
  }
}