using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;

namespace MvcPerfmon.SampleWebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //use the custom container to resolve all dependencies
            DependencyResolver.SetResolver(new AutofacDependencyResolver(ServiceLocator.Current.Container));
            //bootstrap the perf counter with your preferred metrics
            ServiceLocator.Current.Container.Resolve<IPerfCounterUtility>().Bootstrap(GetPerfCounterSetup());
        }

        protected void Application_End()
        {
            //ensure any connections/references to perf counters are closed when the worker process dies
            ServiceLocator.Current.Container.Resolve<IPerfCounterUtility>().Cleanup();
        }

        public PerfCounterSetup GetPerfCounterSetup()
        {
            return new PerfCounterSetup()
                {
                    CounterSetupCategories = new PerfCounterSetupCategory[]
                        {
                            new PerfCounterSetupCategory
                                {
                                    CounterCategoryName = "CustomPerfCounter",
                                    CounterCategoryDescription = "This is just a test",
                                    PerfCounterSetupItems = new PerfCounterSetupItem[]
                                        {
                                            new PerfCounterSetupItem
                                                {
                                                    CounterName = "TotalActionCalls",
                                                    CounterDescription = "Testing Total Action Calls",
                                                    PerformanceCounterType = PerformanceCounterType.NumberOfItems64.ToString()
                                                },
                                            new PerfCounterSetupItem
                                                {
                                                    CounterName = "TotalExceptionsOnHomePage",
                                                    CounterDescription = "Testing Total Exceptions",
                                                    PerformanceCounterType = PerformanceCounterType.NumberOfItems64.ToString()
                                                },
                                            new PerfCounterSetupItem
                                                {
                                                    CounterName = "TotalActionCallsPerSecond",
                                                    CounterDescription = "Testing Total Calls/Sec",
                                                    PerformanceCounterType = PerformanceCounterType.RateOfCountsPerSecond32.ToString()
                                                },
                                            new PerfCounterSetupItem
                                                {
                                                    CounterName = "AverageCallTime",
                                                    CounterDescription = "Testing Avg Call Time",
                                                    PerformanceCounterType = PerformanceCounterType.AverageTimer32.ToString()
                                                },
                                            new PerfCounterSetupItem
                                                {
                                                    CounterName = "AverageCallTimeBase",
                                                    CounterDescription = "Testing Avg Call Time",
                                                    PerformanceCounterType = PerformanceCounterType.AverageBase.ToString()
                                                },
                                        }
                                }
                        }
                };

        }
    }
}