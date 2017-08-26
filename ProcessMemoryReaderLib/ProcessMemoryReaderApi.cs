// Decompiled with JetBrains decompiler
// Type: ProcessMemoryReaderLib.ProcessMemoryReaderApi
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Runtime.InteropServices;

enum SystemMetric
{
  SM_CXSCREEN = 0,
  SM_CYSCREEN = 1,
}

namespace ProcessMemoryReaderLib
{
  internal class ProcessMemoryReaderApi
  {
      public const uint PROCESS_VM_READ = (0x0010);
      public const uint PROCESS_VM_WRITE = (0x0020);
      public const uint PROCESS_VM_OPERATION = (0x0008);
      public const uint PROCESS_QUERY_INFORMATION = (0x0400);
      public const uint PROCESS_READ_WRITE_QUERY = PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_QUERY_INFORMATION;
      public const uint MEM_COMMIT = (0x1000);
      public const uint MEM_RESERVE = (0x2000);
      public const uint MEM_RELEASE = (0x8000);
      public const uint PAGE_READWRITE = (0x04);        

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern int CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll")]
    public static extern Int32 VirtualProtectEx(
        IntPtr hProcess,
        uint dwAddress, //IntPtr lpAddress,
        int nSize,      //UIntPtr dwSize,
        uint flNewProtect,
        out uint lpflOldProtect);



    [DllImport("kernel32.dll")]
    public static extern int ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, [In, Out] byte[] buffer, uint size, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern int ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, out int value, uint size, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern int ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, out byte value, uint size, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern int ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, out float value, uint size, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern Int32 WriteProcessMemory(IntPtr hProcess, Int32 lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out Int32 lpNumberOfBytesWritten);
  //  public static extern Int32 WriteProcessMemory(IntPtr hProcess, Int32 lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out Int32 lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    public static extern int WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, [In] ref int buffer, uint size, out int lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    public static extern int WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, [In] ref byte buffer, uint size, out int lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    public static extern int WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, [In] ref float buffer, uint size, out int lpNumberOfBytesWritten);

    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(SystemMetric smIndex);


  }
}
