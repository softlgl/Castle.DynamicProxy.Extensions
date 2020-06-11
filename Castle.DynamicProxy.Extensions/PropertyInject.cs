using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Castle.DynamicProxy.Extensions
{
    internal static class PropertyInject
    {
        private static readonly ConcurrentDictionary<string, IEnumerable<PropertyInfo>> _filterFromServices = new ConcurrentDictionary<string, IEnumerable<PropertyInfo>>();
        public static void PropertieInject(IServiceProvider serviceProvider, AbstractInterceptorAttribute interceptorAttribute)
        {
            var properties = _filterFromServices.GetOrAdd($"{interceptorAttribute.GetType().FullName}",key => interceptorAttribute.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(i => i.GetCustomAttribute<FromServicesAttribute>() != null));
            if (properties.Any())
            {
                foreach (var propertyInfo in properties)
                {
                    propertyInfo.SetValue(interceptorAttribute, serviceProvider.GetService(propertyInfo.PropertyType));
                }
            }
        }

        public static void PropertiesInject(IServiceProvider serviceProvider, IEnumerable<AbstractInterceptorAttribute> interceptorAttributes)
        {
            foreach (var fitler in interceptorAttributes)
            {
                PropertieInject(serviceProvider, fitler);
            }
        }
    }
}
