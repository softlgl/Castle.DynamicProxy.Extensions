using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Castle.DynamicProxy.Extensions.Pipline
{
    public class AspectPiplineBuilder
    {
        private readonly IList<Func<AspectDelegate, AspectDelegate>> _components ;

        public AspectPiplineBuilder()
        {
            _components = new List<Func<AspectDelegate, AspectDelegate>>();
        }

        public AspectPiplineBuilder Use(Func<AspectContext, AspectDelegate, Task> middleware)
        {
            _components.Add(next => context => middleware(context,next));
            return this;
        }

        public AspectDelegate Build(AspectDelegate _complete)
        {
            var invoke = _complete;
            foreach (var component in _components.Reverse())
            {
                invoke = component(invoke);
            }
            return invoke;
        }
    }
}
