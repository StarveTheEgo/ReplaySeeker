// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.Win32Helper
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ReplaySeeker
{
  public class Win32Helper
  {
    public static Win32Msg.WINDOWPLACEMENT GetWindowPlacement(IntPtr hwnd)
    {
      Win32Msg.WINDOWPLACEMENT lpwndpl = new Win32Msg.WINDOWPLACEMENT();
      lpwndpl.Length = Marshal.SizeOf((object) lpwndpl);
      Win32Msg.GetWindowPlacement(hwnd, ref lpwndpl);
      return lpwndpl;
    }

    public static void SendKey(Keys key, bool down)
    {
      Send.KeyboardInput(new Send.KEYBDINPUT()
      {
        wVk = (ushort) key,
        dwFlags = down ? 0U : 2U
      });
    }
  }
}
