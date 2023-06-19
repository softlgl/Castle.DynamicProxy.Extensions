using System;
namespace Castle.DynamicProxy.Extensions.Test.ServiceClass
{
    [Logger]
    public class Foo : IFoo
    {
        private readonly IBar _bar;
        public Foo(IBar bar)
        {
            _bar = bar;
        }

        public bool Add(FooModel fooModel)
        {
            _bar.AddAsync(fooModel).GetAwaiter().GetResult();
            return true;
        }

        [Limit]
        public void Delete(int id)
        {
            
        }

        //[Limit]
        [Cache]
        public FooModel Get(int id)
        {
            _bar.GetAsync(id).GetAwaiter().GetResult();
            return new FooModel { Id = id, Name = "foo" + id, Date = DateTime.Now };
        }
    }
}
