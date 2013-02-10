using Ninject.Modules;
using Ninject.Web.Common;
using misechko.com.data.EF;
using misechko.com.data.Repositories;

namespace misechko.com.data
{
    public class MPDataModule : NinjectModule
    {
        public override void Load()
        {
            Bind<MPDataContext>().ToSelf().InRequestScope();
            Bind<IMPUserRepository>().To<MPUserRepository>().InRequestScope();
        }
    }
}
