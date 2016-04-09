using UnityEngine;
using ColossalFramework.UI;
using UltimateEyecandy.GUI;

namespace UltimateEyecandy
{
    public class UIPresetItem : UIPanel, IUIFastListRow
    {
        private UILabel m_name;
        private UILabel m_size;

        private Configuration.Preset m_preset;
        
        public Configuration.Preset preset;

        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            if (m_name == null) return;

            m_size.relativePosition = new Vector3(width - 35f, 5);
        }

        private void SetupControls()
        {
            if (m_name != null) return;

            isVisible = true;
            isInteractive = true;
            width = parent.width;
            height = 30;

            m_name = AddUIComponent<UILabel>();
            m_name.relativePosition = new Vector3(5, 5);
            m_name.textColor = new Color32(170, 170, 170, 255);

            m_size = AddUIComponent<UILabel>();
            m_size.width = 30;
            m_size.textAlignment = UIHorizontalAlignment.Center;
            m_size.textColor = new Color32(170, 170, 170, 255);
            m_size.relativePosition = new Vector3(width - 35f, 5);
        }

        protected override void OnMouseDown(UIMouseEventParameter p)
        {
            p.Use();
            //UIThemeManager.instance.ChangeUpgradeBuilding(m_preset);

            base.OnMouseDown(p);
        }

        protected override void OnMouseEnter(UIMouseEventParameter p)
        {
            base.OnMouseEnter(p);
            //UIThemeManager.instance.buildingPreview.Show(m_preset);
        }


        protected override void OnMouseLeave(UIMouseEventParameter p)
        {
            base.OnMouseLeave(p);
            //UIThemeManager.instance.buildingPreview.Show(UIThemeManager.instance.selectedBuilding);
        }

        #region IUIFastListRow implementation
        public void Display(object data, bool isRowOdd)
        {
            SetupControls();

            m_preset = data as Configuration.Preset;
            m_name.text = m_preset.name;

            //UIUtils.TruncateLabel(m_name, width - 40);
            //m_size.text = m_preset.sizeAsString;

            backgroundSprite = null;
        }

        public void Select(bool isRowOdd)
        {
            backgroundSprite = "ListItemHighlight";
            color = new Color32(255, 255, 255, 255);
        }

        public void Deselect(bool isRowOdd)
        {
            backgroundSprite = null;
        }
        #endregion
    }
}
