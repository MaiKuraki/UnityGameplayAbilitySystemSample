using System;
using ThirdParty.JoystickUI.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ARPGSample.UI
{
    public class RPGFloatingJoystick : FixedJoystick
    {
        private bool bShouldTickJoystick;
        public event Action<Vector2> OnUpdateJoystick;
        private Vector2 cachedJoystickVec = Vector2.zero;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            
            cachedJoystickVec.x = Horizontal;
            cachedJoystickVec.y = Vertical;
            OnUpdateJoystick?.Invoke(cachedJoystickVec);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            
            OnUpdateJoystick?.Invoke(Vector2.zero);
        }

        protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
        {
            base.HandleInput(magnitude, normalised, radius, cam);

            cachedJoystickVec.x = Horizontal;
            cachedJoystickVec.y = Vertical;
            
            OnUpdateJoystick?.Invoke(cachedJoystickVec);
        }
    }
}

