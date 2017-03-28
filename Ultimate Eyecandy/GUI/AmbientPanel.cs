using ColossalFramework.UI;
using UnityEngine;

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
        //private UILabel _fovLabel;
        //private UISlider _fovSlider;

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

        //public UISlider fovSlider
        //{
        //    get { return _fovSlider; }
        //}

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
            topContainer.name = "heightSliderContainer";

            _heightLabel = topContainer.AddUIComponent<UILabel>();
            _heightLabel.text = "Sun height (0)";
            _heightLabel.textScale = 0.8f;
            _heightLabel.padding = new RectOffset(0, 0, 0, 5);

            _heightSlider = UIUtils.CreateSlider(topContainer, -120f, 120f);
            _heightSlider.name = "heightSlider";
            _heightSlider.tooltip = "Move this slider to change the sun's Vertical Position.";
            _heightSlider.value = UltimateEyecandyTool.currentSettings.ambient_height;
            _heightSlider.eventValueChanged += ValueChanged;
            _heightSlider.stepSize = 0.005f;

            //  Sun rotation:
            var sunContainer = UIUtils.CreateFormElement(this, "center");
            sunContainer.name = "rotationSliderContainer";
            sunContainer.relativePosition = new Vector3(0, 70);

            _rotationLabel = sunContainer.AddUIComponent<UILabel>();
            _rotationLabel.text = "Sun rotation (0)";
            _rotationLabel.textScale = 0.8f;
            _rotationLabel.padding = new RectOffset(0, 0, 0, 5);

            _rotationSlider = UIUtils.CreateSlider(sunContainer, -180f, 180f);
            _rotationSlider.name = "rotationSlider";
            _rotationSlider.tooltip = "Move this slider to change the sun's Horizontal Position.";
            _rotationSlider.value = UltimateEyecandyTool.currentSettings.ambient_rotation;
            _rotationSlider.eventValueChanged += ValueChanged;
            _rotationSlider.stepSize = 0.005f;

            //  Global light intensity:
            var globalContainer = UIUtils.CreateFormElement(this, "center");
            globalContainer.name = "intensitySliderContainer";
            globalContainer.relativePosition = new Vector3(0, 120);

            _intensityLabel = globalContainer.AddUIComponent<UILabel>();
            _intensityLabel.text = "Global light intensity (0)";
            _intensityLabel.textScale = 0.8f;
            _intensityLabel.padding = new RectOffset(0, 0, 0, 5);

            _intensitySlider = UIUtils.CreateSlider(globalContainer, 0f, 10f);
            _intensitySlider.name = "intensitySlider";
            _intensitySlider.tooltip = "Move this slider to change the Sun Light Intensity.";
            _intensitySlider.value = UltimateEyecandyTool.currentSettings.ambient_intensity;
            _intensitySlider.eventValueChanged += ValueChanged;
            _intensitySlider.stepSize = 0.0005f;

            //  Ambient light intensity:
            var ambientContainer = UIUtils.CreateFormElement(this, "center");
            ambientContainer.name = "ambientSliderContainer";
            ambientContainer.relativePosition = new Vector3(0, 170);

            _ambientLabel = ambientContainer.AddUIComponent<UILabel>();
            _ambientLabel.text = "Ambient light intensity (0)";
            _ambientLabel.textScale = 0.8f;
            _ambientLabel.padding = new RectOffset(0, 0, 0, 5);

            _ambientSlider = UIUtils.CreateSlider(ambientContainer, 0f, 2f);
            _ambientSlider.name = "ambientSlider";
            _ambientSlider.tooltip = "Move this slider to change the Ambient Light Intensity.";
            _ambientSlider.value = UltimateEyecandyTool.currentSettings.ambient_ambient;
            _ambientSlider.eventValueChanged += ValueChanged;
            _ambientSlider.stepSize = 0.0005f;

            //  Field of View:
            //var fovContainer = UIUtils.CreateFormElement(this, "center");
            //fovContainer.name = "fovContainer";
            //fovContainer.relativePosition = new Vector3(0, 220);

            //_fovLabel = fovContainer.AddUIComponent<UILabel>();
            //_fovLabel.text = "Field of View (45)";
            //_fovLabel.textScale = 0.8f;
            //_fovLabel.padding = new RectOffset(0, 0, 0, 5);

            //_fovSlider = UIUtils.CreateSlider(fovContainer, 10f, 80f);
            //_fovSlider.name = "fovSlider";
            //_fovSlider.tooltip = "Move this slider to change the Field of View (FoV).";
            //_fovSlider.value = 45f;
            //_fovSlider.eventValueChanged += ValueChanged;
            //_fovSlider.stepSize = 0.5f;

            //  Reset button:
            var resetContainer = UIUtils.CreateFormElement(this, "bottom");

            _resetAmbientButton = UIUtils.CreateButton(resetContainer);
            _resetAmbientButton.name = "resetButton";
            _resetAmbientButton.text = "Reset";
            _resetAmbientButton.tooltip = "Reset all values set in this panel to default values.";
            _resetAmbientButton.eventClicked += (c, e) =>
            {
                if (UltimateEyecandyTool.config.outputDebug)
                {
                    DebugUtils.Log($"AmbientPanel: 'Reset' clicked.");
                }
                //  
                _heightSlider.value = (UltimateEyecandyTool.isWinterMap) ? 66f : 35f;
                _rotationSlider.value = 98f;
                _intensitySlider.value = 6f;
                _ambientSlider.value = (UltimateEyecandyTool.isWinterMap) ? 0.4f : 0.71f;
            };
        }

        void ValueChanged(UIComponent trigger, float value)
        {
            if (UltimateEyecandyTool.config.outputDebug)
            {
                DebugUtils.Log($"AmbientPanel: Slider {trigger.name} = {value}");
            }
            //  
            if (trigger == _heightSlider)
            {
                DayNightProperties.instance.m_Latitude = value;
                UltimateEyecandyTool.currentSettings.ambient_height = value;
                _heightLabel.text = "Sun height (" + value.ToString() + ")";
            }

            if (trigger == _rotationSlider)
            {
                DayNightProperties.instance.m_Longitude = value;
                UltimateEyecandyTool.currentSettings.ambient_rotation = value;
                _rotationLabel.text = "Sun rotation (" + value.ToString() + ")";
            }

            if (trigger == _intensitySlider)
            {
                DayNightProperties.instance.m_SunIntensity = value;
                UltimateEyecandyTool.currentSettings.ambient_intensity = value;
                _intensityLabel.text = "Global light intensity (" + value.ToString() + ")";
            }

            if (trigger == _ambientSlider)
            {
                DayNightProperties.instance.m_Exposure = value;
                UltimateEyecandyTool.currentSettings.ambient_ambient = value;
                _ambientLabel.text = "Ambient light intensity (" + value.ToString() + ")";
            }

            //if (trigger == _fovSlider)
            //{
            //    Camera.main.fieldOfView = value;
            //    _fovLabel.text = "Field of View (" + value.ToString() + ")";
            //}
        }
    }
}
