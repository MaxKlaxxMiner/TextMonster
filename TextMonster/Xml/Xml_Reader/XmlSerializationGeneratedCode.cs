using System;
using System.Reflection;
using System.Threading;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSerializationGeneratedCode.uex' path='docs/doc[@for="XmlSerializationGeneratedCode"]/*' />
  ///<internalonly/>
  public abstract class XmlSerializationGeneratedCode
  {
    TempAssembly tempAssembly;
    int threadCode;
    ResolveEventHandler assemblyResolver;

    internal void Init(TempAssembly tempAssembly)
    {
      this.tempAssembly = tempAssembly;
      // only hook the assembly resolver if we have something to help us do the resolution
      if (tempAssembly != null && tempAssembly.NeedAssembyResolve)
      {
        // we save the threadcode to make sure we don't handle any resolve events for any other threads
        threadCode = Thread.CurrentThread.GetHashCode();
        assemblyResolver = new ResolveEventHandler(OnAssemblyResolve);
        AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver;
      }
    }

    // this method must be called at the end of serialization
    internal void Dispose()
    {
      if (assemblyResolver != null)
        AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolver;
      assemblyResolver = null;
    }

    internal Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
    {
      if (tempAssembly != null && Thread.CurrentThread.GetHashCode() == threadCode)
        return tempAssembly.GetReferencedAssembly(args.Name);
      return null;
    }
  }
}
