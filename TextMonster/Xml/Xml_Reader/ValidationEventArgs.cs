﻿using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\ValidationEventArgs.uex' path='docs/doc[@for="ValidationEventArgs"]/*' />
  /// <devdoc>
  ///    Returns detailed information relating to
  ///    the ValidationEventhandler.
  /// </devdoc>
  public class ValidationEventArgs : EventArgs
  {
    XmlSchemaException ex;
    XmlSeverityType severity;

    internal ValidationEventArgs(XmlSchemaException ex)
    {
      this.ex = ex;
      severity = XmlSeverityType.Error;
    }

    internal ValidationEventArgs(XmlSchemaException ex, XmlSeverityType severity)
    {
      this.ex = ex;
      this.severity = severity;
    }

    /// <include file='doc\ValidationEventArgs.uex' path='docs/doc[@for="ValidationEventArgs.Severity"]/*' />
    public XmlSeverityType Severity
    {
      get { return severity; }
    }

    /// <include file='doc\ValidationEventArgs.uex' path='docs/doc[@for="ValidationEventArgs.Exception"]/*' />
    public XmlSchemaException Exception
    {
      get { return ex; }
    }

    /// <include file='doc\ValidationEventArgs.uex' path='docs/doc[@for="ValidationEventArgs.Message"]/*' />
    /// <devdoc>
    ///    <para>Gets the text description corresponding to the
    ///       validation error.</para>
    /// </devdoc>
    public String Message
    {
      get { return ex.Message; }
    }
  }
}
