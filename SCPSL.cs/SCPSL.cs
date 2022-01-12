using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;
using Newtonsoft.Json.Linq;

namespace WindowsGSM.Plugins
{
    public class SCPSL : SteamCMDAgent
    {
        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.SCPSL", // WindowsGSM.XXXX
            author = "Oscar Hurst",
            description = "⚠Work-In-Progress⚠ 🧩 WindowsGSM plugin for supporting SCP:SL Servers",
            version = "1.0",
            url = "https://github.com/Voidoz/WindowsGSM.SCPSL", // Github repository link (Best practice)
            color = "#FF9966" // Color Hex
        };


        // - Standard Constructor and properties
        public SCPSL(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData;

        // SteamCMD Settings
        public override bool loginAnonymous => true; // true if allows login anonymous on steamcmd, else false
        public override string AppId => "996560"; // Value of app_update <AppId>

        // WGSM Settings
        public override string StartPath => @"LocalAdmin.exe"; // Game server start path
        public string FullName = "SCP:SL Dedicated Server"; // Game server FullName
        public bool AllowsEmbedConsole = false;  // Does this server support output redirect?
        public int PortIncrements = 1; // This tells WindowsGSM how many ports should skip after installation
        public object QueryMethod = null; // Query method. Accepted value: null or new A2S() or new FIVEM() or new UT3()

        // Port Settings
        public string Port = "7777"; // Default port
        public string QueryPort = "7777"; // Default query port


        public string Maxplayers = "See Config"; // Found in the game config

        // Additional command line arguments
        public string Additional = ""; // Additional server start parameter

        // Update Settings
        public bool AutoUpdate = true;
        public bool UpdateOnStart = true;

        public async Task<Process> Start()
        {
            // Prepare start parameter
            var param = new StringBuilder();
            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerPort) ? string.Empty : $" {_serverData.ServerPort}");
            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerName) ? string.Empty : $" {_serverData.ServerParam}");

            // Prepare Process
            var p = new Process
            {
                StartInfo =
                {
                    WindowStyle = ProcessWindowStyle.Minimized,
                    UseShellExecute = false,
                    WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID),
                    FileName = ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath),
                    Arguments = param.ToString()
                },
                EnableRaisingEvents = true
            };

            // TODO Get the embedded console working
            /*if (AllowsEmbedConsole)
            {
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                var serverConsole = new ServerConsole(_serverData.ServerID);
                p.OutputDataReceived += serverConsole.AddOutput;
                p.ErrorDataReceived += serverConsole.AddOutput;

                // Start Process
                try // Process ID isn't being returned yet. It seems to hang on start. WindowsGSM says "Starting" forever
                {
                    p.Start();
                }
                catch (Exception e)
                {
                    Error = e.Message;
                    return null; // return null if fail to start
                }
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                return p;

            }*/

            // Start Process
            try
            {
                p.Start();
                return p;
            }
            catch (Exception e)
            {
                base.Error = e.Message;
                return null;
            }
        }

        public async Task Stop(Process p)
        {
            await Task.Run(() =>
            {
                ServerConsole.SendMessageToMainWindow(p.MainWindowHandle, "quit");
                //p.Kill();
            });
        }
    }
}
