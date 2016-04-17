using UnityEngine;
using ColossalFramework.UI;

namespace UltimateEyecandy.GUI
{
    public class AmbientPanel : UIPanel
    {
        private UILabel _heightLabel;
        private UISlider _heightSlider;
        private UILabel _rotationLabel;
        private UISlider _rotationSlider;
        private UILabel _intensityLabel;
        private UISlider _intensitySlider;
        private UILabel _ambientLabel;
        private UISlider _ambientSlider;

        private UIButton _resetAmbientButton;

        public UISlider heightSlider
        {
            get { return _heightSlider; }
            set { _heightSlider = this.heightSlider; }
        }
        public UISlider rotationSlider
        {
            get { return _rotationSlider; }
        }
        public UISlider intensitySlider
        {
            get { return _intensitySlider; }
        }
        public UISlider ambientSlider
        {
            get { return _ambientSlider; }
        }

        private static AmbientPanel _instance;
        public static AmbientPanel instance
        {
            get { return _instance; }
        }

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
            //  Presets:
            var topContainer = UIUtils.CreateFormElement(this, "top");

            _heightLabel = topContainer.AddUIComponent<UILabel>();
            _heightLabel.text = "Sun height";
            _heightLabel.textScale = 0.9f;
            _heightLabel.padding = new RectOffset(0, 0, 0, 5);

            _heightSlider = UIUtils.CreateSlider(topContainer, -80f, 80f);
            _heightSlider.name = "heightSlider";
            _heightSlider.tooltip = "Move this slider to change the sun's vertical position.";
            _heightSlider.value = 0;
            _heightSlider.eventValueChanged += ValueChanged;

            //  Sun rotation:
            var sunContainer = UIUtils.CreateFormElement(this, "center");
            sunContainer.relativePosition = new Vector3(0, 75);

            _rotationLabel = sunContainer.AddUIComponent<UILabel>();
            _rotationLabel.text = "Sun rotation";
            _rotationLabel.textScale = 0.9f;
            _rotationLabel.padding = new RectOffset(0, 0, 0, 5);

            _rotationSlider = UIUtils.CreateSlider(sunContainer, -180f, 180f);
            _rotationSlider.name = "rotationSlider";
            _rotationSlider.tooltip = "Move this slider to change the sun's horizontal position.";
            _rotationSlider.value = 0;
            _rotationSlider.eventValueChanged += ValueChanged;

            //  Global light intensity:
            var globalContainer = UIUtils.CreateFormElement(this, "center");
            globalContainer.relativePosition = new Vector3(0, 135);

            _intensityLabel = globalContainer.AddUIComponent<UILabel>();
            _intensityLabel.text = "Global light intensity";
            _intensityLabel.textScale = 0.9f;
            _intensityLabel.padding = new RectOffset(0, 0, 0, 5);

            _intensitySlider = UIUtils.CreateSlider(globalContainer, 0f, 10f);
            _intensitySlider.name = "intensitySlider";
            _intensitySlider.tooltip = "Move this slider to change the intensity of the sun light.";
            _intensitySlider.value = 5f;
            _intensitySlider.eventValueChanged += ValueChanged;
            _intensitySlider.stepSize = 0.1f;

            //  Ambient light intensity:
            var ambientContainer = UIUtils.CreateFormElement(this, "center");
            ambientContainer.relativePosition = new Vector3(0, 195);

            _ambientLabel = ambientContainer.AddUIComponent<UILabel>();
            _ambientLabel.text = "Ambient light intensity";
            _ambientLabel.textScale = 0.9f;
            _ambientLabel.padding = new RectOffset(0, 0, 0, 5);

            _ambientSlider = UIUtils.CreateSlider(ambientContainer, 0f, 2f);
            _ambientSlider.name = "ambientSlider";
            _ambientSlider.tooltip = "Move this slider to change the intensity of the ambient light.";
            _ambientSlider.value = 1f;
            _ambientSlider.eventValueChanged += ValueChanged;
            _ambientSlider.stepSize = 0.1f;

            //  Reset button:
            var resetContainer = UIUtils.CreateFormElement(this, "bottom");

            _resetAmbientButton = UIUtils.CreateButton(resetContainer);
            _resetAmbientButton.name = "resetButton";
            _resetAmbientButton.text = "Reset";
            _resetAmbientButton.tooltip = "Reset all values set in this panel to default values.";
            _resetAmbientButton.eventClicked += (c, e) =>
            {
                if (UltimateEyeCandy.config.outputDebug)
                {
                    DebugUtils.Log($"AmbientPanel: 'Reset' clicked.");
                }
                //  
                _heightSlider.value = 0;
                _rotationSlider.value = 0;
                _intensitySlider.value = 5f;
                _ambientSlider.value = 1f;
            };
        }

        void ValueChanged(UIComponent trigger, float value)
        {
            if (UltimateEyeCandy.config.outputDebug)
            {
                DebugUtils.Log($"AmbientPanel: Slider {trigger.name} = {value}");
            }
            //  
            if (trigger == _heightSlider)
            {
                DayNightProperties.instance.m_Latitude = value;
                UltimateEyeCandy.currentSettings.ambient_height = value;
            }

            if (trigger == _rotationSlider)
            {
                DayNightProperties.instance.m_Longitude = value;
                UltimateEyeCandy.currentSettings.ambient_rotation = value;
            }

            if (trigger == _intensitySlider)
            {
                DayNightProperties.instance.m_SunIntensity = value;
                UltimateEyeCandy.currentSettings.ambient_intensity = value;
            }

            if (trigger == _ambientSlider)
            {
                DayNightProperties.instance.m_Exposure = value;
                UltimateEyeCandy.currentSettings.ambient_ambient = value;
            }
        }
    }
}
