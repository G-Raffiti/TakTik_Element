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
        [SerializeField] private List<GearSo> gears;

        public List<GearSo> Gears => gears;
        
        public GearSo GetRandom()
        {
            List<GearSo> _weightedList = new List<GearSo>();
            foreach (GearSo _gear in Gears)
            {
                for (int _i = 0; _i < Math.Abs(_gear.Rarity.Affixes - 5); _i++)
                {
                    _weightedList.Add(_gear);
                }
            }
            return _weightedList.GetRandom();
        }

        public void AddGear(GearSo _newGear)
        {
            if (gears.Contains(_newGear)) return;
            #if (UNITY_EDITOR)
            gears.Add(_newGear);
            EditorUtility.SetDirty(this); 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            #endif
        }
        
        public void ClearDataBase()
        {
            #if (UNITY_EDITOR)
            gears = new List<GearSo>();
            #endif
        }
    }
}