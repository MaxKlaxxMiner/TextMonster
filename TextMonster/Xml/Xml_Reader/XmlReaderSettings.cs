using System;
using System.IO;

namespace TextMonster.Xml.Xml_Reader
{
  public sealed class XmlReaderSettings
  {
    // Nametable
    XmlNameTable nameTable;

    // XmlResolver
    XmlResolver xmlResolver;

    // Text settings
    int lineNumberOffset;
    int linePositionOffset;

    // Conformance settings
    ConformanceLevel conformanceLevel;
    bool checkCharacters;

    long maxCharactersInDocument;
    long maxCharactersFromEntities;

    // Filtering settings
    bool ignoreWhitespace;
    bool ignorePIs;
    bool ignoreComments;

    //Validation settings
    ValidationType validationType;
    XmlSchemaValidationFlags validationFlags;
    XmlSchemaSet schemas;
    ValidationEventHandler valEventHandler;

    // other settings
    bool closeInput;

    // read-only flag
    bool isReadOnly;

    //
    // Constructor
    //
    public XmlReaderSettings()
    {
      Initialize();
    }

    public XmlNameTable NameTable
    {
      get
      {
        return nameTable;
      }
      set
      {
        CheckReadOnly("NameTable");
        nameTable = value;
      }
    }

    // XmlResolver
    internal bool IsXmlResolverSet
    {
      get;
      set; // keep set internal as we need to call it from the schema validation code
    }

    public XmlResolver XmlResolver
    {
      set
      {
        CheckReadOnly("XmlResolver");
        xmlResolver = value;
        IsXmlResolverSet = true;
      }
    }

    internal XmlResolver GetXmlResolver()
    {
      return xmlResolver;
    }

    //This is used by get XmlResolver in Xsd.
    //Check if the config set to prohibit default resovler
    //notice we must keep GetXmlResolver() to avoid dead lock when init System.Config.ConfigurationManager
    internal XmlResolver GetXmlResolver_CheckConfig()
    {
      return null;
    }

    // Text settings
    public int LineNumberOffset
    {
      get
      {
        return lineNumberOffset;
      }
      set
      {
        CheckReadOnly("LineNumberOffset");
        lineNumberOffset = value;
      }
    }

    public int LinePositionOffset
    {
      get
      {
        return linePositionOffset;
      }
      set
      {
        CheckReadOnly("LinePositionOffset");
        linePositionOffset = value;
      }
    }

    // Conformance settings
    public ConformanceLevel ConformanceLevel
    {
      get
      {
        return conformanceLevel;
      }
      set
      {
        CheckReadOnly("ConformanceLevel");

        if ((uint)value > (uint)ConformanceLevel.Document)
        {
          throw new ArgumentOutOfRangeException("value");
        }
        conformanceLevel = value;
      }
    }

    public bool CheckCharacters
    {
      get
      {
        return checkCharacters;
      }
      set
      {
        CheckReadOnly("CheckCharacters");
        checkCharacters = value;
      }
    }

    public long MaxCharactersInDocument
    {
      get
      {
        return maxCharactersInDocument;
      }
      set
      {
        CheckReadOnly("MaxCharactersInDocument");
        if (value < 0)
        {
          throw new ArgumentOutOfRangeException("value");
        }
        maxCharactersInDocument = value;
      }
    }

    public long MaxCharactersFromEntities
    {
      get
      {
        return maxCharactersFromEntities;
      }
      set
      {
        CheckReadOnly("MaxCharactersFromEntities");
        if (value < 0)
        {
          throw new ArgumentOutOfRangeException("value");
        }
        maxCharactersFromEntities = value;
      }
    }

    public bool CloseInput
    {
      get
      {
        return closeInput;
      }
      set
      {
        CheckReadOnly("CloseInput");
        closeInput = value;
      }
    }

    public ValidationType ValidationType
    {
      get
      {
        return validationType;
      }
      set
      {
        CheckReadOnly("ValidationType");

        if ((uint)value > (uint)ValidationType.Schema)
        {
          throw new ArgumentOutOfRangeException("value");
        }
        validationType = value;
      }
    }

    public XmlSchemaValidationFlags ValidationFlags
    {
      get
      {
        return validationFlags;
      }
      set
      {
        CheckReadOnly("ValidationFlags");

        if ((uint)value > (uint)(XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation |
                                   XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessIdentityConstraints |
                                   XmlSchemaValidationFlags.AllowXmlAttributes))
        {
          throw new ArgumentOutOfRangeException("value");
        }
        validationFlags = value;
      }
    }

    public XmlSchemaSet Schemas
    {
      get
      {
        if (schemas == null)
        {
          schemas = new XmlSchemaSet();
        }
        return schemas;
      }
      set
      {
        CheckReadOnly("Schemas");
        schemas = value;
      }
    }

    public event ValidationEventHandler ValidationEventHandler
    {
      add
      {
        CheckReadOnly("ValidationEventHandler");
        valEventHandler += value;
      }
      remove
      {
        CheckReadOnly("ValidationEventHandler");
        valEventHandler -= value;
      }
    }

    public XmlReaderSettings Clone()
    {
      XmlReaderSettings clonedSettings = this.MemberwiseClone() as XmlReaderSettings;
      clonedSettings.ReadOnly = false;
      return clonedSettings;
    }

    //
    // Internal methods
    //
    internal ValidationEventHandler GetEventHandler()
    {
      return valEventHandler;
    }


    internal FastXmlReader CreateReader(Stream input, Uri baseUri, string baseUriString, XmlParserContext inputContext)
    {
      if (input == null)
      {
        throw new ArgumentNullException("input");
      }
      if (baseUriString == null)
      {
        if (baseUri == null)
        {
          baseUriString = string.Empty;
        }
        else
        {
          baseUriString = baseUri.ToString();
        }
      }

      // create text XML reader
      FastXmlReader reader = new XmlTextReaderImpl(input, null, 0, this, baseUri, baseUriString, inputContext, closeInput);

      // wrap with validating reader
      if (this.ValidationType != ValidationType.None)
      {
        reader = AddValidation(reader);
      }

      return reader;
    }

    internal FastXmlReader CreateReader(TextReader input, string baseUriString, XmlParserContext inputContext)
    {
      if (input == null)
      {
        throw new ArgumentNullException("input");
      }
      if (baseUriString == null)
      {
        baseUriString = string.Empty;
      }

      // create xml text reader
      FastXmlReader reader = new XmlTextReaderImpl(input, this, baseUriString, inputContext);

      // wrap with validating reader
      if (this.ValidationType != ValidationType.None)
      {
        reader = AddValidation(reader);
      }

      return reader;
    }

    internal bool ReadOnly
    {
      set
      {
        isReadOnly = value;
      }
    }

    void CheckReadOnly(string propertyName)
    {
      if (isReadOnly)
      {
        throw new XmlException(Res.Xml_ReadOnlyProperty, this.GetType().Name + '.' + propertyName);
      }
    }

    //
    // Private methods
    //
    void Initialize()
    {
      Initialize(null);
    }

    void Initialize(XmlResolver resolver)
    {
      nameTable = null;
      if (!EnableLegacyXmlSettings())
      {
        xmlResolver = resolver;
        // limit the entity resolving to 10 million character. the caller can still
        // override it to any other value or set it to zero for unlimiting it
        maxCharactersFromEntities = (long)1e7;
      }
      else
      {
        xmlResolver = (resolver == null ? CreateDefaultResolver() : resolver);
        maxCharactersFromEntities = 0;
      }
      lineNumberOffset = 0;
      linePositionOffset = 0;
      checkCharacters = true;
      conformanceLevel = ConformanceLevel.Document;

      ignoreWhitespace = false;
      ignorePIs = false;
      ignoreComments = false;
      closeInput = false;

      maxCharactersInDocument = 0;

      schemas = null;
      validationType = ValidationType.None;
      validationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints;
      validationFlags |= XmlSchemaValidationFlags.AllowXmlAttributes;

      isReadOnly = false;
      IsXmlResolverSet = false;
    }

    static XmlResolver CreateDefaultResolver()
    {
      return new XmlUrlResolver();
    }

    internal FastXmlReader AddValidation(FastXmlReader reader)
    {
      if (this.validationType == ValidationType.Schema)
      {
        XmlResolver resolver = GetXmlResolver_CheckConfig();

        if (resolver == null &&
            !this.IsXmlResolverSet &&
            !EnableLegacyXmlSettings())
        {
          resolver = new XmlUrlResolver();
        }
        reader = new XsdValidatingReader(reader, resolver, this);
      }
      else if (this.validationType == ValidationType.DTD)
      {
        reader = CreateDtdValidatingReader(reader);
      }
      return reader;
    }

    private XmlValidatingReaderImpl CreateDtdValidatingReader(FastXmlReader baseReader)
    {
      return new XmlValidatingReaderImpl(baseReader, this.GetEventHandler(), (this.ValidationFlags & XmlSchemaValidationFlags.ProcessIdentityConstraints) != 0);
    }

    private static bool? s_enableLegacyXmlSettings = null;

    static internal bool EnableLegacyXmlSettings()
    {
      if (s_enableLegacyXmlSettings.HasValue)
      {
        return s_enableLegacyXmlSettings.Value;
      }

      s_enableLegacyXmlSettings = true;
      return s_enableLegacyXmlSettings.Value;
    }
  }
}
