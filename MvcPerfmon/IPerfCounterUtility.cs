using System.Collections.Generic;
using System.Diagnostics;

namespace MvcPerfmon
{
    /// <summary>
    /// This should be registered as a singleton by your container
    /// </summary>
    public interface IPerfCounterUtility
    {
        /// <summary>
        /// The named collection of performance counters
        /// </summary>
        Dictionary<string, PerformanceCounter> PerformanceCounters { get; }

        /// <summary>
        /// The real implementation will return true
        /// The mock implementation will return false
        /// </summary>
        bool IsMonitoringEnabled { get; }

        /// <summary>
        /// Initialization method. Usually you want to call this in your global application startup
        /// This will ensure all performance counters are properly installed and initialized
        /// </summary>
        /// <param name="perfCounterSetup">the setup config</param>
        void Bootstrap(PerfCounterSetup perfCounterSetup);

        /// <summary>
        /// Disposal method. Usually you want to call this in your global application end
        /// </summary>
        void Cleanup();
    }
}