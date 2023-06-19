using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy.Extensions.Pipline;
using TestCommon;

namespace Castle.DynamicProxy.Extensions.Test.ServiceClass
{
    public class LimitAttribute : AbstractInterceptorAttribute
    {
        public override async Task InvokeAsync(AspectContext context, AspectDelegate next)
        {
            Debug.WriteLine($"LimitAttribute执行[{context.Method.Name}]前,param.length=[{context.Parameters.Length}]");
            await next(context);

            var returnValueType = context.Method.ReturnType.GetTypeInfo();
            if (returnValueType.IsAsync())
            {
                if (returnValueType.IsTask() || returnValueType.IsValueTask() || returnValueType.IsTaskWithVoidTaskResult())
                {
                    Debug.WriteLine($"LimitAttribute执行[{context.Method.Name}]后,returnValue=[{context.ReturnValue}]");
                    return;
                }

                if (returnValueType.IsTaskWithResult())
                {
                    var returnValue = TaskUtils.CreateFuncToGetTaskResult(returnValueType).Invoke(context.ReturnValue);
                    Debug.WriteLine($"LimitAttribute执行[{context.Method.Name}]后,returnValue=[{returnValue}]");
                    return;
                }

                if (returnValueType.IsValueTaskWithResult())
                {
                    await TaskUtils.ValueTaskWithResultToTask(context.ReturnValue, returnValueType);
                    var returnValue = TaskUtils.CreateFuncToGetTaskResult(returnValueType).Invoke(context.ReturnValue);
                    Debug.WriteLine($"LimitAttribute执行[{context.Method.Name}]后,returnValue=[{returnValue}]");

                    return;
                }
                Debug.WriteLine($"LimitAttribute执行[{context.Method.Name}]后,returnValue=[{context.ReturnValue}]");
            }
        }
    }
}
