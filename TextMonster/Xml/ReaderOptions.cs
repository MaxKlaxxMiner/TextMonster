using System;

namespace TextMonster.Xml
{
  /// <summary>
  /// Gibt an, ob doppelte Namespaces beim Laden eines <see cref="T:System.Xml.Linq.XDocument"/> mit einem <see cref="T:System.Xml.XmlReader"/> weggelassen werden sollen.
  /// </summary>
  [Flags]
  public enum ReaderOptions
  {
    None = 0,
    OmitDuplicateNamespaces = 1
  }
}
