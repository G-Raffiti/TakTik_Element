using System.Collections.Generic;
using Buffs;
using Gears;
using Relics;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ToolTips
{
    public class UnitTooltip : Tooltip
    {
        [HideInInspector] public Unit Unit;
        
        [Header("Unity References")]
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private Image unitSprite;
        [SerializeField] private TextMeshProUGUI statsTxt;
        [SerializeField] private TextMeshProUGUI statsRangeTxt;
        [SerializeField] private Image shadowEffect;
        [SerializeField] private Image frame;
        [SerializeField] private List<Transform> inventory;
        [SerializeField] private Transform relicHolder;
        [SerializeField] private Transform buffsHolder;
        
        [Header("Prefabs")] 
        [SerializeField] private RelicInfo relicPref;
        [SerializeField] private GearInfo gearPref;
        [SerializeField] private BuffInfo buffPref;
        
        protected override void ShowToolTip()
        {
            nameTxt.text = Unit.GetInfoMain();
            unitSprite.sprite = Unit.GetIcon();
            statsTxt.text = Unit.GetInfoLeft();
            statsRangeTxt.text = Unit.GetInfoRight();
            shadowEffect.sprite = Unit.GetIcon();
            frame.color = Unit.GetTeamColor();

            for(int i = 0; i < Unit.Inventory.gears.Count; i ++)
            {
                GameObject pref = Instantiate(gearPref.gameObject, inventory[i].transform);
                pref.GetComponent<GearInfo>().Gear = Unit.Inventory.gears[i];
                pref.GetComponent<GearInfo>().DisplayIcon();
            }

            foreach (RelicSO _relicSO in Unit.Relic.RelicEffects)
            {
                GameObject pref = Instantiate(relicPref.gameObject, relicHolder);
                pref.GetComponent<RelicInfo>().CreateRelic(_relicSO);
                pref.GetComponent<RelicInfo>().DisplayIcon();
            }

            foreach (Buff _buff in Unit.Buffs)
            {
                GameObject pref = Instantiate(buffPref.gameObject, buffsHolder);
                pref.GetComponent<BuffInfo>().Unit = Unit;
                pref.GetComponent<BuffInfo>().Buff = _buff;
                pref.GetComponent<BuffInfo>().DisplayIcon();
            }
        }

        public override void HideTooltip()
        {
            while (relicHolder.childCount > 0)
            {
                DestroyImmediate(relicHolder.GetChild(0).gameObject);
            }

            while (buffsHolder.childCount > 0)
            {
                DestroyImmediate(buffsHolder.GetChild(0).gameObject);
            }

            foreach (Transform _transform in inventory)
            {
                while (_transform.childCount > 0)
                {
                    DestroyImmediate(_transform.GetChild(0).gameObject);
                }
            }
            base.HideTooltip();
        }
    }
}