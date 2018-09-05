namespace TextMonster.Xml.Xml_Reader
{
  internal interface IDtdParserAdapterWithValidation : IDtdParserAdapter
  {
    bool DtdValidation { get; }
    IValidationEventHandling ValidationEventHandling { get; }
  }
}
