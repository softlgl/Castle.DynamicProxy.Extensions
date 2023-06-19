using System;
using System.Reflection;

namespace Castle.DynamicProxy.Extensions.Pipline
{
    public abstract class AspectContext
    {
        public abstract object ReturnValue { get; set; } 

        public abstract object[] Parameters { get; }

        public abstract MethodInfo Method { get; }

        public abstract MethodInfo ImplementationMethod { get; }

        public abstract object Implementation { get; }

        public abstract object Proxy { get; }

        public abstract IServiceProvider ServiceProvider { get; }
    }
}
