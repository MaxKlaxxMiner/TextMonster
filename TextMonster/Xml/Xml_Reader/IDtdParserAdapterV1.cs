namespace TextMonster.Xml.Xml_Reader
{
  internal interface IDtdParserAdapterV1 : IDtdParserAdapterWithValidation
  {
    bool V1CompatibilityMode { get; }
    bool Normalization { get; }
    bool Namespaces { get; }
  }
}
