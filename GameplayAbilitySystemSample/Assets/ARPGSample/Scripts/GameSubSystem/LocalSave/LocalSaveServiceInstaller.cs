using Zenject;

namespace ARPGSample.GameSubSystem
{
    public class LocalSaveServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<LocalSaveService>().AsSingle().NonLazy();
        }
    }
}