using System.Collections.Generic;
using _EventSystem.CustomEvents;
using Grid;
using Skills;
using Units;
using UnityEngine;

namespace UserInterface
{
    /// <summary>
    /// Class that Update the Skills UI on Battle Scene
    /// </summary>
    public class DeckUI : MonoBehaviour
    {
        private List<Deck> decks;
        private Dictionary<SkillInfo, int> skills;
        private Unit unit;

        [Header("Event Listener")]
        [SerializeField] private UnitEvent onUnitStartTurn;
        [SerializeField] private VoidEvent onSkillUsed;
        [SerializeField] private VoidEvent onActionDone;

        private void Start()
        {
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

        private void OnEnable()
        {
            onSkillUsed.EventListeners += onSkillUsedRaised;
            onUnitStartTurn.EventListeners += onUnitStartTurnRaised;
            onActionDone.EventListeners += onSkillUsedRaised;
        }

        private void OnDisable()
        {
            onSkillUsed.EventListeners -= onSkillUsedRaised;
            onUnitStartTurn.EventListeners -= onUnitStartTurnRaised;
            onActionDone.EventListeners -= onSkillUsedRaised;
        }

        public void onUnitStartTurnRaised(Unit item)
        {
            if (item.playerNumber != 0)
            {
                unit = BattleStateManager.instance.Units.Find(Unit => Unit.playerNumber == 0);
            }
            else unit = item;
            
            UpdateDisplay();
        }

        public void onSkillUsedRaised<T>(T param)
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