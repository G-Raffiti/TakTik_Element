using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using Gears;
using Players;
using StateMachine;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using Void = _EventSystem.CustomEvents.Void;

namespace Score
{
    public class ScoreEventHandler : MonoBehaviour
    {
        [FormerlySerializedAs("OnBattleStart")]
        [Header("Event Listener")]
        [SerializeField] private VoidEvent onBattleStart;
        [FormerlySerializedAs("OnCellWalked")]
        [SerializeField] private IntEvent onCellWalked;
        [FormerlySerializedAs("OnDestroyGear")]
        [SerializeField] private GearEvent onDestroyGear;
        [FormerlySerializedAs("OnCraftingMaterialAdded")]
        [SerializeField] private IntEvent onCraftingMaterialAdded;

        private void Start()
        {
            onBattleStart.EventListeners += CatchUnitEvents;
            onBattleStart.EventListeners += CountBosses;
            onCellWalked.EventListeners += ScoreHolder.AddDistance;
            onDestroyGear.EventListeners += OnGearDestroyed;
            onCraftingMaterialAdded.EventListeners += ScoreHolder.AddCraftingMaterial;
        }
        private void OnDestroy()
        {
            onBattleStart.EventListeners -= CatchUnitEvents;
            onBattleStart.EventListeners -= CountBosses;
            onCellWalked.EventListeners -= ScoreHolder.AddDistance;
            onDestroyGear.EventListeners -= OnGearDestroyed;
            onCraftingMaterialAdded.EventListeners -= ScoreHolder.AddCraftingMaterial;
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
            Debug.Log("check");
            List<Monster> _monsters = BattleStateManager.instance.Units.Where(_unit => _unit.playerType != EPlayerType.Human).Cast<Monster>().ToList();

            foreach (Monster _boss in _monsters.Where(_m => _m.Type == EMonster.Boss))
            {
                ScoreHolder.AddBoss(_boss.MonsterSo);
            }

            Debug.Log(ScoreHolder.Bosses.Count);
        }

        private void OnGearDestroyed(Gear _gear)
        {
            ScoreHolder.AddGearSalvaged();
        }
    }
}