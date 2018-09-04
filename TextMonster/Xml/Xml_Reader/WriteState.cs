namespace TextMonster.Xml.Xml_Reader
{
  // Specifies the state of the XmlWriter.
  public enum WriteState
  {
    // Nothing has been written yet.
    Start,

    // Writing the prolog.
    Prolog,

    // Writing a the start tag for an element.
    Element,

    // Writing an attribute value.
    Attribute,

    // Writing element content.
    Content,

    // XmlWriter is closed; Close has been called.
    Closed,

    // Writer is in error state.
    Error,
  };
}
