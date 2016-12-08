using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.Plugins;
using System.Linq;
using UnityEngine;

namespace UltimateEyecandy.GUI
{
    public class UIMainButton : UIButton
    {
        public override void Start()
        {
            const int buttonSize = 36;
            UITextureAtlas toggleButtonAtlas = null;
            string UE = "UltimateEyecandy";

            //  Positioned relative to Freecamera Button:
            var freeCameraButton = UIView.GetAView().FindUIComponent<UIButton>("Freecamera");
            verticalAlignment = UIVerticalAlignment.Middle;

            if (UltimateEyecandy.config.buttonPos.x == -9999)
            {
                absolutePosition = new Vector2(freeCameraButton.absolutePosition.x - (3 * buttonSize) - 5, freeCameraButton.absolutePosition.y);
            }
            else
            {
                absolutePosition = UltimateEyecandy.config.buttonPos; //new Vector2(UltimateEyecandy.config.buttonPosX, UltimateEyecandy.config.buttonPosY);
            }
            //  
            size = new Vector2(36f, 36f);
            playAudioEvents = true;
            tooltip = "Ultimate Eyecandy " + ModInfo.version;
            //  Create custom atlas:
            if (toggleButtonAtlas == null)
            {
                toggleButtonAtlas = UIUtils.CreateAtlas(UE, buttonSize, buttonSize, "ToolbarIcon.png", new[]
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
            atlas = toggleButtonAtlas;
            normalFgSprite = "EyecandyNormalBg";
            normalBgSprite = null;
            hoveredFgSprite = "EyecandyHoveredBg";
            hoveredBgSprite = "EyecandyHoveredFg";
            pressedFgSprite = "EyecandyPressedBg";
            pressedBgSprite = "EyecandyPressedFg";
            focusedFgSprite = "EyecandyPressedBg";
            focusedBgSprite = "EyecandyPressedFg";
            //  
            if (UltimateEyecandy.config.outputDebug)
            {
                DebugUtils.Log("Main button created.");
            }
        }

        protected override void OnClick(UIMouseEventParameter p)
        {
            if (p.buttons.IsFlagSet(UIMouseButton.Left))
            {
                ModMainPanel.instance.ToggleMainPanel();
            }

            base.OnClick(p);
        }

        private bool dragging = false;
        protected override void OnMouseDown(UIMouseEventParameter p)
        {
            if (p.buttons.IsFlagSet(UIMouseButton.Right))
            {
                dragging = true;
            }
            base.OnMouseDown(p);
        }

        protected override void OnMouseUp(UIMouseEventParameter p)
        {
            if (p.buttons.IsFlagSet(UIMouseButton.Right))
            {
                dragging = false;
            }
            base.OnMouseUp(p);
        }

        protected override void OnMouseMove(UIMouseEventParameter p)
        {
            if (p.buttons.IsFlagSet(UIMouseButton.Right))
            {
                var ratio = UIView.GetAView().ratio;
                position = new Vector3(position.x + (p.moveDelta.x * ratio), position.y + (p.moveDelta.y * ratio), position.z);
                //  
                //UltimateEyecandy.config.buttonPosX = (int)(position.x + (p.moveDelta.x * ratio));
                //UltimateEyecandy.config.buttonPosY = (int)(position.y + (p.moveDelta.y * ratio));
                UltimateEyecandy.config.buttonPos = absolutePosition;
                UltimateEyecandy.SaveConfig();
                //  
                if (UltimateEyecandy.config.outputDebug)
                {
                    DebugUtils.Log($"Button position changed to x = {absolutePosition}.");
                }
            }
            base.OnMouseMove(p);
        }

        //private Vector3 m_deltaPos;
        //protected override void OnMouseDown(UIMouseEventParameter p)
        //{
        //    if (p.buttons.IsFlagSet(UIMouseButton.Right))
        //    {
        //        Vector3 mousePos = Input.mousePosition;
        //        mousePos.y = m_OwnerView.fixedHeight - mousePos.y;

        //        m_deltaPos = absolutePosition - mousePos;
        //        BringToFront();
        //    }
        //}

        //protected override void OnMouseMove(UIMouseEventParameter p)
        //{
        //    if (p.buttons.IsFlagSet(UIMouseButton.Right))
        //    {
        //        Vector3 mousePos = Input.mousePosition;
        //        mousePos.y = m_OwnerView.fixedHeight - mousePos.y;

        //        absolutePosition = mousePos + m_deltaPos;
        //        UltimateEyecandy.config.buttonPosX = (int)absolutePosition.x;
        //        UltimateEyecandy.config.buttonPosY = (int)absolutePosition.y;
        //        UltimateEyecandy.SaveConfig();
        //        //  
        //        if (UltimateEyecandy.config.outputDebug)
        //        {
        //            DebugUtils.Log($"Button position changed to x = {(int)absolutePosition.x}, y = {(int)absolutePosition.y}.");
        //        }
        //    }
        //}
    }
}