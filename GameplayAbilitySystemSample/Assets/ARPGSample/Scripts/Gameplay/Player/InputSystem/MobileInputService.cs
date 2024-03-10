using System;
using ARPGSample.GameSubSystem;
using ARPGSample.UI;
using CycloneGames.Service;
using CycloneGames.UIFramework;
using UnityEngine;
using Zenject;

namespace ARPGSample.Gameplay
{
    public class MobileInputService : IInitializable, IInputService
    {
        private static readonly string DEBUG_FLAG = "[MobileInput]";
        
        [Inject] private DiContainer diContainer;
        [Inject] private IServiceDisplay serviceDisplay;
        [Inject] private IUIService uiService;
        [Inject] private ISceneManagementService sceneManagementService;

        private IInputService.InputBlockHandler blockStateHandler;

        private GameplayTouchInputPage cachedGameplayTouchInputPage;

        public void Initialize()
        {
            uiService.OpenUI(PageName.GameplayTouchInputPage, page =>
            {
                cachedGameplayTouchInputPage = (GameplayTouchInputPage)page;
                SetInputBlockState(new IInputService.InputBlockHandler("HiddenOnStart", true));
            });
            
            sceneManagementService.BindStartEvent(BlockInputOnLoading);
        }

        public void SetInputBlockState(IInputService.InputBlockHandler blockHandler)
        {
            cachedGameplayTouchInputPage?.SetPageVisibility(!blockHandler.BlockInput);

            blockStateHandler = blockHandler;
            
            Debug.Log($"{DEBUG_FLAG} BlockInput: {blockHandler.BlockInput}, Feature: {blockHandler.FeatureName}");
        }

        public IInputService.InputBlockHandler BlockStateHandler => blockStateHandler;

        public void AddVecAction_0(Action<Vector2> Evt)
        {
            cachedGameplayTouchInputPage.AddVecAction_0(Evt);
        }

        public void RemoveVecAction_0(Action<Vector2> Evt)
        {
            cachedGameplayTouchInputPage.RemoveVecAction_0(Evt);
        }

        public void AddBtnAction_0(Action Evt)
        {
            cachedGameplayTouchInputPage.AddButtonAction_0(Evt);
        }

        public void RemoveBtnAction_0(Action Evt)
        {
            cachedGameplayTouchInputPage.RemoveButtonAction_0(Evt);
        }

        public void AddBtnAction_1(Action Evt)
        {
            cachedGameplayTouchInputPage.AddButtonAction_1(Evt);
        }

        public void RemoveBtnAction_1(Action Evt)
        {
            cachedGameplayTouchInputPage.RemoveButtonAction_1(Evt);
        }

        public void AttBtnAction_2(Action Evt)
        {
            cachedGameplayTouchInputPage.AddButtonAction_2(Evt);
        }

        public void RemoveBtnAction_2(Action Evt)
        {
            cachedGameplayTouchInputPage.RemoveButtonAction_2(Evt);
        }

        public void AttBtnAction_3(Action Evt)
        {
            cachedGameplayTouchInputPage.AddButtonAction_3(Evt);
        }

        public void RemoveBtnAction_3(Action Evt)
        {
            cachedGameplayTouchInputPage.RemoveButtonAction_3(Evt);
        }

        private void BlockInputOnLoading()
        {
            SetInputBlockState(new IInputService.InputBlockHandler("HiddenOnLoading", true));
        }
    }
}