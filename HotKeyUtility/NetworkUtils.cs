using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using log4net;

namespace HotKeyUtility
{
    public class NetworkUtils
    {
        private List<NetworkInterfaceInfo> NetworkInterfaceNamesObj;
        private readonly char[] Separators = new char[] { '\r', '\n' };
        private bool IsNetworkAdapterPresent;

        public bool GetIsNetworkAdapterPresent()
        {
            return this.IsNetworkAdapterPresent;
        }

        public NetworkUtils()
        {
            Program.LoggerObj = LogManager.GetLogger("NetworkUtils.NetworkUtils()");
            this.CheckForAdapters();
            if (!this.IsNetworkAdapterPresent)
            {
                Program.LoggerObj.Info("No network adapters found!");
            }
            this.NetworkInterfaceNamesObj = new List<NetworkInterfaceInfo>();
        }

        private void CheckForAdapters()
        {
            this.IsNetworkAdapterPresent = true;
            String output = this.GetNetshOuput();
            String[] Tokens = output.Split(this.Separators, StringSplitOptions.RemoveEmptyEntries);
            if (Tokens.Length == 2)
            {
                this.IsNetworkAdapterPresent = false;
            }
        }

        private String GetNetshOuput()
        {
            ProcessStartInfo psi = new ProcessStartInfo("netsh", "interface show interface");
            System.Diagnostics.Process p = new Process();
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();
            String result = p.StandardOutput.ReadToEnd();
            return result;
        }

        private void GetNetworkInterfaces()
        {
            if (this.NetworkInterfaceNamesObj.Count > 0)
            {
                this.NetworkInterfaceNamesObj.Clear();
            }
            String output = this.GetNetshOuput();
            String[] Tokens = output.Split(this.Separators, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder Name = new StringBuilder();
            for (int indexTokens = 2; indexTokens < Tokens.Length; indexTokens++)
            {
                String[] TokensInterface = Tokens[indexTokens].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                String AdminState = TokensInterface[0];
                for (int StartIndex = 3; StartIndex < TokensInterface.Length; StartIndex++)
                {
                    Name.Append(TokensInterface[StartIndex]);
                    if (StartIndex < TokensInterface.Length - 1)
                    {
                        Name.Append(" ");
                    }
                }
                String NicName = Name.ToString();
                Name.Clear();
                this.NetworkInterfaceNamesObj.Add(new NetworkInterfaceInfo(NicName, AdminState));
            }
        }

        public void SetNetworkInterfaceState(String State, String Command)
        {
            if (this.IsNetworkAdapterPresent)
            {
                GetNetworkInterfaces();
                foreach (NetworkInterfaceInfo NicInfo in this.NetworkInterfaceNamesObj)
                {
                    if (NicInfo.GetNicState().ToLower().Equals(State.ToLower()))
                    {
                        ProcessStartInfo psi = new ProcessStartInfo("netsh", "interface set interface \"" + NicInfo.GetNicName() + "\" " + Command);
                        psi.CreateNoWindow = true;
                        psi.WindowStyle = ProcessWindowStyle.Hidden;
                        Process p = new System.Diagnostics.Process();
                        p.StartInfo = psi;
                        p.Start();
                        p.WaitForExit();
                    }
                }
            }
        }
    }

    class NetworkInterfaceInfo
    {
        private readonly String NicName;
        private readonly String NicState;

        public String GetNicName()
        {
            return this.NicName;
        }

        public String GetNicState()
        {
            return this.NicState;
        }

        public NetworkInterfaceInfo(String Name, String State)
        {
            this.NicName = Name;
            this.NicState = State;
        }
    }
}