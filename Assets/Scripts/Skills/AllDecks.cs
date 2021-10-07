using System;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _EventSystem.Listeners;
using _Instances;
using Grid;
using Units;
using UnityEngine;
using UnityEngine.UI;
using Void = _EventSystem.Void;

namespace Skills
{
    public class AllDecks : MonoBehaviour, IGameEventListener<Unit>
    {
        public List<Deck> Decks;
        private static GameObject instance;
        [SerializeField] private VoidEvent onSkillUsed;
        [SerializeField] private UnitEvent onUnitStartTurn;
        private bool used = false;
        
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
            onUnitStartTurn.RegisterListener(this);
        }

        private void OnDestroy()
        {
            onUnitStartTurn.UnregisterListener(this);
        }

        public void Shuffle()
        {
            Debug.Log("Hello !");
            if (BattleStateManager.instance.PlayingUnit.BattleStats.AP < 1 || used) return;
            BattleStateManager.instance.PlayingUnit.BattleStats.AP--;
            used = true;
            
            foreach (Deck _deck in Decks)
            {
                _deck.ShuffleDeck();
                onSkillUsed.Raise();
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