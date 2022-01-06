using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private int craftPoint = 3;
        [Header("Listener")]
        [SerializeField] private ItemEvent onDestroyGear;
        [SerializeField] private VoidEvent onCraftGear;
        [SerializeField] private ItemEvent onUpgradeGear;
        
        [Header("Sender")]
        [SerializeField] private VoidEvent onDiplayUptade;

        public int CraftPoint => craftPoint;

        private void Start()
        {
            craftPoint = 3;
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
                ret.Add(affix.affix, affix.tier);
            }
            return ret;
        }

        public void CraftNewItem(Void empty)
        {
            if (craftPoint < 1) return;
            craftPoint--;
            ShopForge_UI _shopForgeUI = GameObject.Find("ForgeUI/Left/Main").GetComponent<ShopForge_UI>();
            if (_shopForgeUI.GetCraftMaterial() == null)
                return;
            Dictionary<AffixSO, int> material = _shopForgeUI.GetCraftMaterial();
            foreach (KeyValuePair<AffixSO,int> _pair in _shopForgeUI.GetCraftMaterial())
            {
                if (_pair.Value < 1)
                    material[_pair.Key] = 1;
                if (!PlayerData.getInstance().CraftingMaterial.ContainsKey(_pair.Key))
                    return;
                if (PlayerData.getInstance().CraftingMaterial[_pair.Key] < material[_pair.Key])
                    return;
            }

            foreach (AffixSO _affixSO in material.Keys)
            {
                PlayerData.getInstance().RemoveMaterial(_affixSO, material[_affixSO]);
            }
            
            Gear gear = new Gear();
            gear.CraftNewGear(material);
            
            _shopForgeUI.ShowCraftedGear(gear);
            onDiplayUptade.Raise();
        }


        private void UpgradeItem(Gear gear)
        {
            if (craftPoint < 1) return;
            ShopForge_UI _shopForgeUI = GameObject.Find("ForgeUI/Left/Main").GetComponent<ShopForge_UI>();
            if (!UpgradeGear(gear, _shopForgeUI.GetUpgradeAffix()))
                return;
            
            craftPoint--;
            PlayerData.getInstance().RemoveMaterial(_shopForgeUI.GetUpgradeAffix(), 1);
            
            onDiplayUptade.Raise();
        }

        private bool UpgradeGear(Gear gear, AffixSO affix)
        {
            if (!PlayerData.getInstance().CraftingMaterial.ContainsKey(affix)) 
                return false;
            if (PlayerData.getInstance().CraftingMaterial[affix] < 1)
                return false;
            
            List<Affix> old = gear.Affixes;
            
            int tier = 0;
            
            foreach (Affix _gearAffix in gear.Affixes.Where(_gearAffix => _gearAffix.affix == affix))
            {
                tier = _gearAffix.tier;
                if (tier == affix.Tier.Length - 1)
                    return false;
                old.Remove(_gearAffix);
                break;
            }

            tier += 1;

            old.Add(new Affix(affix, affix.getValueOfTier(tier), tier));
            gear.SetAffixes(old);
            return true;
        }
    }
}