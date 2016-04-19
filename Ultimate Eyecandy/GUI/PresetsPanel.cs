using UnityEngine;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;

namespace UltimateEyecandy.GUI
{
    public class PresetsPanel : UIPanel
    {
        private UILabel _presetLabel;
        public UIFastList _presetFastlist;
        public Configuration.Preset _selectedPreset;

        public UIButton _loadPresetButton;
        public UIButton _deletePresetButton;

        public UIButton _savePresetButton;
        public UIButton _overwritePresetButton;

        public UIButton _resetAllButton;

        public UIPanel _presetsContainer;
        public UIListBox _presetsListbox;
        public string[] _presets;

        private static PresetsPanel _instance;
        public static PresetsPanel instance => _instance;

        public override void Start()
        {
            base.Start();

            UltimateEyeCandy.LoadConfig();
            if (UltimateEyeCandy.config.outputDebug)
            {
                DebugUtils.Log($"Ultimate Eyecandy: configuration loaded");
            }
            _instance = this;
            canFocus = true;
            isInteractive = true;
            //  
            SetupControls();
            PopulatePresetsFastList();
            //  Create temporary preset for current settings:
            if (UltimateEyeCandy.config.loadLastPresetOnStart && !string.IsNullOrEmpty(UltimateEyeCandy.config.lastPreset))
            {
                //  Create temporary preset based on last active preset:
                try {
                    UltimateEyeCandy.LoadPreset(UltimateEyeCandy.config.lastPreset);
                    if (UltimateEyeCandy.config.outputDebug)
                    {
                        DebugUtils.Log($"Temporary preset created based on last active preset '{UltimateEyeCandy.config.lastPreset}'.");
                    }
                }
                catch
                {
                    //  Latest preset not found: create temporary preset from scratch:
                    UltimateEyeCandy.CreateTemporaryPreset();
                }
            }
            else
            {
                //  Create temporary preset from scratch:
                UltimateEyeCandy.CreateTemporaryPreset();
            }
        }

        private void SetupControls()
        {
            //  Presets:
            var topContainer = UIUtils.CreateFormElement(this, "top");

            _presetLabel = topContainer.AddUIComponent<UILabel>();
            _presetLabel.text = "Load preset";
            _presetLabel.textScale = 0.9f;
            _presetLabel.padding = new RectOffset(0, 0, 0, 5);

            // FastList
            _presetFastlist = UIFastList.Create<UIPresetItem>(topContainer);
            _presetFastlist.backgroundSprite = "UnlockingPanel";
            _presetFastlist.width = 245;
            _presetFastlist.height = 150;
            _presetFastlist.canSelect = true;
            _presetFastlist.eventSelectedIndexChanged += OnSelectedItemChanged;

            //  Load/delete preset:
            var loadDeleteContainer = UIUtils.CreateFormElement(this, "center");
            loadDeleteContainer.name = "loadDeleteContainer";
            loadDeleteContainer.relativePosition = new Vector3(0, 190);
            loadDeleteContainer.autoLayout = false;
            loadDeleteContainer.isVisible = true;

            _loadPresetButton = UIUtils.CreateButton(loadDeleteContainer);
            _loadPresetButton.width = 100f;
            _loadPresetButton.opacity = 0.25f;
            _loadPresetButton.isEnabled = false;
            _loadPresetButton.relativePosition = new Vector3(10, 10);
            _loadPresetButton.name = "loadPresetButton";
            _loadPresetButton.text = "Load preset";
            _loadPresetButton.tooltip = "Load preset selected in list.";
            _loadPresetButton.eventClicked += (c, e) =>
            {
                if (UltimateEyeCandy.config.outputDebug)
                {
                    DebugUtils.Log($"PresetsPanel: 'Load preset' clicked: preset '{_selectedPreset.name}'.");
                }
                UltimateEyeCandy.LoadPreset(_selectedPreset.name);
            };

            _deletePresetButton = UIUtils.CreateButton(loadDeleteContainer);
            _deletePresetButton.width = 100f;
            _deletePresetButton.opacity = 0.25f;
            _deletePresetButton.isEnabled = false;
            _deletePresetButton.relativePosition = new Vector3(150, 10);
            _deletePresetButton.name = "deletePresetButton";
            _deletePresetButton.text = "Delete preset";
            _deletePresetButton.tooltip = "Delete preset selected in list.";
            _deletePresetButton.eventClicked += (c, e) =>
            {
                if (UltimateEyeCandy.config.outputDebug)
                {
                    DebugUtils.Log($"PresetsPanel: 'Delete preset' clicked: preset '{_selectedPreset.name}'.");
                }
                UltimateEyeCandy.DeletePreset(_selectedPreset);
                PopulatePresetsFastList();
            };

            //  Save/overwrite preset:
            var saveOverwriteContainer = UIUtils.CreateFormElement(this, "center");
            saveOverwriteContainer.relativePosition = new Vector3(0, 240);
            saveOverwriteContainer.autoLayout = false;
            saveOverwriteContainer.isVisible = true;

            _savePresetButton = UIUtils.CreateButton(saveOverwriteContainer);
            _savePresetButton.width = 100f;
            _savePresetButton.relativePosition = new Vector3(10, 10);
            _savePresetButton.name = "savePresetButton";
            _savePresetButton.text = "Save as new";
            //  Todo: add all settings to tooltip(?)
            _savePresetButton.tooltip = "Save current settings as a new preset (create new preset).";
            _savePresetButton.eventClicked += (c, e) =>
            {
                if (UltimateEyeCandy.config.outputDebug)
                {
                    DebugUtils.Log($"PresetsPanel: 'Save preset' clicked.");
                }
                //  Open 'Preset name' modal:
                UIView.PushModal(UINewPresetModal.instance);
                UINewPresetModal.instance.Show(true);
            };

            _overwritePresetButton = UIUtils.CreateButton(saveOverwriteContainer);
            _overwritePresetButton.width = 100f;
            _overwritePresetButton.opacity = 0.25f;
            _overwritePresetButton.isEnabled = false;
            _overwritePresetButton.relativePosition = new Vector3(150, 10);
            _overwritePresetButton.name = "overwritePresetButton";
            _overwritePresetButton.text = "Overwrite";
            _overwritePresetButton.tooltip = "Save current settings as the preset selected in the list (overwrite existing preset).";
            _overwritePresetButton.eventClicked += (c, e) =>
            {
                if (UltimateEyeCandy.config.outputDebug)
                {
                    DebugUtils.Log($"PresetsPanel: 'Overwrite preset' clicked: preset '{_selectedPreset.name}'.");
                }
                UltimateEyeCandy.CreatePreset(_selectedPreset.name, true);
            };

            //  Reset all:
            var resetContainer = UIUtils.CreateFormElement(this, "bottom");

            _resetAllButton = UIUtils.CreateButton(resetContainer);
            _resetAllButton.name = "resetAllButton";
            _resetAllButton.text = "Reset all";
            _resetAllButton.tooltip = "Reset all values set in all panels to default values.";
            _resetAllButton.eventClicked += (c, e) =>
            {
                if (UltimateEyeCandy.config.outputDebug)
                {
                    DebugUtils.Log($"PresetsPanel: 'Reset all' clicked.");
                }
                UltimateEyeCandy.ResetAll();
            };
        }

        public void PopulatePresetsFastList()
        {
            _presetFastlist.rowsData.Clear();
            _presetFastlist.selectedIndex = -1;
            //  
            List<Configuration.Preset> allPresets = UltimateEyeCandy.config.presets;
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
                _presetFastlist.rowHeight = 32f;
                _presetFastlist.DisplayAt(0);
            }
        }

        protected void OnSelectedItemChanged(UIComponent component, int i)
        {
            if (i >= 0)
            {
                _overwritePresetButton.opacity = 1f;
                _overwritePresetButton.isEnabled = true;
            }
            //  
            _selectedPreset = _presetFastlist.rowsData[i] as Configuration.Preset;
            UltimateEyeCandy.currentSettings = _selectedPreset;
            //  
            if (UltimateEyeCandy.config.outputDebug)
            {
                DebugUtils.Log($"PresetsPanel: FastListItem selected: preset '{_selectedPreset.name}'.");
            }
            //  Show buttons 'Load/Delete Preset' buttons:
            _loadPresetButton.isEnabled = true;
            _loadPresetButton.opacity = 1f;
            _deletePresetButton.isEnabled = true;
            _deletePresetButton.opacity = 1f;
        }

        protected void OnEnableStateChanged(UIComponent component, bool state)
        {
            _presetFastlist.DisplayAt(_presetFastlist.listPosition);
        }
    }
}