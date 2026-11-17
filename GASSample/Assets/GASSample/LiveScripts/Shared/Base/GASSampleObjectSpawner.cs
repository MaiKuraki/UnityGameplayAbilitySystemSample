using VContainer;
using CycloneGames.Factory.Runtime;
using UnityEngine;
using CycloneGames.Logging;

namespace GASSample
{
    public class GASSampleObjectSpawner : IUnityObjectSpawner
    {
        private static readonly LogChannel Log = LogChannel.Create("GASSample");
        [Inject] private IObjectResolver objectResolver;

        public T Create<T>(T origin) where T : UnityEngine.Object
        {
            if (origin == null)
            {
                Log.Error($"Invalid prefab to spawn");
                return null;
            }

            var obj = UnityEngine.Object.Instantiate(origin);
            objectResolver.Inject(obj);
            return obj;
        }

        public T Create<T>(T origin, Transform parent) where T : Object
        {
            if (origin == null)
            {
                Log.Error($"Invalid prefab to spawn");
                return null;
            }

            var obj = UnityEngine.Object.Instantiate(origin, parent);
            objectResolver.Inject(obj);
            return obj;
        }
    }
}