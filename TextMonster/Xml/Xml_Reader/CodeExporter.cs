using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
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