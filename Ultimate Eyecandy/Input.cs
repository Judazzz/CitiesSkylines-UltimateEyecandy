using ICities;
using UnityEngine;
using ColossalFramework.UI;

namespace UltimateEyecandy
{

    public class InputThreadingExtension : ThreadingExtensionBase
    {
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (Input.GetKey(KeyCode.F9)){

                //do stuff when f9 is hit. 
                }
            
        }
    }
}
