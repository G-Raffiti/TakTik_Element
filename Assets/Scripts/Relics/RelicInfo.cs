using Relics.ScriptableObject_RelicEffect;
using Resources.ToolTip.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Relics
{
    /// <summary>
    /// Class that can be attached to an Object to show a Relic
    /// </summary>
    public class RelicInfo: InfoBehaviour
    {
        public RelicSO Relic { get; private set; }

        public void CreateRelic(RelicSO _relic)
        {
            Relic = _relic;
        }

        #region InfoBehaviour
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            TooltipOn.Raise(this);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            TooltipOff.Raise();
        }
        
        public override string GetInfoMain()
        {
            return ColouredName();
        }

        public override string GetInfoLeft()
        {
            string str = "";
            if (Relic.GridEffect != null) str += Relic.GridEffect.InfoEffect() + "\n";
            if (Relic.Effect != null) str += Relic.Effect.InfoEffect() + "\n";
            return str;
        }

        public override string GetInfoRight()
        {
            return "";
        }

        public override string GetInfoDown()
        {
            return $"{Relic.Description}\n\n{Relic.Flavour}";
        }

        public override Sprite GetIcon()
        {
            return Relic.Icon;
        }

        public override string ColouredName()
        {
            return $"<size=35>{Relic.Name}";
        }

        #endregion
    }
}