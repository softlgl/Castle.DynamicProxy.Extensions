using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Castle.DynamicProxy.Extensions.Pipline;

namespace Castle.DynamicProxy.Extensions.Test.ServiceClass
{
    public class LoggerAttribute: AbstractInterceptorAttribute
    {
        public override async Task InvokeAsync(AspectContext context, AspectDelegate next)
        {
            Debug.WriteLine($"LoggerAttribute执行前,param.length=[{context.Parameters.Length}]");
            await next(context);
            Debug.WriteLine($"LoggerAttribute执行后,returnValue=[{context.ReturnValue}]");
        }
    }
}
