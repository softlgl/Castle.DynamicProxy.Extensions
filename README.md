# Castle.DynamicProxy.Extensions
基于Castle.DynamicProxy的扩展支持Filter方式AOP

## 使用方式 
### 控制台程序
```cs
 IServiceCollection services = new ServiceCollection();
            services.AddTransient<IFoo, Foo>()
                .AddSingleton(new FooConfig { Name = "redis" });
            //使用BuildCastleDynamicProxyProvider
            IServiceProvider serviceProvider = services.BuildCastleDynamicProxyProvider();
            IFoo foo = serviceProvider.GetService<IFoo>();
            bool addFlag = foo.Add(new FooModel { Id =1, Name = "test",Date = DateTime.Now });
            foo.Delete(1);
            FooModel fooModel = foo.Get(1);
```
### Web程序
```cs
public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseServiceProviderFactory(new CastleDynamicProxyServiceProviderFactory());
```
### 定义拦截
```cs
public class CacheAttribute : AbstractInterceptorAttribute
{
    [FromServices]
    public FooConfig FooConfig { get; set; }

    public override async Task InvokeAsync(AspectContext context, AspectDelegate next)
    {
        Debug.WriteLine($"CacheAttribute执行前,param.length=[{context.Parameters.Length}],FooModel.Name=[{FooConfig.Name}]");
        await next(context);
        Debug.WriteLine($"CacheAttribute执行后,returnValue=[{context.ReturnValue}]");
    }
}
 ```
