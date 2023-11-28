using _EventSystem.CustomEvents;
using DataBases;
using Resources.ToolTip.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Gears
{
    public class GearInfo : InfoBehaviour
    {
        public Gear Gear { get; set; }
        
        [FormerlySerializedAs("GearTooltip_ON")]
        [Header("Tooltip Events")] 
        [SerializeField] private GearInfoEvent gearTooltipOn;
        [FormerlySerializedAs("GearTooltip_OFF")]
        [SerializeField] private VoidEvent gearTooltipOff;

        public override void OnPointerEnter(PointerEventData _eventData)
        {
            if (Input.GetMouseButton(0)) return;
            gearTooltipOn.Raise(this);
        }

        public override void OnPointerExit(PointerEventData _eventData)
        {
            if (Input.GetMouseButton(0)) return;
            gearTooltipOff.Raise();
        }


        public override string GetInfoMain()
        {
            return $"{Gear.GearSo.Rarity.Name}\n{Gear.GearSo.Type}";
        }

        public override string GetInfoLeft()
        {
            string _ret = "Stats:\n";
            _ret += Gear.GearSo.MainStat.ToString();
            Gear.Affixes.ForEach(_affix => _ret += $"\n{_affix.ToString()}");
            return _ret;
        }

        public override string GetInfoRight()
        {
            string _ret = "\n(implicit)";
            Gear.Affixes.ForEach(_affix => _ret += $"\n{_affix.TierRangeToString()}");
            return _ret;
        }

        public override string GetInfoDown()
        {
            return Gear.GearSo.SpecialEffect != null ? Gear.GearSo.SpecialEffect.InfoEffect() : "";
        }

        public override Sprite GetIcon()
        {
            return Gear.GearSo.Icon;
        }

        public override string ColouredName()
        {
            return $"<color={ColorSet.HexColor(Gear.GearSo.Rarity.TextColour)}>{Gear.GearSo.Name}</color>";
        }
    }
}