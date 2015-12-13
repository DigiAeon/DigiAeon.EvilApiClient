using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using DigiAeon.EvilApiClient.Services;
using DigiAeon.EvilApiClient.Services.Interfaces;
using DigiAeon.EvilApiClient.Services.Interfaces.CustomerFile;
using DigiAeon.EvilApiClient.UI.Bootstrapper.Interfaces;
using StructureMap;

namespace DigiAeon.EvilApiClient.UI.Bootstrapper
{
    public sealed class ObjectDependencyResolver : IDependencyResolver
    {
        public ObjectDependencyResolver(IConfig config)
        {
            Container = new Container(registry =>
            {
                registry.For<IConfig>().Use(config);

                registry.For<CustomerFileServiceSettings>().Use(new CustomerFileServiceSettings
                {
                    AllowedFileExtensions = config.CustomerFileAllowedFileExtensions,
                    FolderPath = config.CustomerFileFolderMapPath
                });

                registry.For<EvilApiServiceSettings>().Use(new EvilApiServiceSettings
                {
                    ApiBaseAddress = config.EvilApiApiBaseAddress,
                    UploadUrl = config.EvilApiUploadUrl,
                    UploadCustomerAction = config.EvilApiUploadCustomerAction,
                    GetCustomerUrl = config.EvilApiGetCustomerUrl
                });

                registry.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<ICustomerFileService>();
                    scanner.AssemblyContainingType<CustomerFileService>();
                    scanner.WithDefaultConventions();
                });
            });
        }

        private IContainer Container { get; }

        /// <summary>
        /// The get service.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return serviceType.IsAbstract || serviceType.IsInterface
                         ? Container.TryGetInstance(serviceType)
                         : Container.GetInstance(serviceType);
        }

        /// <summary>
        /// The get services.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The <see cref="T:IEnumerable"/>.
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Container.GetAllInstances(serviceType).Cast<object>();
        }
    }
}
