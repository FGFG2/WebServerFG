using System.Data.Entity;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataHandler.Serializer;
using Microsoft.Owin.Security.DataProtection;
using Unity.WebApi;
using WebServer.BusinessLogic;
using WebServer.Controllers;
using WebServer.DataContext;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<DbContext, ApplicationDbContext>(new HierarchicalLifetimeManager());
            container.RegisterType<UserManager<ApplicationUser>>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new HierarchicalLifetimeManager());
            container.RegisterType(typeof(ISecureDataFormat<>), typeof(SecureDataFormat<>));
            container.RegisterType<ISecureDataFormat<AuthenticationTicket>, SecureDataFormat<AuthenticationTicket>>();
            container.RegisterType<ISecureDataFormat<AuthenticationTicket>, TicketDataFormat>();
            container.RegisterType<IDataSerializer<AuthenticationTicket>, TicketSerializer>();
            container.RegisterInstance(new DpapiDataProtectionProvider().Create("ASP.NET Identity"));
            //container.RegisterType<AccountController>();

            container.RegisterType<IAchievementDb, AchievementDbAbstraction>();
            container.RegisterType<IAchievementCalculatorDetector, AchievementCalculatorDetector>();
            container.RegisterType<IAchievementCalculationManager, AchievementCalculationManager>();
            container.RegisterType<ILoggerFacade, LoggerNLog>(container.Resolve<ContainerControlledLifetimeManager>());


            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}