using System.Collections.Generic;
using _EventSystem.CustomEvents;
using EndConditions;
using StateMachine;
using Stats;
using Units;
using UnityEngine;

namespace _Instances
{
    public class PlayerData
    {
        private static PlayerData _instance;
        private IntEvent onCraftingMaterialAdded;
        private BoolEvent onBattleIsEnded;
        private List<Hero> heroes;
        private Dictionary<AffixSo, int> craftingMaterial = new Dictionary<AffixSo, int>();
        public int ResurrectionPoints;
        private bool isVictory;
        public List<Hero> Heroes => heroes;
        public Dictionary<AffixSo, int> CraftingMaterial => craftingMaterial;
        public bool IsVictory => isVictory;

        public static PlayerData GetInstance()
        {
            if (_instance == null)
                _instance = new PlayerData();
            return _instance;
            
        }

        private PlayerData()
        {
            onCraftingMaterialAdded = UnityEngine.Resources.Load<IntEvent>("Game Event/PlayerData_Int_OnCraftingMaterialAdded");
            onBattleIsEnded = UnityEngine.Resources.Load<BoolEvent>("Game Event/BattleManager_Bool_OnBattleIsOver");
            onBattleIsEnded.EventListeners += EndVictory;
            heroes = new List<Hero>();
            if (GameObject.Find("Player") != null)
            {
                foreach (Transform _child in GameObject.Find("Player").transform)
                {
                    heroes.Add(_child.GetComponent<Hero>());
                }
            }
            
            foreach (AffixSo _affixSo in DataBase.Affix.AllAffixes)
            {
                craftingMaterial.Add(_affixSo, 0);
            }

            ResurrectionPoints = 2;
        }

        public void AddMaterial(AffixSo _affix, int _number)
        {
            if (!craftingMaterial.ContainsKey(_affix))
                craftingMaterial.Add(_affix, _number);
            else
                craftingMaterial[_affix] += _number;
            onCraftingMaterialAdded.Raise(_number);
        }

        public void RemoveMaterial(AffixSo _affix, int _number)
        {
            if (!craftingMaterial.ContainsKey(_affix))
                return;
            craftingMaterial[_affix] -= _number;
        }

        public void EndVictory(bool _isWin)
        {
            if (BattleStateManager.instance.EndCondition.Type == EConditionType.Last || !_isWin)
            {
                isVictory = _isWin;
            }
        }
    }
}