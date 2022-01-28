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
    public class BattleDeckMono_UI : MonoBehaviour
    {
        private DeckMono deck;
        private Unit unit;

        [SerializeField] private GameObject skillBtn;

        [Header("Event Listener")]
        [SerializeField] private UnitEvent onUnitStartTurn;
        [SerializeField] private VoidEvent onDraw;
        [SerializeField] private VoidEvent onReDraw;
        [SerializeField] private VoidEvent onUnitEndTurn;

        private IEnumerator Start()
        {
            AllDecksMono allDecks = GameObject.Find("DeckMono").GetComponent<AllDecksMono>();

            yield return new WaitForSeconds(0.1f);
            
            deck = allDecks.Deck;
        }

        private void OnEnable()
        {
            onUnitStartTurn.EventListeners += onUnitStartTurnRaised;
            onDraw.EventListeners += OnDrawRaised;
            onUnitEndTurn.EventListeners += onEndTurn;
            onReDraw.EventListeners += DrawFast;
        }

        private void OnDisable()
        {
            onUnitStartTurn.EventListeners -= onUnitStartTurnRaised;
            onDraw.EventListeners -= OnDrawRaised;
            onUnitEndTurn.EventListeners -= onEndTurn;
            onReDraw.EventListeners -= DrawFast;
        }

        public void onUnitStartTurnRaised(Unit item)
        {
            if (item.playerNumber != 0)
            {
                unit = BattleStateManager.instance.Units.Find(Unit => Unit.playerNumber == 0);
            }
            else unit = item;
            
            //StartCoroutine(DrawAnimation());
        }

        public void onEndTurn(Void _obj)
        {
            int childs = transform.childCount;
            for (int i = childs - 1; i > -1; i--)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }

        public void OnDrawRaised<T>(T param)
        {
            StartCoroutine(DrawAnimation());
        }

        private bool drawRunning = false;
        private IEnumerator DrawAnimation()
        {
            if (drawRunning) yield break;
            drawRunning = true;
            
            int childs = transform.childCount;
            for (int i = childs - 1; i > -1; i--)
            {
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }
            
            if (BattleStateManager.instance.PlayingUnit == null) yield break;
            
            Unit currentUnit = BattleStateManager.instance.PlayingUnit;
            foreach (Skill skill in deck.GetHandSkills(currentUnit))
            {
                GameObject skillInfo = GameObject.Instantiate(skillBtn, transform);
                skillInfo.GetComponent<SkillInfo>().skill = skill;
                skillInfo.GetComponent<SkillInfo>().Unit = currentUnit;
                skillInfo.GetComponent<SkillInfo>().DisplayIcon();

                yield return new WaitForSeconds(0.1f);
            }

            drawRunning = false;
        }

        private void DrawFast(Void empty)
        {
            int childs = transform.childCount;
            for (int i = childs - 1; i > -1; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            
            if (BattleStateManager.instance.PlayingUnit == null) return;
            
            Unit currentUnit = BattleStateManager.instance.PlayingUnit;
            foreach (Skill skill in deck.GetHandSkills(currentUnit))
            {
                GameObject skillInfo = Instantiate(skillBtn, transform);
                skillInfo.GetComponent<SkillInfo>().skill = skill;
                skillInfo.GetComponent<SkillInfo>().Unit = currentUnit;
                skillInfo.GetComponent<SkillInfo>().DisplayIcon();
            }
        }
    }
}