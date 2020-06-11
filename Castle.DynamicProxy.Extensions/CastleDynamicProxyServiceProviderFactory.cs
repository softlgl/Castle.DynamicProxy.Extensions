using System;
using Microsoft.Extensions.DependencyInjection;

namespace Castle.DynamicProxy.Extensions
{
    public class CastleDynamicProxyServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            return containerBuilder.BuildCastleDynamicProxyProvider();
        }
    }
}
