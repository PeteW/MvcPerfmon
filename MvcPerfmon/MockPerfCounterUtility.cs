using System.Collections.Generic;
using System.Diagnostics;

namespace MvcPerfmon
{
    public class MockPerfCounterUtility : IPerfCounterUtility
    {
        public Dictionary<string, PerformanceCounter> PerformanceCounters { get; private set; }
        public bool IsMonitoringEnabled { get { return false; } }
        public void Bootstrap(PerfCounterSetup perfCounterSetup)
        {
            PerformanceCounters = new Dictionary<string, PerformanceCounter>();
        }

        public void Cleanup()
        {
        }
    }
}