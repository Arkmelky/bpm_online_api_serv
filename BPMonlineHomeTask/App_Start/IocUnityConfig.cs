using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BPMonlineHomeTask.Controllers;
using BPMonlineHomeTask.Infrastructure;
using BPMonlineHomeTask.Models.Authenticate;
using BPMonlineHomeTask.Models.ServiceBPMonline;
using Microsoft.Practices.Unity;

namespace BPMonlineHomeTask.App_Start
{
    public static class IocUnityConfig
    {
        public static void ConfigureUnityContainer()
        {
            IUnityContainer unityContainer = new UnityContainer();

            DiBindings(unityContainer);

            DependencyResolver.SetResolver(new UnityDependencyResolver(unityContainer));
        }

        private static void DiBindings(IUnityContainer unity)
        {
            unity.RegisterType<IUserAuthModel, UserAuthModel>();
            unity.RegisterType<IUserAuthManager, UserAuthManager>();
            unity.RegisterType<IBPMonlineServiceManager, BPMonlineServiceManager>();

            unity.RegisterType<HomeController>(new InjectionConstructor(new UserAuthModel(), new UserAuthManager(),new BPMonlineServiceManager()));
        }
    }
}