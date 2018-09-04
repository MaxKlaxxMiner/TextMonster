namespace TextMonster.Xml.XmlReader
{
  /// <summary>
  /// This interface is implemented by writers which wish to remove themselves from the processing pipeline once they
  /// have accomplished some work.  An example would be the auto-detect writer, which removes itself from the pipeline
  /// once it has determined whether to use the Xml or the Html output mode.
  /// </summary>
  internal interface IRemovableWriter
  {
    OnRemoveWriter OnRemoveWriterEvent { get; set; }
  }
}