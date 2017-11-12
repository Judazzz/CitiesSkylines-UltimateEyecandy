using ColossalFramework.UI;
using UnityEngine;

namespace UltimateEyecandy.GUI
{
    public class UIMainTitleBar : UIPanel
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
            width = UIUtils.c_titleBarLabelWidth;
            height = UIUtils.c_titleBarLabelHeight;
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            relativePosition = Vector3.zero;

            m_drag = AddUIComponent<UIDragHandle>();
            m_drag.width = width;
            m_drag.height = height;
            m_drag.relativePosition = Vector3.zero;
            m_drag.target = parent;

            m_title = AddUIComponent<UILabel>();
            m_title.width = UIUtils.c_titleBarLabelWidth;
            m_title.relativePosition = new Vector3(UIUtils.c_titleBarLabelXPos, 13);
            m_title.text = Mod.version;
            m_title.textScale = 0.9f;
            m_title.isInteractive = false;

            m_close = AddUIComponent<UIButton>();
            m_close.size = new Vector2(17, 17);
            m_close.relativePosition = new Vector3(UIUtils.c_titleBarCloseButtonXPos, 12);
            m_close.normalBgSprite = "IconError";
            m_close.hoveredBgSprite = "IconError";
            m_close.pressedBgSprite = "IconError";
            m_close.eventClick += (component, param) =>
            {
                if (isModal)
                    UIView.PopModal();
                parent.Hide();
            };
        }
    }
}