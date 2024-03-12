using UnityEngine;
using UnityEngine.UI;

namespace ARPGSample.UI
{
    public class SimpleEnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        public Vector2 PrivateOffset => privateOffset;
        private Vector2 privateOffset;
        
        private void Awake()
        {
            
        }

        public void SetHealthBarVal(float newHealthVal)
        {
            if (!slider)
            {
                Debug.LogError("Invalid Slider");
                return;
            }
            slider.value = newHealthVal;
        }

        public void SetPrivateOffset(Vector2 newOffset)
        {
            privateOffset = newOffset;
        }
    }
}