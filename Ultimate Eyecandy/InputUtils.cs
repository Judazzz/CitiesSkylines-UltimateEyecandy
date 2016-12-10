using UnityEngine;

namespace UltimateEyecandy
{
    class InputUtils
    {
        public static bool IsComboPressed()
        {
            bool validInput = false;
            //  Preferred key combo: [Shift] + [U]:
            if (((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyUp(KeyCode.U)) && UltimateEyecandyTool.config.keyboardShortcut == 0)
            {
                validInput = true;
                //DebugUtils.Log($"FUCKING Shift-U!");
            }
            //  Preferred key combo: [Ctrl] + [U]:
            if (((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyUp(KeyCode.U)) && UltimateEyecandyTool.config.keyboardShortcut == 1)
            {
                validInput = true;
                //DebugUtils.Log($"FUCKING Control-U!");
            }
            //  Preferred key combo: [Alt] + [U]:
            if (((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyUp(KeyCode.U)) && UltimateEyecandyTool.config.keyboardShortcut == 2)
            {
                validInput = true;
                //DebugUtils.Log($"FUCKING Alt-U!");
            }
            return validInput;
        }
    }
}