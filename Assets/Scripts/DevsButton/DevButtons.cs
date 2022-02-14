using System;
using _EventSystem.CustomEvents;
using _Instances;
using Decks;
using Skills;
using UnityEngine;

namespace DevsButton
{
    public class DevButtons : MonoBehaviour
    {
        [SerializeField] private DeckMono deck;

        [SerializeField] private SkillSO OnePunchMan;

        [SerializeField] private VoidEvent onReDraw;

        public void OnClick()
        {
            if (deck == null)
            {
                deck = GameObject.Find("DeckMono/Deck1").GetComponent<DeckMono>();
            }
            deck.AddHandSkill(OnePunchMan);
            onReDraw.Raise();
        }

    }
}