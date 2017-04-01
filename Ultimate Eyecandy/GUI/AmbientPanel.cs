using ColossalFramework;
using ColossalFramework.UI;
using System;
using UnityEngine;

namespace UltimateEyecandy.GUI
{
    public class AmbientPanel : UIPanel
    {
        private UILabel _todLabel;
        private UISlider _todSlider;
        private UILabel _heightLabel;
        private UISlider _heightSlider;
        private UILabel _rotationLabel;
        private UISlider _rotationSlider;
        //private UILabel _sizeLabel;
        //private UISlider _sizeSlider;
        private UILabel _intensityLabel;
        private UISlider _intensitySlider;
        private UILabel _ambientLabel;
        private UISlider _ambientSlider;
        //private UILabel _fovLabel;
        //private UISlider _fovSlider;

        private UIButton _resetAmbientButton;

        public UISlider todSlider
        {
            get { return _todSlider; }
            set { _todSlider = this.todSlider; }
        }
        public UISlider heightSlider
        {
            get { return _heightSlider; }
            set { _heightSlider = this.heightSlider; }
        }
        public UISlider rotationSlider
        {
            get { return _rotationSlider; }
        }
        //public UISlider sizeSlider
        //{
        //    get { return _sizeSlider; }
        //    set { _sizeSlider = this.sizeSlider; }
        //}
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

        private TimeOfDayManager _todManager;
        public TimeOfDayManager todManager
        {
            get { return _todManager; }
            set { _todManager = value; }
        }
        bool pauseUpdates;

        Color32 daytimeColor = new Color32(235, 255, 92, 255);
        Color32 nighttimeColor = new Color32(24, 84, 255, 255);

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
            SetupControls();
        }

        private void SetupControls()
        {
            //  Time of Day:
            var topContainer = UIUtils.CreateFormElement(this, "top");
            topContainer.name = "heightSliderContainer";
            _todLabel = topContainer.AddUIComponent<UILabel>();
            _todLabel.text = "Time of Day";
            _todLabel.textScale = 0.8f;
            _todLabel.padding = new RectOffset(0, 0, 0, 5);
            _todSlider = UIUtils.CreateSlider(topContainer, 0.0f, 24.0f);
            _todSlider.name = "todSlider";
            _todSlider.stepSize = 1f / 60.0f;
            //_todSlider.tooltip = "Move this slider to change the Time of Day.";
            _todSlider.eventValueChanged += ValueChanged;
            _todSlider.eventDragStart += timeSlider_eventDragStart;
            _todSlider.eventMouseUp += timeSlider_eventDragEnd;

            //  Sun height (Latitude):
            var heightContainer = UIUtils.CreateFormElement(this, "center");
            heightContainer.name = "heightContainer";
            heightContainer.relativePosition = new Vector3(0, 70);
            _heightLabel = heightContainer.AddUIComponent<UILabel>();
            //_heightLabel.text = "Sun height (0)";
            _heightLabel.text = "Lattitude (0)";
            _heightLabel.textScale = 0.8f;
            _heightLabel.padding = new RectOffset(0, 0, 0, 5);
            _heightSlider = UIUtils.CreateSlider(heightContainer, -120f, 120f);
            _heightSlider.name = "heightSlider";
            _heightSlider.stepSize = 0.005f;
            //_heightSlider.tooltip = "Move this slider to change the sun's Vertical Position.";
            _heightSlider.value = UltimateEyecandyTool.currentSettings.ambient_height;
            _heightSlider.eventValueChanged += ValueChanged;

            //  Sun rotation (Longitude):
            var rotationContainer = UIUtils.CreateFormElement(this, "center");
            rotationContainer.name = "rotationContainer";
            rotationContainer.relativePosition = new Vector3(0, 120);
            _rotationLabel = rotationContainer.AddUIComponent<UILabel>();
            //_rotationLabel.text = "Sun rotation (0)";
            _rotationLabel.text = "Longitude (0)";
            _rotationLabel.textScale = 0.8f;
            _rotationLabel.padding = new RectOffset(0, 0, 0, 5);
            _rotationSlider = UIUtils.CreateSlider(rotationContainer, -180f, 180f);
            _rotationSlider.name = "rotationSlider";
            _rotationSlider.stepSize = 0.005f;
            //_rotationSlider.tooltip = "Move this slider to change the sun's Horizontal Position.";
            _rotationSlider.value = UltimateEyecandyTool.currentSettings.ambient_rotation;
            _rotationSlider.eventValueChanged += ValueChanged;

            //  Global light intensity:
            var intensityContainer = UIUtils.CreateFormElement(this, "center");
            intensityContainer.name = "intensityContainer";
            intensityContainer.relativePosition = new Vector3(0, 170);
            _intensityLabel = intensityContainer.AddUIComponent<UILabel>();
            _intensityLabel.text = "Global light intensity (0)";
            _intensityLabel.textScale = 0.8f;
            _intensityLabel.padding = new RectOffset(0, 0, 0, 5);
            _intensitySlider = UIUtils.CreateSlider(intensityContainer, 0f, 10f);
            _intensitySlider.name = "intensitySlider";
            _intensitySlider.stepSize = 0.0005f;
            //_intensitySlider.tooltip = "Move this slider to change the Sun Light Intensity.";
            _intensitySlider.value = UltimateEyecandyTool.currentSettings.ambient_intensity;
            _intensitySlider.eventValueChanged += ValueChanged;

            //  Ambient light intensity:
            var ambientContainer = UIUtils.CreateFormElement(this, "center");
            ambientContainer.name = "ambientContainer";
            ambientContainer.relativePosition = new Vector3(0, 220);
            _ambientLabel = ambientContainer.AddUIComponent<UILabel>();
            _ambientLabel.text = "Ambient light intensity (0)";
            _ambientLabel.textScale = 0.8f;
            _ambientLabel.padding = new RectOffset(0, 0, 0, 5);
            _ambientSlider = UIUtils.CreateSlider(ambientContainer, 0f, 2f);
            _ambientSlider.name = "ambientSlider";
            _ambientSlider.stepSize = 0.0005f;
            //_ambientSlider.tooltip = "Move this slider to change the Ambient Light Intensity.";
            _ambientSlider.value = UltimateEyecandyTool.currentSettings.ambient_ambient;
            _ambientSlider.eventValueChanged += ValueChanged;

            ////  Sun size:
            //var sizeContainer = UIUtils.CreateFormElement(this, "center");
            //sizeContainer.name = "sizeContainer";
            //sizeContainer.relativePosition = new Vector3(0, 170);
            //_sizeLabel = sizeContainer.AddUIComponent<UILabel>();
            //_sizeLabel.text = "Sun size";
            //_sizeLabel.textScale = 0.8f;
            //_sizeLabel.padding = new RectOffset(0, 0, 0, 5);
            //_sizeSlider = UIUtils.CreateSlider(sizeContainer, 0.01f, 10.0f);
            //_sizeSlider.name = "todSlider";
            //_sizeSlider.stepSize = 0.005f;
            ////_sizeSlider.tooltip = "Move this slider to change the size of the sun.";
            //_sizeSlider.value = UltimateEyecandyTool.currentSettings.ambient_size;
            //_sizeSlider.eventValueChanged += ValueChanged;

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
            //_fovSlider.stepSize = 0.5f;
            ////_fovSlider.tooltip = "Move this slider to change the Field of View (FoV).";
            //_fovSlider.value = 45f;
            //_fovSlider.eventValueChanged += ValueChanged;

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
                //_sizeSlider.value = 98f;
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
            if (trigger == _todSlider)
            {
                _todManager.TimeOfDay = value;
            }
            if (trigger == _heightSlider)
            {
                DayNightProperties.instance.m_Latitude = value;
                UltimateEyecandyTool.currentSettings.ambient_height = value;
                _heightLabel.text = "Latitude (" + value.ToString() + ")";
            }
            if (trigger == _rotationSlider)
            {
                DayNightProperties.instance.m_Longitude = value;
                UltimateEyecandyTool.currentSettings.ambient_rotation = value;
                _rotationLabel.text = "Longitude (" + value.ToString() + ")";
            }
            //if (trigger == _sizeSlider)
            //{
            //    DayNightProperties.instance.m_SunSize = value;
            //    UltimateEyecandyTool.currentSettings.ambient_size = value;
            //    _sizeLabel.text = "Sun size (" + value.ToString() + ")";
            //}
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

        void timeSlider_eventDragEnd(UIComponent trigger, UIMouseEventParameter eventParam)
        {
            if (UltimateEyecandyTool.config.outputDebug)
            {
                DebugUtils.Log($"AmbientPanel: Slider {trigger.name} drag end.");
            }
            pauseUpdates = false;
        }

        void timeSlider_eventDragStart(UIComponent trigger, UIDragEventParameter eventParam)
        {
            if (UltimateEyecandyTool.config.outputDebug)
            {
                DebugUtils.Log($"AmbientPanel: Slider {trigger.name} drag start.");
            }
            pauseUpdates = true;
        }

        public override void Update()
        {
            //relativePosition = new Vector3(235, 885, 0);
            if (_todManager != null)
            {
                if (_todManager.DayNightEnabled)
                {
                    _todSlider.isEnabled = true;
                    float tod = _todManager.TimeOfDay;
                    int hour = (int)Math.Floor(tod);
                    int minute = (int)Math.Floor((tod - hour) * 60.0f);
                    _todLabel.text = string.Format("Time of Day (currently: {0,2:00}:{1,2:00})", hour, minute);
                    //  
                    if (!pauseUpdates)
                    {
                        _todSlider.value = todManager.TimeOfDay;
                    }
                    //  
                    float fade = Math.Abs(_todManager.TimeOfDay - 12.0f) / 12.0f;
                    ((UISprite)_todSlider.thumbObject).color = Color32.Lerp(daytimeColor, nighttimeColor, fade);
                }
                else
                {
                    _todSlider.isEnabled = false;
                    _todLabel.text = "Time of Day (currently disabled)";
                }
            }
            base.Update();
        }
    }
}