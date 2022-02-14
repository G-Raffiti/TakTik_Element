using _DragAndDropSystem;
using _Extension;
using Relics;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.BattleScene
{
    public class HeroRelicSlot : MonoBehaviour
    {
        private Hero hero;
        [SerializeField] private TextMeshProUGUI heroNameTxt;
        [SerializeField] private SlotDragAndDrop relicSlot;
        [SerializeField] private Transform relicsHolder;
        [SerializeField] private Image heroPortrait;

        [SerializeField] private RelicInfo relicPrefab;

        private void OnEnable()
        {
            relicsHolder.Clear();
        }

        public void Initialize(Hero _hero)
        {
            hero = _hero;
            heroNameTxt.text = hero.UnitName;
            heroPortrait.sprite = hero.UnitSprite;

            foreach (RelicSO _relic in hero.Relics)
            {
                GameObject _pref = Instantiate(relicPrefab.gameObject, relicsHolder);
                _pref.GetComponent<RelicInfo>().CreateRelic(_relic);
                _pref.GetComponent<RelicInfo>().DisplayIcon();
            }
        }

        public void ApplyAndClose()
        {
            if (relicSlot.GetInfoRelic() != null)
            {
                hero.AddRelic(relicSlot.GetInfoRelic().Relic);
            }
        }

        private void OnDisable()
        {
            relicsHolder.Clear();
            relicSlot.RemoveItem();
        }
    }
}