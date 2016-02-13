using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Threading;
using System.Windows;

namespace Webcam_Blocker
{
    class DriverClass
    {
        public enum Status { ENABLE, DISABLE }
        public enum Type { Audio, Image }

        #region Initializing Variables

        //Devcon Return Statement
        private const string strNotFound = "no matching devices found.";
        private const string strRunningStatus = "driver is running.";
        //Status
        private const string strGetAudioStatusClass = "status =audioendpoint";
        private const string strGetImageStatusclass = "status =image";
        //Disable
        private const string strDisableAudioClass = "DISABLE =audioendpoint";
        private const string strDisableImageClass = "DISABLE =image";
        //Enable
        private const string strEnableAudioClass = "ENABLE =audioendpoint";
        private const string strEnableImageClass = "ENABLE =image";
        //When HardwareID is available
        private string strStatusOfDevice = "";
        private string strEnableThisDevice = "";
        private string strDisableThisDevice = "";
        string hwid = "";

        Status status;
        Type type;

        #endregion

        public DriverClass(Type deviceType)
        {
            this.setType(deviceType);
        }

        public bool Enable()
        {
            ArrayList strOutputArray;
            if (this.type == Type.Image) { strOutputArray = DevConCommand(strEnableImageClass); }
            else { strOutputArray = DevConCommand(strEnableThisDevice); }
            string strLine = (string)strOutputArray[0];

            if (strLine.ToLower() == strNotFound) { return false; }
            else { return true; }
        }

        public bool Disable()
        {
            ArrayList strOutputArray;
            if (this.type == Type.Image) { strOutputArray = DevConCommand(strDisableImageClass); }
            else { strOutputArray = DevConCommand(strDisableThisDevice); }
            string strLine = (string)strOutputArray[0];

            if (strLine.ToLower() == strNotFound) { return false; }
            else { return true; }
        }

        public void CheckStatus()
        {
            if (type == Type.Image) { GetWebcamStatus(); }
            else { GetMicrophoneStatus(); }
        }

        private void GetMicrophoneStatus()
        {
            int NumberOfDevices;
            ArrayList strOutputArray;

            if (this.hwid == "") { strOutputArray = DevConCommand(strGetAudioStatusClass); }
            else { strOutputArray = DevConCommand(this.strStatusOfDevice); }

            string strDeviceLine = (string)strOutputArray[strOutputArray.Count - 1];
            string strDeviceCount = strDeviceLine.Substring(0, 1);

            if (!int.TryParse(strDeviceCount, out NumberOfDevices))
            {
                Debug.WriteLine("Failed to Convert '{0}' to {1}.", strDeviceLine, NumberOfDevices);
                MessageBox.Show("Cannot Find NUmber Of Devices");
            }
            else
            {
                string strName;
                string strhwids;
                string strStatus;

                for (int i = 1; i <= NumberOfDevices; i++)
                {
                    int temp = (3 * i) - 1;

                    strName = (string)strOutputArray[temp - 1];
                    strhwids = (string)strOutputArray[temp - 2];
                    strStatus = (string)strOutputArray[temp];

                    if (strName.Contains("Microphone"))
                    {
                        setHWID(strhwids);
                        if (strStatus.Trim().ToLower() == strRunningStatus) { setStatus(Status.ENABLE); }
                        else { setStatus(Status.DISABLE); }
                        break;
                    }
                }
            }
        }

        private void GetWebcamStatus()
        {
            int NumberOfDevices;

            ArrayList strOutputArray = DevConCommand(strGetImageStatusclass);
            string strDeviceLine = (string)strOutputArray[strOutputArray.Count - 1];

            if (!int.TryParse(strDeviceLine.Substring(0, 1), out NumberOfDevices))
            {
                Debug.WriteLine("Failed to Convert '{0}' to {1}.", strDeviceLine, NumberOfDevices);
                MessageBox.Show("Cannot Find NUmber Of Devices");
            }
            else
            {
                for (int i = 1; i <= NumberOfDevices; i++)
                {
                    int temp = (3 * i) - 1;
                    string strTemp = (string)strOutputArray[temp];
                    if (strTemp.Trim().ToLower() == strRunningStatus)
                    {
                        this.status = Status.ENABLE;
                    }
                    else
                    {
                        this.status = Status.DISABLE;
                        break;
                    }
                }
            }
        }

        private string GetDevConPath()
        {
            //checks the mode of the app then decides what to initiate path as
            string path = @"..\..\ExecutableFiles\DevCon.exe";
            return path;

        }

        public void setType(DriverClass.Type driverType) { type = driverType; }

        public Type getType() { return type; }

        public Status getStatus() { return status; }

        public string getHWID() { return hwid; }

        public void setStatus(Status value) { status = value; }

        public void setHWID(string value)
        {
            if (!value.StartsWith("@")) { value = "@" + value; }

            this.hwid = value;
            this.strStatusOfDevice = "Status " + this.hwid;
            this.strEnableThisDevice = "Enable " + this.hwid;
            this.strDisableThisDevice = "Disable " + this.hwid;

        }

        private ArrayList DevConCommand(string strCommand)
        {
            ProcessStartInfo startInfo;
            string path = GetDevConPath();
            startInfo = new ProcessStartInfo(path, strCommand);
            startInfo.WorkingDirectory = @"C:\Windows\System32";
            startInfo.CreateNoWindow = true;
            startInfo.Verb = "runas";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            ArrayList strOutputArray = new ArrayList();
            do
            {
                strOutputArray.Add(process.StandardOutput.ReadLine());
            } while (!process.StandardOutput.EndOfStream);

            process.Close();
            return strOutputArray;
        }


    }
}
