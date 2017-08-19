// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.ReplayManager
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using ProcessMemoryReaderLib;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ReplaySeeker
{
    public struct OffsetsData
    {
        public int ReplayLengthOffset;
        public int TempReplayPathOffset;
        public int ReplayPositionOffset;
        public int ReplaySpeedOffset;
        public int ReplaySpeedDividerOffset;
        public int PauseOffset;
        public int StatusCodeOffset;
        public int TurboModeOffset;
    }
    public class ReplayManager : IReplayManager
    {
        // @todo: learn to make private set here
        public static Dictionary<string, OffsetsData> VersionsData = new Dictionary<string, OffsetsData>();
        public static int GameDllBase { get; private set; }

        public static int TempReplayPathOffset { get; private set; }
        public static int ReplayLengthOffset { get; private set; }
        public static int ReplayPositionOffset { get; private set; }
        public static int ReplaySpeedOffset { get; private set; }
        public static int ReplaySpeedDividerOffset { get; private set; }
        public static int PauseOffset { get; private set; }
        public static int StatusCodeOffset { get; private set; }
        public static int TurboModeOffset { get; private set; } //= 1873326436; // did not find yet.
        public static readonly int STATUS_NONE = 1313820229;
        public static readonly int STATUS_LOOP = 1280266064;
        
        public static int ReplayRestartWaitTime = 1000;

        public static bool isScanning = false;
        public static ReplayManager manager;

        private ProcessMemoryReader pReader;
        public static int memoryBlockLocation { get; private set; }
        private int lastPosition;

        public static string currentVersion;

        public static bool isScanFailed;
        public static bool IsEnabled
        {
            get
            {
                return (ReplayManager.memoryBlockLocation != 0);
            }
        }

        public static OffsetsData getVersionOffsets(string version)
        {
            OffsetsData offsets;
            if (!ReplayManager.VersionsData.TryGetValue(version, out offsets))
            {
                throw new Exception(String.Format("Unable to get {0} version offsets", version));
            }
            return offsets;
        }

        public static void updateCurrentVersion(string version)
        {
            OffsetsData offsets = ReplayManager.getVersionOffsets(version);
            ReplayManager.ReplayLengthOffset = offsets.ReplayLengthOffset;
            ReplayManager.TempReplayPathOffset = offsets.TempReplayPathOffset;
            ReplayManager.ReplayPositionOffset = offsets.ReplayPositionOffset;
            ReplayManager.ReplaySpeedOffset = offsets.ReplaySpeedOffset;
            ReplayManager.ReplaySpeedDividerOffset = offsets.ReplaySpeedDividerOffset;
            ReplayManager.PauseOffset = offsets.PauseOffset;
            ReplayManager.StatusCodeOffset = offsets.StatusCodeOffset;
            ReplayManager.TurboModeOffset = offsets.TurboModeOffset;

            ReplayManager.currentVersion = version;
        }


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
                return this.pReader.ReadInt32(ReplayManager.memoryBlockLocation + ReplayManager.ReplaySpeedOffset);
            }
            set
            {
                this.pReader.WriteInt32(ReplayManager.memoryBlockLocation + ReplayManager.ReplaySpeedOffset, value);
            }
        }

        public int SpeedDivider
        {
            get
            {
                return this.pReader.ReadInt32(ReplayManager.memoryBlockLocation + ReplayManager.ReplaySpeedDividerOffset);
            }
            set
            {
                this.pReader.WriteInt32(ReplayManager.memoryBlockLocation + ReplayManager.ReplaySpeedDividerOffset, value);
            }
        }

        public int CurrentPosition
        {
            get
            {

                return this.pReader.ReadInt32(ReplayManager.memoryBlockLocation + ReplayManager.ReplayPositionOffset);
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
                return this.pReader.ReadInt32(ReplayManager.memoryBlockLocation + ReplayManager.ReplayLengthOffset);
            }
        }

        public bool IsAbandoned
        {
            get
            {
                return this.pReader.ReadInt32(ReplayManager.memoryBlockLocation + ReplayManager.StatusCodeOffset) == ReplayManager.STATUS_NONE;
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
                return this.pReader.ReadInt32(ReplayManager.memoryBlockLocation + ReplayManager.PauseOffset) == 1;
            }
            set
            {
                this.pReader.WriteInt32(ReplayManager.memoryBlockLocation + ReplayManager.PauseOffset, value ? 1 : 0);
            }
        }

        public bool TurboMode
        {
            get
            {
                return ReplayManager.GameDllBase > 0 && this.pReader.ReadInt32(ReplayManager.TurboModeOffset) == 1;
            }
            set
            {
                if (ReplayManager.GameDllBase > 0 && ReplayManager.TurboModeOffset != 0)
                {
                    this.pReader.WriteInt32(ReplayManager.GameDllBase + ReplayManager.TurboModeOffset, value ? 1 : 0);
                }
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

        public static void RegisterVersionData(string key, OffsetsData offsets)
        {
            ReplayManager.VersionsData.Add(key, offsets);
            OffsetsData val;
            if (ReplayManager.VersionsData.TryGetValue(key, out val))
            {
                // overwrite
                ReplayManager.VersionsData[key] = offsets;
            }
            else
            {
                // append
                ReplayManager.VersionsData.Add(key, offsets);
            }
        }

        public ReplayManager(ProcessMemoryReader pReader, int memoryBlockLocation)
        {
            this.pReader = pReader;
            ReplayManager.memoryBlockLocation = memoryBlockLocation;
        }

        public static void InitiateScan(Process process, ProcessMemoryReaderProgress progressReport)
        {
            if (ReplayManager.isScanning)
                return;
            new Thread(new ParameterizedThreadStart(ReplayManager.Scanner)).Start((object)new object[2]
              {
                (object) process,
                (object) progressReport
              });
        }

        public static void Scanner(object obj)
        {
            if (ReplayManager.isScanning || ReplayManager.currentVersion == null)
                return;
            ReplayManager.isScanning = true;
            ReplayManager.isScanFailed = false;
            ProcessMemoryReader pReader = new ProcessMemoryReader();
            Process process = (Process) ((object[]) obj)[0];
            ProcessMemoryReaderProgress memoryScanProgress = (ProcessMemoryReaderProgress) ((object[]) obj)[1];

            pReader.ReadProcess = process;
            pReader.OpenProcess();
            bool flag = false;
            int memoryBlockLocation = 65536;
            while (memoryBlockLocation < 2147418112)
            {
                if (memoryBlockLocation % 3145728 == 0 && memoryScanProgress != null)
                {
                    memoryScanProgress((float)memoryBlockLocation / 2147418112);
                }
                if (pReader.ReadProcessInt32(memoryBlockLocation + ReplayManager.StatusCodeOffset) == ReplayManager.STATUS_LOOP)
                {
                    flag = (int)pReader.ReadProcessByte(memoryBlockLocation+ ReplayManager.TempReplayPathOffset) == 0;
                    break;
                }
                memoryBlockLocation += 65536;
            }
            pReader.CloseHandle();
                
            if (flag)
            {
                ProcessModuleCollection modules = process.Modules;
                foreach (ProcessModule module in modules)
                {
                    if (module.ModuleName == "Game.dll")
                    {
                        ReplayManager.GameDllBase = (int)(module.BaseAddress);
                        break;
                    }
                }
                ReplayManager.manager = new ReplayManager(pReader, memoryBlockLocation);
            } else {
                if (ReplayManager.manager != null)
                {
                    ReplayManager.manager.Dispose();
                    ReplayManager.manager = null;
                }
                ReplayManager.isScanFailed = true;
            }
            ReplayManager.isScanning = false;    
        }

        public void Activate(bool flag)
        {
            Win32Msg.SendMessage(this.pReader.ReadProcess.MainWindowHandle, 6, flag ? 1 : 0, 0);
        }

        public void BringWindowToForeground()
        {
            Win32Msg.SetForegroundWindow(this.pReader.ReadProcess.MainWindowHandle);
        }

        // @todo: Make it working
        public void Restart()
        {
            Point position = Cursor.Position;
            bool minimized = this.Minimized;
            Rectangle normalPosition = Win32Helper.GetWindowPlacement(this.pReader.ReadProcess.MainWindowHandle).NormalPosition;
            this.Minimized = false;
            this.BringWindowToForeground();
            Thread.Sleep(500);         
           /*
            // here is original decompilled code
            double num1 = 365.0 / 400.0;
            double num2 = 23.0 / 24.0;
            */
            // i`ve tried some coordinates, did not make it working yet
            // will investigate someday :D
            double num1 = this.pReader.CalculateAbsoluteCoordinateX(738);
            double num2 = this.pReader.CalculateAbsoluteCoordinateY(575);
            /*if (Screen.PrimaryScreen.Bounds != normalPosition)
            {
              num1 = ((double) normalPosition.X + num1 * (double) normalPosition.Width) / (double) Screen.PrimaryScreen.Bounds.Width;
              num2 = ((double) normalPosition.Y + num2 * (double) normalPosition.Height) / (double) Screen.PrimaryScreen.Bounds.Height;
            }*/
            //int num3 = (int)(num1 * (double)ushort.MaxValue);
            //int num4 = (int)(num2 * (double)ushort.MaxValue);
            Send.MOUSEINPUT[] mInputs = new Send.MOUSEINPUT[3];
            mInputs[0].dx = (int)num1;
            mInputs[0].dy = (int)num2;
            mInputs[0].mouseData = 0U;
            mInputs[0].dwFlags = Send.Constants.MOUSEEVENTF_MOVE | Send.Constants.MOUSEEVENTF_ABSOLUTE;
            mInputs[0].time = 0U;
            mInputs[1] = mInputs[0];
            mInputs[1].dwFlags = Send.Constants.MOUSEEVENTF_LEFTDOWN | Send.Constants.MOUSEEVENTF_ABSOLUTE;
            mInputs[2] = mInputs[0];
            mInputs[2].dwFlags = Send.Constants.MOUSEEVENTF_LEFTUP | Send.Constants.MOUSEEVENTF_ABSOLUTE;
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
            ReplayManager.memoryBlockLocation = 0;
            this.pReader.ReadProcess = (Process)null;
            this.pReader = (ProcessMemoryReader)null;
        }
    }
}
