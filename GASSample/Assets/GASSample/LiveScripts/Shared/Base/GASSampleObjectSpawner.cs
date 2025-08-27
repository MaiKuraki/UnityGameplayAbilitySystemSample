using VContainer;
using CycloneGames.Logger;
using CycloneGames.Factory.Runtime;

namespace GASSample
{
    public class GASSampleObjectSpawner : IUnityObjectSpawner
    {
        [Inject] private IObjectResolver objectResolver;

        public T Create<T>(T origin) where T : UnityEngine.Object
        {
            if (origin == null)
            {
                CLogger.LogError($"Invalid prefab to spawn");
                return null;
            }

            var obj = UnityEngine.Object.Instantiate(origin);
            objectResolver.Inject(obj);
            return obj;
        }
    }
}