using System;
using System.Reflection;
using ColossalFramework.UI;
using UnityEngine;
using ColossalFramework.Plugins;
using System.Linq;

namespace UltimateEyecandy.GUI
{

    class ModMainPanel : UIPanel
    {
        public UIMainTitleBar m_title;

        public UITabstrip panelTabs;
        public UIButton ambientButton;
        public UIButton weatherButton;
        public UIButton colorManagementButton;
        public UIButton presetsButton;

        public AmbientPanel ambientPanel;
        public WeatherPanel weatherPanel;
        public ColorManagementPanel colorManagementPanel;
        public PresetsPanel presetsPanel;

        public UIButton toggleUltimateEyecandyButton;
        public UITextureAtlas toggleButtonAtlas = null;
        static readonly string UE = "UltimateEyecandy";

        private static GameObject _gameObject;
        private static ModMainPanel _instance;

        public static ModMainPanel instance

        {
            get { return _instance; }
        }

        public static void Initialize()
        {
        }

        public override void Start()
        {
            base.Start();
            _instance = this;

            backgroundSprite = (UltimateEyecandy.isEditor) ? "MenuPanel2" : "LevelBarBackground";
            isVisible = false;
            canFocus = true;
            isInteractive = true;
            name = "modMainPanel";
            padding = new RectOffset(10, 10, 4, 4);
            width = UltimateEyecandy.SPACING + UltimateEyecandy.WIDTH;
            height = UltimateEyecandy.TITLE_HEIGHT + UltimateEyecandy.TABS_HEIGHT + UltimateEyecandy.HEIGHT + UltimateEyecandy.SPACING;
            relativePosition = new Vector3(10, 60);
            //  
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
            //  Title Bar:
            m_title = AddUIComponent<UIMainTitleBar>();
            m_title.title = "Ultimate Eyecandy";
            //  Tabs:
            panelTabs = AddUIComponent<UITabstrip>();
            panelTabs.relativePosition = new Vector2(10, UltimateEyecandy.TITLE_HEIGHT + UltimateEyecandy.SPACING);
            panelTabs.size = new Vector2(UltimateEyecandy.WIDTH - (3 * UltimateEyecandy.SPACING), UltimateEyecandy.TABS_HEIGHT);

            //  Tab Buttons:
            ambientButton = UIUtils.CreateTab(panelTabs, "Ambient", true);
            ambientButton.tooltip = "In this section you can change several world-related settings such as the sun's horizontal and vertical position and sun and ambient light intensity.";
            ambientButton.textScale = 0.8f;
            weatherButton = UIUtils.CreateTab(panelTabs, "Weather");
            weatherButton.tooltip = "In this section you can change several weather-related settings such as rain, snow and fog intensity.";
            weatherButton.textScale = 0.8f;
            colorManagementButton = UIUtils.CreateTab(panelTabs, "LUT");
            colorManagementButton.tooltip = "In this section you can quickly change the LUT you want to use.";
            colorManagementButton.textScale = 0.8f;
            presetsButton = UIUtils.CreateTab(panelTabs, "Presets");
            presetsButton.tooltip = "In this section you can save your current settings as a Preset, load previously saved Presets, or reset everything to default.";
            presetsButton.textScale = 0.8f;

            //  Main Panel:
            UIPanel body = AddUIComponent<UIPanel>();
            body.width = UltimateEyecandy.WIDTH;
            body.height = UltimateEyecandy.HEIGHT;
            //  ScrollRect
            body.relativePosition = new Vector3(UltimateEyecandy.SPACING, UltimateEyecandy.TITLE_HEIGHT + UltimateEyecandy.TABS_HEIGHT + UltimateEyecandy.SPACING);

            //  Section Panels:
            //  Ambient Panel:
            ambientPanel = body.AddUIComponent<AmbientPanel>();
            ambientPanel.name = "ambientPanel";
            ambientPanel.width = UltimateEyecandy.WIDTH - (3 * UltimateEyecandy.SPACING);
            ambientPanel.height = UltimateEyecandy.HEIGHT;
            ambientPanel.relativePosition = new Vector3(5, 0);
            ambientPanel.isVisible = true;
            //  Weather Panel:
            weatherPanel = body.AddUIComponent<WeatherPanel>();
            weatherPanel.name = "weatherPanel";
            weatherPanel.width = UltimateEyecandy.WIDTH - 3 * UltimateEyecandy.SPACING;
            weatherPanel.height = UltimateEyecandy.HEIGHT;
            weatherPanel.relativePosition = new Vector3(5, 0);
            weatherPanel.isVisible = false;
            //  Color Management Panel:
            colorManagementPanel = body.AddUIComponent<ColorManagementPanel>();
            colorManagementPanel.name = "colormanagementPanel";
            colorManagementPanel.width = UltimateEyecandy.WIDTH - 3 * UltimateEyecandy.SPACING;
            colorManagementPanel.height = UltimateEyecandy.HEIGHT;
            colorManagementPanel.relativePosition = new Vector3(5, 0);
            colorManagementPanel.isVisible = false;
            //  Presets Panel:
            presetsPanel = body.AddUIComponent<PresetsPanel>();
            presetsPanel.name = "presetsPanel";
            presetsPanel.width = UltimateEyecandy.WIDTH - 3 * UltimateEyecandy.SPACING;
            presetsPanel.height = UltimateEyecandy.HEIGHT;
            presetsPanel.relativePosition = new Vector3(5, 0);
            presetsPanel.isVisible = false;

            //  Tab Button Events:
            ambientButton.eventClick += (c, e) => TabClicked(c, e);
            weatherButton.eventClick += (c, e) => TabClicked(c, e);
            colorManagementButton.eventClick += (c, e) => TabClicked(c, e);
            presetsButton.eventClick += (c, e) => TabClicked(c, e);
        }

        private void TabClicked(UIComponent trigger, UIMouseEventParameter e)
        {
            if (UltimateEyecandy.config.outputDebug)
            {
                DebugUtils.Log($"MainPanel: Tab '{trigger.name}' clicked");
            }
            //  
            weatherPanel.isVisible = false;
            ambientPanel.isVisible = false;
            colorManagementPanel.isVisible = false;
            presetsPanel.isVisible = false;

            if (trigger == ambientButton)
            {
                ambientPanel.isVisible = true;
            }
            if (trigger == weatherButton)
            {
                weatherPanel.isVisible = true;
            }
            if (trigger == colorManagementButton)
            {
                colorManagementPanel.isVisible = true;
                colorManagementPanel.lutFastlist.selectedIndex = ColorCorrectionManager.instance.lastSelection;
                var isActive = (colorManagementPanel._selectedLut.internal_name == UltimateEyecandy.currentSettings.color_selectedlut);
                colorManagementPanel.loadLutButton.isEnabled = (isActive) ? false : true;
                colorManagementPanel.loadLutButton.opacity = (isActive) ? 0.5f : 1.0f;
                colorManagementPanel.loadLutButton.tooltip = (isActive) ? "LUT selected in list is already active." : "Load LUT selected in list.";
            }
            if (trigger == presetsButton)
            {
                if (presetsPanel.presetFastlist.selectedIndex < 0)
                {
                    presetsPanel.updateButtons(true);
                }
                else
                {
                    presetsPanel.updateButtons(false);
                }
                presetsPanel.isVisible = true;
            }
        }

        public void AddGuiToggle()
        {
            const int size = 36;

            //  Positioned relative to Freecamera Button:
            var freeCameraButton = UIView.GetAView().FindUIComponent<UIButton>("Freecamera");
            toggleUltimateEyecandyButton = UIView.GetAView().FindUIComponent<UIPanel>("InfoPanel").AddUIComponent<UIButton>();
            toggleUltimateEyecandyButton.verticalAlignment = UIVerticalAlignment.Middle;
            toggleUltimateEyecandyButton.relativePosition = toggleButtonPositionConflicted()
                //  Enhanced Mouse Light mod detected (position button more to left to prevent overlapping):
                ? new Vector3(freeCameraButton.absolutePosition.x - 76, freeCameraButton.relativePosition.y)
                //  Enhanced Mouse Light mod not detected (position button in default position):
                : new Vector3(freeCameraButton.absolutePosition.x - 42, freeCameraButton.relativePosition.y);
            //  
            toggleUltimateEyecandyButton.size = new Vector2(36f, 36f);
            toggleUltimateEyecandyButton.playAudioEvents = true;
            toggleUltimateEyecandyButton.tooltip = "Ultimate Eyecandy " + ModInfo.version;
            //  Create custom atlas:
            if (toggleButtonAtlas == null)
            {
                toggleButtonAtlas = UIUtils.CreateAtlas(UE, size, size, "ToolbarIcon.png", new[]
                                            {
                                                "EyecandyNormalBg",
                                                "EyecandyHoveredBg",
                                                "EyecandyPressedBg",
                                                "EyecandyNormalFg",
                                                "EyecandyHoveredFg",
                                                "EyecandyPressedFg",
                                                "EyecandyButtonNormal",
                                                "EyecandyButtonHover",
                                                "EyecandyInfoTextBg",
                                            });
            }
            //  Apply custom sprite:
            toggleUltimateEyecandyButton.atlas = toggleButtonAtlas;
            toggleUltimateEyecandyButton.normalFgSprite = "EyecandyNormalBg";
            toggleUltimateEyecandyButton.normalBgSprite = null;
            toggleUltimateEyecandyButton.hoveredFgSprite = "EyecandyHoveredBg";
            toggleUltimateEyecandyButton.hoveredBgSprite = "EyecandyHoveredFg";
            toggleUltimateEyecandyButton.pressedFgSprite = "EyecandyPressedBg";
            toggleUltimateEyecandyButton.pressedBgSprite = "EyecandyPressedFg";
            toggleUltimateEyecandyButton.focusedFgSprite = "EyecandyPressedBg";
            toggleUltimateEyecandyButton.focusedBgSprite = "EyecandyPressedFg";
            //  Event handling:
            toggleUltimateEyecandyButton.eventClicked += (c, e) =>
            {
                isVisible = !isVisible;
                if (!isVisible)
                {
                    toggleUltimateEyecandyButton.Unfocus();
                }
            };

        }

        private static bool toggleButtonPositionConflicted()
        {
            if (PluginManager.instance.GetPluginsInfo().Any(mod => (mod.publishedFileID.AsUInt64 == 527036685 && mod.isEnabled)))
            {
                return true;
            }
            return false;
        }
    }
}