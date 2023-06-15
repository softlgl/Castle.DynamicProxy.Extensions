using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Castle.DynamicProxy.Extensions
{
    public static class CastleDynamicProxyServiceCollectoinExtensions
    {
        public static IServiceProvider BuildCastleDynamicProxyProvider(this IServiceCollection services)
        {
            ServiceProvider oldProvider = services.BuildServiceProvider();

            IServiceCollection dynamciServices = new ServiceCollection();
            dynamciServices.AddSingleton<AbstractInterceptor>();
            dynamciServices.AddSingleton<ProxyGenerator>();

            foreach (ServiceDescriptor item in services)
            {
                Type implementationType;
                if (IsProxyType(item, oldProvider, out implementationType))
                {
                    object implementationFactory(IServiceProvider serviceProvider)
                    {
                        var proxyGenerator = serviceProvider.GetRequiredService<ProxyGenerator>();
                        var abstractInterceptor = serviceProvider.GetRequiredService<AbstractInterceptor>(); ;
                        if (item.ImplementationType != null)
                        {
                            var targetType = item.ServiceType.IsInterface ? proxyGenerator.ProxyBuilder.CreateInterfaceProxyTypeWithTarget(item.ServiceType, new Type[] { item.ServiceType }, item.ImplementationType, ProxyGenerationOptions.Default)
                             : proxyGenerator.ProxyBuilder.CreateClassProxyType(item.ImplementationType, new Type[] { item.ServiceType }, ProxyGenerationOptions.Default);
                            var target = ActivatorUtilities.CreateInstance(serviceProvider, item.ImplementationType);
                            List<object> constructorArguments = GetConstructorArguments(target, ProxyGenerationOptions.Default, abstractInterceptor);
                            return Activator.CreateInstance(targetType, constructorArguments.ToArray());
                        }
                        else if (item.ImplementationInstance != null)
                        {
                            if (item.ServiceType.IsInterface)
                            {
                                return proxyGenerator.CreateInterfaceProxyWithTarget(item.ServiceType, item.ImplementationInstance, abstractInterceptor);
                            }
                            return proxyGenerator.CreateClassProxyWithTarget(item.ServiceType, item.ImplementationInstance, abstractInterceptor);
                        }
                        else if (item.ImplementationFactory != null)
                        {
                            if (item.ServiceType.IsInterface)
                            {
                                return proxyGenerator.CreateInterfaceProxyWithTarget(item.ServiceType, item.ImplementationFactory.Invoke(serviceProvider), abstractInterceptor);
                            }
                            return proxyGenerator.CreateClassProxyWithTarget(item.ServiceType, item.ImplementationFactory.Invoke(serviceProvider), abstractInterceptor);
                        }
                        else
                        {
                            return default;
                        }
                    }

                    dynamciServices.Add(ServiceDescriptor.Describe(item.ServiceType, implementationFactory, item.Lifetime));
                    continue;
                }
                dynamciServices.Add(item);
            }

            return dynamciServices.BuildServiceProvider();
        }

        private static Type GetImplementationType(ServiceDescriptor descriptor, ServiceProvider serviceProvider)
        {
            if (descriptor.ImplementationType != null)
            {
                return descriptor.ImplementationType;
            }
            else if (descriptor.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance.GetType();
            }
            else if (descriptor.ImplementationFactory != null)
            {
                var typeArguments = descriptor.ImplementationFactory.Invoke(serviceProvider);
                return typeArguments.GetType();
            }
            return null;
        }

        private static bool IsProxyType(ServiceDescriptor descriptor, ServiceProvider serviceProvider, out Type implementType)
        {
            implementType = GetImplementationType(descriptor, serviceProvider);

            if (implementType == null)
            {
                throw new ArgumentNullException(nameof(implementType));
            }
            return implementType.IsDefined(typeof(AbstractInterceptorAttribute)) 
                || implementType.GetMethods().Any(i => i.GetCustomAttributes(true).Any(i => typeof(AbstractInterceptorAttribute).IsAssignableFrom(i.GetType())));

        }

        private static List<object> GetConstructorArguments(object target, ProxyGenerationOptions options, params IInterceptor[] interceptors)
        {
            List<object> list = new List<object>(options.MixinData.Mixins) { interceptors, target };
            if (options.Selector != null)
            {
                list.Add(options.Selector);
            }

            return list;
        }
    }
}
