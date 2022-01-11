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
        private static PlayerData instance;
        private IntEvent OnCraftingMaterialAdded;
        private BoolEvent OnBattleIsEnded;
        private List<Hero> heroes;
        private Dictionary<AffixSO, int> craftingMaterial = new Dictionary<AffixSO, int>();
        public int ResurrectionPoints;
        private bool isVictory;
        public List<Hero> Heroes => heroes;
        public Dictionary<AffixSO, int> CraftingMaterial => craftingMaterial;
        public bool IsVictory => isVictory;

        public static PlayerData getInstance()
        {
            if (instance == null)
                instance = new PlayerData();
            return instance;
            
        }

        private PlayerData()
        {
            OnCraftingMaterialAdded = UnityEngine.Resources.Load<IntEvent>("Game Event/PlayerData_Int_OnCraftingMaterialAdded");
            OnBattleIsEnded = UnityEngine.Resources.Load<BoolEvent>("Game Event/BattleManager_Bool_OnBattleIsOver");
            OnBattleIsEnded.EventListeners += EndVictory;
            heroes = new List<Hero>();
            if (GameObject.Find("Player") != null)
            {
                foreach (Transform _child in GameObject.Find("Player").transform)
                {
                    heroes.Add(_child.GetComponent<Hero>());
                }
            }
            
            foreach (AffixSO _affixSO in DataBase.Affix.AllAffixes)
            {
                craftingMaterial.Add(_affixSO, 0);
            }

            ResurrectionPoints = 2;
        }

        public void AddMaterial(AffixSO _affix, int number)
        {
            if (!craftingMaterial.ContainsKey(_affix))
                craftingMaterial.Add(_affix, number);
            else
                craftingMaterial[_affix] += number;
            OnCraftingMaterialAdded.Raise(number);
        }

        public void RemoveMaterial(AffixSO _affix, int number)
        {
            if (!craftingMaterial.ContainsKey(_affix))
                return;
            craftingMaterial[_affix] -= number;
        }

        public void EndVictory(bool isWin)
        {
            if (BattleStateManager.instance.endCondition.Type == EConditionType.Last || !isWin)
            {
                isVictory = isWin;
            }
        }
    }
}