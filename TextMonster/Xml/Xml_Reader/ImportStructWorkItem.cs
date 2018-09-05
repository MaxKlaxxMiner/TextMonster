namespace TextMonster.Xml.Xml_Reader
{
  internal class ImportStructWorkItem
  {
    StructModel model;
    StructMapping mapping;

    internal ImportStructWorkItem(StructModel model, StructMapping mapping)
    {
      this.model = model;
      this.mapping = mapping;
    }

    internal StructModel Model { get { return model; } }
    internal StructMapping Mapping { get { return mapping; } }
  }
}