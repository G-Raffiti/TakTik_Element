using System.Collections.Generic;
using _EventSystem.CustomEvents;
using Grid;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace Skills
{
    public class AllDecksSimplify : MonoBehaviour
    {
        public List<DeckSimplify> Decks;
        private static GameObject instance;
        
        private bool used = false;

        [Header("Event Sender")] 
        [SerializeField] private VoidEvent onActionDone;
        
        [Header("Event Listener")] 
        [SerializeField] private UnitEvent onUnitStartTurn;
        [SerializeField] private VoidEvent onShuffleDecks;
        
        private void Start() 
        {
            DontDestroyOnLoad(gameObject.transform);
            if (instance == null)
                instance = gameObject;
            else
                Destroy(gameObject);
            Decks = new List<DeckSimplify>();
            foreach (Transform _child in transform)
            {
                Decks.Add(_child.gameObject.GetComponent<DeckSimplify>());
            }

            onUnitStartTurn.EventListeners += OnEventRaised;
            onShuffleDecks.EventListeners += Shuffle;
        }

        private void OnDestroy()
        {
            onUnitStartTurn.EventListeners -= OnEventRaised;
            onShuffleDecks.EventListeners -= Shuffle;
        }

        private void Shuffle(Void _obj)
        {
            Shuffle();
        }

        public void Shuffle()
        {
            Debug.Log("Shuffle deck");
            if (BattleStateManager.instance.PlayingUnit.BattleStats.AP < 1 || used) return;
            BattleStateManager.instance.PlayingUnit.BattleStats.AP--;
            used = true;
            
            foreach (DeckSimplify _deck in Decks)
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