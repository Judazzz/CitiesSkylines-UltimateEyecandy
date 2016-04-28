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

        public AmbientPanel ambientPanel;
        public WeatherPanel weatherPanel;
        public ColorManagamentPanel colormanagementPanel;
        public PresetsPanel PresetsPanel;

        public UITabstrip panelTabs;

        public UIButton ambientButton;
        public UIButton weatherButton;
        public UIButton colormanagementButton;
        public UIButton presetsButton;

        public UIButton toggleUltimateEyecandyButton;
        public static UITextureAtlas toggleUltimateEyecandyButtonAtlas = null;
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
            
            //backgroundSprite = "UnlockingPanel2";
            backgroundSprite = "LevelBarBackground";
            isVisible = false;
            canFocus = true;
            isInteractive = true;
            padding = new RectOffset(10, 10, 4, 4);
            width = UltimateEyecandy.SPACING + UltimateEyecandy.WIDTH;
            height = UltimateEyecandy.TITLE_HEIGHT + UltimateEyecandy.TABS_HEIGHT + UltimateEyecandy.HEIGHT + UltimateEyecandy.SPACING;
            relativePosition = new Vector3(10, 60);
            //  
            DebugUtils.Log($"CURRENT FOV: {Camera.main.fieldOfView}");

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
            colormanagementButton = UIUtils.CreateTab(panelTabs, "LUT");
            colormanagementButton.tooltip = "In this section you can quickly change the LUT you want to use.";
            colormanagementButton.textScale = 0.8f;
            presetsButton = UIUtils.CreateTab(panelTabs, "Presets");
            presetsButton.tooltip = "In this section you can save your current settings as a Preset, load previously saved Presets, or reset everything to default.";
            presetsButton.textScale = 0.8f;

            //  Main Panel:
            UIPanel body = AddUIComponent<UIPanel>();
            body.width = UltimateEyecandy.WIDTH;
            body.height = UltimateEyecandy.HEIGHT;
            body.relativePosition = new Vector3(UltimateEyecandy.SPACING, UltimateEyecandy.TITLE_HEIGHT + UltimateEyecandy.TABS_HEIGHT + UltimateEyecandy.SPACING);

            //  Section Panels:
            //  Ambient Panel:
            ambientPanel = body.AddUIComponent<AmbientPanel>();
            ambientPanel.width = UltimateEyecandy.WIDTH - (3 * UltimateEyecandy.SPACING);
            ambientPanel.height = UltimateEyecandy.HEIGHT;
            ambientPanel.relativePosition = new Vector3(5, 0);
            ambientPanel.isVisible = true;
            //  Weather Panel:
            weatherPanel = body.AddUIComponent<WeatherPanel>();
            weatherPanel.width = UltimateEyecandy.WIDTH - 3 * UltimateEyecandy.SPACING;
            weatherPanel.height = UltimateEyecandy.HEIGHT;
            weatherPanel.relativePosition = new Vector3(5, 0);
            weatherPanel.isVisible = false;
            //  Color Management Panel:
            colormanagementPanel = body.AddUIComponent<ColorManagamentPanel>();
            colormanagementPanel.width = UltimateEyecandy.WIDTH - 3 * UltimateEyecandy.SPACING;
            colormanagementPanel.height = UltimateEyecandy.HEIGHT;
            colormanagementPanel.relativePosition = new Vector3(5, 0);
            colormanagementPanel.isVisible = false;
            //  Presets Panel:
            PresetsPanel = body.AddUIComponent<PresetsPanel>();
            PresetsPanel.width = UltimateEyecandy.WIDTH - 3 * UltimateEyecandy.SPACING;
            PresetsPanel.height = UltimateEyecandy.HEIGHT;
            PresetsPanel.relativePosition = new Vector3(5, 0);
            PresetsPanel.isVisible = false;

            //  Tab Button Events:
            ambientButton.eventClick += (c, e) => TabClicked(c, e);
            weatherButton.eventClick += (c, e) => TabClicked(c, e);
            colormanagementButton.eventClick += (c, e) => TabClicked(c, e);
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
                ColorManagamentPanel.instance.lutFastlist.selectedIndex = ColorCorrectionManager.instance.lastSelection;
                var isActive = (ColorManagamentPanel.instance._selectedLut.internal_name == UltimateEyecandy.currentSettings.color_selectedlut);
                ColorManagamentPanel.instance.loadLutButton.isEnabled = (isActive) ? false : true;
                ColorManagamentPanel.instance.loadLutButton.opacity = (isActive) ? 0.5f : 1.0f;
                ColorManagamentPanel.instance.loadLutButton.tooltip = (isActive) ? "LUT selected in list is already active." : "Load LUT selected in list.";
            }
            if (trigger == presetsButton)
            {
                if (PresetsPanel.instance.presetFastlist.selectedIndex < 0)
                {
                    PresetsPanel.instance.updateButtons(true);
                }
                else
                {
                    PresetsPanel.instance.updateButtons(false);
                }
                PresetsPanel.isVisible = true;
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
            if (toggleUltimateEyecandyButtonAtlas == null)
            {
                toggleUltimateEyecandyButtonAtlas = CreateAtlas(UE, size, size, "ToolbarIcon.png", new[]
                                            {
                                                "EyecandyNormalBg",
                                                "EyecandyHoveredBg",
                                                "EyecandyPressedBg",
                                                "EyecandyNormalFg",
                                                "EyecandyHoveredFg",
                                                "EyecandyPressedFg",
                                                "EyecandyUnlockBg",
                                                "EyecandyLogo",
                                                "EyecandyInfoTextBg",
                                            });
            }
            //  Apply custom sprite:
            toggleUltimateEyecandyButton.atlas = toggleUltimateEyecandyButtonAtlas;
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

        public static UITextureAtlas CreateAtlas(string name, int width, int height, string file, string[] spriteNames)
        {
            var tex = new Texture2D(width, height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear,
            };

            var assembly = Assembly.GetExecutingAssembly();
            using (var textureStream = assembly.GetManifestResourceStream(UE + ".Assets." + file))
            {
                var buf = new byte[textureStream.Length];
                textureStream.Read(buf, 0, buf.Length);
                tex.LoadImage(buf);
                tex.Apply(true, false);
            }

            var atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            var material = Instantiate(UIView.Find<UITabstrip>("ToolMode").atlas.material);
            material.mainTexture = tex;

            atlas.material = material;
            atlas.name = name;

            for (var i = 0; i < spriteNames.Length; ++i)
            {
                var uw = 1.0f / spriteNames.Length;

                var sprite = new UITextureAtlas.SpriteInfo
                {
                    name = spriteNames[i],
                    texture = tex,
                    region = new Rect(i * uw, 0, uw, 1),
                };

                atlas.AddSprite(sprite);
            }
            return atlas;
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