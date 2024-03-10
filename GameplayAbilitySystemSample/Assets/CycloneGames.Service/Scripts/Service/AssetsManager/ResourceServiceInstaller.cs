using Zenject;

namespace CycloneGames.Service
{
    public class ResourceServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<AddressablesService>().AsSingle().NonLazy();
            Container.BindInterfacesTo<StreamingAssetsService>().AsSingle().NonLazy();
        }
    }
}