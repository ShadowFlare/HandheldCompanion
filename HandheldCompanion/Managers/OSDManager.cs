﻿using ControllerCommon.Managers;
using HandheldCompanion.Controls;
using PrecisionTiming;
using RTSSSharedMemoryNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ControllerCommon.WinAPI;
using static PInvoke.Kernel32;

namespace HandheldCompanion.Managers
{
    public static class OSDManager
    {
        private static bool IsInitialized;
        private static short OverlayLevel;

        private static PrecisionTimer RefreshTimer;
        private static int RefreshInterval = 100;

        private static Dictionary<int, OSD> OnScreenDisplay = new();

        public static event InitializedEventHandler Initialized;
        public delegate void InitializedEventHandler();

        private const string Header = "<C0=008040><C1=0080C0><C2=C08080><C3=FF0000><C4=FFFFFF><C250=FF8000><A0=-4><A1=5><A2=-2><A3=-3><A4=-4><A5=-5><S0=-50><S1=50>";
        private static List<string> Content;

        static OSDManager()
        {
            SettingsManager.SettingValueChanged += SettingsManager_SettingValueChanged;

            PlatformManager.RTSS.Hooked += RTSS_Hooked;
            PlatformManager.RTSS.Unhooked += RTSS_Unhooked;

            // timer used to monitor foreground application framerate
            RefreshInterval = SettingsManager.GetInt("OnScreenDisplayRefreshRate");

            RefreshTimer = new PrecisionTimer();
            RefreshTimer.SetAutoResetMode(true);
            RefreshTimer.SetResolution(0);
            RefreshTimer.SetPeriod(RefreshInterval);
            RefreshTimer.Tick += UpdateOSD;
        }

        private static void RTSS_Unhooked(int processId)
        {
            // clear previous display
            if (OnScreenDisplay.TryGetValue(processId, out var OSD))
            {
                OSD.Update(string.Empty);
                OSD.Dispose();
            }

            OnScreenDisplay.Remove(processId);
        }

        private static void RTSS_Hooked(int processId)
        {
            ProcessEx processEx = ProcessManager.GetProcess(processId);
            if (processEx is null)
                return;

            OnScreenDisplay[processId] = new(processEx.Title);
        }

        public static void Start()
        {
            IsInitialized = true;
            Initialized?.Invoke();

            LogManager.LogInformation("{0} has started", "OSDManager");
        }

        private static void UpdateOSD(object? sender, EventArgs e)
        {
            if (OverlayLevel == 0)
                return;

            foreach (var pair in OnScreenDisplay)
            {
                int processId = pair.Key;
                OSD processOSD = pair.Value;

                // temp (test)
                var profile = ProfileManager.GetCurrent();
                var ProfileName = profile.Name;
                var UMC = profile.MotionEnabled;
                var FPS = PlatformManager.RTSS.GetFramerate(processId);

                string content = Draw(processId);

                try
                {
                    processOSD.Update(content);
                }
                catch (FileNotFoundException) { }
            }
        }

        public static string Draw(int processId)
        {
            Content = new()
            {
                Header
            };

            switch (OverlayLevel)
            {
                default:
                case 0:
                    break;

                case 1:
                    Content.Add(string.Format("{0} <C4>FPS<C>", PlatformManager.RTSS.GetFramerate(processId)));
                    break;
            }

            return string.Join("\n", Content);
        }

        public static void Stop()
        {
            if (!IsInitialized)
                return;

            RefreshTimer.Stop();

            IsInitialized = false;

            LogManager.LogInformation("{0} has stopped", "OSDManager");
        }

        private static void SettingsManager_SettingValueChanged(string name, object value)
        {
            switch (name)
            {
                case "OnScreenDisplayLevel":
                    {
                        OverlayLevel = Convert.ToInt16(value);

                        if (OverlayLevel >= 0)
                        {
                            RefreshTimer.Start();
                        }
                        else
                        {
                            RefreshTimer.Stop();

                            // clear UI on stop
                            foreach (var pair in OnScreenDisplay)
                            {
                                OSD processOSD = pair.Value;
                                processOSD.Update(string.Empty);
                            }
                        }
                    }
                    break;

                case "OnScreenDisplayRefreshRate":
                    {
                        RefreshInterval = Convert.ToInt32(value);

                        if (RefreshTimer.IsRunning())
                        {
                            RefreshTimer.Stop();
                            RefreshTimer.SetPeriod(RefreshInterval);
                            RefreshTimer.Start();
                        }
                    }
                    break;
            }
        }
    }
}
