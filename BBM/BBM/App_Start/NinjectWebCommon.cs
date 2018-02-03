[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(BBM.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(BBM.App_Start.NinjectWebCommon), "Stop")]

namespace BBM.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Business.Repository;
    using Business.Logic;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            #region Repository
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            #endregion

            #region Business
            kernel.Bind<IOrderBusiness>().To<OrderBusiness>();
            kernel.Bind<IBrachesBusiness>().To<BrachesBusiness>();
            kernel.Bind<ICatalogBusiness>().To<CatalogBusiness>();
            kernel.Bind<IChannelBusiness>().To<ChannelBusiness>();
            kernel.Bind<ICustomerBusiness>().To<CustomerBusiness>();
            kernel.Bind<IDiscountBusiness>().To<DiscountBusiness>();
            kernel.Bind<IImportBusiness>().To<ImportBusiness>();
            kernel.Bind<ISuppliersBusiness>().To<SuppliersBusiness>();
            kernel.Bind<IProductBusiness>().To<ProductBusiness>();
            kernel.Bind<INotificaitonBusiness>().To<NotificaitonBusiness>();
            kernel.Bind<IApiBusiness>().To<ApiBusiness>();
            
            #endregion

        }
    }
}
