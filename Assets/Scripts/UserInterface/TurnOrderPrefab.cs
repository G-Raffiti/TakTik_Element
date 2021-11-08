using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
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
        [SerializeField] private Image shield;
        
        [SerializeField] private ColorSet colorSet;
        [SerializeField] private UnitEvent onUnitStartTurn;
        private Dictionary<EColor, Color> colors = new Dictionary<EColor, Color>();

        private TileIsometric.CellState unitMark;
        
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
            health.fillAmount = unit.BattleStats.HP / (float)unit.Total.HP;
            shield.fillAmount = unit.BattleStats.Shield / (float)unit.Total.HP;
            onUnitStartTurn.EventListeners += updateDisplay;
        }

        private void Unit_UnitDestroyed(object _sender, DeathEventArgs _e)
        {
            Destroy(gameObject);
        }

        private void Unit_UnitAttacked(object _sender, AttackEventArgs _e)
        {
            health.fillAmount = unit.BattleStats.HP / (float)unit.Total.HP;
            shield.fillAmount = unit.BattleStats.Shield / (float)unit.Total.HP;
        }

        private void updateDisplay(Unit _unit)
        {
            health.fillAmount = unit.BattleStats.HP / (float)unit.Total.HP;
            shield.fillAmount = unit.BattleStats.Shield / (float)unit.Total.HP;
        }

        public override void OnPointerEnter(PointerEventData _eventData)
        {
            unitMark = ((TileIsometric)unit.Cell).State;
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
            str += $"<sprite name=MP> <color={colorSet.HexColor(EAffix.MP)}>{(int)unit.Total.MP}</color> \n";
            str += $"<sprite name=HP> <color={colorSet.HexColor(EAffix.HP)}>{unit.BattleStats.HP} </color>/ {unit.Total.HP}    ";
            str += $"<sprite name=Shield> <color={colorSet.HexColor(EAffix.Shield)}>{unit.BattleStats.Shield}</color> \n";
            str += $"<sprite name=Fire> <color={colorSet.HexColor(EAffix.Fire)}>{unit.BattleStats.GetPower(EElement.Fire)}</color>  <sprite name=Water> <color={colorSet.HexColor(EAffix.Water)}>{unit.BattleStats.GetPower(EElement.Water)}</color>  <sprite name=Nature> <color={colorSet.HexColor(EAffix.Nature)}>{unit.BattleStats.GetPower(EElement.Nature)}</color>";

            return str;
        }

        public override string GetInfoRight()
        {
            string str = "";
            str += unit.BattleStats.Range.ToString(unit)+ "\n";
            str += $"<sprite name=Speed> <color={colorSet.HexColor(EAffix.Speed)}>{unit.BattleStats.Speed} </color> \n";
            str += $"<sprite name=TP> <color={colorSet.HexColor(EColor.TurnPoint)}>{unit.TurnPoint} </color> \n";
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