using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using WebServer.BusinessLogic;
using WebServer.DataContext;
using WebServer.Logging;

namespace WebServer
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers
            container.RegisterType<IAchievementDb, AchievementDbAbstraction>();
            container.RegisterType<IAchievementCalculatorDetector, AchievementCalculatorDetector>();
            container.RegisterType<IAchievementCalculationManager, AchievementCalculationManager>(container.Resolve<ContainerControlledLifetimeManager>());
            container.RegisterType<ILoggerFacade, LoggerNLog>(container.Resolve<ContainerControlledLifetimeManager>());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}