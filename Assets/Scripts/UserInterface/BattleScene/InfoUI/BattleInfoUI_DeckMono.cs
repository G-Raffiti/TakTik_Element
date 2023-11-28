using System;
using System.Collections.Generic;
using System.Linq;
using _DragAndDropSystem;
using _Extension;
using Decks;
using Players;
using Skills;
using StateMachine;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace UserInterface.BattleScene.InfoUI
{
    public class BattleInfoUIDeckMono : MonoBehaviour
    {
        [FormerlySerializedAs("SkillPrefab")]
        [SerializeField] private SkillInfo skillPrefab;
        [FormerlySerializedAs("DrawPile")]
        [SerializeField] private Transform drawPile;
        [FormerlySerializedAs("DiscardPile")]
        [SerializeField] private Transform discardPile;
        [FormerlySerializedAs("ConsumedPile")]
        [SerializeField] private Transform consumedPile;
        [FormerlySerializedAs("Hero")]
        [SerializeField] private BattleHero hero;
        [SerializeField] private Hero blancHero;
        [SerializeField] private DeckMono deck;


        private void OnEnable()
        {
            Unit _nextHero;
            if (deck == null)
            {
                deck = GameObject.Find("DeckMono/Deck1").GetComponent<DeckMono>();
            }
            if (BattleStateManager.instance.PlayingUnit == null) return;
            if (BattleStateManager.instance.PlayingUnit.playerType == EPlayerType.Human)
                _nextHero = BattleStateManager.instance.PlayingUnit;
            else
            {
                hero.Spawn(blancHero);
                _nextHero = hero;
            }

            SetPile(deck.drawPile, _nextHero, drawPile);
            SetPile(deck.discardPile, _nextHero, discardPile);
            SetPile(deck.consumedSkills, _nextHero, consumedPile);
        }

        private void SetPile(List<SkillSo> _pile, Unit _user, Transform _holder)
        {
            List<SkillSo> _copie = new List<SkillSo>();
            _copie.AddRange(_pile);
            _copie.Sort((_s, _s1) => String.Compare(_s.Name, _s1.Name, StringComparison.Ordinal));
            _copie.Sort((_s, _s1) => _s.Cost.CompareTo(_s1.Cost));
            foreach (SkillSo _skillSo in _copie)
            {
                GameObject _skillObj = Instantiate(skillPrefab.gameObject, _holder);
                
                _skillObj.GetComponent<SkillInfo>().skill = Skill.CreateSkill(_skillSo, deck, _user);
                _skillObj.GetComponent<SkillInfo>().unit = _user;

                _skillObj.GetComponent<SkillInfo>().DisplayIcon();
            }
        }

        private void OnDisable()
        {
            EmptyPile(drawPile);
            EmptyPile(discardPile);
            EmptyPile(consumedPile);
        }

        private void EmptyPile(Transform _pile)
        {
            int _i = 0;
            //Array to hold all child obj
            GameObject[] _allChildren = new GameObject[_pile.childCount - 2];

            //Find all child obj and store to that array
            foreach (Transform _child in _pile)
            {
                if (_child.GetComponent<SkillInfo>() == null) continue;
                _allChildren[_i] = _child.gameObject;
                _i += 1;
            }

            //Now destroy them
            foreach (GameObject _child in _allChildren)
            {
                DestroyImmediate(_child.gameObject);
            }
            
        }
    }
}