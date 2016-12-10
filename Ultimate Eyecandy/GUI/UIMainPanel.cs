using System;
using System.Reflection;
using ColossalFramework.UI;
using UnityEngine;
using ColossalFramework.Plugins;
using System.Linq;

namespace UltimateEyecandy.GUI
{

    public class UIMainPanel : UIPanel
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

        public static UIMainPanel instance;

        public static void Initialize()
        {
        }

        public override void Start()
        {
            base.Start();
            instance = this;
            //  
            backgroundSprite = (UltimateEyecandyTool.isEditor) ? "MenuPanel2" : "LevelBarBackground";
            isVisible = false;
            canFocus = true;
            isInteractive = true;
            name = "modMainPanel";
            padding = new RectOffset(10, 10, 4, 4);
            width = UltimateEyecandyTool.SPACING + UltimateEyecandyTool.WIDTH;
            height = UltimateEyecandyTool.TITLE_HEIGHT + UltimateEyecandyTool.TABS_HEIGHT + UltimateEyecandyTool.HEIGHT + UltimateEyecandyTool.SPACING;
            relativePosition = new Vector3(10, 60);
            //  
            SetupControls();
        }

        public void SetupControls()
        {
            //  Title Bar:
            m_title = AddUIComponent<UIMainTitleBar>();
            m_title.title = "Ultimate Eyecandy";
            //  Tabs:
            panelTabs = AddUIComponent<UITabstrip>();
            panelTabs.relativePosition = new Vector2(10, UltimateEyecandyTool.TITLE_HEIGHT + UltimateEyecandyTool.SPACING);
            panelTabs.size = new Vector2(UltimateEyecandyTool.WIDTH - (3 * UltimateEyecandyTool.SPACING), UltimateEyecandyTool.TABS_HEIGHT);

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
            body.width = UltimateEyecandyTool.WIDTH;
            body.height = UltimateEyecandyTool.HEIGHT;
            //  ScrollRect
            body.relativePosition = new Vector3(UltimateEyecandyTool.SPACING, UltimateEyecandyTool.TITLE_HEIGHT + UltimateEyecandyTool.TABS_HEIGHT + UltimateEyecandyTool.SPACING);

            //  Section Panels:
            //  Ambient Panel:
            ambientPanel = body.AddUIComponent<AmbientPanel>();
            ambientPanel.name = "ambientPanel";
            ambientPanel.width = UltimateEyecandyTool.WIDTH - (3 * UltimateEyecandyTool.SPACING);
            ambientPanel.height = UltimateEyecandyTool.HEIGHT;
            ambientPanel.relativePosition = new Vector3(5, 0);
            ambientPanel.isVisible = true;
            //  Weather Panel:
            weatherPanel = body.AddUIComponent<WeatherPanel>();
            weatherPanel.name = "weatherPanel";
            weatherPanel.width = UltimateEyecandyTool.WIDTH - 3 * UltimateEyecandyTool.SPACING;
            weatherPanel.height = UltimateEyecandyTool.HEIGHT;
            weatherPanel.relativePosition = new Vector3(5, 0);
            weatherPanel.isVisible = false;
            //  Color Management Panel:
            colorManagementPanel = body.AddUIComponent<ColorManagementPanel>();
            colorManagementPanel.name = "colormanagementPanel";
            colorManagementPanel.width = UltimateEyecandyTool.WIDTH - 3 * UltimateEyecandyTool.SPACING;
            colorManagementPanel.height = UltimateEyecandyTool.HEIGHT;
            colorManagementPanel.relativePosition = new Vector3(5, 0);
            colorManagementPanel.isVisible = false;
            //  Presets Panel:
            presetsPanel = body.AddUIComponent<PresetsPanel>();
            presetsPanel.name = "presetsPanel";
            presetsPanel.width = UltimateEyecandyTool.WIDTH - 3 * UltimateEyecandyTool.SPACING;
            presetsPanel.height = UltimateEyecandyTool.HEIGHT;
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
            if (UltimateEyecandyTool.config.outputDebug)
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
                var isActive = (colorManagementPanel._selectedLut.internal_name == UltimateEyecandyTool.currentSettings.color_selectedlut);
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

        //  Toggle main panel and update button state:
        public void Toggle()
        {
            if (instance.isVisible)
            {
                instance.isVisible = false;
                UIMainButton.instance.state = UIButton.ButtonState.Normal;
            }
            else
            {
                instance.isVisible = true;
                UIMainButton.instance.state = UIButton.ButtonState.Focused;
            }
        }
    }
}