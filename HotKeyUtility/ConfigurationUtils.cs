using HotkeyUtility;
using log4net;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace HotKeyUtility
{
    public class ConfigurationUtils
    {
        private int Modifier;
        private double VolumeChangeValue;
        private int KeyVolumeUpHashCode;
        private int KeyVolumeDownHashCode;
        private int KeyVolumeMuteHashCode;
        private int KeyBrightnessUpHashCode;
        private int KeyBrightnessDownHashCode;
        private int KeyNetworkDownHashCode;
        private int KeyNetworkUpHashCode;
        private int KeyExitHashCode;
        private Configuration ConfigurationObj;
        private KeyValueConfigurationCollection KeyValueConfigurationCollectionObj;
        private KeysConverter KeysConverterObj;

        public int GetModifier()
        {
            return this.Modifier;
        }

        public double GetVolumeChangeValue()
        {
            return this.VolumeChangeValue;
        }

        public int GetKeyVolumeUp()
        {
            return this.KeyVolumeUpHashCode;
        }

        public int GetKeyVolumeDown()
        {
            return this.KeyVolumeDownHashCode;
        }

        public int GetKeyVolumeMute()
        {
            return this.KeyVolumeMuteHashCode;
        }

        public int GetKeyBrightnessUp()
        {
            return this.KeyBrightnessUpHashCode;
        }

        public int GetKeyBrightnessDown()
        {
            return this.KeyBrightnessDownHashCode;
        }

        public int GetKeyNetworkDown()
        {
            return this.KeyNetworkDownHashCode;
        }

        public int GetKeyNetworkUp()
        {
            return this.KeyNetworkUpHashCode;
        }

        public int GetKeyExit()
        {
            return this.KeyExitHashCode;
        }

        public ConfigurationUtils()
        {
            this.KeysConverterObj = new KeysConverter();
            this.ConfigurationObj = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            this.KeyValueConfigurationCollectionObj = KeyValueConfigurationCollectionObj = ConfigurationObj.AppSettings.Settings;
        }

        public void GetAppParams()
        {
            Program.LoggerObj = LogManager.GetLogger("ConfigurationUtils.GetAppParams()");
            try
            {
                this.Modifier = this.GetNumberFromConfiguration("Modifier");
                this.VolumeChangeValue = this.GetNumberFromConfiguration("VolumeChangeValue");
                this.KeyVolumeUpHashCode = this.GetHotKeyFromHashCode("KeyVolumeUp");
                this.KeyVolumeDownHashCode = this.GetHotKeyFromHashCode("KeyVolumeDown");
                this.KeyVolumeMuteHashCode = this.GetHotKeyFromHashCode("KeyVolumeMute");
                this.KeyBrightnessUpHashCode = this.GetHotKeyFromHashCode("KeyBrightnessUp");
                this.KeyBrightnessDownHashCode = this.GetHotKeyFromHashCode("KeyBrightnessDown");
                this.KeyNetworkUpHashCode = this.GetHotKeyFromHashCode("KeyNetworkUp");
                this.KeyNetworkDownHashCode = this.GetHotKeyFromHashCode("KeyNetworkDown");
                this.KeyExitHashCode = this.GetHotKeyFromHashCode("KeyExit");
            }
            catch (Exception ExceptionObj)
            {
                Program.LoggerObj.Error("Unable to read app parameters", ExceptionObj);
                Environment.Exit(-1);
            }
        }

        private int GetHotKeyFromHashCode(String ConfigurationKey)
        {
            String HotKey = this.KeyValueConfigurationCollectionObj[ConfigurationKey].Value;
            return this.ConvertStringToKeysEnum(HotKey).GetHashCode();
        }

        private int GetNumberFromConfiguration(String ConfigurationKey)
        {
            Program.LoggerObj = LogManager.GetLogger("ConfigurationUtils.GetNumberFromConfig(String ConfigurationKey)");
            int result = 0;
            String value = "";
            try
            {
                value = this.KeyValueConfigurationCollectionObj[ConfigurationKey].Value;
                result = Int32.Parse(value);
            }
            catch (Exception ExceptionObj)
            {
                throw new Exception("Failed to convert " + value + " to int32", ExceptionObj);
            }
            return result;
        }

        private Keys ConvertStringToKeysEnum(String KeyAsString)
        {
            Program.LoggerObj = LogManager.GetLogger("ConfigurationUtils.ConvertStringToKeysEnum(String KeyAsString)");
            if (!Enum.TryParse(KeyAsString, true, out Keys KeyAsEnum))
            {
                throw new Exception("Failed to convert " + KeyAsString + " to Keys Enum type");
            }
            return KeyAsEnum;
        }
    }
}