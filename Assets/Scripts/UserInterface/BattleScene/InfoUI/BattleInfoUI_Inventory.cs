using _DragAndDropSystem;
using _Instances;
using Units;
using UnityEngine;

namespace UserInterface.BattleScene.InfoUI
{
    public class BattleInfoUI_Inventory : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryPrefab;

        private void OnEnable()
        {
            foreach (Hero _hero in PlayerData.getInstance().Heroes)
            {
                GameObject inventoryObj = Instantiate(inventoryPrefab, transform);
                inventoryObj.GetComponentInChildren<PersonalInventory>().Initialize(_hero);
                inventoryObj.GetComponentInChildren<PersonalInventory>().FillInventory();
            }

            foreach (DragAndDropCell slot in inventoryPrefab.GetComponentsInChildren<DragAndDropCell>())
            {
                slot.cellType = DragAndDropCell.CellType.DropOnly;
            }
        }

        private void OnDisable()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
    }
}