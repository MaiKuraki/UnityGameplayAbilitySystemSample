using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.AddressableAssets;

namespace GASSample.AOT
{
    public static class BuiltInSceneDefinitions
    {
        public static ISceneIdentifier Initial {get;} = new AddressableSceneIdentifier("Assets/GASSample/LiveContent/Scenes/Scene_Initial.unity");
    }
}