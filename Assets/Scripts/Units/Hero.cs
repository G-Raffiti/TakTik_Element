using System;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _Instances;
using EndConditions;
using Gears;
using Relics;
using Stats;
using UnityEngine;

namespace Units
{
    
    /// <summary>
    /// Stats of the Characters
    /// </summary>
    public class Hero : MonoBehaviour
    {
        [SerializeField] private string unitName;
        [SerializeField] private BattleStats battleStats;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Sprite unitSprite;
        [SerializeField] private Sprite icon;
        [SerializeField] private Inventory inventory;
        [SerializeField] private List<RelicSo> relics = new List<RelicSo>();


        private Unit unit;

        public string UnitName => unitName;
        public BattleStats BattleStats => battleStats;
        public BattleStats TotalStats => new BattleStats(battleStats + Inventory.GearStats() + GetRelic().BattleStats);
        public int ActualHp { get; private set; }
        public GameObject Prefab => prefab;
        public Sprite UnitSprite => unitSprite;
        public Sprite Icon => icon;
        public Inventory Inventory => inventory;

        public List<RelicSo> Relics => relics;
        public Unit Unit => unit;

        public bool isPlaced = false;
        public bool isDead = false;

        public void Spawn(BattleHero _pref)
        {
            if (BattleStage.currentState == EConditionType.LootBox)
            {
                ActualHp = BattleStats.hp;
            }
            BattleHero _hero = _pref.GetComponent<BattleHero>();
            if (_hero != null)
            {
                _hero.Spawn(this);
            }
            if (unit != null) return;
            unit = _pref;
            unit.UnitAttacked += Unit_UnitAttacked;
            unit.UnitDestroyed += Unit_UnitDestroyed;
        }

        public void UpdateHp()
        {
            ActualHp = unit.battleStats.hp;
        }
        private void Unit_UnitDestroyed(object _sender, DeathEventArgs _e)
        {
            isDead = true;
        }

        private void Unit_UnitAttacked(object _sender, AttackEventArgs _e)
        {
            UpdateHp();
        }
        
        
        public void HealHp(int _percent)
        {
            ActualHp = Math.Min(TotalStats.hp, ActualHp + (int) (TotalStats.hp * (_percent / 100f)));
        }
        
        public void HealFixValueHp(int _fixValue)
        {
            ActualHp = Math.Min(TotalStats.hp, ActualHp + _fixValue);
        }

        public Relic GetRelic()
        {
            return Relic.CreateRelic(relics);
        }

        public object CaptureState()
        {
            return new SaveHero(this);
        }

        public void RestoreState(object _state)
        {
            if (!(_state is SaveHero _save))
            {
                Debug.LogError("Error in save system");
                return;
            }
            
            unitName = _save.unitName;
            unitSprite = _save.sprite;
            icon = _save.icon;
            battleStats = _save.battleStats;
            inventory = new Inventory(_save.inventory);
        }

        public void AddRelic(RelicSo _relic)
        {
            relics.Add(_relic);
        }
    }

    [Serializable]
    public class SaveHero
    {
        public string unitName;
        public Sprite sprite;
        public Sprite icon;
        public BattleStats battleStats;
        public SaveInventory inventory;

        public SaveHero(Hero _hero)
        {
            unitName = _hero.UnitName;
            sprite = _hero.UnitSprite;
            icon = _hero.Icon;
            battleStats = _hero.BattleStats;
            inventory = new SaveInventory(_hero.Inventory);
        }
    }
}