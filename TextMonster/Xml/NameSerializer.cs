using System;
using System.Runtime.Serialization;

namespace TextMonster.Xml
{
  [Serializable]
  internal sealed class NameSerializer : IObjectReference, ISerializable
  {
    private string expandedName;

    private NameSerializer(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException("info");
      this.expandedName = info.GetString("name");
    }

    object IObjectReference.GetRealObject(StreamingContext context)
    {
      return (object)XName.Get(this.expandedName);
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      throw new NotSupportedException();
    }
  }
}
