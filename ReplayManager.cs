// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.ReplayManager
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using ProcessMemoryReaderLib;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ReplaySeeker
{
    public class ReplayManager : IReplayManager
    {
        public static readonly int TempReplayPathOffset = 3484;

        public static readonly int ReplayLengthOffset = 2308;

        public static readonly int ReplayPositionOffset = 7456;

        public static readonly int ReplaySpeedOffset = 9060;

        public static readonly int ReplaySpeedDividerOffset = 9064;

        public static readonly int PauseOffset = 9068;

        public static readonly int StatusCodeOffset = 9016;

        public static readonly int STATUS_NONE = 1313820229;

        public static readonly int STATUS_LOOP = 1280266064;

        public static readonly int TurboModeLocation = 1873326436;

        public static int ReplayRestartWaitTime = 1000;
 
        private ProcessMemoryReader pReader;
        private int memoryBlockLocation;
        private int lastPosition;

        public IProcessMemory Memory
        {
            get
            {
                return (IProcessMemory)this.pReader;
            }
        }

        public int CurrentSpeed
        {
            get
            {
                return this.pReader.ReadInt32(this.memoryBlockLocation + ReplayManager.ReplaySpeedOffset);
            }
            set
            {
                this.pReader.WriteInt32(this.memoryBlockLocation + ReplayManager.ReplaySpeedOffset, value);
            }
        }

        public int SpeedDivider
        {
            get
            {
                return this.pReader.ReadInt32(this.memoryBlockLocation + ReplayManager.ReplaySpeedDividerOffset);
            }
            set
            {
                this.pReader.WriteInt32(this.memoryBlockLocation + ReplayManager.ReplaySpeedDividerOffset, value);
            }
        }

        public int CurrentPosition
        {
            get
            {
                return this.pReader.ReadInt32(this.memoryBlockLocation + ReplayManager.ReplayPositionOffset);
            }
        }

        public int ReliableCurrentPosition
        {
            get
            {
                int currentPosition = this.CurrentPosition;
                if (this.lastPosition == -1)
                    this.lastPosition = currentPosition;
                else if (this.lastPosition + 10000 > currentPosition)
                    this.lastPosition = currentPosition;
                return this.lastPosition;
            }
            set
            {
                this.lastPosition = value;
            }
        }

        public int ReplayLength
        {
            get
            {
                return this.pReader.ReadInt32(this.memoryBlockLocation + ReplayManager.ReplayLengthOffset);
            }
        }

        public bool IsAbandoned
        {
            get
            {
                return this.pReader.ReadInt32(this.memoryBlockLocation + ReplayManager.StatusCodeOffset) == ReplayManager.STATUS_NONE;
            }
        }

        public ProcessPriorityClass ProcessPriority
        {
            get
            {
                return this.pReader.ReadProcess.PriorityClass;
            }
        }

        public bool Paused
        {
            get
            {
                return this.pReader.ReadInt32(this.memoryBlockLocation + ReplayManager.PauseOffset) == 1;
            }
            set
            {
                this.pReader.WriteInt32(this.memoryBlockLocation + ReplayManager.PauseOffset, value ? 1 : 0);
            }
        }

        public bool TurboMode
        {
            get
            {
                return this.pReader.ReadInt32(ReplayManager.TurboModeLocation) == 1;
            }
            set
            {
                this.pReader.WriteInt32(ReplayManager.TurboModeLocation, value ? 1 : 0);
            }
        }

        public bool Minimized
        {
            get
            {
                return Win32Msg.IsIconic(this.pReader.ReadProcess.MainWindowHandle);
            }
            set
            {
                Win32Msg.ShowWindow(this.pReader.ReadProcess.MainWindowHandle, value ? 6 : 9);
            }
        }

        public bool Focused
        {
            get
            {
                return Win32Msg.GetForegroundWindow() == this.pReader.ReadProcess.MainWindowHandle;
            }
        }

        public ReplayManager(ProcessMemoryReader pReader, int memoryBlockLocation)
        {
            this.pReader = pReader;
            this.memoryBlockLocation = memoryBlockLocation;
        }

        public static ReplayManager FromProcess(Process process)
        {
            ProcessMemoryReader pReader = new ProcessMemoryReader();
            pReader.ReadProcess = process;
            pReader.OpenProcess();
            bool flag = false;
            int memoryBlockLocation = 65536;
            while (memoryBlockLocation < 2147418112)
            {
                if (pReader.ReadProcessInt32(memoryBlockLocation + ReplayManager.StatusCodeOffset) == ReplayManager.STATUS_LOOP)
                {
                    flag = (int)pReader.ReadProcessByte(memoryBlockLocation + ReplayManager.TempReplayPathOffset) == 0;
                    break;
                }
                memoryBlockLocation += 65536;
            }
            pReader.CloseHandle();
            if (!flag)
                return (ReplayManager)null;
           
            return new ReplayManager(pReader, memoryBlockLocation);
        }

        public void Activate(bool flag)
        {
            Win32Msg.SendMessage(this.pReader.ReadProcess.MainWindowHandle, 6, flag ? 1 : 0, 0);
        }

        public void BringWindowToForeground()
        {
            Win32Msg.SetForegroundWindow(this.pReader.ReadProcess.MainWindowHandle);
        }

        public void Restart()
        {

            Point position = Cursor.Position;

            bool minimized = this.Minimized;

            Rectangle normalPosition = Win32Helper.GetWindowPlacement(this.pReader.ReadProcess.MainWindowHandle).NormalPosition;

            this.Minimized = false;

            this.BringWindowToForeground();

            Thread.Sleep(500);

            double num1 = 369.0 / 400.0;

            double num2 = 23.0 / 24.0;

            if (Screen.PrimaryScreen.Bounds != normalPosition)
            {

                num1 = ((double)normalPosition.X + num1 * (double)normalPosition.Width) / (double)Screen.PrimaryScreen.Bounds.Width;

                num2 = ((double)normalPosition.Y + num2 * (double)normalPosition.Height) / (double)Screen.PrimaryScreen.Bounds.Height;

            }

            int num3 = (int)(num1 * (double)ushort.MaxValue);

            int num4 = (int)(num2 * (double)ushort.MaxValue);

            Send.MOUSEINPUT[] mInputs = new Send.MOUSEINPUT[3];

            mInputs[0].dx = num3;

            mInputs[0].dy = num4;

            mInputs[0].mouseData = 0U;

            mInputs[0].dwFlags = 32769U;

            mInputs[0].time = 0U;

            mInputs[1] = mInputs[0];

            mInputs[1].dwFlags = 32770U;

            mInputs[2] = mInputs[0];

            mInputs[2].dwFlags = 32772U;

            Send.MouseInput(mInputs);

            Thread.Sleep(ReplayManager.ReplayRestartWaitTime);

            if (minimized)

                this.Minimized = true;

            Application.OpenForms[0].Activate();

            Cursor.Position = position;

        }
 

        public void SetSpeed(int newSpeed)
        {
            newSpeed = Math.Min(31, Math.Max(newSpeed, -31));
            int num1;
            int num2;
            if (newSpeed < 0)
            {
                num1 = 1;
                num2 = -(newSpeed - 1);
            }
            else
            {
                num1 = newSpeed + 1;
                num2 = 1;
                if (num1 == 31)
                    num1 = (int)ushort.MaxValue;
            }
            this.CurrentSpeed = num1;
            this.SpeedDivider = num2;
        }

        public int GetSpeed()
        {
            int speedDivider = this.SpeedDivider;
            int currentSpeed = this.CurrentSpeed;
            return speedDivider <= 1 ? (currentSpeed == (int)ushort.MaxValue ? 31 : currentSpeed - 1) : (currentSpeed != 1 ? Math.Min(currentSpeed / speedDivider - 1, 31) : -(speedDivider - 1));
        }

        public void Dispose()
        {
            this.memoryBlockLocation = 0;
            this.pReader.ReadProcess = (Process)null;
            this.pReader = (ProcessMemoryReader)null;
        }
    }
}
