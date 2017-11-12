using ColossalFramework.UI;
using UnityEngine;

namespace UltimateEyecandy.GUI
{
    public class UILutItem : UIPanel, IUIFastListRow
    {
        private UILabel _name;
        private LutList.Lut _lut;

        public LutList.Lut lut;

        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            if (_name == null) return;
        }

        private void SetupControls()
        {
            if (_name != null) return;

            isVisible = true;
            isInteractive = true;
            width = parent.width;
            height = UIUtils.c_fastListRowHeight;

            _name = AddUIComponent<UILabel>();
            _name.name = "LutName";
            _name.relativePosition = new Vector3(5, 9);
            _name.textColor = new Color32(238, 238, 238, 255);
            _name.textScale = 0.85f;
        }

        protected override void OnMouseDown(UIMouseEventParameter p)
        {
            p.Use();

            base.OnMouseDown(p);
        }

        protected override void OnMouseEnter(UIMouseEventParameter p)
        {
            base.OnMouseEnter(p);
        }
        
        protected override void OnMouseLeave(UIMouseEventParameter p)
        {
            base.OnMouseLeave(p);
        }

        #region IUIFastListRow implementation
        public void Display(object data, bool isRowOdd)
        {
            SetupControls();

            _lut = data as LutList.Lut;
            _name.text = _lut.name;
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
