using ColossalFramework.UI;
using UnityEngine;

namespace UltimateEyecandy.GUI
{
    public class UIUtils
    {
        // Figuring all this was a pain (no documentation whatsoever)
        // So if your are using it for your mod consider thanking me (SamsamTS)
        // Extended Public Transport UI's code helped me a lot so thanks a lot AcidFire

        public static UIButton CreateButton(UIComponent parent)
        {
            UIButton button = parent.AddUIComponent<UIButton>();

            button.size = new Vector2(90f, 30f);
            button.playAudioEvents = true;
            button.scaleFactor = 0.75f;
            button.textScale = 0.8f;
            //button.normalBgSprite = "TextFieldUnderline"; /*"ButtonMenu";*/
            //button.hoveredBgSprite = "TextFieldPanel"; /*"ButtonMenuHovered";*/
            //button.pressedBgSprite = "TextFieldPanelHovered"; /*"ButtonMenuPressed";*/
            //button.disabledBgSprite = "TextFieldUnderline"; /*"ButtonMenuDisabled";*/
            button.normalBgSprite = "TextFieldUnderline";
            button.hoveredBgSprite = "TextFieldUnderline";
            button.pressedBgSprite = "TextFieldUnderline";
            button.disabledBgSprite = "TextFieldUnderline";
            button.focusedBgSprite = "TextFieldUnderline";
            
            button.textColor = new Color32(255, 255, 255, 255);
            button.hoveredTextColor = new Color32(187, 187, 187, 255);
            button.pressedTextColor = new Color32(0, 0, 0, 255);
            button.focusedTextColor = new Color32(0, 0, 0, 255);
            button.disabledTextColor = new Color32(128, 128, 128, 255);
            button.canFocus = false;
            //  
            return button;
        }

        public static UIButton CreateTab(UIComponent tabstrip, string text, bool isFirst = false) {

            UIButton tab = tabstrip.AddUIComponent<UIButton>();

            tab.size = new Vector2((tabstrip.width / 4), 28f);
            tab.tabStrip = true;
            tab.text = text;
            //tab.normalBgSprite = "SubBarButtonBase";
            //tab.hoveredBgSprite = "SubBarButtonBaseHovered";
            //tab.pressedBgSprite = "SubBarButtonBasePressed";
            //tab.disabledBgSprite = "SubBarButtonBaseDisabled";
            //tab.focusedBgSprite = "SubBarButtonBaseFocused";
            tab.normalBgSprite = "TextFieldUnderline";
            tab.hoveredBgSprite = "TextFieldUnderline";
            tab.pressedBgSprite = "TextFieldUnderline";
            tab.disabledBgSprite = "TextFieldUnderline";
            tab.focusedBgSprite = "TextFieldUnderline";

            tab.textColor = new Color32(255, 255, 255, 255);
            tab.hoveredTextColor = new Color32(187, 187, 187, 255);
            tab.pressedTextColor = new Color32(0, 0, 0, 255);
            tab.focusedTextColor = new Color32(0, 0, 0, 255);
            //  
            return tab;
        }

        public static UISlider CreateSlider(UIPanel parent, float min, float max)
        {
            UIPanel bg = parent.AddUIComponent<UIPanel>();
            bg.name = "sliderPanel";
            bg.autoLayout = false;
            bg.padding = new RectOffset(0, 0, 10, 0);
            bg.size = new Vector2(parent.width - (2 * UltimateEyecandy.SPACING), 20);

            UISlider slider = bg.AddUIComponent<UISlider>();
            slider.autoSize = false;
            slider.area = new Vector4(8, 0, bg.width, 15);
            slider.width = bg.width - 16;
            slider.height = 1;
            slider.relativePosition = new Vector3(8, slider.relativePosition.y + 10);
            slider.backgroundSprite = "SubBarButtonBasePressed";
            slider.fillPadding = new RectOffset(6, 6, 0, 0);

            slider.maxValue = max;
            slider.minValue = min;

            UISprite thumb = slider.AddUIComponent<UISprite>();
            thumb.size = new Vector2(16, 16);
            thumb.position = new Vector2(0, 0);
            thumb.spriteName = "InfoIconBaseHovered";

            slider.value = 0.0f;
            slider.thumbObject = thumb;


            return slider;
        }

        public static UICheckBox CreateCheckBox(UIComponent parent)
        {
            UICheckBox checkBox = parent.AddUIComponent<UICheckBox>();
            checkBox.width = parent.width - (2 * UltimateEyecandy.SPACING);
            checkBox.height = 20f;
            checkBox.relativePosition = new Vector3(5, 21);

            UISprite sprite = checkBox.AddUIComponent<UISprite>();
            sprite.spriteName = "InfoIconBaseHovered";
            sprite.size = new Vector2(16f, 16f);
            sprite.relativePosition = new Vector3(229, 0);

            checkBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)checkBox.checkedBoxObject).spriteName = "InfoIconBaseFocused";
            checkBox.checkedBoxObject.size = new Vector2(16f, 16f);
            checkBox.checkedBoxObject.relativePosition = Vector3.zero;

            checkBox.label = checkBox.AddUIComponent<UILabel>();
            checkBox.autoSize = false;
            checkBox.label.textScale = 0.8f;
            checkBox.label.width = parent.width - 22;
            checkBox.label.relativePosition = new Vector3(0, 3);

            return checkBox;
        }

        public static UICheckBox CreateIconToggle(UIComponent parent, string atlas, string checkedSprite, string uncheckedSprite)
        {
            UICheckBox checkBox = parent.AddUIComponent<UICheckBox>();

            checkBox.width = 35f;
            checkBox.height = 35f;
            checkBox.clipChildren = true;

            UIPanel panel = checkBox.AddUIComponent<UIPanel>();
            panel.backgroundSprite = "IconPolicyBaseRect";
            panel.size = checkBox.size;
            panel.relativePosition = Vector3.zero;

            checkBox.eventCheckChanged += (c, b) =>
            {
                if (checkBox.isChecked)
                    panel.backgroundSprite = "IconPolicyBaseRect";
                else
                    panel.backgroundSprite = "IconPolicyBaseRectDisabled";
                panel.Invalidate();
            };

            checkBox.eventMouseEnter += (c, p) =>
            {
                panel.backgroundSprite = "IconPolicyBaseRectHovered";
            };

            checkBox.eventMouseLeave += (c, p) =>
            {
                if (checkBox.isChecked)
                    panel.backgroundSprite = "IconPolicyBaseRect";
                else
                    panel.backgroundSprite = "IconPolicyBaseRectDisabled";
            };

            UISprite sprite = panel.AddUIComponent<UISprite>();
            sprite.atlas = GetAtlas(atlas);
            sprite.spriteName = uncheckedSprite;
            sprite.size = checkBox.size;
            sprite.relativePosition = Vector3.zero;

            checkBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)checkBox.checkedBoxObject).atlas = sprite.atlas;
            ((UISprite)checkBox.checkedBoxObject).spriteName = checkedSprite;
            checkBox.checkedBoxObject.size = checkBox.size;
            checkBox.checkedBoxObject.relativePosition = Vector3.zero;

            return checkBox;
        }

        public static UITextField CreateTextField(UIComponent parent)
        {
            UITextField textField = parent.AddUIComponent<UITextField>();

            textField.size = new Vector2(90f, 20f);
            textField.padding = new RectOffset(6, 6, 3, 3);
            textField.builtinKeyNavigation = true;
            textField.isInteractive = true;
            textField.readOnly = false;
            textField.horizontalAlignment = UIHorizontalAlignment.Center;
            textField.selectionSprite = "EmptySprite";
            textField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            textField.normalBgSprite = "TextFieldPanelHovered";
            textField.disabledBgSprite = "TextFieldPanel";
            textField.textColor = new Color32(0, 0, 0, 255);
            textField.disabledTextColor = new Color32(0, 0, 0, 128);
            textField.color = new Color32(255, 255, 255, 255);

            return textField;
        }

        public static UIDropDown CreateDropDown(UIComponent parent)
        {
            UIDropDown dropDown = parent.AddUIComponent<UIDropDown>();
            dropDown.size = new Vector2(90f, 30f);
            dropDown.listBackground = "GenericPanelLight";
            dropDown.itemHeight = 30;
            dropDown.itemHover = "ListItemHover";
            dropDown.itemHighlight = "ListItemHighlight";
            dropDown.normalBgSprite = "ButtonMenu";
            dropDown.disabledBgSprite = "ButtonMenuDisabled";
            dropDown.hoveredBgSprite = "ButtonMenuHovered";
            dropDown.focusedBgSprite = "ButtonMenu";
            dropDown.listWidth = 90;
            dropDown.listHeight = 500;
            dropDown.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
            dropDown.popupColor = new Color32(45, 52, 61, 255);
            dropDown.popupTextColor = new Color32(170, 170, 170, 255);
            dropDown.zOrder = 1;
            dropDown.textScale = 0.8f;
            dropDown.verticalAlignment = UIVerticalAlignment.Middle;
            dropDown.horizontalAlignment = UIHorizontalAlignment.Left;
            dropDown.selectedIndex = 0;
            dropDown.textFieldPadding = new RectOffset(8, 0, 8, 0);
            dropDown.itemPadding = new RectOffset(14, 0, 8, 0);

            UIButton button = dropDown.AddUIComponent<UIButton>();
            dropDown.triggerButton = button;
            button.text = "";
            button.size = dropDown.size;
            button.relativePosition = new Vector3(0f, 0f);
            button.textVerticalAlignment = UIVerticalAlignment.Middle;
            button.textHorizontalAlignment = UIHorizontalAlignment.Left;
            button.normalFgSprite = "IconDownArrow";
            button.hoveredFgSprite = "IconDownArrowHovered";
            button.pressedFgSprite = "IconDownArrowPressed";
            button.focusedFgSprite = "IconDownArrowFocused";
            button.disabledFgSprite = "IconDownArrowDisabled";
            button.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            button.horizontalAlignment = UIHorizontalAlignment.Right;
            button.verticalAlignment = UIVerticalAlignment.Middle;
            button.zOrder = 0;
            button.textScale = 0.8f;

            dropDown.eventSizeChanged += new PropertyChangedEventHandler<Vector2>((c, t) =>
            {
                button.size = t; dropDown.listWidth = (int)t.x;
            });

            return dropDown;
        }

        public static UIListBox CreateListBox(UIComponent parent)
        {
            UIListBox listBox = parent.AddUIComponent<UIListBox>();
            listBox.size = new Vector2(90f, 30f);
            listBox.itemHeight = 30;
            listBox.itemHover = "ListItemHover";
            listBox.itemHighlight = "ListItemHighlight";
            listBox.normalBgSprite = "ButtonMenu";
            listBox.disabledBgSprite = "ButtonMenu";
            listBox.hoveredBgSprite = "ButtonMenu";
            listBox.focusedBgSprite = "ButtonMenu";
            listBox.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
            //listBox.popupColor = new Color32(45, 52, 61, 255);
            //listBox.popupTextColor = new Color32(170, 170, 170, 255);
            listBox.zOrder = 1;
            listBox.textScale = 0.85f;
            //listBox.verticalAlignment = UIVerticalAlignment.Middle;
            //listBox.horizontalAlignment = UIHorizontalAlignment.Left;
            //listBox.selectedIndex = 0;
            //listBox.textFieldPadding = new RectOffset(8, 0, 8, 0);
            listBox.itemPadding = new RectOffset(14, 0, 8, 0);

            //UIButton button = listBox.AddUIComponent<UIButton>();
            //button.triggerButton = button;
            //button.text = "";
            //button.size = listBox.size;
            //button.relativePosition = new Vector3(0f, 0f);
            //button.textVerticalAlignment = UIVerticalAlignment.Middle;
            //button.textHorizontalAlignment = UIHorizontalAlignment.Left;
            //button.normalFgSprite = "IconDownArrow";
            //button.hoveredFgSprite = "IconDownArrowHovered";
            //button.pressedFgSprite = "IconDownArrowPressed";
            //button.focusedFgSprite = "IconDownArrowFocused";
            //button.disabledFgSprite = "IconDownArrowDisabled";
            //button.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            //button.horizontalAlignment = UIHorizontalAlignment.Right;
            //button.verticalAlignment = UIVerticalAlignment.Middle;
            //button.zOrder = 0;
            //button.textScale = 0.8f;

            listBox.eventSizeChanged += new PropertyChangedEventHandler<Vector2>((c, t) =>
            {
                listBox.width = (int)t.x;
            });

            return listBox;
        }

        public static void ResizeIcon(UISprite icon, Vector2 maxSize)
        {
            icon.width = icon.spriteInfo.width;
            icon.height = icon.spriteInfo.height;

            if (icon.height == 0) return;

            float ratio = icon.width / icon.height;

            if (icon.width > maxSize.x)
            {
                icon.width = maxSize.x;
                icon.height = maxSize.x / ratio;
            }

            if (icon.height > maxSize.y)
            {
                icon.height = maxSize.y;
                icon.width = maxSize.y * ratio;
            }
        }

        public static UITextureAtlas[] s_atlases;

        public static UITextureAtlas GetAtlas(string name)
        {
            if (s_atlases == null)
                s_atlases = Resources.FindObjectsOfTypeAll(typeof(UITextureAtlas)) as UITextureAtlas[];

            for (int i = 0; i < s_atlases.Length; i++)
            {
                if (s_atlases[i].name == name)
                    return s_atlases[i];
            }

            return UIView.GetAView().defaultAtlas;
        }

        public static void TruncateLabel(UILabel label, float maxWidth)
        {
            label.autoSize = true;
            while (label.width > maxWidth)
            {
                label.text = label.text.Substring(0, label.text.Length - 4) + "...";
                label.autoSize = true;
            }
        }

        public static UIPanel CreateFormElement(UIComponent parent, string position)
        {
            UIPanel panel = parent.AddUIComponent<UIPanel>();

            panel.height = 50;// 65;
            panel.width = parent.width;
            panel.padding = new RectOffset((int)UltimateEyecandy.SPACING, (int)UltimateEyecandy.SPACING, 0, 15);
            panel.autoLayout = true;
            panel.autoLayoutDirection = LayoutDirection.Vertical;

            if (position == "top")
            {
                panel.height = 65;
                panel.padding = new RectOffset((int)UltimateEyecandy.SPACING, (int)UltimateEyecandy.SPACING, 20, 15);
                panel.AlignTo(parent, UIAlignAnchor.TopLeft);
                panel.relativePosition = new Vector3(0, 0);
            }
            else if (position == "bottom")
            {
                panel.height = 40;
                panel.AlignTo(parent, UIAlignAnchor.BottomLeft);
                panel.relativePosition = new Vector3(0, 305);
            }
            else
            {
                panel.padding = new RectOffset((int)UltimateEyecandy.SPACING, (int)UltimateEyecandy.SPACING, 0, 0);
            }

            return panel;
        }

        //public static UIColorField CreateColorField(UIComponent parent)
        //{
        //    //UIColorField colorField = parent.AddUIComponent<UIColorField>();
        //    // Creating a ColorField from scratch is tricky. Cloning an existing one instead.
        //    // Probably doesn't work when on main menu screen and such as no ColorField exists.
        //    UIColorField colorField = Object.Instantiate<GameObject>(Object.FindObjectOfType<UIColorField>().gameObject).GetComponent<UIColorField>();
        //    parent.AttachUIComponent(colorField.gameObject);

        //    // Reset most everything
        //    colorField.anchor = UIAnchorStyle.Left | UIAnchorStyle.Top;
        //    colorField.arbitraryPivotOffset = new Vector2(0, 0);
        //    colorField.autoSize = false;
        //    colorField.bringTooltipToFront = true;
        //    colorField.builtinKeyNavigation = true;
        //    colorField.canFocus = true;
        //    colorField.enabled = true;
        //    colorField.isEnabled = true;
        //    colorField.isInteractive = true;
        //    colorField.isLocalized = false;
        //    colorField.isTooltipLocalized = false;
        //    colorField.isVisible = true;
        //    colorField.pivot = UIPivotPoint.TopLeft;
        //    colorField.useDropShadow = false;
        //    colorField.useGradient = false;
        //    colorField.useGUILayout = true;
        //    colorField.useOutline = false;
        //    colorField.verticalAlignment = UIVerticalAlignment.Top;

        //    colorField.size = new Vector2(40f, 26f);
        //    colorField.normalBgSprite = "ColorPickerOutline";
        //    colorField.hoveredBgSprite = "ColorPickerOutlineHovered";
        //    colorField.selectedColor = Color.black;
        //    colorField.pickerPosition = UIColorField.ColorPickerPosition.RightAbove;

        //    return colorField;
        //}


        public static void DestroyDeeply(UIComponent component)
        {
            if (component == null) return;

            UIComponent[] children = component.GetComponentsInChildren<UIComponent>();

            if (children != null && children.Length > 0)
            {
                for (int i = 0; i < children.Length; i++)
                {
                    if (children[i].parent == component)
                        DestroyDeeply(children[i]);
                }
            }

            GameObject.Destroy(component);
        }
    }
}
