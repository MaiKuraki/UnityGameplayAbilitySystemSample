using MessagePipe;
using Zenject;
using CycloneGames.UIFramework;

namespace ARPGSample.GameSubSystem
{
    public class UIServiceInstaller : MonoInstaller
    {
        [Inject] private MessagePipeOptions msgOpt;
        
        public override void InstallBindings()
        {
            UIFramework uiFramework = UnityEngine.GameObject.FindObjectOfType<UIFramework>();
            if (uiFramework)
            {
                UnityEngine.GameObject.DontDestroyOnLoad(uiFramework);
                Container.BindInstance(uiFramework).AsSingle();
                Container.QueueForInject(uiFramework);
            }
            else
            {
                
            }
            
            UIRoot uiRoot = UnityEngine.GameObject.FindObjectOfType<UIRoot>();
            if (uiRoot)
            {
                Container.BindInstance(uiRoot).AsSingle();
                Container.QueueForInject(uiRoot);
            }
            
            Container.BindInterfacesTo<UIService>().AsSingle().NonLazy();
            Container.BindMessageBroker<UIMessage>(options: msgOpt);
            GlobalMessagePipe.SetProvider(Container.AsServiceProvider());
        }
    }
}

