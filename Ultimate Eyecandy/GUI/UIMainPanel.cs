using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

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
            width = UIUtils.c_modPanelWidth;
            height = UIUtils.c_modPanelHeight;
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
            panelTabs.size = new Vector2(UIUtils.c_modPanelInnerWidth, UIUtils.c_tabButtonHeight);
            panelTabs.relativePosition = new Vector2(UIUtils.c_spacing, UIUtils.c_titleBarHeight + UIUtils.c_spacing);

            //  Tab Buttons:
            ambientButton = UIUtils.CreateTab(panelTabs, "Ambient", true);
            ambientButton.width = UIUtils.c_tabButtonWidth;
            ambientButton.height = UIUtils.c_tabButtonHeight;
            ambientButton.tooltip = "In this section you can change several world-related settings such as Time of Day, latitude, longitude and sun and ambient light intensity.";
            ambientButton.textScale = 0.9f;

            weatherButton = UIUtils.CreateTab(panelTabs, "Weather");
            weatherButton.width = UIUtils.c_tabButtonWidth;
            weatherButton.height = UIUtils.c_tabButtonHeight;
            weatherButton.tooltip = "In this section you can change several weather-related settings such as rain, snow and fog intensity.";
            weatherButton.textScale = 0.9f;

            colorManagementButton = UIUtils.CreateTab(panelTabs, "LUT");
            colorManagementButton.width = UIUtils.c_tabButtonWidth;
            colorManagementButton.height = UIUtils.c_tabButtonHeight;
            colorManagementButton.tooltip = "In this section you can quickly change the LUT you want to use.";
            colorManagementButton.textScale = 0.9f;

            presetsButton = UIUtils.CreateTab(panelTabs, "Presets");
            presetsButton.width = UIUtils.c_tabButtonWidth;
            presetsButton.height = UIUtils.c_tabButtonHeight;
            presetsButton.tooltip = "In this section you can save your current settings as a Preset, load previously saved Presets, or reset everything to default.";
            presetsButton.textScale = 0.9f;

            //  Main Panel:
            UIPanel body = AddUIComponent<UIPanel>();
            body.width = UIUtils.c_modPanelInnerWidth;
            body.height = UIUtils.c_modPanelInnerHeight;
            //  ScrollRect
            body.relativePosition = new Vector3(5, 36 + 28 + 5);

            //  Section Panels:
            ambientPanel = body.AddUIComponent<AmbientPanel>();
            ambientPanel.name = "ambientPanel";
            body.width = UIUtils.c_modPanelInnerWidth;
            body.height = UIUtils.c_modPanelInnerHeight;

            //  Ambient Panel:
            ambientPanel.width = UIUtils.c_modPanelInnerWidth;
            ambientPanel.height = UIUtils.c_modPanelInnerHeight;
            ambientPanel.relativePosition = Vector3.zero;
            ambientPanel.isVisible = true;
            ambientPanel.todManager = Singleton<DayNightCycleManager>.instance;

            //  Weather Panel:
            weatherPanel = body.AddUIComponent<WeatherPanel>();
            weatherPanel.name = "weatherPanel";
            weatherPanel.width = UIUtils.c_modPanelInnerWidth;
            weatherPanel.height = UIUtils.c_modPanelInnerHeight;
            weatherPanel.relativePosition = Vector3.zero;
            weatherPanel.isVisible = false;

            //  Color Management Panel:
            colorManagementPanel = body.AddUIComponent<ColorManagementPanel>();
            colorManagementPanel.name = "colormanagementPanel";
            colorManagementPanel.width = UIUtils.c_modPanelInnerWidth;
            colorManagementPanel.height = UIUtils.c_modPanelInnerHeight;
            colorManagementPanel.relativePosition = Vector3.zero;
            colorManagementPanel.isVisible = false;

            //  Presets Panel:
            presetsPanel = body.AddUIComponent<PresetsPanel>();
            presetsPanel.name = "presetsPanel";
            presetsPanel.width = UIUtils.c_modPanelInnerWidth;
            presetsPanel.height = UIUtils.c_modPanelInnerHeight;
            presetsPanel.relativePosition = Vector3.zero;
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