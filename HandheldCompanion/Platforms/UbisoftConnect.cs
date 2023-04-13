﻿using ControllerCommon.Platforms;
using ControllerCommon.Utils;
using HandheldCompanion.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HandheldCompanion.Platforms
{
    public class UbisoftConnect : IPlatform
    {
        public UbisoftConnect()
        {
            Name = "Ubisoft Connect";
            ExecutableName = "UbisoftConnect.exe";

            // store specific modules
            Modules = new List<string>()
            {
                "UbisoftConnect.exe",
                "UbisoftExtension.exe",
                "UbisoftGameLauncher.exe",
                "UbisoftGameLauncher64.exe",
                "uplay_r1_loader64.dll",
                "uplay_r164.dll",
            };

            // check if platform is installed
            InstallPath = RegistryUtils.GetString(@"SOFTWARE\WOW6432Node\Ubisoft\Launcher", "InstallDir");
            if (Path.Exists(InstallPath))
            {
                // update paths
                SettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Ubisoft Game Launcher\settings.yaml");
                ExecutablePath = Path.Combine(InstallPath, ExecutableName);

                // check executable
                IsInstalled = File.Exists(ExecutablePath);
            }

            base.PlatformType = PlatformType.UbisoftConnect;
        }

        public override bool IsRunning()
        {
            return Process is not null;
        }

        public override bool Start()
        {
            if (!IsInstalled)
                return false;

            if (!IsRunning())
                return false;

            var process = Process.Start(new ProcessStartInfo()
            {
                FileName = ExecutablePath,
                // ArgumentList = { "-gamepadui" },
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            return process is not null;
        }

        public override bool Stop()
        {
            if (!IsInstalled)
                return false;

            if (IsRunning())
                return false;

            var process = Process.Start(new ProcessStartInfo()
            {
                FileName = ExecutablePath,
                ArgumentList = { "-shutdown" },
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            return process is not null;
        }
    }
}