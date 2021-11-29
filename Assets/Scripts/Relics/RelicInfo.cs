using Resources.ToolTip.Scripts;
using Skills.ScriptableObject_RelicEffect;
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
            if (Relic.GridEffect != null) str += Relic.GridEffect.InfoEffect();
            if (Relic.Effect != null) str += Relic.Effect.InfoEffect();
            return str;
        }

        public override string GetInfoRight()
        {
            string str = "";
            foreach (RelicEffect _effect in Relic.RelicEffects)
            {
                str += _effect.InfoEffect(Relic);
            }
            return str;
        }

        public override string GetInfoDown()
        {
            return "";
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