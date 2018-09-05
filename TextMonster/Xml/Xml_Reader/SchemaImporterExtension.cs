using System.CodeDom;
using System.CodeDom.Compiler;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\SchemaImporterExtension.uex' path='docs/doc[@for="SchemaImporterExtension"]/*' />
  ///<internalonly/>
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public abstract class SchemaImporterExtension
  {
    /// <include file='doc\SchemaImporterExtension.uex' path='docs/doc[@for="SchemaImporterExtension.ImportSchemaType"]/*' />
    public virtual string ImportSchemaType(string name, string ns, XmlSchemaObject context, XmlSchemas schemas, XmlSchemaImporter importer,
      CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeGenerationOptions options, CodeDomProvider codeProvider)
    {
      return null;
    }

    /// <include file='doc\SchemaImporterExtension.uex' path='docs/doc[@for="SchemaImporterExtension.ImportSchemaType1"]/*' />
    public virtual string ImportSchemaType(XmlSchemaType type, XmlSchemaObject context, XmlSchemas schemas, XmlSchemaImporter importer,
      CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeGenerationOptions options, CodeDomProvider codeProvider)
    {
      return null;
    }

    /// <include file='doc\SchemaImporterExtension.uex' path='docs/doc[@for="SchemaImporterExtension.ImportSchemaType1"]/*' />
    public virtual string ImportAnyElement(XmlSchemaAny any, bool mixed, XmlSchemas schemas, XmlSchemaImporter importer,
      CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeGenerationOptions options, CodeDomProvider codeProvider)
    {
      return null;
    }

    /// <include file='doc\SchemaImporterExtension.uex' path='docs/doc[@for="SchemaImporterExtension.ImportDefaultValue"]/*' />
    public virtual CodeExpression ImportDefaultValue(string value, string type)
    {
      return null;
    }
  }
}