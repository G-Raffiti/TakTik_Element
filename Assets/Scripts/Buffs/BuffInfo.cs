using System.Collections.Generic;
using _EventSystem.CustomEvents;
using DataBases;
using Resources.ToolTip.Scripts;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Buffs
{
    public class BuffInfo : InfoBehaviour
    {
        public Buff Buff { get; set; }
        public Unit Unit { get; set; }
        
        [Header("References Unity")]
        [SerializeField] private Image icon;
        [FormerlySerializedAs("Duration")]
        [SerializeField] private TextMeshProUGUI duration;
        [SerializeField] private List<Image> frame;
        [SerializeField] private ColorSet colorSet;
        
        [FormerlySerializedAs("InfoTooltip_ON")]
        [Header("Tooltip Events")] 
        [SerializeField] private InfoEvent infoTooltipOn;
        [FormerlySerializedAs("InfoTooltip_OFF")]
        [SerializeField] private VoidEvent infoTooltipOff;

        public override string GetInfoMain()
        {
            return $"{ColouredName()} {Buff.duration}<sprite name=Duration>";
        }

        public override string GetInfoLeft()
        {
            if (Unit != null)
                return Buff.InfoOnUnit(Buff, Unit);
            return Buff.InfoBuff();
        }

        public override string GetInfoRight()
        {
            return "";
        }

        public override string GetInfoDown()
        {
            return "";
        }

        public override Sprite GetIcon()
        {
            return Buff.Effect.OnFloorSprite;
        }

        public override string ColouredName()
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Buff.Effect.Element.TextColour);
            return $"<color=#{_hexColor}>{Buff.Effect.Name}</color>";
        }

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

        public override void DisplayIcon()
        {
            icon.sprite = GetIcon();
            frame.ForEach(_i => _i.color = Buff.Effect.Type == EBuff.Buff
                ? colorSet.GetColors()[EColor.Buff]
                : colorSet.GetColors()[EColor.Debuff]);
            duration.text = Buff.Effect.IsDefinitive 
                ? $"<sprite name=Infinity>" 
                : $"{Buff.duration}";
        }

        public BuffInfo Initialize(Buff _buff, Unit _unit)
        {
            Buff = _buff;
            Unit = _unit;
            return this;
        }
    }
}