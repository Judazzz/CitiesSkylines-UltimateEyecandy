using System.Linq;
using ICities;
using System;
using ColossalFramework;
using UnityEngine;
using ColossalFramework.UI;

namespace UltimateEyecandy
{
 
    public class Loading : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            ModPanels.Initialize();
            AddGuiToggle();
        }

        public void AddGuiToggle()
        {

            UIMultiStateButton zoomButton = GameObject.Find("ZoomButton").GetComponent<UIMultiStateButton>();
            UIComponent bottomBar = zoomButton.parent;
            UIButton toggle = UIUtils.CreateButton(bottomBar);

            toggle.area = new Vector4(108, 24, 38, 38);
            toggle.playAudioEvents = true;
            toggle.normalBgSprite = "OptionBase";
            toggle.focusedBgSprite = "OptionBaseFocus";
            toggle.hoveredBgSprite = "OptionBaseHover";
            toggle.pressedBgSprite = "OptionBasePressed";
            toggle.tooltip = "Light Control";
            toggle.normalFgSprite = "InfoIconEntertainmentDisabled";
            toggle.scaleFactor = 0.75f;

            toggle.eventClicked += (UIComponent component, UIMouseEventParameter eventParam) =>
            {
                ModPanels.instance.Toggle();
            };
           
        }

    }
}
