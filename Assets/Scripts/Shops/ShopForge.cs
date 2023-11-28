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
        [SerializeField] private GearEvent onDestroyGear;
        [SerializeField] private VoidEvent onCraftGear;
        [SerializeField] private GearEvent onUpgradeGear;
        
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
            foreach (KeyValuePair<AffixSo,int> _pair in MaterialFromGear(_gear))
            {
                PlayerData.GetInstance().AddMaterial(_pair.Key, _pair.Value);
            }
            onDiplayUptade.Raise();
        }

        public Dictionary<AffixSo, int> MaterialFromGear(Gear _gear)
        {
            Dictionary<AffixSo, int> _ret = new Dictionary<AffixSo, int>();
            foreach (Affix _affix in _gear.Affixes)
            { 
                _ret.Add(_affix.affix, _affix.tier);
            }
            return _ret;
        }

        public void CraftNewItem(Void _empty)
        {
            if (craftPoint < 1) return;
            craftPoint--;
            ShopForgeUI _shopForgeUI = GameObject.Find("ForgeUI/Left/Main").GetComponent<ShopForgeUI>();
            if (_shopForgeUI.GetCraftMaterial() == null)
                return;
            Dictionary<AffixSo, int> _material = _shopForgeUI.GetCraftMaterial();
            foreach (KeyValuePair<AffixSo,int> _pair in _shopForgeUI.GetCraftMaterial())
            {
                if (_pair.Value < 1)
                    _material[_pair.Key] = 1;
                if (!PlayerData.GetInstance().CraftingMaterial.ContainsKey(_pair.Key))
                    return;
                if (PlayerData.GetInstance().CraftingMaterial[_pair.Key] < _material[_pair.Key])
                    return;
            }

            foreach (AffixSo _affixSo in _material.Keys)
            {
                PlayerData.GetInstance().RemoveMaterial(_affixSo, _material[_affixSo]);
            }
            
            Gear _gear = new Gear();
            _gear.CraftNewGear(_material);
            
            _shopForgeUI.ShowCraftedGear(_gear);
            onDiplayUptade.Raise();
        }


        private void UpgradeItem(Gear _gear)
        {
            if (craftPoint < 1) return;
            ShopForgeUI _shopForgeUI = GameObject.Find("ForgeUI/BackGround/Back/Left/Main").GetComponent<ShopForgeUI>();
            if (!UpgradeGear(_gear, _shopForgeUI.GetUpgradeAffix()))
                return;
            
            craftPoint--;
            PlayerData.GetInstance().RemoveMaterial(_shopForgeUI.GetUpgradeAffix(), 1);
            
            onDiplayUptade.Raise();
        }

        private bool UpgradeGear(Gear _gear, AffixSo _affix)
        {
            if (!PlayerData.GetInstance().CraftingMaterial.ContainsKey(_affix)) 
                return false;
            if (PlayerData.GetInstance().CraftingMaterial[_affix] < 1)
                return false;
            
            List<Affix> _old = _gear.Affixes;
            
            int _tier = 0;
            
            foreach (Affix _gearAffix in _gear.Affixes.Where(_gearAffix => _gearAffix.affix == _affix))
            {
                _tier = _gearAffix.tier;
                if (_tier == _affix.Tier.Length - 1)
                    return false;
                _old.Remove(_gearAffix);
                break;
            }

            _tier += 1;

            _old.Add(new Affix(_affix, _affix.GetValueOfTier(_tier), _tier));
            _gear.SetAffixes(_old);
            return true;
        }
    }
}