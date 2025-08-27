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
    }
    public class SceneManagementAPIGateway : ISceneManagementAPIGateway
    {
        public UniTask Push(ISceneIdentifier scene, ITransitionDirector transitionDirector = null, ISceneData data = null, IAsyncOperation interruptOperation = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GlobalSceneNavigator.Instance.Push(scene, transitionDirector, data, interruptOperation, cancellationToken);
        }
    }
}