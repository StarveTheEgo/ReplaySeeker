// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.Win32Msg
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ReplaySeeker
{
  public class Win32Msg
  {
    public const int WM_CREATE = 1;
    public const int WS_THICKFRAME = 262144;
    public const int WM_LBUTTONDOWN = 513;
    public const int WM_LBUTTONUP = 514;
    public const int MK_LBUTTON = 1;
    public const int WM_ACTIVATE = 6;
    public const int WA_INACTIVE = 0;
    public const int WA_ACTIVE = 1;
    public const int WA_CLICKACTIVE = 2;
    public const int SW_HIDE = 0;
    public const int SW_SHOWNORMAL = 1;
    public const int SW_NORMAL = 1;
    public const int SW_SHOWMINIMIZED = 2;
    public const int SW_SHOWMAXIMIZED = 3;
    public const int SW_MAXIMIZE = 3;
    public const int SW_SHOWNOACTIVATE = 4;
    public const int SW_SHOW = 5;
    public const int SW_MINIMIZE = 6;
    public const int SW_SHOWMINNOACTIVE = 7;
    public const int SW_SHOWNA = 8;
    public const int SW_RESTORE = 9;
    public const int SW_SHOWDEFAULT = 10;
    public const int SW_FORCEMINIMIZE = 11;
    public const int SW_MAX = 11;

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern int SendMessage(IntPtr handle, int wMsg, int wParam, int lparam);

    [DllImport("user32")]
    public static extern bool IsZoomed(IntPtr hwnd);

    [DllImport("user32")]
    public static extern bool IsIconic(IntPtr hwnd);

    [DllImport("user32")]
    public static extern bool SetForegroundWindow(IntPtr hwnd);

    [DllImport("user32")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    [DllImport("user32", SetLastError = true)]
    public static extern bool GetClientRect(IntPtr hwnd, int[] lpRect);

    [DllImport("user32")]
    public static extern bool SetCursorPos(int x, int y);

    [DllImport("user32")]
    public static extern bool GetWindowPlacement(IntPtr hWnd, ref Win32Msg.WINDOWPLACEMENT lpwndpl);

    public struct WINDOWPLACEMENT
    {
      private int length;
      private int flags;
      private int showCmd;
      public unsafe fixed int ptMinPosition[2];
      public unsafe fixed int ptMaxPosition[2];
      public unsafe fixed int rcNormalPosition[4];

      public int Length
      {
        get
        {
          return this.length;
        }
        set
        {
          this.length = value;
        }
      }

      public int Flags
      {
        get
        {
          return this.flags;
        }
        set
        {
          this.flags = value;
        }
      }

      public int ShowCmd
      {
        get
        {
          return this.showCmd;
        }
        set
        {
          this.showCmd = value;
        }
      }

      public unsafe Rectangle NormalPosition
      {
        get
        {

            fixed (int* numPtr = this.rcNormalPosition)
            {
                return new Rectangle(*numPtr, numPtr[1], numPtr[2] - *numPtr, numPtr[3] - numPtr[1]);
            }
            
        }
      }
        
    /*  public unsafe Point MinPosition
      {
        get
        {
          fixed (int* numPtr = &this.ptMinPosition.FixedElementField)
            return new Point(*numPtr, numPtr[1]);
        }
      }

      public unsafe Point MaxPosition
      {
        get
        {
          fixed (int* numPtr = &this.ptMaxPosition.FixedElementField)
            return new Point(*numPtr, numPtr[1]);
        }
      }*/
    }
  }
}
