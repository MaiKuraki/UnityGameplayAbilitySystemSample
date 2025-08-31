using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GASSample.UI
{
    public class StatusBar : MonoBehaviour
    {
        [SerializeField] private Slider Bar;
        [SerializeField] private TMP_Text DisplayInfo;

        public void Reset()
        {
            Bar.value = 0;
            if (DisplayInfo) DisplayInfo.text = "";
        }
        public void Refresh(in float inValue, in string DisplayInfoStr)
        {
            Bar.value = inValue;
            if (DisplayInfo) DisplayInfo.text = DisplayInfoStr;
        }
    }
}