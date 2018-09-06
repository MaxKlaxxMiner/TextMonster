using System;
using System.IO;
using System.Runtime.Versioning;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlResolver.uex' path='docs/doc[@for="XmlResolver"]/*' />
  /// <devdoc>
  ///    <para>Resolves external XML resources named by a Uniform
  ///       Resource Identifier (URI). This class is <see langword='abstract'/>
  ///       .</para>
  /// </devdoc>
  public abstract partial class XmlResolver
  {
    /// <include file='doc\XmlResolver.uex' path='docs/doc[@for="XmlResolver.GetEntity1"]/*' />
    /// <devdoc>
    ///    <para>Maps a
    ///       URI to an Object containing the actual resource.</para>
    /// </devdoc>

    public abstract Object GetEntity(Uri absoluteUri,
                                     string role,
                                     Type ofObjectToReturn);



    /// <include file='doc\XmlResolver.uex' path='docs/doc[@for="XmlResolver.ResolveUri"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    public virtual Uri ResolveUri(Uri baseUri, string relativeUri)
    {
      if (baseUri == null || (!baseUri.IsAbsoluteUri && baseUri.OriginalString.Length == 0))
      {
        Uri uri = new Uri(relativeUri, UriKind.RelativeOrAbsolute);
        if (!uri.IsAbsoluteUri && uri.OriginalString.Length > 0)
        {
          uri = new Uri(Path.GetFullPath(relativeUri));
        }
        return uri;
      }
      else
      {
        if (relativeUri == null || relativeUri.Length == 0)
        {
          return baseUri;
        }
        // relative base Uri
        if (!baseUri.IsAbsoluteUri)
        {
          throw new NotSupportedException(Res.GetString(Res.Xml_RelativeUriNotSupported));
        }
        return new Uri(baseUri, relativeUri);
      }
    }

    public virtual bool SupportsType(Uri absoluteUri, Type type)
    {
      if (absoluteUri == null)
      {
        throw new ArgumentNullException("absoluteUri");
      }
      if (type == null || type == typeof(Stream))
      {
        return true;
      }
      return false;
    }
  }
}
