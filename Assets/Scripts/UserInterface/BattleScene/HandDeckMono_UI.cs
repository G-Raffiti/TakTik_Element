using System.Collections;
using _EventSystem.CustomEvents;
using Decks;
using Skills;
using StateMachine;
using Units;
using UnityEngine;

namespace UserInterface.BattleScene
{
    /// <summary>
    /// Class that Update the Skills UI on Battle Scene
    /// </summary>
    public class HandDeckMonoUI : MonoBehaviour
    {
        private DeckMono deck;

        [SerializeField] private GameObject skillBtn;

        [Header("Event Listener")]
        [SerializeField] private UnitEvent onUnitStartTurn;
        [SerializeField] private VoidEvent onDraw;
        [SerializeField] private VoidEvent onReDraw;
        [SerializeField] private VoidEvent onUnitEndTurn;
        [SerializeField] private VoidEvent onSkillUsed;

        private IEnumerator Start()
        {
            AllDecksMono _allDecks = GameObject.Find("DeckMono").GetComponent<AllDecksMono>();

            yield return new WaitForSeconds(0.1f);
            
            deck = _allDecks.deck;
        }

        private void OnEnable()
        {
            onUnitStartTurn.EventListeners += OnUnitStartTurnRaised;
            onDraw.EventListeners += OnDrawRaised;
            onUnitEndTurn.EventListeners += OnEndTurn;
            onReDraw.EventListeners += DrawFast;
            onSkillUsed.EventListeners += DrawFast;
        }

        private void OnDisable()
        {
            onUnitStartTurn.EventListeners -= OnUnitStartTurnRaised;
            onDraw.EventListeners -= OnDrawRaised;
            onUnitEndTurn.EventListeners -= OnEndTurn;
            onReDraw.EventListeners -= DrawFast;
            onSkillUsed.EventListeners -= DrawFast;
        }

        public void OnUnitStartTurnRaised(Unit _item)
        {
        }

        public void OnEndTurn(Void _obj)
        {
            int _childs = transform.childCount;
            for (int _i = _childs - 1; _i > -1; _i--)
            {
                GameObject.Destroy(transform.GetChild(_i).gameObject);
            }
        }

        public void OnDrawRaised<T>(T _param)
        {
            StartCoroutine(DrawAnimation());
        }

        private bool drawRunning = false;
        private IEnumerator DrawAnimation()
        {
            if (drawRunning) yield break;
            drawRunning = true;
            
            int _childs = transform.childCount;
            for (int _i = _childs - 1; _i > -1; _i--)
            {
                GameObject.DestroyImmediate(transform.GetChild(_i).gameObject);
            }
            
            if (BattleStateManager.instance.PlayingUnit == null) yield break;
            
            Unit _currentUnit = BattleStateManager.instance.PlayingUnit;
            foreach (Skill _skill in deck.GetHandSkills(_currentUnit))
            {
                GameObject _skillInfo = GameObject.Instantiate(skillBtn, transform);
                _skillInfo.GetComponent<SkillInfo>().skill = _skill;
                _skillInfo.GetComponent<SkillInfo>().unit = _currentUnit;
                _skillInfo.GetComponent<SkillInfo>().DisplayIcon();

                yield return new WaitForSeconds(0.1f);
            }

            drawRunning = false;
        }

        private void DrawFast(Void _empty)
        {
            int _childs = transform.childCount;
            for (int _i = _childs - 1; _i > -1; _i--)
            {
                DestroyImmediate(transform.GetChild(_i).gameObject);
            }
            
            if (BattleStateManager.instance.PlayingUnit == null) return;
            
            Unit _currentUnit = BattleStateManager.instance.PlayingUnit;
            foreach (Skill _skill in deck.GetHandSkills(_currentUnit))
            {
                GameObject _skillInfo = Instantiate(skillBtn, transform);
                _skillInfo.GetComponent<SkillInfo>().skill = _skill;
                _skillInfo.GetComponent<SkillInfo>().unit = _currentUnit;
                _skillInfo.GetComponent<SkillInfo>().DisplayIcon();
            }
        }
    }
}