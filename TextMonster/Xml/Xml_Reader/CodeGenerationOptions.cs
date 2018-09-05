using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\CodeGenerationOptions.uex' path='docs/doc[@for="CodeGenerationOptions"]/*' />
  /// <devdoc>
  ///    Specifies varoius flavours of XmlCodeExporter generated code.
  /// </devdoc>
  [Flags]
  public enum CodeGenerationOptions
  {
    /// <include file='doc\CodeGenerationOptions.uex' path='docs/doc[@for="CodeGenerationOptions.GenerateProperties"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Generate propertyes instead of fields.
    ///    </para>
    /// </devdoc>
    [XmlEnum("properties")]
    GenerateProperties = 0x1,

    /// <include file='doc\CodeGenerationOptions.uex' path='docs/doc[@for="CodeGenerationOptions.GenerateOldAsync"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Generate old asynchronous pattern: BeginXXX/EndXXX.
    ///    </para>
    /// </devdoc>
    [XmlEnum("oldAsync")]
    GenerateOldAsync = 0x4,


    /// <include file='doc\CodeGenerationOptions.uex' path='docs/doc[@for="CodeGenerationOptions.GenerateOrder"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Generate OM using explicit ordering feature.
    ///    </para>
    /// </devdoc>
    [XmlEnum("order")]
    GenerateOrder = 0x08,

    /// <include file='doc\CodeGenerationOptions.uex' path='docs/doc[@for="CodeGenerationOptions.EnableDataBinding"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Generate OM INotifyPropertyChanged interface to enable data binding.
    ///    </para>
    /// </devdoc>
    [XmlEnum("enableDataBinding")]
    EnableDataBinding = 0x10,
  }
}