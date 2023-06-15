using System;
using System.Reflection;

namespace Castle.DynamicProxy.Extensions.Pipline
{
    public class DefaultAspectContext : AspectContext
    {
        private readonly IInvocation _invocation;
        private readonly IServiceProvider _serviceProvider;
        public DefaultAspectContext(IInvocation invocation, IServiceProvider serviceProvider)
        {
            _invocation = invocation;
            _serviceProvider = serviceProvider;
        }

        public override object ReturnValue { get; set; }

        public override object[] Parameters => _invocation.Arguments;

        public override MethodInfo ImplementationMethod => _invocation.MethodInvocationTarget;

        public override object Implementation => _invocation.InvocationTarget;

        public override object Proxy => _invocation.Proxy;

        public override IServiceProvider ServiceProvider => _serviceProvider;
    }
}
