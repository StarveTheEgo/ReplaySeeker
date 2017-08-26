// Decompiled with JetBrains decompiler
// Type: ProcessMemoryReaderLib.ProcessMemoryReader
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using ReplaySeeker;
using System;
using System.Diagnostics;

namespace ProcessMemoryReaderLib
{
  public class ProcessMemoryReader : IProcessMemory
  {
    private IntPtr m_hProcess = IntPtr.Zero;
    private object lockObject = new object();
    private Process m_ReadProcess;
    private int processUsers;

    public Process ReadProcess
    {
      get
      {
        return this.m_ReadProcess;
      }
      set
      {
        this.m_ReadProcess = value;
      }
    }

    public void OpenProcess()
    {
      lock (this.lockObject)
      {
        if (this.processUsers == 0)
            this.m_hProcess = ProcessMemoryReaderApi.OpenProcess(2035711, 0, (uint)this.m_ReadProcess.Id);
        ++this.processUsers;
      }
    }

    public void CloseHandle()
    {
      lock (this.lockObject)
      {
        --this.processUsers;
        if (this.processUsers == 0 && ProcessMemoryReaderApi.CloseHandle(this.m_hProcess) == 0)
          throw new Exception("You probably do not have administrator priviledges. Run this tool as administrator to avoid errors. CloseHandle failed.");
      }
    }

    public byte[] ReadProcessMemory(int MemoryAddress, uint bytesToRead, out int bytesRead)
    {
      byte[] buffer = new byte[bytesToRead];
      ProcessMemoryReaderApi.ReadProcessMemory(this.m_hProcess, MemoryAddress, buffer, bytesToRead, out bytesRead);
      return buffer;
    }

    public int ReadProcessMemory(int MemoryAddress, byte[] buffer, uint bytesToRead)
    {
      int lpNumberOfBytesRead;
      ProcessMemoryReaderApi.ReadProcessMemory(this.m_hProcess, MemoryAddress, buffer, bytesToRead, out lpNumberOfBytesRead);
      return lpNumberOfBytesRead;
    }

    public int WriteProcessMemory(int MemoryAddress, byte[] buffer, uint bytesToWrite)
    {
      int lpNumberOfBytesWritten;
      ProcessMemoryReaderApi.WriteProcessMemory(this.m_hProcess, MemoryAddress, buffer, bytesToWrite, out lpNumberOfBytesWritten);
      return lpNumberOfBytesWritten;
    }

    public int ReadInt32(int MemoryAddress)
    {
      try
      {
        this.OpenProcess();
        int num = 0;
        int lpNumberOfBytesRead;
        ProcessMemoryReaderApi.ReadProcessMemory(this.m_hProcess, MemoryAddress, out num, 4U, out lpNumberOfBytesRead);
        this.CloseHandle();
        return num;
      }
      catch
      {
        return 0;
      }
    }

    public int ReadProcessInt32(int MemoryAddress)
    {
      try
      {
        int num = 0;
        int lpNumberOfBytesRead;
        ProcessMemoryReaderApi.ReadProcessMemory(this.m_hProcess, MemoryAddress, out num, 4U, out lpNumberOfBytesRead);
        return num;
      }
      catch
      {
        return 0;
      }
    }

    public byte ReadByte(int MemoryAddress)
    {
      try
      {
        this.OpenProcess();
        byte num;
        int lpNumberOfBytesRead;
        ProcessMemoryReaderApi.ReadProcessMemory(this.m_hProcess, MemoryAddress, out num, 1U, out lpNumberOfBytesRead);
        this.CloseHandle();
        return num;
      }
      catch
      {
        return 0;
      }
    }

    public byte[] ReadByte(int MemoryAddress, uint bytesToRead)
    {
      try
      {
          this.OpenProcess();
          int lpNumberOfBytesRead;
          byte[] buffer = new byte[bytesToRead];
          ProcessMemoryReaderApi.ReadProcessMemory(this.m_hProcess, MemoryAddress, buffer, bytesToRead, out lpNumberOfBytesRead);
          this.CloseHandle();
          return buffer;
      }
      catch
      {
          return new byte[0];
      }
  }

  public byte ReadProcessByte(int MemoryAddress)
  {
    try
    {
      byte num;
      int lpNumberOfBytesRead;
      ProcessMemoryReaderApi.ReadProcessMemory(this.m_hProcess, MemoryAddress, out num, 1U, out lpNumberOfBytesRead);
      return num;
    }
    catch
    {
      return 0;
    }
  }

  public float ReadFloat32(int MemoryAddress)
  {
    try
    {
      this.OpenProcess();
      float num;
      int lpNumberOfBytesRead;
      ProcessMemoryReaderApi.ReadProcessMemory(this.m_hProcess, MemoryAddress, out num, 4U, out lpNumberOfBytesRead);
      this.CloseHandle();
      return num;
    }
    catch
    {
      return 0.0f;
    }
  }

  public float ReadProcessFloat32(int MemoryAddress)
  {
    try
    {
      float num;
      int lpNumberOfBytesRead;
      ProcessMemoryReaderApi.ReadProcessMemory(this.m_hProcess, MemoryAddress, out num, 4U, out lpNumberOfBytesRead);
      return num;
    }
    catch
    {
      return 0.0f;
    }
  }

  public void WriteInt32(int MemoryAddress, int value)
  {
    try
    {
      this.OpenProcess();
      int lpNumberOfBytesWritten;
      ProcessMemoryReaderApi.WriteProcessMemory(this.m_hProcess, MemoryAddress, ref value, 4U, out lpNumberOfBytesWritten);
      this.CloseHandle();
    }
    catch
    {
    }
  }

  public void WriteProcessInt32(int MemoryAddress, int value)
  {
    try
    {
      int lpNumberOfBytesWritten;
      ProcessMemoryReaderApi.WriteProcessMemory(this.m_hProcess, MemoryAddress, ref value, 4U, out lpNumberOfBytesWritten);
    }
    catch
    {
    }
  }

    public void WriteByte(int MemoryAddress, byte[] bytes, uint bytesToWrite)
    {
        try
        {
            this.OpenProcess();
            int lpNumberOfBytesWritten;
            ProcessMemoryReaderApi.WriteProcessMemory(this.m_hProcess, MemoryAddress, bytes, bytesToWrite, out lpNumberOfBytesWritten);
            this.CloseHandle();
        }
        catch
        {
        }
    }

    public void WriteByte(int MemoryAddress, byte value)
    {
      try
      {
        this.OpenProcess();
        int lpNumberOfBytesWritten;
        ProcessMemoryReaderApi.WriteProcessMemory(this.m_hProcess, MemoryAddress, ref value, 1U, out lpNumberOfBytesWritten);
        this.CloseHandle();
      }
      catch
      {
      }
    }

    public void WriteProcessByte(int MemoryAddress, byte value)
    {
      try
      {
        int lpNumberOfBytesWritten;
        ProcessMemoryReaderApi.WriteProcessMemory(this.m_hProcess, MemoryAddress, ref value, 1U, out lpNumberOfBytesWritten);
      }
      catch
      {
      }
    }

    public void WriteFloat32(int MemoryAddress, float value)
    {
      try
      {
        this.OpenProcess();
        int lpNumberOfBytesWritten;
        ProcessMemoryReaderApi.WriteProcessMemory(this.m_hProcess, MemoryAddress, ref value, 4U, out lpNumberOfBytesWritten);
        this.CloseHandle();
      }
      catch
      {
      }
    }

    public void WriteProcessFloat32(int MemoryAddress, float value)
    {
      try
      {
        int lpNumberOfBytesWritten;
        ProcessMemoryReaderApi.WriteProcessMemory(this.m_hProcess, MemoryAddress, ref value, 4U, out lpNumberOfBytesWritten);
      }
      catch
      {
      }
    }

    public int CalculateAbsoluteCoordinateX(int x)
    {

        //System.Diagnostics.Debug.WriteLine(ProcessMemoryReaderApi.GetSystemMetrics(SystemMetric.SM_CXSCREEN));
        return (x * 65536) / ProcessMemoryReaderApi.GetSystemMetrics(SystemMetric.SM_CXSCREEN);
    }

    public int CalculateAbsoluteCoordinateY(int y)
    {
        //System.Diagnostics.Debug.WriteLine(ProcessMemoryReaderApi.GetSystemMetrics(SystemMetric.SM_CYSCREEN));
        return (y * 65536) / ProcessMemoryReaderApi.GetSystemMetrics(SystemMetric.SM_CYSCREEN);
    }
  }
}
