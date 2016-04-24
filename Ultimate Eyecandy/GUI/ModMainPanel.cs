using System;
using System.Reflection;
using ColossalFramework.UI;
using UnityEngine;

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

        private const float WIDTH = 270;
        private const float HEIGHT = 350;
        private const float SPACING = 5;
        private const float TITLE_HEIGHT = 36;
        private const float TABS_HEIGHT = 28;

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
            
            backgroundSprite = "UnlockingPanel2";
            isVisible = false;
            canFocus = true;
            isInteractive = true;
            padding = new RectOffset(10, 10, 4, 4);
            width = SPACING + WIDTH;
            height = TITLE_HEIGHT + HEIGHT + TABS_HEIGHT + SPACING;
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
            panelTabs.relativePosition = new Vector2(SPACING, TITLE_HEIGHT + SPACING);
            panelTabs.size = new Vector2(WIDTH, TABS_HEIGHT);
            panelTabs.padding = new RectOffset(2, 2, 2, 2);
            //  Tab Buttons:
            ambientButton = UIUtils.CreateTab(panelTabs, "Ambient");
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

        public void AddGuiToggle()
        {
            const int size = 36;
            //  Position button to the left of Freecamera Button:
            var freeCameraButton = UIView.GetAView().FindUIComponent<UIButton>("Freecamera");
            toggleUltimateEyecandyButton = UIView.GetAView().FindUIComponent<UIPanel>("InfoPanel").AddUIComponent<UIButton>();
            toggleUltimateEyecandyButton.verticalAlignment = UIVerticalAlignment.Middle;
            toggleUltimateEyecandyButton.relativePosition = new Vector3(freeCameraButton.absolutePosition.x - 42, freeCameraButton.relativePosition.y);
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
            //toggleUltimateEyecandyButton.normalBgSprite = "EyecandyNormalFg";
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
    }
}