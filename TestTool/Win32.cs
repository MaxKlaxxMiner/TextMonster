using System;
using System.Runtime.InteropServices;
using System.Security;
// ReSharper disable UnusedMember.Global

namespace TestTool
{
  [SuppressUnmanagedCodeSecurity]
  public static unsafe class Win32
  {
    // docs siehe: https://docs.microsoft.com/de-de/windows/desktop/api/fileapi/nf-fileapi-readfile

    public const uint GenericWrite = 0x40000000;
    public const uint GenericRead = 0x80000000;
    public const int FileShareDelete = 4;
    public const int FileShareWrite = 2;
    public const int FileShareRead = 1;
    public const int OpenAlways = 4;
    public const int CreateNew = 1;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr CreateFileW(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    [DllImport("kernel32.dll")]
    public static extern bool GetFileSizeEx(IntPtr hFile, out long size);

    [DllImport("kernel32.dll")]
    public static extern void CloseHandle(IntPtr hFile);

    [DllImport("kernel32.dll")]
    public static extern bool ReadFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, IntPtr lpOverlapped);

    [DllImport("kernel32.dll")]
    public static extern bool ReadFile(IntPtr hFile, byte* lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, IntPtr lpOverlapped);
  }
}
