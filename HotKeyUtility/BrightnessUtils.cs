using System;
using System.Management;
using log4net;

namespace HotKeyUtility
{
    public class BrightnessUtils
    {
        private bool IsBrightnessSupported;
        private byte[] SupportedLevels;
        private int BrightnessChangeValue;

        public bool GetIsBrightnessSupported()
        {
            return this.IsBrightnessSupported;
        }

        private void SetSupportedLevels()
        {
            byte[] allBrightnessLevels = this.GetSupportedBrightnessLevels();
            int supportedLevelsLength = 0;
            for (int i = this.BrightnessChangeValue; i < allBrightnessLevels.Length; i += this.BrightnessChangeValue)
            {
                supportedLevelsLength++;
            }
            this.SupportedLevels = new byte[supportedLevelsLength + 1];
            this.SupportedLevels[0] = allBrightnessLevels[0];
            this.SupportedLevels[supportedLevelsLength] = allBrightnessLevels[allBrightnessLevels.Length - 1];
            int copyIndex = 1;
            for (int i = this.BrightnessChangeValue; i < allBrightnessLevels.Length; i += this.BrightnessChangeValue)
            {
                if (copyIndex < supportedLevelsLength)
                {
                    this.SupportedLevels[copyIndex] = allBrightnessLevels[i];
                    copyIndex++;
                }
            }
        }

        public BrightnessUtils(int NumberOfBrightnessIntervals)
        {
            try
            {
                this.IsBrightnessSupported = true;
                this.BrightnessChangeValue = NumberOfBrightnessIntervals;
                this.SetSupportedLevels();
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
            byte[] AllBrightnessSupportedLevels = null;
            ManagementScope ManagementScopeObj = new ManagementScope("root\\WMI");
            SelectQuery SelectQueryObj = new SelectQuery("WmiMonitorBrightness");
            using (ManagementObjectSearcher ManagementObjectSearcherObj = new ManagementObjectSearcher(ManagementScopeObj, SelectQueryObj))
            {
                using (ManagementObjectCollection ManagementObjectCollectionObj = ManagementObjectSearcherObj.Get())
                {
                    foreach (ManagementObject ManagementObjectObj in ManagementObjectCollectionObj)
                    {
                        AllBrightnessSupportedLevels = (byte[])ManagementObjectObj.GetPropertyValue("Level");
                        break;
                    }
                }
            }
            return AllBrightnessSupportedLevels;
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
                        ManagementObjectObj.InvokeMethod("WmiSetBrightness", new Object[] { Int32.MaxValue, Convert.ToByte(BrightnessValue) });
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