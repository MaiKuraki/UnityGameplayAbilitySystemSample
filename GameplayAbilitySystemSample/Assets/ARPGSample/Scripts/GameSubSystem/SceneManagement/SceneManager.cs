using System.Collections.Concurrent;
using CycloneGames.Service;
using CycloneGames.UIFramework;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System.Linq;

namespace ARPGSample.GameSubSystem
{
    public struct SceneLoadParam
    {
        public string SceneKey;
        public int Priority;
    }

    internal static class SceneAddressableKeyBuilder
    {
        public static string GetAddressablesKey(string sceneName) => $"Assets/ARPGSample/Scenes/{sceneName}.unity";
    }

    public class SceneManager : MonoBehaviour
    {
        [Inject] private IAddressablesService addressablesService;
        [Inject] private IUIService uiService;
        public event System.Action OnStartLoadingEvent;
        public event System.Action OnFinishedLoadingEvent;

        public async UniTask OpenSceneAsync(SceneLoadParam[] ScenesForLoad, string LoadingUIKey = null,
           int delayTransferTimeMS = 0, string[] UnloadScenes = null, System.Action OnStartLoading = null, System.Action OnFinishedLoading = null)
        {
            try
            {
                OnStartLoadingEvent?.Invoke();
                OnStartLoading?.Invoke();
                
                // Show loading UI
                uiService.OpenUI(LoadingUIKey);
                var loadingTunnel = await addressablesService.LoadSceneAsync(
                    SceneAddressableKeyBuilder.GetAddressablesKey("Scene_LoadingTunnel"),
                    AddressablesManager.SceneLoadMode.Additive);

                if (UnloadScenes != null)
                {
                    foreach (var sceneName in UnloadScenes)
                    {
                        await UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
                    }
                }

                ConcurrentDictionary<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance, int> sceneDic =
                    new ConcurrentDictionary<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance, int>();

                if (ScenesForLoad != null)
                {
                    foreach (var param in ScenesForLoad)
                    {
                        UnityEngine.ResourceManagement.ResourceProviders.SceneInstance sceneInst =
                            await addressablesService.LoadSceneAsync(
                                SceneAddressableKeyBuilder.GetAddressablesKey(param.SceneKey), AddressablesManager.SceneLoadMode.Additive,
                                false, param.Priority);
                        
                        sceneDic.TryAdd(sceneInst, param.Priority);
                    }

                    await UniTask.Delay(delayTransferTimeMS);
                    foreach (var sceneKP in sceneDic)
                    {
                        await sceneKP.Key.ActivateAsync();
                    }
                    
                    //  Copy a dictionary for sort
                    var snapshot = sceneDic.ToArray();
                    var highestPriorityScene = snapshot.OrderByDescending(pair => pair.Value).FirstOrDefault();
                    UnityEngine.SceneManagement.SceneManager.SetActiveScene(highestPriorityScene.Key.Scene);
                }
                
                await UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Scene_LoadingTunnel");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error occurred while opening scenes: {ex.Message}");
                // Handle exception (log the error, retry the operation, show an error message, etc.)
            }
            finally
            {
                //  CAUTION: maybe GC its not required
                System.GC.Collect();
                
                await UniTask.DelayFrame(1);
                uiService.CloseUI(LoadingUIKey);
                
                OnFinishedLoadingEvent?.Invoke();
                OnFinishedLoading?.Invoke();
            }
        }
    }
}