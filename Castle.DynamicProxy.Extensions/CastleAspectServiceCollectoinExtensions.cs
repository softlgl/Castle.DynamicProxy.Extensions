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
                if ((item.ServiceType.IsGenericType && item.ServiceType.Name.Contains("IOption"))
                    || (item.ServiceType.IsGenericType && item.ServiceType.Name.Contains("ILogger")))
                {
                    dynamciServices.Add(item);
                    continue;
                }
                var target = oldProvider.GetService(item.ServiceType);
                var targetType = target.GetType();
                var anyAttribute = targetType.GetCustomAttributes(true).Any(i => typeof(AbstractInterceptorAttribute).IsAssignableFrom(i.GetType()))
                && targetType.GetMethods().Any(i => i.GetCustomAttributes(true).Any(i => typeof(AbstractInterceptorAttribute).IsAssignableFrom(i.GetType())));
                if (anyAttribute)
                {
                    Func<IServiceProvider, object> factory = serviceProvider =>
                    {
                        return proxyGenerator.CreateInterfaceProxyWithTarget(item.ServiceType, target, new AbstractInterceptor(dynamicProvider));
                    };
                    dynamciServices.Add(ServiceDescriptor.Describe(item.ServiceType, factory, item.Lifetime));
                    continue;
                }
                dynamciServices.Add(item);
            }
            return dynamicProvider = dynamciServices.BuildServiceProvider();
        }
    }
}
