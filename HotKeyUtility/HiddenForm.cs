using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using log4net;

namespace HotKeyUtility
{
    public partial class HiddenForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        private VolumeUtils VolumeUtilsObj;
        private BrightnessUtils BrightnessUtilsObj;
        private NetworkUtils NetworkUtilsObj;
        private ConfigurationUtils ConfigurationUtilsObj;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams CreateParamsObj = base.CreateParams;
                CreateParamsObj.ExStyle |= 0x80;
                return CreateParamsObj;
            }
        }

        protected override void WndProc(ref Message MessageObjct)
        {
            base.WndProc(ref MessageObjct);

            if (MessageObjct.Msg == 0x0312)
            {
                int HotKeyId = MessageObjct.WParam.ToInt32();
                switch (HotKeyId)
                {
                    case (int)HotKeyIds.IncreaseVolumeHotKeyId:
                        this.VolumeUtilsObj.IncreaseSoundLevel();
                        break;
                    case (int)HotKeyIds.DecreaseVolumeHotKeyId:
                        this.VolumeUtilsObj.DecreaseSoundLevel();
                        break;
                    case (int)HotKeyIds.MuteVolumeHotKeyId:
                        this.VolumeUtilsObj.MuteSound();
                        break;
                    case (int)HotKeyIds.IncreaseBrigthnessHotKeyId:
                        this.BrightnessUtilsObj.IncreaseBrightness();
                        break;
                    case (int)HotKeyIds.DecreaseBrigthnessHotKeyId:
                        this.BrightnessUtilsObj.DecreaseBrightness();
                        break;
                    case (int)HotKeyIds.EnableNetworkConnectionHotKeyId:
                        this.NetworkUtilsObj.SetNetworkInterfaceState("disabled", "enable");
                        break;
                    case (int)HotKeyIds.DisableNetworkConnectionHotKeyId:
                        this.NetworkUtilsObj.SetNetworkInterfaceState("enabled", "disable");
                        break;
                    case (int)HotKeyIds.ExitApplicationHotKeyId:
                        this.Close();
                        break;
                }
            }
        }

        public HiddenForm()
        {
            InitializeComponent();
            this.ConfigurationUtilsObj = new ConfigurationUtils();
            this.VolumeUtilsObj = new VolumeUtils(this.ConfigurationUtilsObj.GetVolumeChangeValue());
            this.BrightnessUtilsObj = new BrightnessUtils(this.ConfigurationUtilsObj.GetBrightnessChangeValue());
            this.NetworkUtilsObj = new NetworkUtils();
        }

        private void HiddenForm_Load(object sender, EventArgs e)
        {
            Program.LoggerObj = LogManager.GetLogger("HiddenForm.HiddenForm_Load()");
            int HotKeyId;
            int HotKeyHashCode;
            int KeyModifier;
            if (this.VolumeUtilsObj.GetIsAdudioDevicePresent())
            {
                HotKeyId = (int)HotKeyIds.DecreaseVolumeHotKeyId;
                HotKeyHashCode = this.ConfigurationUtilsObj.GetKeyVolumeDown();
                KeyModifier = this.ConfigurationUtilsObj.GetModifier();
                if (!RegisterHotKey(this.Handle, HotKeyId, KeyModifier, HotKeyHashCode))
                {
                    Program.LoggerObj.Error("Failed to register global hotkey for VolumeDown " + "Id = " + HotKeyId + " Modifier = " + KeyModifier);
                }
                HotKeyId = (int)HotKeyIds.IncreaseVolumeHotKeyId;
                HotKeyHashCode = this.ConfigurationUtilsObj.GetKeyVolumeUp();
                KeyModifier = this.ConfigurationUtilsObj.GetModifier();
                if (!RegisterHotKey(this.Handle, HotKeyId, KeyModifier, HotKeyHashCode))
                {
                    Program.LoggerObj.Error("Failed to register global hotkey for VolumeUp " + "Id = " + HotKeyId + " Modifier = " + KeyModifier);
                }
                HotKeyId = (int)HotKeyIds.MuteVolumeHotKeyId;
                HotKeyHashCode = this.ConfigurationUtilsObj.GetKeyVolumeMute();
                KeyModifier = this.ConfigurationUtilsObj.GetModifier();
                if (!RegisterHotKey(this.Handle, HotKeyId, KeyModifier, HotKeyHashCode))
                {
                    Program.LoggerObj.Error("Failed to register global hotkey for VolumeMute " + "Id = " + HotKeyId + " Modifier = " + KeyModifier);
                }
            }
            if (this.BrightnessUtilsObj.GetIsBrightnessSupported())
            {
                HotKeyId = (int)HotKeyIds.IncreaseBrigthnessHotKeyId;
                HotKeyHashCode = this.ConfigurationUtilsObj.GetKeyBrightnessUp();
                KeyModifier = this.ConfigurationUtilsObj.GetModifier();
                if (!RegisterHotKey(this.Handle, HotKeyId, KeyModifier, HotKeyHashCode))
                {
                    Program.LoggerObj.Error("Failed to register global hotkey for BrightnessUp " + "Id = " + HotKeyId + " Modifier = " + KeyModifier);
                }
                HotKeyId = (int)HotKeyIds.DecreaseBrigthnessHotKeyId;
                HotKeyHashCode = this.ConfigurationUtilsObj.GetKeyBrightnessDown();
                KeyModifier = this.ConfigurationUtilsObj.GetModifier();
                if (!RegisterHotKey(this.Handle, HotKeyId, KeyModifier, HotKeyHashCode))
                {
                    Program.LoggerObj.Error("Failed to register global hotkey for BrightnessDown " + "Id = " + HotKeyId + " Modifier = " + KeyModifier);
                }
            }
            if (this.NetworkUtilsObj.GetIsNetworkAdapterPresent())
            {
                HotKeyId = (int)HotKeyIds.DisableNetworkConnectionHotKeyId;
                HotKeyHashCode = this.ConfigurationUtilsObj.GetKeyNetworkDown();
                KeyModifier = this.ConfigurationUtilsObj.GetModifier();
                if (!RegisterHotKey(this.Handle, HotKeyId, KeyModifier, HotKeyHashCode))
                {
                    Program.LoggerObj.Error("Failed to register global hotkey for NetworkDown " + "Id = " + HotKeyId + " Modifier = " + KeyModifier);
                }
                HotKeyId = (int)HotKeyIds.EnableNetworkConnectionHotKeyId;
                HotKeyHashCode = this.ConfigurationUtilsObj.GetKeyNetworkUp();
                KeyModifier = this.ConfigurationUtilsObj.GetModifier();
                if (!RegisterHotKey(this.Handle, HotKeyId, KeyModifier, HotKeyHashCode))
                {
                    Program.LoggerObj.Error("Failed to register global hotkey for NetworkUp " + "Id = " + HotKeyId + " Modifier = " + KeyModifier);
                }
            }
            HotKeyId = (int)HotKeyIds.ExitApplicationHotKeyId;
            HotKeyHashCode = this.ConfigurationUtilsObj.GetKeyExit();
            KeyModifier = this.ConfigurationUtilsObj.GetModifier();
            if (!RegisterHotKey(this.Handle, HotKeyId, KeyModifier, HotKeyHashCode))
            {
                Program.LoggerObj.Error("Failed to register global hotkey for Exit " + "Id = " + HotKeyId + " Modifier = " + KeyModifier);
            }
        }

        private void HiddenForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LoggerObj = LogManager.GetLogger("HiddenForm.HiddenForm_Closing()");
            int HotKeyId;
            if (this.VolumeUtilsObj.GetIsAdudioDevicePresent())
            {
                this.VolumeUtilsObj.Dispose();
                HotKeyId = (int)HotKeyIds.IncreaseVolumeHotKeyId;
                if (!UnregisterHotKey(this.Handle, HotKeyId))
                {
                    Program.LoggerObj.Error("Failed to unregistered global hotkey for VolumeUp");
                }
                HotKeyId = (int)HotKeyIds.DecreaseVolumeHotKeyId;
                if (!UnregisterHotKey(this.Handle, HotKeyId))
                {
                    Program.LoggerObj.Error("Failed to unregistered global hotkey for VolumeDown");
                }
                HotKeyId = (int)HotKeyIds.MuteVolumeHotKeyId;
                if (!UnregisterHotKey(this.Handle, HotKeyId))
                {
                    Program.LoggerObj.Error("Failed to unregistered global hotkey for VolumeMute");
                }
            }
            if (this.BrightnessUtilsObj.GetIsBrightnessSupported())
            {
                HotKeyId = (int)HotKeyIds.IncreaseBrigthnessHotKeyId;
                if (!UnregisterHotKey(this.Handle, HotKeyId))
                {
                    Program.LoggerObj.Error("Failed unregistered global hotkey for BrightnessUp");
                }
                HotKeyId = (int)HotKeyIds.DecreaseBrigthnessHotKeyId;
                if (!UnregisterHotKey(this.Handle, HotKeyId))
                {
                    Program.LoggerObj.Error("Failed to unregistered global hotkey for BrightnessDown");
                }
            }
            if (this.NetworkUtilsObj.GetIsNetworkAdapterPresent())
            {
                HotKeyId = (int)HotKeyIds.DisableNetworkConnectionHotKeyId;
                if (!UnregisterHotKey(this.Handle, HotKeyId))
                {
                    Program.LoggerObj.Error("Failed to unregistered global hotkey for NetworkDown");
                }
                HotKeyId = (int)HotKeyIds.EnableNetworkConnectionHotKeyId;
                if (!UnregisterHotKey(this.Handle, HotKeyId))
                {
                    Program.LoggerObj.Error("Failed to unregistered global hotkey for NetworkUp");
                }
            }
            HotKeyId = (int)HotKeyIds.ExitApplicationHotKeyId;
            if (!UnregisterHotKey(this.Handle, HotKeyId))
            {
                Program.LoggerObj.Error("Failed to unregistered global hotkey for Exit");
            }
        }
    }
}