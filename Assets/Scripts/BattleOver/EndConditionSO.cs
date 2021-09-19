using Grid;
using UnityEngine;

namespace BattleOver
{
    public enum EConditionType
    {
        Death,
        LootBox,
        Boss,
    }
    public abstract class EndConditionSO : ScriptableObject
    {
        [SerializeField] private EConditionType type;
        public EConditionType Type => type;

        public abstract bool battleIsOver(BattleStateManager StateManager);
        public bool WinCondition { get; protected set; }
    }
}