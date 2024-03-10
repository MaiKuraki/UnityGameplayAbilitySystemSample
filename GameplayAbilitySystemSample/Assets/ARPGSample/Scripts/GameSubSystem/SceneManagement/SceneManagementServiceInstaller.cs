using Zenject;

namespace ARPGSample.GameSubSystem
{
    public class SceneManagementServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<SceneManagementService>().AsSingle().NonLazy();
        }
    }
}