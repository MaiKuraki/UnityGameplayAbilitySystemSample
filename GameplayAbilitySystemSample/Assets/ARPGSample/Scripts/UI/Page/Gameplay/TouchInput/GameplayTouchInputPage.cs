using System;
using CycloneGames.UIFramework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ARPGSample.UI
{
    public class GameplayTouchInputPage : UIPage
    {
        [SerializeField] private Transform nodeForVisibility;
        [SerializeField] private RPGFloatingJoystick joystickLeft;
        [SerializeField] private RPGTouchInputSimpleButton Btn_0;
        [SerializeField] private RPGTouchInputSimpleButton Btn_1;
        [SerializeField] private RPGTouchInputSimpleButton Btn_2;
        [SerializeField] private RPGTouchInputSimpleButton Btn_3;

        event Action<Vector2> OnUpdateVec_0;
        event Action OnClickBtn_0;
        event Action OnClickBtn_1;
        event Action OnClickBtn_2;
        event Action OnClickBtn_3;
        
        protected override void Awake()
        {
            base.Awake();
            
            joystickLeft.OnUpdateJoystick += UpdateVec_0;
            if(Btn_0) Btn_0.OnPointerDownEvent += PointerDownBtn_0;
            if(Btn_1) Btn_1.OnPointerDownEvent += PointerDownBtn_1;
            if(Btn_2) Btn_2.OnPointerDownEvent += PointerDownBtn_2;
            if(Btn_3) Btn_3.OnPointerDownEvent += PointerDownBtn_3;
        }

        public void SetPageVisibility(bool bVisible)
        {
            nodeForVisibility?.gameObject?.SetActive(bVisible);
        }
        
        private void UpdateVec_0(Vector2 InVec)
        {
            OnUpdateVec_0?.Invoke(InVec);
        }

        private void ClickBtn_0()
        {
            OnClickBtn_0?.Invoke();
        }

        private void PointerDownBtn_0(PointerEventData data)
        {
            OnClickBtn_0?.Invoke();
        }

        private void ClickBtn_1()
        {
            OnClickBtn_1?.Invoke();
        }

        private void PointerDownBtn_1(PointerEventData data)
        {
            OnClickBtn_1?.Invoke();
        }

        private void ClickBtn_2()
        {
            OnClickBtn_2?.Invoke();
        }

        private void PointerDownBtn_2(PointerEventData data)
        {
            OnClickBtn_2?.Invoke();
        }

        private void ClickBtn_3()
        {
            OnClickBtn_3?.Invoke();
        }

        private void PointerDownBtn_3(PointerEventData data)
        {
            OnClickBtn_3?.Invoke();
        }
        
        public void AddVecAction_0(Action<Vector2> evt)
        {
            OnUpdateVec_0 -= evt;
            OnUpdateVec_0 += evt;
        }

        public void RemoveVecAction_0(Action<Vector2> evt)
        {
            OnUpdateVec_0 -= evt;
        }

        public void AddButtonAction_0(Action evt)
        {
            OnClickBtn_0 -= evt;
            OnClickBtn_0 += evt;
        }

        public void RemoveButtonAction_0(Action evt)
        {
            OnClickBtn_0 -= evt;
        }

        public void AddButtonAction_1(Action evt)
        {
            OnClickBtn_1 -= evt;
            OnClickBtn_1 += evt;
        }

        public void RemoveButtonAction_1(Action evt)
        {
            OnClickBtn_1 -= evt;
        }
        
        public void AddButtonAction_2(Action evt)
        {
            OnClickBtn_2 -= evt;
            OnClickBtn_2 += evt;
        }

        public void RemoveButtonAction_2(Action evt)
        {
            OnClickBtn_2 -= evt;
        }
        
        public void AddButtonAction_3(Action evt)
        {
            OnClickBtn_3 -= evt;
            OnClickBtn_3 += evt;
        }

        public void RemoveButtonAction_3(Action evt)
        {
            OnClickBtn_3 -= evt;
        }

        protected override void OnDestroy()
        {
            if(Btn_0) Btn_0.OnPointerDownEvent -= PointerDownBtn_0;
            if(Btn_1) Btn_1.OnPointerDownEvent -= PointerDownBtn_1;
            if(Btn_2) Btn_2.OnPointerDownEvent -= PointerDownBtn_2;
            if(Btn_3) Btn_3.OnPointerDownEvent -= PointerDownBtn_3;
            
            OnUpdateVec_0 = null;
            OnClickBtn_0 = null;
            OnClickBtn_1 = null;
            OnClickBtn_2 = null;
            OnClickBtn_3 = null;
            
            base.OnDestroy();
        }
    }
}