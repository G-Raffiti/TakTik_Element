using System;
using System.Collections.Generic;
using System.Linq;
using _Extension;
using Gears;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DataBases
{
    [CreateAssetMenu(fileName = "DataBase_Gear", menuName = "Scriptable Object/DataBase/Gear")]
    public class DataBaseGear : ScriptableObject
    {
        [SerializeField] private List<GearSO> gears;

        public List<GearSO> Gears => gears;
        
        public GearSO GetRandom()
        {
            List<GearSO> weightedList = new List<GearSO>();
            foreach (GearSO _gear in Gears)
            {
                for (int i = 0; i < Math.Abs(_gear.Rarity.Affixes - 5); i++)
                {
                    weightedList.Add(_gear);
                }
            }
            return weightedList.GetRandom();
        }

        public void AddGear(GearSO newGear)
        {
            if (gears.Contains(newGear)) return;
            #if (UNITY_EDITOR)
            gears.Add(newGear);
            EditorUtility.SetDirty(this); 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            #endif
        }
        
        public void ClearDataBase()
        {
            #if (UNITY_EDITOR)
            gears = new List<GearSO>();
            #endif
        }
    }
}