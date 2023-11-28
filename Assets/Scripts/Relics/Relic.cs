using System.Collections.Generic;
using Buffs;
using Skills;
using Skills._Zone;
using Stats;

namespace Relics
{
    public class Relic
    {
        public List<RelicSo> RelicEffects { get; private set; }
        public List<Effect> Effects { get; private set; }
        public EAffect Affect { get; private set; }
        public BattleStats BattleStats { get; private set; }
        public List<StatusSo> StatusEffects { get; private set; }

        public Relic()
        {
            RelicEffects = new List<RelicSo>();
            Effects = new List<Effect>();
            Affect = EAffect.All;
            BattleStats = new BattleStats();
            StatusEffects = new List<StatusSo>();
        }
        public static Relic CreateRelic(List<RelicSo> _relics)
        {
            Relic _relic = new Relic();

            _relic.BattleStats = new BattleStats();
            _relic.StatusEffects = new List<StatusSo>();
            _relic.Effects = new List<Effect>();
            _relic.RelicEffects = _relics;
            
            foreach (RelicSo _relicSo in _relics)
            {
                _relic.BattleStats += _relicSo.BattleStats;
                _relic.StatusEffects.AddRange(_relicSo.StatusEffects);
                
                if (_relicSo.Effect != null) _relic.Effects.Add(_relicSo.Effect);
                if (_relicSo.GridEffect != null) _relic.Effects.Add(_relicSo.GridEffect);
            }

            return _relic;
        }
    }
}