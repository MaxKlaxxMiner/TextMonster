using System.CodeDom;
using System.Collections.Specialized;

namespace TextMonster.Xml.Xml_Reader
{
  internal class MappedTypeDesc
  {
    string name;
    string ns;
    XmlSchemaType xsdType;
    XmlSchemaObject context;
    string clrType;
    SchemaImporterExtension extension;
    CodeNamespace code;
    bool exported = false;
    StringCollection references;

    internal MappedTypeDesc(string clrType, string name, string ns, XmlSchemaType xsdType, XmlSchemaObject context, SchemaImporterExtension extension, CodeNamespace code, StringCollection references)
    {
      this.clrType = clrType.Replace('+', '.');
      this.name = name;
      this.ns = ns;
      this.xsdType = xsdType;
      this.context = context;
      this.code = code;
      this.references = references;
      this.extension = extension;
    }

    internal string Name { get { return clrType; } }
  }
}