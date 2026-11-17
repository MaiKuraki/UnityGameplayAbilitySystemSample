using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GASSample
{
    /// <summary>
    /// Black full-screen overlay between scene loads. Builds its own overlay canvas on first use and
    /// survives scene changes.
    /// </summary>
    public static class ScreenFade
    {
        private const float Duration = 0.15f;

        private static CanvasGroup group;

        /// <summary>Covers the screen immediately, for the first scene where there is nothing to fade from.</summary>
        public static void CoverNow()
        {
            if (group == null)
            {
                group = CreateOverlay();
            }

            group.alpha = 1f;
            group.blocksRaycasts = true;
        }

        public static async UniTask FadeOutAsync()
        {
            await FadeAsync(1f);
            group.blocksRaycasts = true;
        }

        public static async UniTask FadeInAsync()
        {
            await FadeAsync(0f);
            group.blocksRaycasts = false;
        }

        private static async UniTask FadeAsync(float targetAlpha)
        {
            if (group == null)
            {
                group = CreateOverlay();
            }

            float startAlpha = group.alpha;
            float elapsed = 0f;
            while (elapsed < Duration)
            {
                elapsed += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / Duration);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            group.alpha = targetAlpha;
        }

        private static CanvasGroup CreateOverlay()
        {
            var root = new GameObject("[ScreenFade]");
            Object.DontDestroyOnLoad(root);

            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue;
            root.AddComponent<GraphicRaycaster>();

            var black = new GameObject("Black").AddComponent<Image>();
            black.color = Color.black;
            black.transform.SetParent(root.transform, false);

            RectTransform rect = black.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var canvasGroup = root.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            return canvasGroup;
        }
    }
}
