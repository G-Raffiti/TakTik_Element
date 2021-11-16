using System;
using System.Collections;
using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using _Instances;
using Gears;
using Stats;
using TMPro;
using UnityEngine;
using Void = _EventSystem.CustomEvents.Void;

namespace Shops
{
    public class ForgeUI : MonoBehaviour
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

        private void Start()
        {
            UpdateDisplay(new Void());
            onDiplayUptade.EventListeners += UpdateDisplay;
        }

        private void OnDestroy()
        {
            onDiplayUptade.EventListeners -= UpdateDisplay;
        }

        public static AffixSO GetAffix(TMP_Dropdown _dropdown)
        {
            switch (_dropdown.value)
            {
                case 1: return DataBase.Affix.Affixes[EAffix.HP];
                case 2: return DataBase.Affix.Affixes[EAffix.Shield];
                case 3: return DataBase.Affix.Affixes[EAffix.AP];
                case 4: return DataBase.Affix.Affixes[EAffix.MP];
                case 5: return DataBase.Affix.Affixes[EAffix.Zone];
                case 6: return DataBase.Affix.Affixes[EAffix.Range];
                case 7: return DataBase.Affix.Affixes[EAffix.Speed];
                case 8: return DataBase.Affix.Affixes[EAffix.Fire];
                case 9: return DataBase.Affix.Affixes[EAffix.Nature];
                case 10: return DataBase.Affix.Affixes[EAffix.Water];
                case 11: return DataBase.Affix.Affixes[EAffix.Power];
                default: return null;
            }
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

        private void UpdateDisplay(Void empty)
        {
            resourcesMain.text =
                $"<sprite name=HP>{CraftValue(EAffix.HP)}  | <sprite name=Shield>{CraftValue(EAffix.Shield)}\n<sprite name=AP>{CraftValue(EAffix.AP)}  | <sprite name=MP>{CraftValue(EAffix.MP)}";
            resourcesGrid.text =
                $"<sprite name=Zone>{CraftValue(EAffix.Zone)}  | <sprite name=Range>{CraftValue(EAffix.Range)}\n<sprite name=Speed>{CraftValue(EAffix.Speed)}";
            resourcesPower.text =
                $"<sprite name=Fire>{CraftValue(EAffix.Fire)}  | <sprite name=Nature>{CraftValue(EAffix.Nature)}\n<sprite name=Water>{CraftValue(EAffix.Water)}  | <sprite name=Power>{CraftValue(EAffix.Power)}";
        }

        private int CraftValue(EAffix _affix)
        {
            return PlayerData.getInstance().CraftingMaterial[DataBase.Affix.Affixes[_affix]];
        }

        public void UpgradeItemButton()
        {
            onUpgradeItem.Raise(UpgradeItemSlot.GetInfoGear().Gear);
        }

        public void DestroyItemButton()
        {
            Validate.SetActive(true);
        }

        public IEnumerator ValidateDestroy()
        {
            Validate.SetActive(false);
            onDestroyItem.Raise(DestroyItemSlot.GetInfoGear().Gear);
            yield return new WaitForSeconds(0.1f);
            DestroyItemSlot.RemoveItem();
        }
    }
}