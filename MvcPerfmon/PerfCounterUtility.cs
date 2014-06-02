using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;

namespace MvcPerfmon
{
    public class PerfCounterUtility : IPerfCounterUtility
    {
        public PerfCounterUtility()
        {
            PerformanceCounters = new Dictionary<string, PerformanceCounter>();
        }

        private static object lockObject;

        public Dictionary<string, PerformanceCounter> PerformanceCounters { get; private set; }

        public void Bootstrap(PerfCounterSetup perfCounterSetup)
        {
            try
            {
                foreach (var category in perfCounterSetup.CounterSetupCategories)
                {
                    //load any existing perf metrics so we dont destroy them
                    var existingValues = GetPreservedValues(category.CounterCategoryName, category.PerfCounterSetupItems.Select(x => x.CounterName).ToArray());
                    //delete the category
                    try
                    {
                        PerformanceCounterCategory.Delete(category.CounterCategoryName);
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    //rebuild the category
                    var counterCreationDataCollection = new CounterCreationDataCollection(category.PerfCounterSetupItems.Select(x => x.GetCounterCreationData()).ToArray());
                    PerformanceCounterCategory.Create(category.CounterCategoryName, category.CounterCategoryDescription, PerformanceCounterCategoryType.SingleInstance, counterCreationDataCollection);
                    //load the perf counters into memory and restore any pre-existing values from before they were rebuilt
                    foreach (var perfCounterSetupItem in category.PerfCounterSetupItems)
                    {
                        var c = new PerformanceCounter();
                        c.CategoryName = category.CounterCategoryName;
                        c.CounterName = perfCounterSetupItem.CounterName;
                        //                        c.InstanceName = GetInstanceName();
                        c.ReadOnly = false;
                        //                        c.InstanceLifetime = PerformanceCounterInstanceLifetime.Process;
                        //                        c.RawValue = 0;
                        if (existingValues.ContainsKey(perfCounterSetupItem.CounterName))
                        {
                            c.RawValue = existingValues[perfCounterSetupItem.CounterName];
                        }
                        PerformanceCounters[category.CounterCategoryName + "." + perfCounterSetupItem.CounterName] = c;
                    }
                }
            }
            catch (SecurityException exp)
            {
                throw new SecurityException(string.Format("Error encountered in bootstrap (setting up performance counters): [{0}]. This may be fixed by running in elevated privileges.", exp.Message));
            }
        }

        public void Cleanup()
        {
            lock (lockObject)
            {
                foreach (var pmc in PerformanceCounters)
                {
                    pmc.Value.Dispose();
                }
                PerformanceCounter.CloseSharedResources();
            }
        }

        /// <summary>
        /// When deleting the Category, need to preserve the existing counter values
        /// </summary>
        private Dictionary<string, long> GetPreservedValues(string categoryName, params string[] counterNames)
        {
            if (!PerformanceCounterCategory.Exists(categoryName))
                return new Dictionary<string, long>();
            var preservedValues = new Dictionary<string, long>();
            foreach (var counterName in counterNames)
            {
                if (PerformanceCounterCategory.CounterExists(counterName, categoryName))
                {
                    var performanceCounter = new PerformanceCounter(categoryName, counterName, false);
                    preservedValues.Add(counterName, performanceCounter.RawValue);
                }
            }
            return preservedValues;
        }

        private string GetInstanceName()
        {
            return Assembly.GetCallingAssembly().GetName().Name;
        }

        public bool IsMonitoringEnabled { get { return true; } }
    }
}