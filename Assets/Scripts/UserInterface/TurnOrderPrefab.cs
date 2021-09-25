using System.Collections.Generic;
using System.Linq;
using Cells;
using Grid;
using Resources.ToolTip.Scripts;
using Stats;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface
{
    public class TurnOrderPrefab : InfoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image health;
        
        [SerializeField] private ColorSet colorSet;
        private Dictionary<EColor, Color> colors = new Dictionary<EColor, Color>();

        private Color[] unitMark = new Color[3];
        
        private Unit unit;
        private BattleStateManager cellGrid;

        public void Initialize(Unit _unit)
        {
            cellGrid = GameObject.Find("CellGrid").GetComponent<BattleStateManager>();
            colors = colorSet.GetColors();
            unit = _unit;
            unit.UnitAttacked += Unit_UnitAttacked;
            unit.UnitDestroyed += Unit_UnitDestroyed;
            Color _teamColor;
            switch (unit.playerNumber)
            {
                case 0:
                    _teamColor = colors[EColor.ally];
                    break;
                case 1:
                    _teamColor = colors[EColor.enemy];
                    break;
                default:
                    _teamColor = Color.white;
                    break;
            }
            GetComponent<Image>().color = _teamColor;
            icon.sprite = unit.UnitSprite;
            health.GetComponent<Image>().fillAmount = unit.BattleStats.HP / (float)unit.Total.HP;
        }

        private void Unit_UnitDestroyed(object _sender, DeathEventArgs _e)
        {
            Destroy(gameObject);
        }

        private void Unit_UnitAttacked(object _sender, AttackEventArgs _e)
        {
            health.GetComponent<Image>().fillAmount = unit.BattleStats.HP / (float)unit.Total.HP;
        }


        public override void OnPointerEnter(PointerEventData _eventData)
        {
            unitMark = unit.getColors();
            unit.MarkAsSelected();
            TooltipOn.Raise(this);
        }

        public override void OnPointerExit(PointerEventData _eventData)
        {
            unit.MarkBack(unitMark);
            TooltipOff.Raise();
        }

        public override void OnPointerClick(PointerEventData _eventData)
        {
            cellGrid.BattleState.OnCellClicked(unit.Cell);
        }

        #region IInfo

        public override string GetInfoMain()
        {
            string str = "";
            str += ColouredName();
            if (unit.playerNumber == 0)
            {
                str +=  "\nHero" + "\n";
            }

            else str +=  "\nMonster" + "\n";

            return str;
        }

        public override string GetInfoLeft()
        {
            string str = "";
            str += $"<sprite name=AP> <color={colorSet.HexColor(EAffix.AP)}>{(int)unit.Total.AP}</color>    ";
            str += $"<sprite name=MP> <color={colorSet.HexColor(EAffix.MP)}>{(int)unit.Total.MP}</color>\n";
            str += $"<sprite name=HP> <color={colorSet.HexColor(EAffix.HP)}>{unit.BattleStats.HP} </color>/ {unit.Total.HP}\n";
            if (unit.BattleStats.Shield > 0) str += $"<sprite name=Shield> <color={colorSet.HexColor(EAffix.Shield)}>{unit.BattleStats.Shield} </color>/ {unit.Total.Shield}\n"; 
            if ((int) unit.BattleStats.Dodge > 0) str += $"<sprite name=Dodge> <color={colorSet.HexColor(EAffix.Dodge)}>{(int) unit.BattleStats.Dodge} </color>/ {(int) unit.Total.Dodge}\n"; 
            str += $"<sprite name=Speed> <color={colorSet.HexColor(EAffix.Speed)}>{unit.BattleStats.Speed} </color> \n";
            str += $"<sprite name=TP> <color={colorSet.HexColor(EColor.TurnPoint)}>{unit.BattleStats.TurnPoint} </color> \n";

            return str;
        }

        public override string GetInfoRight()
        {
            string str = "";
            str += $"Basic Power: {unit.BattleStats.Power.Basic} \n";
            str += $"Spell Power : <sprite name=Fire><color={colorSet.HexColor(EAffix.Fire)}>{unit.BattleStats.Power.MagicPercent(EElement.Fire)}</color>  <sprite name=Water><color={colorSet.HexColor(EAffix.Water)}>{unit.BattleStats.Power.MagicPercent(EElement.Water)}</color>  <sprite name=Nature><color={colorSet.HexColor(EAffix.Nature)}>{unit.BattleStats.Power.MagicPercent(EElement.Nature)}</color> (%) \n";
            str += $"Skill Power :  <sprite name=Fire><color={colorSet.HexColor(EAffix.Fire)}>{unit.BattleStats.Power.PhysicPercent(EElement.Fire)}</color>  <sprite name=Water><color={colorSet.HexColor(EAffix.Water)}>{unit.BattleStats.Power.PhysicPercent(EElement.Water)}</color>  <sprite name=Nature><color={colorSet.HexColor(EAffix.Nature)}>{unit.BattleStats.Power.PhysicPercent(EElement.Nature)}</color> (%) \n";
            str += $"Focus Power :  <sprite name=Fire><color={colorSet.HexColor(EAffix.Fire)}>{unit.BattleStats.GetFocus(EElement.Fire)}</color>  <sprite name=Water><color={colorSet.HexColor(EAffix.Water)}>{unit.BattleStats.GetFocus(EElement.Water)}</color>  <sprite name=Nature><color={colorSet.HexColor(EAffix.Nature)}>{unit.BattleStats.GetFocus(EElement.Nature)}</color> \n";
            str += $"Damage Taken :  <sprite name=Fire><color={colorSet.HexColor(EAffix.Fire)}>{affinityDef(EElement.Fire)}</color>  <sprite name=Water><color={colorSet.HexColor(EAffix.Water)}>{affinityDef(EElement.Water)}</color>  <sprite name=Nature><color={colorSet.HexColor(EAffix.Nature)}>{affinityDef(EElement.Nature)}</color> (%) \n";

            return str;
        }

        private string affinityDef(EElement ele)
        {
            if (unit.BattleStats.GetDamageTaken(ele) == 100) return "--";
            string str = "";
            if (unit.BattleStats.GetDamageTaken(ele) > 100)
                str += "+";
            str += (int) unit.BattleStats.GetDamageTaken(ele) - 100;
            str += "%";
            return str;
        }

        public override string GetInfoDown()
        {
            return unit.Buffs.Aggregate("", (_current, _buff) => _current + (_buff.InfoOnUnit(_buff, unit) + "\n"));
        }

        public override string ColouredName()
        {
            string hexColour;
            if (unit.playerNumber == 0)
                hexColour = colorSet.HexColor(EColor.ally);
            else 
                hexColour = colorSet.HexColor(EColor.enemy);
            return $"<color={hexColour}>{unit.UnitName}</color>";
        }

        public override Sprite GetIcon()
        {
            return unit.UnitSprite;
        }

        #endregion
        
    }
}