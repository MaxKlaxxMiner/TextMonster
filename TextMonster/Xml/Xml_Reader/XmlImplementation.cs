namespace TextMonster.Xml.Xml_Reader
{
  // Provides methods for performing operations that are independent of any
  // particular instance of the document object model.
  public class XmlImplementation
  {

    private NameTable nameTable;

    // Initializes a new instance of the XmlImplementation class.
    public XmlImplementation()
      : this(new NameTable())
    {
    }

    public XmlImplementation(NameTable nt)
    {
      nameTable = nt;
    }

    public virtual XmlDocument CreateDocument()
    {
      return new XmlDocument(this);
    }

    internal NameTable NameTable
    {
      get { return nameTable; }
    }
  }
}
