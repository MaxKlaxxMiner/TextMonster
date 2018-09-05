using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Security.Permissions;

namespace TextMonster.Xml.Xml_Reader
{
  [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
  public abstract class CodeExporter
  {
    Hashtable exportedMappings;
    Hashtable exportedClasses; // TypeMapping -> CodeTypeDeclaration
    CodeNamespace codeNamespace;
    CodeCompileUnit codeCompileUnit;
    bool rootExported;
    TypeScope scope;
    CodeGenerationOptions options;
    CodeDomProvider codeProvider;
    CodeAttributeDeclaration generatedCodeAttribute;

    internal CodeExporter(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeDomProvider codeProvider, CodeGenerationOptions options, Hashtable exportedMappings)
    {
      if (codeNamespace != null)
        CodeGenerator.ValidateIdentifiers(codeNamespace);
      this.codeNamespace = codeNamespace;
      if (codeCompileUnit != null)
      {
        if (!codeCompileUnit.ReferencedAssemblies.Contains("System.dll"))
          codeCompileUnit.ReferencedAssemblies.Add("System.dll");
        if (!codeCompileUnit.ReferencedAssemblies.Contains("dll"))
          codeCompileUnit.ReferencedAssemblies.Add("dll");
      }
      this.codeCompileUnit = codeCompileUnit;
      this.options = options;
      this.exportedMappings = exportedMappings;
      this.codeProvider = codeProvider;
    }

    internal CodeCompileUnit CodeCompileUnit
    {
      get { return codeCompileUnit; }
    }

    internal CodeNamespace CodeNamespace
    {
      get
      {
        if (codeNamespace == null)
          codeNamespace = new CodeNamespace();
        return codeNamespace;
      }
    }
    internal CodeDomProvider CodeProvider
    {
      get
      {
        if (codeProvider == null)
          codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
        return codeProvider;
      }
    }

    internal Hashtable ExportedClasses
    {
      get
      {
        if (exportedClasses == null)
          exportedClasses = new Hashtable();
        return exportedClasses;
      }
    }

    internal Hashtable ExportedMappings
    {
      get
      {
        if (exportedMappings == null)
          exportedMappings = new Hashtable();
        return exportedMappings;
      }
    }

    internal bool GenerateProperties
    {
      get { return (options & CodeGenerationOptions.GenerateProperties) != 0; }
    }

    internal CodeAttributeDeclaration GeneratedCodeAttribute
    {
      get
      {
        if (generatedCodeAttribute == null)
        {
          CodeAttributeDeclaration decl = new CodeAttributeDeclaration(typeof(GeneratedCodeAttribute).FullName);
          Assembly a = Assembly.GetEntryAssembly();
          if (a == null)
          {
            a = Assembly.GetExecutingAssembly();
            if (a == null)
            {
              a = typeof(CodeExporter).Assembly;
            }
          }
          AssemblyName assemblyName = a.GetName();
          decl.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(assemblyName.Name)));
          string version = GetProductVersion(a);
          decl.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(version == null ? assemblyName.Version.ToString() : version)));
          generatedCodeAttribute = decl;
        }
        return generatedCodeAttribute;
      }
    }

    internal static CodeAttributeDeclaration FindAttributeDeclaration(Type type, CodeAttributeDeclarationCollection metadata)
    {
      foreach (CodeAttributeDeclaration attribute in metadata)
      {
        if (attribute.Name == type.FullName || attribute.Name == type.Name)
        {
          return attribute;
        }
      }
      return null;
    }

    private static string GetProductVersion(Assembly assembly)
    {
      object[] attributes = assembly.GetCustomAttributes(true);
      for (int i = 0; i < attributes.Length; i++)
      {
        if (attributes[i] is AssemblyInformationalVersionAttribute)
        {
          AssemblyInformationalVersionAttribute version = (AssemblyInformationalVersionAttribute)attributes[i];
          return version.InformationalVersion;
        }
      }
      return null;
    }

    internal static CodeMemberMethod RaisePropertyChangedEventMethod
    {
      get
      {
        CodeMemberMethod raisePropertyChangedEventMethod = new CodeMemberMethod();
        raisePropertyChangedEventMethod.Name = "RaisePropertyChanged";
        raisePropertyChangedEventMethod.Attributes = MemberAttributes.Family | MemberAttributes.Final;
        CodeArgumentReferenceExpression propertyName = new CodeArgumentReferenceExpression("propertyName");
        raisePropertyChangedEventMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), propertyName.ParameterName));
        CodeVariableReferenceExpression propertyChanged = new CodeVariableReferenceExpression("propertyChanged");
        raisePropertyChangedEventMethod.Statements.Add(new CodeVariableDeclarationStatement(typeof(PropertyChangedEventHandler), propertyChanged.VariableName, new CodeEventReferenceExpression(new CodeThisReferenceExpression(), PropertyChangedEvent.Name)));
        CodeConditionStatement ifStatement = new CodeConditionStatement(new CodeBinaryOperatorExpression(propertyChanged, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)));
        raisePropertyChangedEventMethod.Statements.Add(ifStatement);
        ifStatement.TrueStatements.Add(new CodeDelegateInvokeExpression(propertyChanged, new CodeThisReferenceExpression(), new CodeObjectCreateExpression(typeof(PropertyChangedEventArgs), propertyName)));
        return raisePropertyChangedEventMethod;
      }
    }

    internal static CodeMemberEvent PropertyChangedEvent
    {
      get
      {
        CodeMemberEvent propertyChangedEvent = new CodeMemberEvent();
        propertyChangedEvent.Attributes = MemberAttributes.Public;
        propertyChangedEvent.Name = "PropertyChanged";
        propertyChangedEvent.Type = new CodeTypeReference(typeof(PropertyChangedEventHandler));
        propertyChangedEvent.ImplementationTypes.Add(typeof(System.ComponentModel.INotifyPropertyChanged));
        return propertyChangedEvent;
      }
    }

  }
}