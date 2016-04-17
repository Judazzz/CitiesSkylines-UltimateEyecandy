using System;
using UnityEngine;
using ColossalFramework.UI;
using System.Collections.Generic;
using System.Linq;

namespace UltimateEyecandy.GUI
{
    public class ColorManagamentPanel : UIPanel
    {
        private UILabel _lutLabel;
        public UIFastList _lutFastlist;
        public LutList.Lut _selectedLut;
        public UIButton _loadLutButton;

        private UIButton _resetColorManagementButton;

        public UIFastList lutFastlist
        {
            get { return _lutFastlist; }
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

            PopulateLutFastList();
        }

        private void SetupControls()
        {
            //  LUT Selection:
            var topContainer = UIUtils.CreateFormElement(this, "top");

            _lutLabel = topContainer.AddUIComponent<UILabel>();
            _lutLabel.text = "Select LUT";
            _lutLabel.textScale = 0.9f;
            _lutLabel.padding = new RectOffset(0, 0, 0, 5);

            // FastList
            _lutFastlist = UIFastList.Create<UILutItem>(topContainer);
            _lutFastlist.backgroundSprite = "UnlockingPanel";
            _lutFastlist.width = 245;
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
            _loadLutButton.opacity = 0.25f;
            _loadLutButton.isEnabled = false;
            _loadLutButton.relativePosition = new Vector3(10, 10);
            _loadLutButton.name = "loadLutButton";
            _loadLutButton.text = "Load lut";
            _loadLutButton.tooltip = "Load lut selected in list.";
            _loadLutButton.eventClicked += (c, e) =>
            {
                try
                {
                    DebugUtils.Log($"ColorManagementPanel: 'Load lut' clicked: {_selectedLut.name} ({_selectedLut.internal_name} / {_selectedLut.index}).");
                    UltimateEyeCandy.currentSettings.color_selectedlut = _selectedLut.internal_name;
                    ColorCorrectionManager.instance.currentSelection = _selectedLut.index;
                }
                catch (Exception ex)
                {
                    if (UltimateEyeCandy.config.outputDebug)
                    {
                        DebugUtils.Log($"ColorManagementPanel: 'Load lut' clicked: lut {_selectedLut.name} not found, applying default Lut for current biome ({LoadingManager.instance.m_loadedEnvironment}).");
                    }
                    DebugUtils.LogException(ex);
                    _lutFastlist.DisplayAt(0);
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
                if (UltimateEyeCandy.config.outputDebug)
                {
                    DebugUtils.Log($"ColorPanel: 'Reset' clicked.");
                }
                //  
                _lutFastlist.DisplayAt(0);
                _lutFastlist.selectedIndex = 0;
                UltimateEyeCandy.currentSettings.color_selectedlut = "None";
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
            _lutFastlist.rowHeight = 32f;
            _lutFastlist.DisplayAt(ColorCorrectionManager.instance.lastSelection);
            _lutFastlist.selectedIndex = ColorCorrectionManager.instance.lastSelection;
        }

        protected void OnSelectedItemChanged(UIComponent component, int i)
        {
            _selectedLut = _lutFastlist.rowsData[i] as LutList.Lut;
            UltimateEyeCandy.currentSettings.color_selectedlut = _selectedLut.internal_name;
            //  
            if (UltimateEyeCandy.config.outputDebug)
            {
                DebugUtils.Log($"ColorManagementPanel: LutFastList SelectedItemChanged: {_selectedLut.name} selected.");
            }
            //  Show buttons 'Load/Delete Preset' buttons:
            _loadLutButton.isEnabled = true;
            _loadLutButton.opacity = 1f;
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
