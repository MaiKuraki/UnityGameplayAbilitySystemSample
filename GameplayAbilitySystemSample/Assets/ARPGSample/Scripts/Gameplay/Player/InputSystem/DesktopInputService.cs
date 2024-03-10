using System;
using ARPGSample.GameSubSystem;
using CycloneGames.Service;
using UnityEngine;
using Zenject;

namespace ARPGSample.Gameplay
{
    public class DesktopInputService : IInitializable, IInputService
    {
        private static readonly string DEBUG_FLAG = "[DesktopInput]";
        [Inject] private DiContainer diContainer;
        [Inject] private IServiceDisplay serviceDisplay;
        [Inject] private ISceneManagementService sceneManagementService;

        
        private DesktopInputManager desktopInputManager;
        private IInputService.InputBlockHandler blockStateHandler;
        
        public void Initialize()
        {
            desktopInputManager = diContainer.InstantiateComponentOnNewGameObject<DesktopInputManager>("DesktopInputManager");
            desktopInputManager.transform.SetParent(serviceDisplay.ServiceDisplayTransform);
            diContainer.BindInstance(desktopInputManager).AsSingle();
            diContainer.QueueForInject(desktopInputManager);
            
            sceneManagementService.BindStartEvent(BlockInputOnLoading);
        }
        public void SetInputBlockState(IInputService.InputBlockHandler newBlock)
        {
            blockStateHandler = newBlock;

            if (blockStateHandler.BlockInput)
            {
                desktopInputManager.BlockInput();
            }
            else
            {
                desktopInputManager.EnableInput();
            }
            
            Debug.Log($"{DEBUG_FLAG} BlockInput: {newBlock.BlockInput}, Feature: {newBlock.FeatureName}");
        }

        public IInputService.InputBlockHandler BlockStateHandler { get; }
        public void AddVecAction_0(Action<Vector2> Evt)
        {
            desktopInputManager.AddVecAction_0(Evt);
        }

        public void RemoveVecAction_0(Action<Vector2> Evt)
        {
            desktopInputManager.RemoveVecAction_0(Evt);
        }

        public void AddBtnAction_0(Action Evt)
        {
            desktopInputManager.AddButtonAction_0(Evt);
        }

        public void RemoveBtnAction_0(Action Evt)
        {
            desktopInputManager.RemoveButtonAction_0(Evt);
        }

        public void AddBtnAction_1(Action Evt)
        {
            desktopInputManager.AddButtonAction_1(Evt);
        }

        public void RemoveBtnAction_1(Action Evt)
        {
            desktopInputManager.RemoveButtonAction_1(Evt);
        }

        public void AttBtnAction_2(Action Evt)
        {
            desktopInputManager.AddButtonAction_2(Evt);
        }

        public void RemoveBtnAction_2(Action Evt)
        {
            desktopInputManager.RemoveButtonAction_2(Evt);
        }

        public void AttBtnAction_3(Action Evt)
        {
            desktopInputManager.AddButtonAction_3(Evt);
        }

        public void RemoveBtnAction_3(Action Evt)
        {
            desktopInputManager.RemoveButtonAction_3(Evt);
        }

        void BlockInputOnLoading()
        {
            SetInputBlockState(new IInputService.InputBlockHandler("HiddenOnLoading", true));
        }
    }
}