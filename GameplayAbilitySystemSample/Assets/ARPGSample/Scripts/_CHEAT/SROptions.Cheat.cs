using System.ComponentModel;
using CycloneGames.Service;
using UnityEngine;

//  NOTE: This class is come from SRDebugger Tools, you can buy it in AssetStore: https://assetstore.unity.com/packages/tools/gui/srdebugger-console-tools-on-device-27688
public partial class SROptions
{
    private static readonly string DEBUG_FLAG = "<color=red>[CHEAT]</color>";

    private CheatService _cheatServiceInstance;
    private CheatService CheatServiceInstance
    {
        get
        {
            if (_cheatServiceInstance == null)
            {
                _cheatServiceInstance = GameObject.Find("CheatService")?.GetComponent<CheatService>();
            }
            return _cheatServiceInstance;
        }
    }
    
    //  NOTE: You can find proxima here: https://assetstore.unity.com/packages/tools/utilities/proxima-runtime-inspector-free-253649

    // private GameObject _proxima;
    // private Proxima.ProximaInspector _proximaInspector;
    // private static bool bFirstTimeToggleProxima = true;
    //
    // private GameObject Proxima
    // {
    //     get
    //     {
    //         if (_proxima == null)
    //         {
    //             _proxima = GameObject.Find("Proxima");
    //             if (_proxima != null)
    //             {
    //                 _proximaInspector = _proxima.GetComponent<Proxima.ProximaInspector>();
    //             }
    //         }
    //
    //         return _proxima;
    //     }
    // }
    //
    // // Toggles the Proxima's active state after the first call to this method.
    // [Category("DEBUG"), DisplayName("Proxima")]
    // public void ToggleProxima()
    // {
    //     if (Proxima == null) return;
    //
    //     // Only toggle active state after the first invocation.
    //     if (!bFirstTimeToggleProxima) 
    //     {
    //         Proxima.SetActive(!Proxima.activeInHierarchy);
    //     }
    //
    //     // Ensure the _proximaInspector component is enabled when Proxima is active.
    //     if (Proxima.activeInHierarchy && _proximaInspector != null)
    //     {
    //         _proximaInspector.enabled = true;
    //     }
    //
    //     bFirstTimeToggleProxima = false;
    // }
}