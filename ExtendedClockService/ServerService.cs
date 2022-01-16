using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExtendedClockService
{
    public partial class ServerService : ServiceBase
    {
        TcpServer tcpServer;
        public ServerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            tcpServer = new TcpServer();
            Thread tcpServerThread = new Thread(new ThreadStart(tcpServer.Start));
            tcpServerThread.Start();
        }

        protected override void OnStop()
        {
            tcpServer.Stop();
            Thread.Sleep(500);
        }

        public void AddLog(string log)
        {
            try
            {
                if (!EventLog.SourceExists("ExtendedClockServerService"))
                {
                    EventLog.CreateEventSource("ExtendedClockServerService", "ExtendedClockServerService");
                }
                eventLog1.Source = "ExtendedClockServerService";
                eventLog1.WriteEntry(log);
            }
            catch { }
        }

    }
}
