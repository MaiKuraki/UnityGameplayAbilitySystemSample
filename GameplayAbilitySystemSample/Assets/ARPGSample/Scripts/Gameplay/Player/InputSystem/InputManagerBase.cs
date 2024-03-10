using System;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    public interface IInputManager
    {
        event Action<Vector2> OnUpdateVec_0;
        event Action OnClickBtn_0;
        event Action OnClickBtn_1;
        event Action OnClickBtn_2;
        event Action OnClickBtn_3;
        void UpdateVec_0(Vector2 InVec);
        void ClickBtn_0();
        void ClickBtn_1();
        void ClickBtn_2();
        void ClickBtn_3();
        void AddVecAction_0(Action<Vector2> evt);
        void RemoveVecAction_0(Action<Vector2> evt);
        void AddButtonAction_0(Action evt);
        void RemoveButtonAction_0(Action evt);
        void AddButtonAction_1(Action evt);
        void RemoveButtonAction_1(Action evt);
        void AddButtonAction_2(Action evt);
        void RemoveButtonAction_2(Action evt);
        void AddButtonAction_3(Action evt);
        void RemoveButtonAction_3(Action evt);
        void EnableInput();
        void BlockInput();
    }
    public abstract class InputManagerBase : MonoBehaviour, IInputManager
    {
        public event Action<Vector2> OnUpdateVec_0;
        public event Action OnClickBtn_0;
        public event Action OnClickBtn_1;
        public event Action OnClickBtn_2;
        public event Action OnClickBtn_3;

        public virtual void UpdateVec_0(Vector2 InVec)
        {
            OnUpdateVec_0?.Invoke(InVec);
        }

        public virtual void ClickBtn_0()
        {
            OnClickBtn_0?.Invoke();
        }

        public virtual void ClickBtn_1()
        {
            OnClickBtn_1?.Invoke();
        }

        public virtual void ClickBtn_2()
        {
            OnClickBtn_2?.Invoke();
        }

        public virtual void ClickBtn_3()
        {
            OnClickBtn_3?.Invoke();
        }
        
        public virtual void AddVecAction_0(Action<Vector2> evt)
        {
            OnUpdateVec_0 -= evt;
            OnUpdateVec_0 += evt;
        }

        public virtual void RemoveVecAction_0(Action<Vector2> evt)
        {
            OnUpdateVec_0 -= evt;
        }

        public virtual void AddButtonAction_0(Action evt)
        {
            OnClickBtn_0 -= evt;
            OnClickBtn_0 += evt;
        }

        public virtual void RemoveButtonAction_0(Action evt)
        {
            OnClickBtn_0 -= evt;
        }

        public virtual void AddButtonAction_1(Action evt)
        {
            OnClickBtn_1 -= evt;
            OnClickBtn_1 += evt;
        }

        public virtual void RemoveButtonAction_1(Action evt)
        {
            OnClickBtn_1 -= evt;
        }
        
        public virtual void AddButtonAction_2(Action evt)
        {
            OnClickBtn_2 -= evt;
            OnClickBtn_2 += evt;
        }

        public virtual void RemoveButtonAction_2(Action evt)
        {
            OnClickBtn_2 -= evt;
        }
        
        public virtual void AddButtonAction_3(Action evt)
        {
            OnClickBtn_3 -= evt;
            OnClickBtn_3 += evt;
        }

        public virtual void RemoveButtonAction_3(Action evt)
        {
            OnClickBtn_3 -= evt;
        }

        protected virtual void OnDestroy()
        {
            OnUpdateVec_0 = null;
            OnClickBtn_0 = null;
            OnClickBtn_1 = null;
            OnClickBtn_2 = null;
            OnClickBtn_3 = null;
        }

        public abstract void BlockInput();
        public abstract void EnableInput();
    }
}