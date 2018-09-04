namespace TextMonster.Xml.XmlReader
{
  //
  // IDtdAttributeInfo interface
  //
  /// <summary>
  /// Exposes information about an attribute declared in a DTD 
  /// that XmlReader need in order to be able to correctly normalize 
  /// the attribute value according to its data types.
  /// </summary>
  internal interface IDtdAttributeInfo
  {
    /// <summary>
    /// The prefix of the attribute
    /// </summary>
    string Prefix { get; }
    /// <summary>
    /// The local name of the attribute
    /// </summary>
    string LocalName { get; }
    /// <summary>
    /// The line number of the DTD attribute definition
    /// </summary>
    int LineNumber { get; }
    /// <summary>
    /// The line position of the DTD attribute definition
    /// </summary>
    int LinePosition { get; }
    /// <summary>
    /// Returns true if the attribute is of a different type than CDATA
    /// </summary>
    bool IsNonCDataType { get; }
    /// <summary>
    /// Returns true if the attribute was declared in an external DTD subset
    /// </summary>
    bool IsDeclaredInExternal { get; }
    /// <summary>
    /// Returns true if the attribute is xml:space or xml:lang
    /// </summary>
    bool IsXmlAttribute { get; }
  }
}
