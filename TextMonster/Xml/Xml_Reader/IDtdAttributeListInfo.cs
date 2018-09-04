using System.Collections.Generic;

namespace TextMonster.Xml.Xml_Reader
{
  //
  // IDtdAttributeListInfo interface
  //
  /// <summary>
  /// Exposes information about attributes declared in an attribute list in a DTD 
  /// that XmlReader need in order to be able to add default attributes 
  /// and correctly normalize attribute values according to their data types.
  /// </summary>
  internal interface IDtdAttributeListInfo
  {
    /// <summary>
    /// Prefix of an element this attribute list belongs to.
    /// </summary>
    string Prefix { get; }
    /// <summary>
    /// Local name of an element this attribute list belongs to.
    /// </summary>
    string LocalName { get; }
    /// <summary>
    /// Returns true if the attribute list has some declared attributes with
    /// type other than CDATA.
    /// </summary>
    bool HasNonCDataAttributes { get; }
    /// <summary>
    /// Looks up a DTD attribute definition by its name.
    /// </summary>
    /// <param name="prefix">The prefix of the attribute to look for</param>
    /// <param name="localName">The local name of the attribute to look for</param>
    /// <returns>Interface representing an attribute or null is none was found</returns>
    IDtdAttributeInfo LookupAttribute(string prefix, string localName);
    /// <summary>
    /// Returns enumeration of all default attributes
    /// defined in this attribute list.
    /// </summary>
    /// <returns>Enumerator of default attribute.</returns>
    IEnumerable<IDtdDefaultAttributeInfo> LookupDefaultAttributes();
    /// <summary>
    /// Looks up a ID attribute defined in the attribute list. Returns
    /// null if the attribute list does define an ID attribute.
    /// </summary>
    IDtdAttributeInfo LookupIdAttribute();
  }
}
