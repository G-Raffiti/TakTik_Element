﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using Cells;
using Resources.ToolTip.Scripts;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using StateMachine;
using Stats;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Skills
{
    /// <summary>
    /// Class to attach to a Button that will give access to a Deck to Use Skills
    /// </summary>
    public class SkillInfo : InfoBehaviour
    {
        [SerializeField] private Image icon;
        
        [Header("Event Sender")] 
        [SerializeField] private SkillEvent onSkillSelected;
        [SerializeField] private VoidEvent OnSkillUsed;

        public Unit Unit;
        public Skill skill;

        /// <summary>
        /// Methode Called on Skill Use, it will trigger all Skill Effects and Grid Effects on the Affected Cells
        /// </summary>
        /// <param name="_cell">Cell Clicked</param>
        /// <returns></returns>
        public void UseSkill(Cell _cell)
        {
            if (skill.Unit.playerNumber == 0)
                if (!skill.Deck.UseSkill(skill)) return;
            if (Unit.BattleStats.AP < skill.Cost) return;
            if (skill.Effects.Any(_effect => !_effect.CanUse(_cell, this)))
                return;
            Debug.Log($"{Unit.UnitName} Use {ColouredName()}");
            if (skill.Effects.Find(_effect => _effect is Learning) != null)
            {
                skill.Effects.Find(_effect => _effect is Learning).Use(_cell, this);
                OnSkillUsed.Raise();
                return;
            }
            
            Unit.BattleStats.AP -= skill.Cost;
            
            //TODO : Play Skill animation
            List<Cell> _zone = Zone.GetZone(skill.Range, _cell);
            _zone.Sort((_cell1, _cell2) =>
                _cell1.GetDistance(Unit.Cell).CompareTo(_cell2.GetDistance(Unit.Cell)));
            StartCoroutine(HighlightZone(_zone));
            
            foreach (IEffect _effect in skill.Effects)
            {
                _effect.Use(_cell, this);
            }

            OnSkillUsed.Raise();
            if(BattleStateManager.instance.PlayingUnit.playerNumber == 0)
                DestroyImmediate(this.gameObject);
        }
        
        private static IEnumerator HighlightZone(List<Cell> zone)
        {
            foreach (Cell _cell in zone)
            {
                _cell.MarkAsHighlighted();
                yield return new WaitForSeconds(0.05f);
            }
            foreach (Cell _cell in zone)
            {
                _cell.MarkAsHighlighted();
            }
            yield return new WaitForSeconds(0.2f);
            zone.ForEach(c => c.UnMark());
        }

        /// <summary>
        /// Return the Zone to Highlight to show the cell touched by the Actual Skill
        /// </summary>
        /// <param name="_cell"></param>
        /// <returns></returns>
        public List<Cell> GetZoneOfEffect(Cell _cell)
        {
            return Zone.GetZone(skill.Range, _cell);
        }
        
        public List<Cell> GetRangeFrom(Cell _cell)
        {
            return skill.Range.NeedView ? Zone.CellsInView(skill, _cell) : Zone.CellsInRange(skill, _cell);
        }
        
        #region IInfo
        public override string GetInfoMain()
        {
            return $"{ColouredName()}\n{skill.BaseSkill.Type} of {skill.Element.Name}\ncost: {skill.Cost} <sprite name=AP>";
        }

        public override string GetInfoLeft()
        {
            string str = "";
            skill.Effects.ForEach(effect => str += effect.InfoEffect(this) + "\n");
            return str;
        }

        public override string GetInfoRight()
        {
            return skill.Range.ToString();
        }

        public override string GetInfoDown()
        {
            string str = "";
            skill.Buffs.ForEach(buff => str += buff.InfoBuff());
            return str;
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

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (Unit.BattleStats.AP >= skill.Cost)
                onSkillSelected?.Raise(this);
        }
        
        public override void DisplayIcon()
        {
            if (icon != null)
                icon.sprite = GetIcon();
        }

        #endregion

        public int GetPower(EElement _elementType)
        {
            return skill.Power + Unit.BattleStats.GetPower(_elementType);
        }
    }
}