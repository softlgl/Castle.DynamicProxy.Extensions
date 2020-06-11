using System;
namespace Castle.DynamicProxy.Extensions.Test.ServiceClass
{
    public interface IFoo
    {
        bool Add(FooModel fooModel);
        FooModel Get(int id);
        void Delete (int id);
    }
}
