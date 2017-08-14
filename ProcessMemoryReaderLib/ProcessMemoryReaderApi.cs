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
    public const uint PROCESS_VM_READ = 16;
    public const uint PROCESS_VM_WRITE = 32;
    public const uint PROCESS_VM_OPERATION = 8;
    public const uint PROCESS_QUERY_INFORMATION = 1024;
    public const uint PROCESS_READ_WRITE_QUERY = 1080;

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern int CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll")]
    public static extern int ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, [In, Out] byte[] buffer, uint size, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern int ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, out int value, uint size, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern int ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, out byte value, uint size, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern int ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, out float value, uint size, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern int WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, [In, Out] byte[] buffer, uint size, out int lpNumberOfBytesWritten);

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
