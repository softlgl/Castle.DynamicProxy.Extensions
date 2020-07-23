using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Castle.DynamicProxy.Extensions.Pipline;

namespace Castle.DynamicProxy.Extensions.Test.ServiceClass
{
    public class LimitAttribute : AbstractInterceptorAttribute
    {
        public override async Task InvokeAsync(AspectContext context, AspectDelegate next)
        {
            Debug.WriteLine($"LimitAttribute执行前,param.length=[{context.Parameters.Length}]");
            await next(context);
            Debug.WriteLine($"LimitAttribute执行后,returnValue=[{context.ReturnValue}]");
        }
    }
}
