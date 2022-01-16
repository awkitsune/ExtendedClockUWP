using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Sockets;

namespace ExtendedClockServer
{
    class Program
    {
        static PerformanceCounter cpuUsageCounter;
        static PerformanceCounter freeRamCounter;
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

                gpuCounters.ForEach(x =>
                {
                    _ = x.NextValue();
                });

                gpuCounters.ForEach(x =>
                {
                    result += x.NextValue();
                });

                return result;
            }
            catch
            {
                return 0f;
            }
        }

        static string GetStats()
        {
            return $"{cpuUsageCounter.NextValue():F2};" +
                $"{GetGpuUsage():F2};" +
                $"{ramAmountMb - freeRamCounter.NextValue():0}";
        }
        static void Main(string[] args)
        {
            cpuUsageCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            freeRamCounter = new PerformanceCounter("Memory", "Available MBytes");

            ManagementObjectSearcher Search = new ManagementObjectSearcher("Select * From Win32_ComputerSystem");
            foreach (var mobject in Search.Get())
            {
                ramAmountMb = Double.Parse(mobject["TotalPhysicalMemory"].ToString()) / 1024 / 1024;
            }

            var firstCall = cpuUsageCounter.NextValue();

            Console.WriteLine("Started");

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Constant.HOST_PORT);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(10);
                while (true)
                {
                    try
                    {
                        Socket handler = listenSocket.Accept();
                        Console.WriteLine("Connected");

                        var sb = new StringBuilder();
                        byte[] data = new byte[256];

                        string responce = GetStats();
                        data = Encoding.Unicode.GetBytes(responce);
                        handler.Send(data);
                        Console.WriteLine($"Sent {data.Length} bytes --- {responce}");
                        handler.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
