using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using Cells;
using DataBases;
using Players;
using Resources.ToolTip.Scripts;
using StateMachine;
using Stats;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
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

        [FormerlySerializedAs("UnitTooltip_ON")]
        [Header("Tooltip Events")]
        [SerializeField] private UnitEvent unitTooltipOn;
        [FormerlySerializedAs("UnitTooltip_OFF")]
        [SerializeField] private VoidEvent unitTooltipOff;
        
        
        
        private Dictionary<EColor, Color> colors = new Dictionary<EColor, Color>();

        private CellState unitMark;
        
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
            switch (unit.playerType)
            {
                case EPlayerType.Human:
                    _teamColor = colors[EColor.Ally];
                    break;
                case EPlayerType.AI:
                    _teamColor = colors[EColor.Enemy];
                    break;
                default:
                    _teamColor = Color.white;
                    break;
            }
            team.color = _teamColor;
            icon.sprite = unit.UnitSprite;
            health.fillAmount = unit.battleStats.hp / (float)unit.Total.hp;
            shield.fillAmount = unit.battleStats.shield / (float)unit.Total.hp;
            onUnitStartTurn.EventListeners += UpdateDisplay;
            onSkillUsed.EventListeners += UpdateDisplay;
        }

        private void OnDestroy()
        {
            onUnitStartTurn.EventListeners -= UpdateDisplay;
            onSkillUsed.EventListeners -= UpdateDisplay;
        }

        private void Unit_UnitDestroyed(object _sender, DeathEventArgs _e)
        {
            Destroy(gameObject);
        }

        private void Unit_UnitAttacked(object _sender, AttackEventArgs _e)
        {
            health.fillAmount = unit.battleStats.hp / (float)unit.Total.hp;
            shield.fillAmount = unit.battleStats.shield / (float)unit.Total.hp;
        }

        private void UpdateDisplay(Void _empty)
        {
            health.fillAmount = unit.battleStats.hp / (float)unit.Total.hp;
            shield.fillAmount = unit.battleStats.shield / (float)unit.Total.hp;
        }
        
        private void UpdateDisplay(Unit _unit)
        {
            health.fillAmount = unit.battleStats.hp / (float)unit.Total.hp;
            shield.fillAmount = unit.battleStats.shield / (float)unit.Total.hp;
        }

        public override void OnPointerEnter(PointerEventData _eventData)
        {
            if (Input.GetMouseButton(0)) return;
            cellGrid.BattleState.OnCellSelected(unit.Cell);
            unitTooltipOn.Raise(unit);
        }

        public override void OnPointerExit(PointerEventData _eventData)
        {
            if (Input.GetMouseButton(0)) return;
            cellGrid.BattleState.OnCellDeselected(unit.Cell);
            unitTooltipOff.Raise();
        }

        public override void OnPointerClick(PointerEventData _eventData)
        {
            if (_eventData.button == PointerEventData.InputButton.Right)
            {
                unit.OnTooltipOn.Raise(unit);
                return;
            }

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