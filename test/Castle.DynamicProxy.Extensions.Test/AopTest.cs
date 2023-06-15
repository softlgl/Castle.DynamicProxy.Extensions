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
                .AddTransient<IBar, Bar>()
                .AddSingleton(new FooConfig { Name = "redis" });
            IServiceProvider serviceProvider = services.BuildCastleDynamicProxyProvider();
            IFoo foo = serviceProvider.GetService<IFoo>();
            bool addFlag = foo.Add(new FooModel { Id =1, Name = "test",Date = DateTime.Now });
            foo.Delete(1);
            FooModel fooModel = foo.Get(1);
        }

        [Fact]
        public void InterceptorFactoryTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<IFoo>(provider => new Foo(provider.GetService<IBar>()))
                .AddTransient<IBar>(provider => new Bar())
                .AddSingleton(new FooConfig { Name = "redis" });
            IServiceProvider serviceProvider = services.BuildCastleDynamicProxyProvider();
            IFoo foo = serviceProvider.GetService<IFoo>();
            bool addFlag = foo.Add(new FooModel { Id = 1, Name = "test", Date = DateTime.Now });
            foo.Delete(1);
            FooModel fooModel = foo.Get(1);
        }

        [Fact]
        public void InterceptorImplTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IFoo, Foo>()
                .AddSingleton<IBar>(new Bar())
                .AddSingleton(new FooConfig { Name = "redis" });
            IServiceProvider serviceProvider = services.BuildCastleDynamicProxyProvider();
            IFoo foo = serviceProvider.GetService<IFoo>();
            bool addFlag = foo.Add(new FooModel { Id = 1, Name = "test", Date = DateTime.Now });
            foo.Delete(1);
            FooModel fooModel = foo.Get(1);
        }
    }
}
