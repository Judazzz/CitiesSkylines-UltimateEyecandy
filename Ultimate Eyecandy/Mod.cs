using System;
using ColossalFramework.UI;
using ICities;

namespace UltimateEyecandy
{
    public class Mod : IUserMod
    {
        public const string version = "1.4.1";

        public string Name
        {
            get
            {
                return "Ultimate Eyecandy " + version;
            }
        }

        public string Description
        {
            get
            {
                return "A wealth of visual settings at your fingertips!";
            }
        }

        //  Mod options:
        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {
                UltimateEyecandyTool.LoadConfig();
                UIHelperBase group = helper.AddGroup(Name);
                group.AddSpace(10);
                //  Keyboard Shortcut:
                UIDropDown keyboardShortcutDropdown = (UIDropDown)group.AddDropdown("Select your preferred keyboard shortcut for toggling the mod panel", new[] { "Shift + U", "Ctrl + U", "Alt + U" }, UltimateEyecandyTool.config.keyboardShortcut,
                    b =>
                    {
                        UltimateEyecandyTool.config.keyboardShortcut = b;
                        UltimateEyecandyTool.SaveConfig(false);
                    });
                keyboardShortcutDropdown.tooltip = "Select your preferred keyboard shortcut for toggling the mod panel.";
                group.AddSpace(15);
                //  Auto-Load Last Preset:
                UICheckBox loadLastPresetOnStartCheckBox = (UICheckBox)group.AddCheckbox("Load last active preset on start.", UltimateEyecandyTool.config.loadLastPresetOnStart,
                    b =>
                    {
                        if (UltimateEyecandyTool.config.loadLastPresetOnStart != b)
                        {
                            UltimateEyecandyTool.config.loadLastPresetOnStart = b;
                            UltimateEyecandyTool.SaveConfig(false);
                        }
                    });
                loadLastPresetOnStartCheckBox.tooltip = "Load last active preset on start.";
                group.AddSpace(15);
                //  Enable Time of Day:
                UICheckBox enableSimulationControlCheckBox = (UICheckBox)group.AddCheckbox("Enable simulation control (Time of Day and simulation speed).", UltimateEyecandyTool.config.enableSimulationControl,
                    b =>
                    {
                        if (UltimateEyecandyTool.config.enableSimulationControl != b)
                        {
                            UltimateEyecandyTool.config.enableSimulationControl = b;
                            UltimateEyecandyTool.SaveConfig(false);
                        }
                    });
                loadLastPresetOnStartCheckBox.tooltip = "Enable the Time of Day slider functionality.";
                group.AddSpace(15);
                //  Output Debug Data:
                UICheckBox debugCheckBox = (UICheckBox)group.AddCheckbox("Write data to debug log (it's going to be a lot!)", UltimateEyecandyTool.config.outputDebug,
                    b =>
                    {
                        if (UltimateEyecandyTool.config.outputDebug != b)
                        {
                            UltimateEyecandyTool.config.outputDebug = b;
                            UltimateEyecandyTool.SaveConfig(false);
                        }
                    });
                debugCheckBox.tooltip = "Output messages to debug log. This may be useful when experiencing issues with this mod.";
                //group.AddSpace(20);

            }
            catch (Exception e)
            {
                DebugUtils.LogException(e);
            }
        }
    }
}
