using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using Cells;
using DataBases;
using Resources.ToolTip.Scripts;
using StateMachine;
using Stats;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface.BattleScene
{
    public class TurnOrderPrefab : InfoBehaviour
    {
        [SerializeField] private Image team;
        [SerializeField] private Image icon;
        [SerializeField] private Image health;
        [SerializeField] private Image shield;
        
        [SerializeField] private ColorSet colorSet;
        
        [Header("Event Listener")]
        [SerializeField] private UnitEvent onUnitStartTurn;
        [SerializeField] private VoidEvent onSkillUsed;
        
        
        
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
            team.color = _teamColor;
            icon.sprite = unit.UnitSprite;
            health.fillAmount = unit.BattleStats.HP / (float)unit.Total.HP;
            shield.fillAmount = unit.BattleStats.Shield / (float)unit.Total.HP;
            onUnitStartTurn.EventListeners += updateDisplay;
            onSkillUsed.EventListeners += updateDisplay;
        }

        private void OnDestroy()
        {
            onUnitStartTurn.EventListeners -= updateDisplay;
            onSkillUsed.EventListeners -= updateDisplay;
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

        private void updateDisplay(Void empty)
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
            return unit.GetInfoMain();
        }

        public override string GetInfoLeft()
        {
            return unit.GetInfoLeft();
        }

        public override string GetInfoRight()
        {
            return unit.GetInfoRight();
        }

        public override string GetInfoDown()
        {
            return unit.GetInfoDown();
        }

        public override string ColouredName()
        {
            return unit.ColouredName();
        }

        public override Sprite GetIcon()
        {
            return unit.GetIcon();
        }

        #endregion
        
    }
}