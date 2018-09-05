using System;
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

    internal StringCollection ReferencedAssemblies
    {
      get
      {
        if (references == null)
          references = new StringCollection();
        return references;
      }
    }

    internal CodeTypeDeclaration ExportTypeDefinition(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit)
    {
      if (exported)
        return null;
      exported = true;

      foreach (CodeNamespaceImport import in code.Imports)
      {
        codeNamespace.Imports.Add(import);
      }
      CodeTypeDeclaration codeClass = null;
      string comment = Res.GetString(Res.XmlExtensionComment, extension.GetType().FullName);
      foreach (CodeTypeDeclaration type in code.Types)
      {
        if (clrType == type.Name)
        {
          if (codeClass != null)
            throw new InvalidOperationException(Res.GetString(Res.XmlExtensionDuplicateDefinition, extension.GetType().FullName, clrType));
          codeClass = type;
        }
        type.Comments.Add(new CodeCommentStatement(comment, false));
        codeNamespace.Types.Add(type);
      }
      if (codeCompileUnit != null)
      {
        foreach (string reference in ReferencedAssemblies)
        {
          if (codeCompileUnit.ReferencedAssemblies.Contains(reference))
            continue;
          codeCompileUnit.ReferencedAssemblies.Add(reference);
        }
      }
      return codeClass;
    }
  }
}