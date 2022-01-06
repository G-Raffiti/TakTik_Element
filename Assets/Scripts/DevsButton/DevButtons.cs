using System;
using _EventSystem.CustomEvents;
using Skills;
using UnityEngine;

namespace DevsButton
{
    public class DevButtons : MonoBehaviour
    {
        [SerializeField] private DeckMono deck;

        [SerializeField] private SkillSO OnePunchMan;

        [SerializeField] private VoidEvent onReDraw;

        private static GameObject instance;
        private void Start()
        {
            DontDestroyOnLoad(gameObject.transform);
            if (instance == null)
                instance = gameObject;
            else
                Destroy(gameObject);
        }

        public void OnClick()
        {
            deck.AddHandSkill(OnePunchMan);
            onReDraw.Raise();
        }

    }
}