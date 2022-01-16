using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedClockService
{
    internal class TcpServer
    {
        bool enabled = true;

        static PerformanceCounter cpuUsageCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        static PerformanceCounter freeRamCounter = new PerformanceCounter("Memory", "Available MBytes");
        static double ramAmountMb;

        public static float GetGpuUsage()
        {
            try
            {
                var category = new PerformanceCounterCategory("GPU Engine");
                var counterNames = category.GetInstanceNames();
                var gpuCounters = new List<PerformanceCounter>();
                var result = 0f;

                foreach (string counterName in counterNames)
                {
                    if (counterName.EndsWith("engtype_3D"))
                    {
                        foreach (PerformanceCounter counter in category.GetCounters(counterName))
                        {
                            if (counter.CounterName == "Utilization Percentage")
                            {
                                gpuCounters.Add(counter);
                            }
                        }
                    }
                }

                gpuCounters.ForEach(x => { _ = x.NextValue(); });

                gpuCounters.ForEach(x => { result += x.NextValue(); });

                return result;
            }
            catch { return 0f; }
        }
        static string GetStats()
        {
            return $"{cpuUsageCounter.NextValue():F2};" +
                $"{GetGpuUsage():F2};" +
                $"{ramAmountMb - freeRamCounter.NextValue():0}";
        }

        ManagementObjectSearcher ramAmountSearch = new ManagementObjectSearcher("Select * From Win32_ComputerSystem");
        
        public void Start()
        {
            foreach (var mobject in ramAmountSearch.Get())
            {
                ramAmountMb = Double.Parse(mobject["TotalPhysicalMemory"].ToString()) / 1024 / 1024;
            }

            var firstCall = cpuUsageCounter.NextValue();

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), Constant.HOST_PORT);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listenSocket.Bind(ipPoint);
            listenSocket.Listen(10);

            while (enabled)
            {
                try
                {
                    Socket handler = listenSocket.Accept();

                    string responce = GetStats();
                    byte[] data = Encoding.Unicode.GetBytes(responce);
                    handler.Send(data);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                catch (Exception ex)
                {
                    
                }
            }
        }
        public void Stop()
        {
            enabled = false;
        }
    }
}
