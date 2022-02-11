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
        [SerializeField] private Image illustration;
        [SerializeField] private TextMeshProUGUI effectTxt;
        [SerializeField] private TextMeshProUGUI rangeTxt;
        [SerializeField] private Image frame;
        [SerializeField] private Image illustrationFrame;
        [SerializeField] private Image shadowEffect;
        [SerializeField] private Transform buffsHolder;
        [SerializeField] private BuffInfo buffPref;

        protected override void ShowToolTip()
        {
            Name.text = Skill.skill.BaseSkill.Name;
            elementIcon.sprite = Skill.skill.Element.Icon;
            cost.text = $"{Skill.skill.Cost}";
            illustration.sprite = Skill.GetIcon();
            effectTxt.text = Skill.GetInfoLeft();
            rangeTxt.text = Skill.GetInfoRight();
            frame.color = Skill.skill.Element.TextColour;
            illustrationFrame.color = Skill.skill.Element.TextColour;
            foreach (Buff _buff in Skill.skill.Buffs)
            {
                GameObject _pref = Instantiate(buffPref.gameObject, buffsHolder);
                _pref.GetComponent<BuffInfo>().Buff = _buff;
                _pref.GetComponent<BuffInfo>().DisplayIcon();
            }
            if (Skill.skill.BaseSkill.Monster != null)
            {
                shadowEffect.color = new Color(0,0,0,0.8f);
                shadowEffect.sprite = Skill.skill.BaseSkill.Monster.UnitSprite;
            }
            else shadowEffect.color = Color.clear;
        }
        
        public override void HideTooltip()
        {
            while (buffsHolder.childCount > 0)
            {
                DestroyImmediate(buffsHolder.GetChild(0).gameObject);
            }
            
            base.HideTooltip();
        }

        protected override Vector3 LockPosition()
        {
            return lockPosition - backgroundRectTransform.rect.size / 2;
        }
    }
}