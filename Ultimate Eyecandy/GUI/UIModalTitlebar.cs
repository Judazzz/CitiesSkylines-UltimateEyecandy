using UnityEngine;
using ColossalFramework.UI;

namespace UltimateEyecandy.GUI
{
    public class UIModalTitleBar : UIPanel
    {
        private UISprite m_icon;
        private UILabel m_title;
        private UIButton m_close;
        private UIDragHandle m_drag;

        public bool isModal = false;

        public string iconSprite
        {
            get { return m_icon.spriteName; }
            set
            {
                if (m_icon == null) SetupControls();
                m_icon.spriteName = value;

                if (m_icon.spriteInfo != null)
                {
                    UIUtils.ResizeIcon(m_icon, new Vector2(30, 30));
                    m_icon.relativePosition = new Vector3(10, 5);
                }
            }
        }

        public UIButton closeButton
        {
            get { return m_close; }
        }

        public string title
        {
            get { return m_title.text; }
            set
            {
                if (m_title == null) SetupControls();
                m_title.text = value;
            }
        }

        private void SetupControls()
        {
            width = parent.width;
            height = 40;
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            relativePosition = Vector3.zero;

            m_drag = AddUIComponent<UIDragHandle>();
            m_drag.width = width - 50;
            m_drag.height = height;
            m_drag.relativePosition = Vector3.zero;
            m_drag.target = parent;

            //m_icon = AddUIComponent<UISprite>();
            //m_icon.spriteName = iconSprite;
            //m_icon.relativePosition = new Vector3(10, 5);
            //m_icon.isInteractive = false;

            m_title = AddUIComponent<UILabel>();
            m_title.relativePosition = new Vector3(-5, 7);
            m_title.text = title;
            m_title.isInteractive = false;

            m_close = AddUIComponent<UIButton>();
            m_close.relativePosition = new Vector3(width - 45, -2);
            m_close.normalBgSprite = "buttonclose";
            m_close.hoveredBgSprite = "buttonclosehover";
            m_close.pressedBgSprite = "buttonclosepressed";
            m_close.eventClick += (component, param) =>
            {
                if (isModal)
                    UIView.PopModal();
                parent.Hide();
            };
        }
    }
}