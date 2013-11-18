using System;
using System.Web;
using Bifrost.Configuration;
using Bifrost.Execution;
using Bifrost.Ninject;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Bifrost.ContainerCreator), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Bifrost.ContainerCreator), "Stop")]

namespace Bifrost
{
    public class ContainerCreator : ICanCreateContainer
    {
        static Bootstrapper _bootstrapper = new Bootstrapper();
        static IKernel _kernel;

        static ContainerCreator()
        {
            _kernel = new StandardKernel();
            _kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            _kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
        }

        public IContainer CreateContainer()
        {
            var container = new Container(_kernel);
            return container;
        }

        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            _bootstrapper.Initialize(()=>_kernel);
        }

        public static void Stop()
        {
            _bootstrapper.ShutDown();
        }
    }
}