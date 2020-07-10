using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Castle.DynamicProxy.Extensions
{
    public static class CastleDynamicProxyServiceCollectoinExtensions
    {
        public static IServiceProvider BuildCastleDynamicProxyProvider(this IServiceCollection services)
        {
            IServiceProvider oldProvider = services.BuildServiceProvider();
            ProxyGenerator proxyGenerator = new ProxyGenerator();

            IServiceProvider dynamicProvider = null;
            IServiceCollection dynamciServices = new ServiceCollection();
            foreach (ServiceDescriptor item in services)
            {
                Func<IServiceProvider, object> factory = serviceProvider =>
                {
                    var target = oldProvider.GetService(item.ServiceType);
                    var targetType = target.GetType();
                    var anyAttribute = targetType.GetCustomAttributes(true).Any(i => typeof(AbstractInterceptorAttribute).IsAssignableFrom(i.GetType()))
                    && targetType.GetMethods().Any(i=>i.GetCustomAttributes(true).Any(i => typeof(AbstractInterceptorAttribute).IsAssignableFrom(i.GetType())));
                    if (anyAttribute)
                    {
                        return proxyGenerator.CreateInterfaceProxyWithTarget(item.ServiceType, target, new AbstractInterceptor(dynamicProvider));
                    }
                    return target;
                };
                dynamciServices.Add(ServiceDescriptor.Describe(item.ServiceType, factory, item.Lifetime));
            }
            return dynamicProvider = dynamciServices.BuildServiceProvider();
        }
    }
}
