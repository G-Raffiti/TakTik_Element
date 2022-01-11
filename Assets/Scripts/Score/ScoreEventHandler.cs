using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using Gears;
using StateMachine;
using Units;
using UnityEngine;
using Void = _EventSystem.CustomEvents.Void;

namespace Score
{
    public class ScoreEventHandler : MonoBehaviour
    {
        [Header("Event Listener")]
        [SerializeField] private VoidEvent OnBattleStart;
        [SerializeField] private IntEvent OnCellWalked;
        [SerializeField] private GearEvent OnDestroyGear;
        [SerializeField] private IntEvent OnCraftingMaterialAdded;

        private void Start()
        {
            OnBattleStart.EventListeners += CatchUnitEvents;
            OnBattleStart.EventListeners += CountBosses;
            OnCellWalked.EventListeners += ScoreHolder.AddDistance;
            OnDestroyGear.EventListeners += OnGearDestroyed;
            OnCraftingMaterialAdded.EventListeners += ScoreHolder.AddCraftingMaterial;
        }
        private void OnDestroy()
        {
            OnBattleStart.EventListeners -= CatchUnitEvents;
            OnBattleStart.EventListeners -= CountBosses;
            OnCellWalked.EventListeners -= ScoreHolder.AddDistance;
            OnDestroyGear.EventListeners -= OnGearDestroyed;
            OnCraftingMaterialAdded.EventListeners -= ScoreHolder.AddCraftingMaterial;
        }

        private void CatchUnitEvents(Void _obj)
        {
            foreach (Unit _unit in BattleStateManager.instance.Units)
            {
                _unit.UnitAttacked += ScoreHolder.Damage;
            }
        }

        private void CountBosses(Void _obj)
        {
            List<Monster> monsters = new List<Monster>();

            foreach (Unit _unit in BattleStateManager.instance.Units)
            {
                if (_unit.playerNumber != 0)
                {
                    monsters.Add((Monster) _unit);
                }
            }
            
            foreach (Monster boss in monsters.Where(m => m.Type == EMonster.Boss))
            {
                ScoreHolder.AddBoss(boss.MonsterSO);
            }
        }

        private void OnGearDestroyed(Gear _gear)
        {
            ScoreHolder.AddGearSalvaged();
        }
    }
}