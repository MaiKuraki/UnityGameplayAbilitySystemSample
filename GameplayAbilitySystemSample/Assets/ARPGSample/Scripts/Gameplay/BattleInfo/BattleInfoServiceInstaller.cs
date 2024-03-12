using Zenject;

namespace ARPGSample.Gameplay
{
    public class BattleInfoServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<BattleInfoService>().AsSingle().NonLazy();
        }
    }
}
