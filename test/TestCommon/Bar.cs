using System;
using System.Threading.Tasks;

namespace Castle.DynamicProxy.Extensions.Test.ServiceClass
{
    [Logger]
    public class Bar : IBar
    {
        public ValueTask<bool> AddAsync(FooModel fooModel)
        {
            return new ValueTask<bool>(true);
        }

        [Limit]
        public ValueTask DeleteAsync(int id)
        {
            return new ValueTask();
        }

        public Task EditAsync(int id)
        {
            return Task.CompletedTask;
        }

        [Limit]
        [Cache]
        public Task<FooModel> GetAsync(int id)
        {
            return Task.FromResult(new FooModel { Id = id, Name = "foo" + id, Date = DateTime.Now });
        }
    }
}
