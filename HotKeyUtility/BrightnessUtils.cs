﻿using System;
using System.Management;
using System.Windows.Forms;

namespace HotkeyUtility
{
    public class BrightnessUtils
    {
        private bool IsSupported;

        public BrightnessUtils()
        {
            this.IsSupported = true;
        }

        private int GetCurrentBrightnessValue()
        {
            int CurrentBrightnessValue = 0;
            ManagementScope ManagementScopeObj = new ManagementScope("root\\WMI");
            SelectQuery SelectQueryObj = new SelectQuery("WmiMonitorBrightness");
            using (ManagementObjectSearcher ManagementObjectSearcherObj = new ManagementObjectSearcher(ManagementScopeObj, SelectQueryObj))
            {
                using (ManagementObjectCollection ManagementObjectCollectionObj = ManagementObjectSearcherObj.Get())
                {
                    foreach (ManagementObject ManagementObjectObj in ManagementObjectCollectionObj)
                    {
                        CurrentBrightnessValue = Convert.ToInt32(ManagementObjectObj.GetPropertyValue("CurrentBrightness"));
                        break;
                    }
                }
            }
            return CurrentBrightnessValue;
        }

        private byte[] GetSupportedBrightnessLevels()
        {
            byte[] SupportedLevels = null;
            ManagementScope ManagementScopeObj = new ManagementScope("root\\WMI");
            SelectQuery SelectQueryObj = new SelectQuery("WmiMonitorBrightness");
            using (ManagementObjectSearcher ManagementObjectSearcherObj = new ManagementObjectSearcher(ManagementScopeObj, SelectQueryObj))
            {
                using (ManagementObjectCollection ManagementObjectCollectionObj = ManagementObjectSearcherObj.Get())
                {
                    foreach (ManagementObject ManagementObjectObj in ManagementObjectCollectionObj)
                    {
                        SupportedLevels = (byte[])ManagementObjectObj.GetPropertyValue("Level");
                        break;
                    }
                }
            }
            return SupportedLevels;
        }

        private void SetBrightness(int BrightnessValue)
        {
            ManagementScope ManagementScopeObj = new ManagementScope("root\\WMI");
            SelectQuery SelectQueryObj = new SelectQuery("WmiMonitorBrightnessMethods");
            using (ManagementObjectSearcher ManagementObjectSearcherObj = new ManagementObjectSearcher(ManagementScopeObj, SelectQueryObj))
            {
                using (ManagementObjectCollection ManagementObjectCollectionObj = ManagementObjectSearcherObj.Get())
                {
                    foreach (ManagementObject ManagementObjectObj in ManagementObjectCollectionObj)
                    {
                        ManagementObjectObj.InvokeMethod("WmiSetBrightness", new Object[] { UInt32.MaxValue, Convert.ToByte(BrightnessValue) });
                        break;
                    }
                }
            }
        }

        public void IncreaseBrightness()
        {
            if (this.IsSupported)
            {
                try
                {
                    int CurrentBrightnessValue = this.GetCurrentBrightnessValue();
                    byte[] SupportedLevels = this.GetSupportedBrightnessLevels();
                    int IndexCurrentBrightnessValue = Array.FindIndex(SupportedLevels, Result => Result == CurrentBrightnessValue);
                    int NextIndex = IndexCurrentBrightnessValue + 1;
                    if (NextIndex >= 0 && NextIndex < SupportedLevels.Length)
                    {
                        int NewBrightness = SupportedLevels[NextIndex];
                        this.SetBrightness(NewBrightness);
                    }
                }
                catch (Exception)
                {
                    this.IsSupported = false;
                    MessageBox.Show("Brightness Adjust Not Supported By Your Monitor!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public void DecreaseBrightness()
        {
            if (this.IsSupported)
            {
                try
                {
                    int CurrentBrightnessValue = this.GetCurrentBrightnessValue();
                    byte[] SupportedLevels = this.GetSupportedBrightnessLevels();
                    int IndexCurrentBrightnessValue = Array.FindIndex(SupportedLevels, Result => Result == CurrentBrightnessValue);
                    int NewIndex = IndexCurrentBrightnessValue - 1;
                    if (NewIndex >= 0 && NewIndex < SupportedLevels.Length)
                    {
                        int NewBrightness = SupportedLevels[NewIndex];
                        this.SetBrightness(NewBrightness);
                    }
                }
                catch (Exception)
                {
                    this.IsSupported = false;
                    MessageBox.Show("Brightness Adjust Not Supported By Your Monitor!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}