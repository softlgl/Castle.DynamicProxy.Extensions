using System;
namespace Castle.DynamicProxy.Extensions.Test.ServiceClass
{
    public interface IBar
    {
        bool Add(FooModel fooModel);
        FooModel Get(int id);
        void Delete (int id);
    }
}
