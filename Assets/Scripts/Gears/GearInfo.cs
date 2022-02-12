using _EventSystem.CustomEvents;
using DataBases;
using Resources.ToolTip.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gears
{
    public class GearInfo : InfoBehaviour
    {
        public Gear Gear { get; set; }
        
        [Header("Tooltip Events")] 
        [SerializeField] private GearInfoEvent GearTooltip_ON;
        [SerializeField] private VoidEvent GearTooltip_OFF;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            GearTooltip_ON.Raise(this);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            GearTooltip_OFF.Raise();
        }


        public override string GetInfoMain()
        {
            return $"{Gear.GearSO.Rarity.Name}\n{Gear.GearSO.Type}";
        }

        public override string GetInfoLeft()
        {
            string ret = "Stats:\n";
            ret += Gear.GearSO.MainStat.ToString();
            Gear.Affixes.ForEach(affix => ret += $"\n{affix.ToString()}");
            return ret;
        }

        public override string GetInfoRight()
        {
            string ret = "\n(implicit)";
            Gear.Affixes.ForEach(affix => ret += $"\n{affix.TierRangeToString()}");
            return ret;
        }

        public override string GetInfoDown()
        {
            return Gear.GearSO.SpecialEffect != null ? Gear.GearSO.SpecialEffect.InfoEffect() : "";
        }

        public override Sprite GetIcon()
        {
            return Gear.GearSO.Icon;
        }

        public override string ColouredName()
        {
            return $"<color={ColorSet.HexColor(Gear.GearSO.Rarity.TextColour)}>{Gear.GearSO.Name}</color>";
        }
    }
}