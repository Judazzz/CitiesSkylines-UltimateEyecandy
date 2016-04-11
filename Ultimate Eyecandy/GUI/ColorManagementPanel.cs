using UnityEngine;
using ColossalFramework.UI;

namespace UltimateEyecandy.GUI
{
    public class ColorManagamentPanel : UIPanel
    {
        private UILabel _lutLabel;
        private UIDropDown _lutDropdown;
        private UIButton _resetColorManagementButton;

        public UIDropDown lutDropdown
        {
            get { return _lutDropdown; }
        }

        private static ColorManagamentPanel _instance;
        public static ColorManagamentPanel instance
        {
            get { return _instance; }
        }

        public override void Start()
        {
            base.Start();
            _instance = this;
            canFocus = true;
            isInteractive = true;
            //  
            SetupControls();
        }

        private void SetupControls()
        {
            //  LUT Selection:
            var topContainer = UIUtils.CreateFormElement(this, "top");

            _lutLabel = topContainer.AddUIComponent<UILabel>();
            _lutLabel.text = "Select LUT";
            _lutLabel.textScale = 0.9f;
            _lutLabel.padding = new RectOffset(0, 0, 0, 5);

            _lutDropdown = UIUtils.CreateDropDown(topContainer);
            _lutDropdown.tooltip = "Select a different LUT (LookUp Table).";
            _lutDropdown.width = 245;
            foreach (var lut in ColorCorrectionManager.instance.items)
            {
                if (UltimateEyeCandy.config.outputDebug)
                {
                    DebugUtils.Log($"ColorPanel: LUT selected: {lut}");
                }
                //  
                _lutDropdown.AddItem(lut);
            }
            _lutDropdown.selectedIndex = ColorCorrectionManager.instance.lastSelection;
            _lutDropdown.eventSelectedIndexChanged += (c, i) =>
            {
                ColorCorrectionManager.instance.currentSelection = i;
                UltimateEyeCandy.currentSettings.color_selectedlut = i;
            };

            //  Reset button:
            var bottomContainer = UIUtils.CreateFormElement(this, "bottom");

            _resetColorManagementButton = UIUtils.CreateButton(bottomContainer);
            _resetColorManagementButton.name = "resetButton";
            _resetColorManagementButton.text = "Reset";
            _resetColorManagementButton.tooltip = "Reset all values set in this panel to default values.";
            _resetColorManagementButton.eventClicked += (c, e) =>
            {
                if (UltimateEyeCandy.config.outputDebug)
                {
                    DebugUtils.Log($"ColorPanel: 'Reset' clicked.");
                }
                //  
                _lutDropdown.selectedIndex = 0;
                UltimateEyeCandy.currentSettings.color_selectedlut = 0;
            };
        }
    }
}
