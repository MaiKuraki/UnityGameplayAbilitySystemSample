using System.Collections.Generic;
using CycloneGames.Service;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace CycloneGames.UIFramework
{
    internal static class UIPathBuilder
    {
        public static string GetConfigPath(string pageName)
            => $"Assets/ARPGSample/ScriptableObject/UIConfig/Page/{pageName}.asset";
    }

    public class UIManager : MonoBehaviour
    {
        private const string DEBUG_FLAG = "[UIManager]";
        [Inject] private ISubscriber<UIMessage> uiMsgSub;
        [Inject] private IAddressablesService addressablesService;
        [Inject] private UIRoot uiRoot;
        [Inject] private DiContainer diContainer;
        
        private Dictionary<string, UniTaskCompletionSource<bool>> uiOpenTasks = new Dictionary<string, UniTaskCompletionSource<bool>>();
        
        private void Start()
        {
            uiMsgSub.Subscribe(msg =>
            {
                if (msg.Params != null && msg.Params.Length > 0)
                {
                    if (msg.MessageCode == UIMessageCode.OPEN_UI)
                    {
                        if (msg.Params != null && msg.Params.Length > 1 && msg.Params[1] is System.Action<UIPage> onPageCreated)
                        {
                            OpenUI(msg.Params[0].ToString(), onPageCreated);
                        }
                        else
                        {
                            OpenUI(msg.Params[0].ToString());
                        }
                    }

                    if (msg.MessageCode == UIMessageCode.CLOSE_UI)
                    {
                        CloseUI(msg.Params[0].ToString());
                    }
                }
                else
                {
                    Debug.LogError("Invalid Params");
                }
            });
        }

        internal void OpenUI(string PageName, System.Action<UIPage> OnPageCreated = null)
        {
            OpenUIAsync(PageName, OnPageCreated).Forget();
        }

        internal void CloseUI(string PageName)
        {
            CloseUIAsync(PageName).Forget();
        }

        async UniTask OpenUIAsync(string PageName, System.Action<UIPage> OnPageCreated = null)
        {
            // Avoid duplicated open same UI
            if (uiOpenTasks.ContainsKey(PageName))
            {
                Debug.LogError($"{DEBUG_FLAG} Duplicated Open! PageName: {PageName}");
                return;
            }
            var tcs = new UniTaskCompletionSource<bool>();
            uiOpenTasks[PageName] = tcs;
            
            Debug.Log($"{DEBUG_FLAG} Attempting to open UI: {PageName}");
            UIPageConfiguration pageConfig = null;
            Object pagePrefab = null;
            
            try
            {
                // Attempt to load the configuration
                pageConfig = await addressablesService.LoadAssetWithRetentionAsync<UIPageConfiguration>(
                    UIPathBuilder.GetConfigPath(PageName));

                // If the configuration load fails, log the error and exit
                if (pageConfig == null)
                {
                    Debug.LogError($"{DEBUG_FLAG} Failed to load UI Config, PageName: {PageName}");
                    return;
                }

                // Attempt to load the Prefab
                pagePrefab = pageConfig.PagePrefab;

                // If the Prefab load fails, log the error and exit
                if (pagePrefab == null)
                {
                    Debug.LogError($"{DEBUG_FLAG} Invalid UI Prefab in PageConfig, PageName: {PageName}");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                // Catch any exceptions, log the error message
                Debug.LogError($"{DEBUG_FLAG} An exception occurred while loading the UI: {PageName}: {ex.Message}");
                // Perform any necessary cleanup here
                return; // Handle the exception here instead of re-throwing it
            }
    
            // If there are no exceptions and the resources have been successfully loaded, proceed to instantiate and setup the UI page
            string layerName = pageConfig.Layer.LayerName;
            UILayer uiLayer = uiRoot.GetUILayer(layerName);
            if (uiLayer.HasPage(PageName))
            {
                // Please note that within this framework, the opening of a UIPage must be unique;
                // that is, UI pages similar to Notifications should be managed within the page itself and should not be opened repeatedly for the same UI page.
                Debug.LogError($"{DEBUG_FLAG} Page already exists: {PageName}, layer: {uiLayer.LayerName}");
                return;
            }
            UIPage uiPage = diContainer.InstantiatePrefab(pagePrefab).GetComponent<UIPage>();
            System.Type pageType = uiPage.GetType();
            diContainer.Unbind(pageType);
            diContainer.Bind(pageType).FromInstance(uiPage).AsCached();
            uiPage.SetPageConfiguration(pageConfig);
            uiPage.SetPageName(PageName);
            uiLayer.AddPage(uiPage);
            OnPageCreated?.Invoke(uiPage);
            
            tcs.TrySetResult(true);
        }

        async UniTask CloseUIAsync(string PageName)
        {
            if (uiOpenTasks.TryGetValue(PageName, out var openTask))
            {
                // Waiting Open Task Finished
                await openTask.Task;
                uiOpenTasks.Remove(PageName);
            }
            
            UILayer layer = uiRoot.TryGetUILayerFromPageName(PageName);

            if (!layer)
            {
                Debug.LogWarning($"{DEBUG_FLAG} Can not find layer from PageName: {PageName}, you may Close UI multi times");
                return;
            }

            layer?.RemovePage(PageName);
            
            addressablesService.ReleaseAssetHandle(UIPathBuilder.GetConfigPath(PageName));
        }
        
        internal bool IsUIPageValid(string PageName)
        {
            // Check if the UI Root has a layer containing the page with the given name.
            UILayer layer = uiRoot.TryGetUILayerFromPageName(PageName);
            if (!layer)
            {
                // If the layer doesn't exist, the page is not valid.
                Debug.LogError($"{DEBUG_FLAG} Can not find layer from PageName: {PageName}");
                return false;
            }

            // If the page doesn't exist or isn't active, it's not valid.
            return layer.HasPage(PageName);
        }
        
        internal UIPage GetUIPage(string PageName)
        {
            // Check if the UI Root has a layer containing the page with the given name.
            UILayer layer = uiRoot.TryGetUILayerFromPageName(PageName);
            if (!layer)
            {
                // If the layer doesn't exist, the page is not valid.
                Debug.LogError($"{DEBUG_FLAG} Can not find layer from PageName: {PageName}");
                return null;
            }

            // If the page doesn't exist or isn't active, it's not valid.
            return layer.GetUIPage(PageName);
        }
    }
}