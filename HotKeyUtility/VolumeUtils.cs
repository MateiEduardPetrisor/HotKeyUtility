using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Configuration;

namespace HotkeyUtility
{
    public class VolumeUtils : IDisposable
    {
        private CoreAudioController CoreAudioControllerObj;
        private CoreAudioDevice CoreAudioDeviceObj;
        private double VolumeChangeValue;

        public void SetVolumeChangeValue(double NewValue)
        {
            this.VolumeChangeValue = NewValue;
        }

        public VolumeUtils()
        {
            this.CoreAudioControllerObj = new CoreAudioController();
            this.CoreAudioDeviceObj = this.CoreAudioControllerObj.DefaultPlaybackDevice;
        }

        public void IncreaseSoundLevel()
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

        public void DecreaseSoundLevel()
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

        public void MuteSound()
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

        public void Dispose()
        {
            this.CoreAudioControllerObj.Dispose();
            this.CoreAudioDeviceObj.Dispose();
        }
    }
}