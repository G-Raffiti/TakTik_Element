using UnityEngine;

namespace Resources.ToolTip.Scripts
{
    public interface IInfo
    {
        public abstract string GetInfoMain();
        public abstract string GetInfoLeft();
        public abstract string GetInfoRight();
        public abstract string GetInfoDown();
        public abstract Sprite GetIcon();
        public abstract string ColouredName();
    }
}