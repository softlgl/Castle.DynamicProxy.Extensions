using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy.Extensions.Test.ServiceClass;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Web.Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IFoo, Foo>()
                .AddSingleton(new FooConfig { Name = "redis" });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    IFoo foo = app.ApplicationServices.GetService<IFoo>();
                    bool addFlag = foo.Add(new FooModel { Id = 1, Name = "test", Date = DateTime.Now });
                    foo.Delete(1);
                    FooModel fooModel = foo.Get(1);
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
