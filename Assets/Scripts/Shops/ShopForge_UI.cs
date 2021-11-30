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
        
        [SerializeField] private TMP_Dropdown dropdownValueNew0;
        [SerializeField] private TMP_Dropdown dropdownValueNew1;
        [SerializeField] private TMP_Dropdown dropdownValueNew2;
        [SerializeField] private TMP_Dropdown dropdownAffixNew0;
        [SerializeField] private TMP_Dropdown dropdownAffixNew1;
        [SerializeField] private TMP_Dropdown dropdownAffixNew2;
        [SerializeField] private TMP_Dropdown dropdownAffixUpgrade;
        [SerializeField] private DragAndDropCell NewItemSlot;
        [SerializeField] private DragAndDropCell UpgradeItemSlot;
        [SerializeField] private DragAndDropCell DestroyItemSlot;
        [SerializeField] private GameObject Validate;
        
        [SerializeField] private TextMeshProUGUI resourcesMain;
        [SerializeField] private TextMeshProUGUI resourcesGrid;
        [SerializeField] private TextMeshProUGUI resourcesPower;
        
        [Header("Listener")]
        [SerializeField] private VoidEvent onDiplayUptade;
        
        [Header("Sender")] 
        [SerializeField] private ItemEvent onUpgradeItem;
        [SerializeField] private ItemEvent onDestroyItem;

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
            _value.options = new List<TMP_Dropdown.OptionData>();
            _value.options.Add(new TMP_Dropdown.OptionData("0"));
            if (GetAffix(_affix) == null) return;
            
            int index = Math.Min(_affix.options.Count, GetAffix(_affix).Tier.Length);
            for (int i = 1; i <= index; i++)
            {
                _value.options.Add(new TMP_Dropdown.OptionData($"{i}"));
            }
        }
        
        private void ActualiseDropdown(TMP_Dropdown _dropdown)
        {
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
            pref.GetComponent<InfoGear>().Gear = _gear;
            pref.GetComponent<InfoGear>().DisplayIcon();
        }

        public Dictionary<AffixSO, int> GetCraftStats()
        {
            Dictionary<AffixSO, int> ret = new Dictionary<AffixSO, int>();
            if (GetAffix(dropdownAffixNew0) != null)
                ret.Add(GetAffix(dropdownAffixNew0), dropdownValueNew0.value);
            if (GetAffix(dropdownAffixNew1) != null)
                ret.Add(GetAffix(dropdownAffixNew1), dropdownValueNew1.value);
            if (GetAffix(dropdownAffixNew2) != null)
                ret.Add(GetAffix(dropdownAffixNew2), dropdownValueNew2.value);
            if (ret.Count == 0)
                return null;
            return ret;
        }

        private int CraftValue(EAffix _affix)
        {
            if(PlayerData.getInstance().CraftingMaterial.ContainsKey(DataBase.Affix.Affixes[_affix]))
                return PlayerData.getInstance().CraftingMaterial[DataBase.Affix.Affixes[_affix]];
            return 0;
        }

        public void UpgradeItemButton()
        {
            onUpgradeItem.Raise(UpgradeItemSlot.GetInfoGear().Gear);
        }

        public void DestroyItemButton()
        {
            Validate.SetActive(true);
        }

        public void OKButton()
        {
            StartCoroutine(ValidateDestroy());
        }

        public void NOButton()
        {
            Validate.SetActive(false);
        }
        
        public IEnumerator ValidateDestroy()
        {
            Validate.SetActive(false);
            onDestroyItem.Raise(DestroyItemSlot.GetInfoGear().Gear);
            yield return new WaitForSeconds(0.1f);
            DestroyItemSlot.RemoveItem();
        }
        
        
        // TODO If you are Adding a Stat do tha here to 
        private void UpdateDisplay(Void empty)
        {
            resourcesMain.text =
                $"<sprite name=HP>{CraftValue(EAffix.HP)}  | <sprite name=Shield>{CraftValue(EAffix.Shield)}\n<sprite name=AP>{CraftValue(EAffix.AP)}  | <sprite name=MP>{CraftValue(EAffix.MP)}";
            resourcesGrid.text =
                $"<sprite name=Zone>{CraftValue(EAffix.Zone)}  | <sprite name=Range>{CraftValue(EAffix.Range)}\n<sprite name=Speed>{CraftValue(EAffix.Speed)}  | <sprite name=Focus>{CraftValue(EAffix.Focus)}";
            resourcesPower.text =
                $"<sprite name=Fire>{CraftValue(EAffix.Fire)}  | <sprite name=Nature>{CraftValue(EAffix.Nature)}\n<sprite name=Water>{CraftValue(EAffix.Water)}  | <sprite name=Power>{CraftValue(EAffix.Power)}";
            
            ActualiseDropdown(dropdownAffixNew0);
            ActualiseDropdown(dropdownAffixNew1);
            ActualiseDropdown(dropdownAffixNew2);
            ActualiseDropdown(dropdownAffixUpgrade);
        }
    }
}