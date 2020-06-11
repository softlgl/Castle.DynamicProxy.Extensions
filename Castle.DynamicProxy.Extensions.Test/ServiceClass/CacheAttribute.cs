using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Castle.DynamicProxy.Extensions.Pipline;

namespace Castle.DynamicProxy.Extensions.Test.ServiceClass
{
    public class CacheAttribute : AbstractInterceptorAttribute
    {
        public override async Task InvokeAsync(AspectContext context, AspectDelegate next)
        {
            Debug.WriteLine($"CacheAttribute执行前,param.length=[{context.Parameters.Length}]");
            await next(context);
            Debug.WriteLine($"CacheAttribute执行后,returnValue=[{context.ReturnValue}]");
        }
    }
}
