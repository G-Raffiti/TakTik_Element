using Buffs;
using Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ToolTips
{
    public class SkillTooltip : Tooltip
    {
        [HideInInspector]
        public SkillInfo Skill;

        [SerializeField] private TextMeshProUGUI Name;
        [SerializeField] private Image elementIcon;
        [SerializeField] private TextMeshProUGUI cost;
        [SerializeField] private TextMeshProUGUI costShadow;
        [SerializeField] private Image illustration;
        [SerializeField] private TextMeshProUGUI effectTxt;
        [SerializeField] private TextMeshProUGUI rangeTxt;
        [SerializeField] private Image frame;
        [SerializeField] private Image shadowEffect;
        [SerializeField] private Transform buffsHolder;
        [SerializeField] private BuffInfo buffPref;

        protected override void ShowToolTip()
        {
            Name.text = Skill.skill.BaseSkill.Name;
            elementIcon.sprite = Skill.skill.Element.Icon;
            cost.text = $"{Skill.skill.Cost}";
            costShadow.text = $"{Skill.skill.Cost}";
            illustration.sprite = Skill.GetIcon();
            effectTxt.text = Skill.GetInfoLeft();
            rangeTxt.text = Skill.GetInfoRight();
            frame.color = Skill.skill.Element.TextColour;
            if (Skill.skill.BaseSkill.Monster != null)
            {
                shadowEffect.color = new Color(0,0,0,0.8f);
                shadowEffect.sprite = Skill.skill.BaseSkill.Monster.UnitSprite;
                return;
            }
            shadowEffect.color = Color.clear;
            foreach (Buff _buff in Skill.skill.Buffs)
            {
                GameObject _pref = Instantiate(buffPref.gameObject, buffsHolder);
                _pref.GetComponent<BuffInfo>().Buff = _buff;
                _pref.GetComponent<BuffInfo>().DisplayIcon();
            }
        }
        
        public override void HideTooltip()
        {
            while (buffsHolder.childCount > 0)
            {
                DestroyImmediate(buffsHolder.GetChild(0).gameObject);
            }
            
            base.HideTooltip();
        }
    }
}