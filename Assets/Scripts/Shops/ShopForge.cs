using System;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _Instances;
using Gears;
using Stats;
using UnityEngine;
using Void = _EventSystem.CustomEvents.Void;

namespace Shops
{
    public class ShopForge : MonoBehaviour
    {
        [Header("Listener")]
        [SerializeField] private ItemEvent onDestroyGear;
        [SerializeField] private VoidEvent onCraftGear;
        [SerializeField] private ItemEvent onUpgradeGear;
        
        [Header("Sender")]
        [SerializeField] private VoidEvent onDiplayUptade;

        private void Start()
        {
            onDestroyGear.EventListeners += DestroyItem;
            onCraftGear.EventListeners += CraftNewItem;
            onUpgradeGear.EventListeners += UpgradeItem;
        }

        private void OnDestroy()
        {
            onDestroyGear.EventListeners -= DestroyItem;
            onCraftGear.EventListeners -= CraftNewItem;
            onUpgradeGear.EventListeners -= UpgradeItem;
        }

        public void DestroyItem(Gear _gear)
        {
            foreach (KeyValuePair<AffixSO,int> _pair in materialFromGear(_gear))
            {
                PlayerData.getInstance().AddMaterial(_pair.Key, _pair.Value);
            }
            onDiplayUptade.Raise();
        }

        public Dictionary<AffixSO, int> materialFromGear(Gear _gear)
        {
            Dictionary<AffixSO, int> ret = new Dictionary<AffixSO, int>();
            foreach (Affix affix in _gear.Affixes)
            { 
                ret.Add(affix.affix, affix.getTier());
            }
            return ret;
        }

        public void CraftNewItem(Void empty)
        {
            Gear gear = new Gear();
            ShopForge_UI _shopForgeUI = GameObject.Find("ForgeUI/Left/Main").GetComponent<ShopForge_UI>();
            if (_shopForgeUI.GetCraftStats() == null)
                return;

            foreach (KeyValuePair<AffixSO,int> _pair in _shopForgeUI.GetCraftStats())
            {
                if (!PlayerData.getInstance().CraftingMaterial.ContainsKey(_pair.Key))
                    return;
                if (PlayerData.getInstance().CraftingMaterial[_pair.Key] < _pair.Value)
                    return;
            }

            foreach (KeyValuePair<AffixSO, int> _pair in _shopForgeUI.GetCraftStats())
            {
                PlayerData.getInstance().RemoveMaterial(_pair.Key, _pair.Value);
            }
            
            gear.CraftNewGear(_shopForgeUI.GetCraftStats());
            
            _shopForgeUI.ShowCraftedGear(gear);
            onDiplayUptade.Raise();
        }
        

        public void UpgradeItem(Gear gear)
        {
            ShopForge_UI _shopForgeUI = GameObject.Find("ForgeUI/Left/Main").GetComponent<ShopForge_UI>();
            Debug.Log("1 Forge founded :" + (_shopForgeUI != null));
            UpgradeGear(gear, _shopForgeUI.GetUpgradeAffix());
            onDiplayUptade.Raise();
        }
        
        public void UpgradeGear(Gear gear, AffixSO affix)
        {
            Debug.Log($"2 Resources Contain {affix} :" + PlayerData.getInstance().CraftingMaterial.ContainsKey(affix));
            if (!PlayerData.getInstance().CraftingMaterial.ContainsKey(affix)) 
                return;
            if (PlayerData.getInstance().CraftingMaterial[affix] < 1)
                return;
            
            List<Affix> old = gear.Affixes;
            string str = "3";
            old.ForEach(a => str += a.ToString());
            Debug.Log(str);
            
            int tier = 0;
            
            foreach (Affix _gearAffix in gear.Affixes)
            {
                if (_gearAffix.affix == affix)
                {
                    tier = _gearAffix.getTier();
                    old.Remove(_gearAffix);
                    PlayerData.getInstance().RemoveMaterial(affix, 1);
                    break;
                }
            }

            old.Add(new Affix(affix, affix.getValueOfTier(Math.Min(affix.Tier.Length, tier + 1))));
            gear.SetAffixes(old);
            
            onDiplayUptade.Raise();
        }
    }
}