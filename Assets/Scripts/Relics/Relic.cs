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
        public BattleStats BattleStats { get; private set; }
        public List<StatusSO> StatusEffects { get; private set; }

        public Relic()
        {
            RelicEffects = new List<RelicSO>();
            Effects = new List<IEffect>();
            Affect = EAffect.All;
            BattleStats = new BattleStats();
            StatusEffects = new List<StatusSO>();
        }
        public static Relic CreateRelic(List<RelicSO> relics)
        {
            Relic relic = new Relic();

            relic.BattleStats = new BattleStats();
            relic.StatusEffects = new List<StatusSO>();
            relic.Effects = new List<IEffect>();
            relic.RelicEffects = relics;
            
            foreach (RelicSO _relic in relics)
            {
                relic.BattleStats += _relic.BattleStats;
                relic.StatusEffects.AddRange(_relic.StatusEffects);
                
                if (_relic.Effect != null) relic.Effects.Add(_relic.Effect);
                if (_relic.GridEffect != null) relic.Effects.Add(_relic.GridEffect);
            }

            return relic;
        }
    }
}