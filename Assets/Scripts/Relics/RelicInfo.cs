using _EventSystem.CustomEvents;
using Relics.ScriptableObject_RelicEffect;
using Resources.ToolTip.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Relics
{
    /// <summary>
    /// Class that can be attached to an Object to show a Relic
    /// </summary>
    public class RelicInfo: InfoBehaviour
    {
        public RelicSO Relic { get; private set; }
        [SerializeField] private Image icon;
        
        [Header("Tooltip Events")] 
        [SerializeField] private InfoEvent InfoTooltip_ON;
        [SerializeField] private VoidEvent InfoTooltip_OFF;

        public void CreateRelic(RelicSO _relic)
        {
            Relic = _relic;
        }

        #region InfoBehaviour
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            InfoTooltip_ON.Raise(this);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            InfoTooltip_OFF.Raise();
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
            return $"{Relic.Name}";
        }

        public override void DisplayIcon()
        {
            icon.sprite = GetIcon();
        }

        #endregion
    }
}