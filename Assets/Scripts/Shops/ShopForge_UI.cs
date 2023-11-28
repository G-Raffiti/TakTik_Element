using System;
using System.Collections;
using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using _Instances;
using DataBases;
using Gears;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Void = _EventSystem.CustomEvents.Void;

namespace Shops
{
    public class ShopForgeUI : MonoBehaviour
    {
        [SerializeField] private GameObject prefabGear;

        [FormerlySerializedAs("CraftPoints")]
        [SerializeField] private TextMeshProUGUI craftPoints;
        [SerializeField] private TMP_Dropdown dropdownValueNew0;
        [SerializeField] private TMP_Dropdown dropdownValueNew1;
        [SerializeField] private TMP_Dropdown dropdownValueNew2;
        [SerializeField] private TMP_Dropdown dropdownAffixNew0;
        [SerializeField] private TMP_Dropdown dropdownAffixNew1;
        [SerializeField] private TMP_Dropdown dropdownAffixNew2;
        [SerializeField] private TMP_Dropdown dropdownAffixUpgrade;
        [FormerlySerializedAs("NewItemSlot")]
        [SerializeField] private SlotDragAndDrop newItemSlot;
        [FormerlySerializedAs("UpgradeItemSlot")]
        [SerializeField] private SlotDragAndDrop upgradeItemSlot;
        [FormerlySerializedAs("DestroyItemSlot")]
        [SerializeField] private SlotDragAndDrop destroyItemSlot;
        [FormerlySerializedAs("Validate")]
        [SerializeField] private GameObject validate;
        
        [SerializeField] private TextMeshProUGUI resourcesMain;
        [SerializeField] private TextMeshProUGUI resourcesGrid;
        [SerializeField] private TextMeshProUGUI resourcesPower;
        
        [Header("Listener")]
        [SerializeField] private VoidEvent onDiplayUptade;
        
        [Header("Sender")] 
        [SerializeField] private GearEvent onUpgradeGear;
        [SerializeField] private GearEvent onDestroyGear;

        private static Dictionary<int, AffixSo> _getAffixFromDropDown = new Dictionary<int, AffixSo>();

        private void Start()
        {
            onDiplayUptade.EventListeners += UpdateDisplay;
            dropdownAffixNew0.onValueChanged.AddListener(delegate
            {
                ActualiseValueDropdown(dropdownValueNew0, dropdownAffixNew0);
            });
            dropdownAffixNew1.onValueChanged.AddListener(delegate
            {
                ActualiseValueDropdown(dropdownValueNew1, dropdownAffixNew1);
            });
            dropdownAffixNew2.onValueChanged.AddListener(delegate
            {
                ActualiseValueDropdown(dropdownValueNew2, dropdownAffixNew2);
            });
            
            
            UpdateDisplay(new Void());
            
            
        }

        private void ActualiseValueDropdown(TMP_Dropdown _value, TMP_Dropdown _affix)
        {
            _value.ClearOptions();
            _value.options = new List<TMP_Dropdown.OptionData>();
            _value.options.Add(new TMP_Dropdown.OptionData($"0"));
            if (GetAffix(_affix) == null) return;
            
            int _index = Math.Min(GetAffix(_affix).Tier.Length, PlayerData.GetInstance().CraftingMaterial[GetAffix(_affix)]);
            for (int _i = 1; _i <= _index; _i++)
            {
                _value.options.Add(new TMP_Dropdown.OptionData($"{_i}"));
            }
        }
        
        private void ActualiseDropdown(TMP_Dropdown _dropdown)
        {
            _dropdown.ClearOptions();
            _dropdown.options = new List<TMP_Dropdown.OptionData>();
            _dropdown.options.Add(new TMP_Dropdown.OptionData("<sprite name=Affinity>"));

            _getAffixFromDropDown = new Dictionary<int, AffixSo>();
            _getAffixFromDropDown.Add(0, null);

            foreach (AffixSo _affix in DataBase.Affix.AllAffixes)
            {
                if (PlayerData.GetInstance().CraftingMaterial[_affix] <= 0) continue;
                
                _dropdown.options.Add(new TMP_Dropdown.OptionData($"{_affix.Icon} ({PlayerData.GetInstance().CraftingMaterial[_affix]})"));

                _getAffixFromDropDown.Add(_dropdown.options.Count - 1, _affix);
            }
        }
        private void OnDestroy()
        {
            onDiplayUptade.EventListeners -= UpdateDisplay;
            dropdownAffixNew0.onValueChanged.RemoveAllListeners();
            dropdownAffixNew1.onValueChanged.RemoveAllListeners();
            dropdownAffixNew2.onValueChanged.RemoveAllListeners();
        }

        public static AffixSo GetAffix(TMP_Dropdown _dropdown)
        {
            return _getAffixFromDropDown[_dropdown.value];
        }

        public AffixSo GetUpgradeAffix()
        {
            return GetAffix(dropdownAffixUpgrade);
        }

        public void ShowCraftedGear(Gear _gear)
        {
            GameObject _pref = Instantiate(prefabGear, newItemSlot.transform);
            _pref.GetComponent<GearInfo>().Gear = _gear;
            _pref.GetComponent<GearInfo>().DisplayIcon();
        }

        public Dictionary<AffixSo, int> GetCraftMaterial()
        {
            Dictionary<AffixSo, int> _ret = new Dictionary<AffixSo, int>();
            if (GetAffix(dropdownAffixNew0) != null && dropdownValueNew0.value > 0)
                _ret.Add(GetAffix(dropdownAffixNew0), dropdownValueNew0.value);
            if (GetAffix(dropdownAffixNew1) != null && dropdownValueNew1.value > 0)
                _ret.Add(GetAffix(dropdownAffixNew1), dropdownValueNew1.value);
            if (GetAffix(dropdownAffixNew2) != null && dropdownValueNew2.value > 0)
                _ret.Add(GetAffix(dropdownAffixNew2), dropdownValueNew2.value);

            if (_ret.Count == 0)
                return null;
            return _ret;
        }

        private int MaterialInInventory(EAffix _affix)
        {
            if(PlayerData.GetInstance().CraftingMaterial.ContainsKey(DataBase.Affix.Affixes[_affix]))
                return PlayerData.GetInstance().CraftingMaterial[DataBase.Affix.Affixes[_affix]];
            return 0;
        }

        public void UpgradeItemButton()
        {
            onUpgradeGear.Raise(upgradeItemSlot.GetInfoGear().Gear);
        }

        public void DestroyItemButton()
        {
            validate.SetActive(true);
        }

        public void OkButton()
        {
            ValidateDestroy();
        }

        public void NoButton()
        {
            validate.SetActive(false);
        }
        
        public void ValidateDestroy()
        {
            validate.SetActive(false);
            onDestroyGear.Raise(destroyItemSlot.GetInfoGear().Gear);
            destroyItemSlot.RemoveItem();
        }
        
        
        // TODO If you are Adding a Stat do that here to 
        private void UpdateDisplay(Void _empty)
        {
            resourcesMain.text =
                $"<sprite name=HP>{MaterialInInventory(EAffix.Hp)}  | <sprite name=Shield>{MaterialInInventory(EAffix.Shield)}\n<sprite name=AP>{MaterialInInventory(EAffix.AP)}  | <sprite name=MP>{MaterialInInventory(EAffix.Mp)}";
            resourcesGrid.text =
                $"<sprite name=Zone>{MaterialInInventory(EAffix.Zone)}  | <sprite name=Range>{MaterialInInventory(EAffix.Range)}\n<sprite name=Speed>{MaterialInInventory(EAffix.Speed)}  | <sprite name=Focus>{MaterialInInventory(EAffix.Focus)}";
            resourcesPower.text =
                $"<sprite name=Fire>{MaterialInInventory(EAffix.Fire)}  | <sprite name=Nature>{MaterialInInventory(EAffix.Nature)}\n<sprite name=Water>{MaterialInInventory(EAffix.Water)}  | <sprite name=Power>{MaterialInInventory(EAffix.Power)}";
            
            ActualiseDropdown(dropdownAffixNew0);
            ActualiseDropdown(dropdownAffixNew1);
            ActualiseDropdown(dropdownAffixNew2);
            ActualiseDropdown(dropdownAffixUpgrade);
            ActualiseValueDropdown(dropdownValueNew0, dropdownAffixNew0);
            ActualiseValueDropdown(dropdownValueNew1, dropdownAffixNew1);
            ActualiseValueDropdown(dropdownValueNew2, dropdownAffixNew2);
            craftPoints.text = $"Craft Points = {GameObject.FindObjectOfType<ShopForge>().CraftPoint}";
        }
    }
}