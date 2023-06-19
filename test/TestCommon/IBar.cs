using System;
using System.Threading.Tasks;

namespace Castle.DynamicProxy.Extensions.Test.ServiceClass
{
    public interface IBar
    {
        ValueTask<bool> AddAsync(FooModel fooModel);
        Task<FooModel> GetAsync(int id);
        ValueTask DeleteAsync (int id);

        Task EditAsync(int id);
    }
}
