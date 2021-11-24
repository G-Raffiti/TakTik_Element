using Players;
using Units;
using UnityEngine;

namespace _ScriptableObject
{
    [CreateAssetMenu(fileName = "Archetype_", menuName = "Scriptable Object/Archetype")]
    public class Archetype : ScriptableObject
    {
        [SerializeField] private EArchetype type;
        [SerializeField] private AIPlayer.EvaluationValues evaluation;
        
        public EArchetype Type => type;
        public AIPlayer.EvaluationValues Evaluations => evaluation;
    }
}