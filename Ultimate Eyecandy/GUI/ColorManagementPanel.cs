using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using UnityEngine;

namespace UltimateEyecandy.GUI
{
    public class ColorManagementPanel : UIPanel
    {
        private UILabel _lutLabel;
        private UIFastList _lutFastlist;
        private UIButton _loadLutButton;
        private UIButton _resetColorManagementButton;

        public UIFastList lutFastlist
        {
            get { return _lutFastlist; }
        }

        public UIButton loadLutButton
        {
            get { return _loadLutButton; }
        }

        public LutList.Lut _selectedLut;

        private static ColorManagementPanel _instance;
        public static ColorManagementPanel instance
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

            PopulateLutFastList();
        }

        private void SetupControls()
        {
            //  LUT Selection:
            var topContainer = UIUtils.CreateFormElement(this, "top");

            _lutLabel = topContainer.AddUIComponent<UILabel>();
            _lutLabel.text = "Select LUT";
            _lutLabel.textScale = 0.8f;
            _lutLabel.padding = new RectOffset(0, 0, 0, 5);

            // FastList
            _lutFastlist = UIFastList.Create<UILutItem>(topContainer);
            _lutFastlist.backgroundSprite = "UnlockingPanel";
            _lutFastlist.width = parent.width - (3 * UltimateEyecandy.SPACING) - 12;
            _lutFastlist.height = 150;
            _lutFastlist.canSelect = true;
            _lutFastlist.eventSelectedIndexChanged += OnSelectedItemChanged;

            //  Load lut:
            var loadContainer = UIUtils.CreateFormElement(this, "center");
            loadContainer.name = "loadDeleteContainer";
            loadContainer.relativePosition = new Vector3(0, 190);
            loadContainer.autoLayout = false;
            loadContainer.isVisible = true;

            _loadLutButton = UIUtils.CreateButton(loadContainer);
            _loadLutButton.width = 100f;
            _loadLutButton.isEnabled = false;
            _loadLutButton.opacity = 0.5f;
            _loadLutButton.relativePosition = new Vector3(5, 10);
            _loadLutButton.name = "loadLutButton";
            _loadLutButton.text = "Load lut";
            _loadLutButton.tooltip = "LUT selected in list is already active.";
            _loadLutButton.eventClicked += (c, e) =>
            {
                try
                {
                    DebugUtils.Log($"ColorManagementPanel: 'Load lut' clicked: {_selectedLut.name} ({_selectedLut.internal_name} / {_selectedLut.index}).");
                    UltimateEyecandy.currentSettings.color_selectedlut = _selectedLut.internal_name;
                    ColorCorrectionManager.instance.currentSelection = _selectedLut.index;
                }
                catch (Exception ex)
                {
                    if (UltimateEyecandy.config.outputDebug)
                    {
                        DebugUtils.Log($"ColorManagementPanel: 'Load lut' clicked: lut {_selectedLut.name} not found, applying default Lut for current biome ({LoadingManager.instance.m_loadedEnvironment}).");
                    }
                    DebugUtils.LogException(ex);
                    _lutFastlist.DisplayAt(0);
                    UltimateEyecandy.currentSettings.color_selectedlut = LutList.GetLutNameByIndex(ColorCorrectionManager.instance.lastSelection); // "None";
                    ColorCorrectionManager.instance.currentSelection = 0;
                }
            };

            //  Reset button:
            var bottomContainer = UIUtils.CreateFormElement(this, "bottom");

            _resetColorManagementButton = UIUtils.CreateButton(bottomContainer);
            _resetColorManagementButton.name = "resetButton";
            _resetColorManagementButton.text = "Reset";
            _resetColorManagementButton.tooltip = "Reset all values set in this panel to default values.";
            _resetColorManagementButton.eventClicked += (c, e) =>
            {
                if (UltimateEyecandy.config.outputDebug)
                {
                    DebugUtils.Log($"ColorPanel: 'Reset' clicked.");
                }
                //  
                _lutFastlist.DisplayAt(0);
                _lutFastlist.selectedIndex = 0;
                UltimateEyecandy.currentSettings.color_selectedlut = LutList.GetLutNameByIndex(ColorCorrectionManager.instance.lastSelection); // "None";
                ColorCorrectionManager.instance.currentSelection = 0;
            };
        }

        public void PopulateLutFastList()
        {
            _lutFastlist.rowsData.Clear();
            //  
            var allLuts = LutList.GetLutList();
            for (int i = 0; i < allLuts.Count; i++)
            {
                if (allLuts[i] != null)
                {
                    _lutFastlist.rowsData.Add(allLuts[i]);
                }
            }
            //  
            _lutFastlist.rowHeight = 26f;
            _lutFastlist.DisplayAt(ColorCorrectionManager.instance.lastSelection);
            _lutFastlist.selectedIndex = ColorCorrectionManager.instance.lastSelection;
        }

        protected void OnSelectedItemChanged(UIComponent component, int i)
        {
            _selectedLut = _lutFastlist.rowsData[i] as LutList.Lut;
            //  
            if (UltimateEyecandy.config.outputDebug)
            {
                DebugUtils.Log($"ColorManagementPanel: LutFastList SelectedItemChanged: {_selectedLut.name} selected.");
            }
            //  Button appearance:
            var isActive = (_selectedLut.internal_name == UltimateEyecandy.currentSettings.color_selectedlut);
            _loadLutButton.isEnabled = (isActive) ? false : true;
            _loadLutButton.opacity = (isActive) ? 0.5f : 1.0f;
            _loadLutButton.tooltip = (isActive) ? "LUT selected in list is already active." : "Load LUT selected in list.";
        }

        protected void OnEnableStateChanged(UIComponent component, bool state)
        {
            _lutFastlist.DisplayAt(_lutFastlist.listPosition);
        }
    }

    public class LutList
    {
        public static Lut GetLut(string name)
        {
            foreach (var lut in GetLutList())
            {
                if (lut.internal_name == name) return lut;
            }
            return null;
        }

        public static string GetLutNameByIndex(int index)
        {
            foreach (var lut in GetLutList())
            {
                if (lut.index == index) return lut.name;
            }
            return "None";
        }

        public static List<Lut> GetLutList()
        {
            var list = new List<Lut>();
            var i = 0;
            //  
            foreach (var lut in ColorCorrectionManager.instance.items)
            {
                Lut l = new Lut()
                {
                    index = i,
                    name = GetLutDisplayName(lut),
                    internal_name = lut
                };
                list.Add(l);
                i++;
            }
            return list;
        }

        public static string GetLutDisplayName(string name)
        {
            //  Get friendly Workshop Lut name:
            if (name.Contains('.'))
            {
                name = name.Remove(0, 10);
            }
            //  Get friendly Built-In Lut name:
            else {
                if (name.ToLower() == "lutsunny")
                {
                    name = "Temperate";
                }
                if (name.ToLower() == "lutnorth")
                {
                    name = "Boreal";
                }
                if (name.ToLower() == "luttropical")
                {
                    name = "Tropical";
                }
                if (name.ToLower() == "luteurope")
                {
                    name = "European";
                }
            }
            return name;
        }

        public class Lut
        {
            public int index;
            public string name;
            public string internal_name;
        }
    }
}
