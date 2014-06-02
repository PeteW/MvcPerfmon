using System;
using System.Diagnostics;

namespace MvcPerfmon
{
    public class PerfCounterSetup
    {
        public PerfCounterSetupCategory[] CounterSetupCategories { get; set; }
    }

    public class PerfCounterSetupCategory
    {
        public string CounterCategoryName { get; set; }
        public string CounterCategoryDescription { get; set; }
        public PerfCounterSetupItem[] PerfCounterSetupItems { get; set; }
    }

    public class PerfCounterSetupItem
    {
        public string CounterName { get; set; }
        public string CounterDescription { get; set; }
        public string PerformanceCounterType { get; set; }

        public CounterCreationData GetCounterCreationData()
        {
            return new CounterCreationData(CounterName, CounterDescription, (PerformanceCounterType)Enum.Parse(typeof(PerformanceCounterType), PerformanceCounterType));
        }
    }
}