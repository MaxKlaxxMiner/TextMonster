using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security.Permissions;

namespace TextMonster.Xml.Xml_Reader
{
  internal static class DynamicAssemblies
  {
    static Hashtable tableIsTypeDynamic = Hashtable.Synchronized(new Hashtable());
    static volatile FileIOPermission fileIOPermission;
    static FileIOPermission UnrestrictedFileIOPermission
    {
      get
      {
        if (fileIOPermission == null)
        {
          fileIOPermission = new FileIOPermission(PermissionState.Unrestricted);
        }
        return fileIOPermission;
      }
    }

    // SxS: This method does not take any resource name and does not expose any resources to the caller.
    // It's OK to suppress the SxS warning.
    [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
    [ResourceExposure(ResourceScope.None)]
    internal static bool IsTypeDynamic(Type type)
    {
      object oIsTypeDynamic = tableIsTypeDynamic[type];
      if (oIsTypeDynamic == null)
      {
        UnrestrictedFileIOPermission.Assert();
        Assembly assembly = type.Assembly;
        bool isTypeDynamic = assembly.IsDynamic || string.IsNullOrEmpty(assembly.Location);
        if (!isTypeDynamic)
        {
          if (type.IsArray)
          {
            isTypeDynamic = IsTypeDynamic(type.GetElementType());
          }
          else if (type.IsGenericType)
          {
            Type[] parameterTypes = type.GetGenericArguments();
            if (parameterTypes != null)
            {
              for (int i = 0; i < parameterTypes.Length; i++)
              {
                Type parameterType = parameterTypes[i];
                if (!(parameterType == null || parameterType.IsGenericParameter))
                {
                  isTypeDynamic = IsTypeDynamic(parameterType);
                  if (isTypeDynamic)
                    break;
                }
              }
            }
          }
        }
        tableIsTypeDynamic[type] = oIsTypeDynamic = isTypeDynamic;
      }
      return (bool)oIsTypeDynamic;
    }
  }
}