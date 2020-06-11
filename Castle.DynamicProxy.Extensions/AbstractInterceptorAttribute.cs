using System;
using System.Threading.Tasks;
using Castle.DynamicProxy.Extensions.Pipline;

namespace Castle.DynamicProxy.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class AbstractInterceptorAttribute : Attribute
    {
        public abstract Task InvokeAsync(AspectContext context, AspectDelegate next);
    }
}
