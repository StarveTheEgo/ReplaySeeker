// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.ReplayManager
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using ProcessMemoryReaderLib;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;


public enum Protection : uint
{
    PAGE_NOACCESS = 0x01,
    PAGE_READONLY = 0x02,
    PAGE_READWRITE = 0x04,
    PAGE_WRITECOPY = 0x08,
    PAGE_EXECUTE = 0x10,
    PAGE_EXECUTE_READ = 0x20,
    PAGE_EXECUTE_READWRITE = 0x40,
    PAGE_EXECUTE_WRITECOPY = 0x80,
    PAGE_GUARD = 0x100,
    PAGE_NOCACHE = 0x200,
    PAGE_WRITECOMBINE = 0x400
}
namespace ReplaySeeker
{
    using ReplaySeeker.Core;
    public struct PatchData
    {
        public int offset;
        public byte[] original;
        public byte[] patch; 
    }

    public struct OffsetsData
    {
        public int ReplayLengthOffset;
        public int TempReplayPathOffset;
        public int ReplayPositionOffset;
        public int ReplaySpeedOffset;
        public int ReplaySpeedDividerOffset;
        public int PauseOffset;
        public int StatusCodeOffset;
        // Game.dll offsets
        public int TurboModeOffset;

        public List<PatchData> RendererData;

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
        public static int TurboModeOffset { get; private set; }
        public static bool RenderPatchEnabled { get; private set;}
        public static List<PatchData> RendererData { get; private set; }
        public static readonly int STATUS_NONE = 1313820229;
        public static readonly int STATUS_LOOP = 1280266064;
        
        public static int ReplayRestartWaitTime = 1000;

        public static bool isScanning = false;
        public static ReplayManager manager;

        private ProcessMemoryReader pReader;
        public static int memoryBlockLocation { get; private set; }
        private int lastPosition;

        public static string currentVersion;

        public static bool isScanStopped = false;

        public static bool isEnabled
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
            if (version != "Auto")
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
                ReplayManager.RendererData = new List<PatchData>(offsets.RendererData);
            } 
           
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



        public bool RenderState
        {
            get
            {
                if (!ReplayManager.RenderPatchEnabled) 
                    return false;
                    
                // @todo: optimize this ugly code when i`ll learn how to
                // @todo: use IntPtr addresses
                bool result = true;
                if (ReplayManager.GameDllBase > 0)
                {
                    foreach (var patch_entry in ReplayManager.RendererData) {
                        // we will compare bytes to original ones
                        // it it does not match - return false
                        int address = ReplayManager.GameDllBase + patch_entry.offset;
                        uint original_length = (uint)patch_entry.original.Length;
                        if (!Enumerable.SequenceEqual(patch_entry.original, this.pReader.ReadByte(address, original_length)))
                        {
                            result = false;
                            break;
                        }
                    }
                }
                return result;
            }
            set
            {
                if (ReplayManager.GameDllBase > 0)
                {
                    foreach (var patch_entry in ReplayManager.RendererData) {
                        this.pReader.WriteByte(ReplayManager.GameDllBase + patch_entry.offset, value ? patch_entry.original : patch_entry.patch, (uint)(value ? patch_entry.original : patch_entry.patch).Length);
                    }
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
                Win32Msg.ShowWindow(this.pReader.ReadProcess.MainWindowHandle, value ? Win32Msg.SW_MINIMIZE : Win32Msg.SW_RESTORE);
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

        public void CheckRenderPatchSupport()
        {
            ReplayManager.RenderPatchEnabled = true;
        }

        public static void Scanner(object obj)
        {
            if (ReplayManager.isScanning || ReplayManager.currentVersion == null || ReplayManager.isScanStopped)
                return;
            ReplayManager.GameDllBase = 0;
            //ReplayManager.isScanStopped = false;
            ReplayManager.isScanning = true;
            ProcessMemoryReader pReader = new ProcessMemoryReader();
            Process process = (Process) ((object[]) obj)[0];
            ProcessMemoryReaderProgress memoryScanProgress = (ProcessMemoryReaderProgress) ((object[]) obj)[1];

            pReader.ReadProcess = process;
            pReader.OpenProcess();
            bool flag = false;
            int iterations = 0;
            int memoryBlockLocation = 65536;
            int LocalStatusCodeOffset = 0;
            int LocalTempReplayPathOffset = 0;
            Dictionary<string, OffsetsData> offsetsToScan = new Dictionary<string, OffsetsData>();

            foreach (KeyValuePair<string, OffsetsData> entry in ReplayManager.VersionsData)
            {
                if (ReplayManager.currentVersion == "Auto" || ReplayManager.currentVersion == entry.Key)
                {
                    offsetsToScan.Add(entry.Key, entry.Value);
                }
                    
            }

            if (offsetsToScan.Count == 0)
            {
                // @todo Show some error here
                return;
            }

            while (memoryBlockLocation < 2147418112 && !flag && !ReplayManager.isScanStopped && !process.HasExited)
            {
                if (memoryBlockLocation % 3145728 == 0 && memoryScanProgress != null)
                {
                    memoryScanProgress((float)memoryBlockLocation / 2147418112);
                }

                if (iterations % 656 == 0)
                {
                    System.Diagnostics.Debug.WriteLine((float)memoryBlockLocation / 2147418112);
                    Thread.Sleep(10);
                }

                foreach (KeyValuePair<string, OffsetsData> entry in offsetsToScan)
                {
                   LocalStatusCodeOffset = entry.Value.StatusCodeOffset;
                   LocalTempReplayPathOffset = entry.Value.TempReplayPathOffset;
                   if (pReader.ReadProcessInt32(memoryBlockLocation + LocalStatusCodeOffset) == ReplayManager.STATUS_LOOP)
                   {
                       flag = (int)pReader.ReadProcessByte(memoryBlockLocation + LocalTempReplayPathOffset) == 0;
                       if (flag)
                       {
                           if (ReplayManager.currentVersion == "Auto") {
                               ReplayManager.updateCurrentVersion(entry.Key);
                           }
                           break;
                       }
                   }
                }
                if (flag)
                {
                    break;
                }
                memoryBlockLocation += 65536;
                iterations++;
            }
            pReader.CloseHandle();


            if (flag && !process.HasExited)
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
                if (ReplayManager.isScanStopped)
                {
                    ReplayManager.isScanStopped = false;
                }
                ReplayManager.manager = new ReplayManager(pReader, memoryBlockLocation);
            } else {
                if (ReplayManager.manager != null)
                {
                    ReplayManager.manager.Dispose();
                    ReplayManager.manager = null;
                }
            }
            ReplayManager.isScanning = false;    
        }

        public void Activate(bool flag)
        {
            Win32Msg.SendMessage(this.pReader.ReadProcess.MainWindowHandle, Win32Msg.WM_ACTIVATE, flag ? Win32Msg.WA_ACTIVE : Win32Msg.WA_INACTIVE, 0);
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

            // @todo: rework for any resolution
            double num1 = this.pReader.CalculateAbsoluteCoordinateX(1476);
            double num2 = this.pReader.CalculateAbsoluteCoordinateY(864);
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
            Application.OpenForms[0].SynchronizedInvoke(() => Application.OpenForms[0].Activate() );
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
