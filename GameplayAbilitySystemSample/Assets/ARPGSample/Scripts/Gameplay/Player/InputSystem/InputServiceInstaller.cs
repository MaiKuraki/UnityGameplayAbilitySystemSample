using Zenject;

namespace ARPGSample.Gameplay
{
    public class InputServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
#if UNITY_ANDROID || UNITY_IOS
            Container.BindInterfacesTo<MobileInputService>().AsSingle().NonLazy();
#elif UNITY_STANDALONE
            Container.BindInterfacesTo<DesktopInputService>().AsSingle().NonLazy();
#endif
        }
    }
}