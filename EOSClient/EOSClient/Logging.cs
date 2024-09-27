using System;
using IRemote;
using System.IO;
using QuestionLib;
using System.Runtime.Serialization.Formatters.Binary;

namespace EOSClient;
public class EOSLogging
{
    private static string _log = @".\EOSClient.log";
    private static string _flag = $"[{DateTime.Now.ToString()}]";

    public static void InitLogging()
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

    public static void LoggingForURL(string url)
    {
        File.AppendAllText(_log, $"\n{_flag} URL Logging: {url}");
    }

    public static void LoggingMachineInfo(string machineInfo) 
    {
        File.AppendAllText(_log, $"\n{_flag} Machine Info Logging: {machineInfo}");
    }

    public static void LoggingForUserField(string username, string examcode)
    {
        File.AppendAllText(_log, $"\n{_flag} User entered: {(username.Length > 4 ? username.Substring(username.Length - 4) : username)}, Examcode: {examcode}");
    }

    public static void LoggingForError(string error)
    {
        File.AppendAllText(_log, $"\n{_flag} Machine Info Logging:\n{error}\n");
    }

    public static void ExportExamData(string commonMod, EOSData ExamData)
    {
        string ExamFilePath = $"{DateTime.Now.Day.ToString()}-{DateTime.Now.Month.ToString()}-{DateTime.Now.Year.ToString()}_Data_{commonMod}.exam";
        File.AppendAllText(_log, $"\n{_flag} Exporting the exam data with mod {commonMod} to: {ExamFilePath}");
        using (FileStream ExamBinary = new FileStream(ExamFilePath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ExamBinary, ExamData);
        }
    }

    public static void ExportRegisterData(RegisterData registerData)
    {
        string ExportRegisterDataPath = $"{DateTime.Now.Day.ToString()}-{DateTime.Now.Month.ToString()}-{DateTime.Now.Year.ToString()}_Data.registerData";
        File.AppendAllText(_log, $"\n{_flag} Exporting the registered data to: {ExportRegisterDataPath}");
        using (FileStream ExamBinary = new FileStream(ExportRegisterDataPath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ExamBinary, registerData);
        }
    }

    public static void ExportGUIData(byte[] GUI)
    {
        string ExportGUI = $"{DateTime.Now.Day.ToString()}-{DateTime.Now.Month.ToString()}-{DateTime.Now.Year.ToString()}_GUI_Data.gzip";
        File.AppendAllText(_log, $"\n{_flag} Exporting the GUI Data to: {ExportGUI}");
        using (FileStream ExamBinary = new FileStream(ExportGUI, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ExamBinary, GUI);
        }
    }

    public static void ExportServerInfo(ServerInfo serverInfo)
    {
        string exportServerInfo = $"{DateTime.Now.Day.ToString()}-{DateTime.Now.Month.ToString()}-{DateTime.Now.Year.ToString()}_Server_Info.txt";
        File.AppendAllText(_log, $"\n{_flag} Exporting Server Data to: {exportServerInfo}");
        //Server Info
        File.AppendAllText(exportServerInfo,
                "========================\n" +
                $"Server Info, logging date {DateTime.Now.ToString()}:\n" +
                $"Server Info: {serverInfo.Version}\n" +
                $"Server IP: {serverInfo.IP}\n" + 
                $"Server Connection Port: {serverInfo.Port.ToString()}\n" +
                $"Server Alias: {serverInfo.ServerAlias}\n" +
                $"Server + Monitor IP:Port: {serverInfo.MonitorServer_IP}:{serverInfo.MonitorServer_Port}\n" +
                $"IP Range WLAN: {serverInfo.IP_Range_WLAN}\n" +
                $"Public IP: {serverInfo.Public_IP}\n" +
                $"Logging Server ended."
            );
    }
}
