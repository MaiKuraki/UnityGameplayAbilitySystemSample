using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GASSample.Scene
{
    public class SimpleTransitionDirectorBehaviour : MonoBehaviour
    {
        [SerializeField] CanvasGroup m_CanvasGroup;
        [SerializeField] TMP_Text m_ProgressText;
        [SerializeField] TMP_Text m_MessageText;
        [SerializeField] Slider m_ProgressSlider;
        [SerializeField] Transform m_Spinner;

        public CanvasGroup CanvasGroup => m_CanvasGroup;
        public TMP_Text ProgressText => m_ProgressText;
        public TMP_Text MessageText => m_MessageText;
        public Slider ProgressSlider => m_ProgressSlider;

        private float EnterTransitionDuration;
        private float ExitTransitionDuration;
        
        void Awake()
        {
            SetPorgressGroupVisibility(false);
        }

        public void SetPorgressGroupVisibility(bool bNewVisible)
        {
            if (ProgressText?.gameObject.activeInHierarchy != bNewVisible) ProgressText?.gameObject.SetActive(bNewVisible);
            if (MessageText?.gameObject.activeInHierarchy != bNewVisible) MessageText?.gameObject.SetActive(bNewVisible);
            if (ProgressSlider?.gameObject.activeInHierarchy != bNewVisible) ProgressSlider?.gameObject.SetActive(bNewVisible);
        }

        public void SetSpinnerVisibility(bool bNewVisible)
        {
            m_Spinner?.gameObject?.SetActive(bNewVisible);
        }

        public void InitTransitionData(float EnterTransitionDuration, float ExitTransitionDuration)
        {
            this.EnterTransitionDuration = EnterTransitionDuration;
            this.ExitTransitionDuration = ExitTransitionDuration;
        }

        public async UniTask StartTransition(CancellationToken cancellationToken)
        {
            await LMotion.Create(m_CanvasGroup.alpha, 1f, 1f)
                    .WithCancelOnError(true)
                    .BindToAlpha(m_CanvasGroup)
                    .ToUniTask(cancellationToken);
        }

        public async UniTask EndTransition(CancellationToken cancellationToken)
        {
            await LMotion.Create(m_CanvasGroup.alpha, 0f, 1f)
                    .WithCancelOnError(true)
                    .BindToAlpha(m_CanvasGroup)
                    .ToUniTask(cancellationToken);
        }
    }
}