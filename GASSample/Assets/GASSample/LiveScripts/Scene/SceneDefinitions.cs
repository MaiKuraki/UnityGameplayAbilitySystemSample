using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.AddressableAssets;

namespace GASSample.Scene
{
    public class SceneDefinitions
    {
        public static ISceneIdentifier Splash { get; } = new AddressableSceneIdentifier("Assets/GASSample/LiveContent/Scenes/Scene_Splash.unity");
        public static ISceneIdentifier Transition { get; } = new AddressableSceneIdentifier("Assets/GASSample/LiveContent/Scenes/Scene_Transition.unity");
        public static ISceneIdentifier Title { get; } = new AddressableSceneIdentifier("Assets/GASSample/LiveContent/Scenes/Scene_Title.unity");
        public static ISceneIdentifier Gameplay { get; } = new AddressableSceneIdentifier("Assets/GASSample/LiveContent/Scenes/Scene_Gameplay.unity");
    }
}