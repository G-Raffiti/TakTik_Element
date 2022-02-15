using System;
using System.Collections.Generic;
using Buffs;
using Gears;
using Relics;
using Stats;
using TMPro;
using UISetupState;
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
        [SerializeField] private TextMeshProUGUI HPTxt;
        [SerializeField] private Image HPFill;
        [SerializeField] private TextMeshProUGUI ShieldTxt;
        [SerializeField] private Image ShieldFill;
        [SerializeField] private TextMeshProUGUI WaterTxt;
        [SerializeField] private Image WaterFill;
        [SerializeField] private TextMeshProUGUI NatureTxt;
        [SerializeField] private Image NatureFill;
        [SerializeField] private TextMeshProUGUI FireTxt;
        [SerializeField] private Image FireFill;
        [SerializeField] private TextMeshProUGUI PowerTxt;
        [SerializeField] private Image PowerFill;
        [SerializeField] private TextMeshProUGUI SpeedTxt;
        [SerializeField] private TextMeshProUGUI FocusTxt;
        [SerializeField] private TextMeshProUGUI APTxt;
        [SerializeField] private TextMeshProUGUI MPTxt;
        [SerializeField] private TextMeshProUGUI MainElementTxt;
        [SerializeField] private TextMeshProUGUI OtherElementTxt;
        [SerializeField] private TextMeshProUGUI RangeTxt;
        [SerializeField] protected GameObject StatsBlock;
        [SerializeField] private List<Image> frames;
        [SerializeField] private Transform buffsHolder;
        
        [Header("Prefabs")] 
        [SerializeField] protected RelicInfo relicPref;
        [SerializeField] protected GearInfo gearPref;
        [SerializeField] private BuffInfo buffPref;
        
        protected override void ShowToolTip()
        {
            StatsBlock.SetActive(UI_Manager.CompleteStats);
            nameTxt.text = Unit.GetInfoMain();
            unitSprite.sprite = Unit.GetIcon();
            HPTxt.text = $"{Unit.BattleStats.HP}/{Unit.Total.HP}";
            HPFill.fillAmount = Unit.BattleStats.HP / (float) Unit.Total.HP;
            ShieldTxt.text = $"{Unit.BattleStats.Shield}";
            ShieldFill.fillAmount = Unit.BattleStats.Shield / (float) Mathf.Max(Unit.Total.HP, Unit.BattleStats.Shield);
            WaterTxt.text = $"{(int) Unit.BattleStats.Affinity.Water}";
            FireTxt.text = $"{(int) Unit.BattleStats.Affinity.Fire}";
            NatureTxt.text = $"{(int) Unit.BattleStats.Affinity.Nature}";
            PowerTxt.text = $"{Unit.BattleStats.Power}";
            SetDamageBar();
            SpeedTxt.text = $"{Unit.BattleStats.Speed}<sprite name=Speed>";
            FocusTxt.text = $"{Unit.BattleStats.Focus}<sprite name=Focus>";
            APTxt.text = $"{Unit.BattleStats.AP}";
            MPTxt.text = $"{Unit.BattleStats.MP}";
            MainElementTxt.text = Unit.GetInfoDown();
            OtherElementTxt.text = Unit.GetElements();
            RangeTxt.text = Unit.GetInfoRight();
            frames.ForEach(i => i.color = Unit.GetTeamColor());

            foreach (Buff _buff in Unit.Buffs)
            {
                GameObject pref = Instantiate(buffPref.gameObject, buffsHolder);
                pref.GetComponent<BuffInfo>().Unit = Unit;
                pref.GetComponent<BuffInfo>().Buff = _buff;
                pref.GetComponent<BuffInfo>().DisplayIcon();
            }
        }

        private void SetDamageBar()
        {
            float[] values = 
                {
                    Unit.BattleStats.GetPower(EElement.Fire), 
                    Unit.BattleStats.GetPower(EElement.Water), 
                    Unit.BattleStats.GetPower(EElement.Nature), 
                    Unit.BattleStats.GetPower(EElement.None)
                };
            float max = Mathf.Max(values);
            float min = Mathf.Min(values);
            if (min < 0)
            {
                max += Mathf.Abs(min);
            }
            else min = 0;
            FireFill.fillAmount = (Unit.BattleStats.Affinity.Fire - min) / max;
            WaterFill.fillAmount = (Unit.BattleStats.Affinity.Water - min) / max;
            NatureFill.fillAmount = (Unit.BattleStats.Affinity.Nature - min) / max;
            PowerFill.fillAmount = (Unit.BattleStats.Power - min) / max;
        }

        public override void HideTooltip()
        {
            while (buffsHolder.childCount > 0)
            {
                DestroyImmediate(buffsHolder.GetChild(0).gameObject);
            }
            base.HideTooltip();
        }

        protected override Vector3 Position()
        {
            Vector3 _newPos = Camera.main.WorldToScreenPoint(Unit.transform.position) + offset;
            
            float _rightEdgeToScreenEdgeDistance = Screen.width - (_newPos.x + backgroundRectTransform.rect.width * popupCanvas.scaleFactor) - padding.x;
            if (_rightEdgeToScreenEdgeDistance < 0)
            {
                _newPos.x += _rightEdgeToScreenEdgeDistance;
            }

            float _topEdgeToScreenEdgeDistance = Screen.height - (_newPos.y + backgroundRectTransform.rect.height * popupCanvas.scaleFactor) - padding.y;
            if (_topEdgeToScreenEdgeDistance < 0)
            {
                _newPos.y += _topEdgeToScreenEdgeDistance;
            }
            
            return _newPos;
        }
    }
}