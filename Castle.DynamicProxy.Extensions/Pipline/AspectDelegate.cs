using System;
using System.Threading.Tasks;

namespace Castle.DynamicProxy.Extensions.Pipline
{
    public delegate Task AspectDelegate(AspectContext context);
}
