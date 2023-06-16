using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Castle.DynamicProxy.Extensions
{
    public static class CastleDynamicProxyServiceCollectoinExtensions
    {
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

        public static IServiceProvider BuildCastleDynamicProxyProvider(this IServiceCollection services)
        {
            using ServiceProvider oldProvider = services.BuildServiceProvider();

            IServiceCollection dynamciServices = new ServiceCollection();
            dynamciServices.AddSingleton<AbstractInterceptor>();

            foreach (ServiceDescriptor item in services)
            {
                if (IsProxyType(item, oldProvider, out Type implementationType))
                {
                    object implementationFactory(IServiceProvider serviceProvider)
                    {
                        var abstractInterceptor = serviceProvider.GetRequiredService<AbstractInterceptor>(); ;
                        if (item.ImplementationType != null)
                        {
                            var targetType = item.ServiceType.IsInterface ? ProxyGenerator.ProxyBuilder.CreateInterfaceProxyTypeWithTarget(item.ServiceType, Type.EmptyTypes, item.ImplementationType, ProxyGenerationOptions.Default)
                             : ProxyGenerator.ProxyBuilder.CreateClassProxyType(item.ImplementationType, Type.EmptyTypes, ProxyGenerationOptions.Default);
                            var target = ActivatorUtilities.CreateInstance(serviceProvider, item.ImplementationType);
                            List<object> constructorArguments = GetConstructorArguments(target, ProxyGenerationOptions.Default, abstractInterceptor);
                            return Activator.CreateInstance(targetType, constructorArguments.ToArray());
                        }
                        else if (item.ImplementationInstance != null)
                        {
                            if (item.ServiceType.IsInterface)
                            {
                                return ProxyGenerator.CreateInterfaceProxyWithTarget(item.ServiceType, item.ImplementationInstance, abstractInterceptor);
                            }
                            return ProxyGenerator.CreateClassProxyWithTarget(item.ServiceType, item.ImplementationInstance, abstractInterceptor);
                        }
                        else if (item.ImplementationFactory != null)
                        {
                            var target = item.ImplementationFactory.Invoke(serviceProvider);
                            return item.ServiceType.IsInterface ? ProxyGenerator.CreateInterfaceProxyWithTarget(item.ServiceType, target, abstractInterceptor)
                                : ProxyGenerator.CreateClassProxyWithTarget(item.ServiceType, target, abstractInterceptor);
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
                || implementType.GetMethods().Any(m => m.IsDefined(typeof(AbstractInterceptorAttribute), true));

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
