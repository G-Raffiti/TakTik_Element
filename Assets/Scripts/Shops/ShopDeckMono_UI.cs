using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using _Extension;
using _Instances;
using Decks;
using Relics;
using Skills;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using UserInterface;

namespace Shops
{
    public class ShopDeckMonoUI : MonoBehaviour
    {
        [SerializeField] private SlotDragAndDrop prefabSlot;

        [SerializeField] private ItemDragAndDrop prefabSkill;

        [SerializeField] private Transform deckPlaceHolder;
        [SerializeField] private List<PersonalInventory> portraits;
        [FormerlySerializedAs("BattlesHeroes")]
        [SerializeField] private List<BattleHero> battlesHeroes;
        private BattleHero actualHero;
        private List<SkillInfo> AllSkills { get; set; }

        [Header("Event Listener")] 
        [SerializeField] private IntEvent onCampPointUsed;
        [SerializeField] private IntEvent onHeroChange;

        private void OnEnable()
        {
            onHeroChange.EventListeners += ChangeHero;
            onCampPointUsed.EventListeners += InitializeDisplay;
            

            for (int _i = 0; _i < PlayerData.GetInstance().Heroes.Count; _i++)
            {
                PlayerData.GetInstance().Heroes[_i].Spawn(battlesHeroes[_i]);
                portraits[_i].Initialize(PlayerData.GetInstance().Heroes[_i]);
            }

            actualHero = battlesHeroes[0];
            
            InitializeDisplay(0);
        }

        private void OnDisable()
        {
            onHeroChange.EventListeners -= ChangeHero;
            onCampPointUsed.EventListeners -= InitializeDisplay;
        }


        private void InitializeDisplay(int _empty)
        {
            ClearDecks();

            DeckMono _deck = GameObject.FindObjectOfType<DeckMono>();
            _deck.UpdateDeck();
            _deck.InitializeForCamp();
            
            foreach (SkillSo _skillSo in _deck.drawPile)
            {
                GameObject _slot = GameObject.Instantiate(prefabSlot.gameObject, deckPlaceHolder);
                _slot.GetComponent<SlotDragAndDrop>().cellType = SlotDragAndDrop.CellType.DragOnly;
                _slot.GetComponent<SlotDragAndDrop>().unlimitedSource = true;
                _slot.GetComponent<SlotDragAndDrop>().containType = SlotDragAndDrop.ContainType.Skill;

                GameObject _pref = GameObject.Instantiate(prefabSkill.gameObject, _slot.transform);
                _pref.GetComponent<SkillInfo>().skill = Skill.CreateSkill(_skillSo, _deck, actualHero);
                _pref.GetComponent<SkillInfo>().unit = actualHero;
                _pref.GetComponent<SkillInfo>().DisplayIcon();
                
                _slot.GetComponent<SlotDragAndDrop>().UpdateMyItem();
                _slot.GetComponent<SlotDragAndDrop>().UpdateBackgroundState();

                AllSkills.Add(_pref.GetComponent<SkillInfo>());
            }
        }

        private void ClearDecks()
        {
            
            deckPlaceHolder.Clear();

            AllSkills = new List<SkillInfo>();
        }

        private void ChangeHero(int _index)
        {
            DeckMono _deck = GameObject.FindObjectOfType<DeckMono>();
            actualHero = battlesHeroes[_index];
            foreach (SkillInfo _skill in AllSkills)
            {
                SkillSo _s = _skill.skill.BaseSkill;
                _skill.skill = Skill.CreateSkill(_s, _deck, actualHero);
            }
        }
    }
}