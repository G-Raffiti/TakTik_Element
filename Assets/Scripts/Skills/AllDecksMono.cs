using _EventSystem.CustomEvents;
using Units;
using UnityEngine;
using Void = _EventSystem.CustomEvents.Void;

namespace Skills
{
    public class AllDecksMono : MonoBehaviour
    {
        public DeckMono Deck;
        private static GameObject instance;

        [Header("Event Sender")] 
        [SerializeField] private VoidEvent onActionDone;
        
        [Header("Event Listener")] 
        [SerializeField] private UnitEvent onUnitStartTurn;
        [SerializeField] private VoidEvent onEndTurn;
        [SerializeField] private VoidEvent onBattleStart;

        private void Start()
        {
            DontDestroyOnLoad(gameObject.transform);
            if (instance == null)
                instance = gameObject;
            else
                Destroy(gameObject);

            Deck = transform.GetComponentInChildren<DeckMono>();
            
            onUnitStartTurn.EventListeners += OnEventRaised;
            onBattleStart.EventListeners += FirstShuffle;
            onEndTurn.EventListeners += EndTurn;
        }

        private void OnDestroy()
        {
            onUnitStartTurn.EventListeners -= OnEventRaised;
            onBattleStart.EventListeners -= FirstShuffle;
            onEndTurn.EventListeners -= EndTurn;
        }

        public void EndTurn(Void _obj)
        {
            Deck.ClearHandSkills();
            onActionDone.Raise();
            Deck.PrintDebug();
        }

        private void FirstShuffle(Void _obj)
        {
            Deck.Initialize();
            Deck.PrintDebug();
        }

        public void OnEventRaised(Unit item)
        {
            if (item.playerNumber != 0) return;
            Deck.DrawNewHand();
            onActionDone.Raise();
            Deck.PrintDebug();
        }

        public void LearnSkill(SkillSO _skillSO, Skill learning)
        {
            Deck.LearnSkill(_skillSO, learning);
            onActionDone.Raise();
        }
    }
}