using Ninject.Modules;

namespace misechko.com.core
{
    public class MPCoreModule: NinjectModule
    {
        public override void Load()
        {
            Bind<IMPSettings>().To<ProductionMPSettings>().InSingletonScope();
        }
    }
}
