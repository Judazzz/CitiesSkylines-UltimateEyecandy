using System;
using System.Linq;
using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework.Plugins;

namespace UltimateEyecandy
{

    class ModPanels : UIPanel
    {
        public UITitleBar m_title;

        public AmbientPanel ambientPanel;
        public WeatherPanel weatherPanel;
        public ColorManagamentPanel colormanagementPanel;
        public ColorManagamentAdvancedPanel colormanagementAdvancedPanel;
        public FilePanel filePanel;

        public UITabstrip panelTabs;

        public UIButton ambientButton;
        public UIButton weatherButton;
        public UIButton colormanagementButton;
        public UIButton fileButton;

        private const float WIDTH = 270;
        private const float HEIGHT = 350;
        private const float SPACING = 5;
        private const float TITLE_HEIGHT = 36;
        private const float TABS_HEIGHT = 28;

        private static GameObject _gameObject;

        private static ModPanels _instance;
        public static ModPanels instance

        {
            get { return _instance; }
        }

        public static void Initialize()
        {
            try
            {
                // Destroy the UI if already exists
                _gameObject = GameObject.Find("UltimateEyecandyPanel");
                Destroy();

                // Creating our own gameObect, helps finding the UI in ModTools
                _gameObject = new GameObject("UltimateEyecandyPanel");
                _gameObject.transform.parent = UIView.GetAView().transform;
                _instance = _gameObject.AddComponent<ModPanels>();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public override void Start()
        {
            base.Start();

            backgroundSprite = "PoliciesBubble";
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            padding = new RectOffset(10, 10, 4, 4);
            width = SPACING + WIDTH;
            height = TITLE_HEIGHT + HEIGHT + TABS_HEIGHT + SPACING;
            relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width)/2),
                Mathf.Floor((GetUIView().fixedHeight - height)/2));

            SetupControls();
        }

        public static void Destroy()
        {
            try
            {
                if (_gameObject != null)
                    GameObject.Destroy(_gameObject);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void SetupControls()
        {

            m_title = AddUIComponent<UITitleBar>();
            m_title.title = "Ultimate Eyecandy";

            panelTabs = AddUIComponent<UITabstrip>();
            panelTabs.relativePosition = new Vector2(SPACING, TITLE_HEIGHT + SPACING);
            panelTabs.size = new Vector2(WIDTH, TABS_HEIGHT);
            panelTabs.padding = new RectOffset(2, 2, 2, 2);

            ambientButton = UIUtils.CreateTab(panelTabs, "Ambient");
            ambientButton.textScale = 0.8f;
            weatherButton = UIUtils.CreateTab(panelTabs, "Weather");
            weatherButton.textScale = 0.8f;
            colormanagementButton = UIUtils.CreateTab(panelTabs, "Colors");
            colormanagementButton.textScale = 0.8f;
            fileButton = UIUtils.CreateTab(panelTabs, "File");
            fileButton.textScale = 0.8f;

            UIPanel body = AddUIComponent<UIPanel>();
            body.width = WIDTH;
            body.height = HEIGHT;
            body.relativePosition = new Vector3(SPACING, TITLE_HEIGHT + TABS_HEIGHT + SPACING);

            //  Ambient Panel:
            weatherPanel = body.AddUIComponent<WeatherPanel>();
            weatherPanel.width = WIDTH - SPACING;
            weatherPanel.height = HEIGHT;
            weatherPanel.relativePosition = new Vector3(0, 0);
            weatherPanel.isVisible = false;
            //  Weather Panel:
            ambientPanel = body.AddUIComponent<AmbientPanel>();
            ambientPanel.width = WIDTH - SPACING;
            ambientPanel.height = HEIGHT;
            ambientPanel.relativePosition = new Vector3(0, 0);
            ambientPanel.isVisible = false;
            //  Color Management Panel:
            colormanagementPanel = body.AddUIComponent<ColorManagamentPanel>();
            colormanagementPanel.width = WIDTH - SPACING;
            colormanagementPanel.height = HEIGHT;
            colormanagementPanel.relativePosition = new Vector3(0, 0);
            colormanagementPanel.isVisible = false;
            //  File Panel:
            colormanagementAdvancedPanel = body.AddUIComponent<ColorManagamentAdvancedPanel>();
            colormanagementAdvancedPanel.width = WIDTH - SPACING;
            colormanagementAdvancedPanel.height = HEIGHT;
            colormanagementAdvancedPanel.relativePosition = new Vector3(WIDTH + SPACING, 0);
            colormanagementAdvancedPanel.isVisible = false;

            filePanel = body.AddUIComponent<FilePanel>();
            filePanel.width = WIDTH - SPACING;
            filePanel.height = HEIGHT;
            filePanel.relativePosition = new Vector3(0, 0);
            filePanel.isVisible = false;

            ambientButton.eventClick += (sender, e) => TabClicked(sender, e);
            weatherButton.eventClick += (sender, e) => TabClicked(sender, e);
            colormanagementButton.eventClick += (sender, e) => TabClicked(sender, e);
            fileButton.eventClick += (sender, e) => TabClicked(sender, e);

        }

        private void TabClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            weatherPanel.isVisible = false;
            ambientPanel.isVisible = false;
            colormanagementPanel.isVisible = false;
            colormanagementAdvancedPanel.isVisible = false;
            filePanel.isVisible = false;

            if (component == ambientButton)
            {
                ambientPanel.isVisible = true;
            }
            if (component == weatherButton)
            {
                weatherPanel.isVisible = true;
            }
            if (component == colormanagementButton)
            {
                colormanagementPanel.isVisible = true;
            }
            if (component == fileButton)
            {
                filePanel.isVisible = true;
            }
        }

        public void Toggle()
        {
            if (isVisible)
            {
                Hide();
            }
            else
            {
                Show(true);
            }
        }
    }

    public class AmbientPanel : UIPanel
    {
        //private UILabel _titleLabel;
        UISlider heightSlider;
        UISlider rotationSlider;
        UISlider intensitySlider;
        UISlider ambientSlider;

        private static AmbientPanel _instance;
        public static AmbientPanel instance
        {
            get { return _instance; }
        }

        public override void Start()
        {
            base.Start();
            _instance = this;
            //isVisible = true;
            canFocus = true;
            isInteractive = true;
            backgroundSprite = "UnlockingPanel";

            autoLayoutPadding = new RectOffset(10, 10, 4, 4);
            autoLayout = true;
            //autoFitChildrenVertically = true;
            autoLayoutDirection = LayoutDirection.Vertical;

            //autoLayoutPadding.top = 5;
            SetupControls();
        }

        private void SetupControls()
        {
            //_titleLabel = AddUIComponent<UILabel>();
            //_titleLabel.text = "Ambient";
            //_titleLabel.padding = new RectOffset(0, 0, 10, 0);

            AddUIComponent<UILabel>().text = "Height";
            heightSlider = UIUtils.CreateSlider(this, -80f, 80f);
            heightSlider.eventValueChanged += ValueChanged;

            AddUIComponent<UILabel>().text = "Rotation";
            rotationSlider = UIUtils.CreateSlider(this, -180f, 180f);
            rotationSlider.eventValueChanged += ValueChanged;

            AddUIComponent<UILabel>().text = "Intensity";
            intensitySlider = UIUtils.CreateSlider(this, 0f, 10f);
            intensitySlider.eventValueChanged += ValueChanged;
            intensitySlider.stepSize = 0.1f;

            AddUIComponent<UILabel>().text = "Ambient";
            ambientSlider = UIUtils.CreateSlider(this, 0f, 2f);
            ambientSlider.eventValueChanged += ValueChanged;
            ambientSlider.stepSize = 0.1f;
        }

        void ValueChanged(UIComponent component, float value)
        {
            if (component == heightSlider)
            {
                DayNightProperties.instance.m_Latitude = value;
            }

            if (component == rotationSlider)
            {
                DayNightProperties.instance.m_Longitude = value;
            }

            if (component == ambientSlider)
            {
                DayNightProperties.instance.m_Exposure = value;
            }

            if (component == intensitySlider)
            {
                DayNightProperties.instance.m_SunIntensity = value;
            }
        }
    }

    public class WeatherPanel : UIPanel
    {
        //private UILabel _titleLabel;

        private UICheckBox _enableWeatherCheckbox;

        private UISlider _rainIntensitySlider;
        private UICheckBox _rainMotionblurCheckbox;
        private UICheckBox _rainIsSnowCheckBox;

        private UISlider _snowIntensitySlider;

        private UISlider _fogIntensitySlider;
        //private UISlider _rainbowIntensitySlider;
        private UISlider _northernlightIntensitySlider;

        //private UISlider _timeOfDaySlider;

        private static WeatherPanel _instance;
        public static WeatherPanel Instance => _instance;

        public override void Start()
        {
            base.Start();
            _instance = this;
            //isVisible = true;
            canFocus = true;
            isInteractive = true;
            backgroundSprite = "UnlockingPanel";

            autoLayoutPadding = new RectOffset(10, 10, 4, 4);
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            //  
            SetupControls();
        }

        private void SetupControls()
        {
            //_titleLabel = AddUIComponent<UILabel>();
            //_titleLabel.text = "Weather";
            //_titleLabel.padding = new RectOffset(0, 0, 10, 0);

            //  Rain motion blur:
            AddUIComponent<UILabel>().text = "Enable weather";
            _enableWeatherCheckbox = UIUtils.CreateCheckBox(this);
            _enableWeatherCheckbox.name = "enableWeatherCheckbox";
            _enableWeatherCheckbox.isChecked = WeatherManager.instance.enabled;
            _enableWeatherCheckbox.eventCheckChanged += CheckboxChanged;
            
            if (LoadingManager.instance.m_loadedEnvironment.ToLower() == "winter")
            {
                //  Snow intensity (Snowfall-Only):
                AddUIComponent<UILabel>().text = "Snowfall intensity";
                _snowIntensitySlider = UIUtils.CreateSlider(this, 0, 10f);
                _snowIntensitySlider.name = "snowIntensitySlider";
                _snowIntensitySlider.stepSize = 0.25f;
                _snowIntensitySlider.eventValueChanged += SliderChanged;
            }
            else
            {
                //  Rain intensity:
                AddUIComponent<UILabel>().text = "Rain intensity";
                _rainIntensitySlider = UIUtils.CreateSlider(this, 0, 10f);
                _rainIntensitySlider.name = "rainIntensitySlider";
                _rainIntensitySlider.stepSize = 0.25f;
                _rainIntensitySlider.eventValueChanged += SliderChanged;
                //  Rain motion blur:
                AddUIComponent<UILabel>().text = "Rain motion blur";
                _rainMotionblurCheckbox = UIUtils.CreateCheckBox(this);
                _rainMotionblurCheckbox.name = "rainMotionblurCheckbox";
                _rainMotionblurCheckbox.eventCheckChanged += CheckboxChanged;
                //  Turn rain into snow (Snowfall-Only):
                if (SteamHelper.IsDLCOwned(SteamHelper.DLC.SnowFallDLC))
                {
                    //  Rain = snow:
                    AddUIComponent<UILabel>().text = "Rain is snow";
                    _rainIsSnowCheckBox = UIUtils.CreateCheckBox(this);
                    _rainIsSnowCheckBox.name = "rainIsSnowCheckBox ";
                    _rainIsSnowCheckBox.eventCheckChanged += CheckboxChanged;
                }
            }

            //  Fog intensity:
            AddUIComponent<UILabel>().text = "Fog";
            _fogIntensitySlider = UIUtils.CreateSlider(this, 0, 3.5f);
            _fogIntensitySlider.name = "fogIntensitySlider";
            _fogIntensitySlider.stepSize = 0.25f;
            _fogIntensitySlider.eventValueChanged += SliderChanged;

            //  Rainbow intensity:
            //AddUIComponent<UILabel>().text = "Rainbow";
            //_rainbowIntensitySlider = UIUtils.CreateSlider(this, 0, 10f);
            //_rainbowIntensitySlider.name = "rainbowIntensitySlider";
            //_rainbowIntensitySlider.stepSize = 0.25f;
            //_rainbowIntensitySlider.enabled = WeatherManager.instance.enabled;
            //_rainbowIntensitySlider.eventValueChanged += SliderChanged;

            //  Northern Light intensity (Snowfall-Only):
            if (LoadingManager.instance.m_loadedEnvironment.ToLower() == "winter")
            {
                AddUIComponent<UILabel>().text = "Northern Light";
                _northernlightIntensitySlider = UIUtils.CreateSlider(this, 0, 10f);
                _northernlightIntensitySlider.name = "northernlightIntensitySlider";
                _northernlightIntensitySlider.stepSize = 0.25f;
                _northernlightIntensitySlider.eventValueChanged += SliderChanged;
            }
        }

        private void SliderChanged(UIComponent trigger, float value)
        {
            //Debug.Log("UltimateEyecandy: " + trigger.name + " (" + value + ")");
            Debug.Log($"UltimateEyecandy slider: {trigger.name} => {value}");
            //Debug.Log("UltimateEyecandy: " + LoadingManager.instance.m_loadedEnvironment);

            //  
            if (trigger == _rainIntensitySlider)
            {
                WeatherManager.instance.m_currentRain = value;
            }
            //  
            if (trigger == _snowIntensitySlider)
            {
                WeatherManager.instance.m_currentRain = value;
            }
            else if (trigger == _fogIntensitySlider)
            {
                WeatherManager.instance.m_currentFog = value;
            }
            //else if (trigger == _rainbowIntensitySlider)
            //{
            //    WeatherManager.instance.m_currentRainbow = value;
            //}
            else if (trigger == _northernlightIntensitySlider)
            {
                WeatherManager.instance.m_currentNorthernLights = value;
            }
            else if (trigger == _northernlightIntensitySlider)
            {
                DayNightProperties.instance.m_TimeOfDay = value;
            }
        }

        private void CheckboxChanged(UIComponent trigger, bool isChecked)
        {
            Debug.Log($"UltimateEyecandy checkbox: {trigger.name} => {isChecked}");

            if (trigger == _enableWeatherCheckbox)
            {
                WeatherManager.instance.m_enableWeather = isChecked;
                //  Update sliders state:
                var sliders = WeatherPanel.Instance.GetComponents<UISlider>().ToList();
                if (isChecked)
                {
                    _rainIntensitySlider.Enable();
                    _rainMotionblurCheckbox.Enable();
                    _fogIntensitySlider.Enable();
                }
                else
                {
                    _rainIntensitySlider.Disable();
                    _rainMotionblurCheckbox.Disable();
                    _fogIntensitySlider.Disable();
                }
            }
            else if (trigger == _rainMotionblurCheckbox)
            {
                RainParticleProperties rrp = new RainParticleProperties();
                rrp.ForceRainMotionBlur = isChecked;
                //var rpp = WeatherManager.instance.GetComponent<RainParticleProperties>();
                //rpp.ForceRainMotionBlur = isChecked;
            }
            else if (trigger == _rainIsSnowCheckBox)
            {
                WeatherManager.instance.m_properties.m_rainIsSnow = isChecked;
            }
        }
    }

    public class ColorManagamentPanel : UIPanel
    {
        //private UILabel _titleLabel;
        private UILabel _lutLabel;
        public UIDropDown _lutDropdown;
        public UIButton _resetButton;
        public UIButton _saveButton;
        UIButton _toggleColorManagamentAdvanced;

        private const float WIDTH = 270;
        private const float HEIGHT = 350;
        private const float SPACING = 5;

        private static ColorManagamentPanel _instance;
        public static ColorManagamentPanel instance
        {
            get { return _instance; }
        }

        public override void Start()
        {
            base.Start();
            _instance = this;
            //isVisible = true;
            canFocus = true;
            isInteractive = true;
            //backgroundSprite = "UnlockingPanel";
            padding = new RectOffset(5, 5, 5, 0);
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoLayoutPadding.top = 5;

            SetupControls();
        }

        private void SetupControls()
        {
            //_titleLabel = AddUIComponent<UILabel>();
            //_titleLabel.text = "Color Management";
            //_titleLabel.padding = new RectOffset(0, 0, 10, 0);

            //  LUT Selection Label:
            _lutLabel = AddUIComponent<UILabel>();
            _lutLabel.text = "Select LUT";
            //  LUT Selection Dropdown:
            _lutDropdown = UIUtils.CreateDropDown(this);
            _lutDropdown.width = 180;
            foreach (var lud in ColorCorrectionManager.instance.items)
            {
                _lutDropdown.AddItem(lud);
            }
            _lutDropdown.selectedIndex = ColorCorrectionManager.instance.lastSelection;
            _lutDropdown.eventSelectedIndexChanged += (c, i) => ColorCorrectionManager.instance.currentSelection = i;
            //  Reset button:
            _resetButton = UIUtils.CreateButton(this);
            _resetButton.width = 90;
            _resetButton.text = "Reset";
            // Save button:
            _saveButton = UIUtils.CreateButton(this);
            _saveButton.width = 90;
            _saveButton.text = "Save";

            //  Advanced:
            _toggleColorManagamentAdvanced = UIUtils.CreateButton(this);
            _toggleColorManagamentAdvanced.playAudioEvents = true;
            _toggleColorManagamentAdvanced.normalBgSprite = "OptionBase";
            _toggleColorManagamentAdvanced.normalFgSprite = "Options";
            _toggleColorManagamentAdvanced.focusedBgSprite = "OptionBaseFocus";
            _toggleColorManagamentAdvanced.focusedFgSprite = "OptionsFocus";
            _toggleColorManagamentAdvanced.hoveredBgSprite = "OptionBaseHover";
            _toggleColorManagamentAdvanced.hoveredFgSprite = "OptionsHovered";
            _toggleColorManagamentAdvanced.pressedBgSprite = "OptionBasePressed";
            _toggleColorManagamentAdvanced.pressedFgSprite = "OptionsPressed";
            _toggleColorManagamentAdvanced.tooltip = "Advanced options";
            _toggleColorManagamentAdvanced.width = 25f;
            _toggleColorManagamentAdvanced.height = 25f;
            _toggleColorManagamentAdvanced.scaleFactor = 0.75f;


            //_toggleColorManagamentAdvanced.text = "Advanced";
            //_toggleColorManagamentAdvanced.scaleFactor = 0.75f;
            _toggleColorManagamentAdvanced.relativePosition = new Vector3(WIDTH - this.width - (2 * SPACING),
                _lutLabel.height + _lutDropdown.height + (3 * SPACING));
            _toggleColorManagamentAdvanced.eventClicked += (UIComponent component, UIMouseEventParameter eventParam) =>
            {
                ColorManagamentAdvancedPanel.Instance.Toggle();
            };
        }
    }

    public class ColorManagamentAdvancedPanel : UIPanel
    {
        private UILabel _titleLabel;

        private const float WIDTH = 270;
        private const float HEIGHT = 350;
        private const float SPACING = 5;

        private static ColorManagamentAdvancedPanel _instance;
        public static ColorManagamentAdvancedPanel Instance => _instance;

        public override void Start()
        {
            base.Start();
            _instance = this;
            canFocus = true;
            isInteractive = true;
            backgroundSprite = "PoliciesBubble";

            width = SPACING + WIDTH;
            height = 36 + HEIGHT + 28 + SPACING;
            opacity = 1;
            relativePosition = new Vector3(ColorManagamentPanel.instance.width + SPACING, -69f);

            autoLayoutPadding = new RectOffset(10, 10, 4, 4);
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            //  
            SetupControls();
        }

        private void SetupControls()
        {
            _titleLabel = AddUIComponent<UILabel>();
            _titleLabel.text = "Advanced Color Management";
            _titleLabel.padding = new RectOffset(0, 0, 10, 0);
        }

        private void ValueChanged(UIComponent component, float value)
        {
        }

        public void Toggle()
        {
            if (isVisible)
            {
                Hide();
            }
            else
            {
                Show(true);
            }
        }
    }

    public class FilePanel : UIPanel
    {
        //private UILabel _titleLabel;

        private static FilePanel _instance;
        public static FilePanel Instance => _instance;

        public override void Start()
        {
            base.Start();
            _instance = this;
            //isVisible = true;
            canFocus = true;
            isInteractive = true;
            backgroundSprite = "UnlockingPanel";

            autoLayoutPadding = new RectOffset(10, 10, 4, 4);
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            //  
            SetupControls();
        }

        private void SetupControls()
        {
            //_titleLabel = AddUIComponent<UILabel>();
            //_titleLabel.text = "System";
            //_titleLabel.padding = new RectOffset(0, 0, 10, 0);
        }
    }
}