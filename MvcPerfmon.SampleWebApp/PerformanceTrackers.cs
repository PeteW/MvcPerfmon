using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace MvcPerfmon.SampleWebApp
{
    public class TotalActionCallsPerformanceTrackerAttribute : MvcPerformanceTrackerAttribute
    {
        protected override object OnActionStart()
        {
            return null;
        }

        protected override void OnActionComplete(object context, bool isExceptionThrown)
        {
            var performanceCounter = GetPerformanceCounterByKey("CustomPerfCounter.TotalActionCalls");
            performanceCounter.Increment();
        }
    }
    public class TotalExceptionsPerformanceTrackerAttribute : MvcPerformanceTrackerAttribute
    {
        protected override object OnActionStart()
        {
            return null;
        }

        protected override void OnActionComplete(object context, bool isExceptionThrown)
        {
            var performanceCounter = GetPerformanceCounterByKey("CustomPerfCounter.TotalExceptionsOnHomePage");
            if (isExceptionThrown)
                performanceCounter.Increment();
        }
    }

    public class AverageCallTimePerformanceTrackerAttribute : MvcPerformanceTrackerAttribute
    {
        protected override object OnActionStart()
        {
            return Stopwatch.StartNew();
        }

        protected override void OnActionComplete(object context, bool isExceptionThrown)
        {
            var stopwatch = context as Stopwatch;
            stopwatch.Stop();
            var performanceCounterBase = GetPerformanceCounterByKey("CustomPerfCounter.AverageCallTimeBase");
            var performanceCounter = GetPerformanceCounterByKey("CustomPerfCounter.AverageCallTime");
            performanceCounterBase.Increment();
            performanceCounter.IncrementBy(stopwatch.ElapsedTicks);
        }
    }

    public class TotalActionCallsPerSecondPerformanceTrackerAttribute : MvcPerformanceTrackerAttribute
    {
        protected override object OnActionStart()
        {
            return null;
        }

        protected override void OnActionComplete(object context, bool isExceptionThrown)
        {
            var performanceCounter = GetPerformanceCounterByKey("CustomPerfCounter.TotalActionCallsPerSecond");
            performanceCounter.Increment();
        }
    }
}