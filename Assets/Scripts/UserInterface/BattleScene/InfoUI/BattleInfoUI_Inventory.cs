using _DragAndDropSystem;
using _Extension;
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

            foreach (SlotDragAndDrop slot in inventoryPrefab.GetComponentsInChildren<SlotDragAndDrop>())
            {
                slot.cellType = SlotDragAndDrop.CellType.DropOnly;
            }
        }

        private void OnDisable()
        {
            int i = 0;
            //Array to hold all child obj
            GameObject[] allChildren = new GameObject[transform.childCount - 1];

            //Find all child obj and store to that array
            foreach (Transform child in transform)
            {
                if (child.GetComponentInChildren<PersonalInventory>() == null) continue;
                allChildren[i] = child.gameObject;
                i += 1;
            }

            //Now destroy them
            foreach (GameObject child in allChildren)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}