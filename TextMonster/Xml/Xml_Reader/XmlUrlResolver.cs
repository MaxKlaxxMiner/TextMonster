using System;
using System.Net;
using System.Net.Cache;
using System.Runtime.Versioning;
using System.Security.Permissions;
using System.Threading;

namespace TextMonster.Xml.Xml_Reader
{
  // Resolves external XML resources named by a Uniform Resource Identifier (URI).
  public partial class XmlUrlResolver : XmlResolver
  {
    private static object s_DownloadManager;
    private ICredentials _credentials;
    private IWebProxy _proxy;
    private RequestCachePolicy _cachePolicy;

    static XmlDownloadManager DownloadManager
    {
      get
      {
        if (s_DownloadManager == null)
        {
          object dm = new XmlDownloadManager();
          Interlocked.CompareExchange<object>(ref s_DownloadManager, dm, null);
        }
        return (XmlDownloadManager)s_DownloadManager;
      }
    }

    // Construction

    // Creates a new instance of the XmlUrlResolver class.
    public XmlUrlResolver()
    {
    }

    public override ICredentials Credentials
    {
      set { _credentials = value; }
    }

    public IWebProxy Proxy
    {
      set { _proxy = value; }
    }

    public RequestCachePolicy CachePolicy
    {
      set { _cachePolicy = value; }
    }

    // Resource resolution

    // Maps a URI to an Object containing the actual resource.
    [ResourceConsumption(ResourceScope.Machine)]
    [ResourceExposure(ResourceScope.Machine)]
    public override Object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
    {
      if (ofObjectToReturn == null || ofObjectToReturn == typeof(System.IO.Stream) || ofObjectToReturn == typeof(System.Object))
      {
        return DownloadManager.GetStream(absoluteUri, _credentials, _proxy, _cachePolicy);
      }
      else
      {
        throw new XmlException(Res.Xml_UnsupportedClass, string.Empty);
      }
    }

    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    [ResourceConsumption(ResourceScope.Machine)]
    [ResourceExposure(ResourceScope.Machine)]
    public override Uri ResolveUri(Uri baseUri, string relativeUri)
    {
      return base.ResolveUri(baseUri, relativeUri);
    }
  }
}
