﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using System.Collections.Concurrent;
using Castle.DynamicProxy.Extensions.Pipline;
using System.Collections.Generic;
using System.Reflection;

namespace Castle.DynamicProxy.Extensions
{
    public class AbstractInterceptor : IInterceptor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, IEnumerable<AbstractInterceptorAttribute>> _methodFilters = new ConcurrentDictionary<string, IEnumerable<AbstractInterceptorAttribute>>();

        public AbstractInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Intercept(IInvocation invocation)
        {
            var methondInterceptorAttributes = _methodFilters.GetOrAdd($"{invocation.MethodInvocationTarget.DeclaringType.FullName}#{invocation.MethodInvocationTarget.Name}", key => {
                var methondAttributes = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(AbstractInterceptorAttribute), true)
                .Cast<AbstractInterceptorAttribute>();
                var classInterceptorAttributes = invocation.MethodInvocationTarget.DeclaringType.GetCustomAttributes(typeof(AbstractInterceptorAttribute), true)
                .Cast<AbstractInterceptorAttribute>();

                var allInterceptorAttributes = methondAttributes.Concat(classInterceptorAttributes);
                //属性注入
                PropertyInject.PropertiesInject(_serviceProvider, allInterceptorAttributes);

                return allInterceptorAttributes;
            });

            if (methondInterceptorAttributes.Any())
            {
                AspectPiplineBuilder aspectPipline = new AspectPiplineBuilder();
                foreach (var item in methondInterceptorAttributes)
                {
                    aspectPipline.Use(item.InvokeAsync);
                }

                AspectContext aspectContext = new DefaultAspectContext(invocation, _serviceProvider);
                var aspectDelegate = aspectPipline.Build(context => {
                    invocation.Proceed();
                    aspectContext.ReturnValue = invocation.ReturnValue;
                    return Task.CompletedTask;
                });

                aspectDelegate.Invoke(aspectContext).GetAwaiter().GetResult();
                invocation.ReturnValue = aspectContext.ReturnValue;

                return;
            }

            invocation.Proceed();
        }
    }
}
