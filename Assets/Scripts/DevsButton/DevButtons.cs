using System;
using _EventSystem.CustomEvents;
using _Instances;
using Decks;
using Skills;
using UnityEngine;
using UnityEngine.Serialization;

namespace DevsButton
{
    public class DevButtons : MonoBehaviour
    {
        [SerializeField] private DeckMono deck;

        [FormerlySerializedAs("OnePunchMan")]
        [SerializeField] private SkillSo onePunchMan;

        [SerializeField] private VoidEvent onReDraw;

        public void OnClick()
        {
            if (deck == null)
            {
                deck = GameObject.Find("DeckMono/Deck1").GetComponent<DeckMono>();
            }
            deck.AddHandSkill(onePunchMan);
            onReDraw.Raise();
        }

    }
}