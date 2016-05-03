using ColossalFramework.UI;
using UnityEngine;

namespace UltimateEyecandy.GUI
{
    public class WeatherPanel : UIPanel
    {
        private UILabel _enableWeatherLabel;
        private UICheckBox _enableWeatherCheckbox;

        private UILabel _rainMotionblurLabel;
        private UICheckBox _rainMotionblurCheckbox;

        private UILabel _fogIntensityLabel;
        private UISlider _fogIntensitySlider;

        private UILabel _precipitationLabel;
        private UISlider _precipitationSlider;

        private UIButton _resetWeatherButton;

        public UICheckBox enableWeatherCheckbox
        {
            get { return _enableWeatherCheckbox; }
        }
        public UICheckBox rainMotionblurCheckbox
        {
            get { return _rainMotionblurCheckbox; }
        }
        public UISlider fogIntensitySlider
        {
            get { return _fogIntensitySlider; }
        }
        public UISlider precipitationSlider
        {
            get { return _precipitationSlider; }
        }

        private static WeatherPanel _instance;
        public static WeatherPanel instance => _instance;

        public override void Start()
        {
            base.Start();
            _instance = this;
            canFocus = true;
            isInteractive = true;
            //  
            SetupControls();
        }

        private void SetupControls()
        {
            //  Enable weather:
            var topContainer = UIUtils.CreateFormElement(this, "top");
            topContainer.name = "enableWeatherCheckboxContainer";
            topContainer.relativePosition = new Vector3(0, -3);
            //topContainer.autoLayout = false;

            _enableWeatherCheckbox = UIUtils.CreateCheckBox(topContainer);
            _enableWeatherCheckbox.relativePosition = new Vector3(5, 17);
            _enableWeatherCheckbox.name = "enableWeatherCheckbox";
            _enableWeatherCheckbox.tooltip = "Check this box to enable Dynamic Weather. This setting is the same as the Dynamic Weather option in the Gameplay Options panel.";
            _enableWeatherCheckbox.isChecked = WeatherManager.instance.m_enableWeather;
            _enableWeatherCheckbox.eventCheckChanged += CheckboxChanged;

            _enableWeatherCheckbox.label.text = "Enable weather";

            //  Precipitation intensity:
            var precipitationContainer = UIUtils.CreateFormElement(this, "center");
            precipitationContainer.name = "precipitationSliderContainer";
            precipitationContainer.relativePosition = new Vector3(0, 60);
            precipitationContainer.relativePosition = new Vector3(0, 57);

            _precipitationLabel = precipitationContainer.AddUIComponent<UILabel>();
            _precipitationLabel.textScale = 0.8f;
            _precipitationLabel.padding = new RectOffset(0, 0, 0, 5);

            _precipitationSlider = UIUtils.CreateSlider(precipitationContainer, 0, 2.5f);
            _precipitationSlider.name = "precipitationSlider";
            _precipitationSlider.stepSize = 0.05f;
            _precipitationSlider.tooltip = "Move this slider to the right to increase Rain Intensity.\nDynamic Weather will be enabled if necessary.";
            _precipitationSlider.eventValueChanged += SliderChanged;
            //  Winter map?
            if (UltimateEyecandy.isWinterMap)
            {
                //  Snow intensity:
                _precipitationLabel.text = "Snowfall intensity";
                _precipitationSlider.tooltip = "Move this slider to the right to increase Snow Intensity.\nDynamic Weather will be enabled if necessary.";
            }
            //  Non-winter map:
            else
            {
                //  Rain intensity:
                _precipitationLabel.text = "Rain intensity";
                _precipitationSlider.tooltip = "Move this slider to the right to increase Rain Intensity.\nDynamic Weather will be enabled if necessary.";
            }

            //  Precipitation motion blur:
            var rainMotionblurContainer = UIUtils.CreateFormElement(this, "center");
            rainMotionblurContainer.name = "rainMotionblurCheckboxContainer";
            //rainMotionblurContainer.relativePosition = new Vector3(0, 95);
            rainMotionblurContainer.relativePosition = new Vector3(0, 89);
            rainMotionblurContainer.autoLayout = false;

            _rainMotionblurCheckbox = UIUtils.CreateCheckBox(rainMotionblurContainer);
            _rainMotionblurCheckbox.name = "rainMotionblurCheckbox";
            _rainMotionblurCheckbox.tooltip = "Check this box to enable the Rain Motion Blur Effect. This setting is mainly visible when the game is paused.";
            _rainMotionblurCheckbox.eventCheckChanged += CheckboxChanged;
            _rainMotionblurCheckbox.label.text = "Rain motion blur";
            //  Hide on winter maps:
            rainMotionblurContainer.isVisible = (!UltimateEyecandy.isWinterMap);

            //  Fog intensity:
            var fogContainer = UIUtils.CreateFormElement(this, "center");
            fogContainer.name = "fogIntensitySliderContainer";
            //fogContainer.relativePosition = (UltimateEyecandy.isWinterMap) ? new Vector3(0, 110) : new Vector3(0, 156);
            fogContainer.relativePosition = (UltimateEyecandy.isWinterMap) ? new Vector3(0, 104) : new Vector3(0, 150);

            _fogIntensityLabel = fogContainer.AddUIComponent<UILabel>();
            _fogIntensityLabel.text = "Fog intensity";
            _fogIntensityLabel.textScale = 0.8f;
            _fogIntensityLabel.padding = new RectOffset(0, 0, 0, 5);

            _fogIntensitySlider = UIUtils.CreateSlider(fogContainer, 0, 1f);
            _fogIntensitySlider.name = "fogIntensitySlider";
            _fogIntensitySlider.stepSize = 0.02f;
            _fogIntensitySlider.tooltip = "Move this slider to the right to increase Fog Density.\nDynamic Weather will be enabled if necessary.";
            _fogIntensitySlider.eventValueChanged += SliderChanged;

            //  Reset button:
            var resetContainer = UIUtils.CreateFormElement(this, "bottom");

            _resetWeatherButton = UIUtils.CreateButton(resetContainer);
            _resetWeatherButton.name = "resetButton";
            _resetWeatherButton.text = "Reset";
            _resetWeatherButton.tooltip = "Reset all values set in this panel to default values.";
            _resetWeatherButton.eventClicked += (c, e) =>
            {
                if (UltimateEyecandy.config.outputDebug)
                {
                    DebugUtils.Log($"WeatherPanel: 'Reset' clicked.");
                }
                //  
                _enableWeatherCheckbox.isChecked = false;
                _precipitationSlider.value = 0;
                _rainMotionblurCheckbox.isChecked = false;
                _fogIntensitySlider.value = 0;

            };
        }

        private void SliderChanged(UIComponent trigger, float value)
        {
            if (UltimateEyecandy.config.outputDebug)
            {
                DebugUtils.Log($"WeatherPanel: Slider {trigger.name} = {value}");
            }
            //  
            if (trigger == _precipitationSlider)
            {
                WeatherManager.instance.m_currentRain = value;
                if (UltimateEyecandy.isWinterMap)
                {
                    UltimateEyecandy.currentSettings.weather_snowintensity = value;
                }
                else
                {
                    UltimateEyecandy.currentSettings.weather_rainintensity = value;
                }
                //  Enable dynamic weather if disabled (required for rainfall):
                if (!WeatherManager.instance.m_enableWeather)
                {
                    WeatherManager.instance.m_enableWeather = true;
                    UltimateEyecandy.currentSettings.weather = true;
                    _enableWeatherCheckbox.isChecked = true;
                }
            }
            else if (trigger == _fogIntensitySlider)
            {
                WeatherManager.instance.m_currentFog = value;
                UltimateEyecandy.currentSettings.weather_fogintensity = value;
                //  Enable dynamic weather if disabled (required for rainfall):
                if (!WeatherManager.instance.m_enableWeather)
                {
                    WeatherManager.instance.m_enableWeather = true;
                    UltimateEyecandy.currentSettings.weather = true;
                    _enableWeatherCheckbox.isChecked = true;
                }
            }
        }

        private void CheckboxChanged(UIComponent trigger, bool isChecked)
        {
            if (UltimateEyecandy.config.outputDebug)
            {
                DebugUtils.Log($"WeatherPanel: Checkbox {trigger.name} = {isChecked}");
            }
            //  
            if (trigger == _enableWeatherCheckbox)
            {
                WeatherManager.instance.m_enableWeather = isChecked;
                UltimateEyecandy.currentSettings.weather = isChecked;
                //  
                UltimateEyecandy.optionsGameplayPanel.enableWeather = isChecked;
                //  Re-apply current weather settings - To-do: only re-apply when checking box manually (not when triggered by LoadPreset()):
                //if (isChecked)
                //{
                //    if (UltimateEyecandy.isWinterMap)
                //    {
                //        //WeatherManager.instance.m_currentRain = _snowIntensitySlider.value;
                //        UltimateEyecandy.currentSettings.weather_snowintensity = _snowIntensitySlider.value;
                //    }
                //    else
                //    {
                //        WeatherManager.instance.m_currentRain = _rainIntensitySlider.value;
                //        UltimateEyecandy.currentSettings.weather_rainintensity = _rainIntensitySlider.value;
                //        var rrp = FindObjectOfType<RainParticleProperties>();
                //        rrp.ForceRainMotionBlur = _rainMotionblurCheckbox.isChecked;
                //        UltimateEyecandy.currentSettings.weather_rainmotionblur = _rainMotionblurCheckbox.isChecked;
                //    }
                //    WeatherManager.instance.m_currentFog = _fogIntensitySlider.value;
                //    UltimateEyecandy.currentSettings.weather_fogintensity = _fogIntensitySlider.value;
                //}
            }
            else if (trigger == _rainMotionblurCheckbox)
            {
                var rrp = FindObjectOfType<RainParticleProperties>();
                rrp.ForceRainMotionBlur = isChecked;
                UltimateEyecandy.currentSettings.weather_rainmotionblur = isChecked;
            }
        }
    }
}
