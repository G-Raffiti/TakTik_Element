using System;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _EventSystem.Listeners;
using Grid;
using Skills;
using Units;
using UnityEngine;
using Void = _EventSystem.Void;

namespace UserInterface
{
    /// <summary>
    /// Class that Update the Skills UI on Battle Scene
    /// </summary>
    public class DeckUI : MonoBehaviour, IGameEventListener<Unit>, IGameEventListener<Void>
    {
        [SerializeField] private UnitEvent onUnitStartTurn;
        [SerializeField] private VoidEvent onSkillUsed;

        private List<Deck> decks;
        
        private Dictionary<SkillInfo, int> skills;
        private Unit unit;

        private void Start()
        {
            onSkillUsed.RegisterListener(this);
            onUnitStartTurn.RegisterListener(this);

            skills = new Dictionary<SkillInfo, int>();
            for (int i = 0; i < transform.childCount; i++)
            {
                skills.Add(transform.GetChild(i).gameObject.GetComponent<SkillInfo>(), transform.childCount - i - 1);
            }

            decks = new List<Deck>();
            foreach (Transform _child in GameObject.Find("Decks").transform)
            {
                decks.Add(_child.gameObject.GetComponent<Deck>());
            }
            int number = transform.GetSiblingIndex();
            foreach (SkillInfo _skillsKey in skills.Keys)
            {
                _skillsKey.Deck = decks[number];
            }
        }

        private void OnDestroy()
        {
            onSkillUsed.UnregisterListener(this);
            onUnitStartTurn.UnregisterListener(this);
        }

        public void OnEventRaised(Unit item)
        {
            if (item.playerNumber != 0)
            {
                unit = BattleStateManager.instance.Units.Find(Unit => Unit.playerNumber == 0);
            }
            else unit = item;
            
            UpdateDisplay();
        }

        public void OnEventRaised(Void item)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            foreach (KeyValuePair<SkillInfo,int> _skill in skills)
            {
                if (_skill.Key.Deck.Skills.Count - 1 < _skill.Value)
                {
                    foreach (Transform _child in _skill.Key.gameObject.transform)
                    {
                        _child.gameObject.SetActive(false);
                    }
                }
                else
                {
                    foreach (Transform _child in _skill.Key.gameObject.transform)
                    {
                        _child.gameObject.SetActive(true);
                    }
                    _skill.Key.UpdateSkill(_skill.Value, unit);
                    _skill.Key.DisplayIcon();
                }
            }
        }
    }
}