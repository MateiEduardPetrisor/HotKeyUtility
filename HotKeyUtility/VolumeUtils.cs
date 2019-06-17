using AudioSwitcher.AudioApi.CoreAudio;
using System;
using log4net;

namespace HotKeyUtility
{
    public class VolumeUtils : IDisposable
    {
        private CoreAudioController CoreAudioControllerObj;
        private CoreAudioDevice CoreAudioDeviceObj;
        private double VolumeChangeValue;
        private bool IsAudioDevicePresent;

        public void SetVolumeChangeValue(double NewValue)
        {
            this.VolumeChangeValue = NewValue;
        }

        public bool GetIsAdudioDevicePresent()
        {
            return this.IsAudioDevicePresent;
        }

        public VolumeUtils()
        {
            Program.LoggerObj = LogManager.GetLogger("VolumeUtils.VolumeUtils()");
            this.CoreAudioControllerObj = new CoreAudioController();
            this.CoreAudioDeviceObj = this.CoreAudioControllerObj.DefaultPlaybackDevice;
            this.IsAudioDevicePresent = true;
            if (this.CoreAudioDeviceObj == null)
            {
                Program.LoggerObj.Info("No audio device found!");
                this.IsAudioDevicePresent = false;
            }
        }

        public void IncreaseSoundLevel()
        {
            if (this.IsAudioDevicePresent)
            {
                this.CoreAudioDeviceObj = this.CoreAudioControllerObj.DefaultPlaybackDevice;
                double VolumeValue = this.CoreAudioDeviceObj.Volume + this.VolumeChangeValue;
                if (VolumeValue >= 100)
                {
                    this.CoreAudioDeviceObj.Volume = 100;
                }
                else
                {
                    this.CoreAudioDeviceObj.Volume = VolumeValue;
                }
            }
        }

        public void DecreaseSoundLevel()
        {
            if (this.IsAudioDevicePresent)
            {
                this.CoreAudioDeviceObj = this.CoreAudioControllerObj.DefaultPlaybackDevice;
                double VolumeValue = this.CoreAudioDeviceObj.Volume - this.VolumeChangeValue;
                if (VolumeValue <= 0)
                {
                    this.CoreAudioDeviceObj.Volume = 0;
                }
                else
                {
                    this.CoreAudioDeviceObj.Volume = VolumeValue;
                }
            }
        }

        public void MuteSound()
        {
            if (this.IsAudioDevicePresent)
            {
                this.CoreAudioDeviceObj = this.CoreAudioControllerObj.DefaultPlaybackDevice;
                if (this.CoreAudioDeviceObj.IsMuted)
                {
                    this.CoreAudioDeviceObj.ToggleMute();
                }
                else
                {
                    this.CoreAudioDeviceObj.ToggleMute();
                }
            }
        }

        public void Dispose()
        {
            if (this.CoreAudioControllerObj != null)
            {
                this.CoreAudioControllerObj.Dispose();
            }
            if (this.CoreAudioDeviceObj != null)
            {
                this.CoreAudioDeviceObj.Dispose();
            }
        }
    }
}