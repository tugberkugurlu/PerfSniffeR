using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using PerfSniffeR.Web.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace PerfSniffeR.Web.Infrastructure
{
    public sealed class PerfMonitor
    {
        private readonly int _sleepIntervalInMilliSecs;
        public static readonly IEnumerable<PerformanceCounter> ServiceCounters = new[]
        {
            new PerformanceCounter("Processor Information", "% Processor Time", "_Total"),
            new PerformanceCounter("MSSQL$SQLEXPRESS:Access Methods", "Page Splits/sec"),
            new PerformanceCounter("MSSQL$SQLEXPRESS:Buffer Manager", "Buffer cache hit ratio"),
            new PerformanceCounter("MSSQL$SQLEXPRESS:Buffer Manager", "Checkpoint pages/sec"),
            new PerformanceCounter("MSSQL$SQLEXPRESS:Buffer Manager", "Page life expectancy"),
            new PerformanceCounter("MSSQL$SQLEXPRESS:General Statistics", "Processes blocked"),
            new PerformanceCounter("MSSQL$SQLEXPRESS:General Statistics", "User Connections"),
            new PerformanceCounter("MSSQL$SQLEXPRESS:Locks", "Lock Waits/sec", "_Total"),
            new PerformanceCounter("MSSQL$SQLEXPRESS:SQL Statistics", "Batch Requests/sec"),
            new PerformanceCounter("MSSQL$SQLEXPRESS:SQL Statistics", "SQL Compilations/sec")                                                      
        };

        public PerfMonitor(int sleepIntervalInMilliSecs)
        {
            _sleepIntervalInMilliSecs = sleepIntervalInMilliSecs;
        }

        public async Task MonitorAsync()
        {
            IHubConnectionContext clients = GlobalHost.ConnectionManager.GetHubContext<PerfCountersHub>().Clients;

            while (true)
            {
                try
                {
                    await Task.Delay(_sleepIntervalInMilliSecs);
                    List<PerfCounterSampleCarrier> counterSamples = new List<PerfCounterSampleCarrier>();
                    foreach (PerformanceCounter performanceCounter in ServiceCounters)
                    {
                        try
                        {
                            float counterValue = performanceCounter.NextValue();
                            Trace.TraceInformation("Performance counter {0} read value: {1}.", GetPerfCounterPath(performanceCounter), counterValue.ToString(CultureInfo.GetCultureInfo("en-US")));
                            counterSamples.Add(new PerfCounterSampleCarrier
                            {
                                FullName = GetPerfCounterPath(performanceCounter),
                                MachineName = performanceCounter.MachineName,
                                CategoryName = performanceCounter.CategoryName,
                                CounterName = performanceCounter.CounterName,
                                InstanceName = performanceCounter.InstanceName,
                                Value = counterValue
                            });
                        }
                        catch (InvalidOperationException ex)
                        {
                            Trace.TraceInformation("Performance counter {0} didn't send any value.", GetPerfCounterPath(performanceCounter));
                        }
                    }

                    clients.All.countersReceived(counterSamples);
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("Error while trying to retrieve the performance counters. Error: {0}", ex);
                }
            }
        }

        private static string GetPerfCounterPath(PerformanceCounter performanceCounter)
        {
            return string.Format(@"{0}\{1}\{2}\{3}", 
                performanceCounter.MachineName, 
                performanceCounter.CategoryName, 
                performanceCounter.CounterName, 
                performanceCounter.InstanceName);
        }
    }

    public class PerfCounterSampleCarrier
    {
        public string FullName { get; set; }
        public string MachineName { get; set; }
        public string CategoryName { get; set; }
        public string CounterName { get; set; }
        public string InstanceName { get; set; }
        public float Value { get; set; }
    }
}