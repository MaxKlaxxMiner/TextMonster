using System;
using System.IO;

namespace TextMonster.Xml.Xml_Reader
{
  //
  // XmlCachedStream
  //
  internal class XmlCachedStream : MemoryStream
  {
    private const int MoveBufferSize = 4096;

    private Uri uri;

    internal XmlCachedStream(Uri uri, Stream stream)
      : base()
    {

      this.uri = uri;

      try
      {
        byte[] bytes = new byte[MoveBufferSize];
        int read = 0;
        while ((read = stream.Read(bytes, 0, MoveBufferSize)) > 0)
        {
          this.Write(bytes, 0, read);
        }
        base.Position = 0;
      }
      finally
      {
        stream.Close();
      }
    }
  }
}
