using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace UpcomingGames
{
    public class HealthCheck
    {
        public Version supportedVersion = new Version("10.0.18463");
        public Version supportedFrameworkVersion = new Version("4.7");
        public Version windowsversion;
        public Version frameworkversion;

        public void WindowsVersionAndBuild()
        {
            Process pProcess = new Process();
            pProcess.StartInfo.FileName = "cmd.exe";
            pProcess.StartInfo.Arguments = @"/c wmic os get Version,BuildNumber /value";
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;

            //Start the process
            pProcess.Start();

            //Get program output
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            string Build = strOutput.Split('\n')[2];
            int buildver = Int32.Parse(Regex.Replace(Build, "[^.0-9]", ""));
            string Version = strOutput.Split('\n')[3];
            string ver = Regex.Replace(Version, "[^.0-9]", "");
            var version = new Version(ver);
            pProcess.WaitForExit();
            if (version >= supportedVersion)
            {
                windowsversion = version;
            }
            else
            {
                windowsversion = version;
            }
            //Wait for process to finish
        }

        public void FrameworkVersion()
        {
            if (Type.GetType("System.ServiceModel.Description.ServiceHealthBehavior, System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", false) != null)
            {
                frameworkversion = new Version("4.8");
            }
            else if (Type.GetType("System.Web.Caching.CacheInsertOptions, System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false) != null)
            {
                frameworkversion = new Version("4.7");
            }
            else if (Type.GetType("System.Security.Cryptography.AesCng, System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", false) != null)
            {
                frameworkversion = new Version("4.6.2");
            }
            else if (Type.GetType("System.Data.SqlClient.SqlColumnEncryptionCngProvider, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", false) != null)
            {
                frameworkversion = new Version("4.6.1");
            }
            else if (Type.GetType("System.AppContext, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", false) != null)
            {
                frameworkversion = new Version("4.6");
            }
        }

        public HealthCheck()
        {
            WindowsVersionAndBuild();
            FrameworkVersion();
        }

    }
}