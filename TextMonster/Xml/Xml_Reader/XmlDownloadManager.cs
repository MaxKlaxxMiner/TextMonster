using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.Versioning;

namespace TextMonster.Xml.Xml_Reader
{
  //
  // XmlDownloadManager
  //
  internal partial class XmlDownloadManager
  {
    Hashtable connections;

    [ResourceConsumption(ResourceScope.Machine)]
    [ResourceExposure(ResourceScope.Machine)]
    internal Stream GetStream(Uri uri, ICredentials credentials, IWebProxy proxy,
        RequestCachePolicy cachePolicy)
    {
      if (uri.Scheme == "file")
      {
        return new FileStream(uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1);
      }
      else
      {
        return GetNonFileStream(uri, credentials, proxy, cachePolicy);
      }
    }

    private Stream GetNonFileStream(Uri uri, ICredentials credentials, IWebProxy proxy,
        RequestCachePolicy cachePolicy)
    {
      WebRequest req = WebRequest.Create(uri);
      if (credentials != null)
      {
        req.Credentials = credentials;
      }
      if (proxy != null)
      {
        req.Proxy = proxy;
      }
      if (cachePolicy != null)
      {
        req.CachePolicy = cachePolicy;
      }
      WebResponse resp = req.GetResponse();
      HttpWebRequest webReq = req as HttpWebRequest;
      if (webReq != null)
      {
        lock (this)
        {
          if (connections == null)
          {
            connections = new Hashtable();
          }
          OpenedHost openedHost = (OpenedHost)connections[webReq.Address.Host];
          if (openedHost == null)
          {
            openedHost = new OpenedHost();
          }

          if (openedHost.nonCachedConnectionsCount < webReq.ServicePoint.ConnectionLimit - 1)
          {
            // we are not close to connection limit -> don't cache the stream
            if (openedHost.nonCachedConnectionsCount == 0)
            {
              connections.Add(webReq.Address.Host, openedHost);
            }
            openedHost.nonCachedConnectionsCount++;
            return new XmlRegisteredNonCachedStream(resp.GetResponseStream(), this, webReq.Address.Host);
          }
          else
          {
            // cache the stream and save the connection for the next request
            return new XmlCachedStream(resp.ResponseUri, resp.GetResponseStream());
          }
        }
      }
      else
      {
        return resp.GetResponseStream();
      }
    }

    internal void Remove(string host)
    {
      lock (this)
      {
        OpenedHost openedHost = (OpenedHost)connections[host];
        if (openedHost != null)
        {
          if (--openedHost.nonCachedConnectionsCount == 0)
          {
            connections.Remove(host);
          }
        }
      }
    }
  }
}
