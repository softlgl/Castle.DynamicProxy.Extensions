using System;
namespace Castle.DynamicProxy.Extensions.Test.ServiceClass
{
    [Logger]
    public class Foo:IFoo
    {
        public bool Add(FooModel fooModel)
        {
            return true;
        }

        public void Delete(int id)
        {
            
        }

        [Limit]
        [Cache]
        public FooModel Get(int id)
        {
            return new FooModel { Id = id, Name = "foo" + id, Date = DateTime.Now };
        }
    }
}
