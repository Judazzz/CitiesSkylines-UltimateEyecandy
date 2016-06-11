using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;

namespace UltimateEyecandy.GUI
{
    public class PresetsPanel : UIPanel
    {
        private UILabel _presetLabel;
        private UIFastList _presetFastlist;

        private UIButton _loadPresetButton;
        private UIButton _deletePresetButton;
        private UIButton _savePresetButton;
        private UIButton _overwritePresetButton;

        private UIButton _resetAllButton;

        public UIFastList presetFastlist
        {
            get { return _presetFastlist; }
        }
        public UIButton loadPresetButton
        {
            get { return _loadPresetButton; }
        }
        public UIButton deletePresetButton
        {
            get { return _deletePresetButton; }
        }
        public UIButton overwritePresetButton
        {
            get { return _overwritePresetButton; }
        }

        public Configuration.Preset _selectedPreset;
        public string[] _presets;

        private static PresetsPanel _instance;
        public static PresetsPanel instance => _instance;

        public override void Start()
        {
            base.Start();
            _instance = this;
            canFocus = true;
            isInteractive = true;
            //  
            SetupControls();
            PopulatePresetsFastList();
            //  Create temporary preset for current settings:
            if (UltimateEyecandy.config.loadLastPresetOnStart && !string.IsNullOrEmpty(UltimateEyecandy.config.lastPreset))
            {
                //  Create temporary preset based on last active preset:
                try
                {
                    UltimateEyecandy.LoadPreset(UltimateEyecandy.config.lastPreset, false);
                    if (UltimateEyecandy.config.outputDebug)
                    {
                        DebugUtils.Log($"Temporary preset created based on last active preset '{UltimateEyecandy.config.lastPreset}'.");
                    }
                }
                catch
                {
                    //  Latest preset not found: create temporary preset from scratch:
                    UltimateEyecandy.ResetAll();
                }
            }
            else
            {
                //  Fallback: create temporary preset from scratch:
                UltimateEyecandy.ResetAll();
            }
        }

        private void SetupControls()
        {
            //  Presets:
            var topContainer = UIUtils.CreateFormElement(this, "top");

            _presetLabel = topContainer.AddUIComponent<UILabel>();
            _presetLabel.text = "Load preset";
            _presetLabel.textScale = 0.8f;
            _presetLabel.padding = new RectOffset(0, 0, 0, 5);

            // FastList
            _presetFastlist = UIFastList.Create<UIPresetItem>(topContainer);
            _presetFastlist.backgroundSprite = "UnlockingPanel";
            _presetFastlist.width = parent.width - (3 * UltimateEyecandy.SPACING) - 12;
            _presetFastlist.height = 125;
            _presetFastlist.canSelect = true;
            _presetFastlist.eventSelectedIndexChanged += OnSelectedItemChanged;

            //  Load/delete preset:
            var loadDeleteContainer = UIUtils.CreateFormElement(this, "center");
            loadDeleteContainer.height = 40;
            loadDeleteContainer.name = "loadDeleteContainer";
            loadDeleteContainer.relativePosition = new Vector3(0, 165); // new Vector3(0, 190);
            loadDeleteContainer.autoLayout = false;
            loadDeleteContainer.isVisible = true;

            _loadPresetButton = UIUtils.CreateButton(loadDeleteContainer);
            //_loadPresetButton.width = 100f;
            _loadPresetButton.opacity = 0.25f;
            _loadPresetButton.isEnabled = false;
            _loadPresetButton.relativePosition = new Vector3(5, 10);
            _loadPresetButton.name = "loadPresetButton";
            _loadPresetButton.text = "Load preset";
            _loadPresetButton.tooltip = "Load Preset selected in list.";
            _loadPresetButton.eventClicked += (c, e) =>
            {
                //  
                if (UltimateEyecandy.config.outputDebug)
                {
                    DebugUtils.Log($"PresetsPanel: 'Load preset' clicked: preset '{_selectedPreset.name}'.");
                }
                UltimateEyecandy.LoadPreset(_selectedPreset.name, true);
                //  Button appearance:
                updateButtons(true);
            };

            _deletePresetButton = UIUtils.CreateButton(loadDeleteContainer);
            //_deletePresetButton.width = 100f;
            _deletePresetButton.opacity = 0.25f;
            _deletePresetButton.isEnabled = false;
            _deletePresetButton.relativePosition = new Vector3(160, 10);
            _deletePresetButton.name = "deletePresetButton";
            _deletePresetButton.text = "Delete preset";
            _deletePresetButton.tooltip = "Delete Preset selected in list.";
            _deletePresetButton.eventClicked += (c, e) =>
            {
                //  
                if (UltimateEyecandy.config.outputDebug)
                {
                    DebugUtils.Log($"PresetsPanel: 'Delete preset' clicked: preset '{_selectedPreset.name}'.");
                }
                ConfirmPanel.ShowModal("Delete Preset", "Are you sure you want to delete Preset '" + _selectedPreset.name + "'?", (d, i) => {
                    if (i == 1)
                    {
                        UltimateEyecandy.DeletePreset(_selectedPreset);
                        //  Update FastList:
                        PopulatePresetsFastList();
                        //  Button appearance:
                        updateButtons(true);
                    }
                });
            };

            //  Save/overwrite preset:
            var saveOverwriteContainer = UIUtils.CreateFormElement(this, "center");
            saveOverwriteContainer.height = 40;
            saveOverwriteContainer.relativePosition = new Vector3(0, 205); // new Vector3(0, 230);
            saveOverwriteContainer.autoLayout = false;
            saveOverwriteContainer.isVisible = true;

            _savePresetButton = UIUtils.CreateButton(saveOverwriteContainer);
            //_savePresetButton.width = 100f;
            _savePresetButton.relativePosition = new Vector3(5, 10);
            _savePresetButton.name = "savePresetButton";
            _savePresetButton.text = "Save as new";
            //  Todo: add all settings to tooltip(?)
            _savePresetButton.tooltip = "Save current settings as a new Preset (create New Preset).";
            _savePresetButton.eventClicked += (c, e) =>
            {
                //  
                if (UltimateEyecandy.config.outputDebug)
                {
                    DebugUtils.Log($"PresetsPanel: 'Save preset' clicked.");
                }
                //  Open 'Preset name' modal:
                UIView.PushModal(UINewPresetModal.instance);
                UINewPresetModal.instance.Show(true);
                //  Button appearance:
                updateButtons(true);
            };

            _overwritePresetButton = UIUtils.CreateButton(saveOverwriteContainer);
            //_overwritePresetButton.width = 100f;
            _overwritePresetButton.opacity = 0.25f;
            _overwritePresetButton.isEnabled = false;
            _overwritePresetButton.relativePosition = new Vector3(160, 10);
            _overwritePresetButton.name = "overwritePresetButton";
            _overwritePresetButton.text = "Overwrite";
            _overwritePresetButton.tooltip = "Save current settings as the Preset selected in the list (overwrite Existing Preset).";
            _overwritePresetButton.eventClicked += (c, e) =>
            {
                //  
                if (UltimateEyecandy.config.outputDebug)
                {
                    DebugUtils.Log($"PresetsPanel: 'Overwrite preset' clicked: preset '{_selectedPreset.name}'.");
                }
                ConfirmPanel.ShowModal("Overwrite Preset", "Are you sure you want to overwrite Preset '" + _selectedPreset.name + "'?", (d, i) => {
                    if (i == 1)
                    {
                        UltimateEyecandy.CreatePreset(_selectedPreset.name, true);
                        //  Button appearance:
                        updateButtons(true);
                    }
                });
            };

            //  Reset all:
            var resetContainer = UIUtils.CreateFormElement(this, "bottom");

            _resetAllButton = UIUtils.CreateButton(resetContainer);
            _resetAllButton.name = "resetAllButton";
            _resetAllButton.text = "Reset all";
            _resetAllButton.tooltip = "Reset all values set in all panels to default values.";
            _resetAllButton.eventClicked += (c, e) =>
            {
                //  
                if (UltimateEyecandy.config.outputDebug)
                {
                    DebugUtils.Log($"PresetsPanel: 'Reset all' clicked.");
                }
                UltimateEyecandy.ResetAll();
                //  Button appearance:
                updateButtons(true);
            };
        }

        public void PopulatePresetsFastList()
        {
            _presetFastlist.rowsData.Clear();
            _presetFastlist.selectedIndex = -1;
            //  
            List<Configuration.Preset> allPresets = UltimateEyecandy.config.presets;
            if (allPresets.Count > 0)
            {
                for (int i = 0; i < allPresets.Count; i++)
                {
                    if (allPresets[i] != null)
                    {
                        _presetFastlist.rowsData.Add(allPresets[i]);
                    }
                }
                //  
                _presetFastlist.rowHeight = 26f;
                _presetFastlist.DisplayAt(0);
            }
        }

        protected void OnSelectedItemChanged(UIComponent component, int i)
        {
            _selectedPreset = _presetFastlist.rowsData[i] as Configuration.Preset;
            if (UltimateEyecandy.config.outputDebug)
            {
                DebugUtils.Log($"PresetsPanel: FastListItem selected: preset '{_selectedPreset.name}'.");
            }
            //  Button appearance:
            updateButtons(false);
        }

        protected void OnEnableStateChanged(UIComponent component, bool state)
        {
            _presetFastlist.DisplayAt(_presetFastlist.listPosition);
        }

        public void updateButtons(bool disableAll)
        {
            _loadPresetButton.opacity = (disableAll) ? 0.25f : 1f;
            _loadPresetButton.isEnabled = (disableAll) ? false : true;
            _deletePresetButton.opacity = (disableAll) ? 0.25f : 1f;
            _deletePresetButton.isEnabled = (disableAll) ? false : true;
            _overwritePresetButton.opacity = (disableAll) ? 0.25f : 1f;
            _overwritePresetButton.isEnabled = (disableAll) ? false : true;
        }
    }
}