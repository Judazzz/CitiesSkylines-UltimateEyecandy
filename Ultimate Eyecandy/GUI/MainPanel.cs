using System;
using System.Linq;
using UnityEngine;
using ColossalFramework.UI;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace UltimateEyecandy.GUI
{

    class MainPanel : UIPanel
    {
        public UITitleBar m_title;

        public AmbientPanel ambientPanel;
        public WeatherPanel weatherPanel;
        public ColorManagamentPanel colormanagementPanel;
        public PresetsPanel PresetsPanel;

        public UITabstrip panelTabs;

        public UIButton ambientButton;
        public UIButton weatherButton;
        public UIButton colormanagementButton;
        public UIButton presetsButton;

        private const float WIDTH = 270;
        private const float HEIGHT = 350;
        private const float SPACING = 5;
        private const float TITLE_HEIGHT = 36;
        private const float TABS_HEIGHT = 28;

        private static GameObject _gameObject;

        private static MainPanel _instance;
        public static MainPanel instance

        {
            get { return _instance; }
        }

        public static void Initialize()
        {
        }

        public override void Start()
        {
            base.Start();
            
            backgroundSprite = "UnlockingPanel2";
            isVisible = false;
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
            //  Title Bar:
            m_title = AddUIComponent<UITitleBar>();
            m_title.title = "Ultimate Eyecandy";
            //  Tabs:
            panelTabs = AddUIComponent<UITabstrip>();
            panelTabs.relativePosition = new Vector2(SPACING, TITLE_HEIGHT + SPACING);
            panelTabs.size = new Vector2(WIDTH, TABS_HEIGHT);
            panelTabs.padding = new RectOffset(2, 2, 2, 2);
            //  Tab Buttons:
            ambientButton = GUI.UIUtils.CreateTab(panelTabs, "Ambient");
            ambientButton.tooltip = "In this section you can change several world-related settings such as sun height and rotation, and light intensity.";
            ambientButton.textScale = 0.8f;
            weatherButton = GUI.UIUtils.CreateTab(panelTabs, "Weather");
            weatherButton.tooltip = "In this section you can change several weather-related settings such as rain, snow and fog intensity.";
            weatherButton.textScale = 0.8f;
            colormanagementButton = GUI.UIUtils.CreateTab(panelTabs, "LUT");
            colormanagementButton.tooltip = "In this section you can quickly change the LUT you want to use.";
            colormanagementButton.textScale = 0.8f;
            presetsButton = GUI.UIUtils.CreateTab(panelTabs, "Presets");
            presetsButton.tooltip = "In this section you can save your current settings as a preset, load previously saved presets, or reset everything to default.";
            presetsButton.textScale = 0.8f;

            //  Main Panel:
            UIPanel body = AddUIComponent<UIPanel>();
            body.width = WIDTH;
            body.height = HEIGHT;
            body.relativePosition = new Vector3(SPACING, TITLE_HEIGHT + TABS_HEIGHT + SPACING);

            //  Section Panels:
            //  Ambient Panel:
            ambientPanel = body.AddUIComponent<AmbientPanel>();
            ambientPanel.width = WIDTH - SPACING;
            ambientPanel.height = HEIGHT;
            ambientPanel.relativePosition = new Vector3(0, 0);
            ambientPanel.isVisible = true;
            //  Weather Panel:
            weatherPanel = body.AddUIComponent<WeatherPanel>();
            weatherPanel.width = WIDTH - SPACING;
            weatherPanel.height = HEIGHT;
            weatherPanel.relativePosition = new Vector3(0, 0);
            weatherPanel.isVisible = false;
            //  Color Management Panel:
            colormanagementPanel = body.AddUIComponent<ColorManagamentPanel>();
            colormanagementPanel.width = WIDTH - SPACING;
            colormanagementPanel.height = HEIGHT;
            colormanagementPanel.relativePosition = new Vector3(0, 0);
            colormanagementPanel.isVisible = false;
            //  Presets Panel:
            PresetsPanel = body.AddUIComponent<PresetsPanel>();
            PresetsPanel.width = WIDTH - SPACING;
            PresetsPanel.height = HEIGHT;
            PresetsPanel.relativePosition = new Vector3(0, 0);
            PresetsPanel.isVisible = false;

            //  Tab Button Events:
            ambientButton.eventClick += (c, e) => TabClicked(c, e);
            weatherButton.eventClick += (c, e) => TabClicked(c, e);
            colormanagementButton.eventClick += (c, e) => TabClicked(c, e);
            presetsButton.eventClick += (c, e) => TabClicked(c, e);
        }

        private void TabClicked(UIComponent trigger, UIMouseEventParameter e)
        {
            if (UltimateEyeCandy.config.outputDebug)
            {
                DebugUtils.Log($"MainPanel: Tab '{trigger.name}' clicked");
            }
            //  
            weatherPanel.isVisible = false;
            ambientPanel.isVisible = false;
            colormanagementPanel.isVisible = false;
            PresetsPanel.isVisible = false;

            if (trigger == ambientButton)
            {
                ambientPanel.isVisible = true;
            }
            if (trigger == weatherButton)
            {
                weatherPanel.isVisible = true;
            }
            if (trigger == colormanagementButton)
            {
                colormanagementPanel.isVisible = true;
            }
            if (trigger == presetsButton)
            {
                PresetsPanel.isVisible = true;
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

        public void AddGuiToggle()
        {
            UIMultiStateButton zoomButton = GameObject.Find("ZoomButton").GetComponent<UIMultiStateButton>();
            UIComponent bottomBar = zoomButton.parent;
            UIButton toggle = GUI.UIUtils.CreateButton(bottomBar);

            toggle.area = new Vector4(108, 24, 38, 38);
            toggle.playAudioEvents = true;
            toggle.normalBgSprite = "OptionBase";
            toggle.focusedBgSprite = "OptionBaseFocus";
            toggle.hoveredBgSprite = "OptionBaseHover";
            toggle.pressedBgSprite = "OptionBasePressed";
            toggle.tooltip = "Light Control";
            toggle.normalFgSprite = "InfoIconEntertainmentDisabled";
            toggle.scaleFactor = 0.75f;

            toggle.eventClicked += (c, e) =>
            {
                Toggle();
            };

        }
    }
}