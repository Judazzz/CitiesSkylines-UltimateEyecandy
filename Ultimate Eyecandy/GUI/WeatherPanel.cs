using UnityEngine;
using ColossalFramework.UI;
using System.Xml.Serialization;
using System.Text;

namespace UltimateEyecandy.GUI
{
    public class WeatherPanel : UIPanel
    {
        private UILabel _enableWeatherLabel;
        private UICheckBox _enableWeatherCheckbox;

        private UILabel _rainIntensityLabel;
        private UISlider _rainIntensitySlider;
        private UILabel _rainMotionblurLabel;
        private UICheckBox _rainMotionblurCheckbox;

        private UILabel _fogIntensityLabel;
        private UISlider _fogIntensitySlider;

        private UILabel _snowIntensityLabel;
        private UISlider _snowIntensitySlider;

        private UIButton _resetWeatherButton;

        public UICheckBox enableWeatherCheckbox
        {
            get { return _enableWeatherCheckbox; }
        }
        public UISlider rainintensitySlider
        {
            get { return _rainIntensitySlider; }
        }
        public UICheckBox rainMotionblurCheckbox
        {
            get { return _rainMotionblurCheckbox; }
        }
        public UISlider fogIntensitySlider
        {
            get { return _fogIntensitySlider; }
        }
        public UISlider snowIntensitySlider
        {
            get { return _snowIntensitySlider; }
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
            topContainer.autoLayout = false;

            _enableWeatherLabel = topContainer.AddUIComponent<UILabel>();
            _enableWeatherLabel.text = "Enable weather";
            _enableWeatherLabel.tooltip = "Check this box to enable dynamic weather. This setting is the same as the Dynamic Weather option in the Gameplay Options panel.";
            _enableWeatherLabel.textScale = 0.9f;
            _enableWeatherLabel.relativePosition = new Vector3(36, 22);

            _enableWeatherCheckbox = UIUtils.CreateCheckBox(topContainer);
            _enableWeatherCheckbox.name = "enableWeatherCheckbox";
            _enableWeatherCheckbox.relativePosition = new Vector3(10, 21);
            _enableWeatherCheckbox.isChecked = WeatherManager.instance.m_enableWeather;
            _enableWeatherCheckbox.eventCheckChanged += CheckboxChanged;

            //  Weather settings:
            var precipitationContainer = UIUtils.CreateFormElement(this, "center");
            precipitationContainer.relativePosition = new Vector3(0, 68);
            //  Winter map:
            if (UltimateEyeCandy.isWinterMap)
            {
                //  Snow intensity:
                _snowIntensityLabel = precipitationContainer.AddUIComponent<UILabel>();
                _snowIntensityLabel.text = "Snowfall intensity";
                _snowIntensityLabel.textScale = 0.9f;
                _snowIntensityLabel.padding = new RectOffset(0, 0, 0, 5);

                _snowIntensitySlider = UIUtils.CreateSlider(precipitationContainer, 0, 10f);
                _snowIntensitySlider.name = "snowIntensitySlider";
                _rainIntensitySlider.tooltip = "Move this slider to the right to increase snow intensity.\nIf disabled, dynamic Weather will be enabled.";
                _snowIntensitySlider.stepSize = 0.25f;
                _snowIntensitySlider.eventValueChanged += SliderChanged;
            }
            //  Non-winter map:
            else
            {
                //  Rain intensity:
                _rainIntensityLabel = precipitationContainer.AddUIComponent<UILabel>();
                _rainIntensityLabel.text = "Rain intensity";
                _rainIntensityLabel.textScale = 0.9f;
                _rainIntensityLabel.padding = new RectOffset(0, 0, 0, 5);

                _rainIntensitySlider = UIUtils.CreateSlider(precipitationContainer, 0, 10f);
                _rainIntensitySlider.name = "rainIntensitySlider";
                _rainIntensitySlider.stepSize = 0.25f;
                _rainIntensitySlider.tooltip = "Move this slider to the right to increase rain intensity.\nIf disabled, dynamic Weather will be enabled.";
                _rainIntensitySlider.eventValueChanged += SliderChanged;

                //  Rain motion blur:
                var rainMotionblurContainer = UIUtils.CreateFormElement(this, "center");
                rainMotionblurContainer.relativePosition = new Vector3(0, 115);
                rainMotionblurContainer.autoLayout = false;

                _rainMotionblurLabel = rainMotionblurContainer.AddUIComponent<UILabel>();
                _rainMotionblurLabel.text = "Rain motion blur";
                _rainMotionblurLabel.textScale = 0.9f;
                _rainMotionblurLabel.relativePosition = new Vector3(36, 24);

                _rainMotionblurCheckbox = UIUtils.CreateCheckBox(rainMotionblurContainer);
                _rainMotionblurCheckbox.name = "rainMotionblurCheckbox";
                _rainMotionblurCheckbox.tooltip = "Check this box to enable the rain motion blur effect. This setting is mainly visible when the game is paused.";
                _rainMotionblurCheckbox.relativePosition = new Vector3(10, 23);
                _rainMotionblurCheckbox.eventCheckChanged += CheckboxChanged;
            }

            //  Fog intensity:
            var fogContainer = UIUtils.CreateFormElement(this, "center");
            fogContainer.relativePosition = (UltimateEyeCandy.isWinterMap) ? new Vector3(0, 115) : new Vector3(0, 183);

            _fogIntensityLabel = fogContainer.AddUIComponent<UILabel>();
            _fogIntensityLabel.text = "Fog intensity";
            _fogIntensityLabel.textScale = 0.9f;
            _fogIntensityLabel.padding = new RectOffset(0, 0, 0, 5);

            _fogIntensitySlider = UIUtils.CreateSlider(fogContainer, 0, 3.5f);
            _fogIntensitySlider.name = "fogIntensitySlider";
            _fogIntensitySlider.stepSize = 0.25f;
            _rainIntensitySlider.tooltip = "Move this slider to the right to increase fog density.\nIf disabled, dynamic Weather will be enabled.";
            _fogIntensitySlider.eventValueChanged += SliderChanged;

            //  Reset button:
            var resetContainer = UIUtils.CreateFormElement(this, "bottom");

            _resetWeatherButton = UIUtils.CreateButton(resetContainer);
            _resetWeatherButton.name = "resetButton";
            _resetWeatherButton.text = "Reset";
            _resetWeatherButton.tooltip = "Reset all values set in this panel to default values.";
            _resetWeatherButton.eventClicked += (c, e) =>
            {
                if (UltimateEyeCandy.config.outputDebug)
                {
                    DebugUtils.Log($"WeatherPanel: 'Reset' clicked.");
                }
                //  
                _enableWeatherCheckbox.isChecked = false;
                _rainIntensitySlider.value = 0;
                _rainMotionblurCheckbox.isChecked = false;
                _fogIntensitySlider.value = 0;

            };
        }

        private void SliderChanged(UIComponent trigger, float value)
        {
            if (UltimateEyeCandy.config.outputDebug)
            {
                DebugUtils.Log($"WeatherPanel: Slider {trigger.name} = {value}");
            }
            //  
            if (trigger == _rainIntensitySlider)
            {
                WeatherManager.instance.m_currentRain = value;
                UltimateEyeCandy.currentSettings.weather_rainintensity = value;
                //  Enable dynamic weather if disabled (required for rainfall):
                if (!WeatherManager.instance.m_enableWeather)
                {
                    WeatherManager.instance.m_enableWeather = true;
                    UltimateEyeCandy.currentSettings.weather = true;
                    _enableWeatherCheckbox.isChecked = true;
                }
            }
            //  
            else if (trigger == _snowIntensitySlider)
            {
                //WeatherManager.instance.m_currentRain = value;
                UltimateEyeCandy.currentSettings.weather_snowintensity = value;
                //  Enable dynamic weather if disabled (required for rainfall):
                if (!WeatherManager.instance.m_enableWeather)
                {
                    WeatherManager.instance.m_enableWeather = true;
                    UltimateEyeCandy.currentSettings.weather = true;
                    _enableWeatherCheckbox.isChecked = true;
                }
            }
            else if (trigger == _fogIntensitySlider)
            {
                WeatherManager.instance.m_currentFog = value;
                UltimateEyeCandy.currentSettings.weather_fogintensity = value;
                //  Enable dynamic weather if disabled (required for rainfall):
                if (!WeatherManager.instance.m_enableWeather)
                {
                    WeatherManager.instance.m_enableWeather = true;
                    UltimateEyeCandy.currentSettings.weather = true;
                    _enableWeatherCheckbox.isChecked = true;
                }
            }
        }

        private void CheckboxChanged(UIComponent trigger, bool isChecked)
        {
            if (UltimateEyeCandy.config.outputDebug)
            {
                DebugUtils.Log($"WeatherPanel: Checkbox {trigger.name} = {isChecked}");
            }
            //  
            if (trigger == _enableWeatherCheckbox)
            {
                WeatherManager.instance.m_enableWeather = isChecked;
                UltimateEyeCandy.currentSettings.weather = isChecked;
                //  
                //var optionsGameplayPanel = FindObjectOfType<OptionsGameplayPanel>();
                UltimateEyeCandy.optionsGameplayPanel.enableWeather = isChecked;
                //  Re-apply current weather settings - To-do: only re-apply when checking box manually (not when triggered by LoadPreset()):
                //if (isChecked)
                //{
                //    if (UltimateEyeCandy.isWinterMap)
                //    {
                //        //WeatherManager.instance.m_currentRain = _snowIntensitySlider.value;
                //        UltimateEyeCandy.currentSettings.weather_snowintensity = _snowIntensitySlider.value;
                //    }
                //    else
                //    {
                //        WeatherManager.instance.m_currentRain = _rainIntensitySlider.value;
                //        UltimateEyeCandy.currentSettings.weather_rainintensity = _rainIntensitySlider.value;
                //        var rrp = FindObjectOfType<RainParticleProperties>();
                //        rrp.ForceRainMotionBlur = _rainMotionblurCheckbox.isChecked;
                //        UltimateEyeCandy.currentSettings.weather_rainmotionblur = _rainMotionblurCheckbox.isChecked;
                //    }
                //    WeatherManager.instance.m_currentFog = _fogIntensitySlider.value;
                //    UltimateEyeCandy.currentSettings.weather_fogintensity = _fogIntensitySlider.value;
                //}
            }
            else if (trigger == _rainMotionblurCheckbox)
            {
                var rrp = FindObjectOfType<RainParticleProperties>();
                rrp.ForceRainMotionBlur = isChecked;
                UltimateEyeCandy.currentSettings.weather_rainmotionblur = isChecked;
            }
        }
    }
}
