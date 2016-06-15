using ColossalFramework.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UltimateEyecandy.GUI;
using UnityEngine;

namespace UltimateEyecandy
{
    public class Configuration
    {
        public string version;
        public bool outputDebug;
        public bool enableAdvanced = false;
        public bool loadLastPresetOnStart;
        public string lastPreset;

        [XmlArray(ElementName = "Presets")]
        [XmlArrayItem(ElementName = "Preset")]
        public List<Preset> presets = new List<Preset>();

        public Preset getPreset(string name)
        {
            foreach (Preset preset in presets)
            {
                if (preset.name == name) return preset;
            }
            return null;
        }

        public class Preset
        {
            [XmlAttribute("name")]
            public string name;

            [XmlElement("ambient_height")]
            public float ambient_height;

            [XmlElement("ambient_rotation")]
            public float ambient_rotation;

            [XmlElement("ambient_intensity")]
            public float ambient_intensity;

            [XmlElement("ambient_ambient")]
            public float ambient_ambient;

            //[XmlElement("ambient_fov")]
            //public float ambient_fov;

            [XmlElement("weather")]
            public bool weather = false;

            [XmlElement("weather_rainintensity")]
            public float weather_rainintensity;

            [XmlElement("weather_rainmotionblur")]
            public bool weather_rainmotionblur;

            [XmlElement("weather_fogintensity")]
            public float weather_fogintensity;

            [XmlElement("weather_snowintensity")]
            public float weather_snowintensity;

            [XmlElement("color_selectedlut")]
            public string color_selectedlut;

            [XmlElement("color_lut")]
            public bool color_lut = true;

            [XmlElement("color_tonemapping")]
            public bool color_tonemapping = true;

            [XmlElement("color_bloom")]
            public bool color_bloom = true;

            public Preset(string name)
            {
                this.name = name;
            }

            public Preset(Preset builtInPreset)
            {
                name = builtInPreset.name;
                ambient_height = builtInPreset.ambient_height;
                ambient_rotation = builtInPreset.ambient_rotation;
                ambient_intensity = builtInPreset.ambient_intensity;
                ambient_ambient = builtInPreset.ambient_height;
                weather = builtInPreset.weather;
                weather_rainintensity = builtInPreset.weather_rainintensity;
                weather_rainmotionblur = builtInPreset.weather_rainmotionblur;
                weather_fogintensity = builtInPreset.weather_fogintensity;
                color_selectedlut = builtInPreset.color_selectedlut;
                color_lut = builtInPreset.color_lut;
                color_tonemapping = builtInPreset.color_tonemapping;
                color_bloom = builtInPreset.color_bloom;
            }

            public Preset()
            {
            }
        }

        public static Configuration Deserialize(string filename)
        {
            if (!File.Exists(filename)) return null;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
            try
            {
                using (StreamReader streamReader = new StreamReader(filename))
                {
                    return (Configuration)xmlSerializer.Deserialize(streamReader);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Couldn't load configuration (XML malformed?)");
                throw e;
            }
        }

        public static void Serialize(string filename, Configuration config, bool reloadUI)
        {
            string FileName = "CSL_UltimateEyecandy.xml";
            string FileNameLocal = "CSL_UltimateEyecandy_local.xml";

            var fileName = (PluginManager.noWorkshop) ? FileNameLocal : FileName;
            try
            {
                //  Todo: move serialization code to Serialize method in Config.cs!
                var xmlSerializer = new XmlSerializer(typeof(Configuration));
                using (var streamWriter = new StreamWriter(fileName))
                {
                    UltimateEyecandy.config.version = ModInfo.version;

                    var configCopy = new Configuration();
                    configCopy.version = UltimateEyecandy.config.version;
                    configCopy.outputDebug = UltimateEyecandy.config.outputDebug;
                    configCopy.enableAdvanced = UltimateEyecandy.config.enableAdvanced;
                    configCopy.loadLastPresetOnStart = UltimateEyecandy.config.loadLastPresetOnStart;
                    configCopy.lastPreset = UltimateEyecandy.config.lastPreset;

                    foreach (var preset in UltimateEyecandy.config.presets)
                    {
                        //  Skip Temporary Preset:
                        if (preset.name == string.Empty)
                            continue;
                        //  Existing presets:
                        var newPreset = new Preset
                        {
                            name = preset.name,
                            ambient_height = preset.ambient_height,
                            ambient_rotation = preset.ambient_rotation,
                            ambient_intensity = preset.ambient_intensity,
                            ambient_ambient = preset.ambient_ambient,
                            //ambient_fov = preset.ambient_fov,
                            weather = preset.weather,
                            weather_rainintensity = preset.weather_rainintensity,
                            weather_rainmotionblur = preset.weather_rainmotionblur,
                            weather_fogintensity = preset.weather_fogintensity,
                            weather_snowintensity = preset.weather_snowintensity,
                            color_selectedlut = preset.color_selectedlut,
                            color_lut = preset.color_lut,
                            color_tonemapping = preset.color_tonemapping,
                            color_bloom = preset.color_bloom
                        };
                        configCopy.presets.Add(newPreset);
                    }
                    xmlSerializer.Serialize(streamWriter, configCopy);
                    UltimateEyecandy.config = configCopy;
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
    }
}