namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Event which is used when a raw writer in a processing pipeline wishes to remove itself from the pipeline and
  /// replace itself with another writer.
  /// </summary>
  internal delegate void OnRemoveWriter(XmlRawWriter writer);
}
