using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcPerfmon
{
    /// <summary>
    /// Extend this class to make a performance tracker
    /// </summary>
    public abstract class MvcPerformanceTrackerAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// This needs to be injected by your container.
        /// </summary>
        public IPerfCounterUtility PerfCounterUtility { get; set; }

        /// <summary>
        /// Extend this in your class
        /// </summary>
        /// <returns>The context object to use in action complete</returns>
        protected abstract object OnActionStart();

        /// <summary>
        /// Extend this in your class
        /// </summary>
        /// <param name="context">The context object created at the beginning of the call</param>
        /// <param name="isExceptionThrown">Was an exception thrown</param>
        protected abstract void OnActionComplete(object context, bool isExceptionThrown);

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //exit if monitoring is disabled
            if (!PerfCounterUtility.IsMonitoringEnabled)
                return;
            //call onstart and save context data
            HttpContext.Current.Items[GetUniqueId(filterContext.ActionDescriptor)] = OnActionStart();
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //exit if monitoring is disabled
            if (!PerfCounterUtility.IsMonitoringEnabled)
                return;
            //grab context data if you can
            object context = null;
            if (HttpContext.Current.Items.Contains(GetUniqueId(filterContext.ActionDescriptor)))
            {
                context = HttpContext.Current.Items[GetUniqueId(filterContext.ActionDescriptor)];
            }
            //call async to minimize blocking
            Task.Factory.StartNew(() => OnActionComplete(context, filterContext.Exception != null));
        }

        /// <summary>
        /// Get a perf counter by a key
        /// </summary>
        protected PerformanceCounter GetPerformanceCounterByKey(string key)
        {
            if (!PerfCounterUtility.PerformanceCounters.ContainsKey(key))
                throw new Exception("Unable to find performance counter: [" + key + "]");
            return PerfCounterUtility.PerformanceCounters[key];
        }

        /// <summary>
        /// Gets a unique ID that can be used to identify the specific call from beginning to end
        /// </summary>
        private string GetUniqueId(ActionDescriptor actionDescriptor)
        {
            return GetType().FullName + ":" + actionDescriptor.UniqueId;
        }
    }
}