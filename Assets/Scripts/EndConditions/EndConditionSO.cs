using StateMachine;
using UnityEngine;

namespace EndConditions
{
    public enum EConditionType
    {
        Death,
        LootBox,
        Boss,
        Last,
    }
    public abstract class EndConditionSo : ScriptableObject
    {
        [SerializeField] private EConditionType type;
        public EConditionType Type => type;

        public abstract bool BattleIsOver(BattleStateManager _stateManager);

        public bool WinCondition { get; protected set; }
    }
}