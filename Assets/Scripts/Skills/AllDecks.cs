using System.Collections.Generic;
using _EventSystem.CustomEvents;
using Grid;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace Skills
{
    public class AllDecks : MonoBehaviour
    {
        public List<Deck> Decks;
        private static GameObject instance;
        
        private bool used = false;

        [Header("Event Sender")] 
        [SerializeField] private VoidEvent onActionDone;
        
        [Header("Event Listener")] 
        [SerializeField] private UnitEvent onUnitStartTurn;
        [SerializeField] private VoidEvent onShuffleDecks;
        [SerializeField] private VoidEvent onBattleStart;
        private void Start() 
        {
            DontDestroyOnLoad(gameObject.transform);
            if (instance == null)
                instance = gameObject;
            else
                Destroy(gameObject);
            Decks = new List<Deck>();
            foreach (Transform _child in transform)
            {
                Decks.Add(_child.gameObject.GetComponent<Deck>());
            }

            onUnitStartTurn.EventListeners += OnEventRaised;
            onShuffleDecks.EventListeners += Shuffle;
            onBattleStart.EventListeners += FirstShuffle;
        }

        private void OnDestroy()
        {
            onUnitStartTurn.EventListeners -= OnEventRaised;
            onShuffleDecks.EventListeners -= Shuffle;
            onBattleStart.EventListeners -= FirstShuffle;
        }

        private void FirstShuffle(Void _obj)
        {
            foreach (Deck _deck in Decks)
            {
                _deck.ShuffleDeck();
            }
        }

        private void Shuffle(Void _obj)
        {
            Shuffle();
        }

        public void Shuffle()
        {
            if (BattleStateManager.instance.PlayingUnit.BattleStats.AP < 1 || used) return;
            BattleStateManager.instance.PlayingUnit.BattleStats.AP--;
            used = true;
            
            foreach (Deck _deck in Decks)
            {
                _deck.ShuffleDeck();
                onActionDone.Raise();
            }
        }

        public void OnEventRaised(Unit item)
        {
            GameObject.Find("UI_BattleScene/DecksUI/ShuffleBtn").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("UI_BattleScene/DecksUI/ShuffleBtn").GetComponent<Button>().onClick.AddListener(Shuffle);
            used = false;
        }
    }
}