﻿namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\IXmlTextParser.uex' path='docs/doc[@for="IXmlTextParser"]/*' />
  ///<internalonly/>
  /// <devdoc>
  /// <para>This class is <see langword='interface'/> .</para>
  /// </devdoc>
  public interface IXmlTextParser
  {
    /// <include file='doc\IXmlTextParser.uex' path='docs/doc[@for="IXmlTextParser.Normalized"]/*' />
    /// <internalonly/>
    bool Normalized { get; set; }

    /// <include file='doc\IXmlTextParser.uex' path='docs/doc[@for="IXmlTextParser.WhitespaceHandling"]/*' />
    /// <internalonly/>
    WhitespaceHandling WhitespaceHandling { get; set; }
  }
}