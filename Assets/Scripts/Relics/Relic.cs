using System.Collections.Generic;
using Skills;
using Skills._Zone;
using Stats;
using StatusEffect;

namespace Relics
{
    public class Relic
    {
        public List<RelicSO> RelicEffects { get; private set; }
        public List<IEffect> Effects { get; private set; }
        public EAffect Affect { get; private set; }
        public Range Range { get; private set; }
        public int Power { get; private set; }
        public int Cost { get; private set; }
        public List<StatusSO> StatusEffects { get; private set; }
        
        
        public static Relic CreateRelic(List<RelicSO> relics)
        {
            Relic relic = new Relic();
            
            relic.Range = new Range(0);
            relic.Power = 0;
            relic.Cost = 0;
            relic.StatusEffects = new List<StatusSO>();
            relic.Effects = new List<IEffect>();
            relic.RelicEffects = relics;
            
            foreach (RelicSO _relic in relics)
            {
                relic.Cost += _relic.Cost;
                relic.Range += _relic.Range;
                relic.Power += _relic.Power;
                relic.StatusEffects.AddRange(_relic.StatusEffects);
                
                if (_relic.Effect != null) relic.Effects.Add(_relic.Effect);
                if (_relic.GridEffect != null) relic.Effects.Add(_relic.GridEffect);
            }

            return relic;
        }
    }
}