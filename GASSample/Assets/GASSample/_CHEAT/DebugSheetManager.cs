using System;
using System.Reflection;
using IngameDebugConsole;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Extensions.IngameDebugConsole;
using UnityDebugSheet.Runtime.Extensions.Unity;
using UnityEngine;

namespace GASSample.Cheat
{
    public class DebugSheetManager : MonoBehaviour
    {
        private DebugPage debugPageRoot;

        public static DebugSheetManager Instance { get; private set; }
        [SerializeField] bool _singleton = true;

        void Awake()
        {
            if (_singleton)
            {
                if (Instance != null && Instance != this)
                {
                    Destroy(gameObject);
                    return;
                }

                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
            debugPageRoot = DebugSheet.Instance?.GetOrCreateInitialPage();

            debugPageRoot?.AddButton("Toggle FPS", clicked: () =>
            {
                IsFPSVisible = !IsFPSVisible;
                SetFPSVisibility(IsFPSVisible);
            });
            debugPageRoot?.AddPageLinkButton<IngameDebugConsoleDebugPage>("In-Game Console Debug", onLoad: x => x.page.Setup(DebugLogManager.Instance));
            debugPageRoot?.AddPageLinkButton<ScreenDebugPage>("Screen");
            debugPageRoot?.AddPageLinkButton<SystemInfoDebugPage>("System Info");
        }

        private bool IsFPSVisible = false;
        private void SetFPSVisibility(bool isVisible)
        {
            try
            {
                Type fpsType = Type.GetType("CycloneGames.Utility.Runtime.FPSCounter, Assembly-CSharp");

                if (fpsType == null)
                {
                    fpsType = Type.GetType("CycloneGames.Utility.Runtime.FPSCounter, CycloneGames.Utility.Runtime");

                    if (fpsType == null)
                    {
                        Debug.LogError("FPSCounter type not found");
                        return;
                    }
                }
                UnityEngine.Object[] fpsCounters = UnityEngine.Object.FindObjectsByType(fpsType, FindObjectsSortMode.None);

                if (fpsCounters.Length == 0)
                {
                    Debug.LogWarning("No FPSCounter instances found");
                    return;
                }
                FieldInfo field = fpsType.GetField("IsVisible",
                    BindingFlags.Public | BindingFlags.Instance);

                if (field == null)
                {
                    Debug.LogError("IsVisible field not found");
                    return;
                }
                foreach (UnityEngine.Object counter in fpsCounters)
                {
                    field.SetValue(counter, isVisible);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Reflection failed: {ex}");
            }
        }
    }
}
