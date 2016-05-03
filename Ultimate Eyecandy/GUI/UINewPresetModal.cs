using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace UltimateEyecandy.GUI
{
    public class UINewPresetModal : UIPanel
    {
        private UIModalTitleBar m_title;
        private UITextField m_name;
        private UIButton m_ok;
        private UIButton m_cancel;

        private static UINewPresetModal _instance;

        public static UINewPresetModal instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = UIView.GetAView().AddUIComponent(typeof(UINewPresetModal)) as UINewPresetModal;
                }
                return _instance;
            }
        }

        public override void Start()
        {
            base.Start();

            backgroundSprite = "LevelBarBackground";
            isVisible = false;
            canFocus = true;
            isInteractive = true;
            width = 250;
            height = 130;

            // Title Bar
            m_title = AddUIComponent<UIModalTitleBar>();
            m_title.title = "Create New Preset";
            m_title.isModal = true;
            m_title.relativePosition = new Vector3(10, 5);

            // Name
            UILabel name = AddUIComponent<UILabel>();
            name.height = 30;
            name.text = "Preset name:";
            name.textScale = 0.85f;
            name.relativePosition = new Vector3(10, m_title.height);

            m_name = UIUtils.CreateTextField(this);
            m_name.normalBgSprite = "TextFieldUnderline";
            m_name.width = width - 20;
            m_name.height = 25;
            m_name.padding = new RectOffset(6, 6, 6, 6);
            m_name.textColor = new Color32(222, 222, 222, 255);
            m_name.textScale = 0.8f;
            m_name.tooltip = "Please enter a Preset name";
            m_name.relativePosition = new Vector3(10, name.relativePosition.y + name.height + 5);

            m_name.Focus();
            m_name.eventTextChanged += (c, s) =>
            {
                //  Overwrite existing or create new:
                m_ok.text = (!s.IsNullOrWhiteSpace() && UltimateEyecandy.GetPresetByName(s) == null) ? "Create" : "Overwrite";
                m_name.tooltip = (UltimateEyecandy.GetPresetByName(s) != null) ? $"Preset '{s}' already exists: it will be overwritten!" : "Please enter a Preset name";
                m_ok.isEnabled = !s.IsNullOrWhiteSpace();
            };

            m_name.eventTextSubmitted += (c, s) =>
            {
                if (m_ok.isEnabled && Input.GetKey(KeyCode.Return)) m_ok.SimulateClick();
            };

            // Ok
            m_ok = UIUtils.CreateButton(this);
            m_ok.text = "Create";
            //  Overwrite existing instead of blocking?
            m_ok.isEnabled = false;
            m_ok.relativePosition = new Vector3(10, m_name.relativePosition.y + m_name.height + 5);

            m_ok.eventClick += (c, p) =>
            {
                if (UltimateEyecandy.GetPresetByName(m_name.text) != null) {
                    ConfirmPanel.ShowModal("Overwrite Preset", "Are you sure you want to overwrite Preset '" + m_name.text + "'?", (d, i) => {
                        if (i == 1)
                        {
                            UltimateEyecandy.CreatePreset(m_name.text, true);
                            UIView.PopModal();
                            Hide();
                        }
                    });
                }
                else {
                    UltimateEyecandy.CreatePreset(m_name.text, false);
                    UIView.PopModal();
                    Hide();
                }
            };

            // Cancel
            m_cancel = UIUtils.CreateButton(this);
            m_cancel.text = "Cancel";
            m_cancel.relativePosition = new Vector3(width - m_cancel.width - 10, m_ok.relativePosition.y);

            m_cancel.eventClick += (c, p) =>
            {
                UIView.PopModal();
                Hide();
            };

            height = m_cancel.relativePosition.y + m_cancel.height + 5;
            relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));

            isVisible = true;
        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();

            UIComponent modalEffect = GetUIView().panelsLibraryModalEffect;

            if (isVisible)
            {
                m_name.text = "";
                m_name.Focus();

                if (modalEffect != null)
                {
                    modalEffect.Show(false);
                    ValueAnimator.Animate("NewThemeModalEffect", delegate (float val)
                    {
                        modalEffect.opacity = val;
                    }, new AnimatedFloat(0f, 1f, 0.7f, EasingType.CubicEaseOut));
                }
            }
            else if (modalEffect != null)
            {
                ValueAnimator.Animate("NewThemeModalEffect", delegate (float val)
                {
                    modalEffect.opacity = val;
                }, new AnimatedFloat(1f, 0f, 0.7f, EasingType.CubicEaseOut), delegate
                {
                    modalEffect.Hide();
                });
            }
        }

        protected override void OnKeyDown(UIKeyEventParameter p)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                p.Use();
                UIView.PopModal();
                Hide();
            }

            base.OnKeyDown(p);
        }
    }
}
