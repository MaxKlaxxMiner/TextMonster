namespace TextMonster.Xml.Xml_Reader
{
  public sealed class XmlSchemaCompilationSettings
  {

    bool enableUpaCheck;

    public XmlSchemaCompilationSettings()
    {
      enableUpaCheck = true;
    }

    public bool EnableUpaCheck
    {
      get
      {
        return enableUpaCheck;
      }
      set
      {
        enableUpaCheck = value;
      }
    }
  }
}
