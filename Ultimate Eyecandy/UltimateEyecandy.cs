using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UltimateEyecandy.GUI;
using UnityEngine;

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
                return "All visual settings combined in one ultimate mod!";
            }
        }

        //  Mod options:
        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {
                UltimateEyeCandy.LoadConfig();
                Debug.Log($"OnSettingsUI: configuration {UltimateEyeCandy.config.version} loaded");
                
                //UICheckBox checkBox;
                UIHelperBase group = helper.AddGroup(Name);
                group.AddSpace(10);
                //  
                UICheckBox debugCheckBox = (UICheckBox)group.AddCheckbox("Output to debug log. ", UltimateEyeCandy.config.outputDebug,
                    b =>
                    {
                        if (UltimateEyeCandy.config.outputDebug != b)
                        {
                            UltimateEyeCandy.config.outputDebug = b;
                            UltimateEyeCandy.SaveConfig(false);
                        }
                    });
                debugCheckBox.tooltip = "Output messages to debug log. This may be useful when experiencing issues with this mod.";
                group.AddSpace(20);
                //  
                UICheckBox advancedCheckBox = (UICheckBox)group.AddCheckbox("Enable advanced mod settings (not yet implemented).", UltimateEyeCandy.config.enableAdvanced,
                    b =>
                    {
                        if (UltimateEyeCandy.config.enableAdvanced != b)
                        {
                            UltimateEyeCandy.config.enableAdvanced = b;
                            UltimateEyeCandy.SaveConfig(false);
                        }
                    });
                advancedCheckBox.tooltip = "Enable advanced mod settings (not yet implemented).";
                group.AddSpace(10);
                group.AddGroup("WARNING: playing with the advanced settings may result in unexpected behavior of\nthe game's simulation, so it is strongly recommended to only use these settings on a\nbacked up save game and to NOT save the game afterwards.\nTL;DR: use these settings at your own risk, you have been warned!");
            }
            catch (Exception e)
            {
                //DebugUtils.Log("OnSettingsUI failed");
                DebugUtils.LogException(e);
            }
        }

        public const string version = "1.0.0";
    }

    public class UltimateEyeCandy : LoadingExtensionBase
    {
        private static GameObject _gameObject;
        private static MainPanel _mainPanel;

        private const string FileName = "CSL_UltimateEyecandy.xml";

        public static bool isWinterMap = false;

        public static Configuration config = new Configuration();
        public static Configuration.Preset currentSettings = new Configuration.Preset();

        public static bool isGameLoaded = false;

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

        /// <summary>
        /// Called when the level (game, map editor, asset editor) is loaded
        /// </summary>
        public override void OnLevelLoaded(LoadMode mode)
        {
            try
            {
                // Is it an actual game ?
                if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                {
                    //DefaultOptions.Clear();
                    return;
                }
                //  
                isGameLoaded = true;
                // Creating GUI
                UIView view = UIView.GetAView();
                _gameObject = new GameObject("UltimateEyecandy");
                _gameObject.transform.SetParent(view.transform);

                try
                {
                    isWinterMap = LoadingManager.instance.m_loadedEnvironment.ToLower() == "winter";
                    _mainPanel = _gameObject.AddComponent<MainPanel>();
                    _mainPanel.AddGuiToggle();
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
                    GameObject.Destroy(_gameObject);
                DebugUtils.LogException(e);
            }
        }

        /// <summary>
        /// Called when the level is unloaded
        /// </summary>
        public override void OnLevelUnloading()
        {
            try
            {
                //  Delete current settings:
                currentSettings = null;
                //  
                GUI.UIUtils.DestroyDeeply(_mainPanel);
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

        public static void RestoreBackup()
        {
            if (!File.Exists(FileName + ".bak")) return;

            File.Copy(FileName + ".bak", FileName, true);
            //  
            if (config.outputDebug)
            {
                DebugUtils.Log("Backup configuration file restored.");
            }
        }

        public static void SaveBackup()
        {
            if (!File.Exists(FileName)) return;

            File.Copy(FileName, FileName + ".bak", true);
            //  
            if (config.outputDebug)
            {
                DebugUtils.Log("Backup configuration file created.");
            }
        }

        public static void LoadConfig()
        {
            if (!isGameLoaded)
            {
                if (File.Exists(FileName))
                {
                    config = Configuration.Deserialize(FileName);
                }
                return;
            }
            //  Load config:
            if (!File.Exists(FileName))
            {
                //  No config:
                if (config.outputDebug)
                {
                    DebugUtils.Log("Configuration file not found. Creating new configuration file.");
                }
                //  Create and save new config:
                config = new Configuration();
                SaveConfig();
                //  Create temporary preset for current settings:
                CreateTemporaryPreset();
                return;
            }
            //  
            config = Configuration.Deserialize(FileName);
            //  Create temporary preset for current settings:
            CreateTemporaryPreset();
        }

        public static void SaveConfig(bool reloadUI = true)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(Configuration));
                using (var streamWriter = new StreamWriter(FileName))
                {
                    config.version = ModInfo.version;
                    config.config_version = config.config_version + 1;
                    //  
                    xmlSerializer.Serialize(streamWriter, config);
                    if (reloadUI)
                    {
                        PresetsPanel.instance.PopulateList();
                    }
                    //  
                    if (config.outputDebug)
                    {
                        DebugUtils.Log("Configuration saved.");
                    }
                }
            }
            catch (Exception e)
            {
                DebugUtils.LogException(e);
            }
        }

        public static List<Configuration.Preset> GetAllPresets()
        {
            return config.presets;
        }
        
        public static Configuration.Preset GetPresetByName(string presetName)
        {
            return config.presets.FirstOrDefault(preset => preset.name == presetName);
        }

        public static void CreatePreset(string presetName)
        {
            Configuration.Preset existingPreset = GetPresetByName(presetName);
            //  Overwrite existing preset:
            if (existingPreset != null)
            {
                existingPreset = currentSettings;
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
            SaveConfig();
        }

        public static void CreateTemporaryPreset()
        {
            currentSettings = new Configuration.Preset() {
                name = "TEMP",
                ambient_height = 0f,
                ambient_rotation = 0f,
                ambient_intensity = 5f,
                ambient_ambient = 1f,
                weather = false,
                weather_rainintensity = 0f,
                weather_rainmotionblur = false,
                weather_fogintensity = 0f,
                weather_snowintensity = 0f,
                color_selectedlut = "None"
            };
            //  
            if (config.outputDebug)
            {
                DebugUtils.Log("Temporary preset created.");
            }
        }

        public static void DeletePreset(Configuration.Preset preset)
        {
            config.presets.Remove(preset);
            //  
            SaveConfig();
        }
        
        public static void LoadPreset(string presetName)
        {
            //  
            try
            {
                if (presetName == "Default settings")
                {
                    try
                    {
                        ResetAll();
                        return;
                    }
                    catch (Exception ex)
                    {
                        DebugUtils.LogException(ex);
                    }
                }
                //  
                var selectedPreset = GetPresetByName(presetName);
                currentSettings = selectedPreset;
                //  Apply values to UI Elements:
                AmbientPanel.instance.heightSlider.value = (float)selectedPreset.ambient_height;
                AmbientPanel.instance.rotationSlider.value = (float)selectedPreset.ambient_rotation;
                AmbientPanel.instance.intensitySlider.value = (float)selectedPreset.ambient_intensity;
                AmbientPanel.instance.ambientSlider.value = (float)selectedPreset.ambient_ambient;
                WeatherPanel.instance.enableWeatherCheckbox.isChecked = (bool)selectedPreset.weather;
                WeatherPanel.instance.rainintensitySlider.value = (float)selectedPreset.weather_rainintensity;
                WeatherPanel.instance.rainMotionblurCheckbox.isChecked = (bool)selectedPreset.weather_rainmotionblur;
                WeatherPanel.instance.fogIntensitySlider.value = (float)selectedPreset.weather_fogintensity;
                WeatherPanel.instance.snowIntensitySlider.value = (float)selectedPreset.weather_snowintensity;
                LutList.Lut activeLut = LutList.GetLut(selectedPreset.color_selectedlut);
                try
                {
                    ColorManagamentPanel.instance._lutFastlist.DisplayAt(activeLut.index);
                    ColorCorrectionManager.instance.currentSelection = activeLut.index;
                }
                catch (Exception ex)
                {
                    if (config.outputDebug)
                    {
                        DebugUtils.Log($"Load preset: lut {activeLut} not found, resetting to default.");
                    }
                    DebugUtils.LogException(ex);
                    ColorManagamentPanel.instance._lutFastlist.DisplayAt(0);
                    ColorCorrectionManager.instance.currentSelection = 0;
                }
            }
            catch (Exception ex)
            {
                DebugUtils.LogException(ex);
            }
        }

        public static void ResetAll()
        {
            CreateTemporaryPreset();
            //  
            AmbientPanel.instance.heightSlider.value = 0;
            AmbientPanel.instance.rotationSlider.value = 0;
            AmbientPanel.instance.intensitySlider.value = 5f;
            AmbientPanel.instance.ambientSlider.value = 1f;
            WeatherPanel.instance.enableWeatherCheckbox.isChecked = false;
            WeatherPanel.instance.rainintensitySlider.value = 0f;
            WeatherPanel.instance.rainMotionblurCheckbox.isChecked = false;
            WeatherPanel.instance.fogIntensitySlider.value = 0f;
            WeatherPanel.instance.snowIntensitySlider.value = 0f;
            ColorManagamentPanel.instance._lutFastlist.DisplayAt(0);
            ColorCorrectionManager.instance.currentSelection = 0;
        }
    }
}