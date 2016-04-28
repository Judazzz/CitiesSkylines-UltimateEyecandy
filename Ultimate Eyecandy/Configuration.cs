using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
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

            [XmlElement("weather")]
            public bool weather;

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

            public Preset(string name)
            {
                this.name = name;
            }

            public Preset(Preset builtInPreset)
            {
                this.name = builtInPreset.name;
                this.ambient_height = builtInPreset.ambient_height;
                this.ambient_rotation = builtInPreset.ambient_rotation;
                this.ambient_intensity = builtInPreset.ambient_intensity;
                this.ambient_ambient = builtInPreset.ambient_height;
                this.weather = builtInPreset.weather;
                this.weather_rainintensity = builtInPreset.weather_rainintensity;
                this.weather_rainmotionblur = builtInPreset.weather_rainmotionblur;
                this.weather_fogintensity = builtInPreset.weather_fogintensity;
                this.color_selectedlut = builtInPreset.color_selectedlut;
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

        public static void Serialize(string filename, Configuration configuration)
        {
            var xmlSerializer = new XmlSerializer(typeof(Configuration));
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(filename))
                {
                    var presetsCopy = new Configuration();
                    presetsCopy.version = configuration.version;
                    presetsCopy.outputDebug = configuration.outputDebug;
                    presetsCopy.enableAdvanced = configuration.enableAdvanced;
                    presetsCopy.loadLastPresetOnStart = configuration.loadLastPresetOnStart;
                    presetsCopy.lastPreset = configuration.lastPreset;

                    foreach (var preset in configuration.presets)
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
                            ambient_ambient = preset.ambient_height,
                            weather = preset.weather,
                            weather_rainintensity = preset.weather_rainintensity,
                            weather_rainmotionblur = preset.weather_rainmotionblur,
                            weather_fogintensity = preset.weather_fogintensity,
                            weather_snowintensity = preset.weather_snowintensity,
                            color_selectedlut = preset.color_selectedlut
                        };
                        presetsCopy.presets.Add(newPreset);
                    }

                    xmlSerializer.Serialize(streamWriter, presetsCopy);
                }
            }
            catch (Exception e)
            {
                DebugUtils.LogException(e);
                throw e;
            }
        }
    }
}