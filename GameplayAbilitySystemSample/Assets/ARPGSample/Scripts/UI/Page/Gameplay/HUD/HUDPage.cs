using System;
using CycloneGames.GameFramework;
using CycloneGames.UIFramework;
using MessagePipe;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ARPGSample.UI
{
    public class HUDPage : UIPage
    {
        [Inject] private IWorld world;
        [Inject] private ISubscriber<UIMessage> uiMsgSub;
        
        [SerializeField] private Slider sliderBar_Health;
        [SerializeField] private Slider sliderBar_Stamina;
        
        IDisposable disposableSubscribe;


        protected override void Awake()
        {
            base.Awake();
            
            Reset();
            SubscribeUIMessage();
        }

        void SubscribeUIMessage()
        {
            var subscribeHandler = uiMsgSub.Subscribe(msg =>
            {
                if (msg.Params != null && msg.Params.Length > 0)
                {
                    if (msg.MessageCode == RPGUIMessage.REFRESH_PLAYER_HEALTH_VALUE)
                    {
                        if (msg.Params is { Length: > 0 } && msg.Params[0] is float newHealthVal)
                        {
                            UpdateSlider_Health(newHealthVal);
                        }
                    }

                    if (msg.MessageCode == RPGUIMessage.REFRESH_PLAYER_STAMINA_VALUE)
                    {
                        if (msg.Params is { Length: > 0 } && msg.Params[0] is float newStaminaVal)
                        {
                            UpdateSlider_Stamina(newStaminaVal);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Invalid Params");
                }
            });
            
            disposableSubscribe = DisposableBag.Create(subscribeHandler);
        }

        void Reset()
        {
            sliderBar_Health.value = 0;
            sliderBar_Stamina.value = 0;
        }

        void UpdateSlider_Health(float newVal)
        {
            sliderBar_Health.value = newVal;
        }
        
        void UpdateSlider_Stamina(float newVal)
        {
            sliderBar_Stamina.value = newVal;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            disposableSubscribe?.Dispose();
        }
    }
}
