﻿using System;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _Instances;
using _SaveSystem;
using Cells;
using Gears;
using Skills._Zone;
using Stats;
using UnityEngine;

namespace Units
{
    
    /// <summary>
    /// Stats of the Characters
    /// </summary>
    public class Hero : MonoBehaviour
    {
        [SerializeField] private BoolEvent onBattleEnd;
        [SerializeField] private string unitName;
        [SerializeField] private BattleStats battleStats;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Sprite unitSprite;
        [SerializeField] private Sprite icon;
        [SerializeField] private Inventory inventory;
        private Unit unit;

        public string UnitName => unitName;
        public BattleStats BattleStats => battleStats;
        public int ActualHP { get; private set; }
        public GameObject Prefab => prefab;
        public Sprite UnitSprite => unitSprite;
        public Sprite Icon => icon;
        public Inventory Inventory => inventory;
        
        public bool isPlaced = false;
        public bool isDead = false;

        public void Spawn(GameObject _pref)
        {
            if (KeepBetweenScene.Stage == 0)
            {
                ActualHP = BattleStats.HP;
            }
            BattleHero _hero = _pref.GetComponent<BattleHero>();
            if (_hero != null)
            {
                _hero.Spawn(this);
            }
            unit = _pref.GetComponent<Unit>();
            unit.UnitAttacked += Unit_UnitAttacked;
            unit.UnitDestroyed += Unit_UnitDestroyed;
        }

        private void Unit_UnitDestroyed(object _sender, DeathEventArgs _e)
        {
            isDead = true;
        }

        private void Unit_UnitAttacked(object _sender, AttackEventArgs _e)
        {
            if (_sender is Unit { } u) ActualHP = u.BattleStats.HP;
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