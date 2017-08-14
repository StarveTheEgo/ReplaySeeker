// Decompiled with JetBrains decompiler
// Type: Utils.PerfCounter
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Runtime.InteropServices;

namespace Utils
{
  public struct PerfCounter
  {
    private long _start;

    public void Start()
    {
      this._start = 0L;
      PerfCounter.QueryPerformanceCounter(ref this._start);
    }

    public float Finish()
    {
      long performanceCount = 0;
      PerfCounter.QueryPerformanceCounter(ref performanceCount);
      long frequency = 0;
      PerfCounter.QueryPerformanceFrequency(ref frequency);
      return (float) (performanceCount - this._start) / (float) frequency;
    }

    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceCounter(ref long performanceCount);

    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceFrequency(ref long frequency);
  }
}
