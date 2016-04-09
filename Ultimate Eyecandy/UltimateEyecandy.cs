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
                Debug.Log($"Ultimate Eyecandy: configuration {UltimateEyeCandy.config.version} loaded");
                
                //UICheckBox checkBox;
                //UIHelperBase group = helper.AddGroup(Name);

                //checkBox = (UICheckBox)group.AddCheckbox("Hide the user interface", AdvancedVehicleOptions.config.hideGUI, (b) =>
                //{
                //    if (AdvancedVehicleOptions.config.hideGUI != b)
                //    {
                //        AdvancedVehicleOptions.hideGUI = b;
                //        AdvancedVehicleOptions.SaveConfig();
                //    }
                //});
                //checkBox.tooltip = "Hide the UI completely if you feel like you are done with it\nand want to save the little bit of memory it takes\nEverything else will still be functional";

                //checkBox = (UICheckBox)group.AddCheckbox("Disable warning at map loading", !AdvancedVehicleOptions.config.onLoadCheck, (b) =>
                //{
                //    if (AdvancedVehicleOptions.config.onLoadCheck == b)
                //    {
                //        AdvancedVehicleOptions.config.onLoadCheck = !b;
                //        AdvancedVehicleOptions.SaveConfig();
                //    }
                //});
                //checkBox.tooltip = "Disable service vehicle availability check at the loading of a map";
            }
            catch (Exception e)
            {
                DebugUtils.Log("OnSettingsUI failed");
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

        public static Configuration config = new Configuration();
        public static bool isGameLoaded = false;

        #region LoadingExtensionBase overrides
        public override void OnCreated(ILoading loading)
        {
            //try
            //{
            //    // Storing default values ASAP (before any mods have the time to change values)
            //    DefaultOptions.StoreAll();

            //    // Creating a backup
            //    SaveBackup();
            //}
            //catch (Exception e)
            //{
            //    DebugUtils.LogException(e);
            //}
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

                isGameLoaded = true;

                // Creating GUI
                UIView view = UIView.GetAView();
                _gameObject = new GameObject("UltimateEyecandy");
                _gameObject.transform.SetParent(view.transform);

                try
                {
                    //VehicleOptions.Clear();
                    _mainPanel = _gameObject.AddComponent<MainPanel>();
                    DebugUtils.Log("UIMainPanel created");
                }
                catch
                {
                    DebugUtils.Log("Could not create UIMainPanel");

                    if (_gameObject != null)
                        GameObject.Destroy(_gameObject);

                    return;
                }

                //new EnumerableActionThread(BrokenAssetsFix);
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
                //SaveConfig();

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


        //  Config-related (keep here?):
        //private Configuration config;

        //private Configuration Configuration
        //{
        //    get
        //    {
        //        if (config == null)
        //        {
        //            try
        //            {
        //                config = Configuration.Deserialize(FileName);
        //                Debug.Log("Ultimate Eyecandy: Preset loaded.");

        //                if (config == null)
        //                {
        //                    config = new Configuration();
        //                    SaveConfig();
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.Log($"Retrieving Ultimate Eyecandy config failed: {e}");
        //            }
        //        }

        //        return config;
        //    }
        //}

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

            // Store modded values
            //DefaultOptions.StoreAllModded();

            if (!File.Exists(FileName))
            {
                DebugUtils.Log("Configuration file not found. Creating new configuration file.");

                config = new Configuration();
                SaveConfig();
                return;
            }

            config = Configuration.Deserialize(FileName);
        }

        public static void SaveConfig()
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(Configuration));
                using (var streamWriter = new StreamWriter(FileName))
                {
                    config.version = config.version + 1;
                    //  Todo: read from mod config:
                    config.outputDebug = true;
                    config.applyAtStart = false;

                    xmlSerializer.Serialize(streamWriter, config);
                    PresetsPanel.instance.PopulateList();
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Unexpected {0} while saving options: {1}\n{2}\n\nInnerException:\n{3}",
                    e.GetType().Name, e.Message, e.StackTrace, e.InnerException.Message);
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
            if (GetPresetByName(presetName) != null) return;

            var newPreset = new Configuration.Preset()
            {
                name = presetName,
                ambient_height = AmbientPanel.instance.ambientSlider.value,
                ambient_rotation = AmbientPanel.instance.rotationSlider.value,
                ambient_intensity = AmbientPanel.instance.intensitySlider.value,
                ambient_ambient = AmbientPanel.instance.ambientSlider.value,
                weather = WeatherPanel.instance.enableWeatherCheckbox.isChecked,
                weather_rainintensity = WeatherPanel.instance.rainintensitySlider.value,
                weather_rainmotionblur = WeatherPanel.instance.rainMotionblurCheckbox,
                weather_fogintensity = WeatherPanel.instance.fogIntensitySlider.value,
                color_selectedlut = ColorManagamentPanel.instance.lutDropdown.selectedIndex
            };
            config.presets.Add(newPreset);
            //  
            SaveConfig();
        }

        public static void DeletePreset(Configuration.Preset preset)
        {
            config.presets.Remove(preset);
            //  
            SaveConfig();
        }

        //  From FastList
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
                    catch (Exception e)
                    {
                        Debug.LogErrorFormat("Ultimate Eyecandy: unexpected {0} while resetting all: {1}\n{2}\n\nInnerException:\n{3}",
                            e.GetType().Name, e.Message, e.StackTrace, e.InnerException.Message);
                    }
                }
                //  
                var selectedPreset = GetPresetByName(presetName);
                AmbientPanel.instance.heightSlider.value = (float)selectedPreset.ambient_height;
                AmbientPanel.instance.rotationSlider.value = (float)selectedPreset.ambient_rotation;
                AmbientPanel.instance.intensitySlider.value = (float)selectedPreset.ambient_intensity;
                AmbientPanel.instance.ambientSlider.value = (float)selectedPreset.ambient_ambient;
                WeatherPanel.instance.enableWeatherCheckbox.isChecked = (bool)selectedPreset.weather;
                WeatherPanel.instance.rainintensitySlider.value = (float)selectedPreset.weather_rainintensity;
                WeatherPanel.instance.rainMotionblurCheckbox.isChecked = (bool)selectedPreset.weather_rainmotionblur;
                WeatherPanel.instance.fogIntensitySlider.value = (float)selectedPreset.weather_fogintensity;
                ColorManagamentPanel.instance.lutDropdown.selectedIndex = (int)selectedPreset.color_selectedlut;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Ultimate Eyecandy: unexpected {0} while applying preset: {1}\n{2}\n\nInnerException:\n{3}",
                    e.GetType().Name, e.Message, e.StackTrace, e.InnerException.Message);
            }
        }

        public static void ResetAll()
        {
            AmbientPanel.instance.heightSlider.value = 0;
            AmbientPanel.instance.rotationSlider.value = 0;
            AmbientPanel.instance.intensitySlider.value = 5f;
            AmbientPanel.instance.ambientSlider.value = 1f;
            WeatherPanel.instance.enableWeatherCheckbox.isChecked = false;
            WeatherPanel.instance.rainintensitySlider.value = 0f;
            WeatherPanel.instance.rainMotionblurCheckbox.isChecked = false;
            WeatherPanel.instance.fogIntensitySlider.value = 0f;
            //  
            ColorManagamentPanel.instance.lutDropdown.selectedIndex = 0;
            ColorCorrectionManager.instance.currentSelection = 0;
        }
    }
}