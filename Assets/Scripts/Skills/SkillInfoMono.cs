using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using _ScriptableObject;
using Cells;
using Grid;
using Grid.GridStates;
using Resources.ToolTip.Scripts;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Stats;
using StatusEffect;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Range = Stats.Range;

namespace Skills
{
    /// <summary>
    /// Class to attach to a Button that will give access to a Deck to Use Skills
    /// </summary>
    public class SkillInfoMono : InfoBehaviour
    {
        [SerializeField] private Image icon;
        
        [Header("Event Sender")] 
        [SerializeField] private SkillEvent onSkillSelected;
        [SerializeField] private VoidEvent OnSkillUsed;

        public Unit Unit;
        public Skill skill;

        /// <summary>
        /// Method Called by the IA to Use a Skill that is not in the Deck
        /// </summary>
        /// <param name="skill">
        /// the skill to Use
        /// </param>
        /// <param name="cell">
        /// the targeted Cell
        /// </param>
        /// <param name="sender">
        /// the Unit who use the skill
        /// </param>
        public void UseSkill(SkillSO skill, Cell cell, Unit sender)
        {
            /*
            UpdateSkill(skill, sender);
            UseSkill(cell);
            Deck.NextSkill();
            UpdateSkill(0, Unit);
            */
        }
        
        /// <summary>
        /// Method Called buy the Player by Clicking in the Icon in BattleScene
        /// </summary>
        /// <param name="cell"></param>
        public void UseSkill(Cell cell)
        {
            if (Unit.BattleStats.AP < skill.cost) return;
            //if (!deck.UseSkill(this, cell)) return;

            Unit.BattleStats.AP -= skill.cost;

            OnSkillUsed?.Raise();
        }
        
        /// <summary>
        /// Methode Called on Skill Use, it will trigger all Skill Effects and Grid Effects on the Affected Cells
        /// </summary>
        /// <param name="_cell">Cell Clicked</param>
        /// <returns></returns>
        public void UseSkill(SkillInfo _skillInfo, Cell _cell)
        {
            if (Unit.BattleStats.AP < skill.cost) return;
            if (skill.effects.Any(_effect => !_effect.CanUse(_cell, _skillInfo)))
                return;
            Debug.Log($"{_skillInfo.Unit} Use {_skillInfo.ColouredName()}");
            if (skill.effects.Find(_effect => _effect is Learning) != null)
            {
                skill.effects.Find(_effect => _effect is Learning).Use(_cell, _skillInfo);
                return;
            }
            
            Unit.BattleStats.AP -= skill.cost;
            
            //TODO : Play Skill animation
            List<Cell> _zone = Zone.GetZone(_skillInfo.Range, _cell);
            _zone.Sort((_cell1, _cell2) =>
                _cell1.GetDistance(_skillInfo.Unit.Cell).CompareTo(_cell2.GetDistance(_skillInfo.Unit.Cell)));
            StartCoroutine(HighlightZone(_zone));
            
            foreach (IEffect _effect in skill.effects)
            {
                _effect.Use(_cell, _skillInfo);
            }

            OnSkillUsed.Raise();
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
            return Zone.GetZone(skill.range, _cell);
        }
        
        public List<Cell> GetRangeFrom(Cell _cell)
        {
            return null; //skill.range.NeedView ? Zone.CellsInView(this, _cell) : Zone.CellsInRange(this, _cell);
        }
        
        #region IInfo
        public override string GetInfoMain()
        {
            return $"{ColouredName()}\n{skill.skillso.Type} of {skill.element.Name}\ncost: {skill.cost} <sprite name=AP>";
        }

        public override string GetInfoLeft()
        {
            string str = "";
            skill.effects.ForEach(effect => str += /*effect.InfoEffect(this) +*/ "\n");
            return str;
        }

        public override string GetInfoRight()
        {
            return skill.range.ToString();
        }

        public override string GetInfoDown()
        {
            string str = "";
            Buffs.ForEach(buff => str += buff.InfoBuff());
            return str;
        }

        public override Sprite GetIcon()
        {
            return skill.skillso.Icon;
        }

        public override string ColouredName()
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(skill.element.TextColour);
            return $"<color=#{_hexColor}>{skill.name}</color>";
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            /*
            if (Unit.BattleStats.AP >= skill.cost)
                onSkillSelected?.Raise(this);
            */
        }
        
        public override void DisplayIcon()
        {
            if (icon != null)
                icon.sprite = GetIcon();
        }

        #endregion

        public int GetPower(EElement _elementType)
        {
            return skill.power + Unit.BattleStats.GetPower(_elementType);
        }
    }
}