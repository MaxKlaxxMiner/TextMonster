using System;
using System.Runtime.Serialization;

namespace TextMonster.Xml
{
  [Serializable]
  internal sealed class NameSerializer : IObjectReference, ISerializable
  {
    readonly string expandedName;

    NameSerializer(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException("info");
      expandedName = info.GetString("name");
    }

    object IObjectReference.GetRealObject(StreamingContext context)
    {
      return expandedName;
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      throw new NotSupportedException();
    }
  }
}
