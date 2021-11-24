using System.Collections.Generic;
using _ScriptableObject;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Skills.ScriptableObject_GridEffect;
using Stats;
using StatusEffect;
using Units;
using UnityEngine;

namespace Skills
{
    public class Skill
    {
        public SkillSO skillso { get; set; }
        public string name { get; set; }
        public Element element { get; set; }
        public Sprite icon { get; set; }
        
        public List<IEffect> effects { get; set; }
        public EAffect affect { get; set; }
        
        public Range range { get; set; }
        public int power { get; set; }
        public List<StatusSO> statusEffects { get; set; }

        public bool consumable { get; set; }
        public int cost { get; set; }
        public EArchetype archetype { get; set; }


        public Skill(SkillSO skillso,
                     string name, 
                     Element element, 
                     Sprite icon, 
                     List<IEffect> effects,
                     EAffect affect, 
                     Range range, 
                     int power, 
                     List<StatusSO> statusEffects, 
                     bool consumable, 
                     int cost, 
                     EArchetype archetype)
        {
            this.skillso = skillso;
            this.name = name;
            this.element = element;
            this.icon = icon;
            this.effects = effects;
            this.affect = affect;
            this.range = range;
            this.power = power;
            this.statusEffects = statusEffects;
            this.consumable = consumable;
            this.cost = cost;
            this.archetype = archetype;
        }
        
        public Skill(SkillSO skill)
        {
            this.skillso = skill;
            this.name = skill.Name;
            this.element = skill.Element;
            this.icon = skill.Icon;
            this.effects.AddRange(skill.Effects);
            this.effects.Add(skill.GridEffect);
            this.affect = skill.Affect;
            this.range = skill.Range;
            this.power = skill.Power;
            this.statusEffects = skill.StatusEffects;
            this.consumable = skill.Consumable;
            this.cost = skill.Cost;
            this.archetype = skill.Archetype;
        }
    }
}