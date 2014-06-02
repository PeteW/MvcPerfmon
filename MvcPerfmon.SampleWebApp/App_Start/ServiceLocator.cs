
using Autofac;
using Autofac.Integration.Mvc;

namespace MvcPerfmon.SampleWebApp
{
    /// <summary>
    /// This is just an example of how you can wire dependencies
    /// </summary>
    public class ServiceLocator
    {
        private static ServiceLocator _serviceLocator = null;
        private ServiceLocator()
        {
            var containerBuilder = new ContainerBuilder();
            //the switch between enabling/disabling monitoring is by switching to/from the mock implementation
            containerBuilder.RegisterType<PerfCounterUtility>().As<IPerfCounterUtility>().SingleInstance();
            //            containerBuilder.RegisterType<MockPerfCounterUtility>().As<IPerfCounterUtility>().SingleInstance();
            //this enables autofac to register dependencies in action filter attribute objects
            containerBuilder.RegisterFilterProvider();
            Container = containerBuilder.Build();
        }

        public static ServiceLocator Current
        {
            get
            {
                if (_serviceLocator == null)
                {
                    lock (typeof(ServiceLocator))
                    {
                        if (_serviceLocator == null)
                        {
                            _serviceLocator = new ServiceLocator();
                        }
                    }
                }
                return _serviceLocator;
            }
        }
        public IContainer Container { get; private set; }
    }

}