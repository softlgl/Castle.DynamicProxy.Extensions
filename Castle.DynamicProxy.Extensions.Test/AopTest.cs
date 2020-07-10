using System;
using Castle.DynamicProxy.Extensions.Test.ServiceClass;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Castle.DynamicProxy.Extensions.Test
{
    public class AopTest
    {
        [Fact]
        public void InterceptorTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<IFoo, Foo>()
                .AddSingleton(new FooConfig { Name = "redis" });
            IServiceProvider serviceProvider = services.BuildCastleDynamicProxyProvider();
            IFoo foo = serviceProvider.GetService<IFoo>();
            bool addFlag = foo.Add(new FooModel { Id =1, Name = "test",Date = DateTime.Now });
            foo.Delete(1);
            FooModel fooModel = foo.Get(1);
        }
    }
}
