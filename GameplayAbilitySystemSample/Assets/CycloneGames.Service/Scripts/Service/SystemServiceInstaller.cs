#define ENABLE_CHEAT

using Zenject;
using MessagePipe;

namespace CycloneGames.Service
{
    public class SystemServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var opt = Container.BindMessagePipe();

            Container.BindInterfacesTo<ServiceDisplay>().AsSingle().NonLazy();
            Container.BindInterfacesTo<GraphicsSettingService>().AsSingle().NonLazy();

            MainCamera mainCamera = UnityEngine.GameObject.FindObjectOfType<MainCamera>();
            UnityEngine.GameObject.DontDestroyOnLoad(mainCamera);
            Container.BindInstance(mainCamera).AsSingle();
            Container.QueueForInject(mainCamera);
            
#if ENABLE_CHEAT
            Container.Bind<ICheatService>().To<CheatService>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("CheatService")
                .AsSingle().NonLazy();
            Container.BindMessageBroker<CheatMessage>(options: opt);
#endif
        }
    }
}