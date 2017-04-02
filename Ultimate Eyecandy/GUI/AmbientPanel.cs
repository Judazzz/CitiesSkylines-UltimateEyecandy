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
        private UILabel _speedLabel;
        private UISlider _speedSlider;
        private UILabel _heightLabel;
        private UISlider _heightSlider;
        private UILabel _rotationLabel;
        private UISlider _rotationSlider;
        private UILabel _intensityLabel;
        private UISlider _intensitySlider;
        private UILabel _ambientLabel;
        private UISlider _ambientSlider;

        //private UILabel _sizeLabel;
        //private UISlider _sizeSlider;
        //private UILabel _fovLabel;
        //private UISlider _fovSlider;

        private UIButton _resetAmbientButton;

        public UISlider todSlider
        {
            get { return _todSlider; }
            set { _todSlider = this.todSlider; }
        }
        public UISlider speedSlider
        {
            get { return _speedSlider; }
            set { _speedSlider = this.speedSlider; }
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
        public UISlider intensitySlider
        {
            get { return _intensitySlider; }
        }
        public UISlider ambientSlider
        {
            get { return _ambientSlider; }
        }

        //public UISlider sizeSlider
        //{
        //    get { return _sizeSlider; }
        //    set { _sizeSlider = this.sizeSlider; }
        //}
        //public UISlider fovSlider
        //{
        //    get { return _fovSlider; }
        //}

        private DayNightCycleManager _todManager;
        public DayNightCycleManager todManager
        {
            get { return _todManager; }
            set { _todManager = value; }
        }

        private static Fraction[] speeds = {
            new Fraction(){num=0, den=1},
            new Fraction(){num=1, den=128},
            new Fraction(){num=1, den=64},
            new Fraction(){num=1, den=16},
            new Fraction(){num=1, den=8},
            new Fraction(){num=1, den=4},
            new Fraction(){num=1, den=2},
            new Fraction(){num=1, den=1},
            new Fraction(){num=2, den=1},
            new Fraction(){num=4, den=1},
            new Fraction(){num=8, den=1},
            new Fraction(){num=16, den=1},
            new Fraction(){num=32, den=1},
            new Fraction(){num=64, den=1},
            new Fraction(){num=128,den=1}
        };

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
            _todLabel.padding = new RectOffset(0, 0, 0, 0);
            _todSlider = UIUtils.CreateSlider(topContainer, 0.0f, 24.0f);
            _todSlider.name = "todSlider";
            _todSlider.stepSize = 1f / 60.0f;
            _todSlider.eventValueChanged += ValueChanged;
            _todSlider.eventDragStart += timeSlider_eventDragStart;
            _todSlider.eventMouseUp += timeSlider_eventDragEnd;
            _todSlider.isEnabled = UltimateEyecandyTool.config.enableSimulationControl;

            //  Simulation speed:
            var speedContainer = UIUtils.CreateFormElement(this, "center");
            speedContainer.name = "sizeContainer";
            speedContainer.relativePosition = new Vector3(0, 65);
            _speedLabel = speedContainer.AddUIComponent<UILabel>();
            _speedLabel.text = "Day/night cycle speed";
            _speedLabel.textScale = 0.8f;
            _speedLabel.padding = new RectOffset(0, 0, 0, 0);
            _speedSlider = UIUtils.CreateSlider(speedContainer, 0f, speeds.Length);
            _speedSlider.name = "speedSlider";
            _speedSlider.stepSize = 0.005f;
            _speedSlider.eventValueChanged += ValueChanged;
            _speedSlider.isEnabled = UltimateEyecandyTool.config.enableSimulationControl;

            //  Sun height (Latitude):
            var heightContainer = UIUtils.CreateFormElement(this, "center");
            heightContainer.name = "heightContainer";
            heightContainer.relativePosition = new Vector3(0, 110);
            _heightLabel = heightContainer.AddUIComponent<UILabel>();
            //_heightLabel.text = "Sun height (0)";
            _heightLabel.text = "Lattitude (0)";
            _heightLabel.textScale = 0.8f;
            _heightLabel.padding = new RectOffset(0, 0, 0, 0);
            _heightSlider = UIUtils.CreateSlider(heightContainer, -120f, 120f);
            _heightSlider.name = "heightSlider";
            _heightSlider.stepSize = 0.005f;
            _heightSlider.value = UltimateEyecandyTool.currentSettings.ambient_height;
            _heightSlider.eventValueChanged += ValueChanged;

            //  Sun rotation (Longitude):
            var rotationContainer = UIUtils.CreateFormElement(this, "center");
            rotationContainer.name = "rotationContainer";
            rotationContainer.relativePosition = new Vector3(0, 155);
            _rotationLabel = rotationContainer.AddUIComponent<UILabel>();
            //_rotationLabel.text = "Sun rotation (0)";
            _rotationLabel.text = "Longitude (0)";
            _rotationLabel.textScale = 0.8f;
            _rotationLabel.padding = new RectOffset(0, 0, 0, 0);
            _rotationSlider = UIUtils.CreateSlider(rotationContainer, -180f, 180f);
            _rotationSlider.name = "rotationSlider";
            _rotationSlider.stepSize = 0.005f;
            _rotationSlider.value = UltimateEyecandyTool.currentSettings.ambient_rotation;
            _rotationSlider.eventValueChanged += ValueChanged;

            //  Global light intensity:
            var intensityContainer = UIUtils.CreateFormElement(this, "center");
            intensityContainer.name = "intensityContainer";
            intensityContainer.relativePosition = new Vector3(0, 200);
            _intensityLabel = intensityContainer.AddUIComponent<UILabel>();
            _intensityLabel.text = "Global light intensity (0)";
            _intensityLabel.textScale = 0.8f;
            _intensityLabel.padding = new RectOffset(0, 0, 0, 0);
            _intensitySlider = UIUtils.CreateSlider(intensityContainer, 0f, 10f);
            _intensitySlider.name = "intensitySlider";
            _intensitySlider.stepSize = 0.0005f;
            _intensitySlider.value = UltimateEyecandyTool.currentSettings.ambient_intensity;
            _intensitySlider.eventValueChanged += ValueChanged;

            //  Ambient light intensity:
            var ambientContainer = UIUtils.CreateFormElement(this, "center");
            ambientContainer.name = "ambientContainer";
            ambientContainer.relativePosition = new Vector3(0, 245);
            _ambientLabel = ambientContainer.AddUIComponent<UILabel>();
            _ambientLabel.text = "Ambient light intensity (0)";
            _ambientLabel.textScale = 0.8f;
            _ambientLabel.padding = new RectOffset(0, 0, 0, 0);
            _ambientSlider = UIUtils.CreateSlider(ambientContainer, 0f, 2f);
            _ambientSlider.name = "ambientSlider";
            _ambientSlider.stepSize = 0.0005f;
            _ambientSlider.value = UltimateEyecandyTool.currentSettings.ambient_ambient;
            _ambientSlider.eventValueChanged += ValueChanged;

            ////  Sun size:
            //var sizeContainer = UIUtils.CreateFormElement(this, "center");
            //sizeContainer.name = "sizeContainer";
            //sizeContainer.relativePosition = new Vector3(0, 170);
            //_sizeLabel = sizeContainer.AddUIComponent<UILabel>();
            //_sizeLabel.text = "Sun size";
            //_sizeLabel.textScale = 0.8f;
            //_sizeLabel.padding = new RectOffset(0, 0, 0, 0);
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
            //_fovLabel.padding = new RectOffset(0, 0, 0, 0);
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
            if (trigger == _speedSlider)
            {
                _todManager.speed = speeds[(uint)value];
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

            //if (trigger == _sizeSlider)
            //{
            //    DayNightProperties.instance.m_SunSize = value;
            //    UltimateEyecandyTool.currentSettings.ambient_size = value;
            //    _sizeLabel.text = "Sun size (" + value.ToString() + ")";
            //}
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
            if (!UltimateEyecandyTool.config.enableSimulationControl)
            {
                _todSlider.isEnabled = false;
                _todLabel.text = "Time of Day (disabled)";
                //  
                _speedLabel.text = "Day/night cycle speed (disabled)";
                _speedSlider.value = 0;
                _speedSlider.isEnabled = false;
                return;
            }
            if (_todManager != null)
            {
                if (_todManager.DayNightEnabled)
                {
                    float tod = _todManager.TimeOfDay;
                    int hour = (int)Math.Floor(tod);
                    int minute = (int)Math.Floor((tod - hour) * 60.0f);
                    _todLabel.text = string.Format("Time of Day (currently: {0,2:00}:{1,2:00})", hour, minute);
                    _todSlider.isEnabled = true;
                    //  
                    if (!pauseUpdates)
                    {
                        _todSlider.value = todManager.TimeOfDay;
                    }
                    //  
                    float fade = Math.Abs(_todManager.TimeOfDay - 12.0f) / 12.0f;
                    ((UISprite)_todSlider.thumbObject).color = Color32.Lerp(daytimeColor, nighttimeColor, fade);
                    //  
                    _speedLabel.text = string.Format($"Day/night cycle speed ({_todManager.speed})");
                    _speedSlider.value = Array.IndexOf(speeds, _todManager.speed);
                }
                else
                {
                    _todLabel.text = "Time of Day (disabled)";
                    _todSlider.isEnabled = false;
                    //  
                    _speedLabel.text = "Day/night cycle speed (disabled)";
                    _speedSlider.value = 0;
                    _speedSlider.isEnabled = false;
                }
            }

            base.Update();
        }
    }
}