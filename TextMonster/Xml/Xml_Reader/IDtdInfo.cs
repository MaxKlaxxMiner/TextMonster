using System.Collections.Generic;

namespace TextMonster.Xml.Xml_Reader
{
  //
  // IDtdInfo interface
  //
  /// <summary>
  /// This is an interface for a compiled DTD information. 
  /// It exposes information and functionality that XmlReader need in order to be able
  /// to expand entities, add default attributes and correctly normalize attribute values 
  /// according to their data types.
  /// </summary>
  internal interface IDtdInfo
  {
    /// <summary>
    /// Returns true if the DTD contains any declaration of a default attribute
    /// </summary>
    bool HasDefaultAttributes { get; }

    /// <summary>
    /// Returns true if the DTD contains any declaration of an attribute 
    /// whose type is other than CDATA
    /// </summary>
    bool HasNonCDataAttributes { get; }

    /// <summary>
    /// Looks up a DTD attribute list definition by its name. 
    /// </summary>
    /// <param name="prefix">The prefix of the attribute list to look for</param>
    /// <param name="localName">The local name of the attribute list to look for</param>
    /// <returns>Interface representing an attribute list or null if none was found.</returns>
    IDtdAttributeListInfo LookupAttributeList(string prefix, string localName);

    /// <summary>
    /// Returns an enumerator of all attribute lists defined in the DTD.
    /// </summary>
    IEnumerable<IDtdAttributeListInfo> GetAttributeLists();

    /// <summary>
    /// Looks up a general DTD entity by its name.
    /// </summary>
    /// <param name="name">The name of the entity to look for</param>
    /// <returns>Interface representing an entity or null if none was found.</returns>
    IDtdEntityInfo LookupEntity(string name);
  };
}
