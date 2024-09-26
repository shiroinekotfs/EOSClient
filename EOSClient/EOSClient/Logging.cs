using System;
using System.IO;

namespace EOSClient
{
    internal class Logging
    {
        private string _log = @".\EOSClient.log";
        private string _flag = $"[{DateTime.Now.ToString()}]";

        public void InitLogging()
        {
            try
            {
                File.Delete(_log);
                File.WriteAllText(_log, "Starting EOS Client 23.03.20.24 Logging...\n");
            } catch 
            {
                File.WriteAllText(_log, "Starting EOS Client 23.03.20.24 Logging...\n");
            }
        }

        public void LoggingForURL(string url)
        {
            File.AppendAllText(_log, $"\n{_flag} URL Logging: {url}");
        }

        public void LoggingMachineInfo(string machineInfo) 
        {
            File.AppendAllText(_log, $"\n{_flag} Machine Info Logging: {machineInfo}");
        }

        public void LoggingForUserField(string username, string examcode)
        {
            File.AppendAllText(_log, $"\n{_flag} User entered: {username}, Password: *****, Examcode: {examcode}");
        }


    }
}
