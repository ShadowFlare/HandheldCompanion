﻿using ControllerCommon.Inputs;
using System.Collections.Generic;
using System.Numerics;
using WindowsInput.Events;

namespace ControllerCommon.Devices
{
    public class AOKZOEA1 : IDevice
    {
        public AOKZOEA1() : base()
        {
            // device specific settings
            this.ProductIllustration = "device_aokzoe_a1";
            this.ProductModel = "AOKZOEA1";

            // https://www.amd.com/en/products/apu/amd-ryzen-7-6800u 
            this.nTDP = new double[] { 15, 15, 20 };
            this.cTDP = new double[] { 10, 28 };
            this.GfxClock = new double[] { 100, 2200 };

            this.AngularVelocityAxisSwap = new()
            {
                { 'X', 'X' },
                { 'Y', 'Z' },
                { 'Z', 'Y' },
            };

            this.AccelerationAxis = new Vector3(-1.0f, 1.0f, -1.0f);
            this.AccelerationAxisSwap = new()
            {
                { 'X', 'X' },
                { 'Y', 'Z' },
                { 'Z', 'Y' },
            };

            // device specific capacities
            this.Capacities = DeviceCapacities.FanControl;

            this.ECDetails = new ECDetails()
            {
                AddressControl = 0x44A,
                AddressDuty = 0x44B,
                AddressRegistry = 0x4E,
                AddressData = 0x4F,
                ValueMin = 0,
                ValueMax = 184
            };

            // Home
            OEMChords.Add(new DeviceChord("Home",
                new List<KeyCode>() { KeyCode.LWin, KeyCode.D },
                new List<KeyCode>() { KeyCode.LWin, KeyCode.D },
                false, ButtonFlags.OEM1
                ));

            // Home (long press 1.5s)
            OEMChords.Add(new DeviceChord("Home, Long-press",
                new List<KeyCode>() { KeyCode.LWin, KeyCode.G },
                new List<KeyCode>() { KeyCode.LWin, KeyCode.G },
                false, ButtonFlags.OEM6
                ));

            // Keyboard
            OEMChords.Add(new DeviceChord("Keyboard",
                new List<KeyCode>() { KeyCode.RControlKey, KeyCode.LWin, KeyCode.O },
                new List<KeyCode>() { KeyCode.O, KeyCode.LWin, KeyCode.RControlKey },
                false, ButtonFlags.OEM2
                ));

            // Turbo
            OEMChords.Add(new DeviceChord("Turbo",
                new List<KeyCode>() { KeyCode.LControl, KeyCode.LWin, KeyCode.LMenu },
                new List<KeyCode>() { KeyCode.LControl, KeyCode.LWin, KeyCode.LMenu },
                false, ButtonFlags.OEM3
                ));

            // Home + Keyboard
            OEMChords.Add(new DeviceChord("Home + Keyboard",
                new List<KeyCode>() { KeyCode.RAlt, KeyCode.RControlKey, KeyCode.Delete },
                new List<KeyCode>() { KeyCode.Delete, KeyCode.RControlKey, KeyCode.RAlt },
                false, ButtonFlags.OEM4
                ));

            // Home + Turbo
            OEMChords.Add(new DeviceChord("Home + Turbo",
                new List<KeyCode>() { KeyCode.LWin, KeyCode.Snapshot },
                new List<KeyCode>() { KeyCode.Snapshot, KeyCode.LWin },
                false, ButtonFlags.OEM5
                ));
        }
    }
}
