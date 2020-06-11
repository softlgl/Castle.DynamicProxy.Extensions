using System;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using System.Collections.Concurrent;
using Castle.DynamicProxy.Extensions.Pipline;
using System.Collections.Generic;

namespace Castle.DynamicProxy.Extensions
{
    public class AbstractInterceptor : IInterceptor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, List<AbstractInterceptorAttribute>> _methodFilters = new ConcurrentDictionary<string, List<AbstractInterceptorAttribute>>();
        public AbstractInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void Intercept(IInvocation invocation)
        {
            var methondInterceptorAttributes = _methodFilters.GetOrAdd($"{invocation.TargetType.DeclaringType.FullName}#{invocation.Method.Name}",key => {
                var methondAttributes = invocation.MethodInvocationTarget.GetCustomAttributes(true)
                    .Where(i => typeof(AbstractInterceptorAttribute).IsAssignableFrom(i.GetType()))
                    .Cast<AbstractInterceptorAttribute>().ToList();
                var classInterceptorAttributes = invocation.TargetType.GetCustomAttributes(true)
                    .Where(i => typeof(AbstractInterceptorAttribute).IsAssignableFrom(i.GetType()))
                    .Cast<AbstractInterceptorAttribute>();
                methondAttributes.AddRange(classInterceptorAttributes);
                return methondAttributes;
            });
            PropertyInject.PropertiesInject(_serviceProvider, methondInterceptorAttributes);

            if (methondInterceptorAttributes.Any())
            {
                AspectPiplineBuilder aspectPipline = new AspectPiplineBuilder();
                foreach (var item in methondInterceptorAttributes)
                {
                    aspectPipline.Use(item.InvokeAsync);
                }

                AspectContext aspectContext = new DefaultAspectContext(invocation);
                var aspectDelegate = aspectPipline.Build(context=> {
                    invocation.Proceed();
                    aspectContext.ReturnValue = invocation.ReturnValue;
                    return Task.CompletedTask;
                });
                aspectDelegate.Invoke(aspectContext).GetAwaiter().GetResult();
            }
        }
    }
}
