using _EventSystem.CustomEvents;
using Players;
using Skills;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using Void = _EventSystem.CustomEvents.Void;

namespace Decks
{
    public class AllDecksMono : MonoBehaviour
    {
        [FormerlySerializedAs("Deck")]
        public DeckMono deck;
        private static GameObject _instance;

        [Header("Event Sender")] 
        [SerializeField] private VoidEvent onDraw;
        [SerializeField] private VoidEvent onReDraw;
        
        [FormerlySerializedAs("OnUIDisable")]
        [Header("Event Listener")] 
        [SerializeField] private VoidEvent onUIDisable;
        [SerializeField] private UnitEvent onUnitStartTurn;
        [SerializeField] private VoidEvent onEndTurn;
        [SerializeField] private VoidEvent onBattleStart;

        private void Start()
        {
            DontDestroyOnLoad(gameObject.transform);
            if (_instance == null)
                _instance = gameObject;
            else
                Destroy(gameObject);

            deck = transform.GetComponentInChildren<DeckMono>();
            
            onUnitStartTurn.EventListeners += OnUnitStartTurn;
            onBattleStart.EventListeners += FirstShuffle;
            onEndTurn.EventListeners += EndTurn;
            onUIDisable.EventListeners += ActualizeHand;
        }

        private void OnDestroy()
        {
            onUnitStartTurn.EventListeners -= OnUnitStartTurn;
            onBattleStart.EventListeners -= FirstShuffle;
            onEndTurn.EventListeners -= EndTurn;
            onUIDisable.EventListeners -= ActualizeHand;
        }

        private void ActualizeHand(Void _empty)
        {
            onReDraw.Raise();
        }

        public void EndTurn(Void _obj)
        {
            deck.ClearHandSkills();
            onDraw.Raise();
        }

        private void FirstShuffle(Void _obj)
        {
            deck.Initialize();
        }

        private void OnUnitStartTurn(Unit _item)
        {
            if (_item.playerType != EPlayerType.Human) return;
            deck.DrawNewHand();
            onDraw.Raise();
        }

        public void LearnSkill(SkillSo _skillSo, Skill _learning)
        {
            deck.LearnSkill(_skillSo, _learning);
        }
    }
}