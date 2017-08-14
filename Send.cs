// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.Send
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Runtime.InteropServices;

namespace ReplaySeeker
{
  public class Send
  {
    [DllImport("user32.dll")]
    private static extern IntPtr GetMessageExtraInfo();

    [DllImport("user32")]
    private static extern int SendInput(int nInputs, Send.INPUT[] pInputs, int cbSize);

    [DllImport("user32")]
    private static extern int SendInput(int nInputs, ref Send.INPUT pInputs, int cbSize);

    public static void MouseInput(Send.MOUSEINPUT[] mInputs)
    {
      Send.INPUT[] pInputs = new Send.INPUT[mInputs.Length];
      for (int index = 0; index < pInputs.Length; ++index)
      {
        pInputs[index] = new Send.INPUT();
        pInputs[index].type = 0;
        pInputs[index].mi = mInputs[index];
        pInputs[index].mi.dwExtraInfo = Send.GetMessageExtraInfo();
      }
      Send.SendInput(pInputs.Length, pInputs, Marshal.SizeOf(typeof (Send.INPUT)));
    }

    public static void KeyboardInput(Send.KEYBDINPUT[] kbInputs)
    {
      Send.INPUT[] pInputs = new Send.INPUT[kbInputs.Length];
      for (int index = 0; index < pInputs.Length; ++index)
      {
        pInputs[index] = new Send.INPUT();
        pInputs[index].type = 1;
        pInputs[index].ki = kbInputs[index];
        pInputs[index].ki.dwExtraInfo = Send.GetMessageExtraInfo();
      }
      Send.SendInput(pInputs.Length, pInputs, Marshal.SizeOf(typeof (Send.INPUT)));
    }

    public static void KeyboardInput(Send.KEYBDINPUT kbInput)
    {
      Send.INPUT pInputs = new Send.INPUT();
      pInputs.type = 1;
      pInputs.ki = kbInput;
      pInputs.ki.dwExtraInfo = Send.GetMessageExtraInfo();
      Send.SendInput(1, ref pInputs, Marshal.SizeOf(typeof (Send.INPUT)));
    }

    public class Constants
    {
      public const int INPUT_MOUSE = 0;
      public const int INPUT_KEYBOARD = 1;
      public const int INPUT_HARDWARE = 2;
      public const int MOUSEEVENTF_MOVE = 1;
      public const int MOUSEEVENTF_LEFTDOWN = 2;
      public const int MOUSEEVENTF_LEFTUP = 4;
      public const int MOUSEEVENTF_RIGHTDOWN = 8;
      public const int MOUSEEVENTF_RIGHTUP = 16;
      public const int MOUSEEVENTF_MIDDLEDOWN = 32;
      public const int MOUSEEVENTF_MIDDLEUP = 64;
      public const int MOUSEEVENTF_XDOWN = 128;
      public const int MOUSEEVENTF_XUP = 256;
      public const int MOUSEEVENTF_WHEEL = 2048;
      public const int MOUSEEVENTF_VIRTUALDESK = 16384;
      public const int MOUSEEVENTF_ABSOLUTE = 32768;
      public const uint KEYEVENTF_EXTENDEDKEY = 1;
      public const uint KEYEVENTF_KEYUP = 2;
      public const uint KEYEVENTF_UNICODE = 4;
      public const uint KEYEVENTF_SCANCODE = 8;
    }

    public struct MOUSEINPUT
    {
      public int dx;
      public int dy;
      public uint mouseData;
      public uint dwFlags;
      public uint time;
      public IntPtr dwExtraInfo;
    }

    public struct KEYBDINPUT
    {
      public ushort wVk;
      public ushort wScan;
      public uint dwFlags;
      public uint time;
      public IntPtr dwExtraInfo;
    }

    public struct HARDWAREINPUT
    {
      public uint uMsg;
      public ushort wParamL;
      public ushort wParamH;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT
    {
      [FieldOffset(0)]
      public int type;
      [FieldOffset(4)]
      public Send.MOUSEINPUT mi;
      [FieldOffset(4)]
      public Send.KEYBDINPUT ki;
      [FieldOffset(4)]
      public Send.HARDWAREINPUT hi;
    }
  }
}
