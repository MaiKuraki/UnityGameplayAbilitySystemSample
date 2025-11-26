using System.Threading;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.Transitions;

namespace GASSample.APIGateway
{
    public interface ISceneManagementAPIGateway
    {
        UniTask Push(ISceneIdentifier scene, ITransitionDirector transitionDirector = null, ISceneData data = null, IAsyncOperation interruptOperation = null, CancellationToken cancellationToken = default(CancellationToken));
        UniTask Pop(ITransitionDirector transitionDirector = null, IAsyncOperation interruptOperation = null, CancellationToken cancellationToken = default);
        UniTask Change(ISceneIdentifier scene, ITransitionDirector transitionDirector = null, ISceneData data = null, IAsyncOperation interruptOperation = null, CancellationToken cancellationToken = default);
        UniTask Replace(ISceneIdentifier scene, ITransitionDirector transitionDirector = null, ISceneData data = null, IAsyncOperation interruptOperation = null, CancellationToken cancellationToken = default);
        UniTask Reload(ITransitionDirector transitionDirector = null, IAsyncOperation interruptOperation = null, CancellationToken cancellationToken = default);
    }
    public class SceneManagementAPIGateway : ISceneManagementAPIGateway
    {
        public UniTask Push(ISceneIdentifier scene, ITransitionDirector transitionDirector = null, ISceneData data = null, IAsyncOperation interruptOperation = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GlobalSceneNavigator.Instance.Push(scene, transitionDirector, data, interruptOperation, cancellationToken);
        }

        public UniTask Pop(ITransitionDirector transitionDirector = null, IAsyncOperation interruptOperation = null, CancellationToken cancellationToken = default)
        {
            return GlobalSceneNavigator.Instance.Pop(transitionDirector, interruptOperation, cancellationToken);
        }

        public UniTask Change(ISceneIdentifier scene, ITransitionDirector transitionDirector = null, ISceneData data = null, IAsyncOperation interruptOperation = null, CancellationToken cancellationToken = default)
        {
            return GlobalSceneNavigator.Instance.Change(scene, transitionDirector, data, interruptOperation, cancellationToken);
        }

        public UniTask Replace(ISceneIdentifier scene, ITransitionDirector transitionDirector = null, ISceneData data = null, IAsyncOperation interruptOperation = null, CancellationToken cancellationToken = default)
        {
            return GlobalSceneNavigator.Instance.Replace(scene, transitionDirector, data, interruptOperation, cancellationToken);
        }

        public UniTask Reload(ITransitionDirector transitionDirector = null, IAsyncOperation interruptOperation = null, CancellationToken cancellationToken = default)
        {
            return GlobalSceneNavigator.Instance.Reload(transitionDirector, interruptOperation, cancellationToken);
        }
    }
}