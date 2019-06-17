using System;
using System.Management;
using log4net;

namespace HotKeyUtility
{
    public class BrightnessUtils
    {
        private bool IsBrightnessSupported;
        private byte[] SupportedLevels;

        public bool GetIsBrightnessSupported()
        {
            return this.IsBrightnessSupported;
        }

        public BrightnessUtils()
        {
            try
            {
                this.IsBrightnessSupported = true;
                this.SupportedLevels = this.GetSupportedBrightnessLevels();
            }
            catch (Exception)
            {
                Program.LoggerObj = LogManager.GetLogger("BrightnessUtils.BrightnessUtils()");
                Program.LoggerObj.Info("Brightness adjustment is not supported by this monitor!");
                this.IsBrightnessSupported = false;
            }
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
            if (this.IsBrightnessSupported)
            {
                int CurrentBrightnessValue = this.GetCurrentBrightnessValue();
                int IndexCurrentBrightnessValue = Array.FindIndex(this.SupportedLevels, Result => Result == CurrentBrightnessValue);
                int NextIndex = IndexCurrentBrightnessValue + 1;
                if (NextIndex >= 0 && NextIndex < SupportedLevels.Length)
                {
                    int NewBrightness = SupportedLevels[NextIndex];
                    this.SetBrightness(NewBrightness);
                }
            }
        }

        public void DecreaseBrightness()
        {
            if (this.IsBrightnessSupported)
            {
                int CurrentBrightnessValue = this.GetCurrentBrightnessValue();
                int IndexCurrentBrightnessValue = Array.FindIndex(this.SupportedLevels, Result => Result == CurrentBrightnessValue);
                int NewIndex = IndexCurrentBrightnessValue - 1;
                if (NewIndex >= 0 && NewIndex < SupportedLevels.Length)
                {
                    int NewBrightness = SupportedLevels[NewIndex];
                    this.SetBrightness(NewBrightness);
                }
            }
        }
    }
}