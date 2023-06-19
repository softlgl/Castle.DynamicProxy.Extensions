using System;
using System.Threading.Tasks;
using Castle.DynamicProxy.Extensions.Test.ServiceClass;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Castle.DynamicProxy.Extensions.Test
{
    public class AopAsyncTest
    {
        [Fact]
        public async void InterceptorAsyncTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<IBar, Bar>()
                .AddSingleton(new FooConfig { Name = "redis" });
            IServiceProvider serviceProvider = services.BuildCastleDynamicProxyProvider();

            IBar bar = serviceProvider.GetService<IBar>();

            bool addFlag = await bar.AddAsync(new FooModel { Id =1, Name = "test",Date = DateTime.Now });

            Task editTask = bar.EditAsync(1);

            FooModel fooModel = await bar.GetAsync(1);

            await bar.DeleteAsync(1);
        }
    }
}
