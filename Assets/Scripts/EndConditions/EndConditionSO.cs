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
    public abstract class EndConditionSO : ScriptableObject
    {
        [SerializeField] private EConditionType type;
        public EConditionType Type => type;

        public abstract bool battleIsOver(BattleStateManager StateManager);

        public bool WinCondition { get; protected set; }
    }
}