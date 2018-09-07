using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace TextMonster.Xml.Xml_Reader
{
  [Serializable]
  public class XmlSchemaValidationException : XmlSchemaException
  {
    protected XmlSchemaValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    [SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
    }

    internal XmlSchemaValidationException(string res, string arg, string sourceUri, int lineNumber, int linePosition) :
      base(res, new[] { arg }, null, sourceUri, lineNumber, linePosition, null)
    {
    }

    internal XmlSchemaValidationException(string res, string[] args, string sourceUri, int lineNumber, int linePosition) :
      base(res, args, null, sourceUri, lineNumber, linePosition, null)
    {
    }

    internal XmlSchemaValidationException(string res, string[] args, Exception innerException, string sourceUri, int lineNumber, int linePosition) :
      base(res, args, innerException, sourceUri, lineNumber, linePosition, null)
    {
    }
  };
}
