using _EventSystem.CustomEvents;
using Relics.ScriptableObject_RelicEffect;
using Resources.ToolTip.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Relics
{
    /// <summary>
    /// Class that can be attached to an Object to show a Relic
    /// </summary>
    public class RelicInfo: InfoBehaviour
    {
        public RelicSo Relic { get; private set; }
        [SerializeField] private Image icon;
        
        [FormerlySerializedAs("InfoTooltip_ON")]
        [Header("Tooltip Events")] 
        [SerializeField] private InfoEvent infoTooltipOn;
        [FormerlySerializedAs("InfoTooltip_OFF")]
        [SerializeField] private VoidEvent infoTooltipOff;

        public void CreateRelic(RelicSo _relic)
        {
            Relic = _relic;
        }

        #region InfoBehaviour
        
        public override void OnPointerEnter(PointerEventData _eventData)
        {
            if (Input.GetMouseButton(0)) return;
            infoTooltipOn.Raise(this);
        }

        public override void OnPointerExit(PointerEventData _eventData)
        {
            if (Input.GetMouseButton(0)) return;
            infoTooltipOff.Raise();
        }
        
        public override string GetInfoMain()
        {
            return ColouredName();
        }

        public override string GetInfoLeft()
        {
            string _str = "";
            if (Relic.GridEffect != null) _str += Relic.GridEffect.InfoEffect() + "\n";
            if (Relic.Effect != null) _str += Relic.Effect.InfoEffect() + "\n";
            return _str;
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