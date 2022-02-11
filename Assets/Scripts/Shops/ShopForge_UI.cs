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
using Void = _EventSystem.CustomEvents.Void;

namespace Shops
{
    public class ShopForge_UI : MonoBehaviour
    {
        [SerializeField] private GameObject prefabGear;

        [SerializeField] private TextMeshProUGUI CraftPoints;
        [SerializeField] private TMP_Dropdown dropdownValueNew0;
        [SerializeField] private TMP_Dropdown dropdownValueNew1;
        [SerializeField] private TMP_Dropdown dropdownValueNew2;
        [SerializeField] private TMP_Dropdown dropdownAffixNew0;
        [SerializeField] private TMP_Dropdown dropdownAffixNew1;
        [SerializeField] private TMP_Dropdown dropdownAffixNew2;
        [SerializeField] private TMP_Dropdown dropdownAffixUpgrade;
        [SerializeField] private SlotDragAndDrop NewItemSlot;
        [SerializeField] private SlotDragAndDrop UpgradeItemSlot;
        [SerializeField] private SlotDragAndDrop DestroyItemSlot;
        [SerializeField] private GameObject Validate;
        
        [SerializeField] private TextMeshProUGUI resourcesMain;
        [SerializeField] private TextMeshProUGUI resourcesGrid;
        [SerializeField] private TextMeshProUGUI resourcesPower;
        
        [Header("Listener")]
        [SerializeField] private VoidEvent onDiplayUptade;
        
        [Header("Sender")] 
        [SerializeField] private GearEvent onUpgradeGear;
        [SerializeField] private GearEvent onDestroyGear;

        private static Dictionary<int, AffixSO> getAffixFromDropDown = new Dictionary<int, AffixSO>();

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
            
            int index = Math.Min(GetAffix(_affix).Tier.Length, PlayerData.getInstance().CraftingMaterial[GetAffix(_affix)]);
            for (int i = 1; i <= index; i++)
            {
                _value.options.Add(new TMP_Dropdown.OptionData($"{i}"));
            }
        }
        
        private void ActualiseDropdown(TMP_Dropdown _dropdown)
        {
            _dropdown.ClearOptions();
            _dropdown.options = new List<TMP_Dropdown.OptionData>();
            _dropdown.options.Add(new TMP_Dropdown.OptionData("<sprite name=Affinity>"));

            getAffixFromDropDown = new Dictionary<int, AffixSO>();
            getAffixFromDropDown.Add(0, null);

            foreach (AffixSO _affix in DataBase.Affix.AllAffixes)
            {
                if (PlayerData.getInstance().CraftingMaterial[_affix] <= 0) continue;
                
                _dropdown.options.Add(new TMP_Dropdown.OptionData($"{_affix.Icon} ({PlayerData.getInstance().CraftingMaterial[_affix]})"));

                getAffixFromDropDown.Add(_dropdown.options.Count - 1, _affix);
            }
        }
        private void OnDestroy()
        {
            onDiplayUptade.EventListeners -= UpdateDisplay;
            dropdownAffixNew0.onValueChanged.RemoveAllListeners();
            dropdownAffixNew1.onValueChanged.RemoveAllListeners();
            dropdownAffixNew2.onValueChanged.RemoveAllListeners();
        }

        public static AffixSO GetAffix(TMP_Dropdown _dropdown)
        {
            return getAffixFromDropDown[_dropdown.value];
        }

        public AffixSO GetUpgradeAffix()
        {
            return GetAffix(dropdownAffixUpgrade);
        }

        public void ShowCraftedGear(Gear _gear)
        {
            GameObject pref = Instantiate(prefabGear, NewItemSlot.transform);
            pref.GetComponent<GearInfo>().Gear = _gear;
            pref.GetComponent<GearInfo>().DisplayIcon();
        }

        public Dictionary<AffixSO, int> GetCraftMaterial()
        {
            Dictionary<AffixSO, int> ret = new Dictionary<AffixSO, int>();
            if (GetAffix(dropdownAffixNew0) != null && dropdownValueNew0.value > 0)
                ret.Add(GetAffix(dropdownAffixNew0), dropdownValueNew0.value);
            if (GetAffix(dropdownAffixNew1) != null && dropdownValueNew1.value > 0)
                ret.Add(GetAffix(dropdownAffixNew1), dropdownValueNew1.value);
            if (GetAffix(dropdownAffixNew2) != null && dropdownValueNew2.value > 0)
                ret.Add(GetAffix(dropdownAffixNew2), dropdownValueNew2.value);

            if (ret.Count == 0)
                return null;
            return ret;
        }

        private int MaterialInInventory(EAffix _affix)
        {
            if(PlayerData.getInstance().CraftingMaterial.ContainsKey(DataBase.Affix.Affixes[_affix]))
                return PlayerData.getInstance().CraftingMaterial[DataBase.Affix.Affixes[_affix]];
            return 0;
        }

        public void UpgradeItemButton()
        {
            onUpgradeGear.Raise(UpgradeItemSlot.GetInfoGear().Gear);
        }

        public void DestroyItemButton()
        {
            Validate.SetActive(true);
        }

        public void OKButton()
        {
            ValidateDestroy();
        }

        public void NOButton()
        {
            Validate.SetActive(false);
        }
        
        public void ValidateDestroy()
        {
            Validate.SetActive(false);
            onDestroyGear.Raise(DestroyItemSlot.GetInfoGear().Gear);
            DestroyItemSlot.RemoveItem();
        }
        
        
        // TODO If you are Adding a Stat do that here to 
        private void UpdateDisplay(Void empty)
        {
            resourcesMain.text =
                $"<sprite name=HP>{MaterialInInventory(EAffix.HP)}  | <sprite name=Shield>{MaterialInInventory(EAffix.Shield)}\n<sprite name=AP>{MaterialInInventory(EAffix.AP)}  | <sprite name=MP>{MaterialInInventory(EAffix.MP)}";
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
            CraftPoints.text = $"Craft Points = {GameObject.FindObjectOfType<ShopForge>().CraftPoint}";
        }
    }
}