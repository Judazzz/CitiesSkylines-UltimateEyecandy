using ColossalFramework.UI;
using ColossalFramework.Plugins;
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
                UICheckBox debugCheckBox = (UICheckBox)group.AddCheckbox("Write data to debug log (it's going to be a lot!)", UltimateEyeCandy.config.outputDebug,
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
                UICheckBox loadLastPresetOnStartCheckBox = (UICheckBox)group.AddCheckbox("Load last active preset on start.", UltimateEyeCandy.config.loadLastPresetOnStart,
                    b =>
                    {
                        if (UltimateEyeCandy.config.loadLastPresetOnStart != b)
                        {
                            UltimateEyeCandy.config.loadLastPresetOnStart = b;
                            UltimateEyeCandy.SaveConfig(false);
                        }
                    });
                loadLastPresetOnStartCheckBox.tooltip = "Load last active preset on start.";
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
        private const string FileNameLocal = "CSL_UltimateEyecandy_local.xml";

        public static bool isWinterMap = false;

        public static Configuration config = new Configuration();
        public static Configuration.Preset currentSettings = new Configuration.Preset();

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
                    optionsGameplayPanel = UnityEngine.Object.Instantiate<GameObject>(UnityEngine.Object.FindObjectOfType<OptionsGameplayPanel>().gameObject).GetComponent<OptionsGameplayPanel>();
                    //  
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
            var fileName = (PluginManager.noWorkshop) ? FileNameLocal : FileName;
            if (!File.Exists(fileName + ".bak")) return;

            File.Copy(fileName + ".bak", fileName, true);
            //  
            if (config.outputDebug)
            {
                DebugUtils.Log("Backup configuration file restored.");
            }
        }

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
                CreateTemporaryPreset();
                return;
            }
            //  
            config = Configuration.Deserialize(fileName);
        }

        public static void SaveConfig(bool reloadUI = true)
        {
            var fileName = (PluginManager.noWorkshop) ? FileNameLocal : FileName;
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(Configuration));
                using (var streamWriter = new StreamWriter(fileName))
                {
                    config.version = ModInfo.version;
                    //  
                    xmlSerializer.Serialize(streamWriter, config);
                    if (reloadUI)
                    {
                        PresetsPanel.instance.PopulatePresetsFastList();
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

        //  In-Memory Preset solely for handling current settings:
        public static void CreateTemporaryPreset()
        {
            currentSettings = new Configuration.Preset()
            {
                name = "TEMP",
                ambient_height = 0f,
                ambient_rotation = 0f,
                ambient_intensity = 5f,
                ambient_ambient = 1f,
                weather = false,
                weather_rainintensity = 0f,
                weather_rainmotionblur = optionsGameplayPanel.enableWeather,
                weather_fogintensity = 0f,
                weather_snowintensity = 0f,
                color_selectedlut = LutList.GetLutNameByIndex(ColorCorrectionManager.instance.lastSelection) //"None"
            };
            //  
            if (config.outputDebug)
            {
                DebugUtils.Log("Temporary preset created from scratch.");
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
                var selectedPreset = GetPresetByName(presetName);
                currentSettings = selectedPreset;
                //  Apply values to UI Elements:
                //  Ambient values:
                AmbientPanel.instance.heightSlider.value = (float)currentSettings.ambient_height;
                AmbientPanel.instance.rotationSlider.value = (float)currentSettings.ambient_rotation;
                AmbientPanel.instance.intensitySlider.value = (float)currentSettings.ambient_intensity;
                AmbientPanel.instance.ambientSlider.value = (float)currentSettings.ambient_ambient;
                //  Weather vales:
                WeatherPanel.instance.enableWeatherCheckbox.isChecked = currentSettings.weather;
                if (isWinterMap)
                {
                    WeatherPanel.instance.snowIntensitySlider.value = (float)currentSettings.weather_snowintensity;
                }
                else
                {
                    WeatherPanel.instance.rainintensitySlider.value = (float)currentSettings.weather_rainintensity;
                    WeatherPanel.instance.rainMotionblurCheckbox.isChecked = currentSettings.weather_rainmotionblur;
                }
                WeatherPanel.instance.fogIntensitySlider.value = (float)currentSettings.weather_fogintensity;
                //  ColorManagement values:
                LutList.Lut activeLut = LutList.GetLut(currentSettings.color_selectedlut);
                if (activeLut == null)
                {
                    if (config.outputDebug)
                    {
                        DebugUtils.Log($"Load preset: lut {activeLut} not found, applying default Lut for current biome ({LoadingManager.instance.m_loadedEnvironment}).");
                    }
                    ColorManagamentPanel.instance.lutFastlist.DisplayAt(0);
                    ColorManagamentPanel.instance.lutFastlist.selectedIndex = 0;
                    ColorCorrectionManager.instance.currentSelection = 0;
                }
                else
                {
                    ColorManagamentPanel.instance.lutFastlist.DisplayAt(activeLut.index);
                    ColorManagamentPanel.instance.lutFastlist.selectedIndex = activeLut.index;
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

        public static void ResetAll()
        {
            CreateTemporaryPreset();
            //  
            AmbientPanel.instance.heightSlider.value = (float)currentSettings.ambient_height;
            AmbientPanel.instance.rotationSlider.value = (float)currentSettings.ambient_rotation;
            AmbientPanel.instance.intensitySlider.value = (float)currentSettings.ambient_intensity;
            AmbientPanel.instance.ambientSlider.value = (float)currentSettings.ambient_ambient;
            if (isWinterMap)
            {
                WeatherPanel.instance.snowIntensitySlider.value = (float)currentSettings.weather_snowintensity;
            }
            else
            {
                WeatherPanel.instance.rainintensitySlider.value = (float)currentSettings.weather_rainintensity;
                WeatherPanel.instance.rainMotionblurCheckbox.isChecked = currentSettings.weather_rainmotionblur;
            }   
            WeatherPanel.instance.fogIntensitySlider.value = (float)currentSettings.weather_fogintensity;
            WeatherPanel.instance.enableWeatherCheckbox.isChecked = currentSettings.weather;
            ColorManagamentPanel.instance.lutFastlist.DisplayAt(0);
            ColorManagamentPanel.instance.lutFastlist.selectedIndex = 0;
            ColorCorrectionManager.instance.currentSelection = 0;
        }
    }
}