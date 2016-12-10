using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UltimateEyecandy.GUI;

using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace UltimateEyecandy
{
    public class UltimateEyecandyTool : MonoBehaviour
    {
        public static UltimateEyecandyTool instance;
        private static UIMainButton m_mainbutton;
        private static UIMainPanel m_mainpanel;

        public static OptionsGameplayPanel optionsGameplayPanel;

        //  Size Constants:
        public static float WIDTH = 270;
        public static float HEIGHT = 350;
        public static float SPACING = 5;
        public static float TITLE_HEIGHT = 36;
        public static float TABS_HEIGHT = 28;

        public static string FileName;
        private const string FileNameOnline = "CSL_UltimateEyecandy.xml";
        private const string FileNameLocal = "CSL_UltimateEyecandy_local.xml";
        
        public static Configuration config;
        //  In-Memory Preset for storing initial settings:
        public static Configuration.Preset initialSettings;
        //  In-Memory Preset for storing current settings:
        public static Configuration.Preset currentSettings;
        
        public static bool isEditor;
        public static bool isGameLoaded;
        public static bool isWinterMap;

        public static void Reset()
        {
            var go = FindObjectOfType<UltimateEyecandyTool>();
            if (go != null)
            {
                Destroy(go);
            }

            config = null; // do??
            initialSettings = null;
            currentSettings = null;

            optionsGameplayPanel = null;

            isEditor = false;
            isGameLoaded = false;
            isWinterMap = false;
        }

        public static void Initialize(LoadMode mode)
        {
            // Check if in-game or in Asset Editor (else abort):
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.LoadAsset && mode != LoadMode.NewAsset)
            {
                return;
            }
            //  
            var go = new GameObject("UltimateEyecandyTool");
            try
            {
                FileName = (PluginManager.noWorkshop) ? FileNameLocal : FileNameOnline;
                DebugUtils.Log($"Currently used config File: {FileName}.");
                //  
                go.AddComponent<UltimateEyecandyTool>();
                //  
                isEditor = (mode == LoadMode.LoadAsset || mode == LoadMode.NewAsset) ? true : false;
                isGameLoaded = true;
                isWinterMap = LoadingManager.instance.m_loadedEnvironment.ToLower() == "winter";
                optionsGameplayPanel = Instantiate(FindObjectOfType<OptionsGameplayPanel>().gameObject).GetComponent<OptionsGameplayPanel>();
                //  Init. GUI Components:
                m_mainbutton = UIView.GetAView().AddUIComponent(typeof(UIMainButton)) as UIMainButton;
                DebugUtils.Log("MainButton created.");

                m_mainpanel = UIView.GetAView().AddUIComponent(typeof(UIMainPanel)) as UIMainPanel;
                DebugUtils.Log("MainPanel created.");
            }
            catch (Exception e)
            {
                if (go != null)
                {
                    Destroy(go);
                }
                DebugUtils.LogException(e);
            }
        }

        //  Config
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

        public static void LoadConfig()
        {
            if (!isGameLoaded)
            {
                var fileName = (PluginManager.noWorkshop) ? FileNameLocal : FileNameOnline;
                if (File.Exists(fileName))
                {
                    config = Configuration.Load(fileName);
                    if (config.outputDebug)
                    {
                        DebugUtils.Log($"OnSettingsUI: configuration loaded (file name: {fileName}).");
                    }
                }
                return;
            }
            //  Load config:
            if (!File.Exists(FileName))
            {
                //  No config:
                if (config.outputDebug)
                {
                    DebugUtils.Log($"OnLevelLoaded: No configuration found, new configuration file created (file name: {FileName}).");
                }
                //  Create and save new config:
                config = new Configuration();
                SaveConfig(false);
                //  Create temporary preset for current settings:
                ResetAll();
                return;
            }
            //  
            config = Configuration.Load(FileName);
            if (config.outputDebug)
            {
                DebugUtils.Log($"OnLevelLoaded: Configuration loaded (file name: {FileName}).");
            }
            ResetAll();
        }

        public static void SaveConfig(bool reloadUI = true)
        {
            Configuration.Save(config, reloadUI);
        }

        //  Presets
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
                //  
                existingPreset.name = presetName;
                existingPreset.ambient_height = currentSettings.ambient_height;
                existingPreset.ambient_rotation = currentSettings.ambient_rotation;
                existingPreset.ambient_intensity = currentSettings.ambient_intensity;
                existingPreset.ambient_ambient = currentSettings.ambient_ambient;
                //existingPreset.ambient_fov = currentSettings.ambient_fov;
                existingPreset.weather = currentSettings.weather;
                existingPreset.weather_rainintensity = currentSettings.weather_rainintensity;
                existingPreset.weather_rainmotionblur = currentSettings.weather_rainmotionblur;
                existingPreset.weather_fogintensity = currentSettings.weather_fogintensity;
                existingPreset.weather_snowintensity = currentSettings.weather_snowintensity;
                existingPreset.color_selectedlut = currentSettings.color_selectedlut;
                existingPreset.color_lut = currentSettings.color_lut;
                existingPreset.color_tonemapping = currentSettings.color_tonemapping;
                existingPreset.color_bloom = currentSettings.color_bloom;
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
                    color_selectedlut = currentSettings.color_selectedlut,
                    color_lut = currentSettings.color_lut,
                    color_tonemapping = currentSettings.color_tonemapping,
                    color_bloom = currentSettings.color_bloom
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

        public static void LoadPreset(string presetName, bool saveConfig)
        {
            //  
            try
            {
                var selectedPreset = GetPresetByName(presetName);
                //  Apply values to UI Elements:
                //  Ambient values:
                UIMainPanel.instance.ambientPanel.heightSlider.value = selectedPreset.ambient_height;
                UIMainPanel.instance.ambientPanel.rotationSlider.value = selectedPreset.ambient_rotation;
                UIMainPanel.instance.ambientPanel.intensitySlider.value = selectedPreset.ambient_intensity;
                UIMainPanel.instance.ambientPanel.ambientSlider.value = selectedPreset.ambient_ambient;
                //  Weather values:
                UIMainPanel.instance.weatherPanel.enableWeatherCheckbox.isChecked = selectedPreset.weather;
                optionsGameplayPanel.enableWeather = selectedPreset.weather;

                if (isWinterMap)
                {
                    UIMainPanel.instance.weatherPanel.precipitationSlider.value = selectedPreset.weather_snowintensity;
                }
                else
                {
                    UIMainPanel.instance.weatherPanel.precipitationSlider.value = selectedPreset.weather_rainintensity;
                    UIMainPanel.instance.weatherPanel.rainMotionblurCheckbox.isChecked = selectedPreset.weather_rainmotionblur;
                }
                UIMainPanel.instance.weatherPanel.fogIntensitySlider.value = selectedPreset.weather_fogintensity;
                //  ColorManagement values:
                LutList.Lut activeLut = LutList.GetLut(selectedPreset.color_selectedlut);
                if (activeLut == null)
                {
                    if (config.outputDebug)
                    {
                        DebugUtils.Log($"Load preset: lut {activeLut} not found, applying default Lut for current biome ({LoadingManager.instance.m_loadedEnvironment}).");
                    }
                    UIMainPanel.instance.colorManagementPanel.lutFastlist.DisplayAt(0);
                    UIMainPanel.instance.colorManagementPanel.lutFastlist.selectedIndex = 0;
                    ColorCorrectionManager.instance.currentSelection = 0;
                }
                else
                {
                    UIMainPanel.instance.colorManagementPanel.lutFastlist.DisplayAt(activeLut.index);
                    UIMainPanel.instance.colorManagementPanel.lutFastlist.selectedIndex = activeLut.index;
                    ColorCorrectionManager.instance.currentSelection = activeLut.index;
                }
                UIMainPanel.instance.colorManagementPanel.enableLutCheckbox.isChecked = selectedPreset.color_lut;
                UIMainPanel.instance.colorManagementPanel.enableTonemappingCheckbox.isChecked = selectedPreset.color_tonemapping;
                UIMainPanel.instance.colorManagementPanel.enableBloomCheckbox.isChecked = selectedPreset.color_bloom;
                //  
                if (saveConfig)
                {
                    config.lastPreset = presetName;
                    SaveConfig();
                }
                //  
                currentSettings = selectedPreset;
            }
            catch (Exception e)
            {
                DebugUtils.LogException(e);
            }
        }

        public static void DeletePreset(Configuration.Preset preset)
        {
            config.presets.Remove(preset);
            //  
            SaveConfig();
        }

        //  Reset all values and (re-)apply initial settings to In-Memory Preset for storing current settings:
        public static void ResetAll(bool applyToGUI = false)
        {
            currentSettings = initialSettings;
            if (applyToGUI)
            {
                //  Apply to GUI:
                UIMainPanel.instance.ambientPanel.heightSlider.value = currentSettings.ambient_height;
                UIMainPanel.instance.ambientPanel.rotationSlider.value = currentSettings.ambient_rotation;
                UIMainPanel.instance.ambientPanel.intensitySlider.value = currentSettings.ambient_intensity;
                UIMainPanel.instance.ambientPanel.ambientSlider.value = currentSettings.ambient_ambient;
                //ModMainPanel.instance.ambientPanel.fovSlider.value = currentSettings.ambient_fov;
                if (isWinterMap)
                {
                    UIMainPanel.instance.weatherPanel.precipitationSlider.value = currentSettings.weather_snowintensity;
                }
                else
                {
                    UIMainPanel.instance.weatherPanel.precipitationSlider.value = currentSettings.weather_rainintensity;
                    UIMainPanel.instance.weatherPanel.rainMotionblurCheckbox.isChecked = currentSettings.weather_rainmotionblur;
                }
                UIMainPanel.instance.weatherPanel.fogIntensitySlider.value = currentSettings.weather_fogintensity;
                UIMainPanel.instance.weatherPanel.enableWeatherCheckbox.isChecked = currentSettings.weather;
                UIMainPanel.instance.colorManagementPanel.lutFastlist.DisplayAt(0);
                UIMainPanel.instance.colorManagementPanel.lutFastlist.selectedIndex = 0;
                ColorCorrectionManager.instance.currentSelection = 0;
                UIMainPanel.instance.colorManagementPanel.enableLutCheckbox.isChecked = true;
                UIMainPanel.instance.colorManagementPanel.enableTonemappingCheckbox.isChecked = true;
                UIMainPanel.instance.colorManagementPanel.enableBloomCheckbox.isChecked = true;
            }
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
                //ambient_fov = Camera.main.fieldOfView,
                weather = false, //optionsGameplayPanel.enableWeather,
                weather_rainintensity = 0f,
                weather_rainmotionblur = false,
                weather_fogintensity = 0f,
                weather_snowintensity = 0f,
                color_selectedlut = LutList.GetLutNameByIndex(ColorCorrectionManager.instance.lastSelection),
                color_tonemapping = true,
                color_bloom = true
            };
        }
    }
}