using System.Collections.Generic;
using Cells;
using UnityEngine;

namespace Skills
{
    /// <summary>
    /// All Effect that can be triggered on a Skill Use
    /// </summary>
    public abstract class IEffect : ScriptableObject
    {
        /// <summary>
        /// Time when this effect is Used (before or after the main Skill effect)
        /// </summary>
        [SerializeField] private EActionsOrder actionsOrder;
        public EActionsOrder ActionsOrder => actionsOrder;
        
        /// <summary>
        /// Methode should be called by the Deck
        /// </summary>
        /// <param name="_cell">the Targeted Cell</param>
        /// <param name="_skillInfo">skill that will be used (mainly for the Power and Range)</param>
        public abstract void Use(Cell _cell, SkillInfo _skillInfo);

        /// <summary>
        /// Infos of the Effect with value for the Tooltip
        /// </summary>
        public abstract string InfoEffect(SkillInfo _skillInfo);

        /// <summary>
        /// Infos of effects you can see on Relics
        /// </summary>
        public abstract string InfoEffect();

        /// <summary>
        /// Boolean that tell if the skill is usable on a certain Cell 
        /// </summary>
        /// <returns></returns>
        public abstract bool CanUse(Cell _cell, SkillInfo _skillInfo);
        
        /// <summary>
        /// Return the Value of effect on each Cell in Zone
        /// </summary>
        public abstract Dictionary<Cell, int> DamageValue(Cell _cell, SkillInfo _skillInfo);
        
    }
    public enum EActionsOrder
    {
        First = 0,
        Second = 1,
        Before = 4,
        Skill = 5,
        After = 6,
        Late = 9,
        Last = 10,
    }
}