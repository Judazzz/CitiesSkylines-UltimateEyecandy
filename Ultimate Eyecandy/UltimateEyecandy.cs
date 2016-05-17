using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ColossalFramework.UI;
using ColossalFramework.Plugins;
using ICities;
using UnityEngine;
using UltimateEyecandy.GUI;

namespace UltimateEyecandy
{
    public class ModInfo : IUserMod
    {
        public string Name
        {
            get
            {
                return "Ultimate Eyecandy";
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
                UltimateEyecandy.LoadConfig();
                Debug.Log($"OnSettingsUI: configuration {UltimateEyecandy.config.version} loaded");
                
                //UICheckBox checkBox;
                UIHelperBase group = helper.AddGroup(Name);
                group.AddSpace(10);

                //  Output Debug Data:
                UICheckBox debugCheckBox = (UICheckBox)group.AddCheckbox("Write data to debug log (it's going to be a lot!)", UltimateEyecandy.config.outputDebug,
                    b =>
                    {
                        if (UltimateEyecandy.config.outputDebug != b)
                        {
                            UltimateEyecandy.config.outputDebug = b;
                            UltimateEyecandy.SaveConfig(false);
                        }
                    });
                debugCheckBox.tooltip = "Output messages to debug log. This may be useful when experiencing issues with this mod.";
                group.AddSpace(20);

                //  Auto-Load Last Preset:
                UICheckBox loadLastPresetOnStartCheckBox = (UICheckBox)group.AddCheckbox("Load last active preset on start.", UltimateEyecandy.config.loadLastPresetOnStart,
                    b =>
                    {
                        if (UltimateEyecandy.config.loadLastPresetOnStart != b)
                        {
                            UltimateEyecandy.config.loadLastPresetOnStart = b;
                            UltimateEyecandy.SaveConfig(false);
                        }
                    });
                loadLastPresetOnStartCheckBox.tooltip = "Load last active preset on start.";
                group.AddSpace(20);

                //  Advanced Options (disabled for now):
                //UICheckBox advancedCheckBox = (UICheckBox)group.AddCheckbox("Enable advanced mod settings (not yet implemented).", UltimateEyecandy.config.enableAdvanced,
                //    b =>
                //    {
                //        if (UltimateEyecandy.config.enableAdvanced != b)
                //        {
                //            UltimateEyecandy.config.enableAdvanced = b;
                //            UltimateEyecandy.SaveConfig(false);
                //        }
                //    });
                //advancedCheckBox.tooltip = "Enable advanced mod settings (not yet implemented).";
                //group.AddSpace(10);
                //group.AddGroup("WARNING: playing with the advanced settings may result in unexpected behavior of\nthe game's simulation, so it is strongly recommended to only use these settings on a\nbacked up save game and to NOT save the game afterwards.\nTL;DR: use these settings at your own risk, you have been warned!");
            }
            catch (Exception e)
            {
                DebugUtils.LogException(e);
            }
        }

        public const string version = "1.1.1";
    }

    public class UltimateEyecandy : LoadingExtensionBase
    {
        private static GameObject _gameObject;
        private static ModMainPanel _modMainPanel;

        //  Size Constants:
        public static float WIDTH = 270;
        public static float HEIGHT = 350;
        public static float SPACING = 5;
        public static float TITLE_HEIGHT = 36;
        public static float TABS_HEIGHT = 28;

        private const string FileName = "CSL_UltimateEyecandy.xml";
        private const string FileNameLocal = "CSL_UltimateEyecandy_local.xml";

        public static bool isWinterMap = false;

        public static Configuration config = new Configuration();
        //  In-Memory Preset for storing initial settings:
        public static Configuration.Preset initialSettings = new Configuration.Preset();
        //  In-Memory Preset for storing current settings:
        public static Configuration.Preset currentSettings = new Configuration.Preset();

        public static bool hasDaylightClassic = false;

        public static bool isGameLoaded = false;

        public static OptionsGameplayPanel optionsGameplayPanel = new OptionsGameplayPanel();

        #region LoadingExtensionBase overrides
        public override void OnCreated(ILoading loading)
        {
            try
            {
                // Create backup:
                SaveBackup();
            }
            catch (Exception e)
            {
                DebugUtils.LogException(e);
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            try
            {
                // In-game?
                if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                {
                    return;
                }
                //  
                isGameLoaded = true;
                // Creating GUI:
                UIView view = UIView.GetAView();
                _gameObject = new GameObject("UltimateEyecandy");
                _gameObject.transform.SetParent(view.transform);
                //  Back up initial values:
                SaveInitialValues();
                isWinterMap = LoadingManager.instance.m_loadedEnvironment.ToLower() == "winter";
                //  
                try
                {
                    optionsGameplayPanel = UnityEngine.Object.Instantiate<GameObject>(UnityEngine.Object.FindObjectOfType<OptionsGameplayPanel>().gameObject).GetComponent<OptionsGameplayPanel>();
                    //  
                    _modMainPanel = _gameObject.AddComponent<ModMainPanel>();
                    _modMainPanel.AddGuiToggle();
                    if (config.outputDebug)
                    {
                        DebugUtils.Log("MainPanel created");
                    }
                }
                catch (Exception e)
                {
                    DebugUtils.LogException(e);
                    //  
                    if (_gameObject != null)
                        GameObject.Destroy(_gameObject);
                    return;
                }
            }
            catch (Exception e)
            {
                if (_gameObject != null)
                {
                    GameObject.Destroy(_gameObject);
                }
                DebugUtils.LogException(e);
            }
        }

        public override void OnLevelUnloading()
        {
            try
            {
                //  Delete current settings:
                currentSettings = null;
                //  
                GUI.UIUtils.DestroyDeeply(_modMainPanel);
                if (_gameObject != null)
                    GameObject.Destroy(_gameObject);

                isGameLoaded = false;
            }
            catch (Exception e)
            {
                DebugUtils.LogException(e);
            }
        }

        public override void OnReleased()
        {
            //try
            //{
            //    DebugUtils.Log("Restoring default values");
            //    DefaultOptions.RestoreAll();
            //    DefaultOptions.Clear();
            //}
            //catch (Exception e)
            //{
            //    DebugUtils.LogException(e);
            //}
        }
        #endregion

        public static void SaveBackup()
        {
            var fileName = (PluginManager.noWorkshop) ? FileNameLocal : FileName;
            if (!File.Exists(fileName)) return;

            File.Copy(fileName, fileName + ".bak", true);
            //  
            if (config.outputDebug)
            {
                DebugUtils.Log("Backup configuration file created.");
            }
        }

        public static void RestoreBackup()
        {
            var fileName = (PluginManager.noWorkshop) ? FileNameLocal : FileName;
            if (!File.Exists(fileName + ".bak")) return;

            File.Copy(fileName + ".bak", fileName, true);
            //  
            if (config.outputDebug)
            {
                DebugUtils.Log("Backup configuration file restored.");
            }
        }

        public static void LoadConfig()
        {
            var fileName = (PluginManager.noWorkshop) ? FileNameLocal : FileName;
            if (!isGameLoaded)
            {
                if (File.Exists(fileName))
                {
                    config = Configuration.Deserialize(fileName);
                }
                return;
            }
            //  Load config:
            if (!File.Exists(fileName))
            {
                //  No config:
                if (config.outputDebug)
                {
                    DebugUtils.Log("Configuration file not found. Creating new configuration file.");
                }
                //  Create and save new config:
                config = new Configuration();
                SaveConfig(false);
                //  Create temporary preset for current settings:
                ResetAll();
                return;
            }
            //  
            config = Configuration.Deserialize(fileName);
            ResetAll();
        }

        public static void SaveConfig(bool reloadUI = true)
        {
            var fileName = (PluginManager.noWorkshop) ? FileNameLocal : FileName;
            Configuration.Serialize(fileName, config, reloadUI);
        }

        public static List<Configuration.Preset> GetAllPresets()
        {
            return config.presets;
        }
        
        public static Configuration.Preset GetPresetByName(string presetName)
        {
            return config.presets.FirstOrDefault(preset => preset.name == presetName);
        }

        public static void CreatePreset(string presetName, bool overWriteExisting)
        {
            //  Overwrite existing preset:
            if (overWriteExisting)
            {
                Configuration.Preset existingPreset = GetPresetByName(presetName);
                existingPreset = currentSettings;
                //  
                if (config.outputDebug)
                {
                    DebugUtils.Log($"Preset '{presetName}' overwritten.");
                }
            }
            //  Create new preset:
            else {
                var newPreset = new Configuration.Preset()
                {
                    name = presetName,
                    ambient_height = currentSettings.ambient_height,
                    ambient_rotation = currentSettings.ambient_rotation,
                    ambient_intensity = currentSettings.ambient_intensity,
                    ambient_ambient = currentSettings.ambient_ambient,
                    weather = currentSettings.weather,
                    weather_rainintensity = currentSettings.weather_rainintensity,
                    weather_rainmotionblur = currentSettings.weather_rainmotionblur,
                    weather_fogintensity = currentSettings.weather_fogintensity,
                    weather_snowintensity = currentSettings.weather_snowintensity,
                    color_selectedlut = currentSettings.color_selectedlut
                };
                config.presets.Add(newPreset);
                //  
                if (config.outputDebug)
                {
                    DebugUtils.Log($"Preset '{presetName}' created.");
                }
            }
            //  
            config.lastPreset = presetName;
            SaveConfig();
        }

        public static void LoadPreset(string presetName)
        {
            //  
            try
            {
                var selectedPreset = GetPresetByName(presetName);
                currentSettings = selectedPreset;
                //  Apply values to UI Elements:
                //  Ambient values:
                _modMainPanel.ambientPanel.heightSlider.value = currentSettings.ambient_height;
                _modMainPanel.ambientPanel.rotationSlider.value = currentSettings.ambient_rotation;
                _modMainPanel.ambientPanel.intensitySlider.value = currentSettings.ambient_intensity;
                _modMainPanel.ambientPanel.ambientSlider.value = currentSettings.ambient_ambient;
                //  Weather values:
                _modMainPanel.weatherPanel.enableWeatherCheckbox.isChecked = currentSettings.weather;
                if (isWinterMap)
                {
                    _modMainPanel.weatherPanel.precipitationSlider.value = currentSettings.weather_snowintensity;
                }
                else
                {
                    _modMainPanel.weatherPanel.precipitationSlider.value = currentSettings.weather_rainintensity;
                    _modMainPanel.weatherPanel.rainMotionblurCheckbox.isChecked = currentSettings.weather_rainmotionblur;
                }
                _modMainPanel.weatherPanel.fogIntensitySlider.value = currentSettings.weather_fogintensity;
                //  ColorManagement values:
                LutList.Lut activeLut = LutList.GetLut(currentSettings.color_selectedlut);
                if (activeLut == null)
                {
                    if (config.outputDebug)
                    {
                        DebugUtils.Log($"Load preset: lut {activeLut} not found, applying default Lut for current biome ({LoadingManager.instance.m_loadedEnvironment}).");
                    }
                    _modMainPanel.colorManagementPanel.lutFastlist.DisplayAt(0);
                    _modMainPanel.colorManagementPanel.lutFastlist.selectedIndex = 0;
                    ColorCorrectionManager.instance.currentSelection = 0;
                }
                else
                {
                    _modMainPanel.colorManagementPanel.lutFastlist.DisplayAt(activeLut.index);
                    _modMainPanel.colorManagementPanel.lutFastlist.selectedIndex = activeLut.index;
                    ColorCorrectionManager.instance.currentSelection = activeLut.index;
                }
                //  
                config.lastPreset = presetName;
                SaveConfig();
            }
            catch (Exception ex)
            {
                DebugUtils.LogException(ex);
            }
        }

        public static void DeletePreset(Configuration.Preset preset)
        {
            config.presets.Remove(preset);
            //  
            SaveConfig();
        }

        //  Reset all values and (re-)apply initial settings to In-Memory Preset for storing current settings:
        public static void ResetAll()
        {
            currentSettings = initialSettings;
            //  Apply to UI:
            _modMainPanel.ambientPanel.heightSlider.value = currentSettings.ambient_height;
            _modMainPanel.ambientPanel.rotationSlider.value = currentSettings.ambient_rotation;
            _modMainPanel.ambientPanel.intensitySlider.value = currentSettings.ambient_intensity;
            _modMainPanel.ambientPanel.ambientSlider.value = currentSettings.ambient_ambient;
            if (isWinterMap)
            {
                _modMainPanel.weatherPanel.precipitationSlider.value = currentSettings.weather_snowintensity;
            }
            else
            {
                _modMainPanel.weatherPanel.precipitationSlider.value = currentSettings.weather_rainintensity;
                _modMainPanel.weatherPanel.rainMotionblurCheckbox.isChecked = currentSettings.weather_rainmotionblur;
            }
            _modMainPanel.weatherPanel.fogIntensitySlider.value = currentSettings.weather_fogintensity;
            _modMainPanel.weatherPanel.enableWeatherCheckbox.isChecked = currentSettings.weather;
            _modMainPanel.colorManagementPanel.lutFastlist.DisplayAt(0);
            _modMainPanel.colorManagementPanel.lutFastlist.selectedIndex = 0;
            ColorCorrectionManager.instance.currentSelection = 0;
            //  
            if (config.outputDebug)
            {
                DebugUtils.Log("Temporary preset created from scratch.");
            }
        }

        //  Create In-Memory Preset for storing initial settings (to reset to default values independent of similar mods like Daylight Classic):
        public static void SaveInitialValues()
        {
            initialSettings = new Configuration.Preset()
            {
                name = string.Empty,
                ambient_height = DayNightProperties.instance.m_Latitude,
                ambient_rotation = DayNightProperties.instance.m_Longitude,
                ambient_intensity = DayNightProperties.instance.m_SunIntensity,
                ambient_ambient = DayNightProperties.instance.m_Exposure,
                weather = false, //optionsGameplayPanel.enableWeather,
                weather_rainintensity = 0f,
                weather_rainmotionblur = false,
                weather_fogintensity = 0f,
                weather_snowintensity = 0f,
                color_selectedlut = LutList.GetLutNameByIndex(ColorCorrectionManager.instance.lastSelection)
            };
        }
    }
}