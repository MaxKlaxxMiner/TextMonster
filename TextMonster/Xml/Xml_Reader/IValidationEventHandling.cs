using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal interface IValidationEventHandling
  {

    // This is a ValidationEventHandler, but it is not strongly typed due to dependencies on System.Xml.Schema
    object EventHandler { get; }

    // The exception is XmlSchemaException, but it is not strongly typed due to dependencies on System.Xml.Schema
    void SendEvent(Exception exception, XmlSeverityType severity);
  }
}
