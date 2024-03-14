using UnityEngine;
using UnityEngine.UI;

namespace ARPGSample.UI
{
    public class SimpleEnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;

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
    }
}