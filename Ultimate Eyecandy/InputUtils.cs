using UnityEngine;

namespace UltimateEyecandy
{
    class InputUtils
    {
        public static bool HotkeyPressed()
        {
            bool validInput = false;
            //  Preferred hotkey: [Shift] + [U]:
            if (((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyUp(KeyCode.U)) && UltimateEyecandyTool.config.keyboardShortcut == 0)
            {
                validInput = true;
            }
            //  Preferred hotkey: [Ctrl] + [U]:
            if (((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyUp(KeyCode.U)) && UltimateEyecandyTool.config.keyboardShortcut == 1)
            {
                validInput = true;
            }
            //  Preferred hotkey: [Alt] + [U]:
            if (((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyUp(KeyCode.U)) && UltimateEyecandyTool.config.keyboardShortcut == 2)
            {
                validInput = true;
            }
            return validInput;
        }
    }
}