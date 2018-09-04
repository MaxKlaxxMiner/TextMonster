namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Exposes information about a general entity 
  /// declared in a DTD that XmlReader need in order to be able
  /// to expand the entity.
  /// </summary>
  internal interface IDtdEntityInfo
  {
    /// <summary>
    /// The name of the entity
    /// </summary>
    string Name { get; }
    /// <summary>
    /// true if the entity is external (its value is in an external input)
    /// </summary>
    bool IsExternal { get; }
    /// <summary>
    /// true if the entity was declared in external DTD subset
    /// </summary>
    bool IsDeclaredInExternal { get; }
    /// <summary>
    /// true if this is an unparsed entity
    /// </summary>
    bool IsUnparsedEntity { get; }
    /// <summary>
    /// true if this is a parameter entity
    /// </summary>
    bool IsParameterEntity { get; }
    /// <summary>
    /// The base URI of the entity value
    /// </summary>
    string BaseUriString { get; }
    /// <summary>
    /// The URI of the XML document where the entity was declared
    /// </summary>
    string DeclaredUriString { get; }
    /// <summary>
    /// SYSTEM identifier (URI) of the entity value - only used for external entities
    /// </summary>
    string SystemId { get; }
    /// <summary>
    /// PUBLIC identifier of the entity value - only used for external entities
    /// </summary>
    string PublicId { get; }
    /// <summary>
    /// Replacement text of an entity. Valid only for internal entities.
    /// </summary>
    string Text { get; }
    /// <summary>
    /// The line number of the entity value
    /// </summary>
    int LineNumber { get; }
    /// <summary>
    /// The line position of the entity value
    /// </summary>
    int LinePosition { get; }
  }
}
