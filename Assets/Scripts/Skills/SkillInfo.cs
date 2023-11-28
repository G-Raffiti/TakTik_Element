using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using Cells;
using Resources.ToolTip.Scripts;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using _LeanTween.Framework;
using Players;
using UnityEngine.Serialization;

namespace Skills
{
    /// <summary>
    /// Class to attach to a Button that will give access to a Deck to Use Skills
    /// </summary>
    public class SkillInfo : InfoBehaviour
    {
        [Header("Unity References")]
        [SerializeField] private Image illustration;
        [SerializeField] private Image elementIcon;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Image colorFrame;
        [SerializeField] private Image canUse;
        [SerializeField] private bool isHandSkill;
        
        [Header("Event Sender")] 
        [SerializeField] private SkillEvent onSkillSelected;
        [FormerlySerializedAs("OnSkillUsed")]
        [SerializeField] private VoidEvent onSkillUsed;
        
        [FormerlySerializedAs("SkillTooltip_ON")]
        [Header("Tooltip Events")] 
        [SerializeField] private SkillEvent skillTooltipOn;
        [FormerlySerializedAs("SkillTooltip_OFF")]
        [SerializeField] private VoidEvent skillTooltipOff;

        [FormerlySerializedAs("Unit")]
        [Header("Only if Needed")]
        public Unit unit;
        public Skill skill;

        /// <summary>
        /// Methode Called on Skill Use, it will trigger all Skill Effects and Grid Effects on the Affected Cells
        /// </summary>
        /// <param name="_cell">Cell Clicked</param>
        /// <returns></returns>
        public void UseSkill(Cell _cell)
        {
            if (skill.Unit.playerType == EPlayerType.Human)
                if (!skill.Deck.UseSkill(skill)) return;
            if (unit.battleStats.ap < skill.Cost) return;
            if (skill.Effects.Any(_effect => !_effect.CanUse(_cell, this)))
                return;
            Debug.Log($"{unit.ColouredName()} Use {ColouredName()}");
            if (skill.Effects.Find(_effect => _effect is Learning) != null)
            {
                skill.Effects.Find(_effect => _effect is Learning).Use(_cell, this);
                onSkillUsed.Raise();
                return;
            }
            
            unit.battleStats.ap -= skill.Cost;
            
            //TODO : Play Skill animation
            List<Cell> _zone = Zone.GetZone(skill.GridRange, _cell);
            _zone.Sort((_cell1, _cell2) =>
                _cell1.GetDistance(unit.Cell).CompareTo(_cell2.GetDistance(unit.Cell)));
            StartCoroutine(HighlightZone(_zone));
            
            foreach (Effect _effect in skill.Effects)
            {
                _effect.Use(_cell, this);
            }

            onSkillUsed.Raise();
        }
        
        private static IEnumerator HighlightZone(List<Cell> _zone)
        {
            foreach (Cell _cell in _zone)
            {
                _cell.MarkAsHighlighted();
                yield return new WaitForSeconds(0.05f);
            }
            foreach (Cell _cell in _zone)
            {
                _cell.MarkAsHighlighted();
            }
            yield return new WaitForSeconds(0.2f);
            _zone.ForEach(_c => _c.UnMark());
        }

        /// <summary>
        /// Return the Zone to Highlight to show the cell touched by the Actual Skill
        /// </summary>
        /// <param name="_cell"></param>
        /// <returns></returns>
        public List<Cell> GetZoneOfEffect(Cell _cell)
        {
            return Zone.GetZone(skill.GridRange, _cell);
        }
        
        public List<Cell> GetRangeFrom(Cell _cell)
        {
            return skill.GridRange.needView ? Zone.CellsInView(skill, _cell) : Zone.CellsInRange(skill, _cell);
        }
        
        #region IInfo
        public override string GetInfoMain()
        {
            return $"{ColouredName()}\n{skill.BaseSkill.Type} of {skill.Element.Name}\ncost: {skill.Cost} <sprite name=AP>";
        }

        public override string GetInfoLeft()
        {
            string _str = "";
            skill.Effects.ForEach(_effect => _str += _effect.InfoEffect(this) + "\n");
            return _str;
        }

        public override string GetInfoRight()
        {
            return skill.GridRange.ToString();
        }

        public override string GetInfoDown()
        {
            string _str = "";
            skill.Buffs.ForEach(_buff => _str += _buff.InfoBuff());
            return _str;
        }

        public override Sprite GetIcon()
        {
            return skill.BaseSkill.Icon;
        }

        public override string ColouredName()
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(skill.Element.TextColour);
            return $"<color=#{_hexColor}>{skill.BaseSkill.Name}</color>";
        }

        public override void OnPointerEnter(PointerEventData _eventData)
        {
            skillTooltipOn?.Raise(this);
            if (!isHandSkill) return;
            LeanTween.scale(this.gameObject, new Vector3(1.2f, 1.2f, 1), 0.2f);
        }

        public override void OnPointerExit(PointerEventData _eventData)
        {
            skillTooltipOff?.Raise();
            if (!isHandSkill) return;
            LeanTween.scale(this.gameObject, Vector3.one, 0.2f);
        }

        public override void OnPointerClick(PointerEventData _eventData)
        {
            if (!isHandSkill) return;
            if (unit.battleStats.ap >= skill.Cost)
                onSkillSelected.Raise(this);
        }
        
        public override void DisplayIcon()
        {
            if (illustration != null)
                illustration.sprite = GetIcon();
            if (elementIcon != null)
                elementIcon.sprite = skill.Element.Icon;
            if (costText != null)
                costText.text = $"{skill.Cost}";
            if (colorFrame != null)
                colorFrame.color = skill.Element.TextColour;
            CanUse();
        }

        public void CanUse()
        {
            if (canUse == null) return;
            if (!isHandSkill) return;

            canUse.color = skill.Cost <= skill.Unit.battleStats.ap ? Color.white : Color.grey;
            illustration.color = skill.Cost <= skill.Unit.battleStats.ap ? Color.white : new Color(1, 1, 1, 0.5f);
        }

        #endregion

        public int GetPower()
        {
            return skill.Power;
        }
    }
}