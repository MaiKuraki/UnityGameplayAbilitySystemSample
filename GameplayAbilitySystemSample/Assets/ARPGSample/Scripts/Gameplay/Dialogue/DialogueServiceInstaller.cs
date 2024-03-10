using Zenject;

namespace ARPGSample.Gameplay
{
    public class DialogueServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<DialogueService>().AsSingle().NonLazy();
        }
    }
}

