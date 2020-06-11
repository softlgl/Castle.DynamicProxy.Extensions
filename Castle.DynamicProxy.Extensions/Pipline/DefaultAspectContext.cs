using System;
using System.Reflection;

namespace Castle.DynamicProxy.Extensions.Pipline
{
    public class DefaultAspectContext : AspectContext
    {
        private readonly IInvocation _invocation;
        public DefaultAspectContext(IInvocation invocation)
        {
            _invocation = invocation;
        }

        public override object ReturnValue { get; set; }

        public override object[] Parameters => _invocation.Arguments;

        public override MethodInfo ImplementationMethod => _invocation.MethodInvocationTarget;

        public override object Implementation => _invocation.InvocationTarget;

        public override object Proxy => _invocation.Proxy;
    }
}
