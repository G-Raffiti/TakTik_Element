using System.Collections.Generic;
using System.Linq;
using Resources.ToolTip.Scripts;
using Stats;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface
{
    public class PersonalInventory : InfoBehaviour
    {
        [SerializeField] private ColorSet colorSet;
        [SerializeField] private Image icon;
        [SerializeField] private Image health;
        [SerializeField] private List<Transform> slots;
        private BattleStats BattleStats;
        private BattleStats baseStats;
        private BattleStats total;
        
        public Hero Hero { get; private set; }

        public List<Transform> Slots => slots;

        public void Initialize(Hero _hero)
        {
            Hero = _hero;
            
            baseStats = _hero.BattleStats;
            BattleStats = new BattleStats(baseStats + Hero.Inventory.GearStats());
            total = BattleStats;
            BattleStats.HP = _hero.ActualHP;
            
            icon.sprite = Hero.UnitSprite;
            health.GetComponent<Image>().fillAmount = BattleStats.HP / (float)total.HP;
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            TooltipOn.Raise(this);
        }
        
        public override string GetInfoMain()
        {
            return ColouredName();
        }

        public override string GetInfoLeft()
        {
            string str = "";
            str += $"<sprite name=AP> <color={colorSet.HexColor(EAffix.AP)}>{(int)total.AP}</color>    ";
            str += $"<sprite name=MP> <color={colorSet.HexColor(EAffix.MP)}>{(int)total.MP}</color>\n";
            str += $"<sprite name=HP> <color={colorSet.HexColor(EAffix.HP)}>{BattleStats.HP} </color>/ {total.HP}\n";
            str += $"<sprite name=Shield> <color={colorSet.HexColor(EAffix.Shield)}>{BattleStats.Shield} </color>/ {total.Shield}\n"; 
            str += $"<sprite name=Dodge> <color={colorSet.HexColor(EAffix.Dodge)}>{(int) BattleStats.Dodge} </color>/ {(int) total.Dodge}\n"; 
            str += $"<sprite name=Speed> <color={colorSet.HexColor(EAffix.Speed)}>{BattleStats.Speed} </color> \n";
            str += $"<sprite name=TP> <color={colorSet.HexColor(EColor.TurnPoint)}>{BattleStats.TurnPoint} </color> \n";

            return str;
        }

        public override string GetInfoRight()
        {
            string str = "";
            str += $"Basic Power: {BattleStats.Power.Basic} \n";
            str += $"Spell Power : <sprite name=Fire><color={colorSet.HexColor(EAffix.Fire)}>{BattleStats.Power.MagicPercent(EElement.Fire)}</color>  <sprite name=Water><color={colorSet.HexColor(EAffix.Water)}>{BattleStats.Power.MagicPercent(EElement.Water)}</color>  <sprite name=Nature><color={colorSet.HexColor(EAffix.Nature)}>{BattleStats.Power.MagicPercent(EElement.Nature)}</color> (%) \n";
            str += $"Skill Power :  <sprite name=Fire><color={colorSet.HexColor(EAffix.Fire)}>{BattleStats.Power.PhysicPercent(EElement.Fire)}</color>  <sprite name=Water><color={colorSet.HexColor(EAffix.Water)}>{BattleStats.Power.PhysicPercent(EElement.Water)}</color>  <sprite name=Nature><color={colorSet.HexColor(EAffix.Nature)}>{BattleStats.Power.PhysicPercent(EElement.Nature)}</color> (%) \n";
            str += $"Focus Power :  <sprite name=Fire><color={colorSet.HexColor(EAffix.Fire)}>{BattleStats.GetFocus(EElement.Fire)}</color>  <sprite name=Water><color={colorSet.HexColor(EAffix.Water)}>{BattleStats.GetFocus(EElement.Water)}</color>  <sprite name=Nature><color={colorSet.HexColor(EAffix.Nature)}>{BattleStats.GetFocus(EElement.Nature)}</color> \n";
            str += $"Damage Taken :  <sprite name=Fire><color={colorSet.HexColor(EAffix.Fire)}>{affinityDef(EElement.Fire)}</color>  <sprite name=Water><color={colorSet.HexColor(EAffix.Water)}>{affinityDef(EElement.Water)}</color>  <sprite name=Nature><color={colorSet.HexColor(EAffix.Nature)}>{affinityDef(EElement.Nature)}</color> (%) \n";

            return str;
        }

        private string affinityDef(EElement ele)
        {
            if (BattleStats.GetDamageTaken(ele) == 100) return "--";
            string str = "";
            if (BattleStats.GetDamageTaken(ele) > 100)
                str += "+";
            str += (int) BattleStats.GetDamageTaken(ele) - 100;
            str += "%";
            return str;
        }

        public override string GetInfoDown()
        {
            return "";
        }

        public override string ColouredName()
        {
            return $"<color={colorSet.HexColor(EColor.ally)}>{Hero.UnitName}</color>";
        }

        public override Sprite GetIcon()
        {
            return icon.sprite;
        }
    }
}