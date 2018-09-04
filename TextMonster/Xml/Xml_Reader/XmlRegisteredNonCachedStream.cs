using System;
using System.IO;

namespace TextMonster.Xml.Xml_Reader
{
  //
  // XmlRegisteredNonCachedStream
  //
  internal class XmlRegisteredNonCachedStream : Stream
  {
    protected Stream stream;
    XmlDownloadManager downloadManager;
    string host;

    internal XmlRegisteredNonCachedStream(Stream stream, XmlDownloadManager downloadManager, string host)
    {
      this.stream = stream;
      this.downloadManager = downloadManager;
      this.host = host;
    }

    ~XmlRegisteredNonCachedStream()
    {
      if (downloadManager != null)
      {
        downloadManager.Remove(host);
      }
      stream = null;
      // The base class, Stream, provides its own finalizer
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        if (disposing && stream != null)
        {
          if (downloadManager != null)
          {
            downloadManager.Remove(host);
          }
          stream.Close();
        }
        stream = null;
        GC.SuppressFinalize(this); // do not call finalizer
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    //
    // Stream
    //
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
      return stream.BeginRead(buffer, offset, count, callback, state);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
      return stream.BeginWrite(buffer, offset, count, callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
      return stream.EndRead(asyncResult);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
      stream.EndWrite(asyncResult);
    }

    public override void Flush()
    {
      stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      return stream.Read(buffer, offset, count);
    }

    public override int ReadByte()
    {
      return stream.ReadByte();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      return stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      stream.Write(buffer, offset, count);
    }

    public override void WriteByte(byte value)
    {
      stream.WriteByte(value);
    }

    public override Boolean CanRead
    {
      get { return stream.CanRead; }
    }

    public override Boolean CanSeek
    {
      get { return stream.CanSeek; }
    }

    public override Boolean CanWrite
    {
      get { return stream.CanWrite; }
    }

    public override long Length
    {
      get { return stream.Length; }
    }

    public override long Position
    {
      get { return stream.Position; }
      set { stream.Position = value; }
    }
  }
}
