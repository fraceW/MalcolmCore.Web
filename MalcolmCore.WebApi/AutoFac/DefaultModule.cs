using Autofac;
using MalcolmCore.IService;
using MalcolmCore.IService.Login;
using MalcolmCore.Service;
using MalcolmCore.Service.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MalcolmCore.WebApi.AutoFac
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //这里就是AutoFac的注入方式，下面采用常规的方式
            //官网：https://autofac.org/

            //特别注意：其中很大的一个变化在于，Autofac 原来的一个生命周期InstancePerRequest，将不再有效。正如我们前面所说的，整个request的生命周期被ASP.NET Core管理了，
            //所以Autofac的这个将不再有效。我们可以使用 InstancePerLifetimeScope ，同样是有用的，对应了我们ASP.NET Core DI 里面的Scoped。

            //瞬时请求(省略InstancePerDependency 也为瞬时)
            //InstancePerLifetimeScope 单次请求内单例
            //SingleInstance 全局单例
            builder.RegisterType<BaseService>().As<IBaseService>().InstancePerDependency();
            builder.RegisterType<LoginService>().As<ILoginService>().InstancePerDependency();
        }
    }
}
