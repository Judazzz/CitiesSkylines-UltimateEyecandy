using ColossalFramework;
using ICities;
using System;
using UltimateEyecandy.GUI;

namespace UltimateEyecandy
{
    public class ThreadingExtension : ThreadingExtensionBase
    {
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            base.OnUpdate(realTimeDelta, simulationTimeDelta);

            try
            {
                //  Execute code only when in-game:
                if (UltimateEyecandyTool.isGameLoaded)
                {
                    //  Register Key combo:
                    bool flag = InputUtils.IsComboPressed();
                    if (flag)
                    {
                        DebugUtils.Log($"Hotkey pressed.");
                        UIMainPanel.instance.Toggle();
                    }

                    //  Register InfoMode change:
                    //  Re-disable CameraBehavior settings after Resource Overlay is closed (if necessary):
                    if (Singleton<InfoManager>.instance.CurrentMode == InfoManager.InfoMode.None)
                    {
                        //  Re-disable LUT if it was disabled in Color Correction Panel:
                        if (UltimateEyecandyTool.currentSettings.color_lut == false && ColorManagementPanel.instance.GetCameraBehaviour("ColorCorrectionLut").enabled)
                        {
                            ColorManagementPanel.instance.GetCameraBehaviour("ColorCorrectionLut").enabled = false;
                        }
                        //  Re-disable Tonemapping if it was disabled in Color Correction Panel:
                        if (UltimateEyecandyTool.currentSettings.color_tonemapping == false && ColorManagementPanel.instance.GetCameraBehaviour("ToneMapping").enabled)
                        {
                            ColorManagementPanel.instance.GetCameraBehaviour("ToneMapping").enabled = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugUtils.LogException(e);
            }
        }
    }
}